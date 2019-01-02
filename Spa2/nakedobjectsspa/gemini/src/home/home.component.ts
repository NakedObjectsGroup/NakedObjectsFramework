import { Component, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from '@nakedobjects/services';
import { ContextService } from '@nakedobjects/services';
import { ErrorService } from '@nakedobjects/services';
import { ViewModelFactoryService } from '@nakedobjects/view-models';
import { PaneRouteData } from '@nakedobjects/services';
import { MenusViewModel } from '@nakedobjects/view-models';
import { MenuViewModel } from '@nakedobjects/view-models';
import { PaneComponent } from '../pane/pane';
import * as Models from '@nakedobjects/restful-objects';
import { LinkViewModel } from '@nakedobjects/view-models';
import { ErrorWrapper } from '@nakedobjects/services';

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
            .catch((reject: ErrorWrapper) => {
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
                .catch((reject: ErrorWrapper) => {
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
