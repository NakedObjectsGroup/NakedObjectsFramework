



import * as Messageviewmodel from './message-view-model';
import * as Idraggableviewmodel from './idraggable-view-model';
import * as Colorservice from '../color.service';
import * as Contextservice from '../context.service';
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Urlmanagerservice from '../url-manager.service';
import * as Errorservice from '../error.service';
import * as Routedata from '../route-data';
import * as Models from '../models';
import * as Choiceviewmodel from './choice-view-model';
import * as Actionviewmodel from './action-view-model';
import * as Menuitemviewmodel from './menu-item-view-model';
import * as Propertyviewmodel from './property-view-model';
import * as Collectionviewmodel from './collection-view-model';
import * as Parameterviewmodel from './parameter-view-model';
import * as Helpersviewmodels from './helpers-view-models';
import * as Config from '../config';
import { Subject } from 'rxjs/Subject';
import * as _ from "lodash";

export class DomainObjectViewModel extends Messageviewmodel.MessageViewModel implements Idraggableviewmodel.IDraggableViewModel {

    constructor(private colorService: Colorservice.ColorService,
        private contextService: Contextservice.ContextService,
        private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService,
        private urlManager: Urlmanagerservice.UrlManagerService,
        private error: Errorservice.ErrorService) {
        super();
    }

    private routeData: Routedata.PaneRouteData;
    private props: _.Dictionary<Models.Value>;
    private instanceId: string;
    private unsaved: boolean;

    // IDraggableViewModel
    value: string;
    reference: string;
    selectedChoice: Choiceviewmodel.ChoiceViewModel;
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

    actions: Actionviewmodel.ActionViewModel[];
    menuItems: Menuitemviewmodel.MenuItemViewModel[];
    properties: Propertyviewmodel.PropertyViewModel[];
    collections: Collectionviewmodel.CollectionViewModel[];

    private editProperties = () => _.filter(this.properties, p => p.isEditable && p.isDirty());

    private isFormOrTransient = () => this.domainObject.extensions().interactionMode() === "form" || this.domainObject.extensions().interactionMode() === "transient";

    private cancelHandler = () => this.isFormOrTransient() ?
        () => this.urlManager.popUrlState(this.onPaneId) :
        () => this.urlManager.setInteractionMode(Routedata.InteractionMode.View, this.onPaneId);

    private saveHandler = () => this.domainObject.isTransient() ? this.contextService.saveObject : this.contextService.updateObject;

    private validateHandler = () => this.domainObject.isTransient() ? this.contextService.validateSaveObject : this.contextService.validateUpdateObject;

    private handleWrappedError = (reject: Models.ErrorWrapper) => {
        const display = (em: Models.ErrorMap) => this.viewModelFactory.handleErrorResponse(em, this, this.properties);
        this.error.handleErrorAndDisplayMessages(reject, display);
    };

    private propertyMap = () => {
        const pps = _.filter(this.properties, property => property.isEditable);
        return _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Models.Value>;
    };

    private wrapAction(a: Actionviewmodel.ActionViewModel) {
        const wrappedInvoke = a.execute;
        a.execute = (pps: Parameterviewmodel.ParameterViewModel[], right?: boolean) => {
            this.setProperties();
            const pairs = _.map(this.editProperties(), p => [p.id, p.getValue()]);
            const prps = (<any>_.fromPairs)(pairs) as _.Dictionary<Models.Value>;

            const parmValueMap = _.mapValues(a.invokableActionRep.parameters(), p => ({ parm: p, value: prps[p.id()] }));
            const allpps = _.map(parmValueMap, o => this.viewModelFactory.parameterViewModel(o.parm, o.value, this.onPaneId));
            return wrappedInvoke(allpps, right).
                catch((reject: Models.ErrorWrapper) => {
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

    refresh(routeData: Routedata.PaneRouteData) {

        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== Routedata.InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== Routedata.InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        _.forEach(this.properties, p => p.refresh(this.props[p.id]));
        _.forEach(this.collections, c => c.refresh(this.routeData, false));

        this.unsaved = routeData.interactionMode === Routedata.InteractionMode.Transient;

        this.title = this.unsaved ? `Unsaved ${this.domainObject.extensions().friendlyName()}` : this.domainObject.title();

        this.title = this.title + Models.dirtyMarker(this.contextService, this.domainObject.getOid());

        if (routeData.interactionMode === Routedata.InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }

        // leave message from previous refresh 
        this.clearMessage();
    }

    reset(obj: Models.DomainObjectRepresentation, routeData: Routedata.PaneRouteData): DomainObjectViewModel {
        this.domainObject = obj;
        this.onPaneId = routeData.paneId;
        this.routeData = routeData;
        const iMode = this.domainObject.extensions().interactionMode();
        this.isInEdit = routeData.interactionMode !== Routedata.InteractionMode.View || iMode === "form" || iMode === "transient";
        this.props = routeData.interactionMode !== Routedata.InteractionMode.View ? this.contextService.getCurrentObjectValues(this.domainObject.id(), routeData.paneId) : {};

        const actions = _.values(this.domainObject.actionMembers()) as Models.ActionMember[];
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, this.routeData));

        this.menuItems = Helpersviewmodels.createMenuItems(this.actions);

        this.properties = _.map(this.domainObject.propertyMembers(), (property, id) => this.viewModelFactory.propertyViewModel(property, id, this.props[id], this.onPaneId, this.propertyMap));
        this.collections = _.map(this.domainObject.collectionMembers(), collection => this.viewModelFactory.collectionViewModel(collection, this.routeData));

        this.unsaved = routeData.interactionMode === Routedata.InteractionMode.Transient;

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
        this.selectedChoice = sav ? Choiceviewmodel.ChoiceViewModel.create(sav, "") : null;

        this.colorService.toColorNumberFromType(this.domainObject.domainType()).
            then(c => this.color = `${Config.objectColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.resetMessage();

        if (routeData.interactionMode === Routedata.InteractionMode.Form) {
            _.forEach(this.actions, a => this.wrapAction(a));
        }


        return this as DomainObjectViewModel;
    }

    concurrency() {
        return (event: any, em: Models.ErrorMap) => {
            this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
            this.contextService.getObject(this.onPaneId, this.domainObject.getOid(), this.routeData.interactionMode).
                then(obj => {
                    // cleared cached values so all values are from reloaded representation 
                    this.contextService.clearObjectValues(this.onPaneId);
                    return this.contextService.reloadObject(this.onPaneId, obj);
                }).
                then(reloadedObj => {
                    if (this.routeData.dialogId) {
                        this.urlManager.closeDialogReplaceHistory(this.onPaneId);
                    }
                    this.reset(reloadedObj, this.routeData);
                    this.viewModelFactory.handleErrorResponse(em, this, this.properties);
                }).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
        }
    }

    clientValid = () => _.every(this.properties, p => p.clientValid);

    tooltip = () => Helpersviewmodels.tooltip(this, this.properties);

    actionsTooltip = () => Helpersviewmodels.actionsTooltip(this, !!this.routeData.actionsOpen);

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
        this.saveHandler()(this.domainObject, propMap, this.onPaneId, viewObject).
            then(obj => this.reset(obj, this.urlManager.getRouteData().pane()[this.onPaneId])).
            catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doSaveValidate = () => {
        const propMap = this.propertyMap();

        return this.validateHandler()(this.domainObject, propMap).
            then(() => {
                this.resetMessage();
                return true;
            }).
            catch((reject: Models.ErrorWrapper) => {
                this.handleWrappedError(reject);
                return Promise.reject(false);
            });
    };

    doEdit = () => {
        this.contextService.updateValues(); // for other panes
        this.clearCachedFiles();
        this.contextService.clearObjectValues(this.onPaneId);
        this.contextService.getObjectForEdit(this.onPaneId, this.domainObject).
            then(updatedObject => {
                this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]);
                this.urlManager.pushUrlState(this.onPaneId);
                this.urlManager.setInteractionMode(Routedata.InteractionMode.Edit, this.onPaneId);
            }).
            catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    };

    doReload = () => {
        this.contextService.updateValues();
        this.clearCachedFiles();
        this.contextService.reloadObject(this.onPaneId, this.domainObject)
            .then(updatedObject => this.reset(updatedObject, this.urlManager.getRouteData().pane()[this.onPaneId]))
            .catch((reject: Models.ErrorWrapper) => this.handleWrappedError(reject));
    }

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
