import { ContextService } from '../context.service';
import { Component, Input } from '@angular/core';
import { FieldViewModel } from '../view-models/field-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { FormGroup, AbstractControl } from '@angular/forms';
import { Dictionary } from 'lodash';
import { BehaviorSubject } from 'rxjs';
import { accept, dropOn, paste } from '../helpers-components';

@Component({
    selector: 'nof-auto-complete-facade',
    template: require('./auto-complete-facade.component.html'),
    styles: [require('./auto-complete-facade.component.css')]
})
export class AutoCompleteFacadeComponent {

    constructor(private readonly context: ContextService) { }

    private viewModel: FieldViewModel;

    @Input()
    set model(m: FieldViewModel) {
        this.viewModel = m;
    }

    @Input()
    form: FormGroup;

    get model() {
        return this.viewModel;
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

    get choices(): ChoiceViewModel[] {
        return this.model.choices;
    }

    canDrop = false;

    accept(droppableVm: FieldViewModel) {
        return accept(droppableVm, this);
    };

    drop(draggableVm: IDraggableViewModel) {
        dropOn(draggableVm, this.model, this);
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

    paste(event: KeyboardEvent) {
        paste(event, this.model, this, () => this.context.getCopyViewModel(), () => this.context.setCopyViewModel(null));
    }

    clear() {
        this.model.clear();
        this.control.reset();
    }

    private bSubject: BehaviorSubject<any>;

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.control.value;
            this.bSubject = new BehaviorSubject(initialValue);

            this.control.valueChanges.subscribe((data) => {
                this.bSubject.next(data);
            });
        }

        return this.bSubject;
    }
}
