import * as Models from "./models";
import * as _ from "lodash";
import { Injectable } from '@angular/core';
import { ContextService } from './context.service';
import { ColorConfigService } from './color-config.service';
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
        private readonly config: ColorConfigService
    ) {
        super(context);
        this.setDefault(0);
        config.configure(this);
    }

    private typeFromUrl(url: string): string {
        const oid = Models.ObjectIdWrapper.fromHref(url);
        return oid.domainType;
    }

    toColorNumberFromHref = (href: string) => {
        const type = Models.typeFromUrl(href);
        return this.toColorNumberFromType(type);
    }

    toColorNumberFromType = (type: string | null) => this.getResult(type);
}
