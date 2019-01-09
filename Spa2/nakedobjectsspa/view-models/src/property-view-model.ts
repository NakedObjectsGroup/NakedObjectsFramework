import * as Ro from '@nakedobjects/restful-objects';
import {
    ClickHandlerService,
    ColorService,
    ConfigService,
    ContextService,
    ErrorService,
    MaskService,
    Pane,
    UrlManagerService
} from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import concat from 'lodash-es/concat';
import find from 'lodash-es/find';
import { AttachmentViewModel } from './attachment-view-model';
import { ChoiceViewModel } from './choice-view-model';
import { FieldViewModel } from './field-view-model';
import * as Helpers from './helpers-view-models';
import { IDraggableViewModel } from './idraggable-view-model';
import * as Msg from './user-messages';
import { ViewModelFactoryService } from './view-model-factory.service';

export class PropertyViewModel extends FieldViewModel implements IDraggableViewModel {

    readonly isEditable: boolean;
    readonly attachment: AttachmentViewModel | null;
    refType: 'null' | 'navigable' | 'notNavigable';
    // IDraggableViewModel
    readonly draggableType: string;

    constructor(
        public readonly propertyRep: Ro.PropertyMember,
        color: ColorService,
        error: ErrorService,
        private readonly viewModelfactory: ViewModelFactoryService,
        context: ContextService,
        private readonly maskService: MaskService,
        private readonly urlManager: UrlManagerService,
        private readonly clickHandler: ClickHandlerService,
        configService: ConfigService,
        id: string,
        private readonly previousValue: Ro.Value,
        onPaneId: Pane,
        parentValues: () => Dictionary<Ro.Value>
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

        if (fieldEntryType === Ro.EntryType.AutoComplete) {
            this.setupPropertyAutocomplete(parentValues);
        }

        if (fieldEntryType === Ro.EntryType.ConditionalChoices) {
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

    private getDigest(propertyRep: Ro.PropertyMember): string | null {
        const parent = propertyRep.parent;
        if (parent instanceof Ro.DomainObjectRepresentation) {
            if (parent.isTransient()) {
                return Ro.withNull(parent.etagDigest);
            }
        }
        return null;
    }

    private setupPropertyAutocomplete(parentValues: () => Dictionary<Ro.Value>) {
        const propertyRep = this.propertyRep;
        this.setupAutocomplete(propertyRep, parentValues, this.getDigest(propertyRep));
    }

    private setupPropertyConditionalChoices() {
        const propertyRep = this.propertyRep;
        this.setupConditionalChoices(propertyRep, this.getDigest(propertyRep));
    }

    private callIfChanged(newValue: Ro.Value, doRefresh: (newValue: Ro.Value) => void) {
        const propertyRep = this.propertyRep;
        const value = newValue || propertyRep.value();

        if (this.currentValue == null || value.toValueString() !== this.currentValue.toValueString()) {
            doRefresh(value);
            this.currentValue = value;
        }
    }

    private setupChoice(newValue: Ro.Value) {
        const propertyRep = this.propertyRep;
        if (this.entryType === Ro.EntryType.Choices) {

            const choices = propertyRep.choices()!;

            this.setupChoices(choices);

            if (this.optional) {
                const emptyChoice = new ChoiceViewModel(new Ro.Value(''), this.id);
                this.choices = concat<ChoiceViewModel>([emptyChoice], this.choices);
            }

            const currentChoice = new ChoiceViewModel(newValue, this.id);
            this.selectedChoice = find(this.choices, c => c.valuesEqual(currentChoice)) || null;
        } else if (!propertyRep.isScalar()) {
            this.selectedChoice = new ChoiceViewModel(newValue, this.id);
        }
    }

    private setupReference(value: Ro.Value, rep: Ro.IHasExtensions) {
        if (value.isNull()) {
            this.reference = '';
            this.value = this.description;
            this.formattedValue = '';
            this.refType = 'null';
        } else {
            this.reference = value!.link()!.href();
            this.value = value.toString();
            this.formattedValue = value.toString();
            this.refType = rep.extensions().notNavigable() ? 'notNavigable' : 'navigable';
        }
        if (this.entryType === Ro.EntryType.FreeForm) {
            this.description = this.description || Msg.dropPrompt;
        }
    }

    private setupReferencePropertyValue() {
        const propertyRep = this.propertyRep;
        this.refresh = (newValue: Ro.Value) => this.callIfChanged(newValue, (value: Ro.Value) => {
            this.setupChoice(value);
            this.setupReference(value, propertyRep);
        });
    }

    private setupScalarPropertyValue() {
        const propertyRep = this.propertyRep;

        const remoteMask = propertyRep.extensions().mask();
        const localFilter = this.maskService.toLocalFilter(remoteMask, propertyRep.extensions().format()!);
        this.localFilter = localFilter;

        this.refresh = (newValue: Ro.Value) => this.callIfChanged(newValue, (value: Ro.Value) => {

            this.setupChoice(value);
            Helpers.setScalarValueInView(this, this.propertyRep, value);

            if (propertyRep.entryType() === Ro.EntryType.Choices) {
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
