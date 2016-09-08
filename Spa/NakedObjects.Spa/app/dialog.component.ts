import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ActionComponent } from "./action.component";
import { ViewModelFactory } from "./view-model-factory.service";
import { UrlManager } from "./urlmanager.service";
import * as _ from "lodash";
import * as Models from "./models";
import * as ViewModels from "./nakedobjects.viewmodels";
import { GeminiDropDirective } from "./gemini-drop.directive";
import { GeminiBooleanDirective } from './gemini-boolean.directive';
import {GeminiConditionalChoicesDirective} from './gemini-conditional-choices.directive';
import { ActivatedRoute, Router, Data } from '@angular/router';

import "./rxjs-extensions";
import { Subject } from 'rxjs/Subject';
import {AutocompleteComponent} from './autocomplete.component';
import * as Nakedobjectsroutedata from './nakedobjects.routedata';

import { ISubscription } from 'rxjs/Subscription';
import * as Contextservice from './context.service';
import * as Colorservice from './color.service';
import * as Focusmanagerservice from './focus-manager.service';
import { Error } from './error.service';

@Component({
    selector: 'dialog',
    templateUrl: 'app/dialog.component.html',
    directives: [GeminiDropDirective, GeminiBooleanDirective, GeminiConditionalChoicesDirective, AutocompleteComponent],
    styleUrls: ['app/dialog.component.css']
})
export class DialogComponent implements OnInit, OnDestroy {

    constructor(private viewModelFactory: ViewModelFactory,
        private urlManager: UrlManager,
        private activatedRoute: ActivatedRoute,
        private error: Error,
        private color: Colorservice.Color,
        private focusManager: Focusmanagerservice.FocusManager,
        private context: Contextservice.Context) { }

  
    paneId : number;

    dialog: ViewModels.DialogViewModel;

    parameterChanged() {
        this.parameterChangedSource.next(true);
        this.parameterChangedSource.next(false);
    }

    private parameterChangedSource = new Subject<boolean>();

    parameterChanged$ = this.parameterChangedSource.asObservable();

    getDialog(routeData: Nakedobjectsroutedata.PaneRouteData, selectedMenu : ViewModels.MenuViewModel) {
        const dialogId = routeData.dialogId;

        if (dialogId && selectedMenu) {
            const action = selectedMenu.menuRep.actionMember(dialogId);
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

                    this.dialog = dialogViewModel;
                })
                .catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        } else {
            this.dialog = null;
        }
    }

    getMenu(paneRouteData: Nakedobjectsroutedata.PaneRouteData) {
        const menuId = paneRouteData.menuId;
        if (menuId) {
            this.context.getMenu(menuId)
                .then((menu: Models.MenuRepresentation) => {
                    const rd = this.urlManager.getRouteData().pane()[this.paneId];
                    const selectedMenu = this.viewModelFactory.menuViewModel(menu, rd);
                    this.getDialog(paneRouteData, selectedMenu);
                })
                .catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        } else {
            // clear

        }
    }


    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {


        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
       
            this.paneId = data["pane"];

            this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
                .subscribe((rd: Nakedobjectsroutedata.RouteData) => {
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