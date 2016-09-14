import { Component, Input } from '@angular/core';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import { PropertyComponent } from "./property.component";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import * as Objectcomponent from './object.component';

@Component({
    selector: 'parameters',
    templateUrl: 'app/parameters.component.html',
    directives: [PropertyComponent],
    styleUrls: ['app/parameters.component.css']
})

export class ParametersComponent {

    constructor(private viewModelFactory: ViewModelFactory, private urlManager: UrlManager) { }

    parms: ViewModels.ParameterViewModel[];

    @Input()
    edit: boolean;

    @Input()
    parent: ViewModels.DialogViewModel;

    @Input()
    set parameters(value: ViewModels.ParameterViewModel[]) {
        this.parms = value;
    }

    get parameters() {
        return this.parms;
    }
}