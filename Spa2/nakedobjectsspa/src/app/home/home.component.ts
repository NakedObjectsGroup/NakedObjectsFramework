import { Component, AfterViewInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { getAppPath } from "../config";
import { ActivatedRoute, Data } from '@angular/router';
import { UrlManagerService } from "../url-manager.service";
import { ContextService } from "../context.service";
import { ErrorService } from '../error.service';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ColorService } from "../color.service";
import { RouteData, PaneRouteData } from "../route-data";
import { LinkViewModel } from '../view-models/link-view-model';
import { MenusViewModel } from '../view-models/menus-view-model';
import { MenuViewModel } from '../view-models/menu-view-model';
import { PaneComponent } from '../pane/pane';
import * as Models from "../models";

@Component({
    selector: 'nof-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent extends PaneComponent {

    constructor(urlManager: UrlManagerService,
        activatedRoute: ActivatedRoute,
        private readonly viewModelFactory: ViewModelFactoryService,
        private readonly context: ContextService,
        private readonly error: ErrorService,
        private readonly color: ColorService,
        private readonly myElement: ElementRef) {
        super(activatedRoute, urlManager);
    }

    // template API 
    get hasMenus() {
        return !!this.menus;
    }

    get menuItems() {
        return this.menus.items;
    }

    selectedMenu: MenuViewModel | null;
    selectedDialogId : string | null;

    private menus: MenusViewModel;

    getMenus(paneRouteData: PaneRouteData) {
        this.context.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                this.menus = this.viewModelFactory.menusViewModel(menus, paneRouteData);
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
                    this.selectedMenu = this.viewModelFactory.menuViewModel(menu, paneRouteData);
                    this.selectedDialogId = paneRouteData.dialogId;
                })
                .catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        } else {
            this.selectedMenu = null;
            this.selectedDialogId = null;
        }
    }

    protected setup(paneRouteData: PaneRouteData) {
        this.getMenus(paneRouteData);
        this.getMenu(paneRouteData);
    }

}