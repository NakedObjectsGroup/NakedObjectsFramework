import { Injectable } from '@angular/core';
import { CustomComponentConfigService } from './custom-component-config.service';
import { ObjectComponent } from './object/object.component';
import * as Models from './models';
import { ViewType } from './route-data';
import { ListComponent } from './list/list.component';

@Injectable()
export class CustomComponentService {

    constructor(private readonly config: CustomComponentConfigService) {
        config.configure(this);
    }

    setCustomObjectComponent(type : string, component : any) {
        
    }

    getCustomObjectComponent(oid: Models.ObjectIdWrapper, vType: ViewType.Object | ViewType.List) {

        if (vType === ViewType.Object) {
            return Promise.resolve(ObjectComponent);
        } else {
            return Promise.resolve(ListComponent);
        }
    }
}
