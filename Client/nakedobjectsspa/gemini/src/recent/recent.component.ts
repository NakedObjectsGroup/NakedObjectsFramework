import { AfterViewInit, Component, OnDestroy, QueryList, ViewChildren, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContextService, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import { RecentItemsViewModel, RecentItemViewModel, ViewModelFactoryService, DragAndDropService } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { IActionHolder } from '../action/action.component';
import { safeUnsubscribe } from '../helpers-components';
import { PaneComponent } from '../pane/pane';
import { RowComponent } from '../row/row.component';

@Component({
    selector: 'nof-recent',
    templateUrl: 'recent.component.html',
    styleUrls: ['recent.component.css']
})
export class RecentComponent extends PaneComponent implements AfterViewInit, OnInit, OnDestroy {

    private ddSub: ISubscription;
    dropZones: string[] = [];

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly dragAndDrop: DragAndDropService
    ) {
        super(activatedRoute, urlManager, context);
    }

    recent: RecentItemsViewModel;

    private clearButton: IActionHolder = {
        value: '',
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        tempDisabled: () => null,
        title: () => this.recent.getRecentMessage(this.clearDisabled()),
        accesskey: 'c',
        presentationHint: ''
    };

    @ViewChildren('row')
    actionChildren: QueryList<RowComponent>;

    private sub: ISubscription;

    // template API

    title = '';
    items = (): RecentItemViewModel[] => this.recent.items;

    get actionHolders() {
        return [this.clearButton];
    }

    hasItems() {
        return this.recent && this.recent.items.length > 0;
    }

    clear() {
        this.recent.clear();
    }

    clearDisabled() {
        return this.recent.items.length === 0 ? true : null;
    }

    protected setup(routeData: PaneRouteData) {
        this.recent = this.viewModelFactory.recentItemsViewModel(this.paneId);
        this.clearButton.value = this.recent.clearMessage;
        this.title = this.recent.title;
    }

    focusOnFirstRow(rows: QueryList<RowComponent>) {
        if (rows && rows.first) {
            // until first element returns true
            rows.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstRow(this.actionChildren);
        this.sub = this.actionChildren.changes.subscribe((ql: QueryList<RowComponent>) => this.focusOnFirstRow(ql));
    }

    setDropZones(ids: string[]) {
        setTimeout(() => this.dropZones = ids);
    }

    ngOnInit() {
        this.ddSub = this.dragAndDrop.dropZoneIds$.subscribe(ids => this.setDropZones(ids || []));
        super.ngOnInit();
    }

    ngOnDestroy() {
        safeUnsubscribe(this.sub);
        safeUnsubscribe(this.ddSub);
        super.ngOnDestroy();
    }
}
