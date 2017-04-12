import { OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ISubscription } from 'rxjs/Subscription';
import { PaneRouteData, ICustomActivatedRouteData, PaneType, PaneName } from '../route-data';
import { UrlManagerService } from '../url-manager.service';

export abstract class PaneComponent implements OnInit, OnDestroy {

    protected constructor(
        protected readonly activatedRoute: ActivatedRoute,
        protected readonly urlManager: UrlManagerService
    ) {
    }

    // pane API
    paneId: number;
    paneType: PaneType;
    paneIdName: PaneName;
    arData: ICustomActivatedRouteData;

    onChild() {
        this.paneType = "split";
    }

    onChildless() {
        this.paneType = "single";
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;

    protected abstract setup(routeData: PaneRouteData);

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: ICustomActivatedRouteData) => {
            this.arData = data;
            this.paneId = data.pane;
            this.paneType = data.paneType;
            this.paneIdName = this.paneId === 1 ? "pane1" : "pane2";

            if (!this.paneRouteDataSub) {
                this.paneRouteDataSub =
                    this.urlManager.getPaneRouteDataObservable(this.paneId)
                        .subscribe((paneRouteData: PaneRouteData) => {
                            if (!paneRouteData.isEqual(this.lastPaneRouteData)) {
                                this.lastPaneRouteData = paneRouteData;
                                this.setup(paneRouteData);
                            }
                        });
            };
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
