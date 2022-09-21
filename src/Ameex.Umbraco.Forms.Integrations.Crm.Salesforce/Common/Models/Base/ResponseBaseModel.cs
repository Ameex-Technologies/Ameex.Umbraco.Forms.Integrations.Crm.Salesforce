using System.Net;
using Newtonsoft.Json;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models.Base
{
    public abstract class ResponseBaseModel
    {
        [JsonProperty("httpStatusCode")]
        public HttpStatusCode HttpStatusCode { get; set; }
        [JsonProperty("status")]
        public ApiStatus Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public enum ApiStatus
    {
        Success,
        Failed
    }
}
