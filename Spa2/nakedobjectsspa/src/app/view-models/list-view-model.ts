import { MessageViewModel } from './message-view-model';
import { ColorService } from '../color.service';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { UrlManagerService } from '../url-manager.service';
import { ErrorService } from '../error.service';
import { PaneRouteData, CollectionViewState } from '../route-data';
import { ItemViewModel } from './item-view-model';
import { ActionViewModel } from './action-view-model';
import { MenuItemViewModel } from './menu-item-view-model';
import { ParameterViewModel } from './parameter-view-model';
import * as _ from "lodash";
import * as Helpers from './helpers-view-models';
import * as Models from '../models';
import * as Msg from '../user-messages';

export class ListViewModel extends MessageViewModel {

    constructor(private colorService: ColorService,
        private context: ContextService,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService,
        private error: ErrorService, list: Models.ListRepresentation, routeData: PaneRouteData) {
        super();

        this.reset(list, routeData);
    }
    onPaneId: number;

    private routeData: PaneRouteData;
    private page: number;
    private pageSize: number;
    private numPages: number;
    private state: CollectionViewState;

    allSelected = () => _.every(this.items, item => item.selected);

    id: string;
    listRep: Models.ListRepresentation;
    size: number;
    pluralName: string;
    header: string[];
    items: ItemViewModel[];
    actions: ActionViewModel[];
    menuItems: MenuItemViewModel[];
    description: () => string;

    private recreate = (page: number, pageSize: number) => {
        return this.routeData.objectId
            ? this.context.getListFromObject(this.routeData, page, pageSize)
            : this.context.getListFromMenu(this.routeData, page, pageSize);
    };

    private pageOrRecreate = (newPage: number, newPageSize: number, newState?: CollectionViewState) => {
        this.recreate(newPage, newPageSize)
            .then((list: Models.ListRepresentation) => {
                this.urlManager.setListPaging(newPage, newPageSize, newState || this.routeData.state, this.onPaneId);
                this.routeData = this.urlManager.getRouteData().pane()[this.onPaneId];
                this.reset(list, this.routeData);
            })
            .catch((reject: Models.ErrorWrapper) => {
                const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                this.error.handleErrorAndDisplayMessages(reject, display);
            });
    };

    private setPage = (newPage: number, newState: CollectionViewState) => {
        this.context.updateValues();
        this.pageOrRecreate(newPage, this.pageSize, newState);
    };

    private earlierDisabled = () => this.page === 1 || this.numPages === 1;

    private laterDisabled = () => this.page === this.numPages || this.numPages === 1;

    pageFirstDisabled = this.earlierDisabled;

    pageLastDisabled = this.laterDisabled;

    pageNextDisabled = this.laterDisabled;

    pagePreviousDisabled = this.earlierDisabled;

    private updateItems(value: Models.Link[]) {
        this.items = this.viewModelFactory.getItems(value,
            this.state === CollectionViewState.Table,
            this.routeData,
            this);

        const totalCount = this.listRep.pagination().totalCount;
        const count = this.items.length;
        this.size = count;
        if (count > 0) {
            this.description = () => Msg.pageMessage(this.page, this.numPages, count, totalCount);
        } else {
            this.description = () => Msg.noItemsFound;
        }
    }

    hasTableData = () => this.listRep.hasTableData();

    private collectionContributedActionDecorator(actionViewModel: ActionViewModel) {
        const wrappedInvoke = actionViewModel.execute;
        actionViewModel.execute = (pps: ParameterViewModel[], right?: boolean) => {
            const selected = _.filter(this.items, i => i.selected);

            if (selected.length === 0) {

                const em = new Models.ErrorMap({}, 0, Msg.noItemsSelected);
                const rp = new Models.ErrorWrapper(Models.ErrorCategory.HttpClientError, Models.HttpStatusCode.UnprocessableEntity, em);

                return <any>Promise.reject(rp);
            }

            const getParms = (action: Models.IInvokableAction) => {

                const parms = _.values(action.parameters()) as Models.Parameter[];
                const contribParm = _.find(parms, p => p.isCollectionContributed());
                const parmValue = new Models.Value(_.map(selected, i => i.link));
                const collectionParmVm = this.viewModelFactory.parameterViewModel(contribParm, parmValue, this.onPaneId);

                const allpps = _.clone(pps);
                allpps.push(collectionParmVm);
                return allpps;
            };
            if (actionViewModel.invokableActionRep) {
                return wrappedInvoke(getParms(actionViewModel.invokableActionRep), right);
            }

            return this.context.getActionDetails(actionViewModel.actionRep as Models.ActionMember)
                .then((details: Models.ActionRepresentation) => wrappedInvoke(getParms(details), right));
        };
    }

    private collectionContributedInvokeDecorator(actionViewModel: ActionViewModel) {

        const showDialog = () =>
            this.context.getInvokableAction(actionViewModel.actionRep as Models.ActionMember).then(invokableAction => _.keys(invokableAction.parameters()).length > 1);

        // make sure not null while waiting for promise to assign correct function 
        actionViewModel.doInvoke = () => { };

        const invokeWithDialog = (right?: boolean) => {
            this.context.clearDialogValues(this.onPaneId);
            this.urlManager.setDialogOrMultiLineDialog(actionViewModel.actionRep, this.onPaneId);
        };

        const invokeWithoutDialog = (right?: boolean) =>
            actionViewModel.execute([], right)
                .then(result => this.setMessage(result.shouldExpectResult() ? result.warningsOrMessages() || Msg.noResultMessage : ""))
                .catch((reject: Models.ErrorWrapper) => {
                    const display = (em: Models.ErrorMap) => this.setMessage(em.invalidReason() || em.warningMessage);
                    this.error.handleErrorAndDisplayMessages(reject, display);
                });

        showDialog().then(show => actionViewModel.doInvoke = show ? invokeWithDialog : invokeWithoutDialog).catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    private decorate(actionViewModel: ActionViewModel) {
        this.collectionContributedActionDecorator(actionViewModel);
        this.collectionContributedInvokeDecorator(actionViewModel);
    }

    refresh(routeData: PaneRouteData) {

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
                this.updateItems(this.listRep.value());
            }
        }
    }

    private reset(list: Models.ListRepresentation, routeData: PaneRouteData) {
        this.listRep = list;
        this.routeData = routeData;

        this.id = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

        this.onPaneId = routeData.paneId;

        this.pluralName = "Objects";
        this.page = this.listRep.pagination().page;
        this.pageSize = this.listRep.pagination().pageSize;
        this.numPages = this.listRep.pagination().numPages;

        this.state = this.listRep.hasTableData() ? CollectionViewState.Table : CollectionViewState.List;
        this.updateItems(list.value());

        const actions = this.listRep.actionMembers();
        this.actions = _.map(actions, action => this.viewModelFactory.actionViewModel(action, this, routeData));
        this.menuItems = Helpers.createMenuItems(this.actions);

        _.forEach(this.actions, a => this.decorate(a));
    }

    toggleActionMenu = () => {
        if (this.disableActions()) return;
        this.urlManager.toggleObjectMenu(this.onPaneId);
    };

    pageNext = () => {
        if (this.pageNextDisabled()) return;
        this.setPage(this.page < this.numPages ? this.page + 1 : this.page, this.state);
    };

    pagePrevious = () => {
        if (this.pagePreviousDisabled()) return;
        this.setPage(this.page > 1 ? this.page - 1 : this.page, this.state);
    };

    pageFirst = () => {
        if (this.pageFirstDisabled()) return;
        this.setPage(1, this.state);
    };

    pageLast = () => {
        if (this.pageLastDisabled()) return;
        this.setPage(this.numPages, this.state);
    };

    doSummary = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.Summary, this.onPaneId);
    };

    doList = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.List, this.onPaneId);
    };

    doTable = () => {
        this.context.updateValues();
        this.urlManager.setListState(CollectionViewState.Table, this.onPaneId);
    };

    reload = () => {
        this.context.clearCachedList(this.onPaneId, this.routeData.page, this.routeData.pageSize);
        this.setPage(this.page, this.state);
    };

    selectAll = () => {
        const newState = !this.allSelected();

        _.each(this.items,
            (item) => {
                item.selected = newState;
            });
    };

    disableActions = () => !this.actions || this.actions.length === 0 || !this.items || this.items.length === 0;

    actionsTooltip = () => Helpers.actionsTooltip(this, !!this.routeData.actionsOpen);

    actionMember = (id: string) => {
        const actionViewModel = _.find(this.actions, a => a.actionRep.actionId() === id);
        return actionViewModel.actionRep;
    };

    showActions() {
        return !!this.urlManager.getRouteData().pane()[this.onPaneId].actionsOpen;
    }
}