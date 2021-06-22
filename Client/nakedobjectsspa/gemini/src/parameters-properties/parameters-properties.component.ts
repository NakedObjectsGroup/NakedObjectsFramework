import { Component, Input, QueryList, ViewChildren } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DialogViewModel, ParameterViewModel, PropertyViewModel } from '@nakedobjects/view-models';
import some from 'lodash-es/some';
import map from 'lodash-es/map';
import fromPairs from 'lodash-es/fromPairs';
import { EditParameterComponent } from '../edit-parameter/edit-parameter.component';

@Component({
    selector: 'nof-parameters-properties',
    templateUrl: 'parameters-properties.component.html',
    styleUrls: ['parameters-properties.component.css']
})
export class ParametersPropertiesComponent {

    @Input()
    parent: DialogViewModel;

    @Input()
    form: FormGroup;

    @Input()
    parameters: ParameterViewModel[];

    @Input()
    properties: PropertyViewModel[];

    @ViewChildren(EditParameterComponent)
    parmComponents: QueryList<EditParameterComponent>;

    private merged: (ParameterViewModel | PropertyViewModel)[];

    get parametersProperties(): (ParameterViewModel | PropertyViewModel)[] {
        if (!this.merged) {
            const parmMap = fromPairs(this.parameters.map((p) => [p.id.toLowerCase(), p]));
            this.merged = map(this.properties, (p) => parmMap[p.id.toLowerCase()] ?? p);
        }
        return this.merged;
    }

    isParameter(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop instanceof ParameterViewModel;
    }

    isProperty(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop instanceof PropertyViewModel;
    }

    private hasHint(parm: ParameterViewModel) {
        return parm?.presentationHint !== null && parm.presentationHint !== undefined;
    }

    classes(parmprop: PropertyViewModel | ParameterViewModel) {

        if (parmprop instanceof PropertyViewModel) {
            const hint = parmprop.presentationHint ?? '';
            return `property ${hint}`.trim();
        }
        return ({ parameter: true, multilinedialog: this.parent.isMultiLineDialogRow, [parmprop.presentationHint]: this.hasHint(parmprop)});
    }

    focus() {
        const parms = this.parmComponents;
        if (parms && parms.length > 0) {
            // until first element returns true
            return some(parms.toArray(), i => i.focus());
        }
        return false;
    }
}
