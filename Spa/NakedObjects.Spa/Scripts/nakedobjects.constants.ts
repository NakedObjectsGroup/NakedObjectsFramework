/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
/// <reference path="nakedobjects.config.ts" />

namespace NakedObjects {

    // code constants - not for config
    let path: string = null;
    // path local server (ie ~) only needed if you want to hard code paths for some reason
    const svrPath = "";

    export function getSvrPath() {
        if (!path) {
            const trimmedPath = svrPath.trim();
            if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
                path = trimmedPath;
            } else {
                path = trimmedPath + "/";
            }
        }
        return path;
    }

    export const dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export const serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export const errorTemplate = getSvrPath() + "Content/partials/error.html";
    export const concurrencyTemplate = getSvrPath() + "Content/partials/concurrencyError.html";
    export const httpErrorTemplate = getSvrPath() + "Content/partials/httpError.html";
    export const appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export const nullTemplate = getSvrPath() + "Content/partials/null.html";
    export const blankTemplate = getSvrPath() + "Content/partials/blank.html";
    export const homeTemplate = getSvrPath() + "Content/partials/home.html";
    export const homePlaceholderTemplate = getSvrPath() + "Content/partials/homePlaceholder.html";
    export const objectTemplate = getSvrPath() + "Content/partials/object.html";
    export const objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    export const objectNotpersistentTemplate = getSvrPath() + "Content/partials/objectNotpersistent.html";
    export const objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    export const formTemplate = getSvrPath() + "Content/partials/form.html";
    export const expiredTransientTemplate = getSvrPath() + "Content/partials/expiredTransient.html";

    export const listPlaceholderTemplate = getSvrPath() + "Content/partials/ListPlaceholder.html";
    export const listTemplate = getSvrPath() + "Content/partials/List.html";
    export const listAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";

    export const footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export const actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export const formActionsTemplate = getSvrPath() + "Content/partials/formActions.html";
    export const collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export const collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export const collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export const collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";

    export const recentTemplate = getSvrPath() + "Content/partials/recent.html";
    export const attachmentTemplate = getSvrPath() + "Content/partials/attachment.html";
    export const ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";
    export const applicationPropertiesTemplate = getSvrPath() + "Content/partials/applicationProperties.html";

    // routing constants 

    export const geminiPath = "gemini";
    export const ciceroPath = "cicero";
    export const homePath = "home";
    export const objectPath = "object";
    export const listPath = "list";
    export const errorPath = "error";
    export const recentPath = "recent";
    export const attachmentPath = "attachment";
    export const applicationPropertiesPath = "applicationProperties";

    //Restful Objects constants
    export const roDomainType = "x-ro-domain-type";
    export const roInvalidReason = "x-ro-invalidReason";
    export const roSearchTerm = "x-ro-searchTerm";
    export const roPage = "x-ro-page";
    export const roPageSize = "x-ro-pageSize";
    export const roInlinePropertyDetails = "x-ro-inline-property-details";
    export const roValidateOnly = "x-ro-validate-only";
    export const roInlineCollectionItems = "x-ro-inline-collection-items";

    //NOF custom RO constants  
    export const nofWarnings = "x-ro-nof-warnings";
    export const nofMessages = "x-ro-nof-messages";
   
  
    export const supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];

    // events 
    export const geminiFocusEvent = "nof-focus-on";
    export const geminiLogoffEvent = "nof-logoff";
    export const geminiPaneSwapEvent = "nof-pane-swap";
    export const geminiConcurrencyEvent = "nof-concurrency";
    export const geminiAjaxChangeEvent = "nof-ajax-change";
    export const geminiWarningEvent = "nof-warning";
    export const geminiMessageEvent = "nof-message";
}