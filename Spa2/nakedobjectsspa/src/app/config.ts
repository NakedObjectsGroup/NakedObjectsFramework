// custom configuration for a particular implementation 

// todo fix
//export const logoffUrl = "" + "/Account/Logoff";

// this can be a full url eg http://www.google.com
export const postLogoffUrl = "/#/gemini/home";

export const defaultPageSize = 20; // can be overridden by server 
export const listCacheSize = 5;

export const shortCutMarker = "___";
export const urlShortCuts = ["http://nakedobjectsrodemo.azurewebsites.net", "AdventureWorksModel"];

export const keySeparator = "--";
export const objectColor = "object-color";
export const linkColor = "link-color";

export const autoLoadDirty = true;
export const showDirtyFlag = false || !autoLoadDirty;

// caching constants: do not change unless you know what you're doing 
export const httpCacheDepth = 50;
export const transientCacheDepth = 4;
export const recentCacheDepth = 20;

// checks for inconsistencies in url 
// deliberately off in .pp config file 
export const doUrlValidation = true;

// flag for configurable home button behaviour
export const leftClickHomeAlwaysGoesToSinglePane = true;
