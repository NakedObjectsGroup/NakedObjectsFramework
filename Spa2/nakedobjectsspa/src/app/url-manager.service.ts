import { RouteData, PaneRouteData, InteractionMode, CollectionViewState, ApplicationMode, ViewType } from './route-data';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Router, UrlSegment } from '@angular/router';
import { Location } from '@angular/common';
import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';
import * as _ from 'lodash';
import * as Models from "./models";
import * as Constants from './constants';

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
    ToObjectWithMode,
    ToMultiLineDialog
};

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

interface ITransitionResult {
    path: string,
    search: any;
    replace: boolean;
}


@Injectable()
export class UrlManagerService {

    constructor(
        private readonly router: Router,
        private readonly location: Location,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService
    ) {
        this.shortCutMarker = configService.config.shortCutMarker;
        this.urlShortCuts = configService.config.urlShortCuts;
        this.keySeparator = this.configService.config.keySeparator;
    }

    // just for tidyness
    private readonly shortCutMarker: string;
    private readonly urlShortCuts: string[];
    private readonly keySeparator: string;

    private capturedPanes = [] as ({ paneType: Constants.PathSegment; search: Object } | null)[];

    private currentPaneId = 1;

    private createSubMask(arr: boolean[]) {
        let nMask = 0;
        let nFlag = 0;

        if (arr.length > 31) {
            const msg = `UrlManagerService:createSubMask Out of range ${arr.length}`;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }

        const nLen = arr.length;
        for (nFlag; nFlag < nLen; nMask |= (<any>arr)[nFlag] << nFlag++);
        return nMask;
    }


    // convert from array of bools to mask string
    private createArrays(arr: boolean[], arrays?: boolean[][]): boolean[][] {

        arrays = arrays || [];

        if (arr.length > 31) {
            arrays.push(arr.slice(0, 31));
            return this.createArrays(arr.slice(31), arrays);
        }

        arrays.push(arr);
        return arrays;
    }


    private createMask(arr: boolean[]) {
        // split into smaller arrays if necessary 

        const arrays = this.createArrays(arr);
        const masks = _.map(arrays, a => this.createSubMask(a).toString());

        return _.reduce(masks, (res, val) => res + "-" + val);
    }

    // convert from mask string to array of bools
    private arrayFromSubMask(sMask: string) {
        const nMask = parseInt(sMask);
        // nMask must be between 0 and 2147483647 - to keep simple we stick to 31 bits 
        if (nMask > 0x7fffffff || nMask < -0x80000000) {
            const msg = `UrlManagerService:arrayFromSubMask Out of range ${nMask}`;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }
        const aFromMask = [] as boolean[];
        let len = 31; // make array always 31 bit long as we may concat another on end
        for (let nShifted = nMask; len > 0; aFromMask.push(Boolean(nShifted & 1)), nShifted >>>= 1, --len);
        return aFromMask;
    }

    private arrayFromMask(sMask: string) {
        sMask = sMask || "0";
        const sMasks = sMask.split("-");
        const maskArrays = _.map(sMasks, s => this.arrayFromSubMask(s));
        return _.reduce(maskArrays, (res, val) => res.concat(val), [] as boolean[]);
    }

    private getSearch() {

        const url = this.router.url;
        return this.router.parseUrl(url).queryParams;
    }

    private getPath() {

        const url = this.router.url;
        let end = url.indexOf(";");
        end = end === -1 ? url.indexOf("?") : end;
        const path = url.substring(0, end > 0 ? end : url.length);
        return path;
    }

    private setNewSearch(result: ITransitionResult) {

        const tree = this.router.createUrlTree([result.path], { queryParams: result.search });

        this.router.navigateByUrl(tree);
        if (result.replace) {
            const u = this.router.serializeUrl(tree);
            this.location.replaceState(u);
        }
    }

    private getIds(typeOfId: string, paneId: number) {
        return <_.Dictionary<string>>_.pickBy(this.getSearch(), (v, k) => !!k && k.indexOf(typeOfId + paneId) === 0);
    }

    private mapIds(ids: _.Dictionary<string>): _.Dictionary<string> {
        return _.mapKeys(ids, (v: any, k: string) => k.substr(k.indexOf("_") + 1));
    }

    private getAndMapIds(typeOfId: string, paneId: number) {
        const ids = this.getIds(typeOfId, paneId);
        return this.mapIds(ids);
    }

    private getMappedValues(mappedIds: _.Dictionary<string>) {
        return _.mapValues(mappedIds, v => Models.Value.fromJsonString(v, this.shortCutMarker, this.urlShortCuts));
    }

    private getInteractionMode(rawInteractionMode: string): InteractionMode {
        return rawInteractionMode ? (<any>InteractionMode)[rawInteractionMode] : InteractionMode.View;
    }

    private getPaneParams(params : _.Dictionary<string>, paneId : number) : _.Dictionary<string>{
        const paneIds = _.filter(_.keys(params), k => k.indexOf(paneId.toString()) >= 0);
        return _.pick(params, paneIds) as _.Dictionary<string>;
    }

    private setPaneRouteDataFromParms(paneRouteData: PaneRouteData, paneId: number, routeParams: { [key: string]: string }) {
        paneRouteData.rawParms =  this.getPaneParams(routeParams, paneId);
        paneRouteData.menuId = this.getId(akm.menu + paneId, routeParams);
        paneRouteData.actionId = this.getId(akm.action + paneId, routeParams);
        paneRouteData.dialogId = this.getId(akm.dialog + paneId, routeParams);

        const rawErrorCategory = this.getId(akm.errorCat + paneId, routeParams);
        paneRouteData.errorCategory = rawErrorCategory ? (<any>Models.ErrorCategory)[rawErrorCategory] : null;

        paneRouteData.objectId = this.getId(akm.object + paneId, routeParams);
        paneRouteData.actionsOpen = this.getId(akm.actions + paneId, routeParams);

        const rawCollectionState = this.getId(akm.collection + paneId, routeParams);
        paneRouteData.state = rawCollectionState ? (<any>CollectionViewState)[rawCollectionState] : CollectionViewState.List;

        const rawInteractionMode = this.getId(akm.interactionMode + paneId, routeParams);
        paneRouteData.interactionMode = this.getInteractionMode(rawInteractionMode);

        const collKeyMap = this.getAndMapIds(akm.collection, paneId);
        paneRouteData.collections = _.mapValues(collKeyMap, v => (<any>CollectionViewState)[v]);

        const collSelectedKeyMap = this.getAndMapIds(akm.selected, paneId);
        paneRouteData.selectedCollectionItems = _.mapValues(collSelectedKeyMap, v => this.arrayFromMask(v));

        const parmKeyMap = this.getAndMapIds(akm.parm, paneId);
        paneRouteData.actionParams = this.getMappedValues(parmKeyMap);

        paneRouteData.page = parseInt(this.getId(akm.page + paneId, routeParams));
        paneRouteData.pageSize = parseInt(this.getId(akm.pageSize + paneId, routeParams));

        paneRouteData.attachmentId = this.getId(akm.attachment + paneId, routeParams);
    }


    private setPaneRouteData(paneRouteData: PaneRouteData, paneId: number) {
        const routeParams = this.getSearch();
        this.setPaneRouteDataFromParms(paneRouteData, paneId, routeParams);

        paneRouteData.validate(this.location.path());
    }

    private isSinglePane() {
        return this.getPath().split("/").length <= 3;
    }

    private searchKeysForPane(search: any, paneId: number, raw: string[]) {
        const ids = _.map(raw, s => s + paneId);
        return _.filter(_.keys(search), k => _.some(ids, id => k.indexOf(id) === 0));
    }

    private allSearchKeysForPane(search: any, paneId: number) {
        const raw = _.values(akm) as string[];
        return this.searchKeysForPane(search, paneId, raw);
    }

    private clearPane(search: any, paneId: number) {
        const toClear = this.allSearchKeysForPane(search, paneId);
        return _.omit(search, toClear) as _.Dictionary<string>;
    }

    private clearSearchKeys(search: any, paneId: number, keys: string[]) {
        const toClear = this.searchKeysForPane(search, paneId, keys);
        return _.omit(search, toClear);
    }

    private paneIsAlwaysSingle(paneType: string) {
        return paneType === Constants.multiLineDialogPath;
    }

    private setupPaneNumberAndTypes(pane: number, newPaneType: Constants.PathSegment, newMode?: ApplicationMode): { path: string, replace: boolean } {

        const path = this.getPath();
        const segments = path.split("/");
        let [, mode, pane1Type, pane2Type] = segments;
        let changeMode = false;
        let mayReplace = true;
        let newPath = path;

        if (newMode) {
            const newModeString = newMode.toString().toLowerCase();
            changeMode = mode !== newModeString;
            mode = newModeString;
        }

        // changing item on pane 1
        // make sure pane is of correct type
        if (pane === 1 && pane1Type !== newPaneType) {
            const single = this.isSinglePane() || this.paneIsAlwaysSingle(newPaneType);
            newPath = `/${mode}/${newPaneType}${single ? "" : `/${pane2Type}`}`;
            changeMode = false;
            mayReplace = false;
        }

        // changing item on pane 2
        // either single pane so need to add new pane of appropriate type
        // or double pane with second pane of wrong type. 
        if (pane === 2 && (this.isSinglePane() || pane2Type !== newPaneType)) {
            newPath = `/${mode}/${pane1Type}/${newPaneType}`;
            changeMode = false;
            mayReplace = false;
        }

        if (changeMode) {
            newPath = `/${mode}/${pane1Type}/${pane2Type}`;
            mayReplace = false;
        }

        return { path: newPath, replace: mayReplace };
    }

    private capturePane(paneId: number) {
        const search = this.getSearch();
        const toCapture = this.allSearchKeysForPane(search, paneId);

        return _.pick(search, toCapture);
    }

    private getOidFromHref(href: string) {
        const oid = Models.ObjectIdWrapper.fromHref(href, this.keySeparator);
        return oid.getKey();
    }

    private getPidFromHref(href: string) {
        return Models.propertyIdFromUrl(href);
    }

    private setValue(paneId: number, search: any, p: { id: () => string }, pv: Models.Value, valueType: string) {
        this.setId(`${valueType}${paneId}_${p.id()}`, pv.toJsonString(this.shortCutMarker, this.urlShortCuts), search);
    }

    private setParameter(paneId: number, search: any, p: Models.Parameter, pv: Models.Value) {
        this.setValue(paneId, search, p, pv, akm.parm);
    }


    private getId(key: string, search: any) {
        return Models.decompress(search[key], this.shortCutMarker, this.urlShortCuts);
    }

    private setId(key: string, id: string, search: any) {
        search[key] = Models.compress(id, this.shortCutMarker, this.urlShortCuts);
    }

    private clearId(key: string, search: any) {
        delete search[key];
    }

    private validKeysForHome() {
        return [akm.menu, akm.dialog, akm.reload];
    }

    private validKeysForObject() {
        return [akm.object, akm.interactionMode, akm.reload, akm.actions, akm.dialog, akm.collection, akm.prop, akm.selected];
    }

    private validKeysForMultiLineDialog() {
        return [akm.object, akm.dialog, akm.menu];
    }

    private validKeysForList() {
        return [akm.reload, akm.actions, akm.dialog, akm.menu, akm.action, akm.page, akm.pageSize, akm.selected, akm.collection, akm.parm, akm.object];
    }

    private validKeysForAttachment() {
        return [akm.object, akm.attachment];
    }

    private validKeys(path: string) {

        switch (path) {
            case Constants.homePath:
                return this.validKeysForHome();
            case Constants.objectPath:
                return this.validKeysForObject();
            case Constants.listPath:
                return this.validKeysForList();
            case Constants.multiLineDialogPath:
                return this.validKeysForMultiLineDialog();
            case Constants.attachmentPath:
                return this.validKeysForAttachment();
        }

        return [];
    }

    private clearInvalidParmsFromSearch(paneId: number, search: any, path: string) {
        if (path) {
            const vks = this.validKeys(path);
            const ivks = _.without(_.values(akm), ...vks) as string[];
            return this.clearSearchKeys(search, paneId, ivks);
        }
        return search;
    }

    private handleTransition(paneId: number, search: any, transition: Transition): ITransitionResult {

        let replace = true;
        let path = this.getPath();

        switch (transition) {
            case (Transition.ToHome):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.homePath));
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToMenu):
                search = this.clearPane(search, paneId);
                break;
            case (Transition.FromDialog):
                replace = true;
                break;
            case (Transition.ToDialog):
            case (Transition.FromDialogKeepHistory):
                replace = false;
                break;
            case (Transition.ToObjectView):

                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.objectPath));
                replace = false;
                search = this.clearPane(search, paneId);
                this.setId(akm.interactionMode + paneId, InteractionMode[InteractionMode.View], search);
                break;
            case (Transition.ToObjectWithMode):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.objectPath));
                replace = false;
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToList):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.listPath));
                this.clearId(akm.menu + paneId, search);
                this.clearId(akm.object + paneId, search);
                this.clearId(akm.dialog + paneId, search);
                break;
            case (Transition.LeaveEdit):
                search = this.clearSearchKeys(search, paneId, [akm.prop]);
                break;
            case (Transition.Page):
                replace = false;
                break;
            case (Transition.ToTransient):
                replace = false;
                break;
            case (Transition.ToRecent):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.recentPath));
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToAttachment):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, Constants.attachmentPath));
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToMultiLineDialog):
                ({ path, replace } = this.setupPaneNumberAndTypes(1, Constants.multiLineDialogPath)); // always on 1
                if (paneId === 2) {
                    // came from 2 
                    search = this.swapSearchIds(search);
                }
                search = this.clearPane(search, 2); // always on pane 1
                break;
            default:
                // null transition 
                break;
        }

        const segments = path.split("/");
        const [, , pane1Type, pane2Type] = segments;

        search = this.clearInvalidParmsFromSearch(1, search, pane1Type);
        search = this.clearInvalidParmsFromSearch(2, search, pane2Type);

        return { path: path, search: search, replace: replace };
    }

    private executeTransition(newValues: _.Dictionary<string>, paneId: number, transition: Transition, condition: (search: any) => boolean) {
        this.currentPaneId = paneId;
        let search = this.getSearch();
        if (condition(search)) {
            const result = this.handleTransition(paneId, search, transition);
            ({ search } = result);
            _.forEach(newValues,
                (v, k) => {
                    // k should always be non null
                    if (v)
                        this.setId(k!, v, search);
                    else
                        this.clearId(k!, search);
                }
            );
            this.setNewSearch(result);
        }
    }

    setHome = (paneId = 1) => {
        this.executeTransition({}, paneId, Transition.ToHome, () => true);
    }

    setRecent = (paneId = 1) => {
        this.executeTransition({}, paneId, Transition.ToRecent, () => true);
    }

    setMenu = (menuId: string, paneId = 1) => {
        const key = `${akm.menu}${paneId}`;
        const newValues = _.zipObject([key], [menuId]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToMenu, search => this.getId(key, search) !== menuId);
    }

    setDialog = (dialogId: string, paneId = 1) => {
        const key = `${akm.dialog}${paneId}`;
        const newValues = _.zipObject([key], [dialogId]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToDialog, search => this.getId(key, search) !== dialogId);
    }


    setMultiLineDialog = (dialogId: string, paneId: number) => {
        this.pushUrlState();
        const key = `${akm.dialog}${1}`; // always on 1
        const newValues = _.zipObject([key], [dialogId]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToMultiLineDialog, search => this.getId(key, search) !== dialogId);
    }

    setDialogOrMultiLineDialog = (actionRep: Models.ActionMember | Models.ActionRepresentation, paneId = 1) => {
        if (actionRep.extensions().multipleLines()) {
            this.setMultiLineDialog(actionRep.actionId(), paneId);
        } else {
            this.setDialog(actionRep.actionId(), paneId);
        }
    }

    private closeOrCancelDialog(id: string, paneId: number, transition: Transition) {
        const key = `${akm.dialog}${paneId}`;
        const existingValue = this.getSearch()[key];

        if (existingValue === id) {
            const newValues = _.zipObject([key], [null]) as _.Dictionary<string>;
            this.executeTransition(newValues, paneId, transition, () => true);
        }
    }


    closeDialogKeepHistory = (id: string, paneId = 1) => {
        this.closeOrCancelDialog(id, paneId, Transition.FromDialogKeepHistory);
    }

    closeDialogReplaceHistory = (id: string, paneId = 1) => {
        this.closeOrCancelDialog(id, paneId, Transition.FromDialog);
    }

    setObject = (resultObject: Models.DomainObjectRepresentation, paneId = 1) => {
        const oid = resultObject.id(this.keySeparator);
        const key = `${akm.object}${paneId}`;
        const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setObjectWithMode = (resultObject: Models.DomainObjectRepresentation, newMode: InteractionMode, paneId = 1) => {
        const oid = resultObject.id(this.keySeparator);
        const okey = `${akm.object}${paneId}`;
        const mkey = `${akm.interactionMode}${paneId}`;
        const newValues = _.zipObject([okey, mkey], [oid, InteractionMode[newMode]]) as _.Dictionary<string>;

        this.executeTransition(newValues, paneId, Transition.ToObjectWithMode, () => true);
    }

    setList = (actionMember: Models.IInvokableAction, parms: _.Dictionary<Models.Value>, fromPaneId = 1, toPaneId = 1) => {
        const newValues = {} as _.Dictionary<string>;
        const parent = actionMember.parent;

        if (parent instanceof Models.DomainObjectRepresentation) {
            newValues[`${akm.object}${toPaneId}`] = parent.id(this.keySeparator);
        }

        if (parent instanceof Models.MenuRepresentation) {
            newValues[`${akm.menu}${toPaneId}`] = parent.menuId();
        }

        newValues[`${akm.action}${toPaneId}`] = actionMember.actionId();
        newValues[`${akm.page}${toPaneId}`] = "1";
        newValues[`${akm.pageSize}${toPaneId}`] = this.configService.config.defaultPageSize.toString();
        newValues[`${akm.selected}${toPaneId}_`] = "0";

        const newState = actionMember.extensions().renderEagerly() ? CollectionViewState[CollectionViewState.Table] : CollectionViewState[CollectionViewState.List];

        newValues[`${akm.collection}${toPaneId}`] = newState;

        _.forEach(parms, (p, id) => this.setId(`${akm.parm}${toPaneId}_${id}`, p.toJsonString(this.shortCutMarker, this.urlShortCuts), newValues));

        this.executeTransition(newValues, toPaneId, Transition.ToList, () => true);
        return newValues;
    }

    setProperty = (href: string, paneId = 1) => {
        const oid = this.getOidFromHref(href);
        const key = `${akm.object}${paneId}`;
        const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setItem = (link: Models.Link, paneId = 1) => {
        const href = link.href();
        const oid = this.getOidFromHref(href);
        const key = `${akm.object}${paneId}`;
        const newValues = _.zipObject([key], [oid]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setAttachment = (attachmentlink: Models.Link, paneId = 1) => {
        const href = attachmentlink.href();
        const okey = `${akm.object}${paneId}`;
        const akey = `${akm.attachment}${paneId}`;
        const oid = this.getOidFromHref(href);
        const pid = this.getPidFromHref(href);


        const newValues = _.zipObject([okey, akey], [oid, pid]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToAttachment, () => true);
    }

    toggleObjectMenu = (paneId = 1) => {
        const key = akm.actions + paneId;
        const actionsId = this.getSearch()[key] ? null : "open";
        const newValues = _.zipObject([key], [actionsId]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    private checkAndSetValue(paneId: number, check: (search: any) => boolean, set: (search: any) => void) {
        this.currentPaneId = paneId;
        const search = this.getSearch();

        // only add field if matching dialog or dialog (to catch case when swapping panes) 
        if (check(search)) {
            set(search);
            const result = { path: this.getPath(), search: search, replace: false };
            this.setNewSearch(result);
        }
    }

    setParameterValue = (actionId: string, p: Models.Parameter, pv: Models.Value, paneId = 1) =>
        this.checkAndSetValue(paneId,
            search => this.getId(`${akm.action}${paneId}`, search) === actionId,
            search => this.setParameter(paneId, search, p, pv));


    setCollectionMemberState = (collectionMemberId: string, state: CollectionViewState, paneId = 1) => {
        const key = `${akm.collection}${paneId}_${collectionMemberId}`;
        const newValues = _.zipObject([key], [CollectionViewState[state]]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setListState = (state: CollectionViewState, paneId = 1) => {
        const key = `${akm.collection}${paneId}`;
        const newValues = _.zipObject([key], [CollectionViewState[state]]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setInteractionMode = (newMode: InteractionMode, paneId = 1) => {
        const key = `${akm.interactionMode}${paneId}`;
        const routeParams = this.getSearch();
        const currentMode = this.getInteractionMode(this.getId(key, routeParams));
        let transition: Transition;

        if (currentMode === InteractionMode.Edit && newMode !== InteractionMode.Edit) {
            transition = Transition.LeaveEdit;
        } else if (newMode === InteractionMode.Transient) {
            transition = Transition.ToTransient;
        } else {
            transition = Transition.Null;
        }

        const newValues = _.zipObject([key], [InteractionMode[newMode]]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, transition, () => true);
    }

    setItemSelected = (item: number, isSelected: boolean, collectionId: string, paneId = 1) => {

        const key = `${akm.selected}${paneId}_${collectionId}`;
        const currentSelected = this.getSearch()[key];
        const selectedArray = this.arrayFromMask(currentSelected);
        selectedArray[item] = isSelected;
        const currentSelectedAsString = (this.createMask(selectedArray)).toString();
        const newValues = _.zipObject([key], [currentSelectedAsString]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setAllItemsSelected = (isSelected: boolean, collectionId: string, paneId = 1) => {

        const key = `${akm.selected}${paneId}_${collectionId}`;
        const currentSelected = this.getSearch()[key];
        const selectedArray = this.arrayFromMask(currentSelected);
        _.fill(selectedArray, isSelected);
        const currentSelectedAsString = (this.createMask(selectedArray)).toString();
        const newValues = _.zipObject([key], [currentSelectedAsString]) as _.Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setListPaging = (newPage: number, newPageSize: number, state: CollectionViewState, paneId = 1) => {
        const pageValues = {} as _.Dictionary<string>;

        pageValues[`${akm.page}${paneId}`] = newPage.toString();
        pageValues[`${akm.pageSize}${paneId}`] = newPageSize.toString();
        pageValues[`${akm.collection}${paneId}`] = CollectionViewState[state];
        pageValues[`${akm.selected}${paneId}_`] = "0"; // clear selection 

        this.executeTransition(pageValues, paneId, Transition.Page, () => true);
    }

    setError = (errorCategory: Models.ErrorCategory, ec?: Models.ClientErrorCode | Models.HttpStatusCode) => {
        const path = this.getPath();
        const segments = path.split("/");
        const mode = segments[1];
        const newPath = `/${mode}/error`;

        const search = {};
        // always on pane 1
        (<any>search)[akm.errorCat + 1] = Models.ErrorCategory[errorCategory];

        const result = { path: newPath, search: search, replace: false };

        if (errorCategory === Models.ErrorCategory.HttpClientError && ec === Models.HttpStatusCode.PreconditionFailed) {
            result.replace = true;
        }
        this.setNewSearch(result);
    }

    getRouteData = () => {
        const routeData = new RouteData(this.configService, this.loggerService);

        this.setPaneRouteData(routeData.pane1, 1);
        this.setPaneRouteData(routeData.pane2, 2);

        return routeData;
    }

    getViewType(view: string) {
        switch (view) {
            case Constants.homePath: return ViewType.Home;
            case Constants.objectPath: return ViewType.Object;
            case Constants.listPath: return ViewType.List;
            case Constants.errorPath: return ViewType.Error;
            case Constants.recentPath: return ViewType.Recent;
            case Constants.attachmentPath: return ViewType.Attachment;
            case Constants.applicationPropertiesPath: return ViewType.ApplicationProperties;
            case Constants.multiLineDialogPath: return ViewType.MultiLineDialog;
        }
        this.loggerService.throw(`UrlManagerService:getViewType ${view} is not a valid ViewType`);
    }

    getPaneRouteDataObservable = (paneId: number) => {

        return this.router.routerState.root.queryParams.map((ps: { [key: string]: string }) => {
            const routeData = new RouteData(this.configService, this.loggerService);
            const paneRouteData = routeData.pane(paneId) !;
            this.setPaneRouteDataFromParms(paneRouteData, paneId, ps);
            paneRouteData.location = this.getViewType(this.getLocation(paneId)) !;
            return paneRouteData;
        }) as Observable<PaneRouteData>;
    }

    pushUrlState = (paneId = 1) => {
        this.capturedPanes[paneId] = this.getUrlState(paneId);
    }

    getUrlState = (paneId = 1) => {
        this.currentPaneId = paneId;

        const path = this.getPath();
        const segments = path.split("/");

        const paneType = <Constants.PathSegment>segments[paneId + 1] || Constants.homePath;
        let paneSearch = this.capturePane(paneId);

        // clear any dialogs so we don't return  to a dialog

        paneSearch = _.omit(paneSearch, `${akm.dialog}${paneId}`);

        return { paneType: paneType, search: paneSearch };
    }

    getListCacheIndexFromSearch = (search: _.Dictionary<string>, paneId: number, newPage: number, newPageSize: number, format?: CollectionViewState) => {


        const s1 = this.getId(`${akm.menu}${paneId}`, search) || "";
        const s2 = this.getId(`${akm.object}${paneId}`, search) || "";
        const s3 = this.getId(`${akm.action}${paneId}`, search) || "";

        const parms = <_.Dictionary<string>>_.pickBy(search, (v, k) => !!k && k.indexOf(akm.parm + paneId) === 0);
        const mappedParms = _.mapValues(parms, v => decodeURIComponent(Models.decompress(v, this.shortCutMarker, this.urlShortCuts)));

        const s4 = _.reduce(mappedParms, (r, n, k) => r + (k + "=" + n + this.keySeparator), "");

        const s5 = `${newPage}`;
        const s6 = `${newPageSize}`;

        const s7 = format ? `${format}` : "";

        const ss = [s1, s2, s3, s4, s5, s6, s7] as string[];

        return _.reduce(ss, (r, n) => r + this.keySeparator + n, "");
    }

    getListCacheIndex = (paneId: number, newPage: number, newPageSize: number, format?: CollectionViewState) => {
        const search = this.getSearch();
        return this.getListCacheIndexFromSearch(search, paneId, newPage, newPageSize, format);
    }

    popUrlState = (paneId = 1) => {
        this.currentPaneId = paneId;

        const capturedPane = this.capturedPanes[paneId];

        if (capturedPane) {
            this.capturedPanes[paneId] = null;
            let search = this.clearPane(this.getSearch(), paneId);
            search = _.merge(search, capturedPane.search);
            let path: string;
            let replace: boolean;
            ({ path, replace } = this.setupPaneNumberAndTypes(paneId, capturedPane.paneType));

            const result = { path: path, search: search, replace: replace };
            this.setNewSearch(result);
        } else {
            // probably reloaded page so no state to pop. 
            // just go home 
            this.setHome(paneId);
        }
    }

    clearUrlState = (paneId: number) => {
        this.currentPaneId = paneId;
        this.capturedPanes[paneId] = null;
    }

    private swapSearchIds(search: any) {
        return _.mapKeys(search,
            (v: any, k: string) => k.replace(/(\D+)(\d{1})(\w*)/, (match, p1, p2, p3) => `${p1}${p2 === "1" ? "2" : "1"}${p3}`));
    }

    swapPanes = () => {
        const path = this.getPath();
        const segments = path.split("/");
        const [, mode, oldPane1, oldPane2 = Constants.homePath] = segments;
        const newPath = `/${mode}/${oldPane2}/${oldPane1}`;
        const search = this.swapSearchIds(this.getSearch()) as any;
        this.currentPaneId = Models.getOtherPane(this.currentPaneId);

        const tree = this.router.createUrlTree([newPath], { queryParams: search });

        this.router.navigateByUrl(tree);
    }

    private setMode(newMode: string) {
        const newPath = `/${newMode}/${this.getPath().split("/")[2]}`;
        this.router.navigateByUrl(newPath);
    }

    cicero = () => this.setMode(Constants.ciceroPath);

    gemini = () => this.setMode(Constants.geminiPath);

    applicationProperties = () => {
        const newPath = `/${Constants.geminiPath}/${Constants.applicationPropertiesPath}`;
        this.router.navigateByUrl(newPath);
    }

    currentpane = () => this.currentPaneId;

    setHomeSinglePane = () => {
        this.currentPaneId = 1;

        const path = this.getPath();
        const segments = path.split("/");
        const mode = segments[1];
        const newPath = `/${mode}/${Constants.homePath}`;

        const tree = this.router.createUrlTree([newPath]);

        this.router.navigateByUrl(tree);
    }

    singlePane = (paneId = 1) => {
        this.currentPaneId = 1;

        if (!this.isSinglePane()) {

            const paneToKeepId = paneId;
            const paneToRemoveId = Models.getOtherPane(paneToKeepId);

            const path = this.getPath();
            const segments = path.split("/");
            const mode = segments[1];
            const paneToKeep = segments[paneToKeepId + 1];
            const newPath = `/${mode}/${paneToKeep}`;

            let search = this.getSearch();

            if (paneToKeepId === 1) {
                // just remove second pane
                search = this.clearPane(search, paneToRemoveId);
            }

            if (paneToKeepId === 2) {
                // swap pane 2 to pane 1 then remove 2
                search = this.swapSearchIds(search);
                search = this.clearPane(search, 2);
            }

            const tree = this.router.createUrlTree([newPath], { queryParams: search });

            this.router.navigateByUrl(tree);
        }
    }

    private getLocation(paneId: number) {
        const path = this.getPath();
        const segments = path.split("/");
        return segments[paneId + 1]; // e.g. segments 0=~/1=cicero/2=home/3=home
    }

    private isLocation(paneId: number, location: string) {
        return this.getLocation(paneId) === location; // e.g. segments 0=~/1=cicero/2=home/3=home
    }

    isHome = (paneId = 1) => this.isLocation(paneId, Constants.homePath);
    isObject = (paneId = 1) => this.isLocation(paneId, Constants.objectPath);
    isList = (paneId = 1) => this.isLocation(paneId, Constants.listPath);
    isError = (paneId = 1) => this.isLocation(paneId, Constants.errorPath);
    isRecent = (paneId = 1) => this.isLocation(paneId, Constants.recentPath);
    isAttachment = (paneId = 1) => this.isLocation(paneId, Constants.attachmentPath);
    isApplicationProperties = (paneId = 1) => this.isLocation(paneId, Constants.applicationPropertiesPath);
    isMultiLineDialog = (paneId = 1) => this.isLocation(paneId, Constants.multiLineDialogPath);

    private toggleReloadFlag(search: any) {
        const currentFlag = search[akm.reload];
        const newFlag = currentFlag === "1" ? 0 : 1;
        search[akm.reload] = newFlag;
        return search;
    }

    triggerPageReloadByFlippingReloadFlagInUrl = () => {
        const search = this.getSearch();
        this.toggleReloadFlag(search);
        const result = { path: this.getPath(), search: search, replace: true };
        this.setNewSearch(result);
    }
}
