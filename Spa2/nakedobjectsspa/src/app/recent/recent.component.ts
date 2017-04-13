import { ContextService } from '../context.service';
import { Component, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '../url-manager.service';
import { PaneRouteData } from '../route-data';
import { RecentItemsViewModel } from '../view-models/recent-items-view-model';
import { PaneComponent } from '../pane/pane';
import { IActionHolder } from '../action/action.component';
import * as Msg from '../user-messages';
import { RowComponent } from '../row/row.component';
import { RecentItemViewModel } from '../view-models/recent-item-view-model';

@Component({
    selector: 'nof-recent',
    template: require('./recent.component.html'),
    styles: [require('./recent.component.css')]
})
export class RecentComponent extends PaneComponent implements AfterViewInit {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        context : ContextService, 
        private readonly viewModelFactory: ViewModelFactoryService,
    ) {
        super(activatedRoute, urlManager, context);
    }

    // template API 

    title = Msg.recentTitle;
    items = (): RecentItemViewModel[] => this.recent.items;

    recent: RecentItemsViewModel;

    private clearButton: IActionHolder = {
        value: Msg.clear,
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        tempDisabled: () => null,
        title: () => this.clearDisabled() ? Msg.recentDisabledMessage : Msg.recentMessage,
        accesskey: "c"
    };

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


    @ViewChildren("row")
    actionChildren: QueryList<RowComponent>;

    focusOnFirstRow(rows: QueryList<RowComponent>) {
        if (rows && rows.first) {
            // until first element returns true
            rows.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstRow(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<RowComponent>) => this.focusOnFirstRow(ql));
    }
}