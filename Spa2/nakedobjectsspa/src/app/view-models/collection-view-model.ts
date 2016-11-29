import { ItemViewModel } from './item-view-model';
import { PaneRouteData, CollectionViewState, InteractionMode } from '../route-data';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { ContextService } from '../context.service';
import { UrlManagerService } from '../url-manager.service';
import * as Helpers from './helpers-view-models';
import * as Config from "../config";
import * as Models from '../models';
import * as _ from "lodash";
import { ContributedActionParentViewModel} from './contributed-action-parent-view-model';

export class CollectionViewModel extends ContributedActionParentViewModel{

    constructor(
         viewModelFactory: ViewModelFactoryService,
        private colorService: ColorService,
         error: ErrorService,
         context: ContextService,
         urlManager: UrlManagerService,
        public collectionRep: Models.CollectionMember | Models.CollectionRepresentation,
        private routeData: PaneRouteData
    ) {
        super(context, viewModelFactory, urlManager, error);
        this.onPaneId = routeData.paneId;
        this.title = collectionRep.extensions().friendlyName();
        this.presentationHint = collectionRep.extensions().presentationHint();
        this.pluralName = collectionRep.extensions().pluralName();

        this.colorService.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(c => this.color = `${Config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.reset(routeData, true);
    }

    private reset = (routeData: PaneRouteData, resetting: boolean) => {

        let state = routeData.collections[this.collectionRep.collectionId()];

        // collections are always shown as summary on transient 
        if (routeData.interactionMode === InteractionMode.Transient) {
            state = CollectionViewState.Summary;
        }

        function getDefaultTableState(exts: Models.Extensions) {
            if (exts.renderEagerly()) {
                return exts.tableViewColumns() || exts.tableViewTitle() ? CollectionViewState.Table : CollectionViewState.List;
            }
            return CollectionViewState.Summary;
        }


        if (state == null) {
            state = getDefaultTableState(this.collectionRep.extensions());
        }

        this.editing = routeData.interactionMode === InteractionMode.Edit;

        // clear any previous messages
        //this.resetMessage();

        if (resetting || state !== this.currentState) {

            const size = this.collectionRep.size();
            const itemLinks = this.collectionRep.value();

            if (size > 0 || size == null) {
                this.mayHaveItems = true;
            }
            this.details = Helpers.getCollectionDetails(size);
            const getDetails = itemLinks == null || state === CollectionViewState.Table;

            const actions = this.collectionRep.actionMembers();
            this.setActions(actions, routeData);


            if (state === CollectionViewState.Summary) {
                this.items = [];

            } else if (getDetails) {
                this.context.getCollectionDetails(this.collectionRep as Models.CollectionMember, state, resetting).
                    then(details => {
                        this.items = this.viewModelFactory.getItems(details.value(),
                            state === CollectionViewState.Table,
                            routeData,
                            this);
                        this.details = Helpers.getCollectionDetails(this.items.length);
                        //this.allSelected = _.every(this.items, item => item.selected);
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                this.items = this.viewModelFactory.getItems(itemLinks, this.currentState === CollectionViewState.Table, routeData, this);
                //this.allSelected = _.every(this.items, item => item.selected);   
            }
            this.currentState = state;
        } else {
            //this.allSelected = _.every(this.items, item => item.selected);
        }
    }


    doSummary = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.Summary, this.onPaneId);
    doList = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.List, this.onPaneId);
    doTable = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.Table, this.onPaneId);

    hasTableData = () => this.items && _.some(this.items, i => i.tableRowViewModel);

    title: string;
    details: string;
    pluralName: string;
    color: string;
    mayHaveItems: boolean;
    editing: boolean;
   
    header: string[];
  
    currentState: CollectionViewState;

    presentationHint: string;
    template: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    messages: string;

    description = () => this.details.toString();

    disableActions = () => this.editing || !this.actions || this.actions.length === 0;
  
    actionMember = (id: string) => {
        const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
        return actionViewModel ? actionViewModel.actionRep : null;
    }

    setActions(actions: _.Dictionary<Models.ActionMember>, routeData: PaneRouteData) {
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);
        _.forEach(this.actions, a => this.decorate(a));
    }

    hasMatchingLocallyContributedAction(id: string) {
        return id && this.actions && this.actions.length > 0 && !!this.actionMember(id);
    }
}