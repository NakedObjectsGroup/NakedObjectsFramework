import { AfterViewInit, Component, OnDestroy, QueryList, ViewChildren } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContextService, PaneRouteData, UrlManagerService } from '@nakedobjects/services';
import { RecentItemsViewModel, RecentItemViewModel, ViewModelFactoryService } from '@nakedobjects/view-models';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { IActionHolder } from '../action/action.component';
import { safeUnsubscribe } from '../helpers-components';
import { PaneComponent } from '../pane/pane';
import { RowComponent } from '../row/row.component';
import * as Msg from '../user-messages';

@Component({
    selector: 'nof-recent',
    templateUrl: 'recent.component.html',
    styleUrls: ['recent.component.css']
})
export class RecentComponent extends PaneComponent implements AfterViewInit, OnDestroy {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        context: ContextService,
        private readonly viewModelFactory: ViewModelFactoryService,
    ) {
        super(activatedRoute, urlManager, context);
    }

    recent: RecentItemsViewModel;

    private clearButton: IActionHolder = {
        value: Msg.clear,
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        tempDisabled: () => null,
        title: () => this.clearDisabled() ? Msg.recentDisabledMessage : Msg.recentMessage,
        accesskey: 'c'
    };

    @ViewChildren('row')
    actionChildren: QueryList<RowComponent>;

    private sub: ISubscription;

    // template API

    title = Msg.recentTitle;
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

    ngOnDestroy() {
        safeUnsubscribe(this.sub);
        super.ngOnDestroy();
    }
}
