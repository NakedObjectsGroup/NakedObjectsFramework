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
    import ToolBarViewModel = NakedObjects.Angular.Gemini.ToolBarViewModel; /* Declare app level module */
  
    export const app = angular.module("app", ["ngRoute"]);
    //export const app = angular.module("app", ["ngRoute", "ngTouch"]);

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
        toolBar: ToolBarViewModel;
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
    export const nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
    export const nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
    export const nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
    export const dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export const servicesTemplate = getSvrPath() + "Content/partials/services.html";
    export const serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export const errorTemplate = getSvrPath() + "Content/partials/error.html";
    export const appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export const nullTemplate = getSvrPath() + "Content/partials/null.html";

    var singleErrorPageTemplate = getSvrPath() + "Content/partials/singleErrorPage.html";

    //All Gemini2 templates below:
    const singleHomePageTemplate = getSvrPath() + "Content/partials/singleHomePage.html";
    const singleObjectPageTemplate = getSvrPath() + "Content/partials/singleObjectPage.html";
    const singleQueryPageTemplate = getSvrPath() + "Content/partials/singleQueryPage.html";
    const splitHomeHomePageTemplate = getSvrPath() + "Content/partials/splitHomeHomePage.html";
    const splitHomeObjectPageTemplate = getSvrPath() + "Content/partials/splitHomeObjectPage.html";
    const splitHomeQueryPageTemplate = getSvrPath() + "Content/partials/splitHomeQueryPage.html";
    const splitObjectHomePageTemplate = getSvrPath() + "Content/partials/splitObjectHomePage.html";
    const splitObjectObjectPageTemplate = getSvrPath() + "Content/partials/splitObjectObjectPage.html";
    const splitObjectQueryPageTemplate = getSvrPath() + "Content/partials/splitObjectQueryPage.html";
    const splitQueryHomePageTemplate = getSvrPath() + "Content/partials/splitQueryHomePage.html";
    const splitQueryObjectPageTemplate = getSvrPath() + "Content/partials/splitQueryObjectPage.html";
    const splitQueryQueryPageTemplate = getSvrPath() + "Content/partials/splitQueryQueryPage.html";

    export const blankTemplate = getSvrPath() + "Content/partials/blank.html";
    export const homeTemplate = getSvrPath() + "Content/partials/home.html";
    export const objectTemplate = getSvrPath() + "Content/partials/object.html";
    export const objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    export const objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    export const transientObjectTemplate = getSvrPath() + "Content/partials/transient.html";


    export const queryListTemplate = getSvrPath() + "Content/partials/queryList.html";
    export const queryTableTemplate = getSvrPath() + "Content/partials/queryTable.html";

    export const footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export const actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export const collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export const collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export const collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export const collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";


    Angular.app.config(($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider.
            
            //Gemini2 Urls below:
            when("/gemini/home", {
                templateUrl: singleHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object", {
                templateUrl: singleObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/query", {
                templateUrl: singleQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/home", {
                templateUrl: splitHomeHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/object", {
                templateUrl: splitHomeObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/query", {
                templateUrl: splitHomeQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/home", {
                templateUrl: splitObjectHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/object", {
                templateUrl: splitObjectObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/query", {
                templateUrl: splitObjectQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/query/home", {
                templateUrl: splitQueryHomePageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/query/object", {
                templateUrl: splitQueryObjectPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/query/query", {
                templateUrl: splitQueryQueryPageTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/error", {
                templateUrl: singleErrorPageTemplate,
                controller: "ErrorController"
            }).
            otherwise({
            redirectTo: "/gemini/home"
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