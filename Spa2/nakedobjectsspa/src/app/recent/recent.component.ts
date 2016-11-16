import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';
import { UrlManagerService } from "../url-manager.service";
import { RouteData, PaneRouteData } from "../route-data";
import { RecentItemsViewModel } from '../view-models/recent-items-view-model';
import { RecentItemViewModel } from '../view-models/recent-item-view-model';
import { PaneComponent } from '../pane/pane';

@Component({
    selector: 'recent',
    templateUrl: './recent.component.html',
    styleUrls: ['./recent.component.css']
})
export class RecentComponent extends PaneComponent {

    constructor(activatedRoute: ActivatedRoute,
                urlManager: UrlManagerService,
                private viewModelFactory: ViewModelFactoryService,
    ) {
        super(activatedRoute, urlManager);
    }

    // template API 

    title = "Recently Viewed Objects";
    items = () => this.recent.items;

    // todo again a smell - new child component ! 
    itemColor = (item: RecentItemViewModel) => item.color;
    itemTitle = (item: RecentItemViewModel) => item.title;
    itemFriendlyName = (item: RecentItemViewModel) => item.friendlyName;

    doItemClick = (item: RecentItemViewModel, right?: boolean) => item.doClick(right);

    recent: RecentItemsViewModel;

    protected setup(routeData: PaneRouteData) {
        this.recent = this.viewModelFactory.recentItemsViewModel(this.paneId);
    }
}