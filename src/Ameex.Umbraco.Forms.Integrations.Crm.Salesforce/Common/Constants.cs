using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common
{
    public class Constants
    {
        public const string ApiVersion = "55.0";
        public const string GetAccessTokenEndpoint = "https://login.salesforce.com/services/oauth2/token";
        public const string GetObjectPropertiesEndpoit = "{0}/services/data/v{1}/sobjects/{2}/describe";
        public const string PostObjectDataEndpoit = "{0}/services/data/v{1}/sobjects/{2}";
        public const string GetObjectsListEndpoit = "{0}/services/data/v{1}/sobjects/";
        public const string DefaultSalesforceObject = "Lead";

        public const string AuthenticationSuccessMessage = "User authentication is success";
        public const string AutheticationFailedMessage = "User authentication is failed. Error: {0}";
        public const string GetSalesforceObjectFailed = "There is a problem retrieving the salesforce object properties";
        public const string SalesforceHostKeyIsNotSet = "Salesfroce host name is not configured";
        public const string UmbracoFormsSalesforceIntegrationCtrlBaseUrlKey = "umbracoFormsIntegrationsCrmSalesforceBaseUrl";
        public const string ApiControllerDefaultActionName = "Default";

        public const string SalesforceUsername = "Salesforce:Username";
        public const string SalesforcePassword = "Salesforce:Password";
        public const string SalesforceClientId = "Salesforce:ClientId";
        public const string SalesforceClientSecret = "Salesforce:ClientSecret";
        public const string SalesforceToken = "Salesforce:Token";
        public const string SalesforceApiVersion = "Salesforce:ApiVersion";
        public const string SalesforceHost = "Salesforce:Host";
        public const string SalesforceAccountSettingsConfigKey = "Salesforce:AccountSettings";
        public const string SalesforceObjectListConfigKey = "Salesforce:ObjectList";
        public const string SalesforceAccessTokenCacheKey = "SalesforceAccessToken";

        public const string SalesforceRequestFailed = "There is a problem with salesforce object data";
    }
    
    public enum ApiOperations
    {
        Authenticate,
        Update,
        GetObjects,
        GetProperties
    }

    public enum HttpContentType
    {
        JsonContent,
        MultipartContent,
        None
    }
    public enum GrantType
    {
        Password
    }
}
