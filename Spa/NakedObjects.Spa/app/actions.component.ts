import { Component, Input } from '@angular/core';
import * as Models from "./models";
import { Observable } from 'rxjs/Observable';
import * as _ from "lodash";
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import * as ViewModels from "./nakedobjects.viewmodels";
import { UrlManager } from "./urlmanager.service";


@Component({
    selector: 'actions',
    templateUrl: 'app/actions.component.html',
    directives: [ActionsComponent, ActionComponent]
})
export class ActionsComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {}

    @Input()
    menuVm: { menuItems: ViewModels.IMenuItemViewModel[] };
}