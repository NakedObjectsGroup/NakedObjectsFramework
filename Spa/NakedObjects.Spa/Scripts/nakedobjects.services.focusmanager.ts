/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


namespace NakedObjects {

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
        MultiLineDialogRow
    }

    export interface IFocusManager {
        focusOn(target: FocusTarget, index: number, paneId: number, count?: number): void;

        focusOverrideOn(target: FocusTarget, index: number, paneId: number): void;
        focusOverrideOff(): void;

        setCurrentPane(paneId: number): void;

        refresh(paneId: number): void;
    }

    app.service("focusManager", function($timeout: ng.ITimeoutService, $rootScope: ng.IRootScopeService) {
        const helper = <IFocusManager>this;
        let currentTarget: FocusTarget;
        let currentIndex: number;
        let override = false;
        let focusedPane = 1;

        helper.setCurrentPane = (paneId: number) => {
            focusedPane = paneId;
        };
        helper.focusOn = (target: FocusTarget, index: number, paneId: number, count = 0) => {

            if (paneId === focusedPane) {
                if (!override) {
                    currentTarget = target;
                    currentIndex = index;
                }

                $timeout(() => {
                    $rootScope.$broadcast(geminiFocusEvent, currentTarget, currentIndex, paneId, ++count);
                }, 100, false);
            }
        };
        helper.refresh = (paneId: number) => {
            focusedPane = paneId;
            helper.focusOn(currentTarget, currentIndex, paneId);
        };
        helper.focusOverrideOn = (target: FocusTarget, index: number, paneId: number) => {
            focusedPane = paneId;
            override = true;
            currentTarget = target;
            currentIndex = index;
            helper.focusOn(target, index, paneId);
        };
        helper.focusOverrideOff = () => {
            override = false;
        };
    });
}