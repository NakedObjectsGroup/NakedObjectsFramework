import { Component, Input } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FormGroup }                 from '@angular/forms';

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
    form: FormGroup = null;

    @Input()
    set parameters(value: ViewModels.ParameterViewModel[]) {
        this.parms = value;
    }

    get parameters() {
        return this.parms;
    }
}