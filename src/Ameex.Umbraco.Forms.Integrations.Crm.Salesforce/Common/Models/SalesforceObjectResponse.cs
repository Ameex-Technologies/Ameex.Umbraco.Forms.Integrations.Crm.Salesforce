using System.Collections.Generic;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models
{
    public class SalesforceObjectResponse
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public IList<string > Errors { get; set; }
    }
}
