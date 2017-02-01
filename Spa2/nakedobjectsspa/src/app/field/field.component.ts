import * as Models from "../models"
import * as Ro from '../ro-interfaces';
import { AbstractControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { ElementRef, HostListener, QueryList } from '@angular/core';
import * as _ from "lodash";
import { ContextService } from "../context.service";
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { FieldViewModel } from '../view-models/field-view-model';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import {ConfigService} from '../config.service';

export abstract class FieldComponent {

    //filteredList: ViewModels.ChoiceViewModel[] = [];
    elementRef: ElementRef;

    protected constructor(
        myElement: ElementRef,
        private readonly context: ContextService,
        private readonly configService : ConfigService
    ) {
        this.elementRef = myElement;
    }

    private vmParent: DialogViewModel | DomainObjectViewModel;
    private model: ParameterViewModel | PropertyViewModel;
    private isConditionalChoices: boolean;
    private isAutoComplete: boolean;
    private control: AbstractControl;

    protected init(vmParent: DialogViewModel | DomainObjectViewModel,
        vm: ParameterViewModel | PropertyViewModel,
        control: AbstractControl) {

        this.vmParent = vmParent;
        this.model = vm;
        this.control = control;

        this.paneId = this.model.onPaneId;

        this.isConditionalChoices = (this.model.entryType === Models.EntryType.ConditionalChoices ||
            this.model.entryType === Models.EntryType.MultipleConditionalChoices);

        this.isAutoComplete = this.model.entryType === Models.EntryType.AutoComplete;

        if (this.isConditionalChoices) {
            this.pArgs = _.omit(this.model.promptArguments, "x-ro-nof-members") as _.Dictionary<Models.Value>;
            this.populateDropdown();
        }
    }

    currentOptions: ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;
    canDrop = false;

    droppable: FieldViewModel;

    accept(droppableVm: FieldViewModel) {

        return (draggableVm: IDraggableViewModel) => {
            if (draggableVm) {
                draggableVm.canDropOn(droppableVm.returnType).then((canDrop: boolean) => this.canDrop = canDrop).catch(() => this.canDrop = false);
                return true;
            }
            return false;
        }
    };

    drop(draggableVm: IDraggableViewModel) {
        if (this.canDrop) {
            this.droppable.drop(draggableVm)
                .then((success) => {
                    this.control.setValue(this.model.selectedChoice);
                });
        }
    }

    isDomainObjectViewModel(object: any): object is DomainObjectViewModel {
        return object && "properties" in object;
    }

    mapValues(args: _.Dictionary<Models.Value>, parmsOrProps: { argId: string, getValue: () => Models.Value }[]) {
        return _.mapValues(this.pArgs,
            (v, n) => {
                const pop = _.find(parmsOrProps, p => p.argId === n);
                return pop!.getValue();
            });
    }

    populateArguments() {

        const dialog = this.vmParent as DialogViewModel;
        const object = this.vmParent as DomainObjectViewModel;

        if (!dialog && !object) {
            throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
        }

        let parmsOrProps: { argId: string, getValue: () => Models.Value }[];

        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        } else {
            parmsOrProps = dialog.parameters;
        }

        return this.mapValues(this.pArgs, parmsOrProps);
    }

    populateDropdown() {
        const nArgs = this.populateArguments();
        const prompts = this.model.conditionalChoices(nArgs); //  scope.select({ args: nArgs });
        prompts.then((cvms: ChoiceViewModel[]) => {
            // if unchanged return 
            if (cvms.length === this.currentOptions.length && _.every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                return;
            }
            this.model.choices = cvms;
            this.currentOptions = cvms;

            if (this.isConditionalChoices) {
                // need to reset control to find the selected options 
                if (this.model.entryType === Models.EntryType.MultipleConditionalChoices) {
                    this.control.reset(this.model.selectedMultiChoices);
                } else {
                    this.control.reset(this.model.selectedChoice);
                }
            }
        })
            .catch(() => {
                // error clear everything 
                this.model.selectedChoice = null;
                this.currentOptions = [];
            });
    }

    wrapReferences(val: string): string | Ro.ILink {
        if (val && this.model.type === "ref") {
            return { href: val };
        }
        return val;
    }

    onChange() {
        if (this.isConditionalChoices) {
            this.populateDropdown();
        } else if (this.isAutoComplete) {
            this.populateAutoComplete();
        }
    }

    get message() {
        return this.model.getMessage();
    }

    _form: FormGroup;

    onValueChanged(data?: any) {
        // clear previous error message (if any)
        //this.message = '';

        if (this.model) {
            const control = this._form.get(this.model.id);
            if (control && control.dirty && !control.valid) {
                //this.message = this.model.getMessage();
            }
            this.onChange();
        }
    }

    set formGroup(fm: FormGroup) {
        this._form = fm;
        this._form.valueChanges.subscribe((data: any) => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation messages now
    }

    get formGroup() {
        return this._form;
    }

    populateAutoComplete() {
        const input = this.control.value;

        if (input instanceof ChoiceViewModel) {
            return;
        }

        if (input && input.length > 0 && input.length >= this.model.minLength) {
            this.model.prompt(input)
                .then((cvms: ChoiceViewModel[]) => {
                    if (cvms.length === this.currentOptions.length && _.every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                        return;
                    }
                    this.model.choices = cvms;
                    this.currentOptions = cvms;
                    this.model.selectedChoice = null;
                })
                .catch(() => {
                    this.model.choices = [];
                    this.currentOptions = [];
                    this.model.selectedChoice = null;
                });
        } else {
            this.model.choices = [];
            this.currentOptions = [];
            this.model.selectedChoice = null;
        }
    }

    select(item: ChoiceViewModel) {
        this.model.choices = [];
        this.model.selectedChoice = item;
        this.control.reset(item);
    }

    private isInside(clickedComponent: any): boolean {
        if (clickedComponent) {
            return clickedComponent === this.elementRef.nativeElement || this.isInside(clickedComponent.parentNode);
        }
        return false;
    }

    handleClick(event: any) {
        if (this.isAutoComplete) {
            if (!this.isInside(event.target)) {
                this.model.choices = [];
            }
        }
    }

    fileUpload() {
        //const file = (this.elementRef[0] as any).files[0] as File;

        const element = document.querySelector("input[type='file']");
        const file = (element as any).files[0] as File;

        const fileReader = new FileReader();
        fileReader.onloadend = () => {
            const link = new Models.Link({
                href: fileReader.result,
                type: file.type,
                title: file.name
            });

            this.control.reset(link);
            this.model.file = link;
        };

        fileReader.readAsDataURL(file);
    }

    paste(event: any) {
        const vKeyCode = 86;
        const deleteKeyCode = 46;
        if (event && (event.keyCode === vKeyCode && event.ctrlKey)) {
            const cvm = this.context.getCutViewModel();

            if (cvm) {
                this.droppable.drop(cvm)
                    .then((success) => {
                        //this.control.reset(this.model.selectedChoice);
                        this.control.setValue(this.model.selectedChoice);
                    });
                event.preventDefault();
            }


        }
        if (event && event.keyCode === deleteKeyCode) {
            this.context.setCutViewModel(null);
        }
    }

    abstract focusList: QueryList<ElementRef>;

    focus() {
        if (this.focusList && this.focusList.first) {
            setTimeout(() => this.focusList.first.nativeElement.focus(), 0);
        }
    }

}