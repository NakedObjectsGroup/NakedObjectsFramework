import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { PaneRouteData, CollectionViewState } from '../route-data';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import find from 'lodash-es/find';
import * as Helpers from './helpers-view-models';
import * as Models from '../models';
import * as Msg from '../user-messages';
import { ContributedActionParentViewModel } from './contributed-action-parent-view-model';
import { LoggerService } from '../logger.service';
import { IMenuHolderViewModel } from './imenu-holder-view-model';

export class ListViewModel extends ContributedActionParentViewModel implements IMenuHolderViewModel {

    constructor(
        private readonly colorService: ColorService,
        context: ContextService,
        viewModelFactory: ViewModelFactoryService,
        urlManager: UrlManagerService,
        error: ErrorService,
        private readonly loggerService: LoggerService,
        list: Models.ListRepresentation,
        public routeData: PaneRouteData
    ) {
        super(context, viewModelFactory, urlManager, error, routeData.paneId);
        this.reset(list, routeData);
        const actions = this.listRep.actionMembers();
        this.setActions(actions, routeData);
    }

    private page: number;
    private pageSize: number;
    private numPages: number;
    private state: CollectionViewState;
    id: string;
    listRep: Models.ListRepresentation;
    size: number;
    header: string[] | null;
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    description: () => string;
    readonly name = "item";

    private readonly earlierDisabled = () => this.page === 1 || this.numPages === 1;

    private readonly laterDisabled = () => this.page === this.numPages || this.numPages === 1;

    // tslint:disable:member-ordering
    readonly pageFirstDisabled = this.earlierDisabled;
    readonly pageLastDisabled = this.laterDisabled;
    readonly pageNextDisabled = this.laterDisabled;
    readonly pagePreviousDisabled = this.earlierDisabled;
    // tslint:enable:member-ordering

    private readonly recreate = (page: number, pageSize: number) => {
        return this.routeData.objectId
            ? this.context.getListFromObject(this.routeData, page, pageSize)
            : this.context.getListFromMenu(this.routeData, page, pageSize);
    }

    readonly currentPaneData = () => this.urlManager.getRouteData().pane(this.onPaneId) !;

    private readonly pageOrRecreate = (newPage: number, newPageSize: number, newState?: CollectionViewState) => {
        this.recreate(newPage, newPageSize)
            .then((list: Models.ListRepresentation) => {
                this.urlManager.setListPaging(newPage, newPageSize, newState || this.routeData.state, this.onPaneId);
                this.routeData = this.currentPaneData();
                this.reset(list, this.routeData);
            })
            .catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
    }

    private readonly setPage = (newPage: number, newState: CollectionViewState) => {
        this.pageOrRecreate(newPage, this.pageSize, newState);
    }

    private readonly updateItems = (value: Models.Link[]) => {
        this.items = this.viewModelFactory.getItems(value,
            this.state === CollectionViewState.Table,
            this.routeData,
            this);

        const totalCount = this.listRep.pagination() !.totalCount;
        const count = this.items.length;
        this.size = count;
        if (count > 0) {
            this.description = () => Msg.pageMessage(this.page, this.numPages, count, totalCount);
        } else {
            this.description = () => Msg.noItemsFound;
        }
    }

    readonly hasTableData = () => this.listRep.hasTableData();

    readonly refresh = (routeData: PaneRouteData) => {

        this.routeData = routeData;
        if (this.state !== routeData.state) {
            if (routeData.state === CollectionViewState.Table && !this.hasTableData()) {
                this.recreate(this.page, this.pageSize)
                    .then(list => {
                        this.state = list.hasTableData() ? CollectionViewState.Table : CollectionViewState.List;
                        this.listRep = list;
                        this.updateItems(list.value());
                    })
                    .catch((reject: Models.ErrorWrapper) => {
                        this.error.handleError(reject);
                    });
            } else {
                this.state = routeData.state;
                this.updateItems(this.listRep.value());
            }
        }
    }

    readonly reset = (list: Models.ListRepresentation, routeData: PaneRouteData) => {
        this.listRep = list;
        this.routeData = routeData;

        this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

        this.page = this.listRep.pagination() !.page;
        this.pageSize = this.listRep.pagination() !.pageSize;
        this.numPages = this.listRep.pagination() !.numPages;

        this.state = this.listRep.hasTableData() ? CollectionViewState.Table : CollectionViewState.List;
        this.updateItems(list.value());
    }

    readonly toggleActionMenu = () => {
        if (this.noActions()) { return; }
        this.urlManager.toggleObjectMenu(this.onPaneId);
    }

    readonly pageNext = () => {
        if (this.pageNextDisabled()) { return; }
        this.setPage(this.page < this.numPages ? this.page + 1 : this.page, this.state);
    }

    readonly pagePrevious = () => {
        if (this.pagePreviousDisabled()) { return; }
        this.setPage(this.page > 1 ? this.page - 1 : this.page, this.state);
    }

    readonly pageFirst = () => {
        if (this.pageFirstDisabled()) { return; }
        this.setPage(1, this.state);
    }

    readonly pageLast = () => {
        if (this.pageLastDisabled()) { return; }
        this.setPage(this.numPages, this.state);
    }

    readonly doSummary = () => {
        this.urlManager.setListState(CollectionViewState.Summary, this.onPaneId);
    }

    readonly doList = () => {
        this.urlManager.setListState(CollectionViewState.List, this.onPaneId);
    }

    readonly doTable = () => {
        this.urlManager.setListState(CollectionViewState.Table, this.onPaneId);
    }

    readonly reload = () => {
        this.context.clearCachedList(this.onPaneId, this.routeData.page, this.routeData.pageSize);
        this.setPage(this.page, this.state);
    }

    readonly noActions = () => !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;

    readonly actionsTooltip = () => Helpers.actionsTooltip(this, !!this.routeData.actionsOpen);

    readonly actionMember = (id: string) => {
        const actionViewModel = find(this.actions, a => a.actionRep.actionId() === id);

        if (actionViewModel) {
            return actionViewModel.actionRep;
        }
        return this.loggerService.throw(`no actionviewmodel ${id} on ${this.id}`);
    }

    readonly showActions = () => {
        return !!this.currentPaneData().actionsOpen;
    }
}
