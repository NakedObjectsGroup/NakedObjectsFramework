import { Inject, Injectable } from '@angular/core';
import { Http , RequestOptionsArgs, RequestOptions} from '@angular/http';
import { Observable } from 'rxjs/Rx';

export interface IAppConfig {
    appPath: string;
}

export function configFactory(config: AppConfig) {
    return () => config.load();
}

@Injectable()
export class AppConfig {

    private appConfig: IAppConfig;

    constructor(private readonly http: Http) {

    }

    get config() {
        if (this.appConfig) {
            return this.appConfig;
        }
        throw new Error("Config has not been set");
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

        //return this.http.get('config.json', config).map(res => res.json()).catch((error: any): any => {
        //    console.log('Configuration file "config.json" could not be read');
        //    return Observable.throw(error.json().error || 'Server error');
        //}).subscribe((envResponse) => {
        //    this.appConfig = envResponse as IAppConfig;
        //    this.checkAppPath();
        //    
        //    });

        return this.http.get('config.json', config).
            map(res => res.json()).
            toPromise().
            then(j => {
                this.appConfig = j as IAppConfig;
                this.checkAppPath();
                return true;
            });
    }
}