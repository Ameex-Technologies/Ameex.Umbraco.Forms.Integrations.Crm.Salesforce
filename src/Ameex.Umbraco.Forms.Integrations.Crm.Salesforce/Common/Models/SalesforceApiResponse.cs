using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class SalesforceApiResponse : ResponseBaseModel
    {
        [JsonProperty("data")]
        public object Data { get; set; }
    }
    public class SalesforceObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("fields")]
        public List<FieldProperty> Fields { get; set; }

    }

    public class FieldProperty
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("defaultType")]
        public string DefaultValue { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
