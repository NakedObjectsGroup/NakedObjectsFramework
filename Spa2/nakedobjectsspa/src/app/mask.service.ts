import * as _ from "lodash";
import * as Ro from './ro-interfaces';
import { DatePipe, CurrencyPipe, DecimalPipe } from '@angular/common';
import { MaskConfigService } from './mask-config.service';
import { Injectable } from '@angular/core';

export interface IMaskServiceConfigurator {
    setNumberMaskMapping: (customMask: string, format: Ro.formatType, fractionSize?: number) => void;

    setDateMaskMapping: (customMask: string, format: Ro.formatType, mask: string, tz?: string) => void;

    setCurrencyMaskMapping: (customMask: string, format: Ro.formatType, symbol?: string, digits?: string) => void;
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

function transform(tfm: () => string) {
    try {
        return tfm();
    }
    catch (e) {
        return "";
    }
}

class LocalCurrencyFilter implements ILocalFilter {

    constructor(
        private readonly symbol?: string,
        private readonly digits?: string) { }

    filter(val: any): string {
        if (!val) {
            return "";
        }

        // todo fix locales !!
        const pipe = new CurrencyPipe("en-GB");
        // return $filter("currency")(val, this.symbol, this.fractionSize);

        return transform(() => pipe.transform(val, this.symbol, true, this.digits));

    }
}

class LocalDateFilter implements ILocalFilter {

    constructor(
        private readonly mask?: string,
        private readonly tz?: string) { }

    filter(val: any): string {
        if (!val){
            return "";
        }
        // todo fix this ! 
        const pipe = new DatePipe("en-GB");
        //   return $filter("date")(val, this.mask, this.tz);
        return transform(() =>pipe.transform(val, this.mask));
    }
}

class LocalNumberFilter implements ILocalFilter {

    constructor(private readonly fractionSize?: number) {}

    filter(val: any): string {
        if (val == null || val === ""){
            return "";
        }

        //  return $filter("number")(val, this.fractionSize);
        // todo fix this ! 
        const pipe = new DecimalPipe("en-GB");
        // return $filter("currency")(val, this.symbol, this.fractionSize);
        return transform(() =>pipe.transform(val));
    }
}

@Injectable()
export class MaskService implements IMaskServiceConfigurator{

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

    constructor(private readonly config: MaskConfigService) {
        config.configure(this);
    }

    defaultLocalFilter(format: Ro.formatType): ILocalFilter {
        switch (format) {
        case ("string"):
            return new LocalStringFilter();
        case ("date-time"):
            return new LocalDateFilter("d MMM yyyy HH:mm:ss");
        case ("date"):
            return new LocalDateFilter("d MMM yyyy", "+0000");
        case ("time"):
            return new LocalDateFilter("HH:mm", "+0000");
        case ("utc-millisec"):
            return new LocalNumberFilter();
        case ("big-integer"):
            return new LocalNumberFilter();
        case ("big-decimal"):
            return new LocalNumberFilter();
        case ("blob"):
            return new LocalStringFilter();
        case ("clob"):
            return new LocalStringFilter();
        case ("decimal"):
            return new LocalNumberFilter();
        case ("int"):
            return new LocalNumberFilter();
        default:
            return new LocalStringFilter();
        }
    };

    private customFilter(format: Ro.formatType, remoteMask: string | null) {
        if (remoteMask && this.maskMap[format!]) {
            return this.maskMap[format!][remoteMask];
        }
        return undefined;
    }

    toLocalFilter(remoteMask: string | null, format: Ro.formatType) {
        return this.customFilter(format, remoteMask) || this.defaultLocalFilter(format);
    };

    setNumberMaskMapping(customMask: string, format: Ro.formatType, fractionSize?: number) {
        this.maskMap[format!][customMask] = new LocalNumberFilter(fractionSize);
    };

    setDateMaskMapping(customMask: string, format: Ro.formatType, mask: string, tz?: string) {
        this.maskMap[format!][customMask] = new LocalDateFilter(mask, tz);
    };

    setCurrencyMaskMapping(customMask: string, format: Ro.formatType, symbol?: string, digits?: string) {
        this.maskMap[format!][customMask] = new LocalCurrencyFilter(symbol, digits);
    };
}
