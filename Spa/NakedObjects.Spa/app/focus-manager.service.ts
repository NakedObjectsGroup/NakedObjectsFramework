

import * as Nakedobjectsconstants from "./nakedobjects.constants";

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

export class FocusManager {

    private currentTarget: FocusTarget;
    private currentIndex: number;
    private override = false;
    private focusedPane = 1;

    setCurrentPane = (paneId: number) => {
        this.focusedPane = paneId;
    };


    focusOn = (target: FocusTarget, index: number, paneId: number, count = 0) => {

        if (paneId === this.focusedPane) {
            if (!this.override) {
                this.currentTarget = target;
                this.currentIndex = index;
            }

            //$timeout(() => {
            //    $rootScope.$broadcast(Nakedobjectsconstants.geminiFocusEvent, currentTarget, currentIndex, paneId, ++count);
            //}, 100, false);
        }
    };

    refresh = (paneId: number) => {
        this.focusedPane = paneId;
        this.focusOn(this.currentTarget, this.currentIndex, paneId);
    };

    focusOverrideOn = (target: FocusTarget, index: number, paneId: number) => {
        this.focusedPane = paneId;
        this.override = true;
        this.currentTarget = target;
        this.currentIndex = index;
        this.focusOn(target, index, paneId);
    };

    focusOverrideOff = () => {
        this.override = false;
    };
}