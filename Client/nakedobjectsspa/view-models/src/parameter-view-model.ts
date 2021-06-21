import * as Ro from '@nakedobjects/restful-objects';
import { ColorService, ConfigService, ContextService, ErrorService, MaskService, Pane } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import filter from 'lodash-es/filter';
import find from 'lodash-es/find';
import map from 'lodash-es/map';
import some from 'lodash-es/some';
import { ChoiceViewModel } from './choice-view-model';
import { FieldViewModel } from './field-view-model';
import * as Msg from './user-messages';
import { ViewModelFactoryService } from './view-model-factory.service';

export class ParameterViewModel extends FieldViewModel {

    constructor(
        public readonly parameterRep: Ro.Parameter,
        onPaneId: Pane,
        color: ColorService,
        error: ErrorService,
        private readonly maskService: MaskService,
        private readonly previousValue: Ro.Value,
        private readonly viewModelFactory: ViewModelFactoryService,
        context: ContextService,
        configService: ConfigService
    ) {

        super(parameterRep,
            color,
            error,
            context,
            configService,
            onPaneId,
            parameterRep.isScalar(),
            parameterRep.id(),
            parameterRep.isCollectionContributed(),
            parameterRep.entryType(),
            !parameterRep.disabledReason());

        this.dflt = parameterRep.default().toString();
        this.hasValue = !!this.dflt;

        const fieldEntryType = this.entryType;

        if (parameterRep.isScalar()) {
            const remoteMask = parameterRep.extensions().mask();
            this.localFilter = this.maskService.toLocalFilter(remoteMask, parameterRep.extensions().format()!);
        }

        if (fieldEntryType === Ro.EntryType.Choices || fieldEntryType === Ro.EntryType.MultipleChoices) {
            this.setupParameterChoices();
        }

        if (fieldEntryType === Ro.EntryType.AutoComplete) {
            this.setupParameterAutocomplete();
        }

        if (fieldEntryType === Ro.EntryType.FreeForm && this.type === 'ref') {
            this.setupParameterFreeformReference();
        }

        if (fieldEntryType === Ro.EntryType.ConditionalChoices || fieldEntryType === Ro.EntryType.MultipleConditionalChoices) {
            this.setupParameterConditionalChoices();
        }

        if (fieldEntryType !== Ro.EntryType.FreeForm || this.isCollectionContributed) {
            this.setupParameterSelectedChoices();
        } else {
            this.setupParameterSelectedValue();
        }

        this.update();
        this.description = this.getRequiredIndicator() + this.description;
    }

    private readonly dflt: string;

    private setupParameterChoices() {
        this.setupChoices(this.parameterRep.choices()!);
    }

    private setupParameterAutocomplete() {
        const parmRep = this.parameterRep;
        this.setupAutocomplete(parmRep, () => <Dictionary<Ro.Value>>{});
    }

    private setupParameterFreeformReference() {
        const parmRep = this.parameterRep;
        this.description = this.description || Msg.dropPrompt;

        const val = this.previousValue && !this.previousValue.isNull() ? this.previousValue : parmRep.default();

        if (!val.isNull() && val.isReference()) {
            const link = val.link()!;
            this.reference = link.href();
            this.selectedChoice = new ChoiceViewModel(val, this.id, link.title());
        }
    }

    private setupParameterConditionalChoices() {
        const parmRep = this.parameterRep;
        this.setupConditionalChoices(parmRep);
    }

    private setupParameterSelectedChoices() {
        const parmRep = this.parameterRep;
        const fieldEntryType = this.entryType;
        const parmViewModel = this;
        function setCurrentChoices(vals: Ro.Value) {
            const list = vals.list()!;
            const choicesToSet = map(list, val => new ChoiceViewModel(val, parmViewModel.id, val.link() ? val.link()!.title() : undefined));

            if (fieldEntryType === Ro.EntryType.MultipleChoices) {
                parmViewModel.selectedMultiChoices = filter(parmViewModel.choices, c => some(choicesToSet, choiceToSet => c.valuesEqual(choiceToSet)));
            } else {
                parmViewModel.selectedMultiChoices = choicesToSet;
            }
        }

        function setCurrentChoice(val: Ro.Value) {
            const choiceToSet = new ChoiceViewModel(val, parmViewModel.id, val.link() ? val.link()!.title() : undefined);

            if (fieldEntryType === Ro.EntryType.Choices) {
                const choices = parmViewModel.choices!;
                parmViewModel.selectedChoice = find(choices, c => c.valuesEqual(choiceToSet)) || null;
            } else {
                if (!parmViewModel.selectedChoice || parmViewModel.selectedChoice.getValue().toValueString() !== choiceToSet.getValue().toValueString()) {
                    parmViewModel.selectedChoice = choiceToSet;
                }
            }
        }

        parmViewModel.refresh = (newValue: Ro.Value) => {

            if (newValue || parmViewModel.dflt) {
                const toSet = newValue || parmRep.default();
                if (fieldEntryType === Ro.EntryType.MultipleChoices || fieldEntryType === Ro.EntryType.MultipleConditionalChoices ||
                    parmViewModel.isCollectionContributed) {
                    setCurrentChoices(toSet);
                } else {
                    setCurrentChoice(toSet);
                }
            }
        };

        parmViewModel.refresh(this.previousValue);

    }

    private toTriStateBoolean(valueToSet: string | boolean | number | null): boolean | null {

        // looks stupid but note type checking
        if (valueToSet === true || valueToSet === 'true') {
            return true;
        }
        if (valueToSet === false || valueToSet === 'false') {
            return false;
        }
        return null;
    }

    private setupParameterSelectedValue() {
        const parmRep = this.parameterRep;
        const returnType = parmRep.extensions().returnType();

        this.refresh = (newValue: Ro.Value) => {

            if (returnType === 'boolean') {
                const valueToSet = (newValue ? newValue.toValueString() : null) || parmRep.default().scalar();
                const bValueToSet = this.toTriStateBoolean(valueToSet);

                this.value = bValueToSet;
            } else if (Ro.isDateOrDateTime(parmRep)) {
                const date = Ro.toUtcDate(newValue || new Ro.Value(this.dflt));
                this.value = date ? Ro.toDateString(date) : '';
            } else if (Ro.isTime(parmRep)) {
                const time = Ro.toTime(newValue || new Ro.Value(this.dflt));
                this.value = time ? Ro.toTimeString(time) : '';
            } else {
                this.value = (newValue ? newValue.toString() : null) || this.dflt || '';
            }
        };

        this.refresh(this.previousValue);
    }

    readonly setAsRow = (i: number) => this.paneArgId = `${this.argId}${i}`;

    protected update() {
        super.update();

        switch (this.entryType) {
            case (Ro.EntryType.FreeForm):
                if (this.type === 'scalar') {
                    if (this.localFilter) {
                        this.formattedValue = this.value ? this.localFilter.filter(this.value) : '';
                    } else {
                        this.formattedValue = this.value ? this.value.toString() : '';
                    }
                    break;
                }
            // fall through
            // tslint:disable-next-line:no-switch-case-fall-through
            case (Ro.EntryType.AutoComplete):
            case (Ro.EntryType.Choices):
            case (Ro.EntryType.ConditionalChoices):
                this.formattedValue = this.selectedChoice ? this.selectedChoice.toString() : '';
                break;
            case (Ro.EntryType.MultipleChoices):
            case (Ro.EntryType.MultipleConditionalChoices):
                const selectedChoices = this.selectedMultiChoices?.map((c) => c.toString());
                this.formattedValue = selectedChoices ? selectedChoices.join() : '';
                break;
            default:
                this.formattedValue = this.value ? this.value.toString() : '';
        }
    }
}
