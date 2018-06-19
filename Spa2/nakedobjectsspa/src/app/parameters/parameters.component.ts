import { Component, Input, ViewChildren, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { EditParameterComponent } from '../edit-parameter/edit-parameter.component';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import some from 'lodash-es/some';

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
