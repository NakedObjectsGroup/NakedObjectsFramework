import { FieldViewModel } from './field-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import * as Models from '../models';
import * as _ from "lodash";
import * as Choiceviewmodel from './choice-view-model';
import * as Usermessages from '../user-messages';
import * as Contextservice from '../context.service';
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Maskservice from '../mask.service';
import * as Momentwrapperservice from '../moment-wrapper.service';
import * as Helpersviewmodels from './helpers-view-models';

export class ParameterViewModel extends FieldViewModel {

    private setupParameterChoices() {
        const parmRep = this.parameterRep;
        this.choices = _.map(parmRep.choices(), (v, n) => Choiceviewmodel.ChoiceViewModel.create(v, parmRep.id(), n));
    }

    private setupParameterAutocomplete() {
        const parmRep = this.parameterRep;
        this.prompt = (searchTerm: string) => {
            const createcvm = _.partial(Helpersviewmodels.createChoiceViewModels, this.id, searchTerm);
            return this.context.autoComplete(parmRep, this.id, () => <_.Dictionary<Models.Value>>{}, searchTerm).
                then(createcvm);
        };
        this.minLength = parmRep.promptLink().extensions().minLength();
        this.description = this.description || Usermessages.autoCompletePrompt;
    }

    private setupParameterFreeformReference() {
        const parmRep = this.parameterRep;
        this.description = this.description || Usermessages.dropPrompt;

        const val = this.previousValue && !this.previousValue.isNull() ? this.previousValue : parmRep.default();

        if (!val.isNull() && val.isReference()) {
            this.reference = val.link().href();
            this.selectedChoice = Choiceviewmodel.ChoiceViewModel.create(val, this.id, val.link() ? val.link().title() : null);
        }
    }

    private setupParameterConditionalChoices() {
        const parmRep = this.parameterRep;
        this.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(Helpersviewmodels.createChoiceViewModels, this.id, null);
            return this.context.conditionalChoices(parmRep, this.id, () => <_.Dictionary<Models.Value>>{}, args).
                then(createcvm);
        };
        this.promptArguments = (<any>_.fromPairs)(_.map(parmRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private setupParameterSelectedChoices() {
        const parmRep = this.parameterRep;
        const fieldEntryType = this.entryType;
        const parmViewModel = this;
        function setCurrentChoices(vals: Models.Value) {

            const choicesToSet = _.map(vals.list(), val => Choiceviewmodel.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

            if (fieldEntryType === Models.EntryType.MultipleChoices) {
                parmViewModel.selectedMultiChoices = _.filter(parmViewModel.choices, c => _.some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
            } else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }

        function setCurrentChoice(val: Models.Value) {
            const choiceToSet = Choiceviewmodel.ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

            if (fieldEntryType === Models.EntryType.Choices) {
                parmViewModel.selectedChoice = _.find(parmViewModel.choices, c => c.valuesEqual(choiceToSet));
            } else {
                if (!parmViewModel.selectedChoice || parmViewModel.selectedChoice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                    parmViewModel.selectedChoice = choiceToSet;
                }
            }
        }

        parmViewModel.refresh = (newValue: Models.Value) => {

            if (newValue || parmViewModel.dflt) {
                const toSet = newValue || parmRep.default();
                if (fieldEntryType === Models.EntryType.MultipleChoices || fieldEntryType === Models.EntryType.MultipleConditionalChoices ||
                    parmViewModel.isCollectionContributed) {
                    setCurrentChoices(toSet);
                } else {
                    setCurrentChoice(toSet);
                }
            }
        }

        parmViewModel.refresh(this.previousValue);

    }

    private toTriStateBoolean(valueToSet: string | boolean | number) {

        // looks stupid but note type checking
        if (valueToSet === true || valueToSet === "true") {
            return true;
        }
        if (valueToSet === false || valueToSet === "false") {
            return false;
        }
        return null;
    }

    private setupParameterSelectedValue() {
        const parmRep = this.parameterRep;
        const returnType = parmRep.extensions().returnType();

        this.refresh = (newValue: Models.Value) => {

            if (returnType === "boolean") {
                const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                const bValueToSet = this.toTriStateBoolean(valueToSet);

                this.value = bValueToSet;
            } else if (Models.isDateOrDateTime(parmRep)) {
                //parmViewModel.value = Models.toUtcDate(newValue || new Models.Value(parmViewModel.dflt));
                const date = Models.toUtcDate(newValue || new Models.Value(this.dflt));
                this.value = date ? Models.toDateString(date) : "";
            } else if (Models.isTime(parmRep)) {
                this.value = Models.toTime(newValue || new Models.Value(this.dflt));
            } else {
                this.value = (newValue ? newValue.toString() : null) || this.dflt || "";
            }
        }

        this.refresh(this.previousValue);
    }

    private getRequiredIndicator() {
        return this.optional || typeof this.value === "boolean" ? "" : "* ";
    }

    constructor(parmRep: Models.Parameter,
                paneId: number,     
                color: ColorService,
                error: ErrorService,
                private momentWrapperService : Momentwrapperservice.MomentWrapperService, 
                private maskService : Maskservice.MaskService,
                private previousValue: Models.Value,
                private viewModelFactory : Viewmodelfactoryservice.ViewModelFactoryService,
                private context: Contextservice.ContextService) {


        super(parmRep.extensions(), color, error, paneId);

        this.parameterRep = parmRep;
        this.onPaneId = paneId;
        this.type = parmRep.isScalar() ? "scalar" : "ref";
        this.dflt = parmRep.default().toString();
        this.id = parmRep.id();
        this.argId = `${this.id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${this.onPaneId}`;
        this.isCollectionContributed = parmRep.isCollectionContributed();
        this.entryType = parmRep.entryType();
        this.value = null;


        const fieldEntryType = this.entryType;

        if (fieldEntryType === Models.EntryType.Choices || fieldEntryType === Models.EntryType.MultipleChoices) {
            this.setupParameterChoices();
        }

        if (fieldEntryType === Models.EntryType.AutoComplete) {
            this.setupParameterAutocomplete();
        }

        if (fieldEntryType === Models.EntryType.FreeForm && this.type === "ref") {
            this.setupParameterFreeformReference();
        }

        if (fieldEntryType === Models.EntryType.ConditionalChoices || fieldEntryType === Models.EntryType.MultipleConditionalChoices) {
            this.setupParameterConditionalChoices();
        }

        if (fieldEntryType !== Models.EntryType.FreeForm || this.isCollectionContributed) {
            this.setupParameterSelectedChoices();
        } else {
            this.setupParameterSelectedValue();
        }

        const remoteMask = parmRep.extensions().mask();

        if (remoteMask && parmRep.isScalar()) {
            const localFilter = this.maskService.toLocalFilter(remoteMask, parmRep.extensions().format());
            this.localFilter = localFilter;
            // formatting also happens in in directive - at least for dates - value is now date in that case
            this.formattedValue = localFilter.filter(this.value.toString());
        }

        this.description = this.getRequiredIndicator() + this.description;
        this.validate = _.partial(Helpersviewmodels.validate, parmRep, this, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        this.drop = _.partial(Helpersviewmodels.drop, this.context, this.error, this);
    }


    parameterRep: Models.Parameter;
    dflt: string;
}