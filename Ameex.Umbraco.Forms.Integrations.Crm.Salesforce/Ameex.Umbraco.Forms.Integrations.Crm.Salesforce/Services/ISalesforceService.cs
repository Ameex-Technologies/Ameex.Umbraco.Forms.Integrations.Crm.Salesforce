using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Services
{
    public interface ISalesforceObjectService
    {
        SalesforceApiResponse SetAccountSettings(AccountSettings accountSettings);
        SalesforceApiResponse DeleteAccountSettings();
        Task<SalesforceApiResponse> AuthenticateAsync();
        Task<SalesforceApiResponse> GetSalesforceObjectAsync(string objectName);
        SalesforceApiResponse GetSalesforceObjectList();
        Task<SalesforceApiResponse> PostSalesforceObjectAsync(SalesforceObjectRequest salesforceObjectRequest, Record record, Form form);
    }
}
