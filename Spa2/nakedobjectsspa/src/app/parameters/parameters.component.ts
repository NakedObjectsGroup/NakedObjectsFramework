import { Component, Input, ViewChildren, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ParameterComponent } from "../parameter/parameter.component";
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import * as Models from "../models";

@Component({
    selector: 'parameters',
    templateUrl: './parameters.component.html',
    styleUrls: ['./parameters.component.css']
})
export class ParametersComponent {

    @Input()
    parent: DialogViewModel;

    @Input()
    form: FormGroup;

    @Input()
    parameters: ParameterViewModel[];

    @ViewChildren(ParameterComponent)
    parmComponents: QueryList<ParameterComponent>;

    focusOnFirstAction(parms: QueryList<ParameterComponent>) {
        if (parms && parms.first) {
            parms.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.parmComponents);
        this.parmComponents.changes.subscribe((ql: QueryList<ParameterComponent>) => this.focusOnFirstAction(ql));
    }
}