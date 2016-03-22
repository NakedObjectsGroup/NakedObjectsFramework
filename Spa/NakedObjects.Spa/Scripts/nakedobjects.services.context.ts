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
    import Link = Models.Link;
    import ErrorCategory = Models.ErrorCategory;
    import PromptMap = Models.PromptMap;
    import PromptRepresentation = Models.PromptRepresentation;
    import InvokeMap = Models.InvokeMap;
    import DomainTypeActionInvokeRepresentation = Models.DomainTypeActionInvokeRepresentation;
    import HttpStatusCode = Models.HttpStatusCode;

    export interface IContext {

        getCachedList: (paneId: number, page: number, pageSize: number) => ListRepresentation;
        clearCachedList: (paneId: number, page: number, pageSize: number) => void;

        getVersion: () => ng.IPromise<VersionRepresentation>;
        getMenus: () => ng.IPromise<MenusRepresentation>;
        getMenu: (menuId: string) => ng.IPromise<MenuRepresentation>;
        getObject: (paneId: number, type: string, id: string[], isTransient: boolean) => ng.IPromise<DomainObjectRepresentation>;
        getObjectByOid: (paneId: number, objectId: string) => ng.IPromise<DomainObjectRepresentation>;
        getListFromMenu: (paneId: number, menuId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => angular.IPromise<ListRepresentation>;
        getListFromObject: (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => angular.IPromise<ListRepresentation>;

        getActionFriendlyName: (action: ActionMember) => ng.IPromise<string>;
        getError: () => ErrorWrapper;
        getPreviousUrl: () => string;

        //The object values are only needed on a transient object / editable view model
        autoComplete(field: IField, id: string, objectValues: () => _.Dictionary<Value>, searchTerm: string): ng.IPromise<_.Dictionary<Value>>;
        //The object values are only needed on a transient object / editable view model
        conditionalChoices(field: IField, id: string, objectValues: () => _.Dictionary<Value>, args: _.Dictionary<Value>): ng.IPromise<_.Dictionary<Value>>;

        invokeAction(action: ActionMember, paneId: number, parms: _.Dictionary<Value>): ng.IPromise<ActionResultRepresentation>;

        updateObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean): ng.IPromise<DomainObjectRepresentation>;
        saveObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>, paneId: number, viewSavedObject: boolean): ng.IPromise<DomainObjectRepresentation>;

        validateUpdateObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>): ng.IPromise<boolean>;
        validateSaveObject(object: DomainObjectRepresentation, props: _.Dictionary<Value>): ng.IPromise<boolean>;


        reloadObject: (paneId: number, object: DomainObjectRepresentation) => angular.IPromise<DomainObjectRepresentation>;

        setError: (reject: ErrorWrapper) => void;

        isSubTypeOf(toCheckType: string, againstType: string): ng.IPromise<boolean>;

        getActionExtensionsFromMenu: (menuId: string, actionId: string) => angular.IPromise<Extensions>;
        getActionExtensionsFromObject: (paneId: number, objectId: string, actionId: string) => angular.IPromise<Extensions>;

        swapCurrentObjects(): void;

        clearMessages(): void;
        clearWarnings(): void;

        handleWrappedError(reject: ErrorWrapper,
            toReload: DomainObjectRepresentation,
            onReload: (updatedObject: DomainObjectRepresentation) => void,
            displayMessages: (em: ErrorMap) => void,
            customClientHandler?: (ec: ClientErrorCode) => boolean): void;

        getRecentlyViewed() : DomainObjectRepresentation[];

    }

    interface IContextInternal extends IContext {
        getHome: () => ng.IPromise<HomePageRepresentation>;
        getDomainObject: (paneId: number, type: string, id: string, transient: boolean) => ng.IPromise<DomainObjectRepresentation>;
        getServices: () => ng.IPromise<DomainServicesRepresentation>;
        getService: (paneId: number, type: string) => ng.IPromise<DomainObjectRepresentation>;
        setObject: (paneId: number, object: DomainObjectRepresentation) => void;
        setResult(action: ActionMember, result: ActionResultRepresentation, paneId: number, page: number, pageSize: number): void;
        setPreviousUrl: (url: string) => void;
    }

    class DirtyCache {
        private dirtyObjects: _.Dictionary<boolean> = {};

        private getKey(type: string, id: string) {
            return type + "-" + id;
        }

        setDirty(obj: DomainObjectRepresentation | Link) {
            if (obj instanceof DomainObjectRepresentation) {
                this.setDirtyObject(obj);
            }
            if (obj instanceof Link) {
                this.setDirtyLink(obj);
            }
        }


        setDirtyObject(objectRepresentation: DomainObjectRepresentation) {
            const key = this.getKey(objectRepresentation.domainType(), objectRepresentation.instanceId());
            this.dirtyObjects[key] = true;
        }

        setDirtyLink(link: Link) {
            const href = link.href().split("/");
            const [id, dt] = href.reverse();
            const key = this.getKey(dt, id);
            this.dirtyObjects[key] = true;
        }


        getDirty(type: string, id: string) {
            const key = this.getKey(type, id);
            return this.dirtyObjects[key];
        }

        clearDirty(type: string, id: string) {
            const key = this.getKey(type, id);
            this.dirtyObjects = _.omit(this.dirtyObjects, key) as _.Dictionary<boolean>;
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

        private depth = 4;

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

    }


    app.service("context", function($q: ng.IQService,
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

        const dirtyCache = new DirtyCache();
        const currentLists: _.Dictionary<{ list: ListRepresentation; added: number }> = {};

        function getAppPath() {
            if (appPath.charAt(appPath.length - 1) === "/") {
                return appPath.length > 1 ? appPath.substring(0, appPath.length - 2) : "";
            }
            return appPath;
        }

        // exposed for test mocking
        context.getDomainObject = (paneId: number, type: string, id: string, transient: boolean): ng.IPromise<DomainObjectRepresentation> => {

            const isDirty = dirtyCache.getDirty(type, id);

            if (!isDirty && isSameObject(currentObjects[paneId], type, id)) {
                return $q.when(currentObjects[paneId]);
            }

            // deeper cache for transients
            if (transient) {
                const transientObj = transientCache.get(paneId, type, id);
                return transientObj ? $q.when(transientObj) : $q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.ExpiredTransient, ""));
            }

            const object = new DomainObjectRepresentation();
            object.hateoasUrl = getAppPath() + "/objects/" + type + "/" + id;

            return repLoader.populate<DomainObjectRepresentation>(object, isDirty).
                then((obj: DomainObjectRepresentation) => {
                    currentObjects[paneId] = obj;
                    dirtyCache.clearDirty(type, id);
                    addRecentlyViewed(obj);
                    return $q.when(obj);
                });
        };

        context.reloadObject = (paneId: number, object: DomainObjectRepresentation) => {
            return repLoader.retrieveFromLink<DomainObjectRepresentation>(object.selfLink()).
                then(obj => {
                    currentObjects[paneId] = obj;
                    return $q.when(obj);
                });
        };

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
            $rootScope.$broadcast("nof-message", []);
        };

        context.clearWarnings = () => {
            $rootScope.$broadcast("nof-warning", []);
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

        context.getObject = (paneId: number, type: string, id: string[], transient: boolean) => {
            const oid = _.reduce(id, (a, v) => `${a}${a ? "-" : ""}${v}`, "");
            return oid ? context.getDomainObject(paneId, type, oid, transient) : context.getService(paneId, type);
        };

        context.getObjectByOid = (paneId: number, objectId: string) => {
            const [dt, ...id] = objectId.split("-");
            return context.getObject(paneId, dt, id, false);
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

        context.getActionExtensionsFromObject = (paneId: number, objectId: string, actionId: string) =>
            context.getObjectByOid(paneId, objectId).then(object => $q.when(object.actionMember(actionId).extensions()));

        function getPagingParms(page: number, pageSize: number): _.Dictionary<string> {
            return (page && pageSize) ? { "x-ro-page": page.toString(), "x-ro-pageSize": pageSize.toString() } : {};
        }

        context.getListFromMenu = (paneId: number, menuId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => {
            const urlParms = getPagingParms(page, pageSize);
            const promise = () => context.getMenu(menuId).then(menu => repLoader.invoke(menu.actionMember(actionId), parms, urlParms));
            return getList(paneId, promise, page, pageSize);
        };

        context.getListFromObject = (paneId: number, objectId: string, actionId: string, parms: _.Dictionary<Value>, page?: number, pageSize?: number) => {
            const urlParms = getPagingParms(page, pageSize);
            const promise = () => context.getObjectByOid(paneId, objectId).then(object => repLoader.invoke(object.actionMember(actionId), parms, urlParms));
            return getList(paneId, promise, page, pageSize);
        };

        context.setObject = (paneId: number, co : DomainObjectRepresentation) => currentObjects[paneId] = co;

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

        context.setResult = (action: ActionMember, result: ActionResultRepresentation, paneId: number, page: number, pageSize: number) => {

            const warnings = result.extensions().warnings() || [];
            const messages = result.extensions().messages() || [];

            $rootScope.$broadcast("nof-warning", warnings);
            $rootScope.$broadcast("nof-message", messages);

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

        function invokeActionInternal(invokeMap: InvokeMap, action: ActionMember, paneId: number, setDirty: () => void) {

            focusManager.setCurrentPane(paneId);

            return repLoader.retrieve(invokeMap, ActionResultRepresentation, action.parent.etagDigest).
                then((result: ActionResultRepresentation) => {
                    setDirty();
                    context.setResult(action, result, paneId, 1, defaultPageSize);
                    return $q.when(result);
                });
        }

        function getSetDirtyFunction(action: ActionMember, parms: _.Dictionary<Value>) {
            const parent = action.parent;
            const actionIsNotQueryOnly = action.invokeLink().method() !== "GET";

            if (actionIsNotQueryOnly) {
                if (parent instanceof DomainObjectRepresentation) {
                    return () => dirtyCache.setDirty(parent);
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

                        return () => _.forEach(links, l => dirtyCache.setDirty(l));
                    }
                }
            }

            return () => {};
        }

        context.invokeAction = (action: ActionMember, paneId: number, parms: _.Dictionary<Value>) => {
            const invokeMap = action.getInvokeMap();

            _.each(parms, (parm, k) => invokeMap.setParameter(k, parm));

            const setDirty = getSetDirtyFunction(action, parms);
            return invokeActionInternal(invokeMap, action, paneId, setDirty);
        };

        function setNewObject(updatedObject: DomainObjectRepresentation, paneId: number, viewSavedObject: Boolean) {
            context.setObject(paneId, updatedObject);
            dirtyCache.setDirty(updatedObject);

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


        const subTypeCache: _.Dictionary<_.Dictionary<boolean>> = {};

        context.isSubTypeOf = (toCheckType: string, againstType: string): ng.IPromise<boolean> => {

            if (subTypeCache[toCheckType] && typeof subTypeCache[toCheckType][againstType] !== "undefined") {
                return $q.when(subTypeCache[toCheckType][againstType]);
            }

            const isSubTypeOf = new DomainTypeActionInvokeRepresentation(againstType, toCheckType);

            return repLoader.populate(isSubTypeOf, true).
                then((updatedObject: DomainTypeActionInvokeRepresentation) => {
                    const is = updatedObject.value();
                    const entry: _.Dictionary<boolean> = {};
                    entry[againstType] = is;
                    subTypeCache[toCheckType] = entry;
                    return is;
                }).
                catch((reject: ErrorWrapper) => {
                    return false;
                });
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
                            urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
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


        function cacheRecentlyViewed(object: DomainObjectRepresentation) {
            const cache = $cacheFactory.get("recentlyViewed");

            if (cache && object && !object.persistLink()) {
                const key = object.domainType();
                const subKey = object.selfLink().href();
                const dict = cache.get(key) || {};
                dict[subKey] = { value: new Value(object.selfLink()), name: object.title() };
                cache.put(key, dict);
            }
        }


        // naive impl

        const recentlyViewed : DomainObjectRepresentation[] = [];

        function addRecentlyViewed(obj: DomainObjectRepresentation) {
            recentlyViewed.push(obj);
        }

        context.getRecentlyViewed = () => {
            return recentlyViewed;
        }

    });

}