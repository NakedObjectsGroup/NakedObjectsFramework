﻿import { Injectable } from '@angular/core';
import { ErrorCategory, HttpStatusCode } from '@nakedobjects/services';
import { ICustomComponentConfigurator } from './custom-component.service';
import { ICustomErrorComponentConfigurator } from './custom-component.service';
import { ObjectNotFoundErrorComponent } from './object-not-found-error/object-not-found-error.component';

export interface ICustomComponentConfigService {
    configureCustomObjects(custom: ICustomComponentConfigurator): void;

    configureCustomLists(custom: ICustomComponentConfigurator): void;

    configureCustomErrors(custom: ICustomErrorComponentConfigurator): void;
}

// default implementation which does nothing
@Injectable()
export class CustomComponentConfigService implements ICustomComponentConfigService {

    // Remember custom components need to be added to "entryComponents" in app.module.ts !

    configureCustomObjects(custom: ICustomComponentConfigurator) { }

    configureCustomLists(custom: ICustomComponentConfigurator) { }

    configureCustomErrors(custom: ICustomErrorComponentConfigurator) {
        // by default configure page for 404 errors
        custom.addError(ErrorCategory.HttpClientError, HttpStatusCode.NotFound, ObjectNotFoundErrorComponent);
    }
}
