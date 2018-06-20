import { Injectable } from '@angular/core';
import * as Ro from './ro-interfaces';

import assign from 'lodash-es/assign';
import { HttpClient, HttpRequest, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';

export enum ConfigState {
    pending,
    loaded,
    failed
}

export interface IAppConfig {
    authenticate: boolean;
    authClientId?: string;
    authDomain?: string;
    appPath: string;
    applicationName: string;
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
    showDirtyFlag: boolean;

    defaultLocale: string;

    // caching constants: do not change unless you know what you're doing
    httpCacheDepth: number;
    transientCacheDepth: number;
    recentCacheDepth: number;

    // checks for inconsistencies in url
    // deliberately off by default
    doUrlValidation: boolean;

    // flag for configurable home button behaviour
    leftClickHomeAlwaysGoesToSinglePane: boolean;

    logLevel: "error" | "warn" | "info" | "debug" | "none";

    dateInputFormat: string;

    // color set by first matching rule in order type, regex, subtype, default (faster to slower)
    colors?: {
        typeMap?: {
            [index: string]: number;
        },
        regexArray?: { regex: RegExp, color: number }[],
        subtypeMap?: {
            [index: string]: number;
        },
        default?: number;
    };

    // Note: "D" is the default mask for anything sent to the client as a date+time,
    // where no other mask is specified.
    // This mask deliberately does not specify the timezone as "+0000", unlike the other masks,
    // with the result that the date+time will be transformed to the timezone of the client.
    masks?: {
        currencyMasks?: {
            [index: string]: {
                format: Ro.FormatType;
                symbol?: string;
                digits?: string;
                locale?: string;
            }
        }
        dateMasks?: {
            [index: string]: {
                format: Ro.FormatType;
                mask: string;
                tz?: string;
                locale?: string;
            }
        }
        numberMasks?: {
            [index: string]: {
                format: Ro.FormatType;
                digits?: string;
                locale?: string;
            }
        }
    };
}

export function configFactory(config: ConfigService) {
    return () => config.load();
}

export function localeFactory(config: ConfigService) {
    return config.config.defaultLocale;
}

@Injectable()
export class ConfigService {

    // defaults
    private appConfig: IAppConfig = {
        authenticate: false,
        appPath: "",
        applicationName: "",
        logoffUrl: "",
        postLogoffUrl: "/gemini/home",
        defaultPageSize: 20,
        listCacheSize: 5,
        shortCutMarker: "___",
        urlShortCuts: [],
        keySeparator: "--",
        objectColor: "object-color",
        linkColor: "link-color",
        autoLoadDirty: true,
        showDirtyFlag: false,
        defaultLocale: "en-GB",
        httpCacheDepth: 50,
        transientCacheDepth: 4,
        recentCacheDepth: 20,
        doUrlValidation: false,
        leftClickHomeAlwaysGoesToSinglePane: true,
        logLevel: "error",
        dateInputFormat: "D MMM YYYY"
    };

    constructor(private readonly http: HttpClient) {

    }

    loadingStatus: ConfigState = ConfigState.pending;

    get config() {
        return this.appConfig;
    }

    set config(newConfig: IAppConfig) {
        // merge defaults
        assign(this.appConfig, newConfig);
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

        const options = {
            withCredentials: true
        };

        return this.http.get<IAppConfig>('config.json', options).
            toPromise().
            then((r) => {
                this.config = r;
                this.checkAppPath();
                this.loadingStatus = ConfigState.loaded;
                return true;
            }).
            catch(() => {
                this.loadingStatus = ConfigState.failed;
            });
    }
}
