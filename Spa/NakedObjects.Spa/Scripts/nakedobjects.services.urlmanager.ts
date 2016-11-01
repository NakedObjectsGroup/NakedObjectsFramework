/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


namespace NakedObjects {

    import Decompress = Models.decompress;
    import Compress = Models.compress;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;
    import HttpStatusCode = Models.HttpStatusCode;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import PropertyMember = Models.PropertyMember;
    import Link = Models.Link;
    import Parameter = Models.Parameter;
    import Value = Models.Value;
    import MenuRepresentation = Models.MenuRepresentation;
    import IAction = Models.IInvokableAction;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import propertyIdFromUrl = Models.propertyIdFromUrl;
    import getOtherPane = Models.getOtherPane;

    export interface IUrlManager {
        getRouteData(): RouteData;
        setError(errorCategory: ErrorCategory, ec?: ClientErrorCode | HttpStatusCode): void;

        setRecent(paneId?: number): void;
        setHome(paneId?: number): void;
        setMenu(menuId: string, paneId?: number): void;
        setDialog(dialogId: string, paneId?: number): void;

        setMultiLineDialog(dialogId: string, paneId?: number): void;

        setDialogOrMultiLineDialog(actionRep: Models.ActionMember | Models.ActionRepresentation, paneId? : number) : void;

        closeDialogKeepHistory(paneId?: number): void;

        closeDialogReplaceHistory(paneId?: number): void;

        setObject(resultObject: DomainObjectRepresentation, paneId?: number): void;
        setList(action: IAction, parms : _.Dictionary<Value>,  fromPaneId? : number, paneId?: number): void;
        setProperty(propertyMember: PropertyMember, paneId?: number): void;
        setItem(link: Link, paneId?: number): void;

        setAttachment(attachmentLink: Link, paneId?: number): void;

        toggleObjectMenu(paneId?: number): void;

        setInteractionMode(newMode: InteractionMode, paneId?: number): void;
        setCollectionMemberState(collectionMemberId: string, state: CollectionViewState, paneId?: number): void;
        setListState(state: CollectionViewState, paneId?: number): void;
        setListPaging(newPage: number, newPageSize: number, state: CollectionViewState, paneId?: number): void;
        setListItem(item: number, selected: boolean, paneId?: number): void;

        pushUrlState(paneId?: number): void;
        clearUrlState(paneId?: number): void;
        popUrlState(onPaneId?: number): void;

        swapPanes(): void;
        singlePane(paneId?: number): void;

        setParameterValue: (actionId: string, p: Parameter, pv: Value, paneId?: number) => void;

        currentpane(): number;

        getUrlState: (paneId?: number) => { paneType: string; search: Object };
        getListCacheIndex: (paneId: number, newPage: number, newPageSize: number, format?: CollectionViewState) => string;

        isHome(paneId?: number): boolean;
        isObject(paneId?: number): boolean;
        isList(paneId?: number): boolean;
        isError(paneId?: number): boolean;
        isRecent(paneId?: number): boolean;
        isAttachment(paneId?: number): boolean;
        isApplicationProperties(paneId?: number): boolean;

        cicero(): void;

        reload(): void;
        applicationProperties(): void;

        //Flips the reload parameter in the Url between 0 and 1
        //which serves only to alert Angular and reload the page as needed.
        triggerPageReloadByFlippingReloadFlagInUrl() : void;
    }

    app.service("urlManager", function ($routeParams: ng.route.IRouteParamsService, $location: ng.ILocationService, $window: ng.IWindowService) {
        const helper = <IUrlManager>this;

        // keep in alphabetic order to help avoid name collisions 

        // all key map
        const akm = {
            action: "a",
            actions: "as",
            attachment: "at",
            collection: "c",
            dialog: "d",
            errorCat: "et",
            interactionMode: "i",
            menu: "m",
            object: "o",
            page: "pg",
            pageSize: "ps",
            parm: "pm",
            prop: "pp",
            reload: "r",
            selected: "s"
        };

        const capturedPanes = [] as { paneType: string; search: Object }[];

        let currentPaneId = 1;

        function createSubMask(arr: boolean[]) {
            let nMask = 0;
            let nFlag = 0;

            if (arr.length > 31) {
                throw new TypeError("createSubMask - out of range");
            }

            const nLen = arr.length;
            for (nFlag; nFlag < nLen; nMask |= (<any>arr)[nFlag] << nFlag++);
            return nMask;
        }


        // convert from array of bools to mask string
        function createArrays(arr: boolean[], arrays?: boolean[][]): boolean[][] {

            arrays = arrays || [];

            if (arr.length > 31) {
                arrays.push(arr.slice(0, 31));
                return createArrays(arr.slice(31), arrays);
            }

            arrays.push(arr);
            return arrays;
        }


        function createMask(arr: boolean[]) {
            // split into smaller arrays if necessary 

            const arrays = createArrays(arr);
            const masks = _.map(arrays, a => createSubMask(a).toString());

            return _.reduce(masks, (res, val) => res + "-" + val);
        }

        // convert from mask string to array of bools
        function arrayFromSubMask(sMask: string) {
            const nMask = parseInt(sMask);
            // nMask must be between 0 and 2147483647 - to keep simple we stick to 31 bits 
            if (nMask > 0x7fffffff || nMask < -0x80000000) {
                throw new TypeError("arrayFromMask - out of range");
            }
            const aFromMask = [] as boolean[];
            let len = 31; // make array always 31 bit long as we may concat another on end
            for (let nShifted = nMask; len > 0; aFromMask.push(Boolean(nShifted & 1)), nShifted >>>= 1, --len);
            return aFromMask;
        }

        function arrayFromMask(sMask: string) {
            sMask = sMask || "0";
            const sMasks = sMask.split("-");
            const maskArrays = _.map(sMasks, s => arrayFromSubMask(s));
            return _.reduce(maskArrays, (res, val) => res.concat(val), [] as boolean[]);
        }

        function getSearch() {
            return $location.search();
        }

        function setNewSearch(search: any) {
            $location.search(search);
        }

        function getIds(typeOfId: string, paneId: number) {
            return <_.Dictionary<string>>_.pickBy($routeParams, (v, k) => k.indexOf(typeOfId + paneId) === 0);
        }

        function mapIds(ids: _.Dictionary<string>): _.Dictionary<string> {
            return _.mapKeys(ids, (v: any, k: string) => k.substr(k.indexOf("_") + 1));
        }

        function getAndMapIds(typeOfId: string, paneId: number) {
            const ids = getIds(typeOfId, paneId);
            return mapIds(ids);
        }

        function getMappedValues(mappedIds: _.Dictionary<string>) {
            return _.mapValues(mappedIds, v => Value.fromJsonString(v));
        }

        function getInteractionMode(rawInteractionMode: string): InteractionMode {
            return rawInteractionMode ? (<any>InteractionMode)[rawInteractionMode] : InteractionMode.View;
        }

        function setPaneRouteData(paneRouteData: PaneRouteData, paneId: number) {

            paneRouteData.menuId = getId(akm.menu + paneId, $routeParams);
            paneRouteData.actionId = getId(akm.action + paneId, $routeParams);
            paneRouteData.dialogId = getId(akm.dialog + paneId, $routeParams);

            const rawErrorCategory = getId(akm.errorCat + paneId, $routeParams);
            paneRouteData.errorCategory = rawErrorCategory ? (<any>ErrorCategory)[rawErrorCategory] : null;

            paneRouteData.objectId = getId(akm.object + paneId, $routeParams);
            paneRouteData.actionsOpen = getId(akm.actions + paneId, $routeParams);

            const rawCollectionState = getId(akm.collection + paneId, $routeParams);
            paneRouteData.state = rawCollectionState ? (<any>CollectionViewState)[rawCollectionState] : CollectionViewState.List;

            const rawInteractionMode = getId(akm.interactionMode + paneId, $routeParams);
            paneRouteData.interactionMode = getInteractionMode(rawInteractionMode);

            const collKeyMap = getAndMapIds(akm.collection, paneId);
            paneRouteData.collections = _.mapValues(collKeyMap, v => (<any>CollectionViewState)[v]);

            const parmKeyMap = getAndMapIds(akm.parm, paneId);
            paneRouteData.actionParams = getMappedValues(parmKeyMap);

            paneRouteData.page = parseInt(getId(akm.page + paneId, $routeParams));
            paneRouteData.pageSize = parseInt(getId(akm.pageSize + paneId, $routeParams));

            paneRouteData.selectedItems = arrayFromMask(getId(akm.selected + paneId, $routeParams));

            paneRouteData.attachmentId = getId(akm.attachment + paneId, $routeParams);

            paneRouteData.validate($location.url());
        }

        function singlePane() {
            return $location.path().split("/").length <= 3;
        }

        function searchKeysForPane(search: any, paneId: number, raw: string[]) {
            const ids = _.map(raw, s => s + paneId);
            return _.filter(_.keys(search), k => _.some(ids, id => k.indexOf(id) === 0));
        }

        function allSearchKeysForPane(search: any, paneId: number) {
            const raw = _.values(akm) as string[];
            return searchKeysForPane(search, paneId, raw);
        }

        function clearPane(search: any, paneId: number) {           
            const toClear = allSearchKeysForPane(search, paneId);
             // always add reload flag 
            toClear.push(akm.reload);
            return _.omit(search, toClear);
        }

        function clearSearchKeys(search: any, paneId: number, keys: string[]) {
            const toClear = searchKeysForPane(search, paneId, keys);
            return _.omit(search, toClear);
        }

        function setupPaneNumberAndTypes(pane: number, newPaneType: string, newMode?: ApplicationMode) {

            const path = $location.path();
            const segments = path.split("/");
            let [, mode, pane1Type, pane2Type] = segments;
            let changeMode = false;
            let mayReplace = true;

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
                mayReplace = false;
                $location.path(newPath);
            }

            // changing item on pane 2
            // either single pane so need to add new pane of appropriate type
            // or double pane with second pane of wrong type. 
            if (pane === 2 && (singlePane() || pane2Type !== newPaneType)) {
                const newPath = `/${mode}/${pane1Type}/${newPaneType}`;
                changeMode = false;
                mayReplace = false;
                $location.path(newPath);
            }

            if (changeMode) {
                const newPath = `/${mode}/${pane1Type}/${pane2Type}`;
                $location.path(newPath);
                mayReplace = false;
            }

            return mayReplace;
        }

        function capturePane(paneId: number) {
            const search = getSearch();
            const toCapture = allSearchKeysForPane(search, paneId);

            return _.pick(search, toCapture);
        }

        function getOidFromHref(href: string) {
            const oid = ObjectIdWrapper.fromHref(href);
            return oid.getKey();
        }

        function getPidFromHref(href: string) {
            return propertyIdFromUrl(href);
        }

        function setValue(paneId: number, search: any, p: { id: () => string }, pv: Value, valueType: string) {
            setId(`${valueType}${paneId}_${p.id()}`, pv.toJsonString(), search);
        }

        function setParameter(paneId: number, search: any, p: Parameter, pv: Value) {
            setValue(paneId, search, p, pv, akm.parm);
        }


        enum Transition {
            Null,
            ToHome,
            ToMenu,
            ToDialog,
            FromDialog,
            FromDialogKeepHistory,
            ToObjectView,
            ToList,
            LeaveEdit,
            Page,
            ToTransient,
            ToRecent,
            ToAttachment,
            ToMultiLineDialog
        }

        function getId(key: string, search: any) {
            return Decompress(search[key]);
        }

        function setId(key: string, id: string, search: any) {
            search[key] = Compress(id);
        }

        function clearId(key: string, search: any) {
            delete search[key];
        }

        function validKeysForHome() {
            return [akm.menu, akm.dialog, akm.reload];
        }

        function validKeysForObject() {
            return [akm.object, akm.interactionMode, akm.reload, akm.actions, akm.dialog, akm.collection, akm.prop];
        }

        function validKeysForMultiLineDialog() {
            return [akm.object,  akm.dialog,  akm.menu];
        }

        function validKeysForList() {
            return [akm.reload, akm.actions, akm.dialog, akm.menu, akm.action, akm.page, akm.pageSize, akm.selected, akm.collection, akm.parm, akm.object];
        }

        function validKeys(path: string) {

            switch (path) {
                case homePath:
                    return validKeysForHome();
                case objectPath:
                    return validKeysForObject();
                case listPath:
                    return validKeysForList();
                case multiLineDialogPath:
                    return validKeysForMultiLineDialog();
            }

            return [];
        }

    
        function clearInvalidParmsFromSearch(paneId: number, search: any, path: string) {
            if (path) {
                const vks = validKeys(path);
                const ivks = _.without(_.values(akm), ...vks) as string[];
                return clearSearchKeys(search, paneId, ivks);
            }
            return search;
        }

        function handleTransition(paneId: number, search: any, transition: Transition) {

            let replace = true;

            switch (transition) {
                case (Transition.ToHome):
                    replace = setupPaneNumberAndTypes(paneId, homePath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToMenu):
                    search = clearPane(search, paneId);
                    break;
                case (Transition.FromDialog):
                    replace = true;
                    break;
                case (Transition.ToDialog):
                case (Transition.FromDialogKeepHistory):
                    replace = false;
                    break;
                case (Transition.ToObjectView):
                    replace = false;
                    setupPaneNumberAndTypes(paneId, objectPath);
                    search = clearPane(search, paneId);
                    setId(akm.interactionMode + paneId, InteractionMode[InteractionMode.View], search);
                    search = toggleReloadFlag(search);
                    break;
                case (Transition.ToList):
                    replace = setupPaneNumberAndTypes(paneId, listPath);
                    clearId(akm.menu + paneId, search);
                    clearId(akm.object + paneId, search);
                    clearId(akm.dialog + paneId, search);
                    break;
                case (Transition.LeaveEdit):
                    search = clearSearchKeys(search, paneId, [akm.prop]);
                    break;
                case (Transition.Page):
                    replace = false;
                    break;
                case (Transition.ToTransient):
                    replace = false;
                    break;
                case (Transition.ToRecent):
                    replace = setupPaneNumberAndTypes(paneId, recentPath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToAttachment):
                    replace = setupPaneNumberAndTypes(paneId, attachmentPath);
                    search = clearPane(search, paneId);
                    break;
                case (Transition.ToMultiLineDialog):
                    replace = setupPaneNumberAndTypes(paneId, multiLineDialogPath);
                    break;
                default:
                    // null transition 
                    break;
            }

            if (replace) {
                $location.replace();
            }

            const path = $location.path();
            const segments = path.split("/");
            const [, , pane1Type, pane2Type] = segments;

            search = clearInvalidParmsFromSearch(1, search, pane1Type);
            search = clearInvalidParmsFromSearch(2, search, pane2Type);

            return search;
        }

        function executeTransition(newValues: _.Dictionary<string>, paneId: number, transition: Transition, condition: (search: any) => boolean) {
            currentPaneId = paneId;
            let search = getSearch();
            if (condition(search)) {
                search = handleTransition(paneId, search, transition);

                _.forEach(newValues, (v, k) => {
                    if (v)
                        setId(k, v, search);
                    else
                        clearId(k, search);
                }
                );
                setNewSearch(search);
            }
        }

        helper.setHome = (paneId = 1) => {
            executeTransition({}, paneId, Transition.ToHome, () => true);
        };

        helper.setRecent = (paneId = 1) => {
            executeTransition({}, paneId, Transition.ToRecent, () => true);
        };

        helper.setMenu = (menuId: string, paneId = 1) => {
            const key = `${akm.menu}${paneId}`;
            const newValues = _.zipObject([key], [menuId]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToMenu, search => getId(key, search) !== menuId);
        };

        helper.setDialog = (dialogId: string, paneId = 1) => {
            const key = `${akm.dialog}${paneId}`;
            const newValues = _.zipObject([key], [dialogId]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToDialog, search => getId(key, search) !== dialogId);
        };

        helper.setMultiLineDialog = (dialogId: string, paneId = 1) => {
            helper.pushUrlState();
            const key = `${akm.dialog}${paneId}`;
            const newValues = _.zipObject([key], [dialogId]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToMultiLineDialog, search => getId(key, search) !== dialogId);
        };

        helper.setDialogOrMultiLineDialog = (actionRep : Models.ActionMember | Models.ActionRepresentation, paneId = 1) => {
            if (actionRep.extensions().multipleLines()) {
                helper.setMultiLineDialog(actionRep.actionId(), paneId);
            } else {
                helper.setDialog(actionRep.actionId(), paneId);
            }
        };



        function closeOrCancelDialog(paneId: number, transition: Transition) {
            const key = `${akm.dialog}${paneId}`;
            const newValues = _.zipObject([key], [null]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, transition, () => true);
        }


        helper.closeDialogKeepHistory = (paneId = 1) => {
            closeOrCancelDialog(paneId, Transition.FromDialogKeepHistory);
        };

        helper.closeDialogReplaceHistory = (paneId = 1) => {
            closeOrCancelDialog(paneId, Transition.FromDialog);
        };

        helper.setObject = (resultObject: DomainObjectRepresentation, paneId = 1) => {
            const oid = resultObject.id();
            const key = `${akm.object}${paneId}`;
            const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
        };

        helper.setList = (actionMember: IAction, parms: _.Dictionary<Value>, fromPaneId = 1, toPaneId = 1) => {
            let newValues = {} as _.Dictionary<string>;
            const parent = actionMember.parent;

            if (parent instanceof DomainObjectRepresentation) {
                newValues[`${akm.object}${toPaneId}`] = parent.id();
            }

            if (parent instanceof MenuRepresentation) {
                newValues[`${akm.menu}${toPaneId}`] = parent.menuId();
            }

            newValues[`${akm.action}${toPaneId}`] = actionMember.actionId();
            newValues[`${akm.page}${toPaneId}`] = "1";
            newValues[`${akm.pageSize}${toPaneId}`] = defaultPageSize.toString();
            newValues[`${akm.selected}${toPaneId}`] = "0";

            const newState = actionMember.extensions().renderEagerly() ?
                CollectionViewState[CollectionViewState.Table] :
                CollectionViewState[CollectionViewState.List];

            newValues[`${akm.collection}${toPaneId}`] = newState;

            // This will also swap the panes of the field values if we are 
            // right clicking into the other pane.

            _.forEach(parms, (p, id) => setId(`${akm.parm}${toPaneId}_${id}`, p.toJsonString(), newValues));


            executeTransition(newValues, toPaneId, Transition.ToList, () => true);
        };

        helper.setProperty = (propertyMember: PropertyMember, paneId = 1) => {
            const href = propertyMember.value().link().href();
            const oid = getOidFromHref(href);
            const key = `${akm.object}${paneId}`;
            const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
        };

        helper.setItem = (link: Link, paneId = 1) => {
            const href = link.href();
            const oid = getOidFromHref(href);
            const key = `${akm.object}${paneId}`;
            const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
        };

        helper.setAttachment = (attachmentlink: Link, paneId = 1) => {
            const href = attachmentlink.href();
            const okey = `${akm.object}${paneId}`;
            const akey = `${akm.attachment}${paneId}`;
            const oid = getOidFromHref(href);
            const pid = getPidFromHref(href);


            const newValues = _.zipObject([okey, akey], [oid, pid]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.ToAttachment, () => true);
        };

        helper.toggleObjectMenu = (paneId = 1) => {
            const key = akm.actions + paneId;
            const actionsId = getSearch()[key] ? null : "open";
            const newValues = _.zipObject([key], [actionsId]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.Null, () => true);
        };

        function checkAndSetValue(paneId: number, check: (search: any) => boolean, set: (search: any) => void) {
            currentPaneId = paneId;
            const search = getSearch();

            // only add field if matching dialog or dialog (to catch case when swapping panes) 
            if (check(search)) {
                set(search);
                setNewSearch(search);
                $location.replace();
            }
        }

        helper.setParameterValue = (actionId: string, p: Parameter, pv: Value, paneId = 1) =>
            checkAndSetValue(paneId,
                search => getId(`${akm.action}${paneId}`, search) === actionId,
                search => setParameter(paneId, search, p, pv));


        helper.setCollectionMemberState = (collectionMemberId: string, state: CollectionViewState, paneId = 1) => {
            const key = `${akm.collection}${paneId}_${collectionMemberId}`;
            const newValues = _.zipObject([key], [CollectionViewState[state]]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.Null, () => true);
        };

        helper.setListState = (state: CollectionViewState, paneId = 1) => {
            const key = `${akm.collection}${paneId}`;
            const newValues = _.zipObject([key], [CollectionViewState[state]]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.Null, () => true);
        };

        helper.setInteractionMode = (newMode: InteractionMode, paneId = 1) => {
            const key = `${akm.interactionMode}${paneId}`;
            const currentMode = getInteractionMode(getId(key, $routeParams));
            let transition: Transition;

            if (currentMode === InteractionMode.Edit && newMode !== InteractionMode.Edit) {
                transition = Transition.LeaveEdit;
            }
            else if (newMode === InteractionMode.Transient) {
                transition = Transition.ToTransient;
            } else {
                transition = Transition.Null;
            }

            const newValues = _.zipObject([key], [InteractionMode[newMode]]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, transition, () => true);
        };


        helper.setListItem = (item: number, isSelected: boolean, paneId = 1) => {

            const key = `${akm.selected}${paneId}`;
            const currentSelected = getSearch()[key];
            const selectedArray: boolean[] = arrayFromMask(currentSelected);
            selectedArray[item] = isSelected;
            const currentSelectedAsString = (createMask(selectedArray)).toString();
            const newValues = _.zipObject([key], [currentSelectedAsString]) as _.Dictionary<string>;
            executeTransition(newValues, paneId, Transition.Null, () => true);
        };
        helper.setListPaging = (newPage: number, newPageSize: number, state: CollectionViewState, paneId = 1) => {
            const pageValues = {} as _.Dictionary<string>;

            pageValues[`${akm.page}${paneId}`] = newPage.toString();
            pageValues[`${akm.pageSize}${paneId}`] = newPageSize.toString();
            pageValues[`${akm.collection}${paneId}`] = CollectionViewState[state];
            pageValues[`${akm.selected}${paneId}`] = "0"; // clear selection 

            executeTransition(pageValues, paneId, Transition.Page, () => true);
        };

        helper.setError = (errorCategory: ErrorCategory, ec?: ClientErrorCode | HttpStatusCode) => {
            const path = $location.path();
            const segments = path.split("/");
            const mode = segments[1];
            const newPath = `/${mode}/error`;

            const search = {};
            // always on pane 1
            (<any>search)[akm.errorCat + 1] = ErrorCategory[errorCategory];

            $location.path(newPath);
            setNewSearch(search);

            if (errorCategory === ErrorCategory.HttpClientError && ec === HttpStatusCode.PreconditionFailed) {
                // on concurrency fail replace url so we can't just go back
                $location.replace();
            }
        };


        helper.getRouteData = () => {
            const routeData = new RouteData();

            setPaneRouteData(routeData.pane1, 1);
            setPaneRouteData(routeData.pane2, 2);

            return routeData;
        };

        helper.pushUrlState = (paneId = 1) => {
            capturedPanes[paneId] = helper.getUrlState(paneId);
        };
        helper.getUrlState = (paneId = 1) => {
            currentPaneId = paneId;

            const path = $location.path();
            const segments = path.split("/");

            const paneType = segments[paneId + 1] || homePath;
            let paneSearch = capturePane(paneId);

            // clear any dialogs so we don't return  to a dialog

            paneSearch = _.omit(paneSearch, `${akm.dialog}${paneId}`);

            return { paneType: paneType, search: paneSearch };
        };

        helper.getListCacheIndex = (paneId: number, newPage: number, newPageSize: number, format?: CollectionViewState) => {
            const search = getSearch();

            const s1 = getId(`${akm.menu}${paneId}`, search) || "";
            const s2 = getId(`${akm.object}${paneId}`, search) || "";
            const s3 = getId(`${akm.action}${paneId}`, search) || "";

            const parms = <_.Dictionary<string>>_.pickBy(search, (v, k) => k.indexOf(akm.parm + paneId) === 0);
            const mappedParms = _.mapValues(parms, v => decodeURIComponent(Decompress(v)));

            const s4 = _.reduce(mappedParms, (r, n, k) => r + (k + "=" + n + keySeparator), "");

            const s5 = `${newPage}`;
            const s6 = `${newPageSize}`;

            const s7 = format ? `${format}` : "";

            const ss = [s1, s2, s3, s4, s5, s6, s7] as string[];

            return _.reduce(ss, (r, n) => r + keySeparator + n, "");
        };

        helper.popUrlState = (paneId = 1) => {
            currentPaneId = paneId;

            const capturedPane = capturedPanes[paneId];
            let mayReplace = true;

            if (capturedPane) {
                capturedPanes[paneId] = null;
                let search = clearPane(getSearch(), paneId);
                search = _.merge(search, capturedPane.search);
                mayReplace = setupPaneNumberAndTypes(paneId, capturedPane.paneType);
                setNewSearch(search);
            } else {
                // probably reloaded page so no state to pop. 
                // just go home 
                helper.setHome(paneId);
            }

            if (mayReplace) {
                $location.replace();
            }
        };

        helper.clearUrlState = (paneId: number) => {
            currentPaneId = paneId;
            capturedPanes[paneId] = null;
        };

        function swapSearchIds(search: any) {
            return _.mapKeys(search,
                (v: any, k: string) => k.replace(/(\D+)(\d{1})(\w*)/, (match, p1, p2, p3) => `${p1}${p2 === "1" ? "2" : "1"}${p3}`));
        }


        helper.swapPanes = () => {
            const path = $location.path();
            const segments = path.split("/");
            const [, mode, oldPane1, oldPane2 = homePath] = segments;
            const newPath = `/${mode}/${oldPane2}/${oldPane1}`;
            const search = swapSearchIds(getSearch());
            currentPaneId = getOtherPane(currentPaneId);

            $location.path(newPath).search(search);
        };

        helper.cicero = () => {
            const newPath = `/${ciceroPath}/${$location.path().split("/")[2]}`;
            $location.path(newPath);
        };

        helper.applicationProperties = () => {
            $location.path(`/${geminiPath}/${applicationPropertiesPath}`);
        };

        helper.currentpane = () => currentPaneId;

        helper.singlePane = (paneId = 1) => {
            currentPaneId = 1;

            if (!singlePane()) {

                const paneToKeepId = paneId;
                const paneToRemoveId = getOtherPane(paneToKeepId);

                const path = $location.path();
                const segments = path.split("/");
                const mode = segments[1];
                const paneToKeep = segments[paneToKeepId + 1];
                const newPath = `/${mode}/${paneToKeep}`;

                let search = getSearch();

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
        };

        helper.reload = () => {
            $window.location.reload(true);
        }

        function isLocation(paneId: number, location: string) {
            const path = $location.path();
            const segments = path.split("/");
            return segments[paneId + 1] === location; // e.g. segments 0=~/1=cicero/2=home/3=home
        };

        helper.isHome = (paneId = 1) => isLocation(paneId, homePath);
        helper.isObject = (paneId = 1) => isLocation(paneId, objectPath);
        helper.isList = (paneId = 1) => isLocation(paneId, listPath);
        helper.isError = (paneId = 1) => isLocation(paneId, errorPath);
        helper.isRecent = (paneId = 1) => isLocation(paneId, recentPath);
        helper.isAttachment = (paneId = 1) => isLocation(paneId, attachmentPath);
        helper.isApplicationProperties = (paneId = 1) => isLocation(paneId, applicationPropertiesPath);

        function toggleReloadFlag(search: any) {
            const currentFlag = search[akm.reload];
            const newFlag = currentFlag ? 0 : 1;
            search[akm.reload] = newFlag;
            return search;
        }

        helper.triggerPageReloadByFlippingReloadFlagInUrl = () => {
            const search = getSearch();
            setNewSearch(toggleReloadFlag(search));
            $location.replace();
        }
    });
}