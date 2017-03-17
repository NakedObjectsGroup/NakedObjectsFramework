/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="nakedobjects.config.ts" />
var NakedObjects;
(function (NakedObjects) {
    // code constants - not for config
    var path = null;
    // path local server (ie ~) only needed if you want to hard code paths for some reason
    var svrPath = "";
    function getSvrPath() {
        if (!path) {
            var trimmedPath = svrPath.trim();
            if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
                path = trimmedPath;
            }
            else {
                path = trimmedPath + "/";
            }
        }
        return path;
    }
    NakedObjects.getSvrPath = getSvrPath;
    NakedObjects.dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    NakedObjects.serviceTemplate = getSvrPath() + "Content/partials/service.html";
    NakedObjects.errorTemplate = getSvrPath() + "Content/partials/error.html";
    NakedObjects.concurrencyTemplate = getSvrPath() + "Content/partials/concurrencyError.html";
    NakedObjects.httpErrorTemplate = getSvrPath() + "Content/partials/httpError.html";
    NakedObjects.appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    NakedObjects.nullTemplate = getSvrPath() + "Content/partials/null.html";
    NakedObjects.blankTemplate = getSvrPath() + "Content/partials/blank.html";
    NakedObjects.homeTemplate = getSvrPath() + "Content/partials/home.html";
    NakedObjects.homePlaceholderTemplate = getSvrPath() + "Content/partials/homePlaceholder.html";
    NakedObjects.objectTemplate = getSvrPath() + "Content/partials/object.html";
    NakedObjects.objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    NakedObjects.objectNotpersistentTemplate = getSvrPath() + "Content/partials/objectNotpersistent.html";
    NakedObjects.objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    NakedObjects.formTemplate = getSvrPath() + "Content/partials/form.html";
    NakedObjects.expiredTransientTemplate = getSvrPath() + "Content/partials/expiredTransient.html";
    NakedObjects.listPlaceholderTemplate = getSvrPath() + "Content/partials/ListPlaceholder.html";
    NakedObjects.listTemplate = getSvrPath() + "Content/partials/List.html";
    NakedObjects.listAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";
    NakedObjects.footerTemplate = getSvrPath() + "Content/partials/footer.html";
    NakedObjects.actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    NakedObjects.formActionsTemplate = getSvrPath() + "Content/partials/formActions.html";
    NakedObjects.collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    NakedObjects.collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    NakedObjects.collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    NakedObjects.collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";
    NakedObjects.recentTemplate = getSvrPath() + "Content/partials/recent.html";
    NakedObjects.attachmentTemplate = getSvrPath() + "Content/partials/attachment.html";
    NakedObjects.ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";
    NakedObjects.applicationPropertiesTemplate = getSvrPath() + "Content/partials/applicationProperties.html";
    NakedObjects.multiLineDialogTemplate = getSvrPath() + "Content/partials/multiLineDialog.html";
    NakedObjects.propertiesTemplate = getSvrPath() + "Content/partials/properties.html";
    NakedObjects.parametersTemplate = getSvrPath() + "Content/partials/parameters.html";
    NakedObjects.propertyTemplate = getSvrPath() + "Content/partials/readOnlyProperty.html";
    NakedObjects.parameterTemplate = getSvrPath() + "Content/partials/parameter.html";
    NakedObjects.readOnlyParameterTemplate = getSvrPath() + "Content/partials/readOnlyParameter.html";
    // routing constants 
    NakedObjects.geminiPath = "gemini";
    NakedObjects.ciceroPath = "cicero";
    NakedObjects.homePath = "home";
    NakedObjects.objectPath = "object";
    NakedObjects.listPath = "list";
    NakedObjects.errorPath = "error";
    NakedObjects.recentPath = "recent";
    NakedObjects.attachmentPath = "attachment";
    NakedObjects.applicationPropertiesPath = "applicationProperties";
    NakedObjects.multiLineDialogPath = "multiLineDialog";
    //Restful Objects constants
    NakedObjects.roDomainType = "x-ro-domain-type";
    NakedObjects.roInvalidReason = "x-ro-invalidReason";
    NakedObjects.roSearchTerm = "x-ro-searchTerm";
    NakedObjects.roPage = "x-ro-page";
    NakedObjects.roPageSize = "x-ro-pageSize";
    NakedObjects.roInlinePropertyDetails = "x-ro-inline-property-details";
    NakedObjects.roValidateOnly = "x-ro-validate-only";
    NakedObjects.roInlineCollectionItems = "x-ro-inline-collection-items";
    //NOF custom RO constants  
    NakedObjects.nofWarnings = "x-ro-nof-warnings";
    NakedObjects.nofMessages = "x-ro-nof-messages";
    NakedObjects.supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];
    // events 
    NakedObjects.geminiFocusEvent = "nof-focus-on";
    NakedObjects.geminiLogoffEvent = "nof-logoff";
    NakedObjects.geminiPaneSwapEvent = "nof-pane-swap";
    NakedObjects.geminiConcurrencyEvent = "nof-concurrency";
    NakedObjects.geminiAjaxChangeEvent = "nof-ajax-change";
    NakedObjects.geminiWarningEvent = "nof-warning";
    NakedObjects.geminiMessageEvent = "nof-message";
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.constants.js.map