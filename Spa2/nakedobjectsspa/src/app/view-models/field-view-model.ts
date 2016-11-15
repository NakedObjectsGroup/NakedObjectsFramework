import { MessageViewModel } from './message-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { ILocalFilter } from '../mask.service';
import { ChoiceViewModel } from './choice-view-model';
import { IDraggableViewModel } from './idraggable-view-model';
import { AbstractControl } from '@angular/forms';
import { Subject } from 'rxjs/Subject';
import * as Models from '../models';
import * as Ro from '../ro-interfaces';
import * as Config from '../config';
import * as Msg from '../user-messages';
import * as _ from "lodash";

export abstract class FieldViewModel extends MessageViewModel {

    constructor(ext: Models.Extensions, color: ColorService, error: ErrorService) {
        super();
        this.optional = ext.optional();
        this.description = ext.description();
        this.presentationHint = ext.presentationHint();
        this.mask = ext.mask();
        this.title = ext.friendlyName();
        this.returnType = ext.returnType();
        this.format = ext.format();
        this.multipleLines = ext.multipleLines() || 1;
        this.password = ext.dataType() === "password";
        this.updateColor = _.partial(this.setColor, color);
        this.error = error;
    }

    id: string;
    argId: string;
    paneArgId: string;
    onPaneId: number;

    optional: boolean;
    description: string;
    presentationHint: string;
    mask: string;
    title: string;
    returnType: string;
    format: Ro.formatType;
    multipleLines: number;
    password: boolean;

    private _clientValid = true;

    get clientValid() {
        return this._clientValid;
    }

    set clientValid(val: boolean) {
        this._clientValid = val;
        this.validChangedSource.next(true);
        this.validChangedSource.next(false);
    }

    private validChangedSource = new Subject<boolean>();

    validChanged$ = this.validChangedSource.asObservable();

    type: "scalar" | "ref";
    reference = "";
    minLength: number;

    color: string;

    isCollectionContributed: boolean;

    promptArguments: _.Dictionary<Models.Value>;

    currentValue: Models.Value;
    originalValue: Models.Value;

    localFilter: ILocalFilter;
    formattedValue: string;

    private choiceOptions: any[] = [];

    get choices(): ChoiceViewModel[] {
        return this.choiceOptions;
    }

    set choices(options: ChoiceViewModel[]) {
        this.choiceOptions = options;

        if (this.entryType === Models.EntryType.MultipleConditionalChoices) {
            const currentSelectedOptions = this.selectedMultiChoices;
            this.selectedMultiChoices = _.filter(this.choiceOptions, c => _.some(currentSelectedOptions, (choiceToSet: any) => c.valuesEqual(choiceToSet)));
        } else if (this.entryType === Models.EntryType.ConditionalChoices) {
            const currentSelectedOption = this.selectedChoice;
            this.selectedChoice = _.find(this.choiceOptions, (c: any) => c.valuesEqual(currentSelectedOption));
        }
    }

    private currentChoice: ChoiceViewModel;

    private error: ErrorService;

    get selectedChoice(): ChoiceViewModel {
        return this.currentChoice;
    }

    set selectedChoice(newChoice: ChoiceViewModel) {
        // type guard because angular pushes string value here until directive finds 
        // choice
        if (newChoice instanceof ChoiceViewModel || newChoice == null) {
            this.currentChoice = newChoice;
            this.updateColor();
        }
    }

    private currentRawValue: Ro.scalarValueType | Date;

    get value(): Ro.scalarValueType | Date {
        return this.currentRawValue;
    }

    set value(newValue: Ro.scalarValueType | Date) {
        this.currentRawValue = newValue;
        this.updateColor();
    }

    selectedMultiChoices: ChoiceViewModel[];

    file: Models.Link;

    entryType: Models.EntryType;

    validate: (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;

    private isValid(viewValue: any): boolean {

        let val: string;

        if (viewValue instanceof ChoiceViewModel) {
            val = viewValue.getValue().toValueString();
        } else if (viewValue instanceof Array) {
            if (viewValue.length) {
                return _.every(viewValue as (string | ChoiceViewModel)[], (v: any) => this.isValid(v));
            }
            val = "";
        } else {
            val = viewValue as string;
        }

        if (this.entryType === Models.EntryType.AutoComplete && !(viewValue instanceof ChoiceViewModel)) {

            if (val) {
                this.setMessage(Msg.pendingAutoComplete);
                this.clientValid = false;
                return false;
            } else if (!this.optional) {
                this.resetMessage();
                this.clientValid = false;
                return false;
            }
        }

        // only fully validate freeform scalar 
        const fullValidate = this.entryType === Models.EntryType.FreeForm && this.type === "scalar";

        return this.validate(viewValue, val, !fullValidate);
    };

    validator(c: AbstractControl): { [index: string]: any; } {
        const viewValue = c.value;
        const isvalid = this.isValid(viewValue);
        return isvalid ? null : { invalid: "invalid entry" };
        //return null;
    };

    refresh: (newValue: Models.Value) => void;

    prompt: (searchTerm: string) => Promise<ChoiceViewModel[]>;

    conditionalChoices: (args: _.Dictionary<Models.Value>) => Promise<ChoiceViewModel[]>;

    setNewValue(newValue: IDraggableViewModel) {
        this.selectedChoice = newValue.selectedChoice;
        this.value = newValue.value;
        this.reference = newValue.reference;
    }

    drop: (newValue: IDraggableViewModel) => Promise<boolean>;

    clear() {
        this.selectedChoice = null;
        this.value = null;
        this.reference = "";
    }

    private updateColor: () => void;

    private setColor(color: ColorService) {

        if (this.entryType === Models.EntryType.AutoComplete && this.selectedChoice && this.type === "ref") {
            const href = this.selectedChoice.getValue().href();
            if (href) {
                color.toColorNumberFromHref(href)
                    .then(c => {
                        // only if we still have a choice may have been cleared by a later call
                        if (this.selectedChoice) {
                            this.color = `${Config.linkColor}${c}`;
                        }
                    })
                    .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
                return;
            }
        } else if (this.entryType !== Models.EntryType.AutoComplete && this.value) {
            color.toColorNumberFromType(this.returnType)
                .then(c => {
                    // only if we still have a value may have been cleared by a later call
                    if (this.value) {
                        this.color = `${Config.linkColor}${c}`;
                    }
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            return;
        }

        this.color = "";
    }

    setValueFromControl(newValue: Ro.scalarValueType | Date | ChoiceViewModel | ChoiceViewModel[]) {

        if (newValue instanceof Array) {
            this.selectedMultiChoices = newValue;
        } else if (newValue instanceof ChoiceViewModel) {
            this.selectedChoice = newValue;
        } else {
            this.value = newValue;
        }

    }

    getValueForControl() {
        return this.selectedMultiChoices || this.selectedChoice || this.value;
    }

    getValue(): Models.Value {

        if (this.entryType === Models.EntryType.File) {
            return new Models.Value(this.file);
        }

        if (this.entryType !== Models.EntryType.FreeForm || this.isCollectionContributed) {

            if (this.entryType === Models.EntryType.MultipleChoices || this.entryType === Models.EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                const selections = this.selectedMultiChoices || [];
                if (this.type === "scalar") {
                    const selValues = _.map(selections, cvm => cvm.getValue().scalar());
                    return new Models.Value(selValues);
                }
                const selRefs = _.map(selections, cvm => ({ href: cvm.getValue().href(), title: cvm.name })); // reference 
                return new Models.Value(selRefs);
            }

            const choiceValue = this.selectedChoice ? this.selectedChoice.getValue() : null;
            if (this.type === "scalar") {
                return new Models.Value(choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : "");
            }

            // reference 
            return new Models.Value(choiceValue && choiceValue.isReference() ? { href: choiceValue.href(), title: this.selectedChoice.name } : null);
        }

        if (this.type === "scalar") {
            if (this.value == null) {
                return new Models.Value("");
            }

            if (this.value instanceof Date) {

                if (this.format === "time") {
                    // time format
                    return new Models.Value(Models.toTimeString(this.value as Date));
                }

                if (this.format === "date") {
                    // truncate time;
                    return new Models.Value(Models.toDateString(this.value as Date));
                }
                // date-time
                return new Models.Value((this.value as Date).toISOString());
            }

            return new Models.Value(this.value as Ro.scalarValueType);
        }

        // reference
        return new Models.Value(this.reference ? { href: this.reference, title: this.value.toString() } : null);
    }
}