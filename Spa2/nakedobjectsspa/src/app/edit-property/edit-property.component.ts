import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { Component, Input, ElementRef, OnInit, HostListener, ViewChildren, QueryList, Renderer, AfterViewInit } from '@angular/core';
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorService } from '../error.service';
import { ContextService } from '../context.service';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { ConfigService } from '../config.service';
import { LoggerService } from '../logger.service';
import * as Models from '../models';
import { AttachmentViewModel } from '../view-models/attachment-view-model';
import { Dictionary } from 'lodash';

@Component({
    selector: 'nof-edit-property',
    templateUrl: 'edit-property.component.html',
    styleUrls: ['edit-property.component.css']
})
export class EditPropertyComponent extends FieldComponent implements OnInit, AfterViewInit {

    constructor(
        private readonly router: Router,
        private readonly error: ErrorService,
        context: ContextService,
        configService: ConfigService,
        loggerService: LoggerService,
        renderer: Renderer
    ) {
        super(context, configService, loggerService, renderer);
    }

    private prop: PropertyViewModel;

    @ViewChildren("focus")
    focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    @ViewChildren("checkbox")
    checkboxList: QueryList<ElementRef>;

    @Input()
    parent: DomainObjectViewModel;

    @Input()
    set property(value: PropertyViewModel) {
        this.prop = value;
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

    get propertyEntryType(): Models.EntryType {
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
        return this.property.format === "blob";
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
            "candrop": this.canDrop
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
}
