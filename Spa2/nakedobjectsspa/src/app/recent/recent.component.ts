import { Component, ViewChildren, ElementRef, QueryList, AfterViewInit } from '@angular/core';
import { ContextService } from '../context.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '../url-manager.service';
import { RouteData, PaneRouteData } from '../route-data';
import { RecentItemsViewModel } from '../view-models/recent-items-view-model';
import { RecentItemViewModel } from '../view-models/recent-item-view-model';
import { PaneComponent } from '../pane/pane';
import { IActionHolder } from '../action/action.component';
import * as Msg from '../user-messages';
import * as Rowcomponent from '../row/row.component';

@Component({
    selector: 'nof-recent',
    template: require('./recent.component.html'),
    styles: [require('./recent.component.css')]
})
export class RecentComponent extends PaneComponent implements AfterViewInit {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private readonly viewModelFactory: ViewModelFactoryService,
    ) {
        super(activatedRoute, urlManager);
    }

    // template API 

    title = Msg.recentTitle;
    items = () => this.recent.items;

    recent: RecentItemsViewModel;

    private clearButton: IActionHolder = {
        value: Msg.clear,
        doClick: () => this.clear(),
        show: () => true,
        disabled: () => this.clearDisabled(),
        tempDisabled:  () => null,
        title: () => this.clearDisabled() ? Msg.recentDisabledMessage : Msg.recentMessage,
        accesskey: "c"
    };

    get actionHolders() {
        return [this.clearButton];
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
    actionChildren: QueryList<Rowcomponent.RowComponent>;

    focusOnFirstRow(rows: QueryList<Rowcomponent.RowComponent>) {
        if (rows && rows.first) {
            // until first element returns true
            rows.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstRow(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<Rowcomponent.RowComponent>) => this.focusOnFirstRow(ql));
    }
}