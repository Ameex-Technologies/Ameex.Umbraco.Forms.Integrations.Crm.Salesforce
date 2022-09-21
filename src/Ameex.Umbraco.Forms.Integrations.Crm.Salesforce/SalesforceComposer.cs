using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Services;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Providers;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce
{
    public class SalesforceComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<ISalesforceObjectService, SalesforceObjectService>(ServiceLifetime.Singleton);
            builder.WithCollectionBuilder<WorkflowCollectionBuilder>().Add<SalesforceWorkflow>();
            builder.AddNotificationAsyncHandler<ServerVariablesParsingNotification, ServerVariablesParsingNotificationHandler>();
        }
    }
}
