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
        listTemplate: string;
        collection: Angular.Gemini.CollectionViewModel;
        title: string;
        toolBar: ToolBarViewModel;
        objectTemplate: string;
        collectionsTemplate: string;
        cicero: Angular.Gemini.CiceroViewModel;
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

    var singleErrorTemplate = getSvrPath() + "Content/partials/singleError.html";

    //All Gemini2 templates below:
    const singleHomeTemplate = getSvrPath() + "Content/partials/singleHome.html";
    const singleObjectTemplate = getSvrPath() + "Content/partials/singleObject.html";
    const singleListTemplate = getSvrPath() + "Content/partials/singleList.html";
    const splitHomeHomeTemplate = getSvrPath() + "Content/partials/splitHomeHome.html";
    const splitHomeObjectTemplate = getSvrPath() + "Content/partials/splitHomeObject.html";
    const splitHomeListTemplate = getSvrPath() + "Content/partials/splitHomeList.html";
    const splitObjectHomeTemplate = getSvrPath() + "Content/partials/splitObjectHome.html";
    const splitObjectObjectTemplate = getSvrPath() + "Content/partials/splitObjectObject.html";
    const splitObjectListTemplate = getSvrPath() + "Content/partials/splitObjectList.html";
    const splitListHomeTemplate = getSvrPath() + "Content/partials/splitListHome.html";
    const splitListObjectTemplate = getSvrPath() + "Content/partials/splitListObject.html";
    const splitListListTemplate = getSvrPath() + "Content/partials/splitListList.html";

    export const blankTemplate = getSvrPath() + "Content/partials/blank.html";
    export const homeTemplate = getSvrPath() + "Content/partials/home.html";
    export const objectTemplate = getSvrPath() + "Content/partials/object.html";
    export const objectViewTemplate = getSvrPath() + "Content/partials/objectView.html";
    export const objectEditTemplate = getSvrPath() + "Content/partials/objectEdit.html";
    export const transientObjectTemplate = getSvrPath() + "Content/partials/transient.html";


    export const ListTemplate = getSvrPath() + "Content/partials/List.html";
    export const ListAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";

    export const footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export const actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export const collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export const collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export const collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export const collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";

    //Cicero
    export const ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";

    Angular.app.config(($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider.
            
            //Gemini2 Urls below:
            when("/gemini/home", {
                templateUrl: singleHomeTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object", {
                templateUrl: singleObjectTemplate,
                controller: "BackgroundController"
            }).
            //TODO: Remove
            when("/gemini/query", { 
                templateUrl: singleListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/list", {
                templateUrl: singleListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/home", {
                templateUrl: splitHomeHomeTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/object", {
                templateUrl: splitHomeObjectTemplate,
                controller: "BackgroundController"
            }).
            //TODO: remove
            when("/gemini/home/query", { 
                templateUrl: splitHomeListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/home/list", {
                templateUrl: splitHomeListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/home", {
                templateUrl: splitObjectHomeTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/object", {
                templateUrl: splitObjectObjectTemplate,
                controller: "BackgroundController"
            }).
            //TODO: remove
            when("/gemini/object/query", {
                templateUrl: splitObjectListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/object/list", {
                templateUrl: splitObjectListTemplate,
                controller: "BackgroundController"
            }).
             //TODO: remove
            when("/gemini/query/home", {
                templateUrl: splitListHomeTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/list/home", {
                templateUrl: splitListHomeTemplate,
                controller: "BackgroundController"
            }).
            //TODO: remove
            when("/gemini/query/object", {
                templateUrl: splitListObjectTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/list/object", {
                templateUrl: splitListObjectTemplate,
                controller: "BackgroundController"
            }).
            //TODO: remove
            when("/gemini/query/query", {
                templateUrl: splitListListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/list/list", {
                templateUrl: splitListListTemplate,
                controller: "BackgroundController"
            }).
            when("/gemini/error", {
                templateUrl: singleErrorTemplate,
                controller: "ErrorController"
            }).
            //Cicero
            when("/cicero/home", {
                templateUrl: ciceroTemplate,
                controller: "CiceroController"
            }).
            when("/cicero/object", {
                templateUrl: ciceroTemplate,
                controller: "CiceroController"
            }).
            //TODO: remove
            when("/cicero/query", {
                templateUrl: ciceroTemplate,
                controller: "CiceroController"
            }).
            when("/cicero/list", {
                templateUrl: ciceroTemplate,
                controller: "CiceroController"
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