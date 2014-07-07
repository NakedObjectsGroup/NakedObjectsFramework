/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Angular.app.service('loaderWithTransform', function (repLoader, $q, transformStrategy) {
            var transformer = this;

            transformer.loadAndTransform = function tt(url, repType) {
                var obj = new repType({});
                obj.hateoasUrl = url;
                return repLoader.populate(obj).then(function (resource) {
                    return transformStrategy.transform(resource, transformer);
                });
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.loaderwithtransform.js.map
