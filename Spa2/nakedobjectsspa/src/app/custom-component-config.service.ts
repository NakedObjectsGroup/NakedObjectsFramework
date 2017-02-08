import { Injectable } from '@angular/core';
import { CustomComponentService } from './custom-component.service';
import { ICustomComponentConfigurator } from './custom-component.service';
import { ICustomErrorComponentConfigurator } from './custom-component.service';

@Injectable()
export class CustomComponentConfigService {

    // Remember custom components need to be added to "entryComponents" in app.module.ts !


    configureCustomObjects(custom: ICustomComponentConfigurator) {


    }

    configureCustomLists(custom: ICustomComponentConfigurator) {


    }

    configureCustomErrors(custom: ICustomErrorComponentConfigurator) {


    }

}
