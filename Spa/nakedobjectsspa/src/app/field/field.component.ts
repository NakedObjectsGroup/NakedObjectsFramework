import * as ViewModels from "../view-models";
import * as Models from "../models"
import * as Ro from '../ro-interfaces';
import { AbstractControl } from '@angular/forms';

export class FieldComponent {

    private vmParent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel;
    private model: ViewModels.ParameterViewModel | ViewModels.PropertyViewModel;
    private isConditionalChoices: boolean;
    private control: AbstractControl

    protected init(vmParent: ViewModels.DialogViewModel | ViewModels.DomainObjectViewModel,
        vm: ViewModels.ParameterViewModel | ViewModels.PropertyViewModel,
        control: AbstractControl) {

        this.vmParent = vmParent;
        this.model = vm;
        this.control = control;

        this.paneId = this.model.onPaneId;

        this.isConditionalChoices = (this.model.entryType === Models.EntryType.ConditionalChoices ||
            this.model.entryType === Models.EntryType.MultipleConditionalChoices);

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
           
            //this.control.reset(this.model.getValueForControl());




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
    }

}