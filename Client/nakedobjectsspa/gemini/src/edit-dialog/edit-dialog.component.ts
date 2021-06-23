import { AfterViewInit, Component, Input, OnDestroy, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ContextService, ErrorService, ErrorWrapper } from '@nakedobjects/services';
import {
    PropertyViewModel,
    ViewModelFactoryService
} from '@nakedobjects/view-models';
import { DialogComponent } from '../dialog/dialog.component';

@Component({
    selector: 'nof-edit-dialog',
    templateUrl: 'edit-dialog.component.html',
    styleUrls: ['edit-dialog.component.css']
})
export class EditDialogComponent  extends DialogComponent implements AfterViewInit, OnDestroy, OnChanges {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        error: ErrorService,
        context: ContextService,
        formBuilder: FormBuilder) {
            super(viewModelFactory, error, context, formBuilder);
    }

    @Input()
    properties: PropertyViewModel[];
}
