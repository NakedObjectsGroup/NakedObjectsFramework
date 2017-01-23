import { Injectable } from '@angular/core';
import { CustomComponentConfigService } from './custom-component-config.service';
import { ObjectComponent } from './object/object.component';
import * as Models from './models';

@Injectable()
export class CustomComponentService {

    constructor(private readonly config: CustomComponentConfigService) {
        config.configure(this);
    }

    setCustomObjectComponent(type : string, component : any) {
        
    }

    getCustomObjectComponent(oid: Models.ObjectIdWrapper) {
        return Promise.resolve(ObjectComponent);
    }
}
