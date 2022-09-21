angular
    .module("umbraco")
    .component("umbFormsIntegrationsCrmSalesforceFields", {
        controller: SalesforceFieldsController,
        controllerAs: "vm",
        templateUrl: "/App_Plugins/Ameex.Umbraco.Forms.Integrations/Crm/Salesforce/salesforce-field-mapper-template.html",
        bindings: {
            setting: "<"
        },
    }
    );

function SalesforceFieldsController($routeParams, umbracoFormsIntegrationsCrmSalesforceResource, pickerResource, overlayService, notificationsService) {
    var vm = this;
    vm.loading = true;
    vm.authorizationStatus = "Unauthenticated";
    vm.authorizationStatusMessage = "";
    vm.objectList = [];
    vm.selectedSfObject = "";
    vm.accountSettings = {};
    vm.showSfObjects = false;
    vm.showMappingBtn = false;

    vm.getFieldsForMapping = function (objectName, resetMapping) {
        vm.loading = true;
        vm.showMappingBtn = false;

        if (resetMapping)
            vm.mapping.fields = [];

        // Get the fields for the form.
        var formId = $routeParams.id;
        if (formId !== -1) {
            pickerResource.getAllFields(formId).then(function (response) {
                vm.fields = response.data;
            });
        }

        vm.mapping.objectName = objectName;

        // Get the Salesforce object fields.
        umbracoFormsIntegrationsCrmSalesforceResource.getAllProperties(objectName).then(function (response) {
            if (response.httpStatusCode == 'OK') {
                var salesforceObjectProps = response.data.fields.map(x => {
                    return {
                        value: x.name,
                        name: x.label
                    }
                });
                vm.sfObjectFields = salesforceObjectProps;
                vm.loading = false;
                vm.showMappingBtn = true;
            }
        });

    }

    getObjectList = function () {
        vm.loading = true;
        umbracoFormsIntegrationsCrmSalesforceResource.getObjectList().then(function (response) {
            if (response.httpStatusCode == 'OK') {
                vm.objectList = response.data.map(x => {
                    return {
                        label: x,
                        name: x
                    }
                });
                vm.showSfObjects = true;
            }
            if (vm.selectedSfObject !== '') {
                vm.getFieldsForMapping(vm.selectedSfObject, false);
                vm.showMappingBtn = true;
            }
            vm.loading = false;
        });
    }

    vm.$onInit = function () {
        if (!vm.setting.value) {
            vm.mapping = {};
            vm.mapping.fields = [];
        } else {
            vm.mapping = JSON.parse(vm.setting.value);
            vm.selectedSfObject = vm.mapping.objectName;
        }

        umbracoFormsIntegrationsCrmSalesforceResource.authenticate().then(function (response) {
            if (response.httpStatusCode != 'OK') {
                vm.authorizationStatus = response.httpStatusCode;
                vm.authorizationStatusMessage = response.message;
                vm.accountSettings = response.data;
                vm.loading = false;
            }
            else {
                vm.authorizationStatus = response.httpStatusCode;
                getObjectList();
            }
        });
    };

    vm.authenticate = function () {
        vm.loading = true;
        umbracoFormsIntegrationsCrmSalesforceResource.setAccountSettings(vm.accountSettings).then(function (response) {
            umbracoFormsIntegrationsCrmSalesforceResource.authenticate().then(function (response) {
                if (response.httpStatusCode != 'OK') {
                    vm.authorizationStatus = response.httpStatusCode;
                    vm.authorizationStatusMessage = response.message;
                    showNotification(2, "Authentication failed", response.message);
                    vm.loading = false;
                }
                else {
                    showNotification(0, "Authentication succeeded", "Your Salesforce account is connected");
                    vm.authorizationStatus = response.httpStatusCode;
                    getObjectList();
                }
            });
        });
    }

    vm.disconnect = function () {
        var overlay = {
            view: "confirm",
            title: "Confirmation",
            content: "Are you sure you wish to disconnect your Salesforce account?",
            closeButtonLabel: "No",
            submitButtonLabel: "Yes",
            submitButtonStyle: "danger",
            close: function () {
                overlayService.close();
            },
            submit: function () {
                umbracoFormsIntegrationsCrmSalesforceResource.deleteAccountSettings().then(function (response) {
                    if (response.httpStatusCode != 'OK') {
                        showNotification(2, "Account disconnection failed", response.message);
                    }
                    else {
                        showNotification(0, "Disconnected", "Your Salesforce account is disconnected");
                        vm.authorizationStatus = "Unauthenticated";
                        vm.authorizationStatusMessage = "Your salesforce account is disconnected";
                        vm.accountSettings = {};
                        vm.showSfObjects = false;
                    }
                    overlayService.close();
                });
            }
        };
        overlayService.open(overlay);
    }

    vm.addMapping = function () {
        vm.mapping.fields.push({
            formField: "",
            objectField: ""
        });
    };

    vm.deleteMapping = function (index) {
        vm.mapping.fields.splice(index, 1);
        vm.setting.value = JSON.stringify(vm.mapping);
    };

    vm.stringifyValue = function () {
        vm.setting.value = JSON.stringify(vm.mapping);
    };

    showNotification = function (type, header, message) {
        notificationsService.showNotification({
            type: type,
            header: header,
            message: message,
        });
    }
}