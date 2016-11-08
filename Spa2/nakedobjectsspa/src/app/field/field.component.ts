import * as ViewModels from "../view-models";
import * as Models from "../models"
import * as Ro from '../ro-interfaces';
import { AbstractControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { ElementRef } from '@angular/core';
import * as _ from "lodash";


export class FieldComponent {

    //filteredList: ViewModels.ChoiceViewModel[] = [];
    elementRef: ElementRef;

    constructor(myElement: ElementRef) {
        this.elementRef = myElement;
    }

    private vmParent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;
    private model: ViewModels.ParameterViewModel | ViewModels.PropertyViewModel;
    private isConditionalChoices: boolean;
    private isAutoComplete: boolean;
    private control: AbstractControl;

    protected init(vmParent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel,
        vm: ViewModels.ParameterViewModel | ViewModels.PropertyViewModel,
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

    currentOptions: ViewModels.ChoiceViewModel[] = [];
    pArgs: _.Dictionary<Models.Value>;

    paneId: number;
    canDrop = false;

    droppable: ViewModels.IFieldViewModel;

    accept = (draggableVm: ViewModels.IDraggableViewModel) => {

        if (draggableVm) {
            draggableVm.canDropOn(this.droppable.returnType).
                then((canDrop: boolean) => this.canDrop = canDrop).
                catch(() => this.canDrop = false);
            return true;
        }
        return false;
    };

    drop(draggableVm: ViewModels.IDraggableViewModel) {
        if (this.canDrop) {
            this.droppable.drop(draggableVm);
        }
    }

    isDomainObjectViewModel(object: any): object is ViewModels.DomainObjectViewModel {
        return object && "properties" in object;
    }

    mapValues(args: _.Dictionary<Models.Value>, parmsOrProps: { argId: string, getValue: () => Models.Value }[]) {
        return _.mapValues(this.pArgs, (v, n) => {
            const pop = _.find(parmsOrProps, p => p.argId === n);
            return pop.getValue();
        });
    }

    populateArguments() {

        const dialog = this.vmParent as ViewModels.DialogViewModel;
        const object = this.vmParent as ViewModels.DomainObjectViewModel;

        if (!dialog && !object) {
            throw { message: "Expect dialog or object in geminiConditionalchoices", stack: "" };
        }

        let parmsOrProps: { argId: string, getValue: () => Models.Value }[] = [];

        if (this.isDomainObjectViewModel(object)) {
            parmsOrProps = object.properties;
        } else {
            parmsOrProps = dialog.parameters;
        }

        return this.mapValues(this.pArgs, parmsOrProps);
    }

    populateDropdown() {
        const nArgs = this.populateArguments();
        const prompts = this.model.conditionalChoices(nArgs);//  scope.select({ args: nArgs });
        prompts.then((cvms: ViewModels.ChoiceViewModel[]) => {
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
        }).catch(() => {
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
        }
        else if (this.isAutoComplete) {
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
        this._form.valueChanges.subscribe(data => this.onValueChanged(data));
        this.onValueChanged(); // (re)set validation messages now
    }

    get formGroup() {
        return this._form;
    }

    populateAutoComplete() {
        const input = this.control.value;

        if (input instanceof ViewModels.ChoiceViewModel) {
            return;
        }

        if (input.length > 0 && input.length >= this.model.minLength) {
            this.model.prompt(input).
                then((cvms: ViewModels.ChoiceViewModel[]) => {
                    if (cvms.length === this.currentOptions.length && _.every(cvms, (c, i) => c.equals(this.currentOptions[i]))) {
                        return;
                    }
                    this.model.choices = cvms;
                    this.currentOptions = cvms;
                    this.model.selectedChoice = null;
                }).
                catch(() => {
                    this.model.choices = [];
                    this.currentOptions = [];
                    this.model.selectedChoice = null;
                });
        }
        else {
            this.model.choices = [];
            this.currentOptions = [];
            this.model.selectedChoice = null;
        }
    }

    select(item: ViewModels.ChoiceViewModel) {
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

        const element =  document.querySelector("input[type='file']");
        const file = (element as any).files[0] as File;

        const fileReader = new FileReader();
        fileReader.onloadend = () => {
            const link = new Models.Link({
                href: fileReader.result,
                type: file.type,
                title: file.name
            } as Ro.ILink);

            this.control.reset(link);
            this.model.file = link;
        };

        fileReader.readAsDataURL(file);
    }
}