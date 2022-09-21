function salesforceResource($http, umbRequestHelper) {

    return {   
        authenticate: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(
                    Umbraco.Sys.ServerVariables.umbracoFormsIntegrationsCrmSalesforceBaseUrl +  "authenticate"),
                'Failed to get Salesforce authentication status');
        },
        setAccountSettings: function (accountSettings) {
            return umbRequestHelper.resourcePromise(
                $http.post(
                    Umbraco.Sys.ServerVariables.umbracoFormsIntegrationsCrmSalesforceBaseUrl + "setaccount",
                    accountSettings   ),
                'Failed to set Salesforce account settings');
        },
        deleteAccountSettings: function (accountSettings) {
            return umbRequestHelper.resourcePromise(
                $http.post(
                    Umbraco.Sys.ServerVariables.umbracoFormsIntegrationsCrmSalesforceBaseUrl + "deleteaccount"),
                'Failed to delete Salesforce account settings');
        },
        getAllProperties: function (objectName) {
            return umbRequestHelper.resourcePromise(
                $http.get(
                    Umbraco.Sys.ServerVariables.umbracoFormsIntegrationsCrmSalesforceBaseUrl + "getobjectproperties/?objectName=" + objectName),
                    'Failed to get Salesforce Properties');
        },
        getObjectList: function () {
            return umbRequestHelper.resourcePromise(
                $http.get(
                    Umbraco.Sys.ServerVariables.umbracoFormsIntegrationsCrmSalesforceBaseUrl +   "getobjectlist"),
                'Failed to get Salesforce Object List');
        }
    };
}

angular.module('umbraco.resources').factory('umbracoFormsIntegrationsCrmSalesforceResource', salesforceResource);