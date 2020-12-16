import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as Ro from '@nakedobjects/restful-objects';
import {
    CollectionViewState,
    ConfigService,
    ContextService,
    ErrorService,
    ErrorWrapper,
    ICustomActivatedRouteData,
    LoggerService,
    PaneRouteData,
    UrlManagerService
} from '@nakedobjects/services';
import { ItemViewModel, ListViewModel, ViewModelFactoryService, DragAndDropService } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { IActionHolder } from '../action/action.component';
import { safeUnsubscribe } from '../helpers-components';

@Component({
    selector: 'nof-list',
    templateUrl: 'list.component.html',
    styleUrls: ['list.component.css']
})
export class ListComponent implements OnInit, OnDestroy {

    private ddSub: ISubscription;
    dropZones: string[] = [];

    constructor(
        private readonly activatedRoute: ActivatedRoute,
        private readonly urlManager: UrlManagerService,
        private readonly context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly error: ErrorService,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService,
        private readonly dragAndDrop: DragAndDropService
    ) {
    }

    collection: ListViewModel;
    title = '';
    currentState = CollectionViewState.List;
    selectedDialogId: string;

    private actionButton: IActionHolder = {
        value: 'Actions',
        doClick: () => this.toggleActionMenu(),
        show: () => true,
        disabled: () => this.disableActions(),
        tempDisabled: () => null,
        title: () => this.actionsTooltip,
        accesskey: 'a'
    };

    private reloadButton: IActionHolder = {
        value: 'Reload',
        doClick: () => this.reloadList(),
        show: () => true,
        disabled: () => null,
        tempDisabled: () => null,
        title: () => '',
        accesskey: null
    };

    private firstButton: IActionHolder = {
        value: 'First',
        doClick: () => this.pageFirst(),
        show: () => true,
        disabled: () => this.pageFirstDisabled(),
        tempDisabled: () => null,
        title: () => '',
        accesskey: null
    };

    private previousButton: IActionHolder = {
        value: 'Previous',
        doClick: () => this.pagePrevious(),
        show: () => true,
        disabled: () => this.pagePreviousDisabled(),
        tempDisabled: () => null,
        title: () => '',
        accesskey: null
    };

    private nextButton: IActionHolder = {
        value: 'Next',
        doClick: () => this.pageNext(),
        show: () => true,
        disabled: () => this.pageNextDisabled(),
        tempDisabled: () => null,
        title: () => '',
        accesskey: null
    };

    private lastButton: IActionHolder = {
        value: 'Last',
        doClick: () => this.pageLast(),
        show: () => true,
        disabled: () => this.pageLastDisabled(),
        tempDisabled: () => null,
        title: () => '',
        accesskey: null
    };

    private cachedRouteData: PaneRouteData;
    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;

    toggleActionMenu = () => this.collection.toggleActionMenu();
    reloadList = () => this.collection.reload();
    pageFirst = () => this.collection.pageFirst();
    pagePrevious = () => this.collection.pagePrevious();
    pageNext = () => this.collection.pageNext();
    pageLast = () => this.collection.pageLast();

    disableActions = () => this.collection.noActions() ? true : null;
    hideAllCheckbox = () => this.collection.noActions() || this.collection.items.length === 0;

    pageFirstDisabled = () => this.collection.pageFirstDisabled() ? true : null;
    pagePreviousDisabled = () => this.collection.pagePreviousDisabled() ? true : null;
    pageNextDisabled = () => this.collection.pageNextDisabled() ? true : null;
    pageLastDisabled = () => this.collection.pageLastDisabled() ? true : null;

    showActions = () => this.collection.showActions();

    doTable = () => this.collection.doTable();
    doList = () => this.collection.doList();
    doSummary = () => this.collection.doSummary();

    hasTableData = () => this.collection.hasTableData();

    get actionsTooltip() {
        return this.collection.actionsTooltip();
    }

    get message() {
        return this.collection.getMessage();
    }

    get description() {
        return this.collection.description();
    }

    get size() {
        return this.collection.size;
    }

    get items(): ItemViewModel[] {
        return this.collection.items;
    }

    get header() {
        return this.collection.header;
    }

    get actionHolders() {
        return [this.actionButton, this.reloadButton, this.firstButton, this.previousButton, this.nextButton, this.lastButton];
    }

    get state() {
        return CollectionViewState[this.currentState].toString().toLowerCase();
    }

    getActionExtensions(routeData: PaneRouteData): Promise<Ro.Extensions> {
        return routeData.objectId
            ? this.context.getActionExtensionsFromObject(routeData.paneId, Ro.ObjectIdWrapper.fromObjectId(routeData.objectId, this.configService.config.keySeparator), routeData.actionId)
            : this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    }

    protected setup(routeData: PaneRouteData) {
        this.cachedRouteData = routeData;
        const cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

        this.getActionExtensions(routeData)
            .then((ext: Ro.Extensions) =>
                this.title = ext.friendlyName())
            .catch((reject: ErrorWrapper) => this.error.handleError(reject));

        const listKey = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

        if (this.collection && this.collection.id === listKey) {
            // same collection/page
            this.currentState = routeData.state;
            this.collection.refresh(routeData);
        } else if (this.collection && cachedList) {
            // same collection different page
            this.currentState = routeData.state;
            this.collection.reset(cachedList, routeData);
        } else if (cachedList) {
            // new collection
            this.collection = this.viewModelFactory.listViewModel(cachedList, routeData);
            this.currentState = routeData.state;
            this.collection.refresh(routeData);
        } else {
            // should never get here
            this.loggerService.throw('ListComponent:setup Missing cachedList');
        }

        if (this.collection) {
            // if any previous messages clear them
            this.collection.resetMessage();
        }

        this.selectedDialogId = routeData.dialogId;
    }

    setDropZones(ids: string[]) {
        setTimeout(() => this.dropZones = ids);
    }

    // now this is a child investigate reworking so object is passed in from parent
    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: ICustomActivatedRouteData) => {

            const paneId = data.pane;

            if (!this.paneRouteDataSub) {
                this.paneRouteDataSub =
                    this.urlManager.getPaneRouteDataObservable(paneId)
                        .subscribe((paneRouteData: PaneRouteData) => {
                            if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                                this.lastPaneRouteData = paneRouteData;
                                this.setup(paneRouteData);
                            }
                        });
            }
        });

        this.ddSub = this.dragAndDrop.dropZoneIds$.subscribe(ids => this.setDropZones(ids || []));
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.paneRouteDataSub);
        safeUnsubscribe(this.activatedRouteDataSub);
        safeUnsubscribe(this.ddSub);
    }
}
