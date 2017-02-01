import { ItemViewModel } from './item-view-model';
import { PaneRouteData, CollectionViewState, InteractionMode } from '../route-data';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ColorService } from '../color.service';
import { ErrorService } from '../error.service';
import { ContextService } from '../context.service';
import { UrlManagerService } from '../url-manager.service';
import { ContributedActionParentViewModel } from './contributed-action-parent-view-model';
import * as Helpers from './helpers-view-models';
import * as Models from '../models';
import * as _ from "lodash";
import { ConfigService } from '../config.service';

export class CollectionViewModel extends ContributedActionParentViewModel {

    constructor(
        viewModelFactory: ViewModelFactoryService,
        private readonly colorService: ColorService,
        error: ErrorService,
        context: ContextService,
        urlManager: UrlManagerService,
        private readonly configService: ConfigService,
        public readonly collectionRep: Models.CollectionMember | Models.CollectionRepresentation,
        public readonly routeData: PaneRouteData,
        forceReload: boolean
    ) {
        super(context, viewModelFactory, urlManager, error, routeData.paneId);
        this.title = collectionRep.extensions().friendlyName();
        this.presentationHint = collectionRep.extensions().presentationHint();
        this.pluralName = collectionRep.extensions().pluralName();
        this.id = collectionRep.collectionId().toLowerCase();

        this.colorService.toColorNumberFromType(collectionRep.extensions().elementType()).
            then(c => this.color = `${this.configService.config.linkColor}${c}`).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        this.reset(routeData, forceReload);
    }

    private readonly presentationHint: string;
    private readonly template: string;
    private readonly messages: string;
    private readonly pluralName: string;

    private color: string;
    private editing: boolean;

    readonly id: string;
    readonly title: string;

    details: string;
    mayHaveItems: boolean;
    header: string[];
    currentState: CollectionViewState;

    readonly reset = (routeData: PaneRouteData, resetting: boolean) => {

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
                    }).
                    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
            } else {
                this.items = this.viewModelFactory.getItems(itemLinks!, this.currentState === CollectionViewState.Table, routeData, this);
            }
            this.currentState = state;
        }
    }


    readonly doSummary = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.Summary, this.onPaneId);
    readonly doList = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.List, this.onPaneId);
    readonly doTable = () => this.urlManager.setCollectionMemberState(this.collectionRep.collectionId(), CollectionViewState.Table, this.onPaneId);
    readonly hasTableData = () => this.items && _.some(this.items, (i: ItemViewModel) => i.tableRowViewModel);

    readonly description = () => this.details.toString();

    readonly disableActions = () => this.editing || !this.actions || this.actions.length === 0;

    readonly actionMember = (id: string, keySeparator: string) => {
        const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
        if (actionViewModel) {
            return actionViewModel.actionRep;
        }
        throw new Error(`no member ${id} on ${this.id}`);
    }

    private hasActionMember(id: string) {
        return !!_.find(this.actions, a => a.actionRep.actionId() === id);
    }

    readonly hasMatchingLocallyContributedAction = (id: string) => {
        return id && this.actions && this.actions.length > 0 && this.hasActionMember(id);
    }
}