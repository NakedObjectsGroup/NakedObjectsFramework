var Spiro;
(function (Spiro) {
    /// <reference path="typings/underscore/underscore.d.ts" />
    /// <reference path="spiro.models.ts" />
    /// <reference path="spiro.angular.viewmodels.ts" />
    /// <reference path="spiro.angular.app.ts" />
    /// <reference path="spiro.angular.config.ts" />
    (function (Angular) {
        Angular.app.service('navigation', function ($location, $routeParams) {
            var nav = this;

            nav.back = function () {
                parent.history.back(1);

                if ($routeParams.resultObject || $routeParams.resultCollection) {
                    // looking at an action result = so go back two
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
