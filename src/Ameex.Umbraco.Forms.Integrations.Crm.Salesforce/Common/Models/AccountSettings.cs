using Newtonsoft.Json;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class AccountSettings
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }
        [JsonProperty("token")]
        public string Token { get;set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

    }
}
