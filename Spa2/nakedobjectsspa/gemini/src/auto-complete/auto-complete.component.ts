import { CdkDrag, CdkDragDrop, CdkDropList } from '@angular/cdk/drag-drop';
import { Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { ChoiceViewModel, DragAndDropService, FieldViewModel, IDraggableViewModel } from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import { BehaviorSubject, SubscriptionLike as ISubscription } from 'rxjs';
import { accept, dropOn, focus, paste, safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-auto-complete',
    templateUrl: 'auto-complete.component.html',
    styleUrls: ['auto-complete.component.css']
})
export class AutoCompleteComponent implements OnDestroy {

    constructor(
        private readonly dragAndDrop: DragAndDropService
    ) { }

    private fieldViewModel: FieldViewModel;
    private bSubject: BehaviorSubject<any>;
    private sub: ISubscription;
    private currentIndex = -1;
    @ViewChild('focus')
    inputField: ElementRef;
    canDrop = false;
    dragOver = false;

    @Input()
    set model(m: FieldViewModel) {
        this.fieldViewModel = m;
        this.dragAndDrop.setDropZoneId(this.modelPaneId);
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

    get accept() {
        const _this = this;
        return (cdkDrag: CdkDrag<IDraggableViewModel>, cdkDropList: CdkDropList) => {
            return accept(_this.model, _this, cdkDrag.data);
        };
    }

    drop(event: CdkDragDrop<CdkDrag<IDraggableViewModel>>) {
        const cdkDrag: CdkDrag<IDraggableViewModel> = event.item;
        if (event.isPointerOverContainer) {
            dropOn(cdkDrag.data, this.model, this);
        }
        this.canDrop = false;
        this.dragOver = false;
    }

    exit() {
        this.canDrop = false;
        this.dragOver = false;
    }

    enter() {
        this.dragOver = true;
    }

    classes(): Dictionary<boolean | null> {
        return {
            [this.model.color]: true,
            'candrop': this.canDrop,
            'dnd-drag-over': this.dragOver,
        };
    }

    get description() {
        return this.model.description;
    }

    paste(event: KeyboardEvent) {
        paste(event, this.model, this, () => this.dragAndDrop.getCopyViewModel(), () => this.dragAndDrop.setCopyViewModel(null));
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
        return { 'selected': i === this.currentIndex };
    }

    onArrowUp() {
        this.currentIndex--;
        this.currentIndex = this.currentIndex < -1 ? -1 : this.currentIndex;
        return false;
    }

    onArrowDown() {
        this.currentIndex++;
        const maxIndex = this.choices.length - 1;
        this.currentIndex = this.currentIndex > maxIndex ? maxIndex : this.currentIndex;
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
        this.dragAndDrop.clearDropZoneId(this.modelPaneId);
        safeUnsubscribe(this.sub);
    }

    focus() {
        return focus(this.inputField);
    }
}
