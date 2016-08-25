import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { FooterComponent } from "./footer.component";
import { RepresentationsService } from "./representations.service";
import { UrlManager } from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";
import { Context} from "./context.service";
import { RepLoader} from "./reploader.service";
import { ActivatedRoute, Router} from '@angular/router';
import { Color } from "./color.service";
import { Error } from "./error.service";
import { PaneRouteData, RouteData } from "./nakedobjects.routedata";
import { ViewModelFactory } from "./view-model-factory.service";
import { FocusManager, FocusTarget } from "./focus-manager.service";
import { ActionsComponent } from "./actions.component";
import { GeminiClickDirective } from "./gemini-click.directive";
import { ISubscription } from 'rxjs/Subscription';
import * as Models from "./models";
import * as Constants from "./nakedobjects.constants";
import * as Config from "./nakedobjects.config";
import * as ViewModels from "./nakedobjects.viewmodels";
import { CollectionViewState } from './nakedobjects.routedata';

@Component({
    selector: 'list',
    templateUrl: `app/list.component.html`,
    directives: [ROUTER_DIRECTIVES, FooterComponent, ActionsComponent, GeminiClickDirective]
})

export class ListComponent implements OnInit, OnDestroy {

    constructor(private urlManager: UrlManager,
        private context: Context,
        private color: Color,
        private viewModelFactory: ViewModelFactory,
        private focusManager: FocusManager,
        private error: Error,
        private activatedRoute: ActivatedRoute) {
    }

    collection: ViewModels.ListViewModel;
    title = "";
    class: string;
    paneId: number;
    state = "list";

    getActionExtensions(routeData: PaneRouteData) {
        return routeData.objectId ?
            this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
            this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    }


    setupList(routeData : PaneRouteData) {
        const cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

     
        this.getActionExtensions(routeData).
                then(ext =>
                    this.title = ext.friendlyName()).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        if (cachedList) {

            //if (routeData.state === cachedList.state)

            const listViewModel = new ViewModels.ListViewModel(
                this.color,
                this.context,
                this.viewModelFactory,
                this.urlManager,
                this.focusManager,
                this.error
            );
            //$scope.listTemplate = template.getTemplateName(cachedList.extensions().elementType(), TemplateType.List, routeData.state);
            listViewModel.reset(cachedList, routeData);
            //$scope.collection = listViewModel;
            this.collection = listViewModel;
            this.state = CollectionViewState[routeData.state].toString().toLowerCase();

            listViewModel.refresh(routeData);

            //handleListActionsAndDialog($scope, routeData);
        } else {
            //$scope.listTemplate = Nakedobjectsconstants.listPlaceholderTemplate;
            //$scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
            
            this.focusManager.focusOn(FocusTarget.Action, 0, routeData.paneId);
        }
    }

  
    onChild() {
        this.class = "split";
    }

    onChildless() {
        this.class = "single";
    }

   

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {

        this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.class = data["class"];

       

            this.urlManager.getRouteDataObservable()
                .subscribe((rd: RouteData) => {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.setupList(paneRouteData);
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