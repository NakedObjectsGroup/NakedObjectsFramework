import { Component, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '../url-manager.service';
import { ContextService } from '../context.service';
import { ErrorService } from '../error.service';
import { ViewModelFactoryService } from '../view-model-factory.service';
import { PaneRouteData } from '../route-data';
import { MenusViewModel } from '../view-models/menus-view-model';
import { MenuViewModel } from '../view-models/menu-view-model';
import { PaneComponent } from '../pane/pane';
import * as Models from '../models';
import { LinkViewModel } from '../view-models/link-view-model';

@Component({
    selector: 'nof-home',
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent extends PaneComponent {

    constructor(urlManager: UrlManagerService,
        activatedRoute: ActivatedRoute,
        private readonly viewModelFactory: ViewModelFactoryService,
        context: ContextService,
        private readonly errorService: ErrorService,
        private readonly myElement: ElementRef) {
        super(activatedRoute, urlManager, context);
    }

    // template API
    get hasMenus() {
        return !!this.menus;
    }

    get menuItems(): LinkViewModel[] {
        return this.menus.items;
    }

    selectedMenu: MenuViewModel | null;
    selectedDialogId: string | null;

    hasAuthorisedMenus = true;

    private menus: MenusViewModel;

    getMenus(paneRouteData: PaneRouteData) {
        this.context.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                this.menus = this.viewModelFactory.menusViewModel(menus, paneRouteData);
                this.hasAuthorisedMenus = this.menus && this.menus.items && this.menus.items.length > 0;
            })
            .catch((reject: Models.ErrorWrapper) => {
                this.errorService.handleError(reject);
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
                    this.errorService.handleError(reject);
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
