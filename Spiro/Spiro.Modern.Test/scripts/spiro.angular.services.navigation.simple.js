var Spiro;
(function (Spiro) {
    /// <reference path="typings/underscore/underscore.d.ts" />
    /// <reference path="spiro.models.ts" />
    /// <reference path="spiro.angular.viewmodels.ts" />
    /// <reference path="spiro.angular.app.ts" />
    /// <reference path="spiro.angular.config.ts" />
    (function (Angular) {
        Angular.app.service('navigation', function ($location) {
            var nav = this;
            var history = [];
            var index = -1;
            var navigating = false;

            nav.back = function () {
                if ((index - 1) >= 0) {
                    index--;
                    navigating = true;
                    $location.url(history[index]);
                }
            };
            nav.forward = function () {
                if ((index + 1) <= (history.length - 1)) {
                    index++;
                    navigating = true;
                    $location.url(history[index]);
                }
            };
            nav.push = function () {
                if (!navigating) {
                    var newUrl = $location.url();
                    var curUrl = history[history.length - 1];
                    var isActionUrl = newUrl.indexOf("?action") > 0;

                    if (!isActionUrl && newUrl != curUrl) {
                        history.push($location.url());
                    }

                    index = history.length - 1;
                }
                navigating = false;
            };
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.services.navigation.simple.js.map
