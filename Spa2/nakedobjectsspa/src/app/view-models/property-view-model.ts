import { FieldViewModel } from './field-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { AttachmentViewModel } from './attachment-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ContextService } from '../context.service';
import { ChoiceViewModel } from './choice-view-model';
import { MaskService } from '../mask.service';
import { ClickHandlerService } from '../click-handler.service';
import { UrlManagerService } from '../url-manager.service';
import { MomentWrapperService } from '../moment-wrapper.service';
import { IDraggableViewModel } from './idraggable-view-model';
import * as _ from "lodash";
import * as Msg from "../user-messages";
import * as Models from '../models';
import * as Helpers from './helpers-view-models';

export class PropertyViewModel extends FieldViewModel {

    constructor(
        public propertyRep: Models.PropertyMember,
        color: ColorService,
        error: ErrorService,
        private viewModelfactory: ViewModelFactoryService,
        private context: ContextService,
        private maskService: MaskService,
        private urlManager: UrlManagerService,
        private clickHandler: ClickHandlerService,
        private momentWrapperService: MomentWrapperService,
        id: string,
        private previousValue: Models.Value,
        onPaneId: number,
        parentValues: () => _.Dictionary<Models.Value>
    ) {

        super(propertyRep.extensions(), color, error, onPaneId);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();

        this.id = id;
        this.argId = `${id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${onPaneId}`;

        if (propertyRep.attachmentLink() != null) {
            this.attachment = this.viewModelfactory.attachmentViewModel(propertyRep, onPaneId);
        }

        const fieldEntryType = this.entryType;

        if (fieldEntryType === Models.EntryType.AutoComplete) {
            this.setupPropertyAutocomplete(parentValues);
        }

        if (fieldEntryType === Models.EntryType.ConditionalChoices) {
            this.setupPropertyConditionalChoices();
        }

        if (propertyRep.isScalar()) {
            this.setupScalarPropertyValue();
        } else {
            // is reference
            this.setupReferencePropertyValue();
        }

        this.refresh(previousValue);

        if (!previousValue) {
            this.originalValue = this.getValue();
        }

        const required = this.optional ? "" : "* ";
        this.description = required + this.description;
    }


    private getDigest(propertyRep: Models.PropertyMember) {
        const parent = propertyRep.parent;
        if (parent instanceof Models.DomainObjectRepresentation) {
            if (parent.isTransient()) {
                return parent.etagDigest;
            }
        }
        return null;
    }

    private setupPropertyAutocomplete(parentValues: () => _.Dictionary<Models.Value>) {
        const propertyRep = this.propertyRep;
        this.prompt = (searchTerm: string) => {
            const createcvm = _.partial(Helpers.createChoiceViewModels, this.id, searchTerm);
            const digest = this.getDigest(propertyRep);

            return this.context.autoComplete(propertyRep, this.id, parentValues, searchTerm, digest).then(createcvm);
        };
        this.minLength = propertyRep.promptLink().extensions().minLength();
        this.description = this.description || Msg.autoCompletePrompt;
    }

    private setupPropertyConditionalChoices() {
        const propertyRep = this.propertyRep;
        this.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(Helpers.createChoiceViewModels, this.id, null);
            const digest = this.getDigest(propertyRep);
            return this.context.conditionalChoices(propertyRep, this.id, () => <_.Dictionary<Models.Value>>{}, args, digest).then(createcvm);
        };
        this.promptArguments = (<any>_.fromPairs)(_.map(propertyRep.promptLink().arguments(), (v: any, key: string) => [key, new Models.Value(v.value)]));
    }

    private callIfChanged(newValue: Models.Value, doRefresh: (newValue: Models.Value) => void) {
        const propertyRep = this.propertyRep;
        const value = newValue || propertyRep.value();

        if (this.currentValue == null || value.toValueString() !== this.currentValue.toValueString()) {
            doRefresh(value);
            this.currentValue = value;
        }
    }

    private setupChoice(newValue: Models.Value) {
        const propertyRep = this.propertyRep;
        if (this.entryType === Models.EntryType.Choices) {

            const choices = propertyRep.choices();

            this.choices = _.map(choices, (v, n) => new ChoiceViewModel(v, this.id, n));

            if (this.optional) {
                const emptyChoice = new ChoiceViewModel(new Models.Value(""), this.id);
                this.choices = _.concat([emptyChoice], this.choices);
            }

            const currentChoice = new ChoiceViewModel(newValue, this.id);
            this.selectedChoice = _.find(this.choices, c => c.valuesEqual(currentChoice));
        } else if (!propertyRep.isScalar()) {
            this.selectedChoice = new ChoiceViewModel(newValue, this.id);
        }
    }

    private setupReference(value: Models.Value, rep: Models.IHasExtensions) {
        this.type = "ref";
        if (value.isNull()) {
            this.reference = "";
            this.value = this.description;
            this.formattedValue = "";
            this.refType = "null";
        } else {
            this.reference = value.link().href();
            this.value = value.toString();
            this.formattedValue = value.toString();
            this.refType = rep.extensions().notNavigable() ? "notNavigable" : "navigable";
        }
        if (this.entryType === Models.EntryType.FreeForm) {
            this.description = this.description || Msg.dropPrompt;
        }
    }

    private setupReferencePropertyValue() {
        const propertyRep = this.propertyRep;
        this.refresh = (newValue: Models.Value) => this.callIfChanged(newValue, (value: Models.Value) => {
            this.setupChoice(value);
            this.setupReference(value, propertyRep);
        });
    }

    private setupScalarPropertyValue() {
        const propertyRep = this.propertyRep;
        this.type = "scalar";

        const remoteMask = propertyRep.extensions().mask();
        const localFilter = this.maskService.toLocalFilter(remoteMask, propertyRep.extensions().format());
        this.localFilter = localFilter;
        // formatting also happens in in directive - at least for dates - value is now date in that case

        this.refresh = (newValue: Models.Value) => this.callIfChanged(newValue, (value: Models.Value) => {

            this.setupChoice(value);
            Helpers.setScalarValueInView(this, this.propertyRep, value);

            if (propertyRep.entryType() === Models.EntryType.Choices) {
                if (this.selectedChoice) {
                    this.value = this.selectedChoice.name;
                    this.formattedValue = this.selectedChoice.name;
                }
            } else if (this.password) {
                this.formattedValue = Msg.obscuredText;
            } else {
                this.formattedValue = localFilter.filter(this.value);
            }
        });
    }

    isEditable: boolean;
    attachment: AttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    // IDraggableViewModel
    draggableType: string;
    draggableTitle = () => this.formattedValue;

    isDirty = () => !!this.previousValue || this.getValue().toValueString() !== this.originalValue.toValueString();
    validate = _.partial(Helpers.validate, this.propertyRep, this, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
    canDropOn = (targetType: string) => this.context.isSubTypeOf(this.returnType, targetType) as Promise<boolean>;
    drop = _.partial(Helpers.drop, this.context, this.error, this) as (newValue: IDraggableViewModel) => Promise<boolean>;
    doClick = (right?: boolean) => this.urlManager.setProperty(this.propertyRep, this.clickHandler.pane(this.onPaneId, right));
}