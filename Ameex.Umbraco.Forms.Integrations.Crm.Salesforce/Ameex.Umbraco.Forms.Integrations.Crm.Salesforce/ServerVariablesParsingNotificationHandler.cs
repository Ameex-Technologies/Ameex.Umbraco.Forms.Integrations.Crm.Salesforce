using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce
{
    public class ServerVariablesParsingNotificationHandler : INotificationAsyncHandler<ServerVariablesParsingNotification>
    {
        private readonly LinkGenerator _linkGenerator;
        public ServerVariablesParsingNotificationHandler(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        public Task HandleAsync(ServerVariablesParsingNotification notification, CancellationToken cancellationToken)
        {
            var salesforceControllerBaseUrl = _linkGenerator.GetUmbracoApiServiceBaseUrl<SalesforceController>(controller => controller.Authenticate());
            notification.ServerVariables.Add(Common.Constants.UmbracoFormsSalesforceIntegrationCtrlBaseUrlKey, salesforceControllerBaseUrl);
            return Task.CompletedTask;
        }
    }
}
