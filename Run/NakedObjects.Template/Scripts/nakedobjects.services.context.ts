/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />


module NakedObjects {
    import ListRepresentation = Models.ListRepresentation;
    import VersionRepresentation = Models.VersionRepresentation;
    import MenusRepresentation = Models.MenusRepresentation;
    import MenuRepresentation = Models.MenuRepresentation;
    import DomainObjectRepresentation = Models.DomainObjectRepresentation;
    import Value = Models.Value;
    import ActionMember = Models.ActionMember;
    import ErrorWrapper = Models.ErrorWrapper;
    import IField = Models.IField;
    import ActionResultRepresentation = Models.ActionResultRepresentation;
    import Extensions = Models.Extensions;
    import ErrorMap = Models.ErrorMap;
    import ClientErrorCode = Models.ClientErrorCode;
    import HomePageRepresentation = Models.HomePageRepresentation;
    import DomainServicesRepresentation = Models.DomainServicesRepresentation;
    import ErrorCategory = Models.ErrorCategory;
    import PromptMap = Models.PromptMap;
    import PromptRepresentation = Models.PromptRepresentation;
    import InvokeMap = Models.InvokeMap;
    import DomainTypeActionInvokeRepresentation = Models.DomainTypeActionInvokeRepresentation;
    import HttpStatusCode = Models.HttpStatusCode;
    import ActionRepresentation = Models.ActionRepresentation;
    import IInvokableAction = Models.IInvokableAction;
    import CollectionMember = Models.CollectionMember;
    import CollectionRepresentation = Models.CollectionRepresentation;
    import ObjectIdWrapper = Models.ObjectIdWrapper;
    import InvokableActionMember = NakedObjects.Models.InvokableActionMember;
    import UserRepresentation = NakedObjects.Models.UserRepresentation;

    export interface IContext {

        getCachedList: (paneId: number, page: number, pageSize: number) => ListRepresentation;
        clearCachedList: (paneId: number, page: number, pageSize: number) => void;

        getUser: () => ng.IPromise<UserRepresentation>;
        getVersion: () => ng.IPromise<VersionRepresentation>;
        getMenus: () => ng.IPromise<MenusRepresentation>;
        getMenu: (menuId: string) => ng.IPromise<MenuRepresentation>;
        getObject: (paneId: number, oid: ObjectIdWrapper, interactionMode: InteractionMode) => ng.IPromise<DomainObjectRepresentation>;
        getListFromMenu: (paneId: number, routeData: PaneRouteData, page?: number, pageSize?: number) => angular.IPromise<ListRepresentation>;
        getListFromObject: (paneId: number, routeData: PaneRouteData, page?: number, pageSize?: number) => angular.IPromise<ListRepresentation>;

        getActionDetails: (actionMember: ActionMember) => ng.IPromise<ActionRepresentation>;
        getCollectionDetails: (collectionMember: CollectionMember, state: CollectionViewState, ignoreCache: boolean) => ng.IPromise<CollectionRepresentation>;

        getInvokableAction: (actionmember: ActionMember | ActionRepresentation | IInvokableAction) => ng.IPromise<InvokableActionMember | ActionRepresentation>;

        getError: () => ErrorWrapper;
        getPreviousUrl: () => string;

        getIsDirty: (oid: ObjectIdWrapper) => boolean;

        mustReload: (oid: ObjectIdWrapper) => boolean;

        //The object values are only needed on a transient object / editable view model
        autoComplete(field: IField, id: string, objectValues: () => _.Dictionary<Value>, searchTerm: string): ng.IPromise<_.Dictionary<Value>>;
        //The object values are only needed on a transient object / editable view model
        conditionalChoices(field: IField, id: string, objectValues: () => _.Dictionary<Value>, args: _.Dictionary<Value>): ng.IPromise<_.Dictionary<Value>>;

        invokeAction(action: IInvokableAction, paneId: number, parms: _.Dictionary<Value>): ng.IPromise<ActionResultRepresentation>;

        updateObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean): ng.IPromise<DomainObjectRepresentation>;
        saveObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean): ng.IPromise<DomainObjectRepresentation>;

        validateUpdateObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>): ng.IPromise<boolean>;
        validateSaveObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>): ng.IPromise<boolean>;


        reloadObject: (paneId: number, object: DomainObjectRepresentation) => angular.IPromise<DomainObjectRepresentation>;

        getObjectForEdit: (paneId: number, object: DomainObjectRepresentation) => angular.IPromise<DomainObjectRepresentation>;

        setError: (reject: ErrorWrapper) => void;

        isSubTypeOf(toCheckType: string, againstType: string): ng.IPromise<boolean>;

        getActionExtensionsFromMenu: (menuId: string, actionId: string) => angular.IPromise<Extensions>;
        getActionExtensionsFromObject: (paneId: number, oid: ObjectIdWrapper, actionId: string) => angular.IPromise<Extensions>;

        swapCurrentObjects(): void;

        clearMessages(): void;
        clearWarnings(): void;

        handleWrappedError(reject: ErrorWrapper,
            toReload: DomainObjectRepresentation,
            onReload: (updatedObject: DomainObjectRepresentation) => void,
            displayMessages: (em: ErrorMap) => void,
            customClientHandler?: (ec: ClientErrorCode) => boolean): void;

        getRecentlyViewed(): DomainObjectRepresentation[];

        getFile: (object: DomainObjectRepresentation, url: string, mt: string) => angular.IPromise<Blob>;

        setFile: (object: DomainObjectRepresentation, url: string, mt: string, file: Blob) => angular.IPromise<boolean>;

        clearCachedFile: (url: string) => void;
    }

    interface IContextInternal extends IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getDomainObject: (paneId: number, oid: ObjectIdWrapper, interactionMode: InteractionMode) => ng.IPromise<DomainObjectRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getService: (paneId: number, type: string) => ng.IPromise<DomainObjectRepresentation>;
        setObject: (paneId: number, object: DomainObjectRepresentation) => void;
        setResult(action: IInvokableAction, result: ActionResultRepresentation, paneId: number, page: number, pageSize: number): void;
        setPreviousUrl: (url: string) => void;
    }

    enum DirtyState {
        DirtyMustReload,
        DirtyMayReload,
        Clean
    }

    class DirtyList {
        private dirtyObjects: _.Dictionary<DirtyState> = {};

        setDirty(oid: ObjectIdWrapper, alwaysReload: boolean = false) {
            this.setDirtyInternal(oid, alwaysReload ? DirtyState.DirtyMustReload : DirtyState.DirtyMayReload);
        }

        setDirtyInternal(oid: ObjectIdWrapper, dirtyState: DirtyState) {
            const key = oid.getKey();
            this.dirtyObjects[key] = dirtyState;
        }

        getDirty(oid: ObjectIdWrapper) {
            const key = oid.getKey();
            return this.dirtyObjects[key] || DirtyState.Clean;
        }

        clearDirty(oid: ObjectIdWrapper) {
            const key = oid.getKey();
            this.dirtyObjects = _.omit(this.dirtyObjects, key) as _.Dictionary<DirtyState>;
        }

        clear() {
            this.dirtyObjects = {};
        }
    }

    function isSameObject(object: DomainObjectRepresentation, type: string, id?: string) {
        if (object) {
            const sid = object.serviceId();
            return sid ? sid === type : (object.domainType() === type && object.instanceId() === id);
        }
        return false;
    }

    class TransientCache {
        private transientCache: DomainObjectRepresentation[][] = [, [], []]; // per pane 

        private depth = transientCacheDepth;

        add(paneId: number, obj: DomainObjectRepresentation) {
            let paneObjects = this.transientCache[paneId];
            if (paneObjects.length >= this.depth) {
                paneObjects = paneObjects.slice(-(this.depth - 1));
            }
            paneObjects.push(obj);
            this.transientCache[paneId] = paneObjects;
        }

        get(paneId: number, type: string, id: string): DomainObjectRepresentation {
            const paneObjects = this.transientCache[paneId];
            return _.find(paneObjects, o => isSameObject(o, type, id));
        }

        remove(paneId: number, type: string, id: string) {
            let paneObjects = this.transientCache[paneId];
            paneObjects = _.remove(paneObjects, o => isSameObject(o, type, id));
            this.transientCache[paneId] = paneObjects;
        }

        clear() {
            this.transientCache = [, [], []];
        }

    }

    class RecentCache {
        private recentCache: DomainObjectRepresentation[] = [];

        private depth = recentCacheDepth;

        add(obj: DomainObjectRepresentation) {

            // find any matching entries and remove them - should only be one
            _.remove(this.recentCache, i => i.id() === obj.id());

            // push obj on top of array 
            this.recentCache = [obj].concat(this.recentCache);

            // drop oldest if we're full 
            if (this.recentCache.length > this.depth) {
                this.recentCache = this.recentCache.slice(0, this.depth);
            }
        }

        items(): DomainObjectRepresentation[] {
            return this.recentCache;
        }

        clear() {
            this.recentCache = [];
        }
    }

    app.service("context", function ($q: ng.IQService,
        repLoader: IRepLoader,
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        $cacheFactory: ng.ICacheFactoryService,
        $rootScope: ng.IRootScopeService) {
        const context = <IContextInternal>this;

        // cached values

        const currentObjects: DomainObjectRepresentation[] = []; // per pane 
        const transientCache = new TransientCache();

        const currentMenuList: _.Dictionary<MenuRepresentation> = {};
        let currentServices: DomainServicesRepresentation = null;
        let currentMenus: MenusRepresentation = null;
        let currentVersion: VersionRepresentation = null;
        let currentUser: UserRepresentation = null;

        const recentcache = new RecentCache();
        const dirtyList = new DirtyList();
        const currentLists: _.Dictionary<{ list: ListRepresentation; added: number }> = {};

        context.getFile = (object: DomainObjectRepresentation, url: string, mt: string) => {
            const isDirty = context.getIsDirty(object.getOid());
            return repLoader.getFile(url, mt, isDirty);
        }

        context.setFile = (object: DomainObjectRepresentation, url: string, mt: string, file: Blob) => repLoader.uploadFile(url, mt, file);

        context.clearCachedFile = (url: string) => repLoader.clearCache(url);

        // exposed for test mocking
        context.getDomainObject = (paneId: number, oid: ObjectIdWrapper, interactionMode: InteractionMode): ng.IPromise<DomainObjectRepresentation> => {
            const type = oid.domainType;
            const id = oid.instanceId;

            const dirtyState = dirtyList.getDirty(oid);
            const forceReload = (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && autoLoadDirty);

            if (!forceReload && isSameObject(currentObjects[paneId], type, id)) {
                return $q.when(currentObjects[paneId]);
            }

            // deeper cache for transients
            if (interactionMode === InteractionMode.Transient) {
                const transientObj = transientCache.get(paneId, type, id);
                return transientObj ? $q.when(transientObj) : $q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.ExpiredTransient, ""));
            }

            const object = new DomainObjectRepresentation();
            object.hateoasUrl = getAppPath() + "/objects/" + type + "/" + id;
            object.setInlinePropertyDetails(interactionMode === InteractionMode.Edit);

            return repLoader.populate<DomainObjectRepresentation>(object, forceReload).
                then((obj: DomainObjectRepresentation) => {
                    currentObjects[paneId] = obj;
                    if (forceReload) {
                        dirtyList.clearDirty(oid);
                    }
                    addRecentlyViewed(obj);
                    return $q.when(obj);
                });
        };

        function editOrReloadObject(paneId: number, object: DomainObjectRepresentation, inlineDetails: boolean) {
            const parms: _.Dictionary<Object> = {};
            parms[roInlinePropertyDetails] = inlineDetails;

            return repLoader.retrieveFromLink<DomainObjectRepresentation>(object.selfLink(), parms).
                then(obj => {
                    currentObjects[paneId] = obj;
                    const oid = obj.getOid();
                    dirtyList.clearDirty(oid);
                    return $q.when(obj);
                });
        }

        context.getIsDirty = (oid: ObjectIdWrapper) => !oid.isService && dirtyList.getDirty(oid) !== DirtyState.Clean;

        context.mustReload = (oid: ObjectIdWrapper) => {
            const dirtyState = dirtyList.getDirty(oid);
            return (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && autoLoadDirty);
        };

        context.getObjectForEdit = (paneId: number, object: DomainObjectRepresentation) => editOrReloadObject(paneId, object, true);

        context.reloadObject = (paneId: number, object: DomainObjectRepresentation) => editOrReloadObject(paneId, object, false);

        context.getService = (paneId: number, serviceType: string): ng.IPromise<DomainObjectRepresentation> => {

            if (isSameObject(currentObjects[paneId], serviceType)) {
                return $q.when(currentObjects[paneId]);
            }

            return context.getServices().
                then((services: DomainServicesRepresentation) => {
                    const service = services.getService(serviceType);
                    return repLoader.populate(service);
                }).
                then((service: DomainObjectRepresentation) => {
                    currentObjects[paneId] = service;
                    return $q.when(service);
                });
        };

        context.getActionDetails = (actionMember: ActionMember): ng.IPromise<ActionRepresentation> => {
            const details = actionMember.getDetails();
            return repLoader.populate(details, true);
        };

        context.getCollectionDetails = (collectionMember: CollectionMember, state: CollectionViewState, ignoreCache: boolean): ng.IPromise<CollectionRepresentation> => {
            const details = collectionMember.getDetails();

            if (state === CollectionViewState.Table) {
                details.setUrlParameter(roInlineCollectionItems, true);
            }
            const parent = collectionMember.parent;
            const oid = parent.getOid();
            const isDirty = dirtyList.getDirty(oid) !== DirtyState.Clean;

            return repLoader.populate(details, isDirty || ignoreCache);
        };

        context.getInvokableAction = (action: ActionMember | ActionRepresentation | IInvokableAction): ng.IPromise<IInvokableAction> => {

            if (action.invokeLink()) {
                return $q.when(action as IInvokableAction);
            }

            return context.getActionDetails(action as ActionMember);
        };

        context.getMenu = (menuId: string): ng.IPromise<MenuRepresentation> => {

            if (currentMenuList[menuId]) {
                return $q.when(currentMenuList[menuId]);
            }

            return context.getMenus().
                then((menus: MenusRepresentation) => {
                    const menu = menus.getMenu(menuId);
                    return repLoader.populate(menu);
                }).
                then((menu: MenuRepresentation) => {
                    currentMenuList[menuId] = menu;
                    return $q.when(menu);
                });
        };

        context.clearMessages = () => {
            $rootScope.$broadcast(geminiMessageEvent, []);
        };

        context.clearWarnings = () => {
            $rootScope.$broadcast(geminiWarningEvent, []);
        };


        context.getHome = () => {
            // for moment don't bother caching only called on startup and for whatever resaon cache doesn't work. 
            // once version cached no longer called.  
            return repLoader.populate<HomePageRepresentation>(new HomePageRepresentation());
        };

        context.getServices = () => {

            if (currentServices) {
                return $q.when(currentServices);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const ds = home.getDomainServices();
                    return repLoader.populate<DomainServicesRepresentation>(ds);
                }).
                then((services: DomainServicesRepresentation) => {
                    currentServices = services;
                    return $q.when(services);
                });
        };

        context.getMenus = () => {
            if (currentMenus) {
                return $q.when(currentMenus);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const ds = home.getMenus();
                    return repLoader.populate<MenusRepresentation>(ds);
                }).
                then((menus: MenusRepresentation) => {
                    currentMenus = menus;
                    return $q.when(currentMenus);
                });
        };

        context.getVersion = () => {

            if (currentVersion) {
                return $q.when(currentVersion);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const v = home.getVersion();
                    return repLoader.populate<VersionRepresentation>(v);
                }).
                then((version: VersionRepresentation) => {
                    currentVersion = version;
                    return $q.when(version);
                });
        };

        context.getUser = () => {

            if (currentUser) {
                return $q.when(currentUser);
            }

            return context.getHome().
                then((home: HomePageRepresentation) => {
                    const u = home.getUser();
                    return repLoader.populate<UserRepresentation>(u);
                }).
                then((user: UserRepresentation) => {
                    currentUser = user;
                    return $q.when(user);
                });
        };

        context.getObject = (paneId: number, oid: ObjectIdWrapper, interactionMode: InteractionMode) => {
            return oid.isService ? context.getService(paneId, oid.domainType) : context.getDomainObject(paneId, oid, interactionMode);
        };

        context.getCachedList = (paneId: number, page: number, pageSize: number) => {
            const index = urlManager.getListCacheIndex(paneId, page, pageSize);
            const entry = currentLists[index];
            return entry ? entry.list : null;
        };
        context.clearCachedList = (paneId: number, page: number, pageSize: number) => {
            const index = urlManager.getListCacheIndex(paneId, page, pageSize);
            delete currentLists[index];
        };

        function cacheList(list: ListRepresentation, index: string) {

            const entry = currentLists[index];
            if (entry) {
                entry.list = list;
                entry.added = Date.now();
            } else {

                if (_.keys(currentLists).length >= listCacheSize) {
                    //delete oldest;
                    const oldest = _.first(_.sortBy(currentLists, "e.added")).added;
                    const oldestIndex = _.findKey(currentLists, (e: { added: number }) => e.added === oldest);
                    if (oldestIndex) {
                        delete currentLists[oldestIndex];
                    }
                }

                currentLists[index] = { list: list, added: Date.now() };
            }
        }

        const handleResult = (paneId: number, result: ActionResultRepresentation, page: number, pageSize: number) => {

            if (result.resultType() === "list") {
                const resultList = result.result().list();
                const index = urlManager.getListCacheIndex(paneId, page, pageSize);
                cacheList(resultList, index);
                return $q.when(resultList);
            } else {
                return $q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.WrongType, "expect list"));
            }
        };
        const getList = (paneId: number, resultPromise: () => ng.IPromise<ActionResultRepresentation>, page: number, pageSize: number) => {
            return resultPromise().then(result => handleResult(paneId, result, page, pageSize));
        };

        context.getActionExtensionsFromMenu = (menuId: string, actionId: string) =>
            context.getMenu(menuId).then(menu => $q.when(menu.actionMember(actionId).extensions()));

        context.getActionExtensionsFromObject = (paneId: number, oid: ObjectIdWrapper, actionId: string) =>
            context.getObject(paneId, oid, InteractionMode.View).then(object => $q.when(object.actionMember(actionId).extensions()));

        function getPagingParms(page: number, pageSize: number): _.Dictionary<Object> {
            return (page && pageSize) ? { "x-ro-page": page, "x-ro-pageSize": pageSize } : {};
        }

        context.getListFromMenu = (paneId: number, routeData: PaneRouteData, page?: number, pageSize?: number) => {
            const menuId = routeData.menuId;
            const actionId = routeData.actionId;
            const parms = routeData.actionParams;
            const state = routeData.state;
            const urlParms = getPagingParms(page, pageSize);

            if (state === CollectionViewState.Table) {
                urlParms[roInlineCollectionItems] = true;
            }

            const promise = () => context.getMenu(menuId).
                then(menu => context.getInvokableAction(menu.actionMember(actionId))).
                then(details => repLoader.invoke(details, parms, urlParms));
            return getList(paneId, promise, page, pageSize);
        };

        context.getListFromObject = (paneId: number, routeData: PaneRouteData, page?: number, pageSize?: number) => {
            const objectId = routeData.objectId;
            const actionId = routeData.actionId;
            const parms = routeData.actionParams;
            const state = routeData.state;
            const oid = ObjectIdWrapper.fromObjectId(objectId);
            const urlParms = getPagingParms(page, pageSize);

            if (state === CollectionViewState.Table) {
                urlParms[roInlineCollectionItems] = true;
            }

            const promise = () => context.getObject(paneId, oid, InteractionMode.View).
                then(object => context.getInvokableAction(object.actionMember(actionId))).
                then(details => repLoader.invoke(details, parms, urlParms));

            return getList(paneId, promise, page, pageSize);
        };

        context.setObject = (paneId: number, co: DomainObjectRepresentation) => currentObjects[paneId] = co;

        context.swapCurrentObjects = () => {
            const [, p1, p2] = currentObjects;
            currentObjects[1] = p2;
            currentObjects[2] = p1;
        };
        let currentError: ErrorWrapper = null;

        context.getError = () => currentError;

        context.setError = (e: ErrorWrapper) => currentError = e;

        let previousUrl: string = null;

        context.getPreviousUrl = () => previousUrl;

        context.setPreviousUrl = (url: string) => previousUrl = url;

        const doPrompt = (field: IField, id: string, searchTerm: string, setupPrompt: (map: PromptMap) => void, objectValues: () => _.Dictionary<Value>) => {
            const map = field.getPromptMap();
            map.setMembers(objectValues);
            setupPrompt(map);
            return repLoader.retrieve(map, PromptRepresentation).then((p: PromptRepresentation) => p.choices());
        };

        context.autoComplete = (field: IField, id: string, objectValues: () => _.Dictionary<Value>, searchTerm: string) =>
            doPrompt(field, id, searchTerm, (map: PromptMap) => map.setSearchTerm(searchTerm), objectValues);

        context.conditionalChoices = (field: IField, id: string, objectValues: () => _.Dictionary<Value>, args: _.Dictionary<Value>) =>
            doPrompt(field, id, null, (map: PromptMap) => map.setArguments(args), objectValues);

        let nextTransientId = 0;

        context.setResult = (action: IInvokableAction, result: ActionResultRepresentation, paneId: number, page: number, pageSize: number) => {

            const warnings = result.extensions().warnings() || [];
            const messages = result.extensions().messages() || [];

            $rootScope.$broadcast(geminiWarningEvent, warnings);
            $rootScope.$broadcast(geminiMessageEvent, messages);

            if (!result.result().isNull()) {
                if (result.resultType() === "object") {

                    const resultObject = result.result().object();

                    if (resultObject.persistLink()) {
                        // transient object
                        const domainType = resultObject.extensions().domainType();
                        resultObject.wrapped().domainType = domainType;
                        resultObject.wrapped().instanceId = (nextTransientId++).toString();

                        resultObject.hateoasUrl = `/${domainType}/${nextTransientId}`;

                        context.setObject(paneId, resultObject);
                        transientCache.add(paneId, resultObject);
                        urlManager.pushUrlState(paneId);
                        urlManager.setObject(resultObject, paneId);
                        urlManager.setInteractionMode(InteractionMode.Transient, paneId);
                    } else {

                        // persistent object
                        // set the object here and then update the url. That should reload the page but pick up this object 
                        // so we don't hit the server again. 

                        // copy the etag down into the object
                        resultObject.etagDigest = result.etagDigest;

                        context.setObject(paneId, resultObject);
                        urlManager.setObject(resultObject, paneId);

                        // update angular cache 
                        const url = resultObject.selfLink().href() + `?${roInlinePropertyDetails}=false`;
                        repLoader.addToCache(url, resultObject.wrapped());

                        // if render in edit must be  a form 
                        if (resultObject.extensions().interactionMode() === "form") {
                            urlManager.pushUrlState(paneId);
                            urlManager.setInteractionMode(InteractionMode.Form, paneId);
                        } else {
                            addRecentlyViewed(resultObject);
                        }
                    }
                } else if (result.resultType() === "list") {

                    const resultList = result.result().list();

                    urlManager.setList(action, paneId);

                    const index = urlManager.getListCacheIndex(paneId, page, pageSize);
                    cacheList(resultList, index);
                }
            }
        };

        function invokeActionInternal(invokeMap: InvokeMap, action: IInvokableAction, paneId: number, setDirty: () => void) {

            focusManager.setCurrentPane(paneId);

            invokeMap.setUrlParameter(roInlinePropertyDetails, false);

            if (action.extensions().returnType() === "list" && action.extensions().renderEagerly()) {
                invokeMap.setUrlParameter(roInlineCollectionItems, true);
            }

            return repLoader.retrieve(invokeMap, ActionResultRepresentation, action.parent.etagDigest).
                then((result: ActionResultRepresentation) => {
                    setDirty();
                    context.setResult(action, result, paneId, 1, defaultPageSize);
                    return $q.when(result);
                });
        }

        function getSetDirtyFunction(action: IInvokableAction, parms: _.Dictionary<Value>) {
            const parent = action.parent;
            const actionIsNotQueryOnly = action.invokeLink().method() !== "GET";

            if (actionIsNotQueryOnly) {
                if (parent instanceof DomainObjectRepresentation) {
                    return () => dirtyList.setDirty(parent.getOid());
                } else if (parent instanceof ListRepresentation && parms) {

                    const ccaParm = _.find(action.parameters(), p => p.isCollectionContributed());
                    const ccaId = ccaParm ? ccaParm.id() : null;
                    const ccaValue = ccaId ? parms[ccaId] : null;

                    // this should always be true 
                    if (ccaValue && ccaValue.isList()) {

                        const links = _
                            .chain(ccaValue.list())
                            .filter(v => v.isReference())
                            .map(v => v.link())
                            .value();

                        return () => _.forEach(links, l => dirtyList.setDirty(l.getOid()));
                    }
                }
            }

            return () => { };
        }

        context.invokeAction = (action: IInvokableAction, paneId: number, parms: _.Dictionary<Value>) => {

            const invokeOnMap = (iAction: IInvokableAction) => {
                const im = iAction.getInvokeMap();
                _.each(parms, (parm, k) => im.setParameter(k, parm));
                const setDirty = getSetDirtyFunction(iAction, parms);
                return invokeActionInternal(im, iAction, paneId, setDirty);
            }

            return invokeOnMap(action);
        };

        function setNewObject(updatedObject: DomainObjectRepresentation, paneId: number, viewSavedObject: Boolean) {
            context.setObject(paneId, updatedObject);
            dirtyList.setDirty(updatedObject.getOid(), true);

            if (viewSavedObject) {
                urlManager.setObject(updatedObject, paneId);
            } else {
                urlManager.popUrlState(paneId);
            }
        }

        context.updateObject = (object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean) => {
            const update = object.getUpdateMap();

            _.each(props, (v, k) => update.setProperty(k, v));

            return repLoader.retrieve(update, DomainObjectRepresentation, object.etagDigest).
                then((updatedObject: DomainObjectRepresentation) => {
                    // This is a kludge because updated object has no self link.
                    const rawLinks = object.wrapped().links;
                    updatedObject.wrapped().links = rawLinks;
                    setNewObject(updatedObject, paneId, viewSavedObject);
                    return $q.when(updatedObject);
                });
        };

        context.saveObject = (object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean) => {
            const persist = object.getPersistMap();

            _.each(props, (v, k) => persist.setMember(k, v));

            return repLoader.retrieve(persist, DomainObjectRepresentation).
                then((updatedObject: DomainObjectRepresentation) => {
                    transientCache.remove(paneId, object.domainType(), object.id());
                    setNewObject(updatedObject, paneId, viewSavedObject);
                    return $q.when(updatedObject);
                });
        };


        context.validateUpdateObject = (object: DomainObjectRepresentation, props: _.Dictionary<Value>) => {
            const update = object.getUpdateMap();
            update.setValidateOnly();
            _.each(props, (v, k) => update.setProperty(k, v));
            return repLoader.validate(update, object.etagDigest);
        };

        context.validateSaveObject = (object: DomainObjectRepresentation, props: _.Dictionary<Value>) => {
            const persist = object.getPersistMap();
            persist.setValidateOnly();
            _.each(props, (v, k) => persist.setMember(k, v));
            return repLoader.validate(persist);
        };


        const subTypeCache: _.Dictionary<_.Dictionary<ng.IPromise<boolean>>> = {};

        context.isSubTypeOf = (toCheckType: string, againstType: string): ng.IPromise<boolean> => {

            if (subTypeCache[toCheckType] && typeof subTypeCache[toCheckType][againstType] !== "undefined") {
                return subTypeCache[toCheckType][againstType];
            }

            const isSubTypeOf = new DomainTypeActionInvokeRepresentation(againstType, toCheckType);

            const promise = repLoader.populate(isSubTypeOf, true).
                then((updatedObject: DomainTypeActionInvokeRepresentation) => {
                    const is = updatedObject.value();
                    //const entry: _.Dictionary<boolean> = {};
                    //entry[againstType] = is;
                    //subTypeCache[toCheckType] = entry;
                    return is;
                }).
                catch((reject: ErrorWrapper) => {
                    return false;
                });

            const entry: _.Dictionary<ng.IPromise<boolean>> = {};
            entry[againstType] = promise;
            subTypeCache[toCheckType] = entry;

            return promise;
        };

        function handleHttpServerError(reject: ErrorWrapper) {
            urlManager.setError(ErrorCategory.HttpServerError);
        }

        function handleHttpClientError(reject: ErrorWrapper,
            toReload: DomainObjectRepresentation,
            onReload: (updatedObject: DomainObjectRepresentation) => void,
            displayMessages: (em: ErrorMap) => void) {
            switch (reject.httpErrorCode) {
                case (HttpStatusCode.PreconditionFailed):

                    if (toReload.isTransient()) {
                        urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
                    } else {
                        context.reloadObject(1, toReload).
                            then((updatedObject: DomainObjectRepresentation) => {
                                onReload(updatedObject);
                            });
                    }
                    break;
                case (HttpStatusCode.UnprocessableEntity):
                    displayMessages(reject.error as ErrorMap);
                    break;
                default:
                    urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
            }

        }

        function handleClientError(reject: ErrorWrapper, customClientHandler: (ec: ClientErrorCode) => boolean) {

            if (!customClientHandler(reject.clientErrorCode)) {
                urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
            }
        }

        context.handleWrappedError = (reject: ErrorWrapper,
            toReload: DomainObjectRepresentation,
            onReload: (updatedObject: DomainObjectRepresentation) => void,
            displayMessages: (em: ErrorMap) => void,
            customClientHandler: (ec: ClientErrorCode) => boolean = () => false) => {
            if (reject.handled) {
                return;
            }
            reject.handled = true;

            context.setError(reject);
            switch (reject.category) {
                case (ErrorCategory.HttpServerError):
                    handleHttpServerError(reject);
                    break;
                case (ErrorCategory.HttpClientError):
                    handleHttpClientError(reject, toReload, onReload, displayMessages);
                    break;
                case (ErrorCategory.ClientError):
                    handleClientError(reject, customClientHandler);
                    break;
            }
        };

        function addRecentlyViewed(obj: DomainObjectRepresentation) {
            recentcache.add(obj);
        }

        context.getRecentlyViewed = () => recentcache.items();

        function logoff() {
            for (let pane = 1; pane <= 2; pane++) {
                delete currentObjects[pane];
            }
            currentServices = null;
            currentMenus = null;
            currentVersion = null;
            currentUser = null;

            transientCache.clear();
            recentcache.clear();
            dirtyList.clear();

            _.forEach(currentMenuList, (k, v) => delete currentMenuList[v]);
            _.forEach(currentLists, (k, v) => delete currentLists[v]);

        }

        $rootScope.$on(geminiLogoffEvent, () => logoff());

    });

}