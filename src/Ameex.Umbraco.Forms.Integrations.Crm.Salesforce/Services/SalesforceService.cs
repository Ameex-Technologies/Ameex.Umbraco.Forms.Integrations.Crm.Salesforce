using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Extension;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models.Base;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Logging.Serilog;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Services
{
    public class SalesforceObjectService : ISalesforceObjectService
    {
        private readonly SerilogLogger _logger;
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Func<HttpClient> ClientFactory = () => httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IKeyValueService _keyValueService;
        private AccountSettings accountSettings;
        private IConfiguration _configuration;
        private bool ForceRemoveCache
        {
            get; set;
        }
        public string AccessToken
        {
            get
            {
                
                return GetAccessTokenFromCahe(Constants.SalesforceAccessTokenCacheKey, ForceRemoveCache).Result;
            }
        }

        public SalesforceObjectService(IMemoryCache memoryCache, IKeyValueService keyValueService, IConfiguration configuration)
        {
            _logger = new SerilogLogger(new Serilog.LoggerConfiguration());
            _memoryCache = memoryCache;
            _keyValueService = keyValueService;
            accountSettings = GetAccountSettings();
            _configuration = configuration;
        }

        public async Task<SalesforceApiResponse> AuthenticateAsync()
        {
            var authenticationResponse = await GetAccessToken();
            if (authenticationResponse.IsNull() || authenticationResponse.AccessToken.IsNull())
            {
                return new SalesforceApiResponse()
                {
                    Data = accountSettings,
                    Status = ApiStatus.Failed,
                    HttpStatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Message = string.Format(Constants.AutheticationFailedMessage, authenticationResponse?.ErrorDescription)
                };
            }

            _keyValueService.SetValue(Constants.SalesforceHost, authenticationResponse.InstanceUrl);
            accountSettings = GetAccountSettings();


            return new SalesforceApiResponse()
            {
                Status = ApiStatus.Success,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Message = Constants.AuthenticationSuccessMessage
            };

        }

        public async Task<SalesforceApiResponse> GetSalesforceObjectAsync(string objectName)
        {
            var requestUri = new Uri(string.Format(Constants.GetObjectPropertiesEndpoit, accountSettings.Host, Constants.ApiVersion, objectName));
            return await GetResponseAsync<SalesforceObject>(requestUri, HttpMethod.Get, HttpContentType.None, null, true);
        }

        public async Task<SalesforceApiResponse> PostSalesforceObjectAsync(SalesforceObjectRequest salesforceObjectRequest, Record record, Form form)
        {

            if (salesforceObjectRequest.IsNull() || salesforceObjectRequest.FieldMappings.IsNull())
                _logger.Error(typeof(SalesforceObjectService), new Exception("There is an issue with salesforce field mapping"));

            var sfObjectProperties = new Dictionary<string, string>();

            foreach (var field in salesforceObjectRequest.FieldMappings)
            {
                var recordField = record.GetRecordField(Guid.Parse(field.FormField));
                if (recordField.IsNull())
                {
                    _logger.Warn(typeof(SalesforceObjectService), $"Form field {field.FormField} is missing in the record");
                    continue;
                }
                var value = recordField.ValuesAsString(false);
                sfObjectProperties.Add(field.ObjectField, value);
            }
            var requestUri = new Uri(string.Format(Constants.PostObjectDataEndpoit, accountSettings.Host, Constants.ApiVersion, salesforceObjectRequest.ObjectName));
            return await GetResponseAsync<SalesforceObject>(requestUri, HttpMethod.Post, HttpContentType.JsonContent, sfObjectProperties, true);
        }

        public SalesforceApiResponse GetSalesforceObjectList()
        {
            var sfObjectList = _configuration.GetSection(Constants.SalesforceObjectListConfigKey).Get<List<string>>();
            if (sfObjectList.IsNullOrEmpty<string>())
            {
                return new SalesforceApiResponse()
                {
                    Data = Constants.DefaultSalesforceObject.Split(","),
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    Status = ApiStatus.Success
                };
            }

            return new SalesforceApiResponse()
            {
                Data = sfObjectList,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Status = ApiStatus.Success
            };

        }

        public SalesforceApiResponse SetAccountSettings(AccountSettings sfAccountSettings)
        {
            if (sfAccountSettings.IsNull())
                return new SalesforceApiResponse()
                {
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Status = ApiStatus.Failed
                };

            SetKeyValue(sfAccountSettings);
            accountSettings = GetAccountSettings();

            return new SalesforceApiResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Status = ApiStatus.Success
            };
        }

        public SalesforceApiResponse DeleteAccountSettings()
        {
            SetKeyValue(new AccountSettings());
            accountSettings = GetAccountSettings();

            return new SalesforceApiResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                Status = ApiStatus.Success
            };
        }

        private void SetKeyValue(AccountSettings sfAccountSettings)
        {
            _keyValueService.SetValue(Constants.SalesforceUsername, sfAccountSettings.Username);
            _keyValueService.SetValue(Constants.SalesforcePassword, sfAccountSettings.Password);
            _keyValueService.SetValue(Constants.SalesforceClientId, sfAccountSettings.ClientId);
            _keyValueService.SetValue(Constants.SalesforceClientSecret, sfAccountSettings.ClientSecret);
        }

        private AccountSettings GetAccountSettings()
        {
            return new AccountSettings()
            {
                Username = _keyValueService.GetValue(Constants.SalesforceUsername),
                Password = _keyValueService.GetValue(Constants.SalesforcePassword),
                ClientId = _keyValueService.GetValue(Constants.SalesforceClientId),
                ClientSecret = _keyValueService.GetValue(Constants.SalesforceClientSecret),
                Host = _keyValueService.GetValue(Constants.SalesforceHost)
            };
        }

        private async Task<SalesforceApiResponse> GetResponseAsync<T>(Uri requestUri, HttpMethod httpMethod, HttpContentType httpContentType, object content, bool addAuthTokenHeader) where T : class
        {
            var requestMessage = GetHttpRequestMessage(httpMethod, requestUri, content, httpContentType, addAuthTokenHeader);
            var response = await SendAsync(requestMessage);

            return await GetSalesforceApiResponse<T>(response, requestUri, httpMethod, httpContentType, content, addAuthTokenHeader);
        }

        private async Task<SalesforceApiResponse> GetSalesforceApiResponse<T>(HttpResponseMessage httpResponseMessage, Uri requestUri, HttpMethod httpMethod, HttpContentType httpContentType, object content, bool addAuthTokenHeader) where T : class
        {
            if (httpResponseMessage.IsNull() || httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                _logger.Warn(typeof(SalesforceObjectService), string.Format(Constants.GetSalesforceObjectFailed));
                return new SalesforceApiResponse()
                {
                    Status = ApiStatus.Failed,
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = Constants.SalesforceRequestFailed
                };
            }

            if (httpResponseMessage.IsSuccessStatusCode == false && httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ForceRemoveCache = true;
                return await GetResponseAsync<T>(requestUri, httpMethod, httpContentType, content, addAuthTokenHeader);
            }

            var sfObjectString = await httpResponseMessage.Content.ReadAsStringAsync();
            var sfObject = sfObjectString.ToObject<T>();

            return new SalesforceApiResponse()
            {
                Data = sfObject,
                HttpStatusCode = httpResponseMessage.StatusCode,
                Status = ApiStatus.Success
            };
        }

        private async Task<string> GetAccessTokenFromCahe(string cacheKey, bool forceRemove)
        {
            if (string.IsNullOrEmpty(cacheKey))
                return string.Empty;

            if (forceRemove)
            {
                _memoryCache.Remove(cacheKey);
                ForceRemoveCache = false;
            }

            var accessToken = _memoryCache.Get(cacheKey) as string;
            if (!string.IsNullOrEmpty(accessToken)) 
                return accessToken;

            var authenticationResponse = await GetAccessToken();
            if (authenticationResponse == null || authenticationResponse.AccessToken.IsNull())
            {
                _logger.Warn(typeof(SalesforceObjectService), string.Format(Constants.AutheticationFailedMessage, authenticationResponse.ErrorDescription));
                return String.Empty;
            }

            var memoryCacheOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            };

            _memoryCache.Set(cacheKey, authenticationResponse.AccessToken, memoryCacheOptions);

            return authenticationResponse.AccessToken;

        }

        private async Task<AuthenticationResponse> GetAccessToken()
        {
            var authenticationRequest = new AuthenticationRequest()
            {
                Grant_type = GrantType.Password.ToString().ToLower(),
                Client_id = accountSettings.ClientId,
                Client_secret = accountSettings.ClientSecret,
                Username = accountSettings.Username,
                Password = $"{accountSettings.Password}{accountSettings.Token}"
            };

            var requestUri = new Uri(Constants.GetAccessTokenEndpoint);
            var httpRequestMessage = GetHttpRequestMessage(HttpMethod.Post, requestUri, authenticationRequest, HttpContentType.MultipartContent, false);
            var response = await SendAsync(httpRequestMessage);

            if (response == null)
                return null;

            var tokenResponseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = tokenResponseString.ToObject<AuthenticationResponse>();
            return tokenResponse;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            try
            {
                return  await ClientFactory().SendAsync(httpRequestMessage);
            }
            catch(Exception ex)
            {
                _logger.Error(typeof(SalesforceObjectService), Constants.SalesforceRequestFailed, ex);
                return null;
            }
           
        }

        private HttpRequestMessage GetHttpRequestMessage(HttpMethod httpMethod, Uri requestUri, object content, HttpContentType httpContentType, bool addAuthorizationToken )
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = httpMethod,
                RequestUri = requestUri,
                Content = content != null ? CreateHttpContent(httpContentType, content) : null
            };

            if(addAuthorizationToken)
                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            return httpRequestMessage;
        }

        private HttpContent CreateHttpContent(HttpContentType httpContentType, object content)
        {
            if (content == null)
                return null;

            switch (httpContentType)
            {
                case HttpContentType.MultipartContent:
                    return new FormUrlEncodedContent(content.AsDictionary(true));
                case HttpContentType.JsonContent:
                    return new StringContent(content.ToJsonString(), Encoding.UTF8, "application/json");
            }

            throw new InvalidOperationException($" Unexpected Http content type {httpContentType.ToString()}");

        }
    }
}
