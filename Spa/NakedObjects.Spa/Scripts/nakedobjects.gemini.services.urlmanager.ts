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

        setObject(resultObject: DomainObjectRepresentation, paneId: number, mode?: ApplicationMode);
        setList(action: ActionMember, paneId: number, dvm?: DialogViewModel);
        setProperty(propertyMember: PropertyMember, paneId: number);
        setItem(link: Link, paneId: number): void;

        toggleObjectMenu(paneId: number): void;

        setCollectionMemberState(paneId: number, collection: CollectionMember, state: CollectionViewState): void;
        setListState(paneId: number, state: CollectionViewState): void;
        setListPaging(paneId: number, newPageSize: number, newPage: number): void;

        setObjectEdit(edit: boolean, paneId: number);
        setHome(paneId: number, mode? : ApplicationMode);

        pushUrlState(paneId: number): void;
        clearUrlState(paneId: number): void;
        popUrlState(onPaneId: number): void;

        swapPanes(): void;
        singlePane(paneId : number) : void;
        setParameterValue: (dialogId: string, p: ParameterViewModel, paneId: number, reload?: boolean) => void;
        setPropertyValue: (obj: DomainObjectRepresentation, p: PropertyViewModel, paneId: number, reload?: boolean) => void;

        currentpane() : number;
        getUrlState: (paneId: number) => { paneType: string;search: Object };
    }

    app.service("urlManager", function ($routeParams: INakedObjectsRouteParams, $location: ng.ILocationService) {
        const helper = <IUrlManager>this;

        const home = "home";
        const menu = "menu";
        const object = "object";
        const collection = "collection";
        const edit = "edit";
        const list = "list";
        const action = "action";
        const dialog = "dialog";
        const parm = "parm";
        const prop = "prop";
        const actions = "actions";
        const page = "page";
        const pageSize = "pageSize";


        const gemini = "gemini";
        const cicero = "cicero";

        const capturedPanes = [];

        let currentPaneId = 1;

        function getIds(typeOfId : string,  paneId : number) {
            return <_.Dictionary<string>> _.pick($routeParams, (v, k) => k.indexOf(typeOfId + paneId) === 0);
        }

        function mapIds(ids : _.Dictionary<string>) : _.Dictionary<string>  {
            //missing from lodash types :-( 
            return (<any>_).mapKeys(ids, (v, k : string) => k.substr(k.indexOf("_") + 1));
        }

        function getAndMapIds(typeOfId: string, paneId: number) {
            const ids = getIds(typeOfId, paneId);
            return mapIds(ids);
        }

        function getMappedValues(mappedIds: _.Dictionary<string>) {
            return _.mapValues(mappedIds, v => Value.fromJsonString(decodeURIComponent(v)));
        }

        function setPaneRouteData(paneRouteData: PaneRouteData, paneId: number) {
            paneRouteData.menuId = $routeParams[menu + paneId];
            paneRouteData.actionId = $routeParams[action + paneId];
            paneRouteData.dialogId = $routeParams[dialog + paneId];

            paneRouteData.objectId = $routeParams[object + paneId];
            paneRouteData.actionsOpen = $routeParams[actions + paneId];
            paneRouteData.edit = $routeParams[edit + paneId] === "true";

            const rawCollectionState: string = $routeParams[collection + paneId];
            paneRouteData.state = rawCollectionState ? CollectionViewState[rawCollectionState] : CollectionViewState.List;

            const collKeyMap = getAndMapIds(collection, paneId);
            paneRouteData.collections = _.mapValues(collKeyMap, v => CollectionViewState[v]);

            const parmKeyMap = getAndMapIds(parm, paneId);
            paneRouteData.parms = getMappedValues(parmKeyMap);

            const propKeyMap = getAndMapIds(prop, paneId);
            paneRouteData.props = getMappedValues(propKeyMap);

            paneRouteData.page = $routeParams[page + paneId];
            paneRouteData.pageSize = $routeParams[pageSize + paneId];
        }

        function setSearch(parmId: string, parmValue: string, clearOthers: boolean) {
            const search = clearOthers ? {} : $location.search();
            search[parmId] = parmValue;
            $location.search(search);
        }

        function clearSearch(parmIds: string[]) {
            let search = $location.search();
             _.forEach(parmIds, parmId => search = _.omit(search, parmId));
            $location.search(search);
        }

        function singlePane() {
            return $location.path().split("/").length <= 3;
        }

        function searchKeysForPane(search: any, paneId: number) {
            const raw = [menu, dialog, object, collection, edit, action, parm, actions];
            const ids = _.map(raw, s => s + paneId);
            return _.filter(_.keys(search), k => _.any(ids, id => k.indexOf(id) === 0));
        }

        function clearPane(search: any, paneId: number) {
            const toClear = searchKeysForPane(search, paneId);
            return _.omit(search, toClear);
        }

        function setupPaneNumberAndTypes(pane: number, newPaneType: string, newMode ?: ApplicationMode) {

            const path = $location.path();
            const segments = path.split("/");
            let [, mode, pane1Type, pane2Type] = segments;
            let changeMode = false;

            if (newMode) {
                const newModeString = newMode.toString().toLowerCase();
                changeMode = mode !== newModeString;
                mode = newModeString;
            }

            // changing item on pane 1
            // make sure pane is of correct type
            if (pane === 1 && pane1Type !== newPaneType) {
                const newPath = `/${mode}/${newPaneType}${singlePane() ? "" : `/${pane2Type}`}`;              
                changeMode = false;
                $location.path(newPath);
            }

            // changing item on pane 2
            // either single pane so need to add new pane of appropriate type
            // or double pane with second pane of wrong type. 
            if (pane === 2 && (singlePane() || pane2Type !== newPaneType)) {
                const newPath = `/${mode}/${pane1Type}/${newPaneType}`;            
                changeMode = false;
                $location.path(newPath);
            }

            if (changeMode) {
                const newPath = `/${mode}/${pane1Type}/${pane2Type}`;
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

        function setObjectSearch(paneId: number, oid: string,  mode?: ApplicationMode) {
            setupPaneNumberAndTypes(paneId, object, mode);

            const search = clearPane($location.search(), paneId);
            search[object + paneId] = oid;
            
            $location.search(search);
        }

        function setParameter(paneId: number, search: any, p: ParameterViewModel) {
            search[`${parm}${paneId}_${p.id}`] = encodeURIComponent(p.getValue().toJsonString());
        }

        helper.setMenu = (menuId: string, paneId: number) => {
            currentPaneId = paneId;
            let search = $location.search();
            if (search[`${menu}${paneId}`] !== menuId) {
                search = clearPane(search, paneId);
                search[`${menu}${paneId}`] = menuId;
                $location.search(search);
            }

        };

        helper.setDialog = (dialogId: string, paneId: number) => {
            currentPaneId = paneId;
            setSearch(`${dialog}${paneId}`, dialogId, false);
        };

        helper.closeDialog = (paneId: number) => {
            currentPaneId = paneId;
            const dialogId = `${dialog}${paneId}`;
            const ids = _.filter(_.keys($location.search()), k => k.indexOf(`${parm}${paneId}`) === 0);
            ids.push(dialogId);

            clearSearch(ids);
        };

        helper.setObject = (resultObject: DomainObjectRepresentation, paneId: number) => {
            currentPaneId = paneId;
            const oid = toOid(resultObject.domainType(), resultObject.instanceId());
            setObjectSearch(paneId, oid);  
        };

        helper.setList = (actionMember: ActionMember, paneId: number, dvm?: DialogViewModel) => {
            currentPaneId = paneId;

            const aid = actionMember.actionId();
            const search = clearPane($location.search(), paneId);

            setupPaneNumberAndTypes(paneId, list);

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
                _.each(dvm.parameters, p => setParameter(paneId, search, p));
            }

            $location.search(search);
        };

        helper.setParameterValue = (dialogId: string, pvm: ParameterViewModel, paneId: number, reload = true) => {
            currentPaneId = paneId;

            const search = $location.search();

            // only add parm if matching dialog (to catch case when swapping panes) 
            if (search[`${dialog}${paneId}`] === dialogId) {
                setParameter(paneId, search, pvm);
                $location.search(search);

                if (!reload) {
                    $location.replace();
                }
            }
        };

        helper.setPropertyValue = (obj: DomainObjectRepresentation, p: PropertyViewModel, paneId: number, reload?: boolean) => {
            currentPaneId = paneId;

            const search = $location.search();
            const oid = toOid(obj.domainType(), obj.instanceId());

            // only add parm if matching object (to catch case when swapping panes) 
            if (search[`${object}${paneId}`] === oid) {

                search[`${prop}${paneId}_${p.id}`] = encodeURIComponent(p.getValue().toJsonString());

                $location.search(search);

                if (!reload) {
                    $location.replace();
                }
            }
        };


        helper.setProperty = (propertyMember: PropertyMember, paneId: number) => {
            currentPaneId = paneId;

            const href = propertyMember.value().link().href();
            const oid = getOidFromHref(href);
            setObjectSearch(paneId, oid);
        };

        helper.setItem = (link: Link, paneId: number) => {
            currentPaneId = paneId;

            const href = link.href();
            const oid = getOidFromHref(href);
            setObjectSearch(paneId, oid);
        };

        helper.toggleObjectMenu = (paneId: number) => {
            currentPaneId = paneId;

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
            currentPaneId = paneId;

            const collectionPrefix = `${collection}${paneId}`;
            setSearch(`${collectionPrefix}_${collectionObject.collectionId() }`, CollectionViewState[state], false);
        };

        helper.setListState = (paneId: number, state: CollectionViewState) => {
            currentPaneId = paneId;

            const collectionPrefix = `${collection}${paneId}`;
            setSearch(collectionPrefix, CollectionViewState[state], false);
        };

        helper.setListPaging = (paneId: number, newPageSize: number, newPage: number) => {
            currentPaneId = paneId;
            const search = $location.search();

            search[`${page}${paneId}`] = newPage;
            search[`${pageSize}${paneId}`] = newPageSize;

            $location.search(search);
        };

        helper.setObjectEdit = (editFlag: boolean, paneId: number) => {
            currentPaneId = paneId;

            setSearch(edit + paneId, editFlag.toString(), false);
        };

        helper.setError = () => {
            $location.path("/gemini/error").search({});
        };

        helper.setHome = (paneId: number, mode?: ApplicationMode) => {
            currentPaneId = paneId;

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
            capturedPanes[paneId] = helper.getUrlState(paneId);
        }

        helper.getUrlState = (paneId: number) => {
            currentPaneId = paneId;

            const path = $location.path();
            const segments = path.split("/");

            const paneType = segments[paneId + 1] || home;
            const paneSearch = capturePane(paneId);

            return { paneType: paneType, search: paneSearch };
        }

        helper.popUrlState = (paneId: number) => {
            currentPaneId = paneId;

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
            currentPaneId = paneId;

            capturedPanes[paneId] = null;
        }


        function swapSearchIds(search: any) {
            return (<any>_).mapKeys(search,
                (v, k: string) => k.replace(/(\D+)(\d{1})(\w*)/, (match, p1, p2, p3) => `${p1}${p2 === "1" ? "2" : "1"}${p3}`));
        }


        helper.swapPanes = () => {
            const path = $location.path();
            const segments = path.split("/");
            const [, mode, oldPane1, oldPane2 = home] = segments;
            const newPath = `/${mode}/${oldPane2}/${oldPane1}`;
            const search = swapSearchIds($location.search());
            currentPaneId = currentPaneId === 1 ? 2 : 1;

            $location.path(newPath).search(search);
        }

        helper.currentpane = () => currentPaneId;

        helper.singlePane = (paneId: number) => {
            currentPaneId = 1;

            if (!singlePane()) {
           
                const paneToKeepId = paneId;
                const paneToRemoveId = paneToKeepId === 1 ? 2 : 1;

                const path = $location.path();
                const segments = path.split("/");
                const mode = segments[1];
                const paneToKeep = segments[paneToKeepId + 1];            
                const newPath = `/${mode}/${paneToKeep}`;

                let search = $location.search();

                if (paneToKeepId === 1) {
                    // just remove second pane
                    search = clearPane(search, paneToRemoveId);
                }

                if (paneToKeepId === 2) {
                    // swap pane 2 to pane 1 then remove 2
                    search = swapSearchIds(search);
                    search = clearPane(search, 2);
                }

                $location.path(newPath).search(search);
            }
        } 
    });

}