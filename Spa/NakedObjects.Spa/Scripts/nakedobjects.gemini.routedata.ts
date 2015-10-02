//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

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
        parms: { id: string; val: string }[];
        dialogId : string;
    }
}