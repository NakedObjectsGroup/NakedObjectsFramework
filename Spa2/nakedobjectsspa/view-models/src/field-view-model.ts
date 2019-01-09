import { AbstractControl } from '@angular/forms';
import * as Ro from '@nakedobjects/restful-objects';
import { ColorService, ConfigService, ContextService, ErrorService, ErrorWrapper, ILocalFilter, Pane } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import concat from 'lodash-es/concat';
import every from 'lodash-es/every';
import filter from 'lodash-es/filter';
import find from 'lodash-es/find';
import fromPairs from 'lodash-es/fromPairs';
import map from 'lodash-es/map';
import partial from 'lodash-es/partial';
import some from 'lodash-es/some';
import { ChoiceViewModel } from './choice-view-model';
import * as Helpers from './helpers-view-models';
import { IDraggableViewModel } from './idraggable-view-model';
import { MessageViewModel } from './message-view-model';
import * as Msg from './user-messages';

export abstract class FieldViewModel extends MessageViewModel {

    protected constructor(
        private readonly rep: Ro.IHasExtensions,
        protected readonly colorService: ColorService,
        protected readonly error: ErrorService,
        protected readonly context: ContextService,
        private readonly configService: ConfigService,
        public readonly onPaneId: Pane,
        public readonly isScalar: boolean,
        public readonly id: string,
        public readonly isCollectionContributed: boolean,
        public readonly entryType: Ro.EntryType
    ) {
        super();
        const ext = rep.extensions();
        this.optional = ext.optional();
        this.description = ext.description();
        this.presentationHint = ext.presentationHint();
        this.mask = ext.mask();
        this.title = ext.friendlyName();
        this.returnType = ext.returnType() !;
        this.format = Ro.withNull(ext.format());
        this.multipleLines = ext.multipleLines() || 1;
        this.password = ext.dataType() === 'password';
        this.type = isScalar ? 'scalar' : 'ref';
        this.argId = `${id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${onPaneId}`;
    }

    readonly argId: string;
    paneArgId: string;

    readonly optional: boolean;
    description: string;
    readonly presentationHint: string;
    readonly mask: string;
    readonly title: string;
    readonly returnType: string;
    readonly format: Ro.FormatType | null;
    readonly multipleLines: number;
    readonly password: boolean;
    readonly type: 'scalar' | 'ref';

    clientValid = true;
    reference = '';
    minLength: number;
    color: string;
    promptArguments: Dictionary<Ro.Value>;
    currentValue: Ro.Value;
    originalValue: Ro.Value;
    localFilter: ILocalFilter;
    formattedValue: string;
    private currentChoice: ChoiceViewModel | null;
    private currentMultipleChoices: ChoiceViewModel[];
    private currentRawValue: Ro.ScalarValueType | Date | null = null;
    private choiceOptions: ChoiceViewModel[] = [];
    hasValue = false;

    file: Ro.Link;

    refresh: (newValue: Ro.Value) => void;
    prompt: (searchTerm: string) => Promise<ChoiceViewModel[]>;
    conditionalChoices: (args: Dictionary<Ro.Value>) => Promise<ChoiceViewModel[]>;

    readonly drop = (newValue: IDraggableViewModel) => Helpers.drop(this.context, this.error, this, newValue);

    readonly validate = (modelValue: string | ChoiceViewModel | string[] | ChoiceViewModel[], viewValue: string, mandatoryOnly: boolean) =>
        Helpers.validate(this.rep, this, modelValue, viewValue, mandatoryOnly)

    get choices(): ChoiceViewModel[] {
        return this.choiceOptions;
    }

    set choices(options: ChoiceViewModel[]) {
        this.choiceOptions = options;

        if (this.entryType === Ro.EntryType.MultipleConditionalChoices) {
            const currentSelectedOptions = this.selectedMultiChoices;
            this.selectedMultiChoices = filter(this.choiceOptions, c => some(currentSelectedOptions, (choiceToSet: any) => c.valuesEqual(choiceToSet)));
        } else if (this.entryType === Ro.EntryType.ConditionalChoices) {
            const currentSelectedOption = this.selectedChoice;
            this.selectedChoice = find(this.choiceOptions, c => c.valuesEqual(currentSelectedOption));
        }

        if (!this.optional && !this.hasValue && this.entryType !== Ro.EntryType.AutoComplete) {
            // mandatory and not selected so add a mandatory indicator choice
            const indicatorChoice = new ChoiceViewModel(new Ro.Value(''), this.id, '*');
            this.choiceOptions = concat<ChoiceViewModel>([indicatorChoice], this.choices);
            this.selectedChoice = indicatorChoice;
        }
    }

    get selectedChoice(): ChoiceViewModel | null {
        return this.currentChoice;
    }

    set selectedChoice(newChoice: ChoiceViewModel | null) {
        // type guard because angular pushes string value here until directive finds
        // choice
        if (newChoice instanceof ChoiceViewModel || newChoice == null) {
            this.currentChoice = newChoice;
            this.update();
        }
    }

    get value(): Ro.ScalarValueType | Date | null {
        return this.currentRawValue;
    }

    set value(newValue: Ro.ScalarValueType | Date | null) {
        this.currentRawValue = newValue;
        this.update();
    }

    get selectedMultiChoices(): ChoiceViewModel[] {
        return this.currentMultipleChoices;
    }

    set selectedMultiChoices(choices: ChoiceViewModel[]) {
        this.currentMultipleChoices = choices;
        this.update();
    }

    private isValid(viewValue: string | ChoiceViewModel | string[] | ChoiceViewModel[]): boolean {

        let val: string;

        if (viewValue instanceof ChoiceViewModel) {
            val = viewValue.getValue().toValueString();
        } else if (viewValue instanceof Array) {
            if (viewValue.length) {
                return every(viewValue as (string | ChoiceViewModel)[], (v: any) => this.isValid(v));
            }
            val = '';
        } else {
            val = viewValue as string;
        }

        if (this.entryType === Ro.EntryType.AutoComplete && !(viewValue instanceof ChoiceViewModel)) {

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
        const fullValidate = this.entryType === Ro.EntryType.FreeForm && this.type === 'scalar';

        return this.validate(viewValue, val, !fullValidate);
    }

    readonly validator = (c: AbstractControl): { [index: string]: any; } | null => {
        const viewValue = c.value as string | ChoiceViewModel | string[] | ChoiceViewModel[];
        const isvalid = this.isValid(viewValue);
        return isvalid ? null : { invalid: 'invalid entry' };
    }

    readonly setNewValue = (newValue: IDraggableViewModel) => {
        this.selectedChoice = newValue.selectedChoice;
        this.value = newValue.value;
        this.reference = newValue.reference;
    }

    readonly clear = () => {
        this.selectedChoice = null;
        this.value = null;
        this.reference = '';
    }

    protected update() {
        this.setColor();
    }

    protected setupChoices(choices: Dictionary<Ro.Value>) {
        this.choices = map(choices, (v, n) => new ChoiceViewModel(v, this.id, n));
    }

    protected setupAutocomplete(rep: Ro.IField, parentValues: () => Dictionary<Ro.Value>, digest?: string | null) {

        this.prompt = (searchTerm: string) => {
            const createcvm = partial(Helpers.createChoiceViewModels, this.id, searchTerm);
            return this.context.autoComplete(rep, this.id, parentValues, searchTerm, digest).then(createcvm);
        };
        const promptLink = rep.promptLink() as Ro.Link; // always
        this.minLength = promptLink.extensions().minLength() as number; // always
        this.description = this.description || Msg.autoCompletePrompt;
    }

    protected setupConditionalChoices(rep: Ro.IField, digest?: string | null) {

        this.conditionalChoices = (args: Dictionary<Ro.Value>) => {
            const createcvm = partial(Helpers.createChoiceViewModels, this.id, null);
            return this.context.conditionalChoices(rep, this.id, () => <Dictionary<Ro.Value>>{}, args, digest).then(createcvm);
        };
        const promptLink = rep.promptLink() as Ro.Link;
        this.promptArguments = fromPairs(map(promptLink!.arguments() !, (v: any, key: string) => [key, new Ro.Value(v.value)]));
    }

    protected getRequiredIndicator() {
        return this.optional || typeof this.value === 'boolean' ? '' : '* ';
    }

    private setColor() {

        if (this.entryType === Ro.EntryType.AutoComplete && this.selectedChoice && this.type === 'ref') {
            const href = this.selectedChoice.getValue().getHref();
            if (href) {
                this.colorService.toColorNumberFromHref(href)
                    .then(c => {
                        // only if we still have a choice may have been cleared by a later call
                        if (this.selectedChoice) {
                            this.color = `${this.configService.config.linkColor}${c}`;
                        }
                    })
                    .catch((reject: ErrorWrapper) => this.error.handleError(reject));
                return;
            }
        } else if (this.entryType !== Ro.EntryType.AutoComplete && this.value) {
            this.colorService.toColorNumberFromType(this.returnType)
                .then(c => {
                    // only if we still have a value may have been cleared by a later call
                    if (this.value) {
                        this.color = `${this.configService.config.linkColor}${c}`;
                    }
                })
                .catch((reject: ErrorWrapper) => this.error.handleError(reject));
            return;
        }

        this.color = '';
    }

    readonly setValueFromControl = (newValue: Ro.ScalarValueType | Date | ChoiceViewModel | ChoiceViewModel[]) => {

        if (newValue instanceof Array) {
            this.selectedMultiChoices = newValue;
        } else if (newValue instanceof ChoiceViewModel) {
            this.selectedChoice = newValue;
        } else {
            this.value = newValue;
        }
    }

    readonly getValueForControl = () => this.selectedMultiChoices || this.selectedChoice || this.value;

    readonly getValue = () => {

        if (this.entryType === Ro.EntryType.File) {
            return new Ro.Value(this.file);
        }

        if (this.entryType !== Ro.EntryType.FreeForm || this.isCollectionContributed) {

            if (this.entryType === Ro.EntryType.MultipleChoices || this.entryType === Ro.EntryType.MultipleConditionalChoices || this.isCollectionContributed) {
                const selections = this.selectedMultiChoices || [];
                if (this.type === 'scalar') {
                    const selValues = map(selections, (cvm: ChoiceViewModel) => cvm.getValue().scalar());
                    return new Ro.Value(selValues);
                }
                const selRefs = map(selections, cvm => ({ href: cvm.getValue().getHref() !, title: cvm.name })); // reference
                return new Ro.Value(selRefs);
            }

            const choiceValue = this.selectedChoice ? this.selectedChoice.getValue() : null;
            if (this.type === 'scalar') {
                return new Ro.Value(choiceValue && choiceValue.scalar() != null ? choiceValue.scalar() : '');
            }

            // reference
            return new Ro.Value(choiceValue && choiceValue.isReference() && this.selectedChoice ? { href: choiceValue.getHref() !, title: this.selectedChoice.name } : null);
        }

        if (this.type === 'scalar') {
            if (this.value == null) {
                return new Ro.Value('');
            }

            if (this.value instanceof Date) {

                if (this.format === 'time') {
                    // time format
                    return new Ro.Value(Ro.toTimeString(this.value as Date));
                }

                if (this.format === 'date') {
                    // truncate time;
                    return new Ro.Value(Ro.toDateString(this.value as Date));
                }
                // date-time
                return new Ro.Value((this.value as Date).toISOString());
            }

            return new Ro.Value(this.value as Ro.ScalarValueType);
        }

        // reference
        return new Ro.Value(this.reference ? { href: this.reference, title: this.value!.toString() } : null);
    }
}
