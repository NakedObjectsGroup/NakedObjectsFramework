/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    (function (FocusTarget) {
        FocusTarget[FocusTarget["Menu"] = 0] = "Menu";
        FocusTarget[FocusTarget["SubAction"] = 1] = "SubAction";
        FocusTarget[FocusTarget["Action"] = 2] = "Action";
        FocusTarget[FocusTarget["ObjectTitle"] = 3] = "ObjectTitle";
        FocusTarget[FocusTarget["Dialog"] = 4] = "Dialog";
        FocusTarget[FocusTarget["ListItem"] = 5] = "ListItem";
        FocusTarget[FocusTarget["Property"] = 6] = "Property";
        FocusTarget[FocusTarget["TableItem"] = 7] = "TableItem";
        FocusTarget[FocusTarget["Input"] = 8] = "Input";
        FocusTarget[FocusTarget["CheckBox"] = 9] = "CheckBox";
        FocusTarget[FocusTarget["MultiLineDialogRow"] = 10] = "MultiLineDialogRow";
    })(NakedObjects.FocusTarget || (NakedObjects.FocusTarget = {}));
    var FocusTarget = NakedObjects.FocusTarget;
    NakedObjects.app.service("focusManager", function ($timeout, $rootScope) {
        var helper = this;
        var currentTarget;
        var currentIndex;
        var override = false;
        var focusedPane = 1;
        helper.setCurrentPane = function (paneId) {
            focusedPane = paneId;
        };
        helper.focusOn = function (target, index, paneId, count) {
            if (count === void 0) { count = 0; }
            if (paneId === focusedPane) {
                if (!override) {
                    currentTarget = target;
                    currentIndex = index;
                }
                $timeout(function () {
                    $rootScope.$broadcast(NakedObjects.geminiFocusEvent, currentTarget, currentIndex, paneId, ++count);
                }, 100, false);
            }
        };
        helper.refresh = function (paneId) {
            focusedPane = paneId;
            helper.focusOn(currentTarget, currentIndex, paneId);
        };
        helper.focusOverrideOn = function (target, index, paneId) {
            focusedPane = paneId;
            override = true;
            currentTarget = target;
            currentIndex = index;
            helper.focusOn(target, index, paneId);
        };
        helper.focusOverrideOff = function () {
            override = false;
        };
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.focusmanager.js.map