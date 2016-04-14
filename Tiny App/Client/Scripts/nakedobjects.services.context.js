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
    var Link = NakedObjects.Models.Link;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var PromptRepresentation = NakedObjects.Models.PromptRepresentation;
    var DomainTypeActionInvokeRepresentation = NakedObjects.Models.DomainTypeActionInvokeRepresentation;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var DirtyCache = (function () {
        function DirtyCache() {
            this.dirtyObjects = {};
        }
        DirtyCache.prototype.getKey = function (type, id) {
            return type + NakedObjects.keySeparator + id;
        };
        DirtyCache.prototype.setDirty = function (obj) {
            if (obj instanceof DomainObjectRepresentation) {
                this.setDirtyObject(obj);
            }
            if (obj instanceof Link) {
                this.setDirtyLink(obj);
            }
        };
        DirtyCache.prototype.setDirtyObject = function (objectRepresentation) {
            var key = this.getKey(objectRepresentation.domainType(), objectRepresentation.instanceId());
            this.dirtyObjects[key] = true;
        };
        DirtyCache.prototype.setDirtyLink = function (link) {
            var href = link.href().split("/");
            var _a = href.reverse(), id = _a[0], dt = _a[1];
            var key = this.getKey(dt, id);
            this.dirtyObjects[key] = true;
        };
        DirtyCache.prototype.getDirty = function (type, id) {
            var key = this.getKey(type, id);
            return this.dirtyObjects[key];
        };
        DirtyCache.prototype.clearDirty = function (type, id) {
            var key = this.getKey(type, id);
            this.dirtyObjects = _.omit(this.dirtyObjects, key);
        };
        return DirtyCache;
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
            this.depth = 4;
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
        return TransientCache;
    }());
    var RecentCache = (function () {
        function RecentCache() {
            this.recentCache = [];
            this.depth = 20;
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
        return RecentCache;
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
        var recentcache = new RecentCache();
        var dirtyCache = new DirtyCache();
        var currentLists = {};
        // exposed for test mocking
        context.getDomainObject = function (paneId, type, id, interactionMode) {
            var isDirty = dirtyCache.getDirty(type, id);
            if (!isDirty && isSameObject(currentObjects[paneId], type, id)) {
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
            return repLoader.populate(object, isDirty).
                then(function (obj) {
                currentObjects[paneId] = obj;
                dirtyCache.clearDirty(type, id);
                addRecentlyViewed(obj);
                return $q.when(obj);
            });
        };
        function editOrReloadObject(paneId, object, inlineDetails) {
            var parms = {};
            parms[NakedObjects.roInlinePropertyDetails] = inlineDetails;
            return repLoader.retrieveFromLink(object.selfLink(), parms).
                then(function (obj) {
                currentObjects[paneId] = obj;
                return $q.when(obj);
            });
        }
        context.getObjectForEdit = function (paneId, object) {
            return editOrReloadObject(paneId, object, true);
        };
        context.reloadObject = function (paneId, object) {
            return editOrReloadObject(paneId, object, false);
        };
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
                return $q.when(service);
            });
        };
        context.getActionDetails = function (actionMember) {
            var details = actionMember.getDetails();
            return repLoader.populate(details);
        };
        context.getCollectionDetails = function (collectionMember) {
            var details = collectionMember.getDetails();
            return repLoader.populate(details);
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
                return $q.when(menu);
            });
        };
        context.clearMessages = function () {
            $rootScope.$broadcast("nof-message", []);
        };
        context.clearWarnings = function () {
            $rootScope.$broadcast("nof-warning", []);
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
                return $q.when(services);
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
                return $q.when(currentMenus);
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
                return $q.when(version);
            });
        };
        context.getObject = function (paneId, type, id, interactionMode) {
            var oid = _.reduce(id, function (a, v) { return ("" + a + (a ? NakedObjects.keySeparator : "") + v); }, "");
            return oid ? context.getDomainObject(paneId, type, oid, interactionMode) : context.getService(paneId, type);
        };
        context.getObjectByOid = function (paneId, objectId, interactionMode) {
            var _a = objectId.split(NakedObjects.keySeparator), dt = _a[0], id = _a.slice(1);
            return context.getObject(paneId, dt, id, interactionMode);
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
            return context.getMenu(menuId).then(function (menu) { return $q.when(menu.actionMember(actionId).extensions()); });
        };
        context.getActionExtensionsFromObject = function (paneId, objectId, actionId) {
            return context.getObjectByOid(paneId, objectId, NakedObjects.InteractionMode.View).then(function (object) { return $q.when(object.actionMember(actionId).extensions()); });
        };
        function getPagingParms(page, pageSize) {
            return (page && pageSize) ? { "x-ro-page": page, "x-ro-pageSize": pageSize } : {};
        }
        context.getListFromMenu = function (paneId, menuId, actionId, parms, page, pageSize) {
            var urlParms = getPagingParms(page, pageSize);
            var promise = function () { return context.getMenu(menuId).
                then(function (menu) { return context.getInvokableAction(menu.actionMember(actionId)); }).
                then(function (details) { return repLoader.invoke(details, parms, urlParms); }); };
            return getList(paneId, promise, page, pageSize);
        };
        context.getListFromObject = function (paneId, objectId, actionId, parms, page, pageSize) {
            var urlParms = getPagingParms(page, pageSize);
            var promise = function () { return context.getObjectByOid(paneId, objectId, NakedObjects.InteractionMode.View).
                then(function (object) { return context.getInvokableAction(object.actionMember(actionId)); }).
                then(function (details) { return repLoader.invoke(details, parms, urlParms); }); };
            return getList(paneId, promise, page, pageSize);
        };
        context.setObject = function (paneId, co) { return currentObjects[paneId] = co; };
        context.swapCurrentObjects = function () {
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
        var doPrompt = function (field, id, searchTerm, setupPrompt, objectValues) {
            var map = field.getPromptMap();
            map.setMembers(objectValues);
            setupPrompt(map);
            return repLoader.retrieve(map, PromptRepresentation).then(function (p) { return p.choices(); });
        };
        context.autoComplete = function (field, id, objectValues, searchTerm) {
            return doPrompt(field, id, searchTerm, function (map) { return map.setSearchTerm(searchTerm); }, objectValues);
        };
        context.conditionalChoices = function (field, id, objectValues, args) {
            return doPrompt(field, id, null, function (map) { return map.setArguments(args); }, objectValues);
        };
        var nextTransientId = 0;
        context.setResult = function (action, result, paneId, page, pageSize) {
            var warnings = result.extensions().warnings() || [];
            var messages = result.extensions().messages() || [];
            $rootScope.$broadcast("nof-warning", warnings);
            $rootScope.$broadcast("nof-message", messages);
            if (!result.result().isNull()) {
                if (result.resultType() === "object") {
                    var resultObject = result.result().object();
                    if (resultObject.persistLink()) {
                        // transient object
                        var domainType = resultObject.extensions().domainType();
                        resultObject.wrapped().domainType = domainType;
                        resultObject.wrapped().instanceId = (nextTransientId++).toString();
                        resultObject.hateoasUrl = "/" + domainType + "/" + nextTransientId;
                        context.setObject(paneId, resultObject);
                        transientCache.add(paneId, resultObject);
                        urlManager.pushUrlState(paneId);
                        urlManager.setObject(resultObject, paneId);
                        urlManager.setInteractionMode(NakedObjects.InteractionMode.Transient, paneId);
                    }
                    else {
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
                            urlManager.setInteractionMode(NakedObjects.InteractionMode.Form, paneId);
                        }
                        else {
                            addRecentlyViewed(resultObject);
                        }
                    }
                }
                else if (result.resultType() === "list") {
                    var resultList = result.result().list();
                    urlManager.setList(action, paneId);
                    var index = urlManager.getListCacheIndex(paneId, page, pageSize);
                    cacheList(resultList, index);
                }
            }
        };
        function invokeActionInternal(invokeMap, action, paneId, setDirty) {
            focusManager.setCurrentPane(paneId);
            invokeMap.setUrlParameter(NakedObjects.roInlinePropertyDetails, false);
            return repLoader.retrieve(invokeMap, ActionResultRepresentation, action.parent.etagDigest).
                then(function (result) {
                setDirty();
                context.setResult(action, result, paneId, 1, NakedObjects.defaultPageSize);
                return $q.when(result);
            });
        }
        function getSetDirtyFunction(action, parms) {
            var parent = action.parent;
            var actionIsNotQueryOnly = action.invokeLink().method() !== "GET";
            if (actionIsNotQueryOnly) {
                if (parent instanceof DomainObjectRepresentation) {
                    return function () { return dirtyCache.setDirty(parent); };
                }
                else if (parent instanceof ListRepresentation && parms) {
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
                        return function () { return _.forEach(links_1, function (l) { return dirtyCache.setDirty(l); }); };
                    }
                }
            }
            return function () { };
        }
        context.invokeAction = function (action, paneId, parms) {
            var invokeOnMap = function (iAction) {
                var im = iAction.getInvokeMap();
                _.each(parms, function (parm, k) { return im.setParameter(k, parm); });
                var setDirty = getSetDirtyFunction(iAction, parms);
                return invokeActionInternal(im, iAction, paneId, setDirty);
            };
            return context.getInvokableAction(action).then(function (details) { return invokeOnMap(details); });
        };
        function setNewObject(updatedObject, paneId, viewSavedObject) {
            context.setObject(paneId, updatedObject);
            dirtyCache.setDirty(updatedObject);
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
                return $q.when(updatedObject);
            });
        };
        context.saveObject = function (object, props, paneId, viewSavedObject) {
            var persist = object.getPersistMap();
            _.each(props, function (v, k) { return persist.setMember(k, v); });
            return repLoader.retrieve(persist, DomainObjectRepresentation).
                then(function (updatedObject) {
                transientCache.remove(paneId, object.domainType(), object.id());
                setNewObject(updatedObject, paneId, viewSavedObject);
                return $q.when(updatedObject);
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
            return repLoader.validate(persist);
        };
        var subTypeCache = {};
        context.isSubTypeOf = function (toCheckType, againstType) {
            if (subTypeCache[toCheckType] && typeof subTypeCache[toCheckType][againstType] !== "undefined") {
                return $q.when(subTypeCache[toCheckType][againstType]);
            }
            var isSubTypeOf = new DomainTypeActionInvokeRepresentation(againstType, toCheckType);
            return repLoader.populate(isSubTypeOf, true).
                then(function (updatedObject) {
                var is = updatedObject.value();
                var entry = {};
                entry[againstType] = is;
                subTypeCache[toCheckType] = entry;
                return is;
            }).
                catch(function (reject) {
                return false;
            });
        };
        function handleHttpServerError(reject) {
            urlManager.setError(ErrorCategory.HttpServerError);
        }
        function handleHttpClientError(reject, toReload, onReload, displayMessages) {
            switch (reject.httpErrorCode) {
                case (HttpStatusCode.PreconditionFailed):
                    if (toReload.isTransient()) {
                        urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
                    }
                    else {
                        context.reloadObject(1, toReload).
                            then(function (updatedObject) {
                            onReload(updatedObject);
                            urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
                        });
                    }
                    break;
                case (HttpStatusCode.UnprocessableEntity):
                    displayMessages(reject.error);
                    break;
                default:
                    urlManager.setError(ErrorCategory.HttpClientError, reject.httpErrorCode);
            }
        }
        function handleClientError(reject, customClientHandler) {
            if (!customClientHandler(reject.clientErrorCode)) {
                urlManager.setError(ErrorCategory.ClientError, reject.clientErrorCode);
            }
        }
        context.handleWrappedError = function (reject, toReload, onReload, displayMessages, customClientHandler) {
            if (customClientHandler === void 0) { customClientHandler = function () { return false; }; }
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
        function addRecentlyViewed(obj) {
            recentcache.add(obj);
        }
        context.getRecentlyViewed = function () { return recentcache.items(); };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.context.js.map