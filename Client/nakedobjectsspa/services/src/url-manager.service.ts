import { Location } from '@angular/common';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as Ro from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';
import fill from 'lodash-es/fill';
import filter from 'lodash-es/filter';
import forEach from 'lodash-es/forEach';
import keys from 'lodash-es/keys';
import map from 'lodash-es/map';
import mapKeys from 'lodash-es/mapKeys';
import mapValues from 'lodash-es/mapValues';
import merge from 'lodash-es/merge';
import omit from 'lodash-es/omit';
import pick from 'lodash-es/pick';
import pickBy from 'lodash-es/pickBy';
import reduce from 'lodash-es/reduce';
import some from 'lodash-es/some';
import values from 'lodash-es/values';
import without from 'lodash-es/without';
import zipObject from 'lodash-es/zipObject';
import { map as rxjsmap } from 'rxjs/operators';
import { ConfigService } from './config.service';
import { ClientErrorCode, ErrorCategory, HttpStatusCode } from './error.wrapper';
import { LoggerService } from './logger.service';
import { ObfuscateService } from './obfuscate.service';

import {
    ApplicationMode,
    CollectionViewState,
    getOtherPane,
    InteractionMode,
    Pane,
    PaneRouteData,
    RouteData,
    ViewType
} from './route-data';

type ModePathSegment = 'gemini' | 'cicero';

const geminiPath: ModePathSegment = 'gemini';
const ciceroPath: ModePathSegment = 'cicero';

const homePath: PathSegment = 'home';
const objectPath: PathSegment = 'object';
const listPath: PathSegment = 'list';
const errorPath: PathSegment = 'error';
const recentPath: PathSegment = 'recent';
const attachmentPath: PathSegment = 'attachment';
const applicationPropertiesPath: PathSegment = 'applicationProperties';
const multiLineDialogPath: PathSegment = 'multiLineDialog';
const logoffPath: PathSegment = 'logoff';

type PathSegment = 'home' | 'object' | 'list' | 'error' | 'recent' | 'attachment' | 'applicationProperties' | 'multiLineDialog' | 'logoff';

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
}

// keep in alphabetic order to help avoid name collisions
// all key map
const akm = {
    action: 'a',
    actions: 'as',
    attachment: 'at',
    collection: 'c',
    dialog: 'd',
    errorCat: 'et',
    interactionMode: 'i',
    menu: 'm',
    object: 'o',
    page: 'pg',
    pageSize: 'ps',
    parm: 'pm',
    prop: 'pp',
    reload: 'r',
    selected: 's'
};

interface ITransitionResult {
    path: string;
    search: any;
    replace: boolean;
}

@Injectable()
export class UrlManagerService {

    constructor(
        private readonly router: Router,
        private readonly location: Location,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService,
        private readonly obfuscateService: ObfuscateService
    ) {
    }

    private get shortCutMarker() {
        return this.configService.config.shortCutMarker;
    }

    private get urlShortCuts() {
        return this.configService.config.urlShortCuts;
    }

    private get keySeparator() {
        return this.configService.config.keySeparator;
    }

    private capturedPanes = [] as ({ paneType: PathSegment; search: Object } | null)[];

    private currentPaneId: Pane = Pane.Pane1;

    private createSubMask(arr: boolean[]) {
        let nMask = 0;
        let nFlag = 0;

        if (arr.length > 31) {
            const msg = `UrlManagerService:createSubMask Out of range ${arr.length}`;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }

        const nLen = arr.length;
        // tslint:disable-next-line
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
        const masks = map(arrays, a => this.createSubMask(a).toString());

        return reduce(masks, (res: string, val) => res + '-' + val) || '';
    }

    // convert from mask string to array of bools
    private arrayFromSubMask(sMask: string) {
        const nMask = parseInt(sMask, 10);
        // nMask must be between 0 and 2147483647 - to keep simple we stick to 31 bits
        if (nMask > 0x7fffffff || nMask < -0x80000000) {
            const msg = `UrlManagerService:arrayFromSubMask Out of range ${nMask}`;
            this.loggerService.error(msg);
            throw new TypeError(msg);
        }
        const aFromMask = [] as boolean[];
        let len = 31; // make array always 31 bit long as we may concat another on end
        // tslint:disable-next-line
        for (let nShifted = nMask; len > 0; aFromMask.push(Boolean(nShifted & 1)), nShifted >>>= 1, --len);
        return aFromMask;
    }

    private arrayFromMask(sMask: string) {
        sMask = sMask || '0';
        const sMasks = sMask.split('-');
        const maskArrays = map(sMasks, s => this.arrayFromSubMask(s));
        return reduce(maskArrays, (res, val) => res.concat(val), [] as boolean[]);
    }

    private getSearch() {

        const url = this.router.url;
        return this.router.parseUrl(url).queryParams;
    }

    private getPath() {

        const url = this.router.url;
        let end = url.indexOf(';');
        end = end === -1 ? url.indexOf('?') : end;
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

    private getIds(typeOfId: string, paneId: Pane) {
        return <Dictionary<string>>pickBy(this.getSearch(), (v, k) => !!k && k.indexOf(typeOfId + paneId) === 0);
    }

    private mapIds(ids: Dictionary<string>): Dictionary<string> {
        return mapKeys(ids, (v: any, k: string) => k.substr(k.indexOf('_') + 1));
    }

    private getAndMapIds(typeOfId: string, paneId: Pane) {
        const ids = this.getIds(typeOfId, paneId);
        return this.mapIds(ids);
    }

    private getMappedValues(mappedIds: Dictionary<string>) {
        return mapValues(mappedIds, v => Ro.Value.fromJsonString(this.deobfuscate(v), this.shortCutMarker, this.urlShortCuts));
    }

    private getInteractionMode(rawInteractionMode: string): InteractionMode {
        return rawInteractionMode ? (<any>InteractionMode)[rawInteractionMode] : InteractionMode.View;
    }

    private getPaneParams(params: Dictionary<string>, paneId: number): { rp: Dictionary<string>, rpwr: Dictionary<string> } {
        const paneIds = filter(keys(params), k => (k.indexOf(paneId.toString()) >= 0));
        const allRawParams = pick(params, paneIds) as Dictionary<string>;
        const rawParamsWithoutReload = omit(allRawParams, akm.reload + paneId) as Dictionary<string>;
        return { rp: allRawParams, rpwr: rawParamsWithoutReload };
    }

    private obfuscate(s: string) {
        return this.obfuscateService.obfuscate(s);
    }

    private deobfuscate(s: string) {
        return this.obfuscateService.deobfuscate(s);
    }

    private setPaneRouteDataFromParms(paneRouteData: PaneRouteData, paneId: Pane, routeParams: { [key: string]: string }) {
        ({ rp: paneRouteData.rawParms, rpwr: paneRouteData.rawParmsWithoutReload } = this.getPaneParams(routeParams, paneId));
        paneRouteData.menuId = this.getId(akm.menu + paneId, routeParams);
        paneRouteData.actionId = this.getId(akm.action + paneId, routeParams);
        paneRouteData.dialogId = this.getId(akm.dialog + paneId, routeParams);

        const rawErrorCategory = this.getId(akm.errorCat + paneId, routeParams);
        paneRouteData.errorCategory = rawErrorCategory ? (<any>ErrorCategory)[rawErrorCategory] : null;

        paneRouteData.objectId =  this.deobfuscate(this.getId(akm.object + paneId, routeParams));
        paneRouteData.actionsOpen = this.getId(akm.actions + paneId, routeParams);

        const rawCollectionState = this.getId(akm.collection + paneId, routeParams);
        paneRouteData.state = rawCollectionState ? (<any>CollectionViewState)[rawCollectionState] : CollectionViewState.List;

        const rawInteractionMode = this.getId(akm.interactionMode + paneId, routeParams);
        paneRouteData.interactionMode = this.getInteractionMode(rawInteractionMode);

        const collKeyMap = this.getAndMapIds(akm.collection, paneId);
        paneRouteData.collections = mapValues(collKeyMap, v => (<any>CollectionViewState)[v]);

        const collSelectedKeyMap = this.getAndMapIds(akm.selected, paneId);
        paneRouteData.selectedCollectionItems = mapValues(collSelectedKeyMap, v => this.arrayFromMask(v));

        const parmKeyMap = this.getAndMapIds(akm.parm, paneId);
        paneRouteData.actionParams = this.getMappedValues(parmKeyMap);

        paneRouteData.page = parseInt(this.getId(akm.page + paneId, routeParams), 10);
        paneRouteData.pageSize = parseInt(this.getId(akm.pageSize + paneId, routeParams), 10);

        paneRouteData.attachmentId = this.getId(akm.attachment + paneId, routeParams);
    }

    private setPaneRouteData(paneRouteData: PaneRouteData, paneId: Pane) {
        const routeParams = this.getSearch();
        this.setPaneRouteDataFromParms(paneRouteData, paneId, routeParams);

        paneRouteData.validate(this.location.path());
    }

    private isSinglePane() {
        return this.getPath().split('/').length <= 3;
    }

    private searchKeysForPane(search: any, paneId: Pane, raw: string[]) {
        const ids = map(raw, s => s + paneId);
        return filter(keys(search), k => some(ids, id => k.indexOf(id) === 0));
    }

    private allSearchKeysForPane(search: any, paneId: Pane) {
        const raw = values(akm) as string[];
        return this.searchKeysForPane(search, paneId, raw);
    }

    private clearPane(search: any, paneId: Pane) {
        const toClear = this.allSearchKeysForPane(search, paneId);
        return omit(search, toClear) as Dictionary<string>;
    }

    private clearSearchKeys(search: any, paneId: Pane, searchKeys: string[]) {
        const toClear = this.searchKeysForPane(search, paneId, searchKeys);
        return omit(search, toClear);
    }

    private paneIsAlwaysSingle(paneType: string) {
        return paneType === multiLineDialogPath;
    }

    private setupPaneNumberAndTypes(pane: Pane, newPaneType: PathSegment, newMode?: ApplicationMode): { path: string, replace: boolean } {

        const path = this.getPath();
        const segments = path.split('/');
        // tslint:disable-next-line:prefer-const
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
        if (pane === Pane.Pane1 && pane1Type !== newPaneType) {
            const single = this.isSinglePane() || this.paneIsAlwaysSingle(newPaneType);
            newPath = `/${mode}/${newPaneType}${single ? '' : `/${pane2Type}`}`;
            changeMode = false;
            mayReplace = false;
        }

        // changing item on pane 2
        // either single pane so need to add new pane of appropriate type
        // or double pane with second pane of wrong type.
        if (pane === Pane.Pane2 && (this.isSinglePane() || pane2Type !== newPaneType)) {
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

    private capturePane(paneId: Pane) {
        const search = this.getSearch();
        const toCapture = this.allSearchKeysForPane(search, paneId);

        return pick(search, toCapture);
    }

    private getOidFromHref(href: string) {
        const oid = Ro.ObjectIdWrapper.fromHref(href, this.keySeparator);
        return oid.getKey();
    }

    private getPidFromHref(href: string) {
        return Ro.propertyIdFromUrl(href);
    }

    private setValue(paneId: Pane, search: any, p: { id: () => string }, pv: Ro.Value, valueType: string) {
        this.setId(`${valueType}${paneId}_${p.id()}`, pv.toJsonString(this.shortCutMarker, this.urlShortCuts), search);
    }

    private setParameter(paneId: Pane, search: any, p: Ro.Parameter, pv: Ro.Value) {
        this.setValue(paneId, search, p, pv, akm.parm);
    }

    private getId(key: string, search: any) {
        return Ro.decompress(search[key], this.shortCutMarker, this.urlShortCuts);
    }

    private setId(key: string, id: string, search: any) {
        search[key] = Ro.compress(id, this.shortCutMarker, this.urlShortCuts);
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
            case homePath:
                return this.validKeysForHome();
            case objectPath:
                return this.validKeysForObject();
            case listPath:
                return this.validKeysForList();
            case multiLineDialogPath:
                return this.validKeysForMultiLineDialog();
            case attachmentPath:
                return this.validKeysForAttachment();
        }

        return [];
    }

    private clearInvalidParmsFromSearch(paneId: Pane, search: any, path: string) {
        if (path) {
            const vks = this.validKeys(path);
            const ivks = without(values(akm), ...vks) as string[];
            return this.clearSearchKeys(search, paneId, ivks);
        }
        return search;
    }

    private handleTransition(paneId: Pane, search: any, transition: Transition): ITransitionResult {

        let replace = true;
        let path = this.getPath();

        switch (transition) {
            case (Transition.ToHome):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, homePath));
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

                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, objectPath));
                replace = false;
                search = this.clearPane(search, paneId);
                this.setId(akm.interactionMode + paneId, InteractionMode[InteractionMode.View], search);
                break;
            case (Transition.ToObjectWithMode):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, objectPath));
                replace = false;
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToList):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, listPath));
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
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, recentPath));
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToAttachment):
                ({ path, replace } = this.setupPaneNumberAndTypes(paneId, attachmentPath));
                search = this.clearPane(search, paneId);
                break;
            case (Transition.ToMultiLineDialog):
                ({ path, replace } = this.setupPaneNumberAndTypes(Pane.Pane1, multiLineDialogPath)); // always on 1
                if (paneId === Pane.Pane2) {
                    // came from 2
                    search = this.swapSearchIds(search);
                }
                search = this.clearPane(search, Pane.Pane2); // always on pane 1
                break;
            default:
                // null transition
                break;
        }

        const segments = path.split('/');
        const [, , pane1Type, pane2Type] = segments;

        search = this.clearInvalidParmsFromSearch(Pane.Pane1, search, pane1Type);
        search = this.clearInvalidParmsFromSearch(Pane.Pane2, search, pane2Type);

        return { path: path, search: search, replace: replace };
    }

    private executeTransition(newValues: Dictionary<string | null>, paneId: Pane, transition: Transition, condition: (search: any) => boolean) {
        this.currentPaneId = paneId;
        let search = this.getSearch();
        if (condition(search)) {
            const result = this.handleTransition(paneId, search, transition);
            ({ search } = result);
            forEach(newValues,
                (v, k) => {
                    // k should always be non null
                    if (v) {
                        this.setId(k, v, search);
                    } else {
                        this.clearId(k, search);
                    }
                }
            );
            this.setNewSearch(result);
        }
    }

    setHome = (paneId: Pane = Pane.Pane1) => {
        this.executeTransition({}, paneId, Transition.ToHome, () => true);
    }

    setRecent = (paneId: Pane = Pane.Pane1) => {
        this.executeTransition({}, paneId, Transition.ToRecent, () => true);
    }

    setMenu = (menuId: string, paneId: Pane = Pane.Pane1) => {
        const key = `${akm.menu}${paneId}`;
        const newValues = zipObject([key], [menuId]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToMenu, search => this.getId(key, search) !== menuId);
    }

    setDialog = (dialogId: string, paneId: Pane = Pane.Pane1) => {
        const key = `${akm.dialog}${paneId}`;
        const newValues = zipObject([key], [dialogId]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToDialog, search => this.getId(key, search) !== dialogId);
    }

    setMultiLineDialog = (dialogId: string, paneId: Pane) => {
        this.pushUrlState();
        const key = `${akm.dialog}${Pane.Pane1}`; // always on 1
        const newValues = zipObject([key], [dialogId]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToMultiLineDialog, search => this.getId(key, search) !== dialogId);
    }

    setDialogOrMultiLineDialog = (actionRep: Ro.ActionMember | Ro.ActionRepresentation, paneId: Pane = Pane.Pane1) => {
        if (actionRep.extensions().multipleLines()) {
            this.setMultiLineDialog(actionRep.actionId(), paneId);
        } else {
            this.setDialog(actionRep.actionId(), paneId);
        }
    }

    private closeOrCancelDialog(id: string, paneId: Pane, transition: Transition) {
        const key = `${akm.dialog}${paneId}`;
        const existingValue = this.getSearch()[key];

        if (existingValue === id) {
            const newValues = zipObject([key], [null]) as Dictionary<string | null>;
            this.executeTransition(newValues, paneId, transition, () => true);
        }
    }

    closeDialogKeepHistory = (id: string, paneId: Pane = Pane.Pane1) => {
        this.closeOrCancelDialog(id, paneId, Transition.FromDialogKeepHistory);
    }

    closeDialogReplaceHistory = (id: string, paneId: Pane = Pane.Pane1) => {
        this.closeOrCancelDialog(id, paneId, Transition.FromDialog);
    }

    setObject = (resultObject: Ro.DomainObjectRepresentation, paneId: Pane = Pane.Pane1) => {
        const oid = this.obfuscate(resultObject.id());
        const key = `${akm.object}${paneId}`;
        const newValues = zipObject([key], [oid]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setObjectWithMode = (resultObject: Ro.DomainObjectRepresentation, newMode: InteractionMode, paneId: Pane = Pane.Pane1) => {
        const oid =  this.obfuscate(resultObject.id());
        const okey = `${akm.object}${paneId}`;
        const mkey = `${akm.interactionMode}${paneId}`;
        const newValues = zipObject([okey, mkey], [oid, InteractionMode[newMode]]) as Dictionary<string>;

        this.executeTransition(newValues, paneId, Transition.ToObjectWithMode, () => true);
    }

    setList = (actionMember: Ro.ActionRepresentation | Ro.InvokableActionMember, parms: Dictionary<Ro.Value>, fromPaneId = Pane.Pane1, toPaneId = Pane.Pane1) => {
        const newValues = {} as Dictionary<string>;
        const parent = actionMember.parent;

        if (parent instanceof Ro.DomainObjectRepresentation) {
            newValues[`${akm.object}${toPaneId}`] = this.obfuscate(parent.id());
        }

        if (parent instanceof Ro.MenuRepresentation) {
            newValues[`${akm.menu}${toPaneId}`] = parent.menuId();
        }

        newValues[`${akm.action}${toPaneId}`] = actionMember.actionId();
        newValues[`${akm.page}${toPaneId}`] = '1';
        newValues[`${akm.pageSize}${toPaneId}`] = this.configService.config.defaultPageSize.toString();
        newValues[`${akm.selected}${toPaneId}_`] = '0';

        const newState = actionMember.extensions().renderEagerly() ? CollectionViewState[CollectionViewState.Table] : CollectionViewState[CollectionViewState.List];

        newValues[`${akm.collection}${toPaneId}`] = newState;

        forEach(parms, (p, id) => this.setId(`${akm.parm}${toPaneId}_${id}`, this.obfuscate(p.toJsonString(this.shortCutMarker, this.urlShortCuts)), newValues));

        this.executeTransition(newValues, toPaneId, Transition.ToList, () => true);
        return newValues;
    }

    setProperty = (href: string, paneId: Pane = Pane.Pane1) => {
        const oid = this.obfuscate(this.getOidFromHref(href));
        const key = `${akm.object}${paneId}`;
        const newValues = zipObject([key], [oid]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setItem = (link: Ro.Link, paneId: Pane = Pane.Pane1) => {
        const href = link.href();
        const oid = this.obfuscate(this.getOidFromHref(href));
        const key = `${akm.object}${paneId}`;
        const newValues = zipObject([key], [oid]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToObjectView, () => true);
    }

    setAttachment = (attachmentlink: Ro.Link, paneId: Pane = Pane.Pane1) => {
        const href = attachmentlink.href();
        const okey = `${akm.object}${paneId}`;
        const akey = `${akm.attachment}${paneId}`;
        const oid = this.obfuscate(this.getOidFromHref(href));
        const pid = this.getPidFromHref(href);

        const newValues = zipObject([okey, akey], [oid, pid]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.ToAttachment, () => true);
    }

    toggleObjectMenu = (paneId: Pane = Pane.Pane1) => {
        const key = akm.actions + paneId;
        const actionsId = this.getSearch()[key] ? null : 'open';
        const newValues = zipObject([key], [actionsId]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    private checkAndSetValue(paneId: Pane, check: (search: any) => boolean, set: (search: any) => void) {
        this.currentPaneId = paneId;
        const search = this.getSearch();

        // only add field if matching dialog or dialog (to catch case when swapping panes)
        if (check(search)) {
            set(search);
            const result = { path: this.getPath(), search: search, replace: false };
            this.setNewSearch(result);
        }
    }

    setParameterValue = (actionId: string, p: Ro.Parameter, pv: Ro.Value, paneId: Pane = Pane.Pane1) =>
        this.checkAndSetValue(paneId,
            search => this.getId(`${akm.action}${paneId}`, search) === actionId,
            search => this.setParameter(paneId, search, p, pv))

    setCollectionMemberState = (collectionMemberId: string, state: CollectionViewState, paneId: Pane = Pane.Pane1) => {
        const key = `${akm.collection}${paneId}_${collectionMemberId}`;
        const newValues = zipObject([key], [CollectionViewState[state]]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setListState = (state: CollectionViewState, paneId: Pane = Pane.Pane1) => {
        const key = `${akm.collection}${paneId}`;
        const newValues = zipObject([key], [CollectionViewState[state]]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setInteractionMode = (newMode: InteractionMode, paneId: Pane = Pane.Pane1) => {
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

        const newValues = zipObject([key], [InteractionMode[newMode]]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, transition, () => true);
    }

    setItemSelected = (item: number, isSelected: boolean, collectionId: string, paneId: Pane = Pane.Pane1) => {

        const key = `${akm.selected}${paneId}_${collectionId}`;
        const currentSelected = this.getSearch()[key];
        const selectedArray = this.arrayFromMask(currentSelected);
        selectedArray[item] = isSelected;
        const currentSelectedAsString = (this.createMask(selectedArray)).toString();
        const newValues = zipObject([key], [currentSelectedAsString]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setAllItemsSelected = (isSelected: boolean, collectionId: string, paneId: Pane = Pane.Pane1) => {

        const key = `${akm.selected}${paneId}_${collectionId}`;
        const currentSelected = this.getSearch()[key];
        const selectedArray = this.arrayFromMask(currentSelected);
        fill(selectedArray, isSelected);
        const currentSelectedAsString = (this.createMask(selectedArray)).toString();
        const newValues = zipObject([key], [currentSelectedAsString]) as Dictionary<string>;
        this.executeTransition(newValues, paneId, Transition.Null, () => true);
    }

    setListPaging = (newPage: number, newPageSize: number, state: CollectionViewState, paneId: Pane = Pane.Pane1) => {
        const pageValues = {} as Dictionary<string>;

        pageValues[`${akm.page}${paneId}`] = newPage.toString();
        pageValues[`${akm.pageSize}${paneId}`] = newPageSize.toString();
        pageValues[`${akm.collection}${paneId}`] = CollectionViewState[state];
        pageValues[`${akm.selected}${paneId}_`] = '0'; // clear selection

        this.executeTransition(pageValues, paneId, Transition.Page, () => true);
    }

    setError = (errorCategory: ErrorCategory, ec?: ClientErrorCode | HttpStatusCode) => {
        const path = this.getPath();
        const segments = path.split('/');
        const mode = segments[1];
        const newPath = `/${mode}/error`;

        const search: any = {};
        // always on pane 1
        search[akm.errorCat + Pane.Pane1] = ErrorCategory[errorCategory];

        const result = { path: newPath, search: search, replace: false };

        if (errorCategory === ErrorCategory.HttpClientError &&
            (ec === HttpStatusCode.PreconditionFailed || ec === HttpStatusCode.NotFound)) {
            result.replace = true;
        }
        this.setNewSearch(result);
    }

    getRouteData = () => {
        const routeData = new RouteData(this.configService, this.loggerService);

        this.setPaneRouteData(routeData.pane1, Pane.Pane1);
        this.setPaneRouteData(routeData.pane2, Pane.Pane2);

        return routeData;
    }

    getViewType(view: string) {
        switch (view) {
            case homePath: return ViewType.Home;
            case objectPath: return ViewType.Object;
            case listPath: return ViewType.List;
            case errorPath: return ViewType.Error;
            case recentPath: return ViewType.Recent;
            case attachmentPath: return ViewType.Attachment;
            case applicationPropertiesPath: return ViewType.ApplicationProperties;
            case multiLineDialogPath: return ViewType.MultiLineDialog;
        }
        return this.loggerService.throw(`UrlManagerService:getViewType ${view} is not a valid ViewType`);
    }

    getPaneRouteDataObservable = (paneId: Pane) => {

        return this.router.routerState.root.queryParams.pipe(rxjsmap((ps: { [key: string]: string }) => {
            const routeData = new RouteData(this.configService, this.loggerService);
            const paneRouteData = routeData.pane(paneId)!;
            this.setPaneRouteDataFromParms(paneRouteData, paneId, ps);
            paneRouteData.location = this.getViewType(this.getLocation(paneId))!;
            return paneRouteData;
        }));
    }

    pushUrlState = (paneId: Pane = Pane.Pane1) => {
        this.capturedPanes[paneId] = this.getUrlState(paneId);
    }

    getUrlState = (paneId: Pane = Pane.Pane1) => {
        this.currentPaneId = paneId;

        const path = this.getPath();
        const segments = path.split('/');

        const paneType = <PathSegment>segments[paneId + 1] || homePath;
        let paneSearch = this.capturePane(paneId);

        // clear any dialogs so we don't return  to a dialog

        paneSearch = omit(paneSearch, `${akm.dialog}${paneId}`);

        return { paneType: paneType, search: paneSearch };
    }

    getListCacheIndexFromSearch = (search: Dictionary<string>, paneId: Pane, newPage: number, newPageSize: number, format?: CollectionViewState) => {

        const s1 = this.getId(`${akm.menu}${paneId}`, search) || '';
        const s2 = this.deobfuscate(this.getId(`${akm.object}${paneId}`, search) || '');
        const s3 = this.getId(`${akm.action}${paneId}`, search) || '';

        const parms = <Dictionary<string>>pickBy(search, (v, k) => !!k && k.indexOf(akm.parm + paneId) === 0);
        const mappedParms = mapValues(parms, v => decodeURIComponent(Ro.decompress(this.deobfuscate(v), this.shortCutMarker, this.urlShortCuts)));

        const s4 = reduce(mappedParms, (r, n, k) => r + (k + '=' + n + this.keySeparator), '');

        const s5 = `${newPage}`;
        const s6 = `${newPageSize}`;

        const s7 = format ? `${format}` : '';

        const ss = [s1, s2, s3, s4, s5, s6, s7] as string[];

        return reduce(ss, (r, n) => r + this.keySeparator + n, '');
    }

    getListCacheIndex = (paneId: Pane, newPage: number, newPageSize: number, format?: CollectionViewState) => {
        const search = this.getSearch();
        return this.getListCacheIndexFromSearch(search, paneId, newPage, newPageSize, format);
    }

    popUrlState = (paneId: Pane = Pane.Pane1) => {
        this.currentPaneId = paneId;

        const capturedPane = this.capturedPanes[paneId];

        if (capturedPane) {
            this.capturedPanes[paneId] = null;
            let search = this.clearPane(this.getSearch(), paneId);
            search = merge(search, capturedPane.search);
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

    clearUrlState = (paneId: Pane) => {
        this.currentPaneId = paneId;
        this.capturedPanes[paneId] = null;
    }

    private swapSearchIds(search: any) {
        return mapKeys(search,
            (v: any, k: string) => k.replace(/(\D+)(\d{1})(\w*)/, (match, p1, p2, p3) => `${p1}${p2 === '1' ? '2' : '1'}${p3}`));
    }

    swapPanes = () => {
        const path = this.getPath();
        const segments = path.split('/');
        const [, mode, oldPane1, oldPane2 = homePath] = segments;
        const newPath = `/${mode}/${oldPane2}/${oldPane1}`;
        const search = this.swapSearchIds(this.getSearch()) as any;
        this.currentPaneId = getOtherPane(this.currentPaneId);

        const tree = this.router.createUrlTree([newPath], { queryParams: search });

        this.router.navigateByUrl(tree);
    }

    private setMode(newMode: string) {
        const path = this.getPath();
        const segments = path.split('/');
        const [, , pane1] = segments;
        const newPath = `/${newMode}/${pane1}`;
        const search = this.clearPane(this.getSearch(), Pane.Pane2);
        const tree = this.router.createUrlTree([newPath], { queryParams: search });

        this.router.navigateByUrl(tree);
    }

    private getMode() {
        const path = this.getPath();
        const segments = path.split('/');
        const [, mode] = segments;
        return mode as ModePathSegment;
    }

    cicero = () => this.setMode(ciceroPath);

    gemini = () => this.setMode(geminiPath);

    isGemini = () => this.getMode() === geminiPath;

    applicationProperties = () => {
        const newPath = `/${geminiPath}/${applicationPropertiesPath}`;
        this.router.navigateByUrl(newPath);
    }

    logoff = () => {
        const newPath = `/${geminiPath}/${logoffPath}`;
        this.router.navigateByUrl(newPath);
    }

    currentpane = () => this.currentPaneId;

    setHomeSinglePane = () => {
        this.currentPaneId = Pane.Pane1;

        const path = this.getPath();
        const segments = path.split('/');
        const mode = segments[1] || geminiPath;
        const newPath = `/${mode}/${homePath}`;

        const tree = this.router.createUrlTree([newPath]);

        this.router.navigateByUrl(tree);
    }

    singlePane = (paneId: Pane = Pane.Pane1) => {
        this.currentPaneId = Pane.Pane1;

        if (!this.isSinglePane()) {

            const paneToKeepId = paneId;
            const paneToRemoveId = getOtherPane(paneToKeepId);

            const path = this.getPath();
            const segments = path.split('/');
            const mode = segments[1];
            const paneToKeep = segments[paneToKeepId + 1];
            const newPath = `/${mode}/${paneToKeep}`;

            let search = this.getSearch();

            if (paneToKeepId === Pane.Pane1) {
                // just remove second pane
                search = this.clearPane(search, paneToRemoveId);
            }

            if (paneToKeepId === Pane.Pane2) {
                // swap pane 2 to pane 1 then remove 2
                search = this.swapSearchIds(search);
                search = this.clearPane(search, Pane.Pane2);
            }

            const tree = this.router.createUrlTree([newPath], { queryParams: search });

            this.router.navigateByUrl(tree);
        }
    }

    private getLocation(paneId: Pane) {
        const path = this.getPath();
        const segments = path.split('/');
        return segments[paneId + 1]; // e.g. segments 0=~/1=cicero/2=home/3=home
    }

    private isLocation(paneId: Pane, location: string) {
        return this.getLocation(paneId) === location; // e.g. segments 0=~/1=cicero/2=home/3=home
    }

    isHome = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, homePath);
    isObject = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, objectPath);
    isList = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, listPath);
    isError = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, errorPath);
    isRecent = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, recentPath);
    isAttachment = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, attachmentPath);
    isApplicationProperties = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, applicationPropertiesPath);
    isMultiLineDialog = (paneId: Pane = Pane.Pane1) => this.isLocation(paneId, multiLineDialogPath);

    private toggleReloadFlag(search: any, paneId: Pane) {
        const currentFlag = search[akm.reload + paneId];
        const newFlag = currentFlag === '1' ? 0 : 1;
        search[akm.reload + paneId] = newFlag;
        return search;
    }

    triggerPageReloadByFlippingReloadFlagInUrl = (paneId: Pane = Pane.Pane1) => {
        const search = this.getSearch();
        this.toggleReloadFlag(search, paneId);
        const result = { path: this.getPath(), search: search, replace: true };
        this.setNewSearch(result);
    }
}
