import { AutoCompleteComponent } from '../auto-complete/auto-complete.component';
import { TimePickerFacadeComponent } from '../time-picker-facade/time-picker-facade.component';
import { DatePickerFacadeComponent } from '../date-picker-facade/date-picker-facade.component';
import { Component, Input, OnInit, ElementRef, HostListener, ViewChildren, QueryList, Renderer, AfterViewInit } from '@angular/core';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import * as Models from '../models';
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { ContextService } from '../context.service';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { ConfigService } from '../config.service';
import { LoggerService } from '../logger.service';
import { Dictionary } from 'lodash';

@Component({
    selector: 'nof-edit-parameter',
    templateUrl: 'edit-parameter.component.html',
    styleUrls: ['edit-parameter.component.css']
})
export class EditParameterComponent extends FieldComponent implements OnInit, AfterViewInit {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        context: ContextService,
        configService: ConfigService,
        loggerService: LoggerService,
        renderer: Renderer
    ) {
        super(context, configService, loggerService, renderer);
    }

    parm: ParameterViewModel;

    @ViewChildren("focus")
    focusList: QueryList<ElementRef | DatePickerFacadeComponent | TimePickerFacadeComponent | AutoCompleteComponent>;

    @ViewChildren("checkbox")
    checkboxList: QueryList<ElementRef>;

    @Input()
    parent: DialogViewModel;

    @Input()
    set parameter(value: ParameterViewModel) {
        this.parm = value;
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

    choiceName = (choice: ChoiceViewModel) => choice.name;

    classes(): Dictionary<boolean | null> {
        return {
            [this.parm.color]: true,
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
        super.init(this.parent, this.parameter, this.form.controls[this.parm.id]);
    }

    isChoices() {
        return this.parm.entryType === Models.EntryType.Choices ||
            this.parm.entryType === Models.EntryType.ConditionalChoices ||
            this.parm.entryType === Models.EntryType.MultipleChoices ||
            this.parm.entryType === Models.EntryType.MultipleConditionalChoices;
    }

    isMultiple() {
        return this.parm.entryType === Models.EntryType.MultipleChoices ||
            this.parm.entryType === Models.EntryType.MultipleConditionalChoices;
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
}
