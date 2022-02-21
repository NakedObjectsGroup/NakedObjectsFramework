import {
    AfterViewInit,
    Component,
    ElementRef,
    HostListener,
    Input,
    OnInit,
    QueryList,
    Renderer2,
    ViewChildren,
    OnDestroy
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import * as Ro from '@nakedobjects/restful-objects';
import { LoggerService } from '@nakedobjects/services';
import { ChoiceViewModel, DialogViewModel, DomainObjectViewModel, DragAndDropService, MenuViewModel, ParameterViewModel } from '@nakedobjects/view-models';
import { Dictionary } from 'lodash';
import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { FieldComponent } from '../field/field.component';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';

@Component({
    selector: 'nof-edit-parameter',
    templateUrl: 'edit-parameter.component.html',
    styleUrls: ['edit-parameter.component.css']
})
export class EditParameterComponent extends FieldComponent implements OnInit, OnDestroy, AfterViewInit {

    constructor(
        loggerService: LoggerService,
        renderer: Renderer2,
        dragAndDrop: DragAndDropService
    ) {
        super(loggerService, renderer, dragAndDrop);
    }

    parm: ParameterViewModel;

    @ViewChildren('focus')
    focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    @ViewChildren('checkbox')
    checkboxList: QueryList<ElementRef>;

    @Input()
    parent: DialogViewModel | MenuViewModel | DomainObjectViewModel;

    @Input()
    set parameter(value: ParameterViewModel) {
        this.parm = value;
        if (this.parameterEntryType === Ro.EntryType.FreeForm) {
            this.dragAndDrop.setDropZoneId(this.parameterPaneId);
        }
    }

    get parameter() {
        return this.parm;
    }

    get parameterPaneId() {
        return this.parameter.paneArgId;
    }

    get title() {
        return this.parameter.title;
    }

    get parameterType() {
        return this.parameter.type;
    }

    get parameterEntryType() {
        return this.parameter.entryType;
    }

    get parameterReturnType() {
        return this.parameter.returnType;
    }

    get format() {
        return this.parameter.format;
    }

    get description() {
        return this.parameter.description;
    }

    get parameterId() {
        return this.parameter.id;
    }

    get choices() {
        return this.parameter.choices;
    }

    get isMultiline() {
        return !(this.parameter.multipleLines === 1);
    }

    get isPassword() {
        return this.parameter.password;
    }

    get multilineHeight() {
        return `${this.parameter.multipleLines * 20}px`;
    }

    get rows() {
        return this.parameter.multipleLines;
    }

    get isEditable() {
        return this.parameter.isEditable;
    }

    get formattedValue() {
        return this.parameter.formattedValue;
    }

    choiceName = (choice: ChoiceViewModel) => choice.name;

    classes(): Dictionary<boolean | null> {
        return {
            [this.parm.color]: true,
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
        super.init(this.parent, this.parameter, this.form.controls[this.parm.id]);
    }

    isChoices() {
        return this.parm.entryType === Ro.EntryType.Choices ||
            this.parm.entryType === Ro.EntryType.ConditionalChoices ||
            this.parm.entryType === Ro.EntryType.MultipleChoices ||
            this.parm.entryType === Ro.EntryType.MultipleConditionalChoices;
    }

    isMultiple() {
        return this.parm.entryType === Ro.EntryType.MultipleChoices ||
            this.parm.entryType === Ro.EntryType.MultipleConditionalChoices;
    }

    @HostListener('keydown', ['$event'])
    onKeydown(event: KeyboardEvent) {
        this.handleKeyEvents(event, this.isMultiline);
    }

    @HostListener('keypress', ['$event'])
    onKeypress(event: KeyboardEvent) {
        this.handleKeyEvents(event, this.isMultiline);
    }

    @HostListener('click', ['$event'])
    onClick(event: KeyboardEvent) {
        this.handleClick(event);
    }

    ngAfterViewInit() {
        this.populateBoolean();
    }

    ngOnDestroy(): void {
        this.dragAndDrop.clearDropZoneId(this.parameterPaneId);
        super.ngOnDestroy();
    }
}
