import { Component, Input } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';
import { FooterComponent } from "./footer.component";
import { RepresentationsService } from "./representations.service";
import { ActivatedRoute, Router, UrlPathWithParams} from '@angular/router';
import * as Models from "./models";
import { UrlManager } from "./urlmanager.service";
import { ClickHandlerService } from "./click-handler.service";
import { Context } from "./context.service";
import { RepLoader } from "./reploader.service";
import { ViewModelFactory } from "./view-model-factory.service";
import { Color } from "./color.service";
import { Error } from "./error.service";
import { FocusManager } from "./focus-manager.service";
import { Mask } from "./mask.service";
import { PaneRouteData } from "./nakedobjects.routedata";
import * as ViewModels from "./nakedobjects.viewmodels";
import { ActionsComponent } from "./actions.component";
import { GeminiClickDirective } from "./gemini-click.directive";
import { PropertiesComponent } from "./properties.component";

@Component({
    selector: 'object',
    templateUrl: `app/object.component.html`,
    directives: [ROUTER_DIRECTIVES, FooterComponent, ActionsComponent, GeminiClickDirective, PropertiesComponent]
})

export class ObjectComponent {

    constructor(private urlManager: UrlManager,
        private context: Context,
        private color: Color,
        private viewModelFactory: ViewModelFactory,
        private focusManager: FocusManager,
        private error: Error,
        private activatedRoute :ActivatedRoute) {    
    }

    object : ViewModels.DomainObjectViewModel;

    setupObject(routeData : PaneRouteData) {

        const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId);

        // to ease transition 
        //$scope.objectTemplate = Nakedobjectsconstants.blankTemplate;
        //$scope.actionsTemplate = Nakedobjectsconstants.nullTemplate;

        //this.color.toColorNumberFromType(oid.domainType).
        //    then(c =>
        //        $scope.backgroundColor = `${Nakedobjectsconfig.objectColor}${c}`).
        //    catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));

        //deRegObject[routeData.paneId].deReg();
        this.context.clearObjectUpdater(routeData.paneId);

        const wasDirty = this.context.getIsDirty(oid);

        this.context.getObject(routeData.paneId, oid, routeData.interactionMode).
            then((object: Models.DomainObjectRepresentation) => {

                const ovm = new ViewModels.DomainObjectViewModel(this.color, this.context, this.viewModelFactory, this.urlManager, this.focusManager, this.error  );
                ovm.reset(object, routeData);
                if (wasDirty) {
                    ovm.clearCachedFiles();
                }

                this.object = ovm;

                //$scope.object = ovm;
                //$scope.collectionsTemplate = Nakedobjectsconstants.collectionsTemplate;

                //handleNewObjectSearch($scope, routeData);

                //deRegObject[routeData.paneId].add($scope.$on(Nakedobjectsconstants.geminiConcurrencyEvent, ovm.concurrency()) as () => void);
            }).
            catch((reject: Models.ErrorWrapper) => {
                if (reject.category === Models.ErrorCategory.ClientError && reject.clientErrorCode === Models.ClientErrorCode.ExpiredTransient) {
                    this.context.setError(reject);
                   // $scope.objectTemplate = Nakedobjectsconstants.expiredTransientTemplate;
                } else {
                    //this.error.handleError(reject);
                }
            });
    }


    @Input()
    set paneId(id: number) {

        const routeData = this.urlManager.getRouteData().pane()[id];
        this.setupObject(routeData);

     
    }
}