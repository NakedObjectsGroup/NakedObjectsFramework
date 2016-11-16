import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';
import { UrlManagerService } from "../url-manager.service";
import { RouteData, PaneRouteData } from "../route-data";
import { RecentItemsViewModel} from '../view-models/recent-items-view-model';
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

    vm: RecentItemsViewModel;

    protected setup(routeData: PaneRouteData) {
        this.vm = this.viewModelFactory.recentItemsViewModel(this.paneId);
    }
}