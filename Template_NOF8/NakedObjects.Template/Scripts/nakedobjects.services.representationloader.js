/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var ActionResultRepresentation = NakedObjects.Models.ActionResultRepresentation;
    var ErrorCategory = NakedObjects.Models.ErrorCategory;
    var ErrorRepresentation = NakedObjects.Models.ErrorRepresentation;
    var ErrorMap = NakedObjects.Models.ErrorMap;
    var HttpStatusCode = NakedObjects.Models.HttpStatusCode;
    var ErrorWrapper = NakedObjects.Models.ErrorWrapper;
    var isIDomainObjectRepresentation = NakedObjects.Models.isIDomainObjectRepresentation;
    var isResourceRepresentation = NakedObjects.Models.isResourceRepresentation;
    var ClientErrorCode = NakedObjects.Models.ClientErrorCode;
    var isErrorRepresentation = NakedObjects.Models.isErrorRepresentation;
    NakedObjects.app.service("repLoader", function ($http, $q, $rootScope, $cacheFactory) {
        var _this = this;
        var repLoader = this;
        var loadingCount = 0;
        // use our own LRU cache 
        var cache = $cacheFactory("nof-cache", { capacity: NakedObjects.httpCacheDepth });
        function addIfMatchHeader(config, digest) {
            if (digest && (config.method === "POST" || config.method === "PUT" || config.method === "DELETE")) {
                config.headers = { "If-Match": digest };
            }
        }
        function handleInvalidResponse(rc) {
            var rr = new ErrorWrapper(rc, ClientErrorCode.ConnectionProblem, "The response from the client was not parseable as a RestfulObject json Representation ");
            return $q.reject(rr);
        }
        function handleError(promiseCallback) {
            var category;
            var error;
            if (promiseCallback.status === HttpStatusCode.InternalServerError) {
                // this error should contain an error representatation 
                if (isErrorRepresentation(promiseCallback.data)) {
                    var errorRep = new ErrorRepresentation();
                    errorRep.populate(promiseCallback.data);
                    category = ErrorCategory.HttpServerError;
                    error = errorRep;
                }
                else {
                    return handleInvalidResponse(ErrorCategory.HttpServerError);
                }
            }
            else if (promiseCallback.status === -1) {
                // failed to connect
                category = ErrorCategory.ClientError;
                error = "Failed to connect to server: " + (promiseCallback.config ? promiseCallback.config.url : "unknown");
            }
            else {
                category = ErrorCategory.HttpClientError;
                var message = promiseCallback.headers("warning") || "Unknown client HTTP error";
                if (promiseCallback.status === HttpStatusCode.BadRequest ||
                    promiseCallback.status === HttpStatusCode.UnprocessableEntity) {
                    // these errors should contain a map          
                    error = new ErrorMap(promiseCallback.data, promiseCallback.status, message);
                }
                else {
                    error = message;
                }
            }
            var rr = new ErrorWrapper(category, promiseCallback.status, error);
            return $q.reject(rr);
        }
        function httpValidate(config) {
            $rootScope.$broadcast(NakedObjects.geminiAjaxChangeEvent, ++loadingCount);
            return $http(config)
                .then(function () {
                return true;
            })
                .catch(function (promiseCallback) {
                return handleError(promiseCallback);
            })
                .finally(function () {
                $rootScope.$broadcast(NakedObjects.geminiAjaxChangeEvent, --loadingCount);
            });
        }
        // special handler for case whwre we reciece a redirected object back from server 
        // instead of an actionresult. Wrap the object in an actionresult and then handle normally
        function handleRedirectedObject(response, data) {
            if (response instanceof ActionResultRepresentation && isIDomainObjectRepresentation(data)) {
                var actionResult = {
                    resultType: "object",
                    result: data,
                    links: [],
                    extensions: {}
                };
                return actionResult;
            }
            return data;
        }
        function isValidResponse(data) {
            return isResourceRepresentation(data);
        }
        function httpPopulate(config, ignoreCache, response) {
            $rootScope.$broadcast(NakedObjects.geminiAjaxChangeEvent, ++loadingCount);
            if (ignoreCache) {
                // clear cache of existing values
                cache.remove(config.url);
            }
            return $http(config)
                .then(function (promiseCallback) {
                if (!isValidResponse(promiseCallback.data)) {
                    return handleInvalidResponse(ErrorCategory.ClientError);
                }
                var representation = handleRedirectedObject(response, promiseCallback.data);
                response.populate(representation);
                response.etagDigest = promiseCallback.headers("ETag");
                return response;
            })
                .catch(function (promiseCallback) {
                return handleError(promiseCallback);
            })
                .finally(function () {
                $rootScope.$broadcast(NakedObjects.geminiAjaxChangeEvent, --loadingCount);
            });
        }
        repLoader.populate = function (model, ignoreCache) {
            var response = model;
            var useCache = !ignoreCache;
            var config = {
                withCredentials: true,
                url: model.getUrl(),
                method: model.method,
                cache: useCache ? cache : false,
                data: model.getBody()
            };
            return httpPopulate(config, ignoreCache, response);
        };
        function setConfigFromMap(map, digest) {
            var config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: false,
                data: map.getBody()
            };
            addIfMatchHeader(config, digest);
            return config;
        }
        repLoader.retrieve = function (map, rc, digest) {
            var response = new rc();
            var config = setConfigFromMap(map, digest);
            return httpPopulate(config, true, response);
        };
        repLoader.validate = function (map, digest) {
            var config = setConfigFromMap(map, digest);
            return httpValidate(config);
        };
        repLoader.retrieveFromLink = function (link, parms) {
            var response = link.getTarget();
            var urlParms = "";
            if (parms) {
                var urlParmString = _.reduce(parms, function (result, n, key) { return (result === "" ? "" : result + "&") + key + "=" + n; }, "");
                urlParms = urlParmString !== "" ? "?" + urlParmString : "";
            }
            var config = {
                withCredentials: true,
                url: link.href() + urlParms,
                method: link.method(),
                cache: false
            };
            return httpPopulate(config, true, response);
        };
        repLoader.invoke = function (action, parms, urlParms) {
            var invokeMap = action.getInvokeMap();
            _.each(urlParms, function (v, k) { return invokeMap.setUrlParameter(k, v); });
            _.each(parms, function (v, k) { return invokeMap.setParameter(k, v); });
            return _this.retrieve(invokeMap, ActionResultRepresentation);
        };
        repLoader.clearCache = function (url) {
            cache.remove(url);
        };
        repLoader.addToCache = function (url, m) {
            cache.put(url, m);
        };
        repLoader.getFile = function (url, mt, ignoreCache) {
            if (ignoreCache) {
                // clear cache of existing values
                cache.remove(url);
            }
            var config = {
                method: "GET",
                url: url,
                responseType: "blob",
                headers: { "Accept": mt },
                cache: cache
            };
            return $http(config)
                .then(function (promiseCallback) {
                return promiseCallback.data;
            })
                .catch(function (promiseCallback) {
                return handleError(promiseCallback);
            });
        };
        repLoader.uploadFile = function (url, mt, file) {
            var config = {
                method: "POST",
                url: url,
                data: file,
                headers: { "Content-Type": mt }
            };
            return $http(config)
                .then(function () {
                return true;
            })
                .catch(function () {
                return false;
            });
        };
        function logoff() {
            cache.removeAll();
        }
        $rootScope.$on(NakedObjects.geminiLogoffEvent, function () { return logoff(); });
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.representationloader.js.map