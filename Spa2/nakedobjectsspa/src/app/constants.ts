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
export const multiLineDialogPath = "multiLineDialog";

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
