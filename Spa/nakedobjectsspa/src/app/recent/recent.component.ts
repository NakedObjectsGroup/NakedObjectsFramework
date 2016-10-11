import { Component, OnInit } from '@angular/core';
import { ContextService } from "../context.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import * as ViewModels from "../view-models";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';

@Component({
    selector: 'recent',
    templateUrl: './recent.component.html'
})
export class RecentComponent implements OnInit {

    constructor(private activatedRoute: ActivatedRoute, private viewModelFactory: ViewModelFactoryService) {

    }

    paneId: number;
    vm: ViewModels.RecentItemsViewModel;

    private activatedRouteDataSub: ISubscription;

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.vm = this.viewModelFactory.recentItemsViewModel(this.paneId);
        });
    }

    ngOnDestroy(): void {
        if (this.activatedRouteDataSub) {
            this.activatedRouteDataSub.unsubscribe();
        }
    }
}
