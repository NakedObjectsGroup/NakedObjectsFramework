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
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface IUrlManager {
        getRouteData(): RouteData;

        setError();
        setMenu(menuId: string);
        setDialog(actionId: string);
        closeDialog();
        setObject(resultObject: DomainObjectRepresentation, paneId : number);
        setQuery(action: ActionMember, dvm?: DialogViewModel);
        setProperty(propertyMember: PropertyMember, paneId : number);
        setItem(link: Link): void;

        toggleObjectMenu(paneId : number): void;

        setCollectionState(collection: CollectionMember, state: CollectionViewState);
        setCollectionState(collection: ListRepresentation, state: CollectionViewState);

        setObjectEdit(edit: boolean, paneId : number);
    }

    app.service("urlManager", function($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService) {
        const helper = <IUrlManager>this;

        const menu = "menu";
        const object = "object";
        const collection = "collection";
        const edit = "edit";
        const action = "action";
        const parm = "parm";


        function setSearch(parmId: string, parmValue: string, clearOthers: boolean) {
            const search = clearOthers ? {} : $location.search();
            search[parmId] = parmValue;
            $location.search(search);
        }

        function clearSearch(parmId: string) {
            let search = $location.search();
            search = _.omit(search, parmId);
            $location.search(search);
        }


        helper.setMenu = (menuId: string) => {
            setSearch("menu1", menuId, true);
        };

        helper.setDialog = (actionId: string) => {
            setSearch("action1", actionId, false);
        };

        helper.closeDialog = () => {
            clearSearch("action1");
        };

        function singlePane() {
            return $location.path().split("/").length <= 2;
        }

        function setupPaneNumberAndTypes(pane: number, newPaneType : string) {
        
       
            const path = $location.path();
            const segments = path.split("/");

            // changing item on pane 1
            // make sure pane is of correct type
            if (pane === 1 && segments[1] !== newPaneType) {
                const newPath = `/${newPaneType}${singlePane() ? "" : `/${segments[2]}`  }`;
                $location.path(newPath);
            }

            // changing item on pane 2
            // either single pane so need to add new pane of appropriate type
            // or double pane with second pane of wrong type. 
            if (pane === 2 && (singlePane() || segments[2] !== newPaneType)) {
                const newPath = `/${segments[1]}/${newPaneType}`;
                $location.path(newPath);
            }
       
        }

        helper.setObject = (resultObject: DomainObjectRepresentation, paneId : number) => {

            setupPaneNumberAndTypes(paneId, object);

            const oidParm = object + paneId;
            const editParm = edit + paneId;
            const oid = `${resultObject.domainType() }-${resultObject.instanceId() }`;
            let search = $location.search();
            search[oidParm] = oid;
            // clear edit 
            search = _.omit(search, editParm);

            $location.search(search);
        };

        helper.setQuery = (action: ActionMember, dvm?: DialogViewModel) => {
            const aid = action.actionId();
            const search = $location.search();

            search.action1 = aid;

            if (dvm) {
                _.each(dvm.parameters, (p) => search[`parm1_${p.id}`] = p.getValue());
            }

            $location.path("/query").search(search);
        };

        helper.setProperty = (propertyMember: PropertyMember, paneId : number) => {
            const href = propertyMember.value().link().href();
            const urlRegex = /(objects|services)\/(.*)\/(.*)/;
            const results = (urlRegex).exec(href);
            const oid = `${results[2]}-${results[3]}`;

            setupPaneNumberAndTypes(paneId, object);
            
            const search = $location.search();
            search[object + paneId] = oid;
 
            $location.search(search);
        };

        helper.setItem = (link: Link) => {
            var href = link.href();
            const urlRegex = /(objects|services)\/(.*)\/(.*)/;
            const results = (urlRegex).exec(href);

            const oid = `${results[2]}-${results[3]}`;
            $location.path("/object").search({ object1: oid });
        };

       

        helper.toggleObjectMenu = (paneId : number) => {
            let search = $location.search();
            const paneMenuId = menu + paneId;
            const menuId = search[paneMenuId];

            if (menuId) {
                search = _.omit(search, paneMenuId);
            } else {
                search[paneMenuId] = "actions";
            }

            $location.search(search);
        };

        helper.setCollectionState = (collection: any, state: CollectionViewState) => {
            if (collection instanceof CollectionMember) {
                setSearch(`collection1_${collection.collectionId() }`, CollectionViewState[state], false);
            } else {
                setSearch("collection1", CollectionViewState[state], false);
            }
        };
      
        helper.setObjectEdit = (editFlag: boolean, paneId : number) => {
            setSearch(edit + paneId, editFlag.toString(), false);
        };

        helper.setError = () => {
            $location.path("/error").search({});
        };

        function setPaneRouteData(paneRouteData : PaneRouteData, index : number) {
            paneRouteData.menuId = $routeParams[menu + index];
            paneRouteData.actionId = $routeParams[action + index];
            paneRouteData.objectId = $routeParams[object + index];

            const collIds = <{ [index: string]: string }> _.pick($routeParams, (v: string, k: string) => k.indexOf(collection + index) === 0);
            //missing from lodash types :-( 
            const keysMapped: _.Dictionary<string> = (<any>_).mapKeys(collIds, (v, k) => k.substr(k.indexOf("_") + index));
            paneRouteData.collections = _.mapValues(keysMapped, (v) => CollectionViewState[v]);

            paneRouteData.edit = $routeParams[edit + index] === "true";

            const rawCollectionState : string = $routeParams[collection + index];
            paneRouteData.state = rawCollectionState ? CollectionViewState[rawCollectionState] : CollectionViewState.List;

            // todo make parm ids dictionary same as collections ids ? 
            const parmIds = <{ [index: string]: string }> _.pick($routeParams, (v, k) => k.indexOf(parm + index) === 0);
            paneRouteData.parms = _.map(parmIds, (v, k) => { return { id: k.substr(k.indexOf("_") + index), val: v } });

        }

        helper.getRouteData = () => {
            const routeData = new RouteData();
            
            setPaneRouteData(routeData.pane1, 1);
            setPaneRouteData(routeData.pane2, 2);

            return routeData;
        };
    });

}