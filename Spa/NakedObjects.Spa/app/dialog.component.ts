import { Component, Input } from '@angular/core';
import * as Models from "./models";
import { Observable } from 'rxjs/Observable';
import {NgClass} from '@angular/common';
import * as _ from "lodash";
import { ActionComponent } from "./action.component";
import * as Viewmodelfactoryservice from "./view-model-factory.service";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Urlmanagerservice from "./urlmanager.service";


@Component({
    selector: 'dialog',
    templateUrl: 'app/dialog.component.html',
    directives: [NgClass]
})

export class DialogComponent {

    constructor(private viewModelFactory: Viewmodelfactoryservice.ViewModelFactory, private urlManager: Urlmanagerservice.UrlManager) { }

    @Input()
    dialog: Nakedobjectsviewmodels.DialogViewModel;
}