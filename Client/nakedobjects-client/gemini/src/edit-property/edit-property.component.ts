import {
    AfterViewInit,
    Component,
    ElementRef,
    HostListener,
    Input,
    OnDestroy,
    OnInit,
    QueryList,
    Renderer2,
    ViewChildren
    } from '@angular/core';
import { FormGroup } from '@angular/forms';
import * as Ro from '@nakedobjects/restful-objects';
import { LoggerService } from '@nakedobjects/services';
import { AttachmentViewModel, ChoiceViewModel, DomainObjectViewModel, DragAndDropService, PropertyViewModel } from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { FieldComponent } from '../field/field.component';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';

@Component({
    selector: 'nof-edit-property',
    templateUrl: 'edit-property.component.html',
    styleUrls: ['edit-property.component.css']
})
export class EditPropertyComponent extends FieldComponent implements OnInit, OnDestroy, AfterViewInit {

    constructor(
        loggerService: LoggerService,
        renderer: Renderer2,
        dragAndDrop: DragAndDropService,
    ) {
        super(loggerService, renderer, dragAndDrop);
    }

    private prop: PropertyViewModel;

    @ViewChildren('focus')
    focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    @ViewChildren('checkbox')
    checkboxList: QueryList<ElementRef>;

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    set property(value: PropertyViewModel) {
        this.prop = value;
        if (this.propertyEntryType === Ro.EntryType.FreeForm) {
            this.dragAndDrop.setDropZoneId(this.propertyPaneId);
        }
    }

    get property() {
        return this.prop;
    }

    get propertyPaneId() {
        return this.property.paneArgId;
    }

    get propertyId() {
        return this.property.id;
    }

    get propertyChoices() {
        return this.property.choices;
    }

    get title() {
        return this.property.title;
    }

    get propertyType() {
        return this.property.type;
    }

    get propertyReturnType() {
        return this.property.returnType;
    }

    get propertyEntryType(): Ro.EntryType {
        return this.property.entryType;
    }

    get isEditable() {
        return this.property.isEditable;
    }

    get formattedValue() {
        return this.property.formattedValue;
    }

    get value() {
        return this.property.formattedValue;
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

    get isPassword() {
        return this.property.password;
    }

    get multilineHeight() {
        return `${this.property.multipleLines * 20}px`;
    }

    get rows() {
        return this.property.multipleLines;
    }

    get propertyDescription() {
        return this.property.description;
    }

    get message() {
        return this.property.getMessage();
    }

    get attachment(): AttachmentViewModel | null {
        return this.property.attachment;
    }

    choiceName(choice: ChoiceViewModel) {
        return choice.name;
    }

    classes(): Dictionary<boolean | null> {
        return {
            [this.prop.color]: true,
            'candrop': this.canDrop,
            'dnd-drag-over': this.dragOver,
        };
    }

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    get form() {
        return this.formGroup;
    }

    ngOnInit(): void {
        super.init(this.parent, this.property, this.form.controls[this.prop.id]);
    }

    @HostListener('keydown', ['$event'])
    onKeydown(event: KeyboardEvent) {
        this.handleKeyEvents(event, this.isMultiline);
    }

    @HostListener('keypress', ['$event'])
    onKeypress(event: KeyboardEvent) {
        this.handleKeyEvents(event, this.isMultiline);
    }

    ngAfterViewInit() {
        this.populateBoolean();
    }

    ngOnDestroy(): void {
        this.dragAndDrop.clearDropZoneId(this.propertyPaneId);
        super.ngOnDestroy();
    }
}
