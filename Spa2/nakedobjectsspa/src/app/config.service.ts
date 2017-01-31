import { Inject, Injectable } from '@angular/core';
import { Http , RequestOptionsArgs, RequestOptions} from '@angular/http';
import { Observable } from 'rxjs/Rx';

export interface IAppConfig {
    appPath: string;
    logoffUrl: string;

    // this can be a full url eg http://www.google.com
    postLogoffUrl: string;

    defaultPageSize: number; // can be overridden by server 
    listCacheSize: number;

    shortCutMarker: string;
    urlShortCuts: string[];

    keySeparator: string;
    objectColor: string;
    linkColor: string;

    autoLoadDirty: boolean;
    showDirtyFlag: boolean; // || !this.autoLoadDirty;

    // caching constants: do not change unless you know what you're doing 
    httpCacheDepth: number;
    transientCacheDepth: number;
    recentCacheDepth: number;

    // checks for inconsistencies in url 
    // deliberately off in .pp config file 
    doUrlValidation: boolean;

    // flag for configurable home button behaviour
    leftClickHomeAlwaysGoesToSinglePane: boolean;
}


class DefaultConfig implements IAppConfig {

    appPath : "";
    logoffUrl: "";

    // this can be a full url eg http://www.google.com
    postLogoffUrl: "/#/gemini/home";

    defaultPageSize: 20; // can be overridden by server 
    listCacheSize: 5;

    shortCutMarker: "___";
    urlShortCuts: ["http://nakedobjectsrodemo.azurewebsites.net", "AdventureWorksModel"];

    keySeparator: "--";
    objectColor: "object-color";
    linkColor: "link-color";

    autoLoadDirty: true;
    showDirtyFlag: false; // || !this.autoLoadDirty;

    // caching constants: do not change unless you know what you're doing 
    httpCacheDepth: 50;
    transientCacheDepth: 4;
    recentCacheDepth: 20;

    // checks for inconsistencies in url 
    // deliberately off in .pp config file 
    doUrlValidation: true;

    // flag for configurable home button behaviour
    leftClickHomeAlwaysGoesToSinglePane: true;
}


export function configFactory(config: ConfigService) {
    return () => config.load();
}

@Injectable()
export class ConfigService {

    private appConfig: IAppConfig = new DefaultConfig();

    constructor(private readonly http: Http) {

    }

    get config() {      
        return this.appConfig;      
    }

    set config(newConfig: IAppConfig) {
        // merge default 
        _.assign(this.appConfig, newConfig);
    }

    getAppPath(appPath: string) {
        if (appPath.charAt(appPath.length - 1) === "/") {
            return appPath.length > 1 ? appPath.substring(0, appPath.length - 1) : "";
        }
        return appPath;
    }

    checkAppPath() {
        this.appConfig.appPath = this.getAppPath(this.appConfig.appPath);
    }

    load() {

        const config = {
            withCredentials: true
        } as RequestOptionsArgs;

        return this.http.get('config.json', config).
            map(res => res.json()).
            toPromise().
            then(serverConfig => {
                this.config = serverConfig as IAppConfig;
                this.checkAppPath();
                return true;
            });
    }
}