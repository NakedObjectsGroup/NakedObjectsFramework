import { Component, Input } from '@angular/core';
import * as Models from "./models";
import { Observable } from 'rxjs/Observable';
import * as _ from "lodash";
import { ActionComponent } from "./action.component";
import * as Viewmodelfactoryservice from "./view-model-factory.service";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Urlmanagerservice from "./urlmanager.service";


@Component({
    selector: 'actions',
    templateUrl: 'app/actions.component.html',
    directives: [ActionsComponent, ActionComponent]
})

export class ActionsComponent {

    constructor(private viewModelFactory: Viewmodelfactoryservice.ViewModelFactory, private urlManager : Urlmanagerservice.UrlManager) {   }

    @Input()
    menuVm : {menuItems : Nakedobjectsviewmodels.IMenuItemViewModel[]};
}