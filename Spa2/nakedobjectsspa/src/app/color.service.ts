import * as Models from './models';
import * as _ from 'lodash';
import { Injectable } from '@angular/core';
import { ContextService } from './context.service';
import { ColorConfigService } from './color-config.service';
import { TypeResultCache } from './type-result-cache';
import { ConfigService } from './config.service';

export interface IColorServiceConfigurator {
    addType : (type: string, color: number) => void;

    addMatch: (matcher: RegExp, color: number) => void;

    addSubtype: (type: string, color: number) => void;

    setDefault: (def: number) => void;
}


@Injectable()
export class ColorService extends TypeResultCache<number> implements IColorServiceConfigurator {

    constructor(
        context: ContextService,
        private readonly colorConfig: ColorConfigService,
        private readonly configService: ConfigService
    ) {
        super(context);
        super.setDefault(0);
        colorConfig.configure(this);
        if (!this.serviceConfigured) {
            this.configureFromBasicConfig();
        }
    }

    private serviceConfigured = false;

    private typeFromUrl(url: string): string {
        const oid = Models.ObjectIdWrapper.fromHref(url, this.configService.config.keySeparator);
        return oid.domainType;
    }

    toColorNumberFromHref = (href: string) => {
        const type = this.typeFromUrl(href);
        return this.toColorNumberFromType(type);
    }

    toColorNumberFromType = (type: string | null) => this.getResult(type);

    private setServiceConfigured() {
        this.serviceConfigured = true;
    }

    addType(type: string, result: number) {
        super.addType(type, result);
        this.setServiceConfigured();
    }

    addMatch(matcher: RegExp, result: number) {
        super.addMatch(matcher, result);
        this.setServiceConfigured();
    }

    addSubtype(type: string, result: number) {
        super.addSubtype(type, result);
        this.setServiceConfigured();
    }

    setDefault(def: number) {
        super.setDefault(def);
        this.setServiceConfigured();
    }

    configureFromBasicConfig(): void {
        const basicConfig = this.configService.config.colors;

        if (basicConfig) {
            _.forEach(basicConfig.map, (v, k) => this.addType(k!, v));
            const dflt = basicConfig.default;

            if (dflt !== null) {
                this.setDefault(basicConfig.default);
            }
        }
    }
}
