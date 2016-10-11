import { Component, OnInit, Input, OnDestroy, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { getAppPath } from "../config";
import { Observable } from 'rxjs/Observable';
import { ISubscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router, Data } from '@angular/router';
import { UrlManagerService } from "../url-manager.service";
import { ContextService } from "../context.service";
import { ErrorService } from '../error.service';
import { FocusManagerService } from "../focus-manager.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ColorService } from "../color.service";
import { RouteData, PaneRouteData } from "../route-data";
import * as Models from "../models";
import * as ViewModels from "../view-models";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {

    constructor(private viewModelFactory: ViewModelFactoryService,
        private context: ContextService,
        private error: ErrorService,
        private urlManager: UrlManagerService,
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private color: ColorService,
        private focusManager: FocusManagerService) {
    }

    paneId: number;

    menus: ViewModels.MenusViewModel;
    selectedMenu: ViewModels.MenuViewModel;

    // todo rename to single or split
    class: string;

    onChild() {
        this.class = "split object-color0";
    }

    onChildless() {
        this.class = "single object-color0";
    }

    getMenus() {
        this.context.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                this.menus = new ViewModels.MenusViewModel(this.viewModelFactory);
                const rd = this.urlManager.getRouteData().pane()[this.paneId];
                this.menus.reset(menus, rd);
            })
            .catch((reject: Models.ErrorWrapper) => {
                this.error.handleError(reject);
            });
    }

    getMenu(paneRouteData: PaneRouteData) {
        const menuId = paneRouteData.menuId;
        if (menuId) {
            this.context.getMenu(menuId)
                .then((menu: Models.MenuRepresentation) => {
                    const rd = this.urlManager.getRouteData().pane()[this.paneId];
                    this.selectedMenu = this.viewModelFactory.menuViewModel(menu, rd);
                })
                .catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        }
        else {
            this.selectedMenu = null;
        }
    }

    doClick(linkViewModel: ViewModels.ILinkViewModel) {
        const menuId = linkViewModel.link.rel().parms[0].value;
        this.urlManager.setMenu(menuId, this.paneId);
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {

        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.class = data["class"] + " object-color0";

            this.getMenus();
            this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
                .subscribe((rd: RouteData) => {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.getMenu(paneRouteData);
                });
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
