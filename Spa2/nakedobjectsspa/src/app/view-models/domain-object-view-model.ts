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
import { Subject } from 'rxjs/Subject';
import * as Models from '../models';
import * as Helpers from './helpers-view-models';
import * as Config from '../config';
import * as _ from "lodash";

export class DomainObjectViewModel extends MessageViewModel {

    constructor(private colorService: ColorService,
        private contextService: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService) {
        super();
    }

    private routeData: PaneRouteData;
    private props: _.Dictionary<Models.Value>;
    private instanceId: string;
    unsaved: boolean;

    // IDraggableViewModel
    value: string;
    reference: string;
    selectedChoice: ChoiceViewModel;
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

    private editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());

    private isFormOrTransient = () => this.domainObject.extensions().interactionMode() === "form" || this.domainObject.extensions().interactionMode() === "transient";

    private cancelHandler = () => this.isFormOrTransient() ? () => this.urlManager.popUrlState(this.onPaneId) : () => this.urlManager.setInteractionMode(InteractionMode.View, this.onPaneId);

    private saveHandler = (): (object: Models.DomainObjectRepresentation, props: Object, paneId: number, viewSavedObject: boolean) => Promise<Models.DomainObjectRepresentation> =>
        this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

    private validateHandler = () => this.domainObject.isTransient() ? this.contextService.validateSaveObject : this.contextService.validateUpdateObject;

    private handleWrappedError = (reject: Models.ErrorWrapper) => {
        const display = (em: Models.ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
        this.error.handleErrorAndDisplayMessages(reject, display);
    };

    private propertyMap = () => {
        const pps = _.filter(this.properties, property => property.isEditable);
        return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
    };

    private wrapAction(a: ActionViewModel) {
        const wrappedInvoke = a.execute;
        a.execute = (pps: ParameterViewModel[], right?: boolean) => {
            this.setProperties();
            const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
            const prps = (<any>_.fromPairs)(pairs) as _.Dictionary<Models.Value>;

            const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
            const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
            return wrappedInvoke(allpps, right)
                .catch((reject: Models.ErrorWrapper) => {
                    this.handleWrappedError(reject);
                    return Promise.reject(reject);
                });
        };
    }

    private editComplete = () => {
        this.contextService.updateValues();
        this.contextService.clearObjectUpdater(this.onPaneId);
    };


    // must be careful with this - OK for changes on client but after server updates should use  reset
    // because parameters may have appeared or disappeared etc and refesh just updates existing views. 
    // So OK for view state changes but not eg for a parameter that disappears after saving

    refresh(routeData: PaneRouteData) {

        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        _.forEach(this.properties, p => p.refresh(this.props[p.id]));
        _.forEach(this.collections, c => c.refresh(this.routeData, false));

        this.unsaved = routeData.interactionMode === InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, this.domainObject.getOid());

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }

        // leave message from previous refresh 
        this.clearMessage();
    }

    reset(obj: Models.DomainObjectRepresentation, routeData: PaneRouteData): DomainObjectViewModel {
        this.domainObject = obj;
        this.onPaneId = routeData.paneId;
        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        const actions = _.values(this.domainObject.actionMembers()) as Models.ActionMember[];
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

        this.menuItems = Helpers.createMenuItems(this.actions);

        this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
        this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

        this.unsaved = routeData.interactionMode === InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, obj.getOid());

        this.friendlyName = this.domainObject.extensions().friendlyName();
        this.presentationHint = this.domainObject.extensions().presentationHint();
        this.domainType = this.domainObject.domainType();
        this.instanceId = this.domainObject.instanceId();
        this.draggableType = this.domainObject.domainType();

        const selfAsValue = () => {
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
        this.selectedChoice = sav ? ChoiceViewModel.create(sav, "") : null;

        this.colorService.toColorNumberFromType(this.domainObject.domainType())
            .then(c => this.color = `${Config.objectColor}${c}`)
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.resetMessage();

        if (routeData.interactionMode === InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }


        return this as DomainObjectViewModel;
    }

    concurrency() {
        return (event: any, em: Models.ErrorMap) => {
            this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
            this.contextService.getObject(this.onPaneId, this.domainObject.getOid(), this.routeData.interactionMode)
                .then((obj: Models.DomainObjectRepresentation) => {
                    // cleared cached values so all values are from reloaded representation 
                    this.contextService.clearObjectValues(this.onPaneId);
                    return this.contextService.reloadObject(this.onPaneId, obj);
                })
                .then((reloadedObj: Models.DomainObjectRepresentation) => {
                    if (this.routeData.dialogId) {
                        this.urlManager.closeDialogReplaceHistory(this.onPaneId);
                    }
                    this.reset(reloadedObj, this.routeData);
                    this.viewModelFactory.handleErrorResponse(em, this, this.properties);
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
        };
    }

    clientValid = () => _.every(this.properties, p => p.clientValid);

    tooltip = () => Helpers.tooltip(this, this.properties);

    actionsTooltip = () => Helpers.actionsTooltip(this, !!this.routeData.actionsOpen);

    toggleActionMenu = () => {
        this.contextService.updateValues();
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    setProperties = () => _.forEach(this.editProperties(),
        p => this.contextService.setPropertyValue(this.domainObject, p.propertyRep, p.getValue(), this.onPaneId));

    doEditCancel = () => {
        this.editComplete();
        this.contextService.clearObjectValues(this.onPaneId);
        this.cancelHandler()();
    };

    clearCachedFiles = () => _.forEach(this.properties, p => p.attachment ? p.attachment.clearCachedFile() : null);

    doSave = (viewObject: boolean) => {
        this.clearCachedFiles();
        this.contextService.updateValues();
        const propMap = this.propertyMap();
        this.contextService.clearObjectUpdater(this.onPaneId);
        this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject)
            .then((obj: Models.DomainObjectRepresentation) => this.reset(obj, this.urlManager.getRouteData().pane()[this.onPaneId]))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doSaveValidate = () => {
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

    doEdit = () => {
        this.contextService.updateValues(); // for other panes
        this.clearCachedFiles();
        this.contextService.clearObjectValues(this.onPaneId);
        this.contextService.getObjectForEdit(this.onPaneId, this.domainObject)
            .then((updatedObject: Models.DomainObjectRepresentation) => {
                this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                this.urlManager.pushUrlState(this.onPaneId);
                this.urlManager.setInteractionMode(InteractionMode.Edit, this.onPaneId);
            })
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doReload = () => {
        this.contextService.updateValues();
        this.clearCachedFiles();
        this.contextService.reloadObject(this.onPaneId, this.domainObject)
            .then((updatedObject: Models.DomainObjectRepresentation) => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };
    hideEdit = () => this.isFormOrTransient() || _.every(this.properties, p => !p.isEditable);

    disableActions = () => !this.actions || this.actions.length === 0;

    canDropOn = (targetType: string) => this.contextService.isSubTypeOf(this.domainType, targetType);


    showActions() {
        return !!this.urlManager.getRouteData().pane()[this.onPaneId].actionsOpen;
    }

    propertyChanged() {
        this.propertyChangedSource.next(true);
        this.propertyChangedSource.next(false);
    }

    private propertyChangedSource = new Subject<boolean>();

    propertyChanged$ = this.propertyChangedSource.asObservable();


}