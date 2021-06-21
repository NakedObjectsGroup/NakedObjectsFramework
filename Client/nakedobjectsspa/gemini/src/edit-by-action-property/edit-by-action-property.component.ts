import { Component, HostListener, Input, OnInit, OnDestroy } from '@angular/core';
import { copy, DragAndDropService, AttachmentViewModel, PropertyViewModel } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-edit-by-action-property',
    templateUrl: 'edit-by-action-property.component.html',
    styleUrls: ['edit-by-action-property.component.css']
})
export class EditByActionPropertyComponent implements OnInit, OnDestroy {

    private ddSub: ISubscription;

    constructor(
        private readonly dragAndDrop: DragAndDropService
    ) { }

    dropZones: string[] = [];

    // template inputs

    @Input()
    property: PropertyViewModel;

    // template listeners

    @HostListener('keydown', ['$event'])
    onEnter(event: KeyboardEvent) {
        this.copy(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.copy(event);
    }

    // template API

    get title() {
        return this.property.title;
    }

    get propertyType() {
        return this.property.type;
    }

    get propertyRefType() {
        return this.property.refType;
    }

    get propertyReturnType() {
        return this.property.returnType;
    }

    get formattedValue() {
        return this.property.formattedValue;
    }

    get value() {
        return this.property.value;
    }

    get format() {
        return this.property.format;
    }

    get isBlob() {
        return this.property.format === 'blob';
    }

    get isMultiline() {
        return !(this.property.multipleLines === 1);
    }

    get multilineHeight() {
        return `${this.property.multipleLines * 20}px`;
    }

    get color() {
        return this.property.color;
    }

    get attachment(): AttachmentViewModel | null {
        return this.property.attachment;
    }

    doClick = (right?: boolean) => this.property.doClick(right);

    copy(event: KeyboardEvent) {
        const prop = this.property;
        if (prop) {
            copy(event, prop, this.dragAndDrop);
        }
    }

    setDropZones(ids: string[]) {
        setTimeout(() => this.dropZones = ids);
    }

    ngOnInit(): void {
        this.ddSub = this.dragAndDrop.dropZoneIds$.subscribe(ids => this.setDropZones(ids || []));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.ddSub);
    }
}
