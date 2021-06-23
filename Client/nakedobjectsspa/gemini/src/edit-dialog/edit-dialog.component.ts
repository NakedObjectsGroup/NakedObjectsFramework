import { AfterViewInit, Component, Input, OnDestroy, OnChanges, ViewChildren, QueryList } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ContextService, ErrorService } from '@nakedobjects/services';
import {
    ParameterViewModel,
    PropertyViewModel,
    ViewModelFactoryService
} from '@nakedobjects/view-models';
import { BaseDialogComponent } from '../base-dialog/base-dialog.component';
import some from 'lodash-es/some';
import map from 'lodash-es/map';
import fromPairs from 'lodash-es/fromPairs';
import { EditParameterComponent } from '../edit-parameter/edit-parameter.component';

@Component({
    selector: 'nof-edit-dialog',
    templateUrl: 'edit-dialog.component.html',
    styleUrls: ['edit-dialog.component.css']
})
export class EditDialogComponent  extends BaseDialogComponent implements AfterViewInit, OnDestroy, OnChanges {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        error: ErrorService,
        context: ContextService,
        formBuilder: FormBuilder) {
            super(viewModelFactory, error, context, formBuilder);
    }

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
        return ({ parameter: true, [parmprop.presentationHint]: this.hasHint(parmprop)});
    }

    ngAfterViewInit(): void {
        this.sub = this.parmComponents.changes.subscribe(() => this.focus());
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
