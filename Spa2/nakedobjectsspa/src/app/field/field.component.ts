import * as Models from '../models'
import * as Ro from '../ro-interfaces';
import { AbstractControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { ElementRef, HostListener, QueryList, Renderer } from '@angular/core';
import * as _ from 'lodash';
import { ContextService } from '../context.service';
import { ChoiceViewModel } from '../view-models/choice-view-model';
import { IDraggableViewModel } from '../view-models/idraggable-view-model';
import { FieldViewModel } from '../view-models/field-view-model';
import { ParameterViewModel } from '../view-models/parameter-view-model';
import { DialogViewModel } from '../view-models/dialog-view-model';
import { PropertyViewModel } from '../view-models/property-view-model';
import { DomainObjectViewModel } from '../view-models/domain-object-view-model';
import { ConfigService } from '../config.service';
import { LoggerService } from '../logger.service';
import * as Helpers from '../view-models/helpers-view-models';

export abstract class FieldComponent {

    elementRef: ElementRef;

    protected constructor(
        myElement: ElementRef,
        private readonly context: ContextService,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService,
        private readonly renderer : Renderer
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
            this.loggerService.throw("FieldComponent:populateArguments Expect dialog or object");
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
        prompts.
            then((cvms: ChoiceViewModel[]) => {
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
            }).
            catch(() => {
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

    private formGrp: FormGroup;

    onValueChanged() {
        if (this.model) {
            this.onChange();
        }
    }

    set formGroup(fm: FormGroup) {
        this.formGrp = fm;
        this.formGrp.valueChanges.subscribe((data) => this.onValueChanged());
        this.onValueChanged(); // (re)set validation messages now
    }

    get formGroup() {
        return this.formGrp;
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

    fileUpload(evt : Event) {
       
        const file : File = (evt.target as HTMLInputElement)!.files![0];
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

    paste(event: KeyboardEvent) {
        const vKeyCode = 86;
        const deleteKeyCode = 46;
        if (event && (event.keyCode === vKeyCode && event.ctrlKey)) {
            const cvm = this.context.getCopyViewModel();

            if (cvm) {
                this.droppable.drop(cvm)
                    .then((success) => {
                        this.control.setValue(this.model.selectedChoice);
                    });
                event.preventDefault();
            }


        }
        if (event && event.keyCode === deleteKeyCode) {
            this.context.setCopyViewModel(null);
        }
    }

    filterEnter(event: KeyboardEvent) {
        const enterKeyCode = 13;
        if (event && event.keyCode === enterKeyCode) {
            event.preventDefault();
        }
    }

    abstract focusList: QueryList<ElementRef>;

    focus() {
        return !!(this.focusList && this.focusList.first) && Helpers.focus(this.renderer, this.focusList.first);
    }

}