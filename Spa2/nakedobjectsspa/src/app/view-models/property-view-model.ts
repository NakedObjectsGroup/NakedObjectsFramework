import { FieldViewModel } from './field-view-model';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { AttachmentViewModel } from './attachment-view-model';
import * as Models from '../models';
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Contextservice from '../context.service';
import * as _ from "lodash";
import * as Msg from "../user-messages";
import * as Choiceviewmodel from './choice-view-model';
import * as Maskservice from '../mask.service';
import * as Clickhandlerservice from '../click-handler.service';
import * as Urlmanagerservice from '../url-manager.service';
import * as Momentwrapperservice from '../moment-wrapper.service';
import * as Helpersviewmodels from './helpers-view-models';

export class PropertyViewModel extends FieldViewModel {

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
            const createcvm = _.partial(this.viewModelfactory.createChoiceViewModels, this.id, searchTerm);
            const digest = this.getDigest(propertyRep);

            return this.context.autoComplete(propertyRep, this.id, parentValues, searchTerm, digest).then(createcvm);
        };
        this.minLength = propertyRep.promptLink().extensions().minLength();
        this.description = this.description || Msg.autoCompletePrompt;
    }

    private setupPropertyConditionalChoices() {
        const propertyRep = this.propertyRep;
        this.conditionalChoices = (args: _.Dictionary<Models.Value>) => {
            const createcvm = _.partial(this.viewModelfactory.createChoiceViewModels, this.id, null);
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

    private setupChoice( newValue: Models.Value) {
        const propertyRep = this.propertyRep;
        if (this.entryType === Models.EntryType.Choices) {

            const choices = propertyRep.choices();

            this.choices = _.map(choices, (v, n) => Choiceviewmodel.ChoiceViewModel.create(v, this.id, n));

            if (this.optional) {
                const emptyChoice = Choiceviewmodel.ChoiceViewModel.create(new Models.Value(""), this.id);
                this.choices = _.concat([emptyChoice], this.choices);
            }

            const currentChoice = Choiceviewmodel.ChoiceViewModel.create(newValue, this.id);
            this.selectedChoice = _.find(this.choices, c => c.valuesEqual(currentChoice));
        } else if (!propertyRep.isScalar()) {
            this.selectedChoice = Choiceviewmodel.ChoiceViewModel.create(newValue, this.id);
        }
    }

    private setupReference( value: Models.Value, rep: Models.IHasExtensions) {
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

    private setScalarValueInView( value: Models.Value) {
        if (Models.isDateOrDateTime(this.propertyRep)) {
            //vm.value = Models.toUtcDate(value);
            const date = Models.toUtcDate(value);
            this.value = date ? Models.toDateString(date) : "";
        } else if (Models.isTime(this.propertyRep)) {
            this.value = Models.toTime(value);
        } else {
            this.value = value.scalar();
        }
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
            this.setScalarValueInView(value);

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



    constructor(propertyRep: Models.PropertyMember,
            color: ColorService,
            error: ErrorService,
            private viewModelfactory: Viewmodelfactoryservice.ViewModelFactoryService, 
            private context: Contextservice.ContextService, 
            private maskService: Maskservice.MaskService, 
            private urlManager: Urlmanagerservice.UrlManagerService, 
            private clickHandler: Clickhandlerservice.ClickHandlerService, 
            private momentWrapperService : Momentwrapperservice.MomentWrapperService, 
            id: string,
            previousValue: Models.Value,
            paneId: number,
            parentValues: () => _.Dictionary<Models.Value>) {
        super(propertyRep.extensions(), color, error);
        this.draggableType = propertyRep.extensions().returnType();

        this.propertyRep = propertyRep;
        this.entryType = propertyRep.entryType();
        this.isEditable = !propertyRep.disabledReason();
        this.entryType = propertyRep.entryType();

        this.id = id;
        this.onPaneId = paneId;
        this.argId = `${id.toLowerCase()}`;
        this.paneArgId = `${this.argId}${paneId}`;


        if (propertyRep.attachmentLink() != null) {
            this.attachment = this.viewModelfactory.attachmentViewModel(propertyRep, paneId);
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
  
        this.isDirty = () => !!previousValue || this.getValue().toValueString() !== this.originalValue.toValueString();
        this.validate = _.partial(Helpersviewmodels.validate, propertyRep, this, this.momentWrapperService) as (modelValue: any, viewValue: string, mandatoryOnly: boolean) => boolean;
        this.canDropOn = (targetType: string) => this.context.isSubTypeOf(this.returnType, targetType) as Promise<boolean>;
        this.drop = _.partial(Helpersviewmodels.drop, this.context, this.error, this);
        this.doClick = (right?: boolean) => this.urlManager.setProperty(propertyRep, this.clickHandler.pane(paneId, right));
    }

    propertyRep: Models.PropertyMember;
    isEditable: boolean;
    attachment: AttachmentViewModel;
    refType: "null" | "navigable" | "notNavigable";
    isDirty: () => boolean;
    doClick: (right?: boolean) => void;

    // IDraggableViewModel
    draggableType: string;
    draggableTitle = () => this.formattedValue;

    canDropOn: (targetType: string) => Promise<boolean>;
}