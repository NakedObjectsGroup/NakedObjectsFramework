/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    var getOtherPane = NakedObjects.Models.getOtherPane;
    NakedObjects.app.service("clickHandler", function () {
        var clickHandler = this;
        function leftRightClickHandler(currentPane, right) {
            if (right === void 0) { right = false; }
            return right ? 2 : 1;
        }
        function sameOtherClickHandler(currentPane, right) {
            if (right === void 0) { right = false; }
            return right ? getOtherPane(currentPane) : currentPane;
        }
        clickHandler.pane = sameOtherClickHandler;
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.clickhandler.js.map