import { Component, OnInit } from '@angular/core';
import { PaneComponent } from '../pane/pane';
import { ViewModelFactoryService } from "../view-model-factory.service";
import { ActivatedRoute } from '@angular/router';
import { UrlManagerService } from "../url-manager.service";
import { ContextService } from "../context.service";
import { ErrorService } from "../error.service";
import { RouteData, PaneRouteData } from "../route-data";
import { ActionViewModel } from '../view-models/action-view-model';
import { CollectionViewModel } from '../view-models/collection-view-model';
import * as Models from "../models";
import * as _ from "lodash";

@Component({
    selector: 'app-multi-line-dialog',
    templateUrl: './multi-line-dialog.component.html',
    styleUrls: ['./multi-line-dialog.component.css']
})
export class MultiLineDialogComponent extends PaneComponent {

    constructor(
        activatedRoute: ActivatedRoute,
        urlManager: UrlManagerService,
        private viewModelFactory: ViewModelFactoryService,
        private context: ContextService,
        private error: ErrorService
    ) {
        super(activatedRoute, urlManager);
    }

    get objectFriendlyName() {
        return "";
    }

    get objectTitle() {
        return "";
    }

    get dialogTitle() {
        return "";
    }

    get header() {
        return [];
    }

    get dialogs() {
        return [];
    }

    get submitted() {
        return false;
    }

    get message() {
        return this.submitted ? 'Submitted' : "this.dialog.message";
    }

    get count() {
        return "' with '+multiLineDialog.submittedCount()+' lines submitted.'";
    }

    get toolTip() {
        return "";
    }

    close = () => {

    }

    setMultiLineDialog(holder: Models.MenuRepresentation | Models.DomainObjectRepresentation | CollectionViewModel,
        newDialogId: string,
        routeData: PaneRouteData,
        actionViewModel?: ActionViewModel) {

        const action = holder.actionMember(newDialogId);
        this.context.getInvokableAction(action).
            then(details => {

                if (actionViewModel) {
                    actionViewModel.makeInvokable(details);
                }

                // $scope.multiLineDialogTemplate = multiLineDialogTemplate;
                // $scope.parametersTemplate = parametersTemplate;
                // $scope.parameterTemplate = parameterTemplate;
                // $scope.readOnlyParameterTemplate = readOnlyParameterTemplate;

                // const dialogViewModel = perPaneMultiLineDialogViews[routeData.paneId];                   
                // dialogViewModel.reset(routeData, details);

                // if (holder instanceof DomainObjectRepresentation) {
                //     dialogViewModel.objectTitle = holder.title();
                //     dialogViewModel.objectFriendlyName = holder.extensions().friendlyName();        
                // } else {
                //     dialogViewModel.objectFriendlyName = "";
                //     dialogViewModel.objectTitle = "";
                // }

                // $scope.multiLineDialog = dialogViewModel;
            }).
            catch((reject: Models.ErrorWrapper) => this.error.handleError(reject));
    }

    protected setup(routeData: PaneRouteData) {
        if (routeData.menuId) {
            this.context.getMenu(routeData.menuId)
                .then((menu: Models.MenuRepresentation) => {
                    this.setMultiLineDialog(menu, routeData.dialogId, routeData);
                })
                .catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        }
        else if (routeData.objectId) {
            const oid = Models.ObjectIdWrapper.fromObjectId(routeData.objectId);
            this.context.getObject(routeData.paneId, oid, routeData.interactionMode).
                then((object: Models.DomainObjectRepresentation) => {

                    // const ovm = perPaneObjectViews[routeData.paneId].reset(object, routeData);
                    // const newDialogId = routeData.dialogId;

                    // const lcaCollection = _.find(ovm.collections, c => c.hasMatchingLocallyContributedAction(newDialogId));

                    // if (lcaCollection) {
                    //     const actionViewModel = _.find(lcaCollection.actions, a => a.actionRep.actionId() === newDialogId);
                    //     setMultiLineDialog($scope, lcaCollection, newDialogId, routeData, actionViewModel);
                    // } else {
                    //     setMultiLineDialog($scope, object, newDialogId, routeData);
                    // }

                }).
                catch((reject: Models.ErrorWrapper) => {
                    this.error.handleError(reject);
                });
        }
    }

}
