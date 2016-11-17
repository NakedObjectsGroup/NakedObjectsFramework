import { ItemViewModel } from './item-view-model';
import { PaneRouteData, CollectionViewState } from '../route-data';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import * as Models from '../models';
import * as _ from "lodash";
import * as Viewmodelfactoryservice from '../view-model-factory.service';
import * as Routedata from '../route-data';
import * as Colorservice from '../color.service';
import * as Config from "../config";
import * as Errorservice from '../error.service';
import * as Contextservice from '../context.service';
import * as Urlmanagerservice from '../url-manager.service';
import * as Helpersviewmodels from './helpers-view-models';

export class CollectionViewModel {

    constructor(private viewModelFactory: Viewmodelfactoryservice.ViewModelFactoryService,
                private colorService: Colorservice.ColorService,
                private error: Errorservice.ErrorService,
                private context: Contextservice.ContextService,
                private urlManager: Urlmanagerservice.UrlManagerService,
                public collectionRep: Models.CollectionMember | Models.CollectionRepresentation,
                private routeData: PaneRouteData) {


        this.onPaneId = routeData.paneId;
        this.title = collectionRep.extensions().friendlyName();
        this.presentationHint = collectionRep.extensions().presentationHint();
        this.pluralName = collectionRep.extensions().pluralName();

        this.colorService.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(c => this.color = `${Config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.refresh(routeData, true);
    }

    refresh = (routeData: PaneRouteData, resetting: boolean) => {

        let state = routeData.collections[this.collectionRep.collectionId()];

        // collections are always shown as summary on transient 
        if (routeData.interactionMode === Routedata.InteractionMode.Transient) {
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

        this.editing = routeData.interactionMode === Routedata.InteractionMode.Edit;

        // clear any previous messages
        //this.resetMessage();

        if (resetting || state !== this.currentState) {

            const size = this.collectionRep.size();
            const itemLinks = this.collectionRep.value();

            if (size > 0 || size == null) {
                this.mayHaveItems = true;
            }
            this.details = Helpersviewmodels.getCollectionDetails(size);
            const getDetails = itemLinks == null || state === CollectionViewState.Table;

            if (state === CollectionViewState.Summary) {
                this.items = [];

            } else if (getDetails) {
                this.context.getCollectionDetails(this.collectionRep as Models.CollectionMember, state, resetting).
                    then(details => {
                        this.items = this.viewModelFactory.getItems(details.value(),
                            state === CollectionViewState.Table,
                            routeData,
                            this);
                        this.details = Helpersviewmodels.getCollectionDetails(this.items.length);
                        //collectionViewModel.allSelected = _.every(collectionViewModel.items, item => item.selected);
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                this.items = this.viewModelFactory.getItems(itemLinks, this.currentState === CollectionViewState.Table, routeData, this);
                //collectionViewModel.allSelected = _.every(collectionViewModel.items, item => item.selected);   
            }
            this.currentState = state;
        } else {
            //collectionViewModel.allSelected = _.every(collectionViewModel.items, item => item.selected);
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
    items: ItemViewModel[];
    header: string[];
    onPaneId: number;
    currentState: CollectionViewState;

    presentationHint: string;
    template: string;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    messages: string;

    description = () => this.details.toString();
  
    disableActions = () => this.editing || !this.actions || this.actions.length === 0;
    allSelected = () => _.every(this.items, item => item.selected);

    selectAll() { }
}