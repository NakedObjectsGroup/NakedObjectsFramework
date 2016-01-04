/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export enum FocusTarget {
        Menu, 
        SubAction, 
        Action, 
        ObjectTitle, 
        Dialog,
        ListItem, 
        Property,
        TableItem,
        Input,
        CheckBox,
    }

    export const geminiFocusEvent = "geminiFocuson";

    export interface IFocusManager {
        focusOn(target: FocusTarget, index: number, paneId: number, count?: number): void;

        focusOverrideOn(target: FocusTarget, index: number, paneId: number): void;
        focusOverrideOff(): void;

        refresh(paneId: number): void;
    }

    app.service("focusManager", function ($timeout : ng.ITimeoutService,  $rootScope : ng.IRootScopeService) {
        const helper = <IFocusManager>this;
        let currentTarget: FocusTarget;
        let currentIndex: number;
        let override = false;

        helper.focusOn = (target: FocusTarget, index: number, paneId: number, count = 0) => {

            if (!override) {
                currentTarget = target;
                currentIndex = index;
            }

            $timeout(() => {
                $rootScope.$broadcast(geminiFocusEvent, currentTarget, currentIndex, paneId, ++count);
            }, 0, false);
        }


        helper.refresh = (paneId: number) =>
            helper.focusOn(currentTarget, currentIndex, paneId);

        helper.focusOverrideOn = (target: FocusTarget, index: number, paneId: number) => {
            override = true;
            currentTarget = target;
            currentIndex = index;
            helper.focusOn(target, index, paneId);
        }

        helper.focusOverrideOff = () => {
            override = false;
        }
    });
}