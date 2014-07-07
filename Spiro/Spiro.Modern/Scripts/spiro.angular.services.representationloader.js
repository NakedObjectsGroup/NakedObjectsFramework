/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.app.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        // TODO investigate using transformations to transform results
        Angular.app.service("repLoader", function ($http, $q, $rootScope) {
            var repLoader = this;
            repLoader.loadingCount = 0;

            function getUrl(model) {
                var url = model.url();

                if (model.method === "GET" || model.method === "DELETE") {
                    var asJson = _.clone(model.attributes);

                    if (_.toArray(asJson).length > 0) {
                        var map = JSON.stringify(asJson);
                        var encodedMap = encodeURI(map);
                        url += "?" + encodedMap;
                    }
                }

                return url;
            }

            function getData(model) {
                var data = {};

                if (model.method === "POST" || model.method === "PUT") {
                    data = _.clone(model.attributes);
                }

                return data;
            }

            repLoader.populate = function (model, ignoreCache, expected) {
                var response = expected || model;
                var useCache = !ignoreCache;

                var delay = $q.defer();

                var config = {
                    url: getUrl(model),
                    method: model.method,
                    cache: useCache,
                    data: getData(model)
                };

                $rootScope.$broadcast("ajax-change", ++repLoader.loadingCount);
                $http(config).success(function (data, status, headers, config) {
                    response.attributes = data; // TODO make typed
                    delay.resolve(response);
                    $rootScope.$broadcast("ajax-change", --repLoader.loadingCount);
                }).error(function (data, status, headers, config) {
                    if (status === 500) {
                        var error = new Spiro.ErrorRepresentation(data);
                        delay.reject(error);
                    } else if (status === 400 || status === 422) {
                        var errorMap = new Spiro.ErrorMap(data, status, headers().warning);
                        delay.reject(errorMap);
                    } else {
                        delay.reject(headers().warning);
                    }
                    $rootScope.$broadcast("ajax-change", --repLoader.loadingCount);
                });

                return delay.promise;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.representationloader.js.map
