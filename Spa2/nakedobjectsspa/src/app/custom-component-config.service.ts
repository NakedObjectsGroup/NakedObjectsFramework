import { Injectable } from '@angular/core';
import { CustomComponentService } from './custom-component.service';
import { ICustomComponentConfigurator } from './custom-component.service';
import { ICustomErrorComponentConfigurator } from './custom-component.service';
import * as Routedata from './route-data';

@Injectable()
export class CustomComponentConfigService {

    // Remember custom components need to be added to "entryComponents" in app.module.ts !

    constructor(customComponentService: CustomComponentService) {
        this.configureCustomObjects(customComponentService.objectCache);
        this.configureCustomLists(customComponentService.listCache);
        this.configureCustomErrors(customComponentService);
    }


    configureCustomObjects(custom: ICustomComponentConfigurator) {


    }

    configureCustomLists(custom: ICustomComponentConfigurator) {


    }

    configureCustomErrors(custom: ICustomErrorComponentConfigurator) {


    }

}
