import { AfterViewInit, Component, OnDestroy, QueryList, ViewChildren, OnChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ContextService, ErrorService } from '@nakedobjects/services';
import {
    ViewModelFactoryService
} from '@nakedobjects/view-models';
import { BaseDialogComponent } from '../base-dialog/base-dialog.component';
import some from 'lodash-es/some';
import { ParametersComponent } from '../parameters/parameters.component';

@Component({
    selector: 'nof-dialog',
    templateUrl: 'dialog.component.html',
    styleUrls: ['dialog.component.css']
})
export class DialogComponent extends BaseDialogComponent  implements AfterViewInit, OnDestroy, OnChanges {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        error: ErrorService,
        context: ContextService,
        formBuilder: FormBuilder) {
            super(viewModelFactory, error, context, formBuilder);
    }

    @ViewChildren(ParametersComponent)
    parmComponents: QueryList<ParametersComponent>;

    focus(parms: QueryList<ParametersComponent>) {
        if (parms && parms.length > 0) {
            some(parms.toArray(), p => p.focus());
        }
    }

    ngAfterViewInit(): void {
        this.sub = this.parmComponents.changes.subscribe(ql => this.focus(ql));
    }
}
