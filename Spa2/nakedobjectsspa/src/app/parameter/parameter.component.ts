import { Component, Input, OnInit, ElementRef, HostListener, ViewChildren, QueryList } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Observable } from 'rxjs/Observable';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { UrlManagerService } from "../url-manager.service";
import * as Models from "../models";
import * as ViewModels from "../view-models";
import { FieldComponent } from '../field/field.component';
import { FormGroup } from '@angular/forms';
import { ContextService } from "../context.service";
import { FieldViewModel } from '../view-models/field-view-model';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';

@Component({
    selector: 'parameter',
    host: {
        '(document:click)': 'handleClick($event)'
    },
    templateUrl: './parameter.component.html',
    styleUrls: ['./parameter.component.css']
})
export class ParameterComponent extends FieldComponent implements OnInit {

    constructor(private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        myElement: ElementRef,
        context: ContextService) {
        super(myElement, context);
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