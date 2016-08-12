import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as _ from "lodash";

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