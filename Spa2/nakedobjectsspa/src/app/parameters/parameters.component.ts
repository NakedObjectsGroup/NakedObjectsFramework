import { Component, Input, ViewChildren, QueryList } from '@angular/core';
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FormGroup } from '@angular/forms';
import { ParameterComponent } from "../parameter/parameter.component";
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';

@Component({
    selector: 'parameters',
    templateUrl: './parameters.component.html',
    styleUrls: ['./parameters.component.css']
})
export class ParametersComponent {

    parms: ParameterViewModel[];

    @Input()
    parent: DialogViewModel;

    @Input()
    form: FormGroup = null;

    @Input()
    set parameters(value: ParameterViewModel[]) {
        this.parms = value;
    }

    get parameters() {
        return this.parms;
    }

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