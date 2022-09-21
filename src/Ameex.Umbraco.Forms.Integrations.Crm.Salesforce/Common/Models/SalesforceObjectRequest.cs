using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class SalesforceObjectRequest
    {
        [JsonProperty("objectName")]
        public string ObjectName { get; set; }
        [JsonProperty("fields")]
        public IList<FieldMapping>  FieldMappings { get; set; }
    }
    public class FieldMapping
    {
        [JsonProperty("formField")]
        public string FormField { get; set; }
        [JsonProperty("objectField")]
        public string ObjectField { get; set; }
    }
}
