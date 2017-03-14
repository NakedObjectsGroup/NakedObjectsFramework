import { Component, Input, ViewChildren, QueryList, ChangeDetectorRef  } from '@angular/core';
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

    constructor(private readonly ref : ChangeDetectorRef) { }

    @Input()
    parent: DialogViewModel;

    @Input()
    form: FormGroup;

    @Input()
    parameters: ParameterViewModel[];

    @ViewChildren(EditParameterComponent)
    parmComponents: QueryList<EditParameterComponent>;

    // todo use proper classes syntax ! 
    classes = () =>  `parameter${this.parent.isMultiLineDialogRow ? " multilinedialog" : ""}`;

    focus() {
        const parms = this.parmComponents;
        if (parms && parms.length > 0) {
            // until first element returns true
            return _.some(parms.toArray(), i => i.focus());
        }
        return false;
    }

    // ngAfterViewInit(): void {
    //     // on MLD this causes problem with expression has changed error. 
    //     // we must be changeing page state (ng-untouched) from a change callback.  
    //     //if (!this.parent.isMultiLineDialogRow) {
    //        this.focusOnFirstAction(this.parmComponents);
    //        this.parmComponents.changes.subscribe((ql: QueryList<EditParameterComponent>) => this.focusOnFirstAction(ql));
    //     //}
    // }
}
