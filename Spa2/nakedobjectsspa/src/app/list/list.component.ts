import { Component, Input, OnInit, OnDestroy, ViewChildren, QueryList, AfterViewInit, ElementRef } from '@angular/core';
import { RepresentationsService } from "../representations.service";
import { UrlManagerService } from "../url-manager.service";
import { ClickHandlerService } from "../click-handler.service";
import { ContextService } from "../context.service";
import { RepLoaderService } from "../rep-loader.service";
import { ActivatedRoute, Router } from '@angular/router';
import { ColorService } from "../color.service";
import { ErrorService } from "../error.service";
import { PaneRouteData, RouteData, CollectionViewState } from "../route-data";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ISubscription } from 'rxjs/Subscription';
import * as Models from "../models";
import * as Constants from "../constants";
import * as Config from "../config";
import * as ViewModels from "../view-models";


@Component({
    selector: 'list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit, OnDestroy, AfterViewInit {

    constructor(private urlManager: UrlManagerService,
        private context: ContextService,
        private color: ColorService,
        private viewModelFactory: ViewModelFactoryService,
        private error: ErrorService,
        private activatedRoute: ActivatedRoute) {
    }

    collection: ViewModels.ListViewModel;
    title = "";
    paneType: string;
    paneId: number;
    state = "list";

    paneIdName = () => this.paneId === 1 ? "pane1" : "pane2";

    getActionExtensions(routeData: PaneRouteData): Promise<Models.Extensions> {
        return routeData.objectId
            ? this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId)
            : this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    }

    private cachedRouteData: PaneRouteData;

    setupList(routeData: PaneRouteData) {
        this.cachedRouteData = routeData;
        const cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

        this.getActionExtensions(routeData)
            .then((ext: Models.Extensions) =>
                this.title = ext.friendlyName())
            .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        const listKey = this.urlManager.getListCacheIndex(routeData.paneId, routeData.page, routeData.pageSize);

        if (this.collection && this.collection.id === listKey) {
            this.state = CollectionViewState[routeData.state].toString().toLowerCase();
            this.collection.refresh(routeData);
        } else if (cachedList) {

            //if (routeData.state === cachedList.state)

            const listViewModel = new ViewModels.ListViewModel(
                this.color,
                this.context,
                this.viewModelFactory,
                this.urlManager,
                this.error
            );
            //$scope.listTemplate = template.getTemplateName(cachedList.extensions().elementType(), TemplateType.List, routeData.state);
            listViewModel.reset(cachedList, routeData);
            //$scope.collection = listViewModel;
            this.collection = listViewModel;
            this.state = CollectionViewState[routeData.state].toString().toLowerCase();

            listViewModel.refresh(routeData);

            //handleListActionsAndDialog($scope, routeData);
        }
    }


    onChild() {
        this.paneType = "split";
    }

    onChildless() {
        this.paneType = "single";
    }

    reload() {

        const recreate = () =>
            this.cachedRouteData.objectId
            ? this.context.getListFromObject(this.cachedRouteData.paneId, this.cachedRouteData, this.cachedRouteData.page, this.cachedRouteData.pageSize)
            : this.context.getListFromMenu(this.cachedRouteData.paneId, this.cachedRouteData, this.cachedRouteData.page, this.cachedRouteData.pageSize);

        recreate()
            .then(() => this.setupList(this.cachedRouteData))
            .catch((reject: Models.ErrorWrapper) => {
                this.error.handleError(reject);
            });
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {

        this.activatedRoute.data.subscribe((data: any) => {
            this.paneId = data["pane"];
            this.paneType = data["class"];
        });

        this.urlManager.getRouteDataObservable()
            .subscribe((rd: RouteData) => {
                if (this.paneId) {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.setupList(paneRouteData);
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

    // todo DRY this - and rename - copy not cut
    cut(event: any, item: ViewModels.IDraggableViewModel) {
        const cKeyCode = 67;
        if (event && (event.keyCode === cKeyCode && event.ctrlKey)) {
            this.context.setCutViewModel(item);
            event.preventDefault();
        }
    }

    @ViewChildren("rw")
    row: QueryList<ElementRef>;

    focusOnRow(e: QueryList<ElementRef>) {
        if (e && e.first) {
            e.first.nativeElement.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnRow(this.row);
        this.row.changes.subscribe((ql: QueryList<ElementRef>) => this.focusOnRow(ql));
    }
}