import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { RepresentationsService } from "../representations.service";
import { ActivatedRoute} from '@angular/router';
import * as Models from "../models";
import { UrlManagerService } from "../url-manager.service";
import { ClickHandlerService } from "../click-handler.service";
import { ContextService } from "../context.service";
import { RepLoaderService } from "../rep-loader.service";
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ColorService } from "../color.service";
import { ErrorService } from "../error.service";
import { FocusManagerService } from "../focus-manager.service";
import { MaskService } from "../mask.service";
import { PaneRouteData, RouteData, InteractionMode } from "../route-data";
import * as ViewModels from "../view-models";
import { ISubscription } from 'rxjs/Subscription';
import { Subject } from 'rxjs/Subject';

@Component({
    selector: 'object',
    templateUrl: './object.component.html',
    styleUrls: ['./object.component.css']
})

export class ObjectComponent implements OnInit,  OnDestroy {

    constructor(private urlManager: UrlManagerService,
        private context: ContextService,
        private color: ColorService,
        private viewModelFactory: ViewModelFactoryService,
        private focusManager: FocusManagerService,
        private error: ErrorService,
        private activatedRoute :ActivatedRoute) {    
    }

    object : ViewModels.DomainObjectViewModel;

    mode : InteractionMode;


    setupObject(routeData: PaneRouteData) {
        // subscription means may get with no oid 

        if (!routeData.objectId) {
            this.mode = null;
            return;
        }

        this.mode = routeData.interactionMode;

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

    paneIdName = () => this.paneId === 1 ? "pane1" : "pane2"

    getClass() {
        return this.paneType + " " + this.object.color;
    }

    getColor() {
        return this.object.color;
    }

    paneType: string;
    onChild() {
        this.paneType = "split";
    }

    onChildless() {
        this.paneType = "single";
    }

    private activatedRouteDataSub: ISubscription;
    private paneRouteDataSub: ISubscription;

    ngOnInit(): void {

        this.activatedRouteDataSub = this.activatedRoute.data.subscribe(data => {
            this.paneId = data["pane"];
            this.paneType = data["class"];

            this.paneRouteDataSub = this.urlManager.getRouteDataObservable()
                .subscribe((rd: RouteData) => {
                    const paneRouteData = rd.pane()[this.paneId];
                    this.setupObject(paneRouteData);
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

    paneId: number;
}