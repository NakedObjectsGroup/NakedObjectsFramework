import { AfterViewInit, Component, Input, OnDestroy, OnChanges, ViewChildren, QueryList } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ColorService, ConfigService, ContextService, ErrorService } from '@nakedobjects/services';
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
import { IActionHolder } from '../action/action.component';

@Component({
    selector: 'nof-create-new-dialog',
    templateUrl: 'create-new-dialog.component.html',
    styleUrls: ['create-new-dialog.component.css']
})
export class CreateNewDialogComponent extends BaseDialogComponent implements AfterViewInit, OnDestroy, OnChanges {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        error: ErrorService,
        context: ContextService,
        private readonly colorService: ColorService,
        private readonly configService: ConfigService,
        formBuilder: FormBuilder) {
            super(viewModelFactory, error, context, formBuilder);
            this.pendingColor = `${configService.config.objectColor}${colorService.getConfiguredDefault()}`;
    }

    @ViewChildren(EditParameterComponent)
    parmComponents: QueryList<EditParameterComponent>;

    @Input()
    set toCreateClass(cls: string) {
        this.toCreate = cls;
        this.colorService.toColorNumberFromType(cls).then(c => this.pendingColor = `${this.configService.config.objectColor}${c}`);
    }

    get title() {
        const toCreateSplit = this.toCreate.split('.');
        const toCreateName = toCreateSplit[toCreateSplit.length - 1];
        return `Creating New ${toCreateName}`;
    }

     // used to smooth transition before object set
    private pendingColor: string;

    get color() {
         return this.pendingColor;
    }

    toCreate: string;

    private saveButton: IActionHolder = {
        value: 'Save',
        doClick: () => this.onSubmit(),
        doRightClick: () => this.onSubmit(true),
        show: () => true,
        disabled: () => this.form && !this.form.valid ? true : null,
        tempDisabled: () => null,
        title: () => this.tooltip,
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };

    private cancelButton: IActionHolder = {
        value: 'Cancel',
        doClick: () => this.close(),
        show: () => true,
        disabled: () => null,
        tempDisabled: () => null,
        title: () => '',
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };

    private saveButtons = [this.saveButton, this.cancelButton];

    get actionHolders() {
        return this.saveButtons;
    }

    get parametersProperties(): (ParameterViewModel | string)[] {
        const properties = this.dialog.actionViewModel.createNewProperties;
        const parmMap = fromPairs(this.parameters.map((p) => [p.title.toLowerCase(), p]));
        return map(properties, (p) => parmMap[p.toLowerCase()] ?? p);
    }

    isParameter(parmprop: PropertyViewModel | string) {
        return parmprop instanceof ParameterViewModel;
    }

    isProperty(parmprop: PropertyViewModel | string) {
        return !this.isParameter(parmprop);
    }

    private hasHint(parm: ParameterViewModel) {
        return parm?.presentationHint !== null && parm.presentationHint !== undefined;
    }

    classes(parmprop: ParameterViewModel) {
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
