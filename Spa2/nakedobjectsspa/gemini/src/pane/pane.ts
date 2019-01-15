import { OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ContextService, ICustomActivatedRouteData, Pane, PaneName, PaneRouteData, PaneType, UrlManagerService } from '@nakedobjects/services';
import { SubscriptionLike as ISubscription } from 'rxjs';
import { safeUnsubscribe } from '../helpers-components';

export abstract class PaneComponent implements OnInit, OnDestroy {

    protected constructor(
        protected readonly activatedRoute: ActivatedRoute,
        protected readonly urlManager: UrlManagerService,
        protected readonly context: ContextService
    ) {
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;
    private lastPaneRouteData: PaneRouteData;

    // pane API
    paneId: Pane;
    paneType: PaneType;
    paneIdName: PaneName;
    arData: ICustomActivatedRouteData;

    onChild() {
        setTimeout(() => this.paneType = 'split');
    }

    onChildless() {
        setTimeout(() => this.paneType = 'single');
    }

    protected abstract setup(routeData: PaneRouteData): void;
    protected doSetup(routeData: PaneRouteData) {
        return !routeData.isEqual(this.lastPaneRouteData);
    }

    ngOnInit(): void {
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe((data: ICustomActivatedRouteData) => {
            this.arData = data;
            this.paneId = data.pane;
            this.paneType = data.paneType;
            this.paneIdName = this.paneId === 1 ? 'pane1' : 'pane2';

            if (!this.paneRouteDataSub) {
                this.paneRouteDataSub =
                    this.urlManager.getPaneRouteDataObservable(this.paneId)
                        .subscribe((paneRouteData: PaneRouteData) => {
                            if (!paneRouteData.isEqualIgnoringReload(this.lastPaneRouteData)) {
                                // only remove messages if something more than reload flag has changed
                                this.context.clearMessages();
                                this.context.clearWarnings();
                            }

                            if (this.doSetup(paneRouteData)) {
                                this.lastPaneRouteData = paneRouteData;
                                this.setup(paneRouteData);
                            }
                        });
            }
        });
    }

    ngOnDestroy(): void {
        safeUnsubscribe(this.activatedRouteDataSub);
        safeUnsubscribe(this.paneRouteDataSub);
    }
}
