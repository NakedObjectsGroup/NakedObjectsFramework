import * as Ro from './ro-interfaces';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';
import * as momentNs from 'moment';
import forEach from 'lodash-es/forEach';

const moment = momentNs;

export interface IMaskServiceConfigurator {
    setNumberMaskMapping: (customMask: string, format: Ro.FormatType, digits?: string, locale?: string) => void;

    // date masks now use moment mask conventions https://momentjs.com/docs/#/displaying/
    setDateMaskMapping: (customMask: string, format: Ro.FormatType, mask: string, tz?: string, locale?: string) => void;

    setCurrencyMaskMapping: (customMask: string, format: Ro.FormatType, symbol?: string, digits?: string, locale?: string) => void;
}

export interface ILocalFilter {
    filter(val: any): string;
}

export interface IMaskMap {
    [index: string]: { [index: string]: ILocalFilter };
}

class LocalStringFilter implements ILocalFilter {

    filter(val: any): string {
        return val ? val.toString() : "";
    }
}

function transform(tfm: () => string | null) {
    try {
        return tfm();
    } catch (e) {
        return "";
    }
}

class LocalCurrencyFilter implements ILocalFilter {

    constructor(
        private readonly locale: string,
        private readonly symbol?: string,
        private readonly digits?: string,
    ) { }

    filter(val: any): string {
        if (val == null || val === "") {
            return "";
        }

        const pipe = new CurrencyPipe(this.locale);
        return transform(() => pipe.transform(val, this.symbol, "symbol", this.digits)) || "";
    }
}

class LocalDateFilter implements ILocalFilter {

    constructor(
        private readonly locale: string,
        private readonly mask?: string,
        private readonly tz?: string,
    ) { }

    filter(val: string): string {
        if (!val) {
            return "";
        }
        // Angular date pipes no longer support timezones so we need to use moment here

        // date or time
        let mmt = val.length > 8 ?  moment.utc(val) : moment.utc(val, "HH:mm:ss");

        if (mmt.isValid()) {
            if (this.tz) {
                mmt = mmt.utcOffset(this.tz);
            }
            return mmt.format(this.mask);
        }
        return "";
    }
}

class LocalNumberFilter implements ILocalFilter {

    constructor(
        private readonly locale: string,
        private readonly digits?: string
    ) { }

    filter(val: any): string {
        if (val == null || val === "") {
            return "";
        }
        const pipe = new DecimalPipe(this.locale);
        const result = transform(() => pipe.transform(val, this.digits));
        return result == null ? "" : result;
    }
}

@Injectable()
export class MaskService implements IMaskServiceConfigurator {

    private maskMap: IMaskMap = {
        string: {},
        "date-time": {},
        date: {},
        time: {},
        "utc-millisec": {},
        "big-integer": {},
        "big-decimal": {},
        blob: {},
        clob: {},
        decimal: {},
        int: {}
    };

    constructor(
        private readonly appConfig: ConfigService
    ) {
        this.defaultLocale = appConfig.config.defaultLocale;
        this.configureFromConfig();
    }

    private readonly defaultLocale: string;

    defaultLocalFilter(format: Ro.FormatType): ILocalFilter {
        switch (format) {
            case ("string"):
                return new LocalStringFilter();
            case ("date-time"):
                return new LocalDateFilter(this.defaultLocale, "D MMM YYYY HH:mm:ss");
            case ("date"):
                return new LocalDateFilter(this.defaultLocale, "D MMM YYYY", "+0000");
            case ("time"):
                return new LocalDateFilter(this.defaultLocale, "HH:mm", "+0000");
            case ("utc-millisec"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("big-integer"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("big-decimal"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("blob"):
                return new LocalStringFilter();
            case ("clob"):
                return new LocalStringFilter();
            case ("decimal"):
                return new LocalNumberFilter(this.defaultLocale);
            case ("int"):
                return new LocalNumberFilter(this.defaultLocale);
            default:
                return new LocalStringFilter();
        }
    }

    private customFilter(format: Ro.FormatType, remoteMask: string | null) {
        if (remoteMask && this.maskMap[format]) {
            return this.maskMap[format][remoteMask];
        }
        return undefined;
    }

    toLocalFilter(remoteMask: string | null, format: Ro.FormatType) {
        return this.customFilter(format, remoteMask) || this.defaultLocalFilter(format);
    }

    setNumberMaskMapping(customMask: string, format: Ro.FormatType, digits?: string, locale?: string) {
        this.maskMap[format!][customMask] = new LocalNumberFilter(locale || this.defaultLocale, digits);
    }

    setDateMaskMapping(customMask: string, format: Ro.FormatType, mask: string, tz?: string, locale?: string) {
        this.maskMap[format!][customMask] = new LocalDateFilter(locale || this.defaultLocale, mask, tz);
    }

    setCurrencyMaskMapping(customMask: string, format: Ro.FormatType, symbol?: string, digits?: string, locale?: string) {
        this.maskMap[format!][customMask] = new LocalCurrencyFilter(locale || this.defaultLocale, symbol, digits);
    }

    private configureFromConfig() {
        const maskConfig = this.appConfig.config.masks;

        if (maskConfig) {

            const currencyMasks = maskConfig.currencyMasks;
            const dateMasks = maskConfig.dateMasks;
            const numberMasks = maskConfig.numberMasks;

            if (currencyMasks) {
                forEach(currencyMasks, (v, k) => this.setCurrencyMaskMapping(k!, v.format, v.symbol, v.digits, v.locale));
            }

            if (dateMasks) {
                forEach(dateMasks, (v, k) => this.setDateMaskMapping(k!, v.format, v.mask, v.tz, v.locale));
            }

            if (numberMasks) {
                forEach(numberMasks, (v, k) => this.setNumberMaskMapping(k!, v.format, v.digits, v.locale));
            }
        }
    }
}
