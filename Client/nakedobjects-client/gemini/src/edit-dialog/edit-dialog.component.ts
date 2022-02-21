import { AfterViewInit, Component, Input, OnDestroy, OnChanges, ViewChildren, QueryList } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ContextService, ErrorService } from '@nakedobjects/services';
import {
    DomainObjectViewModel,
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

    @Input()
    set parentObject(obj : DomainObjectViewModel) {
        this.parent = obj;
    }

    get parentObject() {
        return this.parent as DomainObjectViewModel;
    }

    get parametersProperties(): (ParameterViewModel | PropertyViewModel)[] {
        const parmMap = fromPairs(this.parameters.map((p) => [p.id.toLowerCase(), p]));
        return map(this.properties, (p) => parmMap[p.id.toLowerCase()] ?? p);
    }

    isParameter(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop instanceof ParameterViewModel;
    }

    asParameter(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop as ParameterViewModel;
    }

    isLastParameter(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop === this.parameters[this.parameters.length - 1];
    }

    isProperty(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop instanceof PropertyViewModel;
    }

    asProperty(parmprop: PropertyViewModel | ParameterViewModel) {
        return parmprop as PropertyViewModel;
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

    private doNextEditByAction(i: number) {
        const property = this.properties[i];
        if (property.isEditByAction && !property.isEditActionDisabled) {
            property.doEditByAction();
            return true;
        }
        return false;
    }

    onSubmitNext(right?: boolean) {
        this.onSubmit(right);

        const merged = this.parametersProperties;
        const lastParameter = this.parameters[this.parameters.length - 1];
        const lastParameterIndex = merged.indexOf(lastParameter);
        const nextMergedIndex = lastParameterIndex + 1;
        const nextProperty = (nextMergedIndex > merged.length - 1) ? this.properties[0] : merged[nextMergedIndex] as PropertyViewModel;
        const nextPropertyIndex = this.properties.indexOf(nextProperty);

        for (let i = nextPropertyIndex; i < this.properties.length; i++) {
            if (this.doNextEditByAction(i)) {
                return;
            }
        }

        for (let i = 0; i < nextPropertyIndex; i++) {
            if (this.doNextEditByAction(i)) {
                return;
            }
        }
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
