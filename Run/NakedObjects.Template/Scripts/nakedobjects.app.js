/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app = angular.module("app", ["ngRoute"]);
    NakedObjects.app.config(function ($routeProvider) {
        var singleHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/singleHome.html";
        var singleObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/singleObject.html";
        var singleListTemplate = NakedObjects.getSvrPath() + "Content/partials/singleList.html";
        var singleRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/singleRecent.html";
        var splitHomeHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeHome.html";
        var splitHomeObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeObject.html";
        var splitHomeListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeList.html";
        var splitHomeRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeRecent.html";
        var splitObjectHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectHome.html";
        var splitObjectObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectObject.html";
        var splitObjectListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectList.html";
        var splitObjectRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectRecent.html";
        var splitListHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListHome.html";
        var splitListObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListObject.html";
        var splitListListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListList.html";
        var splitListRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListRecent.html";
        var splitRecentHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentHome.html";
        var splitRecentObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentObject.html";
        var splitRecentListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentList.html";
        var splitRecentRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentRecent.html";
        var singleErrorTemplate = NakedObjects.getSvrPath() + "Content/partials/singleError.html";
        $routeProvider.
            //Gemini2 Urls below:
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath, {
            templateUrl: singleHomeTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath, {
            templateUrl: singleObjectTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath, {
            templateUrl: singleListTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath, {
            templateUrl: singleRecentTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.homePath, {
            templateUrl: splitHomeHomeTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.objectPath, {
            templateUrl: splitHomeObjectTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.listPath, {
            templateUrl: splitHomeListTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.recentPath, {
            templateUrl: splitHomeRecentTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.homePath, {
            templateUrl: splitObjectHomeTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitObjectObjectTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.listPath, {
            templateUrl: splitObjectListTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitObjectRecentTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.homePath, {
            templateUrl: splitListHomeTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitListObjectTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.listPath, {
            templateUrl: splitListListTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitListRecentTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.homePath, {
            templateUrl: splitRecentHomeTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitRecentObjectTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.listPath, {
            templateUrl: splitRecentListTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitRecentRecentTemplate,
            controller: "BackgroundController"
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.errorPath, {
            templateUrl: singleErrorTemplate,
            controller: "ErrorController"
        }).
            //Cicero
            when("/" + NakedObjects.ciceroPath + "/" + NakedObjects.homePath, {
            templateUrl: NakedObjects.ciceroTemplate,
            controller: "CiceroHomeController"
        }).
            when("/" + NakedObjects.ciceroPath + "/" + NakedObjects.objectPath, {
            templateUrl: NakedObjects.ciceroTemplate,
            controller: "CiceroObjectController"
        }).
            when("/" + NakedObjects.ciceroPath + "/" + NakedObjects.listPath, {
            templateUrl: NakedObjects.ciceroTemplate,
            controller: "CiceroListController"
        }).
            when("/" + NakedObjects.ciceroPath + "/" + NakedObjects.errorPath, {
            templateUrl: NakedObjects.ciceroTemplate,
            controller: "CiceroErrorController"
        }).
            otherwise({
            redirectTo: "/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath
        });
    });
    NakedObjects.app.run(function ($cacheFactory) {
        $cacheFactory("recentlyViewed");
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.app.js.map