/// <reference path="spiro.angular.app.ts" />

module Spiro.Angular {

    function getSvrPath() {
        var trimmedPath = svrPath.trim();

        if (trimmedPath.length == 0 || trimmedPath.charAt(svrPath.length - 1) == '/') {
            return trimmedPath;
        }
        return trimmedPath + '/';
    }

    // templates 
    export var nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
    export var nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
    export var nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
    export var dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
    export var servicesTemplate = getSvrPath() + "Content/partials/services.html";
    export var serviceTemplate = getSvrPath() + "Content/partials/service.html";
    export var actionTemplate = getSvrPath() + "Content/partials/actions.html";
    export var errorTemplate = getSvrPath() + "Content/partials/error.html";
    export var appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
    export var objectTemplate = getSvrPath() + "Content/partials/object.html";
    export var viewPropertiesTemplate = getSvrPath() + "Content/partials/viewProperties.html";
    export var editPropertiesTemplate = getSvrPath() + "Content/partials/editProperties.html";

    var servicesPageTemplate = getSvrPath() + 'Content/partials/servicesPage.html';
    var servicePageTemplate = getSvrPath() + 'Content/partials/servicePage.html';
    var objectPageTemplate = getSvrPath() + 'Content/partials/objectPage.html';
    var transientObjectPageTemplate = getSvrPath() + 'Content/partials/transientObjectPage.html';
    var errorPageTemplate = getSvrPath() + 'Content/partials/errorPage.html';

    app.config(($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider.
            when('/services', {
                templateUrl: servicesPageTemplate,
                controller: 'BackgroundController'
            }).
            when('/services/:sid', {
                templateUrl: servicePageTemplate,
                controller: 'BackgroundController'
            }).
            when('/objects/:dt/:id', {
                templateUrl: objectPageTemplate,
                controller: 'BackgroundController'
            }).
            when('/objects/:dt', {
                templateUrl: transientObjectPageTemplate,
                controller: 'BackgroundController'
            }).
            when('/error', {
                templateUrl: errorPageTemplate,
                controller: 'BackgroundController'
            }).
            otherwise({
                redirectTo: '/services'
            });

    });

    app.run((color : Spiro.Angular.IColor, mask : Spiro.Angular.IMask) => {
      
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

  