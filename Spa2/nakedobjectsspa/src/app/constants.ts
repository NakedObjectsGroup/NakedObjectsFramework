// code constants - not for config
let path: string | null = null;
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

export const fixedDateFormat = "YYYY-MM-DD";

// routing constants

export const geminiPath: ModePathSegment = "gemini";
export const ciceroPath: ModePathSegment = "cicero";

export type ModePathSegment = "gemini" | "cicero";

export const homePath: PathSegment = "home";
export const objectPath: PathSegment = "object";
export const listPath: PathSegment = "list";
export const errorPath: PathSegment = "error";
export const recentPath: PathSegment = "recent";
export const attachmentPath: PathSegment = "attachment";
export const applicationPropertiesPath: PathSegment = "applicationProperties";
export const multiLineDialogPath: PathSegment = "multiLineDialog";
export const logoffPath: PathSegment = "logoff";

export type PathSegment = "home" | "object" | "list" | "error" | "recent" | "attachment" | "applicationProperties" | "multiLineDialog" | "logoff";

// Restful Objects constants
export const roDomainType = "x-ro-domain-type";
export const roInvalidReason = "x-ro-invalidReason";
export const roSearchTerm = "x-ro-searchTerm";
export const roPage = "x-ro-page";
export const roPageSize = "x-ro-pageSize";
export const roInlinePropertyDetails = "x-ro-inline-property-details";
export const roValidateOnly = "x-ro-validate-only";
export const roInlineCollectionItems = "x-ro-inline-collection-items";

// NOF custom RO constants
export const nofWarnings = "x-ro-nof-warnings";
export const nofMessages = "x-ro-nof-messages";

export const supportedDateFormats = ["D/M/YYYY", "D/M/YY", "D MMM YYYY", "D MMMM YYYY", "D MMM YY", "D MMMM YY"];

// updated by build do not update manually or change name or regex may not match
export const clientVersion = '10.0.0';
