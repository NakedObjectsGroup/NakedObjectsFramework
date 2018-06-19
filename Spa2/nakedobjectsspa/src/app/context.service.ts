import { InteractionMode, PaneRouteData, Pane, CollectionViewState } from './route-data';
import { UrlManagerService } from './url-manager.service';
import { RepLoaderService } from './rep-loader.service';
import { Injectable } from '@angular/core';
import * as Constants from './constants';
import * as Models from './models';
import { Subject ,  Observable } from 'rxjs';
import { IDraggableViewModel } from './view-models/idraggable-view-model';
import { ConfigService } from './config.service';
import { LoggerService } from './logger.service';
import { Dictionary } from 'lodash';
import each from 'lodash-es/each';
import find from 'lodash-es/find';
import filter from 'lodash-es/filter';
import map from 'lodash-es/map';
import forEach from 'lodash-es/forEach';
import keys from 'lodash-es/keys';
import findKey from 'lodash-es/findKey';
import first from 'lodash-es/first';
import omit from 'lodash-es/omit';
import remove from 'lodash-es/remove';
import sortBy from 'lodash-es/sortBy';

enum DirtyState {
    DirtyMustReload = 1,
    DirtyMayReload = 2,
    Clean = 3
}

class DirtyList {
    private dirtyObjects: Dictionary<DirtyState> = {};

    setDirty(oid: Models.ObjectIdWrapper, alwaysReload: boolean = false) {
        this.setDirtyInternal(oid, alwaysReload ? DirtyState.DirtyMustReload : DirtyState.DirtyMayReload);
    }

    setDirtyInternal(oid: Models.ObjectIdWrapper, dirtyState: DirtyState) {
        const key = oid.getKey();
        this.dirtyObjects[key] = dirtyState;
    }

    getDirty(oid: Models.ObjectIdWrapper) {
        const key = oid.getKey();
        return this.dirtyObjects[key] || DirtyState.Clean;
    }

    clearDirty(oid: Models.ObjectIdWrapper) {
        const key = oid.getKey();
        this.dirtyObjects = omit(this.dirtyObjects, key) as Dictionary<DirtyState>;
    }

    clear() {
        this.dirtyObjects = {};
    }
}

function isSameObject(object: Models.DomainObjectRepresentation | null | undefined, type: string, id?: string) {
    if (object) {
        const sid = object.serviceId();
        return sid ? sid === type : (object.domainType() === type && object.instanceId() === Models.withNull(id));
    }
    return false;
}

class TransientCache {
    private transientCache: [undefined, Models.DomainObjectRepresentation[], Models.DomainObjectRepresentation[]] = [undefined, [], []]; // per pane

    constructor(private readonly depth: number) { }

    add(paneId: Pane, obj: Models.DomainObjectRepresentation) {
        let paneObjects = this.transientCache[paneId]!;
        if (paneObjects.length >= this.depth) {
            paneObjects = paneObjects.slice(-(this.depth - 1));
        }
        paneObjects.push(obj);
        this.transientCache[paneId] = paneObjects;
    }

    get(paneId: Pane, type: string, id: string): Models.DomainObjectRepresentation | null {
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

    private recentCache: Models.DomainObjectRepresentation[] = [];

    add(obj: Models.DomainObjectRepresentation) {

        // find any matching entries and remove them - should only be one
        remove(this.recentCache, i => i.id() === obj.id());

        // push obj on top of array
        this.recentCache = [obj].concat(this.recentCache);

        // drop oldest if we're full
        if (this.recentCache.length > this.depth) {
            this.recentCache = this.recentCache.slice(0, this.depth);
        }
    }

    items(): Models.DomainObjectRepresentation[] {
        return this.recentCache;
    }

    clear() {
        this.recentCache = [];
    }
}

class ValueCache {

    private currentValues: [undefined, Dictionary<Models.Value>, Dictionary<Models.Value>] = [undefined, {}, {}];
    private currentId: [undefined, string, string] = [undefined, "", ""];

    addValue(id: string, valueId: string, value: Models.Value, paneId: Pane) {
        if (this.currentId[paneId] !== id) {
            this.currentId[paneId] = id;
            this.currentValues[paneId] = {};
        }

        this.currentValues[paneId]![valueId] = new Models.Value(value);
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

        return this.currentValues[paneId] as Dictionary<Models.Value>;
    }

    clear(paneId: Pane) {
        this.currentId[paneId] = "";
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
    private currentError: Models.ErrorWrapper | null = null;
    private previousUrl: string | null = null;
    private warningsSource = new Subject<string[]>();
    private messagesSource = new Subject<string[]>();

    warning$ = this.warningsSource.asObservable();
    messages$ = this.messagesSource.asObservable();

    private pendingPotentActionCount: [undefined, number, number] = [undefined, 0, 0];

    private copiedViewModelSource = new Subject<IDraggableViewModel>();

    copiedViewModel$ = this.copiedViewModelSource.asObservable();

    private copiedViewModel: IDraggableViewModel | null;

    private concurrencyErrorSource = new Subject<Models.ObjectIdWrapper>();

    concurrencyError$ = this.concurrencyErrorSource.asObservable();

    private subTypeCache: Dictionary<Dictionary<Promise<boolean>>> = {};

    private get keySeparator() {
        return this.configService.config.keySeparator;
    }

    // cached values

    private currentObjects: [undefined, Models.DomainObjectRepresentation | null, Models.DomainObjectRepresentation | null] = [undefined, null, null]; // per pane
    private transientCache = new TransientCache(this.configService.config.transientCacheDepth);

    private currentMenuList: Dictionary<Models.MenuRepresentation> = {};
    private currentServices: Models.DomainServicesRepresentation | null = null;
    private currentMenus: Models.MenusRepresentation | null = null;
    private currentVersion: Models.VersionRepresentation | null = null;
    private currentUser: Models.UserRepresentation | null = null;

    private readonly recentcache = new RecentCache(this.keySeparator, this.configService.config.recentCacheDepth);
    private readonly dirtyList = new DirtyList();
    private currentLists: Dictionary<{ list: Models.ListRepresentation; added: number }> = {};
    private readonly parameterCache = new ValueCache();
    private readonly objectEditCache = new ValueCache();

    getFile = (object: Models.DomainObjectRepresentation, url: string, mt: string) => {
        const isDirty = this.getIsDirty(object.getOid());
        return this.repLoader.getFile(url, mt, isDirty);
    }

    setFile = (object: Models.DomainObjectRepresentation, url: string, mt: string, file: Blob) => this.repLoader.uploadFile(url, mt, file);

    clearCachedFile = (url: string) => this.repLoader.clearCache(url);

    clearCachedCollections(obj: Models.DomainObjectRepresentation) {

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
    getDomainObject = (paneId: Pane, oid: Models.ObjectIdWrapper, interactionMode: InteractionMode): Promise<Models.DomainObjectRepresentation> => {
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
            const p: Promise<Models.DomainObjectRepresentation> = transientObj
                ? Promise.resolve(transientObj)
                : Promise.reject(new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.ExpiredTransient, ""));
            return p;
        }

        const object = new Models.DomainObjectRepresentation();
        object.hateoasUrl = `${this.configService.config.appPath}/objects/${type}/${id}`;
        object.setInlinePropertyDetails(interactionMode === InteractionMode.Edit);

        this.incPendingPotentActionOrReload(paneId);
        return this.repLoader.populate<Models.DomainObjectRepresentation>(object, forceReload)
            .then((obj: Models.DomainObjectRepresentation) => {
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

    private editOrReloadObject(paneId: Pane, object: Models.DomainObjectRepresentation, inlineDetails: boolean) {
        const parms: Dictionary<Object> = {};
        parms[Constants.roInlinePropertyDetails] = inlineDetails;

        return this.repLoader.retrieveFromLink<Models.DomainObjectRepresentation>(object.selfLink(), parms)
            .then(obj => {
                this.currentObjects[paneId] = obj;
                const oid = obj.getOid();
                this.dirtyList.clearDirty(oid);
                this.cacheRecentlyViewed(obj);
                return Promise.resolve(obj);
            });
    }

    getIsDirty = (oid: Models.ObjectIdWrapper) => !oid.isService && this.dirtyList.getDirty(oid) !== DirtyState.Clean;

    mustReload = (oid: Models.ObjectIdWrapper) => {
        const dirtyState = this.dirtyList.getDirty(oid);
        return (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && this.configService.config.autoLoadDirty);
    }

    getObjectForEdit = (paneId: Pane, object: Models.DomainObjectRepresentation) => this.editOrReloadObject(paneId, object, true);

    reloadObject = (paneId: Pane, object: Models.DomainObjectRepresentation) => this.editOrReloadObject(paneId, object, false);

    getService = (paneId: Pane, serviceType: string): Promise<Models.DomainObjectRepresentation> => {

        if (isSameObject(this.currentObjects[paneId], serviceType)) {
            return Promise.resolve(this.currentObjects[paneId]!);
        }

        return this.getServices()
            .then((services: Models.DomainServicesRepresentation) => {
                const service = services.getService(serviceType);
                if (service) {
                    return this.repLoader.populate(service);
                }
                return Promise.reject(`unknown service ${serviceType}`);
            })
            .then((service: Models.DomainObjectRepresentation) => {
                this.currentObjects[paneId] = service;
                return Promise.resolve(service);
            });
    }

    getActionDetails = (actionMember: Models.ActionMember): Promise<Models.ActionRepresentation> => {
        const details = actionMember.getDetails();
        if (details) {
            return this.repLoader.populate(details, true);
        }
        return Promise.reject(`Couldn't find details on ${actionMember.actionId()}`);
    }

    getCollectionDetails = (collectionMember: Models.CollectionMember, state: CollectionViewState, ignoreCache: boolean): Promise<Models.CollectionRepresentation> => {
        const details = collectionMember.getDetails();

        if (details) {
            if (state === CollectionViewState.Table) {
                details.setUrlParameter(Constants.roInlineCollectionItems, true);
            }
            const parent = collectionMember.parent;
            let isDirty = false;
            if (parent instanceof Models.DomainObjectRepresentation) {
                const oid = parent.getOid();
                isDirty = this.dirtyList.getDirty(oid) !== DirtyState.Clean;
            }

            return this.repLoader.populate(details, isDirty || ignoreCache);
        }
        return Promise.reject(`Couldn't find details on ${collectionMember.collectionId()}`);
    }

    getInvokableAction = (action: Models.ActionMember | Models.ActionRepresentation): Promise<Models.InvokableActionMember | Models.ActionRepresentation> => {

        if (action instanceof Models.InvokableActionMember || action instanceof Models.ActionRepresentation) {
            return Promise.resolve(action);
        }

        return this.getActionDetails(action);
    }

    getMenu = (menuId: string): Promise<Models.MenuRepresentation> => {

        if (this.currentMenuList[menuId]) {
            return Promise.resolve(this.currentMenuList[menuId]);
        }

        return this.getMenus()
            .then((menus: Models.MenusRepresentation) => {
                const menu = menus.getMenu(menuId);
                if (menu) {
                    return this.repLoader.populate(menu);
                }
                return Promise.reject(`couldn't find menu ${menuId}`);
            })
            .then((menu: Models.MenuRepresentation) => {
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
        return this.repLoader.populate<Models.HomePageRepresentation>(new Models.HomePageRepresentation({}, this.configService.config.appPath));
    }

    getServices = () => {

        if (this.currentServices) {
            return Promise.resolve(this.currentServices);
        }

        return this.getHome()
            .then((home: Models.HomePageRepresentation) => {
                const ds = home.getDomainServices();
                return this.repLoader.populate<Models.DomainServicesRepresentation>(ds);
            })
            .then((services: Models.DomainServicesRepresentation) => {
                this.currentServices = services;
                return Promise.resolve(services);
            });
    }

    getMenus = () => {
        if (this.currentMenus) {
            return Promise.resolve(this.currentMenus);
        }

        return this.getHome()
            .then((home: Models.HomePageRepresentation) => {
                const ds = home.getMenus();
                return this.repLoader.populate<Models.MenusRepresentation>(ds);
            })
            .then((menus: Models.MenusRepresentation) => {
                this.currentMenus = menus;
                return Promise.resolve(this.currentMenus);
            });
    }

    getVersion = () => {

        if (this.currentVersion) {
            return Promise.resolve(this.currentVersion);
        }

        return this.getHome()
            .then((home: Models.HomePageRepresentation) => {
                const v = home.getVersion();
                return this.repLoader.populate<Models.VersionRepresentation>(v);
            })
            .then((version: Models.VersionRepresentation) => {
                this.currentVersion = version;
                return Promise.resolve(version);
            });
    }

    getUser = () => {

        if (this.currentUser) {
            return Promise.resolve(this.currentUser);
        }

        return this.getHome()
            .then((home: Models.HomePageRepresentation) => {
                const u = home.getUser();
                return this.repLoader.populate<Models.UserRepresentation>(u);
            })
            .then((user: Models.UserRepresentation) => {
                this.currentUser = user;
                return Promise.resolve(user);
            });
    }

    getObject = (paneId: Pane, oid: Models.ObjectIdWrapper, interactionMode: InteractionMode) => {
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

    private cacheList(list: Models.ListRepresentation, index: string) {

        const entry = this.currentLists[index];
        if (entry) {
            entry.list = list;
            entry.added = Date.now();
        } else {

            if (keys(this.currentLists).length >= this.configService.config.listCacheSize) {
                // delete oldest;
                // TODO this looks wrong surely just "added" test !
                // Fix "!"
                const oldest = first(sortBy(this.currentLists, "e.added"))!.added;
                const oldestIndex = findKey(this.currentLists, (e: { added: number }) => e.added === oldest);
                if (oldestIndex) {
                    delete this.currentLists[oldestIndex];
                }
            }

            this.currentLists[index] = { list: list, added: Date.now() };
        }
    }

    private handleResult = (paneId: Pane, result: Models.ActionResultRepresentation, page: number, pageSize: number): Promise<Models.ListRepresentation> => {

        if (result.resultType() === "list") {
            const resultList = result.result().list() as Models.ListRepresentation; // not null
            const index = this.urlManager.getListCacheIndex(paneId, page, pageSize);
            this.cacheList(resultList, index);
            return Promise.resolve(resultList);
        } else {
            return Promise.reject(new Models.ErrorWrapper(Models.ErrorCategory.ClientError, Models.ClientErrorCode.WrongType, "expect list"));
        }
    }

    private getList = (paneId: Pane, resultPromise: () => Promise<Models.ActionResultRepresentation>, page: number, pageSize: number) => {
        return resultPromise().then(result => this.handleResult(paneId, result, page, pageSize));
    }

    getActionExtensionsFromMenu = (menuId: string, actionId: string) =>
        this.getMenu(menuId)
        .then(menu => Promise.resolve(menu.actionMember(actionId).extensions()))

    getActionExtensionsFromObject = (paneId: Pane, oid: Models.ObjectIdWrapper, actionId: string) =>
        this.getObject(paneId, oid, InteractionMode.View).then(object => Promise.resolve(object.actionMember(actionId).extensions()))

    private getPagingParms(page: number, pageSize: number): Dictionary<Object> {
        return (page && pageSize) ? { "x-ro-page": page, "x-ro-pageSize": pageSize } : {};
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
        const oid = Models.ObjectIdWrapper.fromObjectId(objectId, this.keySeparator);
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

    setObject = (paneId: Pane, co: Models.DomainObjectRepresentation) => this.currentObjects[paneId] = co;

    swapCurrentObjects = () => {

        this.parameterCache.swap();
        this.objectEditCache.swap();
        this.transientCache.swap();
        const [, p1, p2] = this.currentObjects;
        this.currentObjects[1] = p2;
        this.currentObjects[2] = p1;

    }

    getError = () => this.currentError;

    setError = (e: Models.ErrorWrapper) => this.currentError = e;

    getPreviousUrl = () => this.previousUrl;

    setPreviousUrl = (url: string) => this.previousUrl = url;

    private doPrompt = (field: Models.IField, id: string, searchTerm: string | null, setupPrompt: (map: Models.PromptMap) => void, objectValues: () => Dictionary<Models.Value>, digest?: string | null) => {
        const promptMap = field.getPromptMap() as Models.PromptMap; // not null
        promptMap.setMembers(objectValues);
        setupPrompt(promptMap);
        const addEmptyOption = field.entryType() !== Models.EntryType.AutoComplete && field.extensions().optional();
        return this.repLoader.retrieve(promptMap, Models.PromptRepresentation, digest).then((p: Models.PromptRepresentation) => p.choices(addEmptyOption));
    }

    autoComplete = (field: Models.IField, id: string, objectValues: () => Dictionary<Models.Value>, searchTerm: string, digest?: string | null) =>
        this.doPrompt(field, id, searchTerm, (promptMap: Models.PromptMap) => promptMap.setSearchTerm(searchTerm), objectValues, digest)

    conditionalChoices = (field: Models.IField, id: string, objectValues: () => Dictionary<Models.Value>, args: Dictionary<Models.Value>, digest?: string | null) =>
        this.doPrompt(field, id, null, (promptMap: Models.PromptMap) => promptMap.setArguments(args), objectValues, digest)

    setResult = (action: Models.ActionRepresentation | Models.InvokableActionMember, result: Models.ActionResultRepresentation, fromPaneId: number, toPaneId: number, page: number, pageSize: number) => {

        if (!result.result().isNull()) {
            if (result.resultType() === "object") {

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

                    const interactionMode = resultObject.extensions().interactionMode() === "transient"
                        ? InteractionMode.Transient
                        : InteractionMode.NotPersistent;
                    this.urlManager.setObjectWithMode(resultObject, interactionMode, toPaneId);
                } else if (resultObject.selfLink()) {

                    const selfLink = resultObject.selfLink() as Models.Link;
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
                    if (resultObject.extensions().interactionMode() === "form") {
                        this.urlManager.pushUrlState(toPaneId);
                        this.urlManager.setObjectWithMode(resultObject, InteractionMode.Form, toPaneId);
                    } else {
                        this.cacheRecentlyViewed(resultObject);
                        this.urlManager.setObject(resultObject, toPaneId);
                    }
                } else {
                    this.loggerService.throw("ContextService:setResult result object without self or persist link");
                }
            } else if (result.resultType() === "list") {

                const resultList = result.result().list()!;
                const parms = this.parameterCache.getValues(action.actionId(), fromPaneId);
                const search = this.urlManager.setList(action, parms, fromPaneId, toPaneId);
                const index = this.urlManager.getListCacheIndexFromSearch(search, toPaneId, page, pageSize);
                this.cacheList(resultList, index);
            }
        } else if (result.resultType() === "void") {
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
            this.loggerService.warn("ContextService:decPendingPotentActionOrReload count less than 0");
        }
        this.pendingPotentActionCount[paneId] = count;
    }

    isPendingPotentActionOrReload(paneId: Pane) {
        return this.pendingPotentActionCount[paneId]! > 0;
    }

    private setMessages(result: Models.ActionResultRepresentation) {
        this.pendingClearMessages = this.pendingClearWarnings = false;

        const warnings = result.extensions().warnings() || [];
        const messages = result.extensions().messages() || [];

        this.warningsSource.next(warnings);
        this.messagesSource.next(messages);
    }

    setCopyViewModel(dvm: IDraggableViewModel | null) {
        this.copiedViewModel = dvm;
        this.copiedViewModelSource.next(Models.withUndefined(dvm));
    }

    getCopyViewModel() {
        return this.copiedViewModel;
    }

    setConcurrencyError(oid: Models.ObjectIdWrapper) {
        this.concurrencyErrorSource.next(oid);
    }

    private invokeActionInternal(
        invokeMap: Models.InvokeMap,
        action: Models.ActionRepresentation | Models.InvokableActionMember,
        fromPaneId: number,
        toPaneId: number,
        setDirty: () => void,
        gotoResult: boolean = false) {

        invokeMap.setUrlParameter(Constants.roInlinePropertyDetails, false);

        if (action.extensions().returnType() === "list" && action.extensions().renderEagerly()) {
            invokeMap.setUrlParameter(Constants.roInlineCollectionItems, true);
        }

        return this.repLoader.retrieve(invokeMap, Models.ActionResultRepresentation, action.parent.etagDigest)
            .then((result: Models.ActionResultRepresentation) => {
                setDirty();
                this.setMessages(result);
                if (gotoResult) {
                    this.setResult(action, result, fromPaneId, toPaneId, 1, this.configService.config.defaultPageSize);
                }
                return result;
            });
    }

    private getSetDirtyFunction(action: Models.ActionRepresentation | Models.InvokableActionMember, parms: Dictionary<Models.Value>) {
        const parent = action.parent;

        if (action.isNotQueryOnly()) {

            const setCurrentObjectsDirty = () => {
                const pane1Obj = this.currentObjects[Pane.Pane1];
                const pane2Obj = this.currentObjects[Pane.Pane2];
                const setDirty = (m: Models.DomainObjectRepresentation | null | undefined) => {
                    if (m) {
                        this.dirtyList.setDirty(m.getOid());
                    }
                };
                setDirty(pane1Obj);
                setDirty(pane2Obj);
            };

            if (parent instanceof Models.DomainObjectRepresentation) {
                return () => {
                    this.dirtyList.setDirty(parent.getOid());
                    setCurrentObjectsDirty();
                };
            }
            if (parent instanceof Models.CollectionRepresentation) {
                return () => {
                    const selfLink = parent.selfLink();
                    const oid = Models.ObjectIdWrapper.fromLink(selfLink, this.keySeparator);
                    this.dirtyList.setDirty(oid);
                    setCurrentObjectsDirty();
                };
            }
            if (parent instanceof Models.CollectionMember) {
                return () => {
                    const memberParent = parent.parent;
                    if (memberParent instanceof Models.DomainObjectRepresentation) {
                        this.dirtyList.setDirty(memberParent.getOid());
                    }
                    setCurrentObjectsDirty();
                };
            }
            if (parent instanceof Models.ListRepresentation && parms) {

                const ccaParm = find(action.parameters(), p => p.isCollectionContributed());
                const ccaId = ccaParm ? ccaParm.id() : null;
                const ccaValue = ccaId ? parms[ccaId] : null;

                // this should always be true
                if (ccaValue && ccaValue.isList()) {

                    const refValues = filter(ccaValue.list()!, v => v.isReference());
                    const links = map(refValues, v => v.link()!);
                    return () => {
                        forEach(links, (l: Models.Link) => this.dirtyList.setDirty(l.getOid(this.keySeparator)));
                        setCurrentObjectsDirty();
                    };
                }
            }

            return setCurrentObjectsDirty;
        }

        return () => { };
    }

    invokeAction = (action: Models.ActionRepresentation | Models.InvokableActionMember, parms: Dictionary<Models.Value>, fromPaneId = 1, toPaneId = 1, gotoResult: boolean = true) => {

        const invokeOnMap = (iAction: Models.ActionRepresentation | Models.InvokableActionMember) => {
            const im = iAction.getInvokeMap() as Models.InvokeMap;
            each(parms, (parm, k) => im.setParameter(k!, parm));
            const setDirty = this.getSetDirtyFunction(iAction, parms);
            return this.invokeActionInternal(im, iAction, fromPaneId, toPaneId, setDirty, gotoResult);
        };

        return invokeOnMap(action);
    }

    private setNewObject(updatedObject: Models.DomainObjectRepresentation, paneId: Pane, viewSavedObject: Boolean) {
        this.setObject(paneId, updatedObject);
        this.dirtyList.clearDirty(updatedObject.getOid());

        if (viewSavedObject) {
            this.urlManager.setObject(updatedObject, paneId);
        } else {
            this.urlManager.popUrlState(paneId);
        }
    }

    updateObject = (object: Models.DomainObjectRepresentation, props: Dictionary<Models.Value>, paneId: Pane, viewSavedObject: boolean) => {
        const update = object.getUpdateMap();

        each(props, (v, k) => update.setProperty(k!, v));

        return this.repLoader.retrieve(update, Models.DomainObjectRepresentation, object.etagDigest)
            .then((updatedObject: Models.DomainObjectRepresentation) => {
                // This is a kludge because updated object has no self link.
                const rawLinks = object.wrapped().links;
                updatedObject.wrapped().links = rawLinks;
                this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
    }

    saveObject = (object: Models.DomainObjectRepresentation, props: Dictionary<Models.Value>, paneId: Pane, viewSavedObject: boolean) => {
        const persist = object.getPersistMap();

        each(props, (v, k) => persist.setMember(k!, v));

        return this.repLoader.retrieve(persist, Models.DomainObjectRepresentation, object.etagDigest)
            .then((updatedObject: Models.DomainObjectRepresentation) => {
                this.transientCache.remove(paneId, object.domainType()!, object.id());
                this.setNewObject(updatedObject, paneId, viewSavedObject);
                return Promise.resolve(updatedObject);
            });
    }

    validateUpdateObject = (object: Models.DomainObjectRepresentation, props: Dictionary<Models.Value>) => {
        const update = object.getUpdateMap();
        update.setValidateOnly();
        each(props, (v, k) => update.setProperty(k!, v));
        return this.repLoader.validate(update, object.etagDigest);
    }

    validateSaveObject = (object: Models.DomainObjectRepresentation, props: Dictionary<Models.Value>) => {
        const persist = object.getPersistMap();
        persist.setValidateOnly();
        each(props, (v, k) => persist.setMember(k!, v));
        return this.repLoader.validate(persist, object.etagDigest);
    }

    isSubTypeOf = (toCheckType: string, againstType: string): Promise<boolean> => {

        if (this.subTypeCache[toCheckType] && typeof this.subTypeCache[toCheckType][againstType] !== "undefined") {
            return this.subTypeCache[toCheckType][againstType];
        }

        const isSubTypeOf = new Models.DomainTypeActionInvokeRepresentation(againstType, toCheckType, this.configService.config.appPath);

        const promise = this.repLoader.populate(isSubTypeOf, true)
            .then((updatedObject: Models.DomainTypeActionInvokeRepresentation) => {
                return updatedObject.value();
            })
            .catch((reject: Models.ErrorWrapper) => {
                return false;
            });

        const entry: Dictionary<Promise<boolean>> = {};
        entry[againstType] = promise;
        this.subTypeCache[toCheckType] = entry;

        return promise;
    }

    private cacheRecentlyViewed(obj: Models.DomainObjectRepresentation) {
        // never cache forms
        if (obj.extensions().interactionMode() !== "form") {
            this.recentcache.add(obj);
        }
    }

    getRecentlyViewed = () => this.recentcache.items();

    clearRecentlyViewed = () => {
        // clear both recent view and cached objects

        each(this.recentcache.items(), i => this.dirtyList.setDirty(i.getOid()));
        this.recentcache.clear();
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

    cacheFieldValue = (dialogId: string, pid: string, pv: Models.Value, paneId: Pane = Pane.Pane1) => {
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

    cachePropertyValue = (obj: Models.DomainObjectRepresentation, p: Models.PropertyMember, pv: Models.Value, paneId: Pane = Pane.Pane1) => {
        this.dirtyList.setDirty(obj.getOid());
        this.objectEditCache.addValue(obj.id(), p.id(), pv, paneId);
    }
}
