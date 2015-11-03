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
        focusOn(target: FocusTarget, paneId: number, count? : number): void;
        refresh(paneId: number): void;
    }

    app.service("focusManager", function ($timeout : ng.ITimeoutService,  $rootScope : ng.IRootScopeService) {
        const helper = <IFocusManager>this;
        let currentTarget: FocusTarget;

        helper.focusOn = (target: FocusTarget, paneId: number, count = 0) =>
            $timeout(() => $rootScope.$broadcast(geminiFocusEvent, (currentTarget = target), paneId, ++count), 0, false);

        helper.refresh = (paneId: number) =>
            helper.focusOn(currentTarget, paneId);
    });
}