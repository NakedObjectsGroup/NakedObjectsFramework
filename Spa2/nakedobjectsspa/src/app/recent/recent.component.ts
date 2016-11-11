import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import * as ViewModels from "../view-models";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';
import { UrlManagerService } from "../url-manager.service";
import { RouteData, PaneRouteData } from "../route-data";

@Component({
    selector: 'recent',
    templateUrl: './recent.component.html',
    styleUrls: ['./recent.component.css']
})

export class RecentComponent implements OnInit {

    constructor(private activatedRoute: ActivatedRoute,
        private viewModelFactory: ViewModelFactoryService,
        private urlManager: UrlManagerService) {

    }

    paneId: number;
    vm: ViewModels.RecentItemsViewModel;

    paneType: string;

    onChild() {
        this.paneType = "split";
    }

    onChildless() {
        this.paneType = "single";
    }

    paneIdName = () => this.paneId === 1 ? "pane1" : "pane2";

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.paneType = data["class"];

            this.vm = this.viewModelFactory.recentItemsViewModel(this.paneId);

        });

        this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
            .subscribe((rd: RouteData) => {
                if (this.paneId) {
                    this.vm = this.viewModelFactory.recentItemsViewModel(this.paneId);
                }
            });
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
        if (this.paneRouteDataSub) {
            this.paneRouteDataSub.unsubscribe();
        }
    }
}
