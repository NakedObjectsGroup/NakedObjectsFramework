import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';

@Injectable()
export class LoggerService {

    private readonly logError: boolean;
    private readonly logWarn: boolean;
    private readonly logInfo: boolean;
    private readonly logDebug: boolean;

    private readonly noop = (message?: any, ...optionalParams: any[]): void => { };

    constructor(private readonly configService: ConfigService) {
        switch (configService.config.logLevel) {
            case ("debug"):
                this.logError = this.logWarn = this.logInfo = this.logDebug = true;
                break;
            case ("info"):
                this.logError = this.logWarn = this.logInfo = true;
                this.logDebug = false;
                break;
            case ("warn"):
                this.logError = this.logWarn = true;
                this.logInfo = this.logDebug = false;
                break;
            case ("none"):
                this.logError = this.logWarn = this.logInfo = this.logDebug = false;
                break;
            case ("error"):
            default:
                this.logError = true;
                this.logWarn = this.logInfo = this.logDebug = false;
                break;
        }
    }

    get error(): (message?: any, ...optionalParams: any[]) => void {
        if (this.logError) { return console.error.bind(console); }
        return this.noop;
    }

    get warn(): (message?: any, ...optionalParams: any[]) => void {
        if (this.logWarn) { return console.warn.bind(console); }
        return this.noop;
    }

    get info(): (message?: any, ...optionalParams: any[]) => void {
        if (this.logInfo) { return console.info.bind(console); }
        return this.noop;
    }

    get debug(): (message?: any, ...optionalParams: any[]) => void {
        if (this.logDebug) { return console.debug.bind(console); }
        return this.noop;
    }

    throw(message?: any, ...optionalParams: any[]): never {
        this.error(message, optionalParams);
        throw new Error(message);
    }
}
