using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Controllers
{
    [PluginController("UmbracoFormsIntegrationsCrmSalesforce")]

    public class SalesforceController : UmbracoAuthorizedApiController
    {
        private readonly ISalesforceObjectService _salesforceObjectService;

        public SalesforceController(ISalesforceObjectService salesforceObjectService)
        {
            _salesforceObjectService = salesforceObjectService;
        }

        [HttpGet]
        public async Task<SalesforceApiResponse> Authenticate() => await _salesforceObjectService.AuthenticateAsync();

        [HttpGet]
        public async Task<SalesforceApiResponse> GetObjectProperties(string objectName) => await _salesforceObjectService.GetSalesforceObjectAsync(objectName);

        [HttpGet]
        public SalesforceApiResponse GetObjectList() =>  _salesforceObjectService.GetSalesforceObjectList();

        [HttpPost]
        public SalesforceApiResponse SetAccount([FromBody] AccountSettings accountSettings) => _salesforceObjectService.SetAccountSettings(accountSettings);
        
        [HttpPost]
        public SalesforceApiResponse DeleteAccount() => _salesforceObjectService.DeleteAccountSettings();
    }
}
