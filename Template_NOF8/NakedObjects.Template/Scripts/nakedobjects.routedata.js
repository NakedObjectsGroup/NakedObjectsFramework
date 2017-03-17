/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="nakedobjects.models.ts" />
var NakedObjects;
(function (NakedObjects) {
    (function (ViewType) {
        ViewType[ViewType["Home"] = 0] = "Home";
        ViewType[ViewType["Object"] = 1] = "Object";
        ViewType[ViewType["List"] = 2] = "List";
    })(NakedObjects.ViewType || (NakedObjects.ViewType = {}));
    var ViewType = NakedObjects.ViewType;
    (function (CollectionViewState) {
        CollectionViewState[CollectionViewState["Summary"] = 0] = "Summary";
        CollectionViewState[CollectionViewState["List"] = 1] = "List";
        CollectionViewState[CollectionViewState["Table"] = 2] = "Table";
    })(NakedObjects.CollectionViewState || (NakedObjects.CollectionViewState = {}));
    var CollectionViewState = NakedObjects.CollectionViewState;
    (function (ApplicationMode) {
        ApplicationMode[ApplicationMode["Gemini"] = 0] = "Gemini";
        ApplicationMode[ApplicationMode["Cicero"] = 1] = "Cicero";
    })(NakedObjects.ApplicationMode || (NakedObjects.ApplicationMode = {}));
    var ApplicationMode = NakedObjects.ApplicationMode;
    (function (InteractionMode) {
        InteractionMode[InteractionMode["View"] = 0] = "View";
        InteractionMode[InteractionMode["Edit"] = 1] = "Edit";
        InteractionMode[InteractionMode["Transient"] = 2] = "Transient";
        InteractionMode[InteractionMode["Form"] = 3] = "Form";
        InteractionMode[InteractionMode["NotPersistent"] = 4] = "NotPersistent";
    })(NakedObjects.InteractionMode || (NakedObjects.InteractionMode = {}));
    var InteractionMode = NakedObjects.InteractionMode;
    var RouteData = (function () {
        function RouteData() {
            var _this = this;
            this.pane = function () { return [, _this.pane1, _this.pane2]; };
            this.pane1 = new PaneRouteData(1);
            this.pane2 = new PaneRouteData(2);
        }
        return RouteData;
    }());
    NakedObjects.RouteData = RouteData;
    var PaneRouteData = (function () {
        function PaneRouteData(paneId) {
            this.paneId = paneId;
            this.isNull = {
                condition: function (val) { return !val; },
                name: "is null"
            };
            this.isNotNull = {
                condition: function (val) { return val; },
                name: "is not null"
            };
            this.isLength0 = {
                condition: function (val) { return val && val.length === 0; },
                name: "is length 0"
            };
            this.isEmptyMap = {
                condition: function (val) { return _.keys(val).length === 0; },
                name: "is an empty map"
            };
        }
        PaneRouteData.prototype.isValid = function (name) {
            if (!this.hasOwnProperty(name)) {
                throw new Error(name + " is not a valid property on PaneRouteData");
            }
        };
        PaneRouteData.prototype.assertMustBe = function (context, name, contextCondition, valueCondition) {
            // make sure context and name are valid
            this.isValid(context);
            this.isValid(name);
            if (contextCondition.condition(this[context])) {
                if (!valueCondition.condition(this[name])) {
                    throw new Error("Expect that " + name + " " + valueCondition.name + " when " + context + " " + contextCondition.name + " within url \"" + this.validatingUrl + "\"");
                }
            }
        };
        PaneRouteData.prototype.assertMustBeEmptyOutsideContext = function (context, name) {
            this.assertMustBe(context, name, this.isNull, this.isEmptyMap);
        };
        PaneRouteData.prototype.assertMustBeNullOutsideContext = function (context, name) {
            this.assertMustBe(context, name, this.isNull, this.isNull);
        };
        PaneRouteData.prototype.assertMustBeNullInContext = function (context, name) {
            this.assertMustBe(context, name, this.isNotNull, this.isNull);
        };
        PaneRouteData.prototype.assertMustBeZeroLengthInContext = function (context, name) {
            this.assertMustBe(context, name, this.isNotNull, this.isLength0);
        };
        PaneRouteData.prototype.validate = function (url) {
            this.validatingUrl = url;
            if (NakedObjects.doUrlValidation) {
                // Can add more conditions here 
                this.assertMustBeNullInContext("objectId", "menuId");
                this.assertMustBeNullInContext("menuId", "objectId");
            }
        };
        return PaneRouteData;
    }());
    NakedObjects.PaneRouteData = PaneRouteData;
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.routedata.js.map