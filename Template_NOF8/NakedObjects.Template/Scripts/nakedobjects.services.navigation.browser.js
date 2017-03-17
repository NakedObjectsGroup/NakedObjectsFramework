/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.service("navigation", function () {
        var nav = this;
        nav.back = function () { return parent.history.back(); };
        nav.forward = function () { return parent.history.forward(); };
        nav.push = function () { };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.navigation.browser.js.map