using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class SalesforceObjectList
    {
        [JsonProperty("sobjects")]
        public List<SalesforceObject> SalesforceObjects { get; set; }
    }
}
