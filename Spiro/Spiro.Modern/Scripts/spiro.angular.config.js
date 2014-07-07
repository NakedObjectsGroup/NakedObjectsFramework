/// <reference path="spiro.angular.app.ts" />
var Spiro;
(function (Spiro) {
    (function (Angular) {
        function getSvrPath() {
            var trimmedPath = Spiro.svrPath.trim();

            if (trimmedPath.length == 0 || trimmedPath.charAt(Spiro.svrPath.length - 1) == '/') {
                return trimmedPath;
            }
            return trimmedPath + '/';
        }

        // templates
        Angular.nestedCollectionTemplate = getSvrPath() + "Content/partials/nestedCollection.html";
        Angular.nestedCollectionTableTemplate = getSvrPath() + "Content/partials/nestedCollectionTable.html";
        Angular.nestedObjectTemplate = getSvrPath() + "Content/partials/nestedObject.html";
        Angular.dialogTemplate = getSvrPath() + "Content/partials/dialog.html";
        Angular.servicesTemplate = getSvrPath() + "Content/partials/services.html";
        Angular.serviceTemplate = getSvrPath() + "Content/partials/service.html";
        Angular.actionTemplate = getSvrPath() + "Content/partials/actions.html";
        Angular.errorTemplate = getSvrPath() + "Content/partials/error.html";
        Angular.appBarTemplate = getSvrPath() + "Content/partials/appbar.html";
        Angular.objectTemplate = getSvrPath() + "Content/partials/object.html";
        Angular.viewPropertiesTemplate = getSvrPath() + "Content/partials/viewProperties.html";
        Angular.editPropertiesTemplate = getSvrPath() + "Content/partials/editProperties.html";

        var servicesPageTemplate = getSvrPath() + 'Content/partials/servicesPage.html';
        var servicePageTemplate = getSvrPath() + 'Content/partials/servicePage.html';
        var objectPageTemplate = getSvrPath() + 'Content/partials/objectPage.html';
        var transientObjectPageTemplate = getSvrPath() + 'Content/partials/transientObjectPage.html';
        var errorPageTemplate = getSvrPath() + 'Content/partials/errorPage.html';

        Angular.app.config(function ($routeProvider) {
            $routeProvider.when('/services', {
                templateUrl: servicesPageTemplate,
                controller: 'BackgroundController'
            }).when('/services/:sid', {
                templateUrl: servicePageTemplate,
                controller: 'BackgroundController'
            }).when('/objects/:dt/:id', {
                templateUrl: objectPageTemplate,
                controller: 'BackgroundController'
            }).when('/objects/:dt', {
                templateUrl: transientObjectPageTemplate,
                controller: 'BackgroundController'
            }).when('/error', {
                templateUrl: errorPageTemplate,
                controller: 'BackgroundController'
            }).otherwise({
                redirectTo: '/services'
            });
        });

        Angular.app.run(function (color, mask) {
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
                "blue",
                "blueLight",
                "blueDark",
                "green",
                "greenLight",
                "greenDark",
                "red",
                "yellow",
                "orange",
                "orange",
                "orangeDark",
                "pink",
                "pinkDark",
                "purple",
                "grayDark",
                "magenta",
                "teal",
                "redLight"
            ]);

            color.setDefaultColor("darkBlue");

            // map to convert from mask representation in RO extension to client represention.
            mask.setMaskMap({
                "d": { name: "date", mask: "d MMM yyyy" }
            });
        });
    })(Spiro.Angular || (Spiro.Angular = {}));
    var Angular = Spiro.Angular;
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.angular.config.js.map
