import { Component, Input, QueryList, ViewChildren } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DialogViewModel, ParameterViewModel } from '@nakedobjects/view-models';
import some from 'lodash-es/some';
import { EditParameterComponent } from '../edit-parameter/edit-parameter.component';

@Component({
    selector: 'nof-parameters',
    templateUrl: 'parameters.component.html',
    styleUrls: ['parameters.component.css']
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

    classes = () => ({ parameter: true, multilinedialog: this.parent.isMultiLineDialogRow });

    focus() {
        const parms = this.parmComponents;
        if (parms && parms.length > 0) {
            // until first element returns true
            return some(parms.toArray(), i => i.focus());
        }
        return false;
    }
}
