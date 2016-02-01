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

        pane = () => [, this.pane1, this.pane2];
    }

    export class PaneRouteData {
        constructor(public paneId: number) {}

        objectId: string;
        menuId: string;
        collections: _.Dictionary<CollectionViewState>;
        edit: boolean;
        transient: boolean;
        actionsOpen: string;
        actionId: string;
        //Note that actionParams applies to executed actions. For dialogs see dialogFields
        actionParams: _.Dictionary<Value>;
        state: CollectionViewState;
        props: _.Dictionary<Value>;
        dialogId: string;
        dialogFields: _.Dictionary<Value>;
        page: number;
        pageSize : number;
        selectedItems: boolean[];
    }
}