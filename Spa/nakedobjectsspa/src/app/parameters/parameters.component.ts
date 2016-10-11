import { Component, Input } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";

@Component({
    selector: 'parameters',
    templateUrl: './parameters.component.html',
    styleUrls: ['./parameters.component.css']
})

export class ParametersComponent {

    parms: ViewModels.ParameterViewModel[];

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