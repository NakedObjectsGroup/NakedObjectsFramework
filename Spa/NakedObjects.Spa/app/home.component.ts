import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { RepresentationsService } from './representations.service'
import { getAppPath } from "./nakedobjects.config";
import { Observable } from 'rxjs/Observable';
import { ISubscription } from 'rxjs/Subscription';
import { ActionsComponent } from "./actions.component";
import { ActivatedRoute, Router, Data } from '@angular/router';
import { UrlManager } from "./urlmanager.service";
import { Context } from "./context.service";
import { Error } from './error.service';
import { FocusManager } from "./focus-manager.service";
import { ViewModelFactory } from "./view-model-factory.service";
import { Color } from "./color.service";
import { DialogComponent } from "./dialog.component";
import { RouteData, PaneRouteData } from "./nakedobjects.routedata";
import { ROUTER_DIRECTIVES } from '@angular/router';
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";


@Component({
    selector: 'home',
    templateUrl: 'app/home.component.html',
    directives: [ActionsComponent, DialogComponent, ROUTER_DIRECTIVES]
})
export class HomeComponent implements OnInit, OnDestroy {

    constructor(private representationsService: RepresentationsService,
        private viewModelFactory: ViewModelFactory,
        private context: Context,
        private error: Error,
        private urlManager: UrlManager,
        private router: Router,
        private activatedRoute : ActivatedRoute,
        private color: Color,
        private focusManager: FocusManager) {
     
    }

    paneId: number;

    menus: ViewModels.MenusViewModel;
    selectedMenu: ViewModels.MenuViewModel;
    selectedDialog: ViewModels.DialogViewModel;

    class: string; 
  
    onChild() {
        this.class = "split";
    }

    onChildless() {
        this.class = "single";
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

    getDialog(routeData: PaneRouteData) {
        const dialogId = routeData.dialogId;

        if (dialogId && this.selectedMenu) {
            const action = this.selectedMenu.menuRep.actionMember(dialogId);
            this.context.getInvokableAction(action)
                .then(details => {
                    //if (actionViewModel) {
                    //    actionViewModel.makeInvokable(details);
                    //}
                    //setDialog($scope, actionViewModel || details, routeData);
                    //this.focusManager.focusOn(Focusmanagerservice.FocusTarget.Dialog, 0, routeData.paneId);

                    this.context.clearParmUpdater(routeData.paneId);

                    //$scope.dialogTemplate = dialogTemplate;
                    const dialogViewModel = new ViewModels.DialogViewModel(this.color,
                        this.context,
                        this.viewModelFactory,
                        this.urlManager,
                        this.focusManager,
                        this.error);
                    //const isAlreadyViewModel = action instanceof Nakedobjectsviewmodels.ActionViewModel;
                    const actionViewModel = //!isAlreadyViewModel ?
                        this.viewModelFactory.actionViewModel(action as Models.ActionMember | Models.ActionRepresentation,
                            dialogViewModel,
                            routeData);
                    //: action as Nakedobjectsviewmodels.IActionViewModel;

                    actionViewModel.makeInvokable(details);
                    dialogViewModel.reset(actionViewModel, routeData);
                    //$scope.dialog = dialogViewModel;

                    this.context.setParmUpdater(dialogViewModel.setParms, routeData.paneId);
                    dialogViewModel.deregister = () => this.context.clearParmUpdater(routeData.paneId);

                    this.selectedDialog = dialogViewModel;
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        } else {
            this.selectedDialog = null;
        }
    }

    getMenu(paneRouteData: PaneRouteData) {
        const menuId = paneRouteData.menuId;
        if (menuId) {
            this.context.getMenu(menuId)
                .then((menu: Models.MenuRepresentation) => {
                    const rd = this.urlManager.getRouteData().pane()[this.paneId];
                    this.selectedMenu = this.viewModelFactory.menuViewModel(menu, rd);
                    this.getDialog(paneRouteData);
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

    private activatedRouteDataSub : ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {

        
        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.class = data["class"];
        
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