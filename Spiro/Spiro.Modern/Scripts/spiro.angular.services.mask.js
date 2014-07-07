/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.app.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Angular.app.service('mask', function () {
            var mask = this;
            var maskMap = {};

            mask.toLocalFilter = function (remoteMask) {
                return maskMap ? maskMap[remoteMask] : null;
            };

            mask.setMaskMap = function (map) {
                maskMap = map;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.mask.js.map
