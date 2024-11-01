import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import assign from 'lodash-es/assign';
import { defaultDateFormat } from './date-constants';
import { lastValueFrom } from 'rxjs';

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
    clearCacheOnChange: boolean;

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

    // left click goes left, right click goes right
    clickAlwaysGoesToSameSide: boolean;

    logLevel: 'error' | 'warn' | 'info' | 'debug' | 'none';

    dateInputFormat: string;

    // color set by first matching rule in order type, regex, subtype, default (faster to slower)
    colors?: {
        typeMap?: Record<string, number>,
        regexArray?: { regex: RegExp, color: number }[],
        subtypeMap?: Record<string, number>,
        default?: number;
        randomMaxIndex?: number;
    };

    // Note: "D" is the default mask for anything sent to the client as a date+time,
    // where no other mask is specified.
    // This mask deliberately does not specify the timezone as "UTC+0", unlike the other masks,
    // with the result that the date+time will be transformed to the timezone of the client.
    masks?: {
        currencyMasks?: Record<string, {
                format: Ro.FormatType;
                symbol?: string;
                digits?: string;
                locale?: string;
            }>
        dateMasks?: Record<string, {
                format: Ro.FormatType;
                mask: string;
                tz?: string;
                locale?: string;
            }>
        numberMasks?: Record<string, {
                format: Ro.FormatType;
                digits?: string;
                locale?: string;
            }>
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
        appPath: '',
        applicationName: '',
        logoffUrl: '',
        postLogoffUrl: '/gemini/home',
        defaultPageSize: 20,
        listCacheSize: 5,
        shortCutMarker: '___',
        urlShortCuts: [],
        keySeparator: '--',
        objectColor: 'object-color',
        linkColor: 'link-color',
        autoLoadDirty: true,
        showDirtyFlag: false,
        clearCacheOnChange: true,
        defaultLocale: 'en-GB',
        httpCacheDepth: 50,
        transientCacheDepth: 4,
        recentCacheDepth: 20,
        doUrlValidation: false,
        leftClickHomeAlwaysGoesToSinglePane: true,
        logLevel: 'error',
        dateInputFormat: defaultDateFormat,
        clickAlwaysGoesToSameSide: true,
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
        if (appPath.charAt(appPath.length - 1) === '/') {
            return appPath.length > 1 ? appPath.substring(0, appPath.length - 1) : '';
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

        const appConfig$ = this.http.get<IAppConfig>('config.json', options);

        return lastValueFrom(appConfig$).
            then((r) => {
                if (r) {
                    this.config = r;
                }
                this.checkAppPath();
                this.loadingStatus = ConfigState.loaded;
                return true;
            }).
            catch(() => {
                this.loadingStatus = ConfigState.failed;
            });
    }
}
