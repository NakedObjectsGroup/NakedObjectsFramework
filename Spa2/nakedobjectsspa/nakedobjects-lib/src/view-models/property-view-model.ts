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
import { IDraggableViewModel } from './idraggable-view-model';
import { Dictionary } from 'lodash';
import * as Msg from '../user-messages';
import * as Models from '../models';
import * as Helpers from './helpers-view-models';
import * as Configservice from '../config.service';
import { Pane } from '../route-data';
import find from 'lodash-es/find';
import concat from 'lodash-es/concat';

export class PropertyViewModel extends FieldViewModel implements IDraggableViewModel {

    readonly isEditable: boolean;
    readonly attachment: AttachmentViewModel | null;
    refType: "null" | "navigable" | "notNavigable";
    // IDraggableViewModel
    readonly draggableType: string;

    constructor(
        public readonly propertyRep: Models.PropertyMember,
        color: ColorService,
        error: ErrorService,
        private readonly viewModelfactory: ViewModelFactoryService,
        context: ContextService,
        private readonly maskService: MaskService,
        private readonly urlManager: UrlManagerService,
        private readonly clickHandler: ClickHandlerService,
        configService: Configservice.ConfigService,
        id: string,
        private readonly previousValue: Models.Value,
        onPaneId: Pane,
        parentValues: () => Dictionary<Models.Value>
    ) {

        super(propertyRep,
            color,
            error,
            context,
            configService,
            onPaneId,
            propertyRep.isScalar(),
            id,
            propertyRep.isCollectionContributed(),
            propertyRep.entryType());

        this.draggableType = propertyRep.extensions().returnType()!;
        this.isEditable = !propertyRep.disabledReason();

        this.attachment = this.viewModelfactory.attachmentViewModel(propertyRep, onPaneId);

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
        this.hasValue = previousValue && !previousValue.isNull;
        this.description = this.getRequiredIndicator() + this.description;
    }

    private getDigest(propertyRep: Models.PropertyMember): string | null {
        const parent = propertyRep.parent;
        if (parent instanceof Models.DomainObjectRepresentation) {
            if (parent.isTransient()) {
                return Models.withNull(parent.etagDigest);
            }
        }
        return null;
    }

    private setupPropertyAutocomplete(parentValues: () => Dictionary<Models.Value>) {
        const propertyRep = this.propertyRep;
        this.setupAutocomplete(propertyRep, parentValues, this.getDigest(propertyRep));
    }

    private setupPropertyConditionalChoices() {
        const propertyRep = this.propertyRep;
        this.setupConditionalChoices(propertyRep, this.getDigest(propertyRep));
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

            const choices = propertyRep.choices() !;

            this.setupChoices(choices);

            if (this.optional) {
                const emptyChoice = new ChoiceViewModel(new Models.Value(""), this.id);
                this.choices = concat<ChoiceViewModel>([emptyChoice], this.choices);
            }

            const currentChoice = new ChoiceViewModel(newValue, this.id);
            this.selectedChoice = find(this.choices, c => c.valuesEqual(currentChoice)) || null;
        } else if (!propertyRep.isScalar()) {
            this.selectedChoice = new ChoiceViewModel(newValue, this.id);
        }
    }

    private setupReference(value: Models.Value, rep: Models.IHasExtensions) {
        if (value.isNull()) {
            this.reference = "";
            this.value = this.description;
            this.formattedValue = "";
            this.refType = "null";
        } else {
            this.reference = value!.link() !.href();
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

        const remoteMask = propertyRep.extensions().mask();
        const localFilter = this.maskService.toLocalFilter(remoteMask, propertyRep.extensions().format() !);
        this.localFilter = localFilter;

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

    readonly draggableTitle = () => this.formattedValue;
    readonly canDropOn = (targetType: string) => this.context.isSubTypeOf(this.returnType, targetType) as Promise<boolean>;

    readonly doClick = (right?: boolean) => this.urlManager.setProperty(this.reference, this.clickHandler.pane(this.onPaneId, right));

    readonly isDirty = () => !!this.previousValue || this.getValue().toValueString() !== this.originalValue.toValueString();
}
