/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export enum FocusTarget {
        FirstMenu, 
        FirstAction, 
        ObjectTitle, 
        Dialog,
        FirstItem, 
        FirstProperty
    }

    export const geminiFocusEvent = "geminiFocuson";

    export interface IFocusManager {
        focusOn(target: FocusTarget, paneId : number): void;
    }

    app.service("focusManager", function ($timeout : ng.ITimeoutService,  $rootScope : ng.IRootScopeService) {
        const helper = <IFocusManager>this;

        helper.focusOn = (target: FocusTarget, paneId: number) =>
            $timeout(() => $rootScope.$broadcast(geminiFocusEvent, target, paneId), 0, false);
    });
}