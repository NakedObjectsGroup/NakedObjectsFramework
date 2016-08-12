import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as _ from "lodash";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";

@Component({
    selector: 'dialog',
    templateUrl: 'app/dialog.component.html'
})
export class DialogComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {}

    @Input()
    dialog: ViewModels.DialogViewModel;
}