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
        setMenu(menuId: string, paneId: number);
        setDialog(dialogId: string, paneId: number);
        closeDialog(paneId: number);

        setObject(resultObject: DomainObjectRepresentation, paneId: number);
        setQuery(action: ActionMember, paneId: number, dvm?: DialogViewModel);
        setProperty(propertyMember: PropertyMember, paneId: number);
        setItem(link: Link, paneId: number): void;

        toggleObjectMenu(paneId: number): void;

        setCollectionMemberState(paneId: number, collection: CollectionMember, state: CollectionViewState): void;
        setQueryState(paneId: number, state: CollectionViewState): void;

        setObjectEdit(edit: boolean, paneId: number);
        setHome(paneId: number);

        pushUrlState(paneId: number): void;
        clearUrlState(paneId: number): void;
        popUrlState(onPaneId: number): void;
    }

    app.service("urlManager", function ($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService) {
        const helper = <IUrlManager>this;

        const home = "home";
        const menu = "menu";
        const object = "object";
        const collection = "collection";
        const edit = "edit";
        const query = "query";
        const action = "action";
        const dialog = "dialog";
        const parm = "parm";
        const actions = "actions";

        const capturedPanes = [];

        function setPaneRouteData(paneRouteData: PaneRouteData, paneId: number) {
            paneRouteData.menuId = $routeParams[menu + paneId];
            paneRouteData.actionId = $routeParams[action + paneId];
            paneRouteData.dialogId = $routeParams[dialog + paneId];

            paneRouteData.objectId = $routeParams[object + paneId];
            paneRouteData.actionsOpen = $routeParams[actions + paneId];
            paneRouteData.edit = $routeParams[edit + paneId] === "true";

            const rawCollectionState: string = $routeParams[collection + paneId];
            paneRouteData.state = rawCollectionState ? CollectionViewState[rawCollectionState] : CollectionViewState.List;

            const collIds = <{ [index: string]: string }> _.pick($routeParams, (v: string, k: string) => k.indexOf(collection + paneId) === 0);
            //missing from lodash types :-( 
            const keysMapped: _.Dictionary<string> = (<any>_).mapKeys(collIds, (v, k) => k.substr(k.indexOf("_") + 1));
            paneRouteData.collections = _.mapValues(keysMapped, v => CollectionViewState[v]);

            // todo make parm ids dictionary same as collections ids ? 
            const parmIds = <{ [index: string]: string }> _.pick($routeParams, (v, k) => k.indexOf(parm + paneId) === 0);
            paneRouteData.parms = _.map(parmIds, (v, k) => { return { id: k.substr(k.indexOf("_") + paneId), val: v } });
        }

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

        function singlePane() {
            return $location.path().split("/").length <= 2;
        }

        function searchKeysForPane(search: any, paneId: number) {
            const raw = [menu, dialog, object, collection, edit, action, parm, actions];
            const ids = _.map(raw, s => s + paneId);
            return _.filter(_.keys(search), (k) => _.any(ids, id => k.indexOf(id) === 0));
        }

        function clearPane(search: any, paneId: number) {
            const toClear = searchKeysForPane(search, paneId);
            return _.omit(search, toClear);
        }

        function setupPaneNumberAndTypes(pane: number, newPaneType: string) {


            const path = $location.path();
            const segments = path.split("/");

            // changing item on pane 1
            // make sure pane is of correct type
            if (pane === 1 && segments[1] !== newPaneType) {
                const newPath = `/${newPaneType}${singlePane() ? "" : `/${segments[2]}`}`;
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

        function capturePane(paneId: number) {
            const search = $location.search();
            const toCapture = searchKeysForPane(search, paneId);

            return _.pick(search, toCapture);
        }

        function toOid(dt: string, id: string) {
            // todo does this formatting belong here ?
            return `${dt}-${id}`;
        }

        function getOidFromHref(href: string) {
            const urlRegex = /(objects|services)\/(.*)\/(.*)/;
            const results = (urlRegex).exec(href);
            return toOid(results[2],results[3]);
        }

        function setObjectSearch(paneId: number, oid: string) {
            setupPaneNumberAndTypes(paneId, object);

            const search = clearPane($location.search(), paneId);
            search[object + paneId] = oid;
            $location.search(search);
        }

        helper.setMenu = (menuId: string, paneId: number) => {
            setSearch(`${menu}${paneId}`, menuId, false);
        };

        helper.setDialog = (dialogId: string, paneId: number) => {
            setSearch(`${dialog}${paneId}`, dialogId, false);
        };

        helper.closeDialog = (paneId: number) => {
            clearSearch(`${dialog}${paneId}`);
        };

        helper.setObject = (resultObject: DomainObjectRepresentation, paneId: number) => {
            const oid = toOid(resultObject.domainType(), resultObject.instanceId());
            setObjectSearch(paneId, oid);  
        };

        helper.setQuery = (actionMember: ActionMember, paneId: number, dvm?: DialogViewModel) => {
            const aid = actionMember.actionId();
            const search = clearPane($location.search(), paneId);

            setupPaneNumberAndTypes(paneId, query);

            const parent = actionMember.parent;

            if (parent instanceof DomainObjectRepresentation) {
                const oidParm = object + paneId;
                const oid = toOid(parent.domainType(), parent.instanceId());
                search[oidParm] = oid;
            }

            if (parent instanceof MenuRepresentation) {
                const menuParm = menu + paneId;
                const menuId = parent.menuId();
                search[menuParm] = menuId;
            }

            search[action + paneId] = aid;

            if (dvm) {
                _.each(dvm.parameters, p => search[`parm${paneId}_${p.id}`] = p.getValue().toString());
            }

            $location.search(search);
        };

        helper.setProperty = (propertyMember: PropertyMember, paneId: number) => {
            const href = propertyMember.value().link().href();
            const oid = getOidFromHref(href);
            setObjectSearch(paneId, oid);
        };

        helper.setItem = (link: Link, paneId: number) => {
            const href = link.href();
            const oid = getOidFromHref(href);
            setObjectSearch(paneId, oid);
        };

        helper.toggleObjectMenu = (paneId: number) => {
            let search = $location.search();
            const paneActionsId = actions + paneId;
            const actionsId = search[paneActionsId];

            if (actionsId) {
                search = _.omit(search, paneActionsId);
            } else {
                search[paneActionsId] = "open";
            }

            $location.search(search);
        };

        helper.setCollectionMemberState = (paneId: number, collectionObject: CollectionMember, state: CollectionViewState) => {
            const collectionPrefix = `${collection}${paneId}`;
            setSearch(`${collectionPrefix}_${collectionObject.collectionId() }`, CollectionViewState[state], false);
        };

        helper.setQueryState = (paneId: number, state: CollectionViewState) => {
            const collectionPrefix = `${collection}${paneId}`;
            setSearch(collectionPrefix, CollectionViewState[state], false);
        };


        helper.setObjectEdit = (editFlag: boolean, paneId: number) => {
            setSearch(edit + paneId, editFlag.toString(), false);
        };

        helper.setError = () => {
            $location.path("/error").search({});
        };

        helper.setHome = (paneId: number) => {
            setupPaneNumberAndTypes(paneId, home);
            // clear search on this pane 
            $location.search(clearPane($location.search(), paneId));
        }

        helper.getRouteData = () => {
            const routeData = new RouteData();

            setPaneRouteData(routeData.pane1, 1);
            setPaneRouteData(routeData.pane2, 2);

            return routeData;
        };

        helper.pushUrlState = (paneId: number) => {
            const path = $location.path();
            const segments = path.split("/");

            const paneType = segments[paneId] || home;
            const paneSearch = capturePane(paneId);

            capturedPanes[paneId] = { paneType: paneType, search: paneSearch };
        }

        helper.popUrlState = (paneId: number) => {
            const capturedPane = capturedPanes[paneId];

            if (capturedPane) {
                capturedPanes[paneId] = null;
                let search = clearPane($location.search(), paneId);
                search = _.merge(search, capturedPane.search);
                setupPaneNumberAndTypes(paneId, capturedPane.paneType);
                $location.search(search);
            }
        }

        helper.clearUrlState = (paneId: number) => {
            capturedPanes[paneId] = null;
        }
    });

}