/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />


module NakedObjects {

    export const app = angular.module("app", ["ngRoute"]);

    app.config(($routeProvider: ng.route.IRouteProvider) => {
        const singleHomeTemplate = getSvrPath() + "Content/partials/singleHome.html";
        const singleObjectTemplate = getSvrPath() + "Content/partials/singleObject.html";
        const singleListTemplate = getSvrPath() + "Content/partials/singleList.html";
        const singleRecentTemplate = getSvrPath() + "Content/partials/singleRecent.html";

        const splitHomeHomeTemplate = getSvrPath() + "Content/partials/splitHomeHome.html";
        const splitHomeObjectTemplate = getSvrPath() + "Content/partials/splitHomeObject.html";
        const splitHomeListTemplate = getSvrPath() + "Content/partials/splitHomeList.html";
        const splitHomeRecentTemplate = getSvrPath() + "Content/partials/splitHomeRecent.html";
        const splitObjectHomeTemplate = getSvrPath() + "Content/partials/splitObjectHome.html";
        const splitObjectObjectTemplate = getSvrPath() + "Content/partials/splitObjectObject.html";
        const splitObjectListTemplate = getSvrPath() + "Content/partials/splitObjectList.html";
        const splitObjectRecentTemplate = getSvrPath() + "Content/partials/splitObjectRecent.html";
        const splitListHomeTemplate = getSvrPath() + "Content/partials/splitListHome.html";
        const splitListObjectTemplate = getSvrPath() + "Content/partials/splitListObject.html";
        const splitListListTemplate = getSvrPath() + "Content/partials/splitListList.html";
        const splitListRecentTemplate = getSvrPath() + "Content/partials/splitListRecent.html";

        const splitRecentHomeTemplate = getSvrPath() + "Content/partials/splitRecentHome.html";
        const splitRecentObjectTemplate = getSvrPath() + "Content/partials/splitRecentObject.html";
        const splitRecentListTemplate = getSvrPath() + "Content/partials/splitRecentList.html";
        const splitRecentRecentTemplate = getSvrPath() + "Content/partials/splitRecentRecent.html";

        const singleErrorTemplate = getSvrPath() + "Content/partials/singleError.html";

        $routeProvider.

            //Gemini2 Urls below:
            when(`/${geminiPath}/${homePath}`, {
                templateUrl: singleHomeTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${objectPath}`, {
                templateUrl: singleObjectTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${listPath}`, {
                templateUrl: singleListTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${recentPath}`, {
                templateUrl: singleRecentTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${homePath}/${homePath}`, {
                templateUrl: splitHomeHomeTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${homePath}/${objectPath}`, {
                templateUrl: splitHomeObjectTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${homePath}/${listPath}`, {
                templateUrl: splitHomeListTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${homePath}/${recentPath}`, {
                templateUrl: splitHomeRecentTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${objectPath}/${homePath}`, {
                templateUrl: splitObjectHomeTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${objectPath}/${objectPath}`, {
                templateUrl: splitObjectObjectTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${objectPath}/${listPath}`, {
                templateUrl: splitObjectListTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${objectPath}/${recentPath}`, {
                templateUrl: splitObjectRecentTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${listPath}/${homePath}`, {
                templateUrl: splitListHomeTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${listPath}/${objectPath}`, {
                templateUrl: splitListObjectTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${listPath}/${listPath}`, {
                templateUrl: splitListListTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${listPath}/${recentPath}`, {
                templateUrl: splitListRecentTemplate,
                controller: "BackgroundController"
            }).

            when(`/${geminiPath}/${recentPath}/${homePath}`, {
                templateUrl: splitRecentHomeTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${recentPath}/${objectPath}`, {
                templateUrl: splitRecentObjectTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${recentPath}/${listPath}`, {
                templateUrl: splitRecentListTemplate,
                controller: "BackgroundController"
            }).
            when(`/${geminiPath}/${recentPath}/${recentPath}`, {
                templateUrl: splitRecentRecentTemplate,
                controller: "BackgroundController"
            }).


            when(`/${geminiPath}/${errorPath}`, {
                templateUrl: singleErrorTemplate,
                controller: "ErrorController"
            }).
            //Cicero
            when(`/${ciceroPath}/${homePath}`, {
                templateUrl: ciceroTemplate,
                controller: "CiceroHomeController"
            }).
            when(`/${ciceroPath}/${objectPath}`, {
                templateUrl: ciceroTemplate,
                controller: "CiceroObjectController"
            }).
            when(`/${ciceroPath}/${listPath}`, {
                templateUrl: ciceroTemplate,
                controller: "CiceroListController"
            }).
            when(`/${ciceroPath}/${errorPath}`, {
                templateUrl: ciceroTemplate,
                controller: "CiceroErrorController"
            }).
            otherwise({
                redirectTo: `/${geminiPath}/${homePath}`
            });


    });

    app.run(($cacheFactory: ng.ICacheFactoryService) => {
        $cacheFactory("recentlyViewed");
    });
}