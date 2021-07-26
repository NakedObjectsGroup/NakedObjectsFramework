import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import forEach from 'lodash-es/forEach';
import { basename } from 'path';
import { ConfigService } from './config.service';
import { ContextService } from './context.service';
import { TypeResultCache } from './type-result-cache';

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
        this.setDefault(0);
        this.configureFromConfig();
    }

    private configuredDefault: number;
    private configuredMaxRandomIndex: number;

    private typeFromUrl(url: string): string {
        const oid = Ro.ObjectIdWrapper.fromHref(url, this.configService.config.keySeparator);
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
        this.configuredDefault = def;
        super.setDefault(def);
    }

    getConfiguredDefault(): number {
        return this.configuredDefault;
    }

    simpleHash(str: string) {
        // tslint:disable-next-line:no-bitwise
        return Math.abs(Array.from(str).reduce((hash, char) => 0 | (31 * hash + char.charCodeAt(0)), 0));
    }

    getRandomColorNumber(type: string) {
        return (this.simpleHash(type) % this.configuredMaxRandomIndex) + 1;
    }

    getDefault(type: string) {
        return this.configuredMaxRandomIndex ?  this.getRandomColorNumber(type) : super.getDefault(type);
    }

    configureFromConfig(): void {
        const colorConfig = this.configService.config.colors;

        if (colorConfig) {
            const typeMap = colorConfig.typeMap;
            const subtypeMap = colorConfig.subtypeMap;
            const regexArray = colorConfig.regexArray;
            const dflt = colorConfig.default;
            this.configuredMaxRandomIndex = colorConfig.randomMaxIndex;

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
