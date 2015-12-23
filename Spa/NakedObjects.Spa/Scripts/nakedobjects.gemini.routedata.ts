/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export enum ViewType {
        Home,
        Object,
        List
    }

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
        constructor(public paneId: number) {}

        objectId: string;
        menuId: string;
        collections: _.Dictionary<CollectionViewState>;
        edit: boolean;
        actionsOpen: string;
        actionId: string;
        state: CollectionViewState;
        parms: _.Dictionary<Value>;
        fields: _.Dictionary<Value>;
        props: _.Dictionary<Value>;
        dialogId: string;
        page: number;
        pageSize : number;
        selectedItems: boolean[];
    }
}