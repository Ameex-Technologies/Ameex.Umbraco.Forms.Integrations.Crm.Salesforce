using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Extension;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Common.Models.Base;
using Ameex.Umbraco.Forms.Integrations.Crm.Salesforce.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Umbraco.Cms.Core.Logging.Serilog;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Ameex.Umbraco.Forms.Integrations.Crm.Salesforce
{
    public class SalesforceWorkflow : WorkflowType
    {
        private readonly ISalesforceObjectService _salesforceObjectService;
        private readonly SerilogLogger _logger;
        public SalesforceWorkflow(ISalesforceObjectService salesforceObjectService)
        {
            _salesforceObjectService = salesforceObjectService;
            _logger = new SerilogLogger(new Serilog.LoggerConfiguration());

            Id = new Guid("5ab03ce6-ed21-4c82-b2e6-3787e7ea8398");
            Name = "Send form to Salesforce CRM";
            Description = "Form submissions are sent to Salesforce CRM";
            Icon = "icon-cloud-upload color-blue";
            Group = "CRM";
        }

        [Setting("Field Mappings", Description = "Map Umbraco form fields to Salesforce object properties", View = "~/App_Plugins/Ameex.Umbraco.Forms.Integrations/Crm/Salesforce/salesforce.html")]
        public string FieldMapping { get; set; }
        public override WorkflowExecutionStatus Execute(WorkflowExecutionContext context)
        {
            _logger.Debug(typeof(SalesforceWorkflow), $"Salesforce workflow execution is started.");

            var jsonString = new JsonTextReader(new StringReader(FieldMapping));
            var salesforceObjectRequest = JsonSerializer.Create().Deserialize<SalesforceObjectRequest>(jsonString);

            var postObjectResponse = _salesforceObjectService.PostSalesforceObjectAsync(salesforceObjectRequest, context.Record, context.Form).Result;

            if (postObjectResponse.IsNull() || postObjectResponse.Status == ApiStatus.Failed)
            {
                _logger.Warn(typeof(SalesforceWorkflow), $"Workflow {Workflow.Name}: Failed for {context.Form.Name} ({context.Form.Id}).");
                return WorkflowExecutionStatus.Failed;
            }

            _logger.Debug(typeof(SalesforceWorkflow), $"Salesforce workflow execution is completed.");
            return WorkflowExecutionStatus.Completed;
        }

        public override List<Exception> ValidateSettings() => new List<Exception>();

    }
}
