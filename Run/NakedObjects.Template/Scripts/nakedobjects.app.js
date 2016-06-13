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
        var singleAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/singleAttachment.html";
        var splitHomeHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeHome.html";
        var splitHomeObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeObject.html";
        var splitHomeListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeList.html";
        var splitHomeRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeRecent.html";
        var splitHomeAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitHomeAttachment.html";
        var splitObjectHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectHome.html";
        var splitObjectObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectObject.html";
        var splitObjectListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectList.html";
        var splitObjectRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectRecent.html";
        var splitObjectAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitObjectAttachment.html";
        var splitListHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListHome.html";
        var splitListObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListObject.html";
        var splitListListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListList.html";
        var splitListRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListRecent.html";
        var splitListAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitListAttachment.html";
        var splitRecentHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentHome.html";
        var splitRecentObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentObject.html";
        var splitRecentListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentList.html";
        var splitRecentRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentRecent.html";
        var splitRecentAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitRecentAttachment.html";
        var splitAttachmentHomeTemplate = NakedObjects.getSvrPath() + "Content/partials/splitAttachmentHome.html";
        var splitAttachmentObjectTemplate = NakedObjects.getSvrPath() + "Content/partials/splitAttachmentObject.html";
        var splitAttachmentListTemplate = NakedObjects.getSvrPath() + "Content/partials/splitAttachmentList.html";
        var splitAttachmentRecentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitAttachmentRecent.html";
        var splitAttachmentAttachmentTemplate = NakedObjects.getSvrPath() + "Content/partials/splitAttachmentAttachment.html";
        var singleApplicationPropertiesTemplate = NakedObjects.getSvrPath() + "Content/partials/singleApplicationProperties.html";
        var singleErrorTemplate = NakedObjects.getSvrPath() + "Content/partials/singleError.html";
        $routeProvider.
            //Gemini2 Urls below:
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath, {
            templateUrl: singleHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath, {
            templateUrl: singleObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath, {
            templateUrl: singleListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath, {
            templateUrl: singleRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath, {
            templateUrl: singleAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.homePath, {
            templateUrl: splitHomeHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.objectPath, {
            templateUrl: splitHomeObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.listPath, {
            templateUrl: splitHomeListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.recentPath, {
            templateUrl: splitHomeRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.homePath + "/" + NakedObjects.attachmentPath, {
            templateUrl: splitHomeAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.homePath, {
            templateUrl: splitObjectHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitObjectObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.listPath, {
            templateUrl: splitObjectListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitObjectRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.objectPath + "/" + NakedObjects.attachmentPath, {
            templateUrl: splitObjectAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.homePath, {
            templateUrl: splitListHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitListObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.listPath, {
            templateUrl: splitListListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitListRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.listPath + "/" + NakedObjects.attachmentPath, {
            templateUrl: splitListAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.homePath, {
            templateUrl: splitRecentHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitRecentObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.listPath, {
            templateUrl: splitRecentListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitRecentRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.recentPath + "/" + NakedObjects.attachmentPath, {
            templateUrl: splitRecentAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath + "/" + NakedObjects.homePath, {
            templateUrl: splitAttachmentHomeTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath + "/" + NakedObjects.objectPath, {
            templateUrl: splitAttachmentObjectTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath + "/" + NakedObjects.listPath, {
            templateUrl: splitAttachmentListTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath + "/" + NakedObjects.recentPath, {
            templateUrl: splitAttachmentRecentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.attachmentPath + "/" + NakedObjects.attachmentPath, {
            templateUrl: splitAttachmentAttachmentTemplate,
            controller: "BackgroundController",
            reloadOnSearch: false
        }).
            when("/" + NakedObjects.geminiPath + "/" + NakedObjects.applicationPropertiesPath, {
            templateUrl: singleApplicationPropertiesTemplate,
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