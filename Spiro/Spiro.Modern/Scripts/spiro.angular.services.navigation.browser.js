/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
/// <reference path="spiro.angular.viewmodels.ts" />
/// <reference path="spiro.angular.app.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        Angular.app.service('navigation', function ($location, $routeParams) {
            var nav = this;

            nav.back = function () {
                if ($routeParams.resultObject || $routeParams.resultCollection) {
                    // looking at an action result = so go back two
                    parent.history.back(2);
                } else {
                    parent.history.back(1);
                }
            };

            nav.forward = function () {
                parent.history.forward(1);
            };

            nav.push = function () {
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.navigation.browser.js.map
