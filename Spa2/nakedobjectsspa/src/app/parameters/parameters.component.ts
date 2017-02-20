import { Component, Input, ViewChildren, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { EditParameterComponent } from '../edit-parameter/edit-parameter.component';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import * as Models from '../models';

@Component({
    selector: 'nof-parameters',
    template: require('./parameters.component.html'),
    styles: [require('./parameters.component.css')]
})
export class ParametersComponent {

    @Input()
    parent: DialogViewModel;

    @Input()
    form: FormGroup;

    @Input()
    parameters: ParameterViewModel[];

    @ViewChildren(EditParameterComponent)
    parmComponents: QueryList<EditParameterComponent>;

    classes = () =>  `parameter${this.parent.isMultiLineDialogRow ? " multilinedialog" : ""}`;

    focusOnFirstAction(parms: QueryList<EditParameterComponent>) {
        if (parms && parms.first) {
            parms.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.parmComponents);
        this.parmComponents.changes.subscribe((ql: QueryList<EditParameterComponent>) => this.focusOnFirstAction(ql));
    }
}
