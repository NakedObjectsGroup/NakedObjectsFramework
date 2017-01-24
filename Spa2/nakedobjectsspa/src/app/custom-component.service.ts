import { Injectable } from '@angular/core';
import { CustomComponentConfigService } from './custom-component-config.service';
import { ObjectComponent } from './object/object.component';
import * as Models from './models';
import { ViewType } from './route-data';
import { ListComponent } from './list/list.component';
import { Type } from '@angular/core/src/type';

@Injectable()
export class CustomComponentService {

    constructor(private readonly config: CustomComponentConfigService) {

        this.customComponents = [];
        this.customComponents[ViewType.Object] = {};
        this.customComponents[ViewType.List] = {};

        config.configure(this);
    }

    private readonly customComponents: _.Dictionary<Type<any>>[];

    setCustomComponent(domainType: string, component: Type<any>, viewType: ViewType.Object | ViewType.List) {

        this.customComponents[viewType][domainType] = component;
    }

    getCustomComponent(domainType: string, viewType: ViewType.Object | ViewType.List) {

        const custom = this.customComponents[viewType][domainType];

        if (custom) {
            return Promise.resolve(custom);
        } else if (viewType === ViewType.Object) {
            return Promise.resolve(ObjectComponent);
        } else {
            return Promise.resolve(ListComponent);
        }
    }
}
