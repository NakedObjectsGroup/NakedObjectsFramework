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
    NakedObjects.app.service("repLoader", function ($http, $q, $rootScope, $cacheFactory) {
        var _this = this;
        var repLoader = this;
        var loadingCount = 0;
        function addIfMatchHeader(config, digest) {
            if (digest && (config.method === "POST" || config.method === "PUT" || config.method === "DELETE")) {
                config.headers = { "If-Match": digest };
            }
        }
        function handleError(promiseCallback) {
            var message;
            var category;
            var error = null;
            if (promiseCallback.status === HttpStatusCode.InternalServerError) {
                // this error contains an error representatation 
                var errorRep = new ErrorRepresentation();
                errorRep.populate(promiseCallback.data);
                category = ErrorCategory.HttpServerError;
                error = errorRep;
            }
            else {
                category = ErrorCategory.HttpClientError;
                message = promiseCallback.headers("warning") || "Unknown client HTTP error";
                if (promiseCallback.status === HttpStatusCode.BadRequest || promiseCallback.status === HttpStatusCode.UnprocessableEntity) {
                    // these errors should contain a map                            
                    error = new ErrorMap(promiseCallback.data, promiseCallback.status, message);
                }
            }
            var rr = new ErrorWrapper(category, promiseCallback.status, error);
            return $q.reject(rr);
        }
        function httpValidate(config) {
            $rootScope.$broadcast("ajax-change", ++loadingCount);
            return $http(config).
                then(function () {
                return $q.when(true);
            }).
                catch(function (promiseCallback) {
                return handleError(promiseCallback);
            }).
                finally(function () {
                $rootScope.$broadcast("ajax-change", --loadingCount);
            });
        }
        function httpPopulate(config, ignoreCache, response) {
            $rootScope.$broadcast("ajax-change", ++loadingCount);
            if (ignoreCache) {
                // clear cache of existing values
                $cacheFactory.get("$http").remove(config.url);
            }
            return $http(config).
                then(function (promiseCallback) {
                response.populate(promiseCallback.data);
                response.etagDigest = promiseCallback.headers("ETag");
                return $q.when(response);
            }).
                catch(function (promiseCallback) {
                return handleError(promiseCallback);
            }).
                finally(function () {
                $rootScope.$broadcast("ajax-change", --loadingCount);
            });
        }
        repLoader.populate = function (model, ignoreCache) {
            var response = model;
            var useCache = !ignoreCache;
            var config = {
                withCredentials: true,
                url: model.getUrl(),
                method: model.method,
                cache: useCache,
                data: model.getBody()
            };
            return httpPopulate(config, ignoreCache, response);
        };
        repLoader.retrieve = function (map, rc, digest) {
            var response = new rc();
            var config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: false,
                data: map.getBody()
            };
            addIfMatchHeader(config, digest);
            return httpPopulate(config, true, response);
        };
        repLoader.validate = function (map, digest) {
            var config = {
                withCredentials: true,
                url: map.getUrl(),
                method: map.method,
                cache: false,
                data: map.getBody()
            };
            addIfMatchHeader(config, digest);
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
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.representationloader.js.map