import { Component, Input, OnInit, ElementRef, HostListener, ViewChildren, QueryList } from '@angular/core';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import * as Models from '../models';
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { ContextService } from '../context.service';
import { FieldViewModel } from '../view-models/field-view-model';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { ConfigService } from '../config.service';
import { LoggerService } from '../logger.service';

@Component({
    selector: 'nof-edit-parameter',
    host: {
        '(document:click)': 'handleClick($event)'
    },
    templateUrl: './edit-parameter.component.html',
    styleUrls: ['./edit-parameter.component.css']
})
export class EditParameterComponent extends FieldComponent implements OnInit {

    constructor(
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        myElement: ElementRef,
        context: ContextService,
        configService: ConfigService,
        loggerService : LoggerService
    ) {
        super(myElement, context, configService, loggerService);
    }

    parm: ParameterViewModel;

    @Input()
    parent: DialogViewModel;

    @Input()
    set parameter(value: ParameterViewModel) {
        this.parm = value;
        this.droppable = value as FieldViewModel;
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


    droppableClasses(): string {
        return `${this.parm.color}${this.canDrop ? " candrop" : ""}`;
    }

    ngOnInit(): void {
        super.init(this.parent, this.parameter, this.form.controls[this.parm.id]);
    }

    @Input()
    set form(fm: FormGroup) {
        this.formGroup = fm;
    }

    get form() {
        return this.formGroup;
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
    onEnter(event: KeyboardEvent) {
        this.paste(event);
    }

    @HostListener('keypress', ['$event'])
    onEnter1(event: KeyboardEvent) {
        this.paste(event);
    }

    @ViewChildren("focus")
    focusList: QueryList<ElementRef>;

}