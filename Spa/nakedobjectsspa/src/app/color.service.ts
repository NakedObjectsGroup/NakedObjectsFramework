import * as Models from "./models";
import * as _ from "lodash";
import { Injectable } from '@angular/core';
import { ContextService } from './context.service';
import { ColorConfigService } from "./color-config.service";

@Injectable()
export class ColorService {

    constructor(private context: ContextService, private config: ColorConfigService) {
        config.configure(this);
    }

    private colorCache: _.Dictionary<number> = {};
    private regexCache: { regex: RegExp, color: number }[] = [];
    private subtypeCache: { type: string, color: number }[] = [];

    private defaultColor = 0;

    private typeFromUrl(url: string): string {
        const oid = Models.ObjectIdWrapper.fromHref(url);
        return oid.domainType;
    }

    private isSubtypeOf(subtype: string, index: number, count: number): Promise<number> {

        if (index >= count) {
            return Promise.reject("") as any;
        }

        const entry = this.subtypeCache[index];
        return this.context.isSubTypeOf(subtype, entry.type).then(b => b ? Promise.resolve(entry.color) : this.isSubtypeOf(subtype, index + 1, count));
    }

    private cacheAndReturn(type: string, color: number) {
        this.colorCache[type] = color;
        return Promise.resolve(color);
    }

    private isSubtype(subtype: string): Promise<any> {

        const subtypeChecks = this.subtypeCache.length;

        if (subtypeChecks > 0) {
            return this.isSubtypeOf(subtype, 0, subtypeChecks)
                .then((c: number) => {
                    return this.cacheAndReturn(subtype, c);
                })
                .catch(() => {
                    return this.cacheAndReturn(subtype, this.defaultColor);
                });
        }

        return this.cacheAndReturn(subtype, this.defaultColor);
    }


    private getColor(type: string) {
        // 1 cache 
        // 2 match regex 
        // 3 match subtype 

        // this is potentially expensive - need to filter out non ref types ASAP

        if (!type || type === "string" || type === "number" || type === "boolean") {
            return Promise.resolve(this.defaultColor);
        }

        const cachedEntry = this.colorCache[type];

        if (cachedEntry) {
            return Promise.resolve(cachedEntry);
        }

        for (const entry of this.regexCache) {
            if (entry.regex.test(type)) {
                return this.cacheAndReturn(type, entry.color);
            }
        }

        return this.isSubtype(type);
    }


    toColorNumberFromHref = (href: string) => {
        const type = Models.typeFromUrl(href);
        return this.toColorNumberFromType(type);
    };

    toColorNumberFromType = (type: string) => this.getColor(type);

    addType = (type: string, color: number) => {
        this.colorCache[type] = color;
    };
    addMatch = (matcher: RegExp, color: number) => {
        this.regexCache.push({ regex: matcher, color: color });
    };
    addSubtype = (type: string, color: number) => {
        this.subtypeCache.push({ type: type, color: color });
    };
    setDefault = (def: number) => {
        this.defaultColor = def;
    };
}
