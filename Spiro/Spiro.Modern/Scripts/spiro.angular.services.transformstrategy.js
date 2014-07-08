/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        // this is the default transform strategy that flattens the object
        Angular.app.service('transformStrategy', function ($q) {
            var transformStrategy = this;

            function transformObject(rep) {
                var defer = $q.defer();

                var names = _.map(rep.propertyMembers(), function (v, n) {
                    return n;
                });
                var values = _.map(rep.propertyMembers(), function (v) {
                    return v.value().toString();
                });
                var result = _.object(names, values);
                result["nof_rep"] = rep;
                result["nof_url"] = "#" + "/" + rep.domainType() + "/" + rep.instanceId();

                defer.resolve(result);

                return defer.promise;
            }
            ;

            function transformActionResult(ar, loader) {
                var list = ar.result().list().value().models;
                var resultArray = [];

                _.each((list), function (link) {
                    resultArray.push(loader.loadAndTransform(link.getTarget().hateoasUrl, Spiro.DomainObjectRepresentation));
                });

                return $q.all(resultArray);
            }
            ;

            transformStrategy.transform = function (rep, loader) {
                if (rep instanceof Spiro.DomainObjectRepresentation) {
                    return transformObject(rep);
                }
                if (rep instanceof Spiro.ActionResultRepresentation) {
                    return transformActionResult(rep, loader);
                }

                var defer = $q.defer();
                defer.reject("not supported representation");
                return defer.promise;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular..services.transformstrategy.js.map
