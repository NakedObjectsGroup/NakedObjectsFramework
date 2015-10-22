/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export enum CollectionViewState {
        Summary, 
        List,
        Table
    }

    export enum ApplicationMode {
        Gemini,
        Cicero
    }

    export class RouteData {
        constructor() {
            this.pane1 = new PaneRouteData(1);
            this.pane2 = new PaneRouteData(2);
        }

        pane1: PaneRouteData;
        pane2: PaneRouteData;
    }

    export class PaneRouteData {
        constructor(public paneId : number) {}

        objectId : string;
        menuId : string;
        collections: _.Dictionary<CollectionViewState>;
        edit: boolean;
        actionsOpen : string;
        actionId: string;
        state: CollectionViewState;
        // todo make parm ids dictionary same as collections ids ? 
        parms: { id: string; val: Value }[];
        dialogId : string;
    }
}