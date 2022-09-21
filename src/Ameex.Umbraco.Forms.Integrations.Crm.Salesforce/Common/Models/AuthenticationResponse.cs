using Newtonsoft.Json;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class AuthenticationResponse
    {
        [JsonProperty("access_token", NullValueHandling= NullValueHandling.Ignore)]
        public string AccessToken { get; set; }
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; set; }
        [JsonProperty("error_description", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorDescription { get; set; }
    }
}
