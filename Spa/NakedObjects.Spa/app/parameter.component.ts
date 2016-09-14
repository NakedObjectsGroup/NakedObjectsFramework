import { Component, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Objectcomponent from './object.component';

@Component({
    selector: 'parameter',
    templateUrl: 'app/parameter.component.html',
    styleUrls: ['app/parameter.component.css']
})
export class ParameterComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) {}

    parm: ViewModels.ParameterViewModel;

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DialogViewModel;

    @Input()
    set parameter(value: ViewModels.ParameterViewModel) {
        this.parm = value;
    }

    get parameter() {
        return this.parm;
    }
}