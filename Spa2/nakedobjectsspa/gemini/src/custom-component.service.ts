import { Injectable } from '@angular/core';
import { CustomComponentConfigService } from './custom-component-config.service';
import { ObjectComponent } from './object/object.component';
import { ViewType } from '@nakedobjects/services';
import { ListComponent } from './list/list.component';
import { Type } from '@angular/core/src/type';
import { TypeResultCache } from '@nakedobjects/services';
import { ContextService } from '@nakedobjects/services';
import { ErrorComponent } from './error/error.component';
import { ErrorCategory, HttpStatusCode, ClientErrorCode } from './constants';

export interface ICustomComponentConfigurator {
    addType: (type: string, result: Type<any>) => void;

    addMatch: (matcher: RegExp, result: Type<any>) => void;

    addSubtype: (type: string, result: Type<any>) => void;

    setDefault: (def: Type<any>) => void;
}

export interface ICustomErrorComponentConfigurator {
    addError(rc: ErrorCategory, code: HttpStatusCode | ClientErrorCode, result: Type<any>): void;
}

class CustomComponentCache extends TypeResultCache<Type<any>> implements ICustomComponentConfigurator {

    constructor(context: ContextService, def: Type<any>) {
        super(context);
        this.setDefault(def);
    }
}

@Injectable()
export class CustomComponentService implements ICustomErrorComponentConfigurator {

    constructor(
        private readonly context: ContextService,
        private readonly config: CustomComponentConfigService) {

        this.customComponentCaches = [];
        this.customComponentCaches[ViewType.Object] = new CustomComponentCache(context, ObjectComponent);
        this.customComponentCaches[ViewType.List] = new CustomComponentCache(context, ListComponent);
        this.customComponentCaches[ViewType.Error] = new CustomComponentCache(context, ErrorComponent);

        config.configureCustomObjects(this.customComponentCaches[ViewType.Object]);
        config.configureCustomLists(this.customComponentCaches[ViewType.List]);
        config.configureCustomErrors(this);
    }

    private readonly customComponentCaches: CustomComponentCache[] = [];

    private getErrorKey(rc: ErrorCategory, code: HttpStatusCode | ClientErrorCode) {
        const key = `${ErrorCategory[rc]}-${rc === ErrorCategory.ClientError ? ClientErrorCode[code] : HttpStatusCode[code]}`;
        return key;
    }

    getCustomComponent(domainType: string, viewType: ViewType.Object | ViewType.List | ViewType.Error) {
        return this.customComponentCaches[viewType].getResult(domainType);
    }

    getCustomErrorComponent(rc: ErrorCategory, code: HttpStatusCode | ClientErrorCode) {
        const key = this.getErrorKey(rc, code);
        return this.customComponentCaches[ViewType.Error].getResult(key);
    }

    addError(rc: ErrorCategory, code: HttpStatusCode | ClientErrorCode, result: Type<any>) {
        const key = this.getErrorKey(rc, code);
        this.customComponentCaches[ViewType.Error].addType(key, result);
    }
}
