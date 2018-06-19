import * as Models from './models';

import { Injectable } from '@angular/core';
import { ContextService } from './context.service';
import { TypeResultCache } from './type-result-cache';
import { ConfigService } from './config.service';
import { Dictionary } from 'lodash';
import forEach from 'lodash-es/forEach';

export interface IColorServiceConfigurator {
    addType: (type: string, color: number) => void;

    addMatch: (matcher: RegExp, color: number) => void;

    addSubtype: (type: string, color: number) => void;

    setDefault: (def: number) => void;
}

@Injectable()
export class ColorService extends TypeResultCache<number> implements IColorServiceConfigurator {

    constructor(
        context: ContextService,
        private readonly configService: ConfigService
    ) {
        super(context);
        super.setDefault(0);
        this.configureFromConfig();
    }

    private typeFromUrl(url: string): string {
        const oid = Models.ObjectIdWrapper.fromHref(url, this.configService.config.keySeparator);
        return oid.domainType;
    }

    toColorNumberFromHref = (href: string) => {
        const type = this.typeFromUrl(href);
        return this.toColorNumberFromType(type);
    }

    toColorNumberFromType = (type: string | null) => this.getResult(type);

    addType(type: string, result: number) {
        super.addType(type, result);
    }

    addMatch(matcher: RegExp, result: number) {
        super.addMatch(matcher, result);
    }

    addSubtype(type: string, result: number) {
        super.addSubtype(type, result);
    }

    setDefault(def: number) {
        super.setDefault(def);
    }

    getDefault(): number {
        return this.default;
    }

    configureFromConfig(): void {
        const colorConfig = this.configService.config.colors;

        if (colorConfig) {
            const typeMap = colorConfig.typeMap;
            const subtypeMap = colorConfig.subtypeMap;
            const regexArray = colorConfig.regexArray;
            const dflt = colorConfig.default;

            if (typeMap) {
                forEach(typeMap, (v, k) => this.addType(k!, v));
            }

            if (regexArray) {
                forEach(regexArray, item => this.addMatch(new RegExp(item.regex), item.color));
            }

            if (subtypeMap) {
                forEach(subtypeMap, (v, k) => this.addSubtype(k!, v));
            }

            if (dflt != null) {
                this.setDefault(dflt);
            }
        }
    }
}
