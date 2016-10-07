import { Component, OnInit } from '@angular/core';
import { Context } from "./context.service";
import { ViewModelFactory } from "./view-model-factory.service";
import * as ViewModels from "./nakedobjects.viewmodels";
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';

@Component({
    selector: 'recent',
    templateUrl: 'app/recent.component.html'
})
export class RecentComponent implements OnInit {

    constructor(private activatedRoute: ActivatedRoute, private viewModelFactory: ViewModelFactory) {

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
