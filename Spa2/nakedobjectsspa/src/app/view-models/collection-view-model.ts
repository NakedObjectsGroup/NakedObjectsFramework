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

export class CollectionViewModel {

    constructor(
        private viewModelFactory: ViewModelFactoryService,
        private colorService: ColorService,
        private error: ErrorService,
        private context: ContextService,
        private urlManager: UrlManagerService,
        public collectionRep: Models.CollectionMember | Models.CollectionRepresentation,
        private routeData: PaneRouteData
    ) {

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