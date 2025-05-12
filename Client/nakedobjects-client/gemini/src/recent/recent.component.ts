import { AfterViewInit, Component, OnDestroy, QueryList, ViewChildren, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContextService, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import { RecentItemsViewModel, RecentItemViewModel, ViewModelFactoryService, DragAndDropService, SortType } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { IActionHolder } from '../action/action.component';
import { safeUnsubscribe } from '../helpers-components';
import { PaneComponent } from '../pane/pane';
import { RowComponent } from '../row/row.component';

@Component({
    selector: 'nof-recent',
    templateUrl: 'recent.component.html',
    styleUrls: ['recent.component.css'],
    standalone: false
})
export class RecentComponent extends PaneComponent implements AfterViewInit, OnInit, OnDestroy {
   

   
    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly dragAndDrop: DragAndDropService
    ) {
        super(activatedRoute, urlManager, context);
    }

    private clearButton: IActionHolder = {
        value: 'Clear All',
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        tempDisabled: () => null,
        title: () => this.getMessage(this.clearDisabled(), 'clearAll'),
        accesskey: 'c',
        presentationHint: '',
        showDialog: () => false
    };

    private clearSelectedButton: IActionHolder = {
        value: 'Clear Selected',
        doClick: () => this.clearSelected(),
        show: () => true,
        disabled: () => this.clearSelectedDisabled(),
        tempDisabled: () => null,
        title: () => this.getMessage(this.clearSelectedDisabled(), 'clearSelected'),
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };

    private sortByUsageButton: IActionHolder = {
        value: 'Sort by Usage',
        doClick: () => this.sortByUsage(),
        show: () => true,
        disabled: () => this.sortByUsageDisabled(),
        tempDisabled: () => null,
        title: () => this.getMessage(this.sortByUsageDisabled(), 'sortUsage'),
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };

    private sortByTypeButton: IActionHolder = {
        value: 'Sort by Type',
        doClick: () => this.sortByType(),
        show: () => true,
        disabled: () => this.sortByTypeDisabled(),
        tempDisabled: () => null,
        title: () => this.getMessage(this.sortByTypeDisabled(), 'sortType'),
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };

    private sortByTitleButton: IActionHolder = {
        value: 'Sort by Title',
        doClick: () => this.sortByTitle(),
        show: () => true,
        disabled: () => this.sortByTitleDisabled(),
        tempDisabled: () => null,
        title: () => this.getMessage(this.sortByTitleDisabled(), 'sortTitle'),
        accesskey: null,
        presentationHint: '',
        showDialog: () => false
    };


    @ViewChildren('row')
    actionChildren?: QueryList<RowComponent>;

    private sub?: ISubscription;
    private ddSub?: ISubscription;
    dropZones: string[] = [];
    recent?: RecentItemsViewModel;
    paneRouteData?: PaneRouteData;

    // template API

    title = '';
    items = (): RecentItemViewModel[] => this.recent?.items || [];

    get actionHolders() {
        return [this.sortByUsageButton, this.sortByTypeButton, this.sortByTitleButton, this.clearSelectedButton, this.clearButton];
    }

    hasItems() {
        return this.recent && this.recent.items.length > 0;
    }

    private clear() {
        this.recent?.clear();
    }

    private clearDisabled() {
        return this.recent?.items.length === 0 ? true : null;
    }

    private clearSelected() {
        this.recent?.clearSelected(this.paneRouteData);
    }

    private clearSelectedDisabled() {
        return !this.paneRouteData?.selectedCollectionItems?.['']?.reduce((p, c) => p || c);
    }

    private sortByUsage() {
        this.recent?.sort(SortType.ByUsage);
    }

    private sortByUsageDisabled() {
        return this.recent?.currentSortType === SortType.ByUsage;
    }

    private sortByType() {
        this.recent?.sort(SortType.ByType);
    }

    private sortByTypeDisabled() {
        return this.recent?.currentSortType === SortType.ByType;
    }

    private sortByTitle() {
        this.recent?.sort(SortType.ByTitle);
    }

    private sortByTitleDisabled() {
        return this.recent?.currentSortType === SortType.ByTitle;
    }

    private getMessage(disabled: boolean | null, holder: string) {
        switch (holder) {
            case 'clearAll': return this.recent?.getRecentMessage(!!disabled) || '';
            case 'clearSelected': return disabled ? 'Nothing selected' : 'Clear selected items';
            case 'sortType': return disabled ? 'Currently sorted by Type' : 'Sort by Type';
            case 'sortTitle': return disabled ? 'Currently sorted by Title' : 'Sort by Title';
            case 'sortUsage': return disabled ? 'Currently sorted by when last used' : 'Sort by when last used';
        }

        return '';
    }

    protected override setup(routeData: PaneRouteData) {
        this.paneRouteData = routeData;
        if (!this.recent) {
            this.recent = this.viewModelFactory.recentItemsViewModel(this.paneId!);
            this.title = this.recent.title;
        }
    }

    focusOnFirstRow(rows?: QueryList<RowComponent>) {
        if (rows && rows.first) {
            // until first element returns true
            rows.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstRow(this.actionChildren);
        this.sub = this.actionChildren?.changes.subscribe((ql: QueryList<RowComponent>) => this.focusOnFirstRow(ql));
    }

    setDropZones(ids: string[]) {
        setTimeout(() => this.dropZones = ids);
    }

    override ngOnInit() {
        this.ddSub = this.dragAndDrop.dropZoneIds$.subscribe(ids => this.setDropZones(ids || []));
        super.ngOnInit();
    }

    override ngOnDestroy() {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.ddSub);
        super.ngOnDestroy();
    }
}
