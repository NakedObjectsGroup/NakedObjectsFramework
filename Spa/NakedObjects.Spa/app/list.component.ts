import { Component, Input } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { FooterComponent } from "./footer.component";
import { RepresentationsService } from "./representations.service";
import { UrlManager } from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";
import {Context} from "./context.service";
import {RepLoader} from "./reploader.service";
import { ActivatedRoute, Router, UrlPathWithParams} from '@angular/router';
import * as Models from "./models";
import * as Nakedobjectsconstants from "./nakedobjects.constants";
import * as Nakedobjectsconfig from "./nakedobjects.config";
import * as Colorservice from "./color.service";
import * as Errorservice from "./error.service";
import * as Nakedobjectsroutedata from "./nakedobjects.routedata";
import * as Nakedobjectsviewmodels from "./nakedobjects.viewmodels";
import * as Viewmodelfactoryservice from "./view-model-factory.service";
import * as Focusmanagerservice from "./focus-manager.service";
import * as Actionscomponent from "./actions.component";
import * as Geminiclickdirective from "./gemini-click.directive";
import {NgClass} from '@angular/common';

@Component({
    selector: 'list',
    templateUrl: `app/list.component.html`,
    directives: [ROUTER_DIRECTIVES, FooterComponent, Actionscomponent.ActionsComponent, Geminiclickdirective.GeminiClickDirective, NgClass]
})

export class ListComponent {

    constructor(private urlManager: UrlManager,
        private context: Context,
        private color: Colorservice.Color,
        private viewModelFactory: Viewmodelfactoryservice.ViewModelFactory,
        private focusManager: Focusmanagerservice.FocusManager,
        private error: Errorservice.Error,
        private activatedRoute: ActivatedRoute) {

    }

    collection: Nakedobjectsviewmodels.ListViewModel;

    title = "";

    getActionExtensions(routeData: Nakedobjectsroutedata.PaneRouteData) {
        return routeData.objectId ?
            this.context.getActionExtensionsFromObject(routeData.paneId, Models.ObjectIdWrapper.fromObjectId(routeData.objectId), routeData.actionId) :
            this.context.getActionExtensionsFromMenu(routeData.menuId, routeData.actionId);
    }


    setupList(routeData : Nakedobjectsroutedata.PaneRouteData) {
        const cachedList = this.context.getCachedList(routeData.paneId, routeData.page, routeData.pageSize);

     
        this.getActionExtensions(routeData).
                then(ext =>
                    this.title = ext.friendlyName()).
                catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        if (cachedList) {
            const listViewModel = new Nakedobjectsviewmodels.ListViewModel(
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
         
            //handleListActionsAndDialog($scope, routeData);
        } else {
            //$scope.listTemplate = Nakedobjectsconstants.listPlaceholderTemplate;
            //$scope.collectionPlaceholder = viewModelFactory.listPlaceholderViewModel(routeData);
            
            this.focusManager.focusOn(Focusmanagerservice.FocusTarget.Action, 0, routeData.paneId);
        }
    }


    @Input()
    set paneId(id: number) {

        const routeData = this.urlManager.getRouteData().pane()[id];
        this.setupList(routeData);


    }
}