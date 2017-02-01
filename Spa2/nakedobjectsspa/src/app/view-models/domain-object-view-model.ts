import { MessageViewModel } from './message-view-model';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { PaneRouteData, InteractionMode } from '../route-data';
import { ChoiceViewModel } from './choice-view-model';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { PropertyViewModel } from './property-view-model';
import { CollectionViewModel } from './collection-view-model';
import { ParameterViewModel } from './parameter-view-model';
import * as Models from '../models';
import * as Helpers from './helpers-view-models';
import * as _ from "lodash";
import { ConfigService } from '../config.service';

export class DomainObjectViewModel extends MessageViewModel {

    constructor(
        private readonly colorService: ColorService,
        private readonly contextService: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly urlManager: UrlManagerService,
        private readonly error: ErrorService,
        private readonly configService: ConfigService,
        obj: Models.DomainObjectRepresentation,
        public routeData: PaneRouteData,
        forceReload: boolean
    ) {
        super();
        this.keySeparator = configService.config.keySeparator;
        this.reset(obj, routeData, forceReload);
    }

    private readonly keySeparator: string;
    private props: _.Dictionary<Models.Value>;
    private instanceId: string;
    unsaved: boolean;

    // IDraggableViewModel
    value: string;
    reference: string;
    selectedChoice: ChoiceViewModel | null;
    color: string;
    draggableType: string;
    draggableTitle = () => this.title;

    domainObject: Models.DomainObjectRepresentation;
    onPaneId: number;

    title: string;
    friendlyName: string;
    presentationHint: string;
    domainType: string;

    isInEdit: boolean;

    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    properties: PropertyViewModel[];
    collections: CollectionViewModel[];

    private readonly editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());

    private readonly isFormOrTransient = () => this.domainObject.extensions().interactionMode() === "form" || this.domainObject.extensions().interactionMode() === "transient";

    private readonly cancelHandler = () => this.isFormOrTransient() ? () => this.urlManager.popUrlState(this.onPaneId) : () => this.urlManager.setInteractionMode(InteractionMode.View, this.onPaneId);

    private readonly saveHandler = (): (object: Models.DomainObjectRepresentation, props: Object, paneId: number, viewSavedObject: boolean) => Promise<Models.DomainObjectRepresentation> =>
        this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

    private readonly validateHandler = () => this.domainObject.isTransient() ? this.contextService.validateSaveObject : this.contextService.validateUpdateObject;

    private handleWrappedError(reject: Models.ErrorWrapper) {
        const display = (em: Models.ErrorMap) => Helpers.handleErrorResponse(em, this, this.properties);
        this.error.handleErrorAndDisplayMessages(reject, display);
    };

    private propertyMap() {
        const pps = _.filter(this.properties, property => property.isEditable);
        return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
    };

    private wrapAction(a: ActionViewModel) {
        const wrappedInvoke = a.execute;
        a.execute = (pps: ParameterViewModel[], right?: boolean) => {
            this.setProperties();
            const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
            const prps = _.fromPairs(pairs) as _.Dictionary<Models.Value>;

            const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
            const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
            return wrappedInvoke(allpps, right)
                .catch((reject: Models.ErrorWrapper) => {
                    this.handleWrappedError(reject);
                    return Promise.reject(reject);
                });
        };
    }

    private reset(obj: Models.DomainObjectRepresentation, routeData: PaneRouteData, resetting: boolean) {
        this.domainObject = obj;
        this.onPaneId = routeData.paneId;
        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(this.keySeparator), routeData.paneId) : {};

        const actions = _.values(this.domainObject.actionMembers()) as Models.ActionMember[];
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

        this.menuItems = Helpers.createMenuItems(this.actions);

        this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id!, this.props[id!], this.onPaneId, this.propertyMap));
        this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData, resetting));

        this.unsaved = routeData.interactionMode === InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, this.configService, obj.getOid(this.keySeparator));

        this.friendlyName = this.domainObject.extensions().friendlyName();
        this.presentationHint = this.domainObject.extensions().presentationHint();
        this.domainType = this.domainObject.domainType() !;
        this.instanceId = this.domainObject.instanceId() !;
        this.draggableType = this.domainObject.domainType() !;

        const selfAsValue = (): Models.Value | null => {
            const link = this.domainObject.selfLink();
            if (link) {
                // not transient - can't drag transients so no need to set up IDraggable members on transients
                link.setTitle(this.title);
                return new Models.Value(link);
            }
            return null;
        };
        const sav = selfAsValue();

        this.value = sav ? sav.toString() : "";
        this.reference = sav ? sav.toValueString() : "";
        this.selectedChoice = sav ? new ChoiceViewModel(sav, "") : null;

        this.colorService.toColorNumberFromType(this.domainObject.domainType())
            .then(c => this.color = `${this.configService.config.objectColor}${c}`)
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.resetMessage();

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }
    }

    readonly concurrency = () => {
        return (event: any, em: Models.ErrorMap) => {
            this.routeData = this.urlManager.getRouteData().pane(this.onPaneId);
            this.contextService.getObject(this.onPaneId, this.domainObject.getOid(this.keySeparator), this.routeData.interactionMode)
                .then((obj: Models.DomainObjectRepresentation) => {
                    // cleared cached values so all values are from reloaded representation 
                    this.contextService.clearObjectValues(this.onPaneId);
                    return this.contextService.reloadObject(this.onPaneId, obj);
                })
                .then((reloadedObj: Models.DomainObjectRepresentation) => {
                    if (this.routeData.dialogId) {
                        this.urlManager.closeDialogReplaceHistory(this.routeData.dialogId, this.onPaneId);
                    }
                    this.reset(reloadedObj, this.routeData, true);
                    Helpers.handleErrorResponse(em, this, this.properties);
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
        };
    }

    readonly clientValid = () => _.every(this.properties, p => p.clientValid);

    readonly tooltip = () => Helpers.tooltip(this, this.properties);

    readonly actionsTooltip = () => Helpers.actionsTooltip(this, !!this.routeData.actionsOpen);

    readonly toggleActionMenu = () => {
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    readonly setProperties = () => _.forEach(this.editProperties(),
        p => this.contextService.setPropertyValue(this.domainObject, p.propertyRep, p.getValue(), this.onPaneId));

    readonly doEditCancel = () => {

        this.contextService.clearObjectValues(this.onPaneId);
        this.cancelHandler()();
    };

    readonly clearCachedFiles = () => _.forEach(this.properties, p => p.attachment ? p.attachment.clearCachedFile() : null);

    readonly doSave = (viewObject: boolean) => {
        this.clearCachedFiles();
        const propMap = this.propertyMap();
        this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject)
            .then((obj: Models.DomainObjectRepresentation) => this.reset(obj, this.urlManager.getRouteData().pane(this.onPaneId), true))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    readonly doSaveValidate = () => {
        const propMap = this.propertyMap();

        return this.validateHandler()(this.domainObject, propMap)
            .then(() => {
                this.resetMessage();
                return true;
            })
            .catch((reject: Models.ErrorWrapper) => {
                this.handleWrappedError(reject);
                return Promise.reject(false);
            });
    };

    readonly doEdit = () => {
        this.clearCachedFiles();
        this.contextService.clearObjectValues(this.onPaneId);
        this.contextService.getObjectForEdit(this.onPaneId, this.domainObject)
            .then((updatedObject: Models.DomainObjectRepresentation) => {
                this.reset(updatedObject, this.urlManager.getRouteData().pane(this.onPaneId), true);
                this.urlManager.pushUrlState(this.onPaneId);
                this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
            })
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    readonly doReload = () => {
        this.clearCachedFiles();
        this.contextService.reloadObject(this.onPaneId, this.domainObject)
            .then((updatedObject: Models.DomainObjectRepresentation) => this.reset(updatedObject, this.urlManager.getRouteData().pane(this.onPaneId), true))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    readonly hideEdit = () => this.isFormOrTransient() || _.every(this.properties, p => !p.isEditable);

    readonly disableActions = () => !this.actions || this.actions.length === 0;

    readonly canDropOn = (targetType: string) => this.contextService.isSubTypeOf(this.domainType, targetType);

    readonly showActions = () => !!this.urlManager.getRouteData().pane(this.onPaneId).actionsOpen;

}