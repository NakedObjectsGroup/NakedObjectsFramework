import { ContextService } from '../context.service';
import { Component, Input, OnDestroy, ElementRef, ViewChild, Renderer } from '@angular/core';
import { FieldViewModel } from '../view-models/field-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { FormGroup, AbstractControl } from '@angular/forms';
import { SubscriptionLike as ISubscription ,  BehaviorSubject } from 'rxjs';
import { Dictionary } from 'lodash';
import { safeUnsubscribe, accept, dropOn, paste, focus } from '../helpers-components';

@Component({
    selector: 'nof-auto-complete',
    templateUrl: 'auto-complete.component.html',
    styleUrls: ['auto-complete.component.css']
})
export class AutoCompleteComponent implements OnDestroy {

    constructor(
        private readonly context: ContextService,
        private readonly renderer: Renderer
    ) { }

    private fieldViewModel: FieldViewModel;
    private bSubject: BehaviorSubject<any>;
    private sub: ISubscription;
    private currentIndex = -1;
    @ViewChild("focus")
    inputField: ElementRef;

    @Input()
    set model(m: FieldViewModel) {
        this.fieldViewModel = m;
    }

    @Input()
    form: FormGroup;

    get model() {
        return this.fieldViewModel;
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
    }

    drop(draggableVm: IDraggableViewModel) {
        dropOn(draggableVm, this.model, this);
    }

    classes(): Dictionary<boolean | null> {
        return {
            [this.model.color]: true,
            "candrop": this.canDrop
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

    select(item: ChoiceViewModel) {
        this.model.choices = [];
        this.model.selectedChoice = item;
        this.control.reset(item);
    }

    choiceName = (choice: ChoiceViewModel) => choice.name;

    get subject() {
        if (!this.bSubject) {
            const initialValue = this.control.value;
            this.bSubject = new BehaviorSubject(initialValue);

            this.sub = this.control.valueChanges.subscribe((data) => {
                this.bSubject.next(data);
                this.currentIndex = -1;
            });
        }

        return this.bSubject;
    }

    isSelected(i: number) {
        return {"selected" : i === this.currentIndex};
    }

    onArrowUp() {
        this.currentIndex--;
        this.currentIndex = this.currentIndex < -1 ? -1 : this.currentIndex;
        return false;
    }

    onArrowDown() {
        this.currentIndex++;
        const maxIndex = this.choices.length - 1;
        this.currentIndex = this.currentIndex > maxIndex  ? maxIndex : this.currentIndex;
        return false;
    }

    selectCurrent() {
        const maxIndex = this.choices.length - 1;
        if (this.currentIndex >= 0 && this.currentIndex <= maxIndex) {
            this.select(this.choices[this.currentIndex]);
            return false;
        }
        return true;
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.sub);
    }

    focus() {
        return focus(this.renderer, this.inputField);
    }
}
