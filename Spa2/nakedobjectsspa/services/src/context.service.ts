import { Injectable } from '@angular/core';
import * as Ro from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';
import each from 'lodash-es/each';
import filter from 'lodash-es/filter';
import find from 'lodash-es/find';
import findKey from 'lodash-es/findKey';
import first from 'lodash-es/first';
import forEach from 'lodash-es/forEach';
import keys from 'lodash-es/keys';
import map from 'lodash-es/map';
import omit from 'lodash-es/omit';
import remove from 'lodash-es/remove';
import sortBy from 'lodash-es/sortBy';
import { Subject } from 'rxjs';
import { ConfigService } from './config.service';
import * as Constants from './constants';
import { ClientErrorCode, ErrorCategory } from './constants';
import { LoggerService } from './logger.service';
import { RepLoaderService } from './rep-loader.service';
import { CollectionViewState, InteractionMode, Pane, PaneRouteData } from './route-data';
import { UrlManagerService } from './url-manager.service';
import { ErrorWrapper, } from './error.wrapper';

enum DirtyState {
    DirtyMustReload = 1,
    DirtyMayReload = 2,
    Clean = 3
}

class DirtyList {
    private dirtyObjects: Dictionary<DirtyState> = {};

    setDirty(oid: Ro.ObjectIdWrapper, alwaysReload: boolean = false) {
        this.setDirtyInternal(oid, alwaysReload ? DirtyState.DirtyMustReload : DirtyState.DirtyMayReload);
    }

    setDirtyInternal(oid: Ro.ObjectIdWrapper, dirtyState: DirtyState) {
        const key = oid.getKey();
        this.dirtyObjects[key] = dirtyState;
    }

    getDirty(oid: Ro.ObjectIdWrapper) {
        const key = oid.getKey();
        return this.dirtyObjects[key] || DirtyState.Clean;
    }

    clearDirty(oid: Ro.ObjectIdWrapper) {
        const key = oid.getKey();
        this.dirtyObjects = omit(this.dirtyObjects, key) as Dictionary<DirtyState>;
    }

    clear() {
        this.dirtyObjects = {};
    }
}

function isSameObject(object: Ro.DomainObjectRepresentation | null | undefined, type: string, id?: string) {
    if (object) {
        const sid = object.serviceId();
        return sid ? sid === type : (object.domainType() === type && object.instanceId() === Ro.withNull(id));
    }
    return false;
}

class TransientCache {
    private transientCache: [undefined, Ro.DomainObjectRepresentation[], Ro.DomainObjectRepresentation[]] = [undefined, [], []]; // per pane

    constructor(private readonly depth: number) { }

    add(paneId: Pane, obj: Ro.DomainObjectRepresentation) {
        let paneObjects = this.transientCache[paneId]!;
        if (paneObjects.length >= this.depth) {
            paneObjects = paneObjects.slice(-(this.depth - 1));
        }
        paneObjects.push(obj);
        this.transientCache[paneId] = paneObjects;
    }

    get(paneId: Pane, type: string, id: string): Ro.DomainObjectRepresentation | null {
        const paneObjects = this.transientCache[paneId]!;
        return find(paneObjects, o => isSameObject(o, type, id)) || null;
    }

    remove(paneId: Pane, type: string, id: string) {
        let paneObjects = this.transientCache[paneId]!;
        paneObjects = remove(paneObjects, o => isSameObject(o, type, id));
        this.transientCache[paneId] = paneObjects;
    }

    clear() {
        this.transientCache = [undefined, [], []];
    }

    swap() {
        const [, t1, t2] = this.transientCache;

        this.transientCache[1] = t2;
        this.transientCache[2] = t1;
    }
}

class RecentCache {

    constructor(private readonly keySeparator: string, private readonly depth: number) { }

    private recentCache: Ro.DomainObjectRepresentation[] = [];

    add(obj: Ro.DomainObjectRepresentation) {

        // find any matching entries and remove them - should only be one
        remove(this.recentCache, i => i.id() === obj.id());

        // push obj on top of array
        this.recentCache = [obj].concat(this.recentCache);

        // drop oldest if we're full
        if (this.recentCache.length > this.depth) {
            this.recentCache = this.recentCache.slice(0, this.depth);
        }
    }

    items(): Ro.DomainObjectRepresentation[] {
        return this.recentCache;
    }

    clear() {
        this.recentCache = [];
    }
}

class ValueCache {

    private currentValues: [undefined, Dictionary<Ro.Value>, Dictionary<Ro.Value>] = [undefined, {}, {}];
    private currentId: [undefined, string, string] = [undefined, '', ''];

    addValue(id: string, valueId: string, value: Ro.Value, paneId: Pane) {
        if (this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }

        this.currentValues[paneId]![valueId] = new Ro.Value(value);
    }

    getValue(id: string, valueId: string, paneId: Pane) {
        if (this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }

        return this.currentValues[paneId]![valueId];
    }

    getValues(id: string | null, paneId: Pane) {
        if (id && this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }

        return this.currentValues[paneId] as Dictionary<Ro.Value>;
    }

    clear(paneId: Pane) {
        this.currentId[paneId] = '';
        this.currentValues[paneId] = {};
    }

    swap() {
        const [, i1, i2] = this.currentId;

        this.currentId[Pane.Pane1] = i2;
        this.currentId[Pane.Pane2] = i1;

        const [, v1, v2] = this.currentValues;

        this.currentValues[Pane.Pane1] = v2;
        this.currentValues[Pane.Pane2] = v1;
    }

}

@Injectable()
export class ContextService {

    clearingDataFlag = false;

    constructor(
        private readonly urlManager: UrlManagerService,
        private readonly repLoader: RepLoaderService,
        private readonly configService: ConfigService,
        private readonly loggerService: LoggerService
    ) {
    }

    private pendingClearMessages = false;
    private pendingClearWarnings = false;
    private nextTransientId = 0;
    private currentError: ErrorWrapper | null = null;
    private previousUrl: string | null = null;
    private warningsSource = new Subject<string[]>();
    private messagesSource = new Subject<string[]>();

    warning$ = this.warningsSource.asObservable();
    messages$ = this.messagesSource.asObservable();

    private pendingPotentActionCount: [undefined, number, number] = [undefined, 0, 0];

    private concurrencyErrorSource = new Subject<Ro.ObjectIdWrapper>();

    concurrencyError$ = this.concurrencyErrorSource.asObservable();

    private subTypeCache: Dictionary<Dictionary<Promise<boolean>>> = {};

    private get keySeparator() {
        return this.configService.config.keySeparator;
    }

    // cached values

    private currentObjects: [undefined, Ro.DomainObjectRepresentation | null, Ro.DomainObjectRepresentation | null] = [undefined, null, null]; // per pane
    private transientCache = new TransientCache(this.configService.config.transientCacheDepth);

    private currentMenuList: Dictionary<Ro.MenuRepresentation> = {};
    private currentServices: Ro.DomainServicesRepresentation | null = null;
    private currentMenus: Ro.MenusRepresentation | null = null;
    private currentVersion: Ro.VersionRepresentation | null = null;
    private currentUser: Ro.UserRepresentation | null = null;

    private readonly recentcache = new RecentCache(this.keySeparator, this.configService.config.recentCacheDepth);
    private readonly dirtyList = new DirtyList();
    private currentLists: Dictionary<{ list: Ro.ListRepresentation; added: number }> = {};
    private readonly parameterCache = new ValueCache();
    private readonly objectEditCache = new ValueCache();

    getFile = (object: Ro.DomainObjectRepresentation, url: string, mt: string) => {
        const isDirty = this.getIsDirty(object.getOid());
        return this.repLoader.getFile(url, mt, isDirty);
    }

    setFile = (object: Ro.DomainObjectRepresentation, url: string, mt: string, file: Blob) => this.repLoader.uploadFile(url, mt, file);

    clearCachedFile = (url: string) => this.repLoader.clearCache(url);

    clearCachedCollections(obj: Ro.DomainObjectRepresentation) {

        each(obj.collectionMembers(), cm => {
            const details = cm.getDetails();

            if (details) {
                const baseUrl = details.getUrl();
                details.setUrlParameter(Constants.roInlineCollectionItems, true);
                const inlineUrl = details.getUrl();

                this.repLoader.clearCache(baseUrl);
                this.repLoader.clearCache(inlineUrl);
            }
        });
    }

    // exposed for test mocking
    getDomainObject = (paneId: Pane, oid: Ro.ObjectIdWrapper, interactionMode: InteractionMode): Promise<Ro.DomainObjectRepresentation> => {
        const type = oid.domainType;
        const id = oid.instanceId;

        const dirtyState = this.dirtyList.getDirty(oid);
        // no need to reload forms
        const forceReload = interactionMode !== InteractionMode.Form &&
                            ((dirtyState === DirtyState.DirtyMustReload) ||
                             ((dirtyState === DirtyState.DirtyMayReload) && this.configService.config.autoLoadDirty));

        if (!forceReload && isSameObject(this.currentObjects[paneId], type, id)) {
            return Promise.resolve(this.currentObjects[paneId]!);
        }

        // deeper cache for transients
        if (interactionMode === InteractionMode.Transient) {
            const transientObj = this.transientCache.get(paneId, type, id);
            const p: Promise<Ro.DomainObjectRepresentation> = transientObj
                ? Promise.resolve(transientObj)
                : Promise.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.ExpiredTransient, ''));
            return p;
        }

        const object = new Ro.DomainObjectRepresentation();
        object.hateoasUrl = `${this.configService.config.appPath}/objects/${type}/${id}`;
        object.setInlinePropertyDetails(interactionMode === InteractionMode.Edit);

        this.incPendingPotentActionOrReload(paneId);
        return this.repLoader.populate<Ro.DomainObjectRepresentation>(object, forceReload)
            .then((obj: Ro.DomainObjectRepresentation) => {
                this.currentObjects[paneId] = obj;
                if (forceReload) {
                    this.dirtyList.clearDirty(oid);
                    this.clearCachedCollections(obj);
                }
                this.cacheRecentlyViewed(obj);
                this.decPendingPotentActionOrReload(paneId);
                return Promise.resolve(obj);
            })
            .catch((e) => {
                this.decPendingPotentActionOrReload(paneId);
                throw e;
            });
    }

    private editOrReloadObject(paneId: Pane, object: Ro.DomainObjectRepresentation, inlineDetails: boolean) {
        const parms: Dictionary<Object> = {};
        parms[Constants.roInlinePropertyDetails] = inlineDetails;

        return this.repLoader.retrieveFromLink<Ro.DomainObjectRepresentation>(object.selfLink(), parms)
            .then(obj => {
                this.currentObjects[paneId] = obj;
                const oid = obj.getOid();
                this.dirtyList.clearDirty(oid);
                this.cacheRecentlyViewed(obj);
                return Promise.resolve(obj);
            });
    }

    getIsDirty = (oid: Ro.ObjectIdWrapper) => !oid.isService && this.dirtyList.getDirty(oid) !== DirtyState.Clean;

    mustReload = (oid: Ro.ObjectIdWrapper) => {
        const dirtyState = this.dirtyList.getDirty(oid);
        return (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && this.configService.config.autoLoadDirty);
    }

    getObjectForEdit = (paneId: Pane, object: Ro.DomainObjectRepresentation) => this.editOrReloadObject(paneId, object, true);

    reloadObject = (paneId: Pane, object: Ro.DomainObjectRepresentation) => this.editOrReloadObject(paneId, object, false);

    getService = (paneId: Pane, serviceType: string): Promise<Ro.DomainObjectRepresentation> => {

        if (isSameObject(this.currentObjects[paneId], serviceType)) {
            return Promise.resolve(this.currentObjects[paneId]!);
        }

        return this.getServices()
            .then((services: Ro.DomainServicesRepresentation) => {
                const service = services.getService(serviceType);
                if (service) {
                    return this.repLoader.populate(service);
                }
                return Promise.reject(`unknown service ${serviceType}`);
            })
            .then((service: Ro.DomainObjectRepresentation) => {
                this.currentObjects[paneId] = service;
                return Promise.resolve(service);
            });
    }

    getActionDetails = (actionMember: Ro.ActionMember): Promise<Ro.ActionRepresentation> => {
        const details = actionMember.getDetails();
        if (details) {
            return this.repLoader.populate(details, true);
        }
        return Promise.reject(`Couldn't find details on ${actionMember.actionId()}`);
    }

    getCollectionDetails = (collectionMember: Ro.CollectionMember, state: CollectionViewState, ignoreCache: boolean): Promise<Ro.CollectionRepresentation> => {
        const details = collectionMember.getDetails();

        if (details) {
            if (state === CollectionViewState.Table) {
                details.setUrlParameter(Constants.roInlineCollectionItems, true);
            }
            const parent = collectionMember.parent;
            let isDirty = false;
            if (parent instanceof Ro.DomainObjectRepresentation) {
                const oid = parent.getOid();
                isDirty = this.dirtyList.getDirty(oid) !== DirtyState.Clean;
            }

            return this.repLoader.populate(details, isDirty || ignoreCache);
        }
        return Promise.reject(`Couldn't find details on ${collectionMember.collectionId()}`);
    }

    getInvokableAction = (action: Ro.ActionMember | Ro.ActionRepresentation): Promise<Ro.InvokableActionMember | Ro.ActionRepresentation> => {

        if (action instanceof Ro.InvokableActionMember || action instanceof Ro.ActionRepresentation) {
            return Promise.resolve(action);
        }

        return this.getActionDetails(action);
    }

    getMenu = (menuId: string): Promise<Ro.MenuRepresentation> => {

        if (this.currentMenuList[menuId]) {
            return Promise.resolve(this.currentMenuList[menuId]);
        }

        return this.getMenus()
            .then((menus: Ro.MenusRepresentation) => {
                const menu = menus.getMenu(menuId);
                if (menu) {
                    return this.repLoader.populate(menu);
                }
                return Promise.reject(`couldn't find menu ${menuId}`);
            })
            .then((menu: Ro.MenuRepresentation) => {
                this.currentMenuList[menuId] = menu;
                return Promise.resolve(menu);
            });
    }

    clearMessages = () => {
        if (this.pendingClearMessages) {
            this.messagesSource.next([]);
        }
        this.pendingClearMessages = !this.pendingClearMessages;
    }

    clearWarnings = () => {
        if (this.pendingClearWarnings) {
            this.warningsSource.next([]);
        }
        this.pendingClearWarnings = !this.pendingClearWarnings;
    }

    broadcastMessage = (m: string) => {
        this.pendingClearMessages = false;
        this.messagesSource.next([m]);
    }

    broadcastWarning = (w: string) => {
        this.pendingClearWarnings = false;
        this.warningsSource.next([w]);
    }

    getHome = () => {
        // for moment don't bother caching only called on startup and for whatever resaon cache doesn't work.
        // once version cached no longer called.
        return this.repLoader.populate<Ro.HomePageRepresentation>(new Ro.HomePageRepresentation({}, this.configService.config.appPath));
    }

    getServices = () => {

        if (this.currentServices) {
            return Promise.resolve(this.currentServices);
        }

        return this.getHome()
            .then((home: Ro.HomePageRepresentation) => {
                const ds = home.getDomainServices();
                return this.repLoader.populate<Ro.DomainServicesRepresentation>(ds);
            })
            .then((services: Ro.DomainServicesRepresentation) => {
                this.currentServices = services;
                return Promise.resolve(services);
            });
    }

    getMenus = () => {
        if (this.currentMenus) {
            return Promise.resolve(this.currentMenus);
        }

        return this.getHome()
            .then((home: Ro.HomePageRepresentation) => {
                const ds = home.getMenus();
                return this.repLoader.populate<Ro.MenusRepresentation>(ds);
            })
            .then((menus: Ro.MenusRepresentation) => {
                this.currentMenus = menus;
                return Promise.resolve(this.currentMenus);
            });
    }

    getVersion = () => {

        if (this.currentVersion) {
            return Promise.resolve(this.currentVersion);
        }

        return this.getHome()
            .then((home: Ro.HomePageRepresentation) => {
                const v = home.getVersion();
                return this.repLoader.populate<Ro.VersionRepresentation>(v);
            })
            .then((version: Ro.VersionRepresentation) => {
                this.currentVersion = version;
                return Promise.resolve(version);
            });
    }

    getUser = () => {

        if (this.currentUser) {
            return Promise.resolve(this.currentUser);
        }

        return this.getHome()
            .then((home: Ro.HomePageRepresentation) => {
                const u = home.getUser();
                return this.repLoader.populate<Ro.UserRepresentation>(u);
            })
            .then((user: Ro.UserRepresentation) => {
                this.currentUser = user;
                return Promise.resolve(user);
            });
    }

    getObject = (paneId: Pane, oid: Ro.ObjectIdWrapper, interactionMode: InteractionMode) => {
        return oid.isService ? this.getService(paneId, oid.domainType) : this.getDomainObject(paneId, oid, interactionMode);
    }

    getCachedList = (paneId: Pane, page: number, pageSize: number) => {
        const index = this.urlManager.getListCacheIndex(paneId, page, pageSize);
        const entry = this.currentLists[index];
        return entry ? entry.list : null;
    }

    clearCachedList = (paneId: Pane, page: number, pageSize: number) => {
        const index = this.urlManager.getListCacheIndex(paneId, page, pageSize);
        delete this.currentLists[index];
    }

    private cacheList(list: Ro.ListRepresentation, index: string) {

        const entry = this.currentLists[index];
        if (entry) {
            entry.list = list;
            entry.added = Date.now();
        } else {

            if (keys(this.currentLists).length >= this.configService.config.listCacheSize) {
                // delete oldest;
                // TODO this looks wrong surely just "added" test !
                // Fix "!"
                const oldest = first(sortBy(this.currentLists, 'e.added'))!.added;
                const oldestIndex = findKey(this.currentLists, (e: { added: number }) => e.added === oldest);
                if (oldestIndex) {
                    delete this.currentLists[oldestIndex];
                }
            }

            this.currentLists[index] = { list: list, added: Date.now() };
        }
    }

    private handleResult = (paneId: Pane, result: Ro.ActionResultRepresentation, page: number, pageSize: number): Promise<Ro.ListRepresentation> => {

        if (result.resultType() === 'list') {
            const resultList = result.result().list() as Ro.ListRepresentation; // not null
            const index = this.urlManager.getListCacheIndex(paneId, page, pageSize);
            this.cacheList(resultList, index);
            return Promise.resolve(resultList);
        } else {
            return Promise.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.WrongType, 'expect list'));
        }
    }

    private getList = (paneId: Pane, resultPromise: () => Promise<Ro.ActionResultRepresentation>, page: number, pageSize: number) => {
        return resultPromise().then(result => this.handleResult(paneId, result, page, pageSize));
    }

    getActionExtensionsFromMenu = (menuId: string, actionId: string) =>
        this.getMenu(menuId)
        .then(menu => Promise.resolve(menu.actionMember(actionId).extensions()))

    getActionExtensionsFromObject = (paneId: Pane, oid: Ro.ObjectIdWrapper, actionId: string) =>
        this.getObject(paneId, oid, InteractionMode.View).then(object => Promise.resolve(object.actionMember(actionId).extensions()))

    private getPagingParms(page: number, pageSize: number): Dictionary<Object> {
        // TODO refactor this - don't think x-ro-... values should be exposed like this
        return (page && pageSize) ? { 'x-ro-page': page, 'x-ro-pageSize': pageSize } : {};
    }

    getListFromMenu = (routeData: PaneRouteData, page?: number, pageSize?: number) => {
        const menuId = routeData.menuId;
        const actionId = routeData.actionId;
        const parms = routeData.actionParams;
        const state = routeData.state;
        const paneId = routeData.paneId;
        const newPage = page || routeData.page;
        const newPageSize = pageSize || routeData.pageSize;
        const urlParms = this.getPagingParms(newPage, newPageSize);

        if (state === CollectionViewState.Table) {
            urlParms[Constants.roInlineCollectionItems] = true;
        }

        const promise = () => this.getMenu(menuId).then(menu => this.getInvokableAction(menu.actionMember(actionId))).then(details => this.repLoader.invoke(details, parms, urlParms));
        return this.getList(paneId, promise, newPage, newPageSize);
    }

    getListFromObject = (routeData: PaneRouteData, page?: number, pageSize?: number) => {
        const objectId = routeData.objectId;
        const actionId = routeData.actionId;
        const parms = routeData.actionParams;
        const state = routeData.state;
        const oid = Ro.ObjectIdWrapper.fromObjectId(objectId, this.keySeparator);
        const paneId = routeData.paneId;
        const newPage = page || routeData.page;
        const newPageSize = pageSize || routeData.pageSize;
        const urlParms = this.getPagingParms(newPage, newPageSize);

        if (state === CollectionViewState.Table) {
            urlParms[Constants.roInlineCollectionItems] = true;
        }

        const promise = () => this.getObject(paneId, oid, InteractionMode.View)
            .then(object => this.getInvokableAction(object.actionMember(actionId)))
            .then(details => this.repLoader.invoke(details, parms, urlParms));

        return this.getList(paneId, promise, newPage, newPageSize);
    }

    setObject = (paneId: Pane, co: Ro.DomainObjectRepresentation) => this.currentObjects[paneId] = co;

    swapCurrentObjects = () => {

        this.parameterCache.swap();
        this.objectEditCache.swap();
        this.transientCache.swap();
        const [, p1, p2] = this.currentObjects;
        this.currentObjects[1] = p2;
        this.currentObjects[2] = p1;

    }

    getError = () => this.currentError;

    setError = (e: ErrorWrapper) => this.currentError = e;

    getPreviousUrl = () => this.previousUrl;

    setPreviousUrl = (url: string) => this.previousUrl = url;

    private doPrompt = (field: Ro.IField, id: string, searchTerm: string | null, setupPrompt: (map: Ro.PromptMap) => void, objectValues: () => Dictionary<Ro.Value>, digest?: string | null) => {
        const promptMap = field.getPromptMap() as Ro.PromptMap; // not null
        promptMap.setMembers(objectValues);
        setupPrompt(promptMap);
        const addEmptyOption = field.entryType() !== Ro.EntryType.AutoComplete && field.extensions().optional();
        return this.repLoader.retrieve(promptMap, Ro.PromptRepresentation, digest).then((p: Ro.PromptRepresentation) => p.choices(addEmptyOption));
    }

    autoComplete = (field: Ro.IField, id: string, objectValues: () => Dictionary<Ro.Value>, searchTerm: string, digest?: string | null) =>
        this.doPrompt(field, id, searchTerm, (promptMap: Ro.PromptMap) => promptMap.setSearchTerm(searchTerm), objectValues, digest)

    conditionalChoices = (field: Ro.IField, id: string, objectValues: () => Dictionary<Ro.Value>, args: Dictionary<Ro.Value>, digest?: string | null) =>
        this.doPrompt(field, id, null, (promptMap: Ro.PromptMap) => promptMap.setArguments(args), objectValues, digest)

    setResult = (action: Ro.ActionRepresentation | Ro.InvokableActionMember, result: Ro.ActionResultRepresentation, fromPaneId: number, toPaneId: number, page: number, pageSize: number) => {

        if (!result.result().isNull()) {
            if (result.resultType() === 'object') {

                const resultObject = result.result().object()!;
                resultObject.keySeparator = this.keySeparator;

                if (resultObject.persistLink()) {
                    // transient object
                    const domainType = resultObject.extensions().domainType()!;
                    resultObject.wrapped().domainType = domainType;
                    resultObject.wrapped().instanceId = (this.nextTransientId++).toString();

                    resultObject.hateoasUrl = `/${domainType}/${this.nextTransientId}`;

                    // copy the etag down into the object
                    resultObject.etagDigest = result.etagDigest;

                    this.setObject(toPaneId, resultObject);
                    this.transientCache.add(toPaneId, resultObject);
                    this.urlManager.pushUrlState(toPaneId);

                    const interactionMode = resultObject.extensions().interactionMode() === 'transient'
                        ? InteractionMode.Transient
                        : InteractionMode.NotPersistent;
                    this.urlManager.setObjectWithMode(resultObject, interactionMode, toPaneId);
                } else if (resultObject.selfLink()) {

                    const selfLink = resultObject.selfLink() as Ro.Link;
                    // persistent object
                    // set the object here and then update the url. That should reload the page but pick up this object
                    // so we don't hit the server again.

                    // copy the etag down into the object
                    resultObject.etagDigest = result.etagDigest;

                    this.setObject(toPaneId, resultObject);

                    // update angular cache
                    const url = `${selfLink.href()}?${Constants.roInlinePropertyDetails}=false`;
                    this.repLoader.addToCache(url, resultObject.wrapped());

                    // if render in edit must be  a form
                    if (resultObject.extensions().interactionMode() === 'form') {
                        this.urlManager.pushUrlState(toPaneId);
                        this.urlManager.setObjectWithMode(resultObject, InteractionMode.Form, toPaneId);
                    } else {
                        this.cacheRecentlyViewed(resultObject);
                        this.urlManager.setObject(resultObject, toPaneId);
                    }
                } else {
                    this.loggerService.throw('ContextService:setResult result object without self or persist link');
                }
            } else if (result.resultType() === 'list') {

                const resultList = result.result().list()!;
                const parms = this.parameterCache.getValues(action.actionId(), fromPaneId);
                const search = this.urlManager.setList(action, parms, fromPaneId, toPaneId);
                const index = this.urlManager.getListCacheIndexFromSearch(search, toPaneId, page, pageSize);
                this.cacheList(resultList, index);
            }
        } else if (result.resultType() === 'void') {
            this.urlManager.triggerPageReloadByFlippingReloadFlagInUrl(fromPaneId);
        }
    }

    incPendingPotentActionOrReload(paneId: Pane) {
        const count = this.pendingPotentActionCount[paneId]! + 1;
        this.pendingPotentActionCount[paneId] = count;
    }

    decPendingPotentActionOrReload(paneId: Pane) {
        let count = this.pendingPotentActionCount[paneId]! - 1;

        if (count < 0) {  // should never happen
            count = 0;
            this.loggerService.warn('ContextService:decPendingPotentActionOrReload count less than 0');
        }
        this.pendingPotentActionCount[paneId] = count;
    }

    isPendingPotentActionOrReload(paneId: Pane) {
        return this.pendingPotentActionCount[paneId]! > 0;
    }

    private setMessages(result: Ro.ActionResultRepresentation) {
        this.pendingClearMessages = this.pendingClearWarnings = false;

        const warnings = result.extensions().warnings() || [];
        const messages = result.extensions().messages() || [];

        this.warningsSource.next(warnings);
        this.messagesSource.next(messages);
    }

    setConcurrencyError(oid: Ro.ObjectIdWrapper) {
        this.concurrencyErrorSource.next(oid);
    }

    private invokeActionInternal(
        invokeMap: Ro.InvokeMap,
        action: Ro.ActionRepresentation | Ro.InvokableActionMember,
        fromPaneId: number,
        toPaneId: number,
        setDirty: () => void,
        gotoResult: boolean = false) {

        invokeMap.setUrlParameter(Constants.roInlinePropertyDetails, false);

        if (action.extensions().returnType() === 'list' && action.extensions().renderEagerly()) {
            invokeMap.setUrlParameter(Constants.roInlineCollectionItems, true);
        }

        return this.repLoader.retrieve(invokeMap, Ro.ActionResultRepresentation, action.parent.etagDigest)
            .then((result: Ro.ActionResultRepresentation) => {
                setDirty();
                this.setMessages(result);
                if (gotoResult) {
                    this.setResult(action, result, fromPaneId, toPaneId, 1, this.configService.config.defaultPageSize);
                }
                return result;
            });
    }

    private getSetDirtyFunction(action: Ro.ActionRepresentation | Ro.InvokableActionMember, parms: Dictionary<Ro.Value>) {
        const parent = action.parent;

        if (action.isNotQueryOnly()) {

            const clearCacheIfNecessary = this.configService.config.clearCacheOnChange
                ? () => this.markDirtyAfterChange()
                : () => { };

            if (parent instanceof Ro.DomainObjectRepresentation) {
                return () => {
                    this.dirtyList.setDirty(parent.getOid());
                    this.setCurrentObjectsDirty();
                    clearCacheIfNecessary();
                };
            }
            if (parent instanceof Ro.CollectionRepresentation) {
                return () => {
                    const selfLink = parent.selfLink();
                    const oid = Ro.ObjectIdWrapper.fromLink(selfLink, this.keySeparator);
                    this.dirtyList.setDirty(oid);
                    this.setCurrentObjectsDirty();
                    clearCacheIfNecessary();
                };
            }
            if (parent instanceof Ro.CollectionMember) {
                return () => {
                    const memberParent = parent.parent;
                    if (memberParent instanceof Ro.DomainObjectRepresentation) {
                        this.dirtyList.setDirty(memberParent.getOid());
                    }
                    this.setCurrentObjectsDirty();
                    clearCacheIfNecessary();
                };
            }
            if (parent instanceof Ro.ListRepresentation && parms) {

                const ccaParm = find(action.parameters(), p => p.isCollectionContributed());
                const ccaId = ccaParm ? ccaParm.id() : null;
                const ccaValue = ccaId ? parms[ccaId] : null;

                // this should always be true
                if (ccaValue && ccaValue.isList()) {

                    const refValues = filter(ccaValue.list()!, v => v.isReference());
                    const links = map(refValues, v => v.link()!);
                    return () => {
                        forEach(links, (l: Ro.Link) => this.dirtyList.setDirty(l.getOid(this.keySeparator)));
                        this.setCurrentObjectsDirty();
                        clearCacheIfNecessary();
                    };
                }
            }

            return () => {
                this.setCurrentObjectsDirty();
                clearCacheIfNecessary();
            };
        }

        return () => { };
    }

    invokeAction = (action: Ro.ActionRepresentation | Ro.InvokableActionMember, parms: Dictionary<Ro.Value>, fromPaneId = 1, toPaneId = 1, gotoResult: boolean = true) => {

        const invokeOnMap = (iAction: Ro.ActionRepresentation | Ro.InvokableActionMember) => {
            const im = iAction.getInvokeMap() as Ro.InvokeMap;
            each(parms, (parm, k) => im.setParameter(k!, parm));
            const setDirty = this.getSetDirtyFunction(iAction, parms);
            return this.invokeActionInternal(im, iAction, fromPaneId, toPaneId, setDirty, gotoResult);
        };

        return invokeOnMap(action);
    }

    private setNewObject(updatedObject: Ro.DomainObjectRepresentation, paneId: Pane, viewSavedObject: Boolean) {
        this.setObject(paneId, updatedObject);
        this.dirtyList.clearDirty(updatedObject.getOid());

        if (viewSavedObject) {
            this.urlManager.setObject(updatedObject, paneId);
        } else {
            this.urlManager.popUrlState(paneId);
        }
    }

    private setDirtyIfNecessary() {
        if (this.configService.config.clearCacheOnChange) {
            this.markDirtyAfterChange();
            this.setCurrentObjectsDirty();
        }
    }

    updateObject = (object: Ro.DomainObjectRepresentation, props: Dictionary<Ro.Value>, paneId: Pane, viewSavedObject: boolean) => {
        const update = object.getUpdateMap();

        each(props, (v, k) => update.setProperty(k!, v));

        return this.repLoader.retrieve(update, Ro.DomainObjectRepresentation, object.etagDigest)
            .then((updatedObject: Ro.DomainObjectRepresentation) => {
                this.setDirtyIfNecessary();
                // This is a kludge because updated object has no self link.
                const rawLinks = object.wrapped().links;
                updatedObject.wrapped().links = rawLinks;
                this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
    }

    saveObject = (object: Ro.DomainObjectRepresentation, props: Dictionary<Ro.Value>, paneId: Pane, viewSavedObject: boolean) => {
        const persist = object.getPersistMap();

        each(props, (v, k) => persist.setMember(k!, v));

        return this.repLoader.retrieve(persist, Ro.DomainObjectRepresentation, object.etagDigest)
            .then((updatedObject: Ro.DomainObjectRepresentation) => {
                this.setDirtyIfNecessary();
                this.transientCache.remove(paneId, object.domainType()!, object.id());
                this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
    }

    validateUpdateObject = (object: Ro.DomainObjectRepresentation, props: Dictionary<Ro.Value>) => {
        const update = object.getUpdateMap();
        update.setValidateOnly();
        each(props, (v, k) => update.setProperty(k!, v));
        return this.repLoader.validate(update, object.etagDigest);
    }

    validateSaveObject = (object: Ro.DomainObjectRepresentation, props: Dictionary<Ro.Value>) => {
        const persist = object.getPersistMap();
        persist.setValidateOnly();
        each(props, (v, k) => persist.setMember(k!, v));
        return this.repLoader.validate(persist, object.etagDigest);
    }

    isSubTypeOf = (toCheckType: string, againstType: string): Promise<boolean> => {

        if (this.subTypeCache[toCheckType] && typeof this.subTypeCache[toCheckType][againstType] !== 'undefined') {
            return this.subTypeCache[toCheckType][againstType];
        }

        const isSubTypeOf = new Ro.DomainTypeActionInvokeRepresentation(againstType, toCheckType, this.configService.config.appPath);

        const promise = this.repLoader.populate(isSubTypeOf, true)
            .then((updatedObject: Ro.DomainTypeActionInvokeRepresentation) => {
                return updatedObject.value();
            })
            .catch((reject: ErrorWrapper) => {
                return false;
            });

        const entry: Dictionary<Promise<boolean>> = {};
        entry[againstType] = promise;
        this.subTypeCache[toCheckType] = entry;

        return promise;
    }

    private cacheRecentlyViewed(obj: Ro.DomainObjectRepresentation) {
        // never cache forms
        if (obj.extensions().interactionMode() !== 'form') {
            this.recentcache.add(obj);
        }
    }

    getRecentlyViewed = () => this.recentcache.items();

    clearRecentlyViewed = () => {
        // clear both recent view and cached objects

        each(this.recentcache.items(), i => this.dirtyList.setDirty(i.getOid()));
        this.recentcache.clear();
    }

    private markDirtyAfterChange = () => {
        each(this.recentcache.items(), i => this.dirtyList.setDirty(i.getOid()));
        this.currentLists = {};
    }

    private setCurrentObjectsDirty = () => {
        const pane1Obj = this.currentObjects[Pane.Pane1];
        const pane2Obj = this.currentObjects[Pane.Pane2];
        const setDirty = (m: Ro.DomainObjectRepresentation | null | undefined) => {
            if (m) {
                this.dirtyList.setDirty(m.getOid());
            }
        };
        setDirty(pane1Obj);
        setDirty(pane2Obj);
    }

    private logoff() {
        for (let pane = 1; pane <= 2; pane++) {
            delete this.currentObjects[pane];
        }
        this.currentServices = null;
        this.currentMenus = null;
        this.currentVersion = null;
        this.currentUser = null;
        this.transientCache.clear();
        this.recentcache.clear();
        this.dirtyList.clear();

        // k will always be defined
        forEach(this.currentMenuList, (v, k) => delete this.currentMenuList[k!]);
        forEach(this.currentLists, (v, k) => delete this.currentLists[k!]);
    }

    cacheFieldValue = (dialogId: string, pid: string, pv: Ro.Value, paneId: Pane = Pane.Pane1) => {
        this.parameterCache.addValue(dialogId, pid, pv, paneId);
    }

    getDialogCachedValues = (dialogId: string | null = null, paneId: Pane = Pane.Pane1) => {
        return this.parameterCache.getValues(dialogId, paneId);
    }

    getObjectCachedValues = (objectId: string | null = null, paneId: Pane = Pane.Pane1) => {
        return this.objectEditCache.getValues(objectId, paneId);
    }

    clearDialogCachedValues = (paneId: Pane = Pane.Pane1) => {
        this.parameterCache.clear(paneId);
    }

    clearObjectCachedValues = (paneId: Pane = Pane.Pane1) => {
        this.objectEditCache.clear(paneId);
    }

    cachePropertyValue = (obj: Ro.DomainObjectRepresentation, p: Ro.PropertyMember, pv: Ro.Value, paneId: Pane = Pane.Pane1) => {
        this.dirtyList.setDirty(obj.getOid());
        this.objectEditCache.addValue(obj.id(), p.id(), pv, paneId);
    }
}
