//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />

module NakedObjects.Angular {
    import AppBarViewModel = NakedObjects.Angular.Gemini.AppBarViewModel; /* Declare app level module */
   
    export var app = angular.module("app", ["ngRoute", "ngTouch"]);

    export interface INakedObjectsRouteParams extends ng.route.IRouteParamsService {
        menu1: string;
        dialog1: string;
        object1: string;
        action1: string;
        collection1: string;
        edit1 : string;
    }

    export interface INakedObjectsScope extends ng.IScope {
        backgroundColor: string;
        menus: Angular.Gemini.MenusViewModel;
        homeTemplate: string;
        actionsTemplate: string;
        // todo this is ugly - fix 
        object: { actions: Angular.Gemini.ActionViewModel[] };
        dialogTemplate: string;
        dialog: Angular.Gemini.DialogViewModel;
        error: Angular.Gemini.ErrorViewModel;
        errorTemplate: string;
        queryTemplate: string;
        collection: Angular.Gemini.CollectionViewModel;
        title: string;
        appBar: AppBarViewModel;
        objectTemplate: string;
        collectionsTemplate: string;
    }


    function getSvrPath() {
        const trimmedPath = svrPath.trim();
        if (trimmedPath.length === 0 || trimmedPath.charAt(svrPath.length - 1) === "/") {
            return trimmedPath;
        }
        return trimmedPath + "/";
    }

    // templates 
    export var nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
    export var nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
    export var nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
    export var dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export var servicesTemplate = getSvrPath() + "Content/partials/services.html";
    export var serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export var actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export var errorTemplate = getSvrPath() + "Content/partials/error.html";
    export var appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export var nullTemplate = getSvrPath() + "Content/partials/null.html";

    var servicesPageTemplate = getSvrPath() + "Content/partials/servicesPage.html";
    var servicePageTemplate = getSvrPath() + "Content/partials/servicePage.html";
    var objectPageTemplate = getSvrPath() + "Content/partials/objectPage.html";
    var transientObjectPageTemplate = getSvrPath() + "Content/partials/transientObjectPage.html";
    var singleErrorPageTemplate = getSvrPath() + "Content/partials/singleErrorPage.html";

    //All Gemini2 templates below:
    var singleHomePageTemplate = getSvrPath() + "Content/partials/singleHomePage.html";
    var singleObjectPageTemplate = getSvrPath() + "Content/partials/singleObjectPage.html";
    var singleQueryPageTemplate = getSvrPath() + "Content/partials/singleQueryPage.html";
    var splitHomeHomePageTemplate = getSvrPath() + "Content/partials/splitHomeHomePage.html";
    var splitHomeObjectPageTemplate = getSvrPath() + "Content/partials/splitHomeObjectPage.html";
    var splitHomeQueryPageTemplate = getSvrPath() + "Content/partials/splitHomeQueryPage.html";
    var splitObjectHomePageTemplate = getSvrPath() + "Content/partials/splitObjectHomePage.html";
    var splitObjectObjectPageTemplate = getSvrPath() + "Content/partials/splitObjectObjectPage.html";
    var splitObjectQueryPageTemplate = getSvrPath() + "Content/partials/splitObjectQueryPage.html";
    var splitQueryHomePageTemplate = getSvrPath() + "Content/partials/splitQueryHomePage.html";
    var splitQueryObjectPageTemplate = getSvrPath() + "Content/partials/splitQueryObjectPage.html";
    var splitQueryQueryPageTemplate = getSvrPath() + "Content/partials/splitQueryQueryPage.html";

    export var homeTemplate = getSvrPath() + "Content/partials/home.html";
    export var objectTemplate = getSvrPath() + "Content/partials/object.html";
    export var objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    export var objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    export var transientObjectTemplate = getSvrPath() + "Content/partials/transient.html";


    export var queryListTemplate = getSvrPath() + "Content/partials/queryList.html";
    export var queryTableTemplate = getSvrPath() + "Content/partials/queryTable.html";


    export var footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export var actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export var collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export var collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export var collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export var collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";


    Angular.app.config(($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider.
            
            //Gemini2 Urls below:
            when("/home", {
                templateUrl: singleHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/object", {
                templateUrl: singleObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/query", {
                templateUrl: singleQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/home/home", {
                templateUrl: splitHomeHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/home/object", {
                templateUrl: splitHomeObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/home/query", {
                templateUrl: splitHomeQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/object/home", {
                templateUrl: splitObjectHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/object/object", {
                templateUrl: splitObjectObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/object/query", {
                templateUrl: splitObjectQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/query/home", {
                templateUrl: splitQueryHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/query/object", {
                templateUrl: splitQueryObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/query/query", {
                templateUrl: splitQueryQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/error", {
                templateUrl: singleErrorPageTemplate,
                controller: "ErrorController"
            }).
            //TODO: change default to /home when Gemini2 is complete
            otherwise({
                redirectTo: "/home"
            });
       
    });

    app.run((color: IColor, mask: IMask, $cacheFactory) => {

        $cacheFactory("recentlyViewed");

        color.setColorMap({
            "AdventureWorksModel.CustomerRepository": "redLight",
            "AdventureWorksModel.Store": "red",
            "AdventureWorksModel.Individual": "red",
            "AdventureWorksModel.OrderRepository": "green",
            "AdventureWorksModel.SalesOrderHeader": "greenDark",
            "AdventureWorksModel.SalesOrderDetail": "green",
            "AdventureWorksModel.ProductRepository": "orange",
            "AdventureWorksModel.Product": "orangeDark",
            "AdventureWorksModel.ProductInventory": "orange",
            "AdventureWorksModel.ProductReview": "orange",
            "AdventureWorksModel.ProductModel": "yellow",
            "AdventureWorksModel.ProductCategory": "redLight",
            "AdventureWorksModel.ProductSubCategory": "red",
            "AdventureWorksModel.EmployeeRepository": "blue",
            "AdventureWorksModel.Employee": "blueDark",
            "AdventureWorksModel.EmployeePayHistory": "blue",
            "AdventureWorksModel.EmployeeDepartmentHistory": "blue",
            "AdventureWorksModel.SalesRepository": "purple",
            "AdventureWorksModel.SalesPerson": "purple",
            "AdventureWorksModel.SpecialOfferRepository": "pink",
            "AdventureWorksModel.SpecialOffer": "pinkDark",
            "AdventureWorksModel.ContactRepository": "teal",
            "AdventureWorksModel.Contact": "teal",
            "AdventureWorksModel.VendorRepository": "greenDark",
            "AdventureWorksModel.Vendor": "greenDark",
            "AdventureWorksModel.PurchaseOrderRepository": "grayDark",
            "AdventureWorksModel.PurchaseOrder": "grayDark",
            "AdventureWorksModel.WorkOrderRepository": "orangeDark",
            "AdventureWorksModel.WorkOrder": "orangeDark",
            "AdventureWorksModel.OrderContributedActions": "darkBlue",
            "AdventureWorksModel.CustomerContributedActions": "darkBlue"
        });

        color.setDefaultColorArray([
            "blue", //0
            "blueLight", //1
            "blueDark", //2
            "green", //3
            "greenLight", //4
            "greenDark", //5
            "red", //6
            "yellow", //7
            "orange", //8
            "orange", //9
            "orangeDark", //10
            "pink", //11
            "pinkDark", //12
            "purple", //13
            "grayDark", //14
            "magenta", //15
            "teal", //16
            "redLight" //17
        ]);

        color.setDefaultColor("darkBlue");

        // map to convert from mask representation in RO extension to client represention.
        mask.setMaskMap({
            "d": { name: "date", mask: "d MMM yyyy" }
        });
    });
}