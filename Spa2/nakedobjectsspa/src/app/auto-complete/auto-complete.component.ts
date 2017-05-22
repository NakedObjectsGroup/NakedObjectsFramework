import { Component, OnInit, Input } from '@angular/core';
import * as Fieldviewmodel from '../view-models/field-view-model';
import * as Choiceviewmodel from '../view-models/choice-view-model';
import * as Idraggableviewmodel from '../view-models/idraggable-view-model';
import { FormGroup, AbstractControl } from '@angular/forms';
import { Dictionary } from 'lodash';

@Component({
    selector: 'nof-auto-complete',
    template: require('./auto-complete.component.html'),
    styles: [require('./auto-complete.component.css')]
})
export class AutoCompleteComponent implements OnInit {

    // todo support cut and paste ! 

    private _model: Fieldviewmodel.FieldViewModel;

    @Input()
    set model(m: Fieldviewmodel.FieldViewModel) {
        this._model = m;
    }

    @Input()
    form: FormGroup;

    get model() {
        return this._model;
    }

    get modelPaneId() {
        return this.model.paneArgId;
    }

    get modelId() {
        return this.model.id;
    }

    get control(): AbstractControl {
        return this.form.controls[this.model.id];
    }

    get choices(): Choiceviewmodel.ChoiceViewModel[] {
        return this.model.choices;
    }

    // todo cloned from field component - move to helpers ?

    canDrop = false;

    accept(droppableVm: Fieldviewmodel.FieldViewModel) {

        return (draggableVm: Idraggableviewmodel.IDraggableViewModel) => {
            if (draggableVm) {
                draggableVm.canDropOn(droppableVm.returnType).then((canDrop: boolean) => this.canDrop = canDrop).catch(() => this.canDrop = false);
                return true;
            }
            return false;
        }
    };

    drop(draggableVm: Idraggableviewmodel.IDraggableViewModel) {
        if (this.canDrop) {
            this.model.drop(draggableVm)
                .then((success) => {
                    this.control.setValue(this.model.selectedChoice);
                });
        }
    }

    classes(): Dictionary<boolean | null> {
        return {
            [this.model.color]: true,
            "candrop": this.canDrop,
            "mat-input-element": null as boolean | null // remove this class to prevent angular/materials styling overiding our styling
        };
    }

    get description() {
        return this.model.description;
    }


    ngOnInit() {
    }

}
