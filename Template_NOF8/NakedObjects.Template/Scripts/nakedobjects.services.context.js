/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ListRepresentation = NakedObjects.Models.ListRepresentation;
    var DomainObjectRepresentation = NakedObjects.Models.DomainObjectRepresentation;
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var ActionResultRepresentation = NakedObjects.Models.ActionResultRepresentation;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var HomePageRepresentation = NakedObjects.Models.HomePageRepresentation;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var PromptRepresentation = NakedObjects.Models.PromptRepresentation;
    var DomainTypeActionInvokeRepresentation = NakedObjects.Models.DomainTypeActionInvokeRepresentation;
    var CollectionMember = NakedObjects.Models.CollectionMember;
    var CollectionRepresentation = NakedObjects.Models.CollectionRepresentation;
    var ObjectIdWrapper = NakedObjects.Models.ObjectIdWrapper;
    var DirtyState;
    (function (DirtyState) {
        DirtyState[DirtyState["DirtyMustReload"] = 0] = "DirtyMustReload";
        DirtyState[DirtyState["DirtyMayReload"] = 1] = "DirtyMayReload";
        DirtyState[DirtyState["Clean"] = 2] = "Clean";
    })(DirtyState || (DirtyState = {}));
    var DirtyList = (function () {
        function DirtyList() {
            this.dirtyObjects = {};
        }
        DirtyList.prototype.setDirty = function (oid, alwaysReload) {
            if (alwaysReload === void 0) { alwaysReload = false; }
            this.setDirtyInternal(oid, alwaysReload ? DirtyState.DirtyMustReload : DirtyState.DirtyMayReload);
        };
        DirtyList.prototype.setDirtyInternal = function (oid, dirtyState) {
            var key = oid.getKey();
            this.dirtyObjects[key] = dirtyState;
        };
        DirtyList.prototype.getDirty = function (oid) {
            var key = oid.getKey();
            return this.dirtyObjects[key] || DirtyState.Clean;
        };
        DirtyList.prototype.clearDirty = function (oid) {
            var key = oid.getKey();
            this.dirtyObjects = _.omit(this.dirtyObjects, key);
        };
        DirtyList.prototype.clear = function () {
            this.dirtyObjects = {};
        };
        return DirtyList;
    }());
    function isSameObject(object, type, id) {
        if (object) {
            var sid = object.serviceId();
            return sid ? sid === type : (object.domainType() === type && object.instanceId() === id);
        }
        return false;
    }
    var TransientCache = (function () {
        function TransientCache() {
            this.transientCache = [, [], []]; // per pane 
            this.depth = NakedObjects.transientCacheDepth;
        }
        TransientCache.prototype.add = function (paneId, obj) {
            var paneObjects = this.transientCache[paneId];
            if (paneObjects.length >= this.depth) {
                paneObjects = paneObjects.slice(-(this.depth - 1));
            }
            paneObjects.push(obj);
            this.transientCache[paneId] = paneObjects;
        };
        TransientCache.prototype.get = function (paneId, type, id) {
            var paneObjects = this.transientCache[paneId];
            return _.find(paneObjects, function (o) { return isSameObject(o, type, id); });
        };
        TransientCache.prototype.remove = function (paneId, type, id) {
            var paneObjects = this.transientCache[paneId];
            paneObjects = _.remove(paneObjects, function (o) { return isSameObject(o, type, id); });
            this.transientCache[paneId] = paneObjects;
        };
        TransientCache.prototype.clear = function () {
            this.transientCache = [, [], []];
        };
        TransientCache.prototype.swap = function () {
            var _a = this.transientCache, t1 = _a[1], t2 = _a[2];
            this.transientCache[1] = t2;
            this.transientCache[2] = t1;
        };
        return TransientCache;
    }());
    var RecentCache = (function () {
        function RecentCache() {
            this.recentCache = [];
            this.depth = NakedObjects.recentCacheDepth;
        }
        RecentCache.prototype.add = function (obj) {
            // find any matching entries and remove them - should only be one
            _.remove(this.recentCache, function (i) { return i.id() === obj.id(); });
            // push obj on top of array 
            this.recentCache = [obj].concat(this.recentCache);
            // drop oldest if we're full 
            if (this.recentCache.length > this.depth) {
                this.recentCache = this.recentCache.slice(0, this.depth);
            }
        };
        RecentCache.prototype.items = function () {
            return this.recentCache;
        };
        RecentCache.prototype.clear = function () {
            this.recentCache = [];
        };
        return RecentCache;
    }());
    var ValueCache = (function () {
        function ValueCache() {
            this.currentValues = [, {}, {}];
            this.currentId = [, "", ""];
        }
        ValueCache.prototype.addValue = function (id, valueId, value, paneId) {
            if (this.currentId[paneId] !== id) {
                this.currentId[paneId] = id;
                this.currentValues[paneId] = {};
            }
            this.currentValues[paneId][valueId] = value;
        };
        ValueCache.prototype.getValue = function (id, valueId, paneId) {
            if (this.currentId[paneId] !== id) {
                this.currentId[paneId] = id;
                this.currentValues[paneId] = {};
            }
            return this.currentValues[paneId][valueId];
        };
        ValueCache.prototype.getValues = function (id, paneId) {
            if (id && this.currentId[paneId] !== id) {
                this.currentId[paneId] = id;
                this.currentValues[paneId] = {};
            }
            return this.currentValues[paneId];
        };
        ValueCache.prototype.clear = function (paneId) {
            this.currentId[paneId] = "";
            this.currentValues[paneId] = {};
        };
        ValueCache.prototype.swap = function () {
            var _a = this.currentId, i1 = _a[1], i2 = _a[2];
            this.currentId[1] = i2;
            this.currentId[2] = i1;
            var _b = this.currentValues, v1 = _b[1], v2 = _b[2];
            this.currentValues[1] = v2;
            this.currentValues[2] = v1;
        };
        return ValueCache;
    }());
    NakedObjects.app.service("context", function ($q, repLoader, urlManager, focusManager, $cacheFactory, $rootScope) {
        var context = this;
        // cached values
        var currentObjects = []; // per pane 
        var transientCache = new TransientCache();
        var currentMenuList = {};
        var currentServices = null;
        var currentMenus = null;
        var currentVersion = null;
        var currentUser = null;
        var recentcache = new RecentCache();
        var dirtyList = new DirtyList();
        var currentLists = {};
        var parameterCache = new ValueCache();
        var objectEditCache = new ValueCache();
        var parmUpdaters = [, function () { }, function () { }];
        var objectUpdaters = [, function () { }, function () { }];
        context.setParmUpdater = function (updater, paneId) {
            if (paneId === void 0) { paneId = 1; }
            parmUpdaters[paneId] = updater;
        };
        context.setObjectUpdater = function (updater, paneId) {
            if (paneId === void 0) { paneId = 1; }
            objectUpdaters[paneId] = updater;
        };
        context.clearParmUpdater = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            parmUpdaters[paneId] = function () { };
        };
        context.clearObjectUpdater = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            objectUpdaters[paneId] = function () { };
        };
        var updateParmValues = function () {
            parmUpdaters[1]();
            parmUpdaters[2]();
        };
        var updateObjectValues = function () {
            objectUpdaters[1]();
            objectUpdaters[2]();
        };
        context.updateValues = function () {
            updateObjectValues();
            updateParmValues();
        };
        context.getFile = function (object, url, mt) {
            var isDirty = context.getIsDirty(object.getOid());
            return repLoader.getFile(url, mt, isDirty);
        };
        context.setFile = function (object, url, mt, file) { return repLoader.uploadFile(url, mt, file); };
        context.clearCachedFile = function (url) { return repLoader.clearCache(url); };
        // exposed for test mocking
        context.getDomainObject = function (paneId, oid, interactionMode) {
            var type = oid.domainType;
            var id = oid.instanceId;
            var dirtyState = dirtyList.getDirty(oid);
            var forceReload = (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && NakedObjects.autoLoadDirty);
            if (!forceReload && isSameObject(currentObjects[paneId], type, id)) {
                return $q.when(currentObjects[paneId]);
            }
            // deeper cache for transients
            if (interactionMode === NakedObjects.InteractionMode.Transient) {
                var transientObj = transientCache.get(paneId, type, id);
                return transientObj ? $q.when(transientObj) : $q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.ExpiredTransient, ""));
            }
            var object = new DomainObjectRepresentation();
            object.hateoasUrl = NakedObjects.getAppPath() + "/objects/" + type + "/" + id;
            object.setInlinePropertyDetails(interactionMode === NakedObjects.InteractionMode.Edit);
            return repLoader.populate(object, forceReload).
                then(function (obj) {
                currentObjects[paneId] = obj;
                if (forceReload) {
                    dirtyList.clearDirty(oid);
                }
                addRecentlyViewed(obj);
                return obj;
            });
        };
        function editOrReloadObject(paneId, object, inlineDetails) {
            var parms = {};
            parms[NakedObjects.roInlinePropertyDetails] = inlineDetails;
            return repLoader.retrieveFromLink(object.selfLink(), parms).
                then(function (obj) {
                currentObjects[paneId] = obj;
                var oid = obj.getOid();
                dirtyList.clearDirty(oid);
                return obj;
            });
        }
        context.getIsDirty = function (oid) { return !oid.isService && dirtyList.getDirty(oid) !== DirtyState.Clean; };
        context.mustReload = function (oid) {
            var dirtyState = dirtyList.getDirty(oid);
            return (dirtyState === DirtyState.DirtyMustReload) || ((dirtyState === DirtyState.DirtyMayReload) && NakedObjects.autoLoadDirty);
        };
        context.getObjectForEdit = function (paneId, object) { return editOrReloadObject(paneId, object, true); };
        context.reloadObject = function (paneId, object) { return editOrReloadObject(paneId, object, false); };
        context.getService = function (paneId, serviceType) {
            if (isSameObject(currentObjects[paneId], serviceType)) {
                return $q.when(currentObjects[paneId]);
            }
            return context.getServices().
                then(function (services) {
                var service = services.getService(serviceType);
                return repLoader.populate(service);
            }).
                then(function (service) {
                currentObjects[paneId] = service;
                return service;
            });
        };
        context.getActionDetails = function (actionMember) {
            var details = actionMember.getDetails();
            return repLoader.populate(details, true);
        };
        context.getCollectionDetails = function (collectionMember, state, ignoreCache) {
            var details = collectionMember.getDetails();
            if (state === NakedObjects.CollectionViewState.Table) {
                details.setUrlParameter(NakedObjects.roInlineCollectionItems, true);
            }
            var parent = collectionMember.parent;
            var oid = parent.getOid();
            var isDirty = dirtyList.getDirty(oid) !== DirtyState.Clean;
            return repLoader.populate(details, isDirty || ignoreCache);
        };
        context.getInvokableAction = function (action) {
            if (action.invokeLink()) {
                return $q.when(action);
            }
            return context.getActionDetails(action);
        };
        context.getMenu = function (menuId) {
            if (currentMenuList[menuId]) {
                return $q.when(currentMenuList[menuId]);
            }
            return context.getMenus().
                then(function (menus) {
                var menu = menus.getMenu(menuId);
                return repLoader.populate(menu);
            }).
                then(function (menu) {
                currentMenuList[menuId] = menu;
                return menu;
            });
        };
        context.clearMessages = function () {
            $rootScope.$broadcast(NakedObjects.geminiMessageEvent, []);
        };
        context.clearWarnings = function () {
            $rootScope.$broadcast(NakedObjects.geminiWarningEvent, []);
        };
        context.getHome = function () {
            // for moment don't bother caching only called on startup and for whatever resaon cache doesn't work. 
            // once version cached no longer called.  
            return repLoader.populate(new HomePageRepresentation());
        };
        context.getServices = function () {
            if (currentServices) {
                return $q.when(currentServices);
            }
            return context.getHome().
                then(function (home) {
                var ds = home.getDomainServices();
                return repLoader.populate(ds);
            }).
                then(function (services) {
                currentServices = services;
                return services;
            });
        };
        context.getMenus = function () {
            if (currentMenus) {
                return $q.when(currentMenus);
            }
            return context.getHome().
                then(function (home) {
                var ds = home.getMenus();
                return repLoader.populate(ds);
            }).
                then(function (menus) {
                currentMenus = menus;
                return currentMenus;
            });
        };
        context.getVersion = function () {
            if (currentVersion) {
                return $q.when(currentVersion);
            }
            return context.getHome().
                then(function (home) {
                var v = home.getVersion();
                return repLoader.populate(v);
            }).
                then(function (version) {
                currentVersion = version;
                return version;
            });
        };
        context.getUser = function () {
            if (currentUser) {
                return $q.when(currentUser);
            }
            return context.getHome().
                then(function (home) {
                var u = home.getUser();
                return repLoader.populate(u);
            }).
                then(function (user) {
                currentUser = user;
                return user;
            });
        };
        context.getObject = function (paneId, oid, interactionMode) {
            return oid.isService ? context.getService(paneId, oid.domainType) : context.getDomainObject(paneId, oid, interactionMode);
        };
        context.getCachedList = function (paneId, page, pageSize) {
            var index = urlManager.getListCacheIndex(paneId, page, pageSize);
            var entry = currentLists[index];
            return entry ? entry.list : null;
        };
        context.clearCachedList = function (paneId, page, pageSize) {
            var index = urlManager.getListCacheIndex(paneId, page, pageSize);
            delete currentLists[index];
        };
        context.clearCachedCollection = function (collectionRep) {
            var details = collectionRep.detailsLink() ? collectionRep.getDetails() : null;
            if (details) {
                repLoader.clearCache(details.getUrl());
                details.setUrlParameter(NakedObjects.roInlineCollectionItems, true);
                repLoader.clearCache(details.getUrl());
            }
        };
        function cacheList(list, index) {
            var entry = currentLists[index];
            if (entry) {
                entry.list = list;
                entry.added = Date.now();
            }
            else {
                if (_.keys(currentLists).length >= NakedObjects.listCacheSize) {
                    //delete oldest;
                    var oldest_1 = _.first(_.sortBy(currentLists, "e.added")).added;
                    var oldestIndex = _.findKey(currentLists, function (e) { return e.added === oldest_1; });
                    if (oldestIndex) {
                        delete currentLists[oldestIndex];
                    }
                }
                currentLists[index] = { list: list, added: Date.now() };
            }
        }
        var handleResult = function (paneId, result, page, pageSize) {
            if (result.resultType() === "list") {
                var resultList = result.result().list();
                var index = urlManager.getListCacheIndex(paneId, page, pageSize);
                cacheList(resultList, index);
                return $q.when(resultList);
            }
            else {
                return $q.reject(new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.WrongType, "expect list"));
            }
        };
        var getList = function (paneId, resultPromise, page, pageSize) {
            return resultPromise().then(function (result) { return handleResult(paneId, result, page, pageSize); });
        };
        context.getActionExtensionsFromMenu = function (menuId, actionId) {
            return context.getMenu(menuId).then(function (menu) { return menu.actionMember(actionId).extensions(); });
        };
        context.getActionExtensionsFromObject = function (paneId, oid, actionId) {
            return context.getObject(paneId, oid, NakedObjects.InteractionMode.View).then(function (object) { return object.actionMember(actionId).extensions(); });
        };
        function getPagingParms(page, pageSize) {
            return (page && pageSize) ? { "x-ro-page": page, "x-ro-pageSize": pageSize } : {};
        }
        context.getListFromMenu = function (paneId, routeData, page, pageSize) {
            var menuId = routeData.menuId;
            var actionId = routeData.actionId;
            var parms = routeData.actionParams;
            var state = routeData.state;
            var urlParms = getPagingParms(page, pageSize);
            if (state === NakedObjects.CollectionViewState.Table) {
                urlParms[NakedObjects.roInlineCollectionItems] = true;
            }
            var promise = function () { return context.getMenu(menuId).
                then(function (menu) { return context.getInvokableAction(menu.actionMember(actionId)); }).
                then(function (details) { return repLoader.invoke(details, parms, urlParms); }); };
            return getList(paneId, promise, page, pageSize);
        };
        context.getListFromObject = function (paneId, routeData, page, pageSize) {
            var objectId = routeData.objectId;
            var actionId = routeData.actionId;
            var parms = routeData.actionParams;
            var state = routeData.state;
            var oid = ObjectIdWrapper.fromObjectId(objectId);
            var urlParms = getPagingParms(page, pageSize);
            if (state === NakedObjects.CollectionViewState.Table) {
                urlParms[NakedObjects.roInlineCollectionItems] = true;
            }
            var promise = function () { return context.getObject(paneId, oid, NakedObjects.InteractionMode.View).
                then(function (object) { return context.getInvokableAction(object.actionMember(actionId)); }).
                then(function (details) { return repLoader.invoke(details, parms, urlParms); }); };
            return getList(paneId, promise, page, pageSize);
        };
        context.setObject = function (paneId, co) { return currentObjects[paneId] = co; };
        context.swapCurrentObjects = function () {
            parameterCache.swap();
            objectEditCache.swap();
            transientCache.swap();
            var p1 = currentObjects[1], p2 = currentObjects[2];
            currentObjects[1] = p2;
            currentObjects[2] = p1;
        };
        var currentError = null;
        context.getError = function () { return currentError; };
        context.setError = function (e) { return currentError = e; };
        var previousUrl = null;
        context.getPreviousUrl = function () { return previousUrl; };
        context.setPreviousUrl = function (url) { return previousUrl = url; };
        var doPrompt = function (field, id, searchTerm, setupPrompt, objectValues, digest) {
            var map = field.getPromptMap();
            map.setMembers(objectValues);
            setupPrompt(map);
            return repLoader.retrieve(map, PromptRepresentation, digest).then(function (p) { return p.choices(); });
        };
        context.autoComplete = function (field, id, objectValues, searchTerm, digest) {
            return doPrompt(field, id, searchTerm, function (map) { return map.setSearchTerm(searchTerm); }, objectValues, digest);
        };
        context.conditionalChoices = function (field, id, objectValues, args, digest) {
            return doPrompt(field, id, null, function (map) { return map.setArguments(args); }, objectValues, digest);
        };
        var nextTransientId = 0;
        function setMessages(result) {
            var warnings = result.extensions().warnings() || [];
            var messages = result.extensions().messages() || [];
            $rootScope.$broadcast(NakedObjects.geminiWarningEvent, warnings);
            $rootScope.$broadcast(NakedObjects.geminiMessageEvent, messages);
        }
        context.setResult = function (action, result, fromPaneId, toPaneId, page, pageSize) {
            if (!result.result().isNull()) {
                if (result.resultType() === "object") {
                    var resultObject = result.result().object();
                    if (resultObject.persistLink()) {
                        // transient object
                        var domainType = resultObject.extensions().domainType();
                        resultObject.wrapped().domainType = domainType;
                        resultObject.wrapped().instanceId = (nextTransientId++).toString();
                        resultObject.hateoasUrl = "/" + domainType + "/" + nextTransientId;
                        // copy the etag down into the object
                        resultObject.etagDigest = result.etagDigest;
                        context.setObject(toPaneId, resultObject);
                        transientCache.add(toPaneId, resultObject);
                        urlManager.pushUrlState(toPaneId);
                        urlManager.setObject(resultObject, toPaneId);
                        var interactionMode = resultObject.extensions().interactionMode() === "transient" ? NakedObjects.InteractionMode.Transient : NakedObjects.InteractionMode.NotPersistent;
                        urlManager.setInteractionMode(interactionMode, toPaneId);
                    }
                    else {
                        // persistent object
                        // set the object here and then update the url. That should reload the page but pick up this object 
                        // so we don't hit the server again. 
                        // copy the etag down into the object
                        resultObject.etagDigest = result.etagDigest;
                        context.setObject(toPaneId, resultObject);
                        urlManager.setObject(resultObject, toPaneId);
                        // update angular cache 
                        var url = resultObject.selfLink().href() + ("?" + NakedObjects.roInlinePropertyDetails + "=false");
                        repLoader.addToCache(url, resultObject.wrapped());
                        // if render in edit must be  a form 
                        if (resultObject.extensions().interactionMode() === "form") {
                            urlManager.pushUrlState(toPaneId);
                            urlManager.setInteractionMode(NakedObjects.InteractionMode.Form, toPaneId);
                        }
                        else {
                            addRecentlyViewed(resultObject);
                        }
                    }
                }
                else if (result.resultType() === "list") {
                    var resultList = result.result().list();
                    var parms = parameterCache.getValues(action.actionId(), fromPaneId);
                    urlManager.setList(action, parms, fromPaneId, toPaneId);
                    var index = urlManager.getListCacheIndex(toPaneId, page, pageSize);
                    cacheList(resultList, index);
                }
            }
            else if (result.resultType() === "void") {
                urlManager.triggerPageReloadByFlippingReloadFlagInUrl();
            }
        };
        function invokeActionInternal(invokeMap, action, fromPaneId, toPaneId, setDirty, gotoResult) {
            if (gotoResult === void 0) { gotoResult = false; }
            focusManager.setCurrentPane(toPaneId);
            invokeMap.setUrlParameter(NakedObjects.roInlinePropertyDetails, false);
            if (action.extensions().returnType() === "list" && action.extensions().renderEagerly()) {
                invokeMap.setUrlParameter(NakedObjects.roInlineCollectionItems, true);
            }
            return repLoader.retrieve(invokeMap, ActionResultRepresentation, action.parent.etagDigest).
                then(function (result) {
                setDirty();
                setMessages(result);
                if (gotoResult) {
                    context.setResult(action, result, fromPaneId, toPaneId, 1, NakedObjects.defaultPageSize);
                }
                return result;
            });
        }
        function getSetDirtyFunction(action, parms) {
            var parent = action.parent;
            var actionIsNotQueryOnly = action.invokeLink().method() !== "GET";
            if (actionIsNotQueryOnly) {
                if (parent instanceof DomainObjectRepresentation) {
                    return function () { return dirtyList.setDirty(parent.getOid()); };
                }
                if (parent instanceof CollectionRepresentation) {
                    return function () {
                        var selfLink = parent.selfLink();
                        var oid = ObjectIdWrapper.fromLink(selfLink);
                        dirtyList.setDirty(oid);
                    };
                }
                if (parent instanceof CollectionMember) {
                    return function () { return dirtyList.setDirty(parent.parent.getOid()); };
                }
                if (parent instanceof ListRepresentation && parms) {
                    var ccaParm = _.find(action.parameters(), function (p) { return p.isCollectionContributed(); });
                    var ccaId = ccaParm ? ccaParm.id() : null;
                    var ccaValue = ccaId ? parms[ccaId] : null;
                    // this should always be true 
                    if (ccaValue && ccaValue.isList()) {
                        var links_1 = _
                            .chain(ccaValue.list())
                            .filter(function (v) { return v.isReference(); })
                            .map(function (v) { return v.link(); })
                            .value();
                        return function () { return _.forEach(links_1, function (l) { return dirtyList.setDirty(l.getOid()); }); };
                    }
                }
            }
            return function () { };
        }
        context.invokeAction = function (action, parms, fromPaneId, toPaneId, gotoResult) {
            if (fromPaneId === void 0) { fromPaneId = 1; }
            if (toPaneId === void 0) { toPaneId = 1; }
            if (gotoResult === void 0) { gotoResult = true; }
            var invokeOnMap = function (iAction) {
                var im = iAction.getInvokeMap();
                _.each(parms, function (parm, k) { return im.setParameter(k, parm); });
                var setDirty = getSetDirtyFunction(iAction, parms);
                return invokeActionInternal(im, iAction, fromPaneId, toPaneId, setDirty, gotoResult);
            };
            return invokeOnMap(action);
        };
        function setNewObject(updatedObject, paneId, viewSavedObject) {
            context.setObject(paneId, updatedObject);
            dirtyList.setDirty(updatedObject.getOid(), true);
            if (viewSavedObject) {
                urlManager.setObject(updatedObject, paneId);
            }
            else {
                urlManager.popUrlState(paneId);
            }
        }
        context.updateObject = function (object, props, paneId, viewSavedObject) {
            var update = object.getUpdateMap();
            _.each(props, function (v, k) { return update.setProperty(k, v); });
            return repLoader.retrieve(update, DomainObjectRepresentation, object.etagDigest).
                then(function (updatedObject) {
                // This is a kludge because updated object has no self link.
                var rawLinks = object.wrapped().links;
                updatedObject.wrapped().links = rawLinks;
                setNewObject(updatedObject, paneId, viewSavedObject);
                return updatedObject;
            });
        };
        context.saveObject = function (object, props, paneId, viewSavedObject) {
            var persist = object.getPersistMap();
            _.each(props, function (v, k) { return persist.setMember(k, v); });
            return repLoader.retrieve(persist, DomainObjectRepresentation, object.etagDigest).
                then(function (updatedObject) {
                transientCache.remove(paneId, object.domainType(), object.id());
                setNewObject(updatedObject, paneId, viewSavedObject);
                return updatedObject;
            });
        };
        context.validateUpdateObject = function (object, props) {
            var update = object.getUpdateMap();
            update.setValidateOnly();
            _.each(props, function (v, k) { return update.setProperty(k, v); });
            return repLoader.validate(update, object.etagDigest);
        };
        context.validateSaveObject = function (object, props) {
            var persist = object.getPersistMap();
            persist.setValidateOnly();
            _.each(props, function (v, k) { return persist.setMember(k, v); });
            return repLoader.validate(persist, object.etagDigest);
        };
        var subTypeCache = {};
        context.isSubTypeOf = function (toCheckType, againstType) {
            if (subTypeCache[toCheckType] && typeof subTypeCache[toCheckType][againstType] !== "undefined") {
                return subTypeCache[toCheckType][againstType];
            }
            var isSubTypeOf = new DomainTypeActionInvokeRepresentation(againstType, toCheckType);
            var promise = repLoader.populate(isSubTypeOf, true).
                then(function (updatedObject) {
                return updatedObject.value();
            }).
                catch(function (reject) {
                return false;
            });
            var entry = {};
            entry[againstType] = promise;
            subTypeCache[toCheckType] = entry;
            return promise;
        };
        function addRecentlyViewed(obj) {
            recentcache.add(obj);
        }
        context.getRecentlyViewed = function () { return recentcache.items(); };
        context.clearRecentlyViewed = function () { return recentcache.clear(); };
        function logoff() {
            for (var pane = 1; pane <= 2; pane++) {
                delete currentObjects[pane];
            }
            currentServices = null;
            currentMenus = null;
            currentVersion = null;
            currentUser = null;
            transientCache.clear();
            recentcache.clear();
            dirtyList.clear();
            _.forEach(currentMenuList, function (k, v) { return delete currentMenuList[v]; });
            _.forEach(currentLists, function (k, v) { return delete currentLists[v]; });
        }
        $rootScope.$on(NakedObjects.geminiLogoffEvent, function () { return logoff(); });
        context.setFieldValue = function (dialogId, pid, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            parameterCache.addValue(dialogId, pid, pv, paneId);
        };
        context.getCurrentDialogValues = function (dialogId, paneId) {
            if (dialogId === void 0) { dialogId = null; }
            if (paneId === void 0) { paneId = 1; }
            return parameterCache.getValues(dialogId, paneId);
        };
        context.getCurrentObjectValues = function (objectId, paneId) {
            if (objectId === void 0) { objectId = null; }
            if (paneId === void 0) { paneId = 1; }
            return objectEditCache.getValues(objectId, paneId);
        };
        context.clearDialogValues = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            parameterCache.clear(paneId);
        };
        context.clearObjectValues = function (paneId) {
            if (paneId === void 0) { paneId = 1; }
            objectEditCache.clear(paneId);
        };
        context.setPropertyValue = function (obj, p, pv, paneId) {
            if (paneId === void 0) { paneId = 1; }
            objectEditCache.addValue(obj.id(), p.id(), pv, paneId);
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.context.js.map