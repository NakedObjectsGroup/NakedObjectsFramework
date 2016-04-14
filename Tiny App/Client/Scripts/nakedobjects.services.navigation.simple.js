/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.service("navigation", function ($location) {
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
                if (!isActionUrl && newUrl !== curUrl) {
                    history.push($location.url());
                }
                index = history.length - 1;
            }
            navigating = false;
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.navigation.simple.js.map