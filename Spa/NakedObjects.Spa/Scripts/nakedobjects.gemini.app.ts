/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/angularjs/angular-route.d.ts" />

module NakedObjects.Angular {

    // consts to control app 
    
    export const defaultPageSize = 20; // can be overriden by server 
    export const listCacheSize = 5;

    import ToolBarViewModel = NakedObjects.Angular.Gemini.ToolBarViewModel; /* Declare app level module */
  
    export const app = angular.module("app", ["ngRoute"]);
    //export const app = angular.module("app", ["ngRoute", "ngTouch"]);

    export const shortCutMarker = "___";
    export const urlShortCuts = ["http://nakedobjectsrodemo.azurewebsites.net", "AdventureWorksModel"];

    export interface INakedObjectsScope extends ng.IScope {
        backgroundColor: string;
        menus: Angular.Gemini.MenusViewModel;
        homeTemplate: string;
        actionsTemplate: string;
        object: Angular.Gemini.DomainObjectViewModel;
        menu: Angular.Gemini.MenuViewModel;
        dialogTemplate: string;
        dialog: Angular.Gemini.DialogViewModel;
        error: Angular.Gemini.ErrorViewModel;
        errorTemplate: string;
        listTemplate: string;
        collection: Angular.Gemini.ListViewModel;
        collectionPlaceholder: Angular.Gemini.CollectionPlaceholderViewModel;

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
    export const concurrencyTemplate = getSvrPath() + "Content/partials/concurrencyError.html";
    export const httpErrorTemplate = getSvrPath() + "Content/partials/httpError.html";
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
    export const formTemplate = getSvrPath() + "Content/partials/form.html";
    export const expiredTransientTemplate = getSvrPath() + "Content/partials/expiredTransient.html";

    export const listPlaceholderTemplate = getSvrPath() + "Content/partials/ListPlaceholder.html";
    export const listTemplate = getSvrPath() + "Content/partials/List.html";
    export const listAsTableTemplate = getSvrPath() + "Content/partials/ListAsTable.html";

    export const footerTemplate = getSvrPath() + "Content/partials/footer.html";
    export const actionsTemplate = getSvrPath() + "Content/partials/actions.html";
    export const formActionsTemplate = getSvrPath() + "Content/partials/formActions.html";
    export const collectionsTemplate = getSvrPath() + "Content/partials/collections.html";
    export const collectionSummaryTemplate = getSvrPath() + "Content/partials/collectionSummary.html";
    export const collectionListTemplate = getSvrPath() + "Content/partials/collectionList.html";
    export const collectionTableTemplate = getSvrPath() + "Content/partials/collectionTable.html";

    //Cicero
    export const ciceroTemplate = getSvrPath() + "Content/partials/cicero.html";


    // routing constants 

    export const geminiPath = "gemini";
    export const ciceroPath = "cicero";
    export const homePath = "home";
    export const objectPath = "object";
    export const listPath = "list";
    export const errorPath = "error";

    //Restful Objects constants
    export const roDomainType = "x-ro-domain-type";
    export const roInvalidReason = "x-ro-invalidReason";
    export const roSearchTerm = "x-ro-searchTerm";
    export const roPage = "x-ro-page";
    export const roPageSize = "x-ro-pageSize";

    //NOF custom RO constants
    export const nofChoices ="x-ro-nof-choices";
    export const nofMenuPath = "x-ro-nof-menuPath";
    export const nofMask = "x-ro-nof-mask";
    export const nofRenderInEditMode = "x-ro-nof-renderInEditMode";

    export const nofTableViewTitle = "x-ro-nof-tableViewTitle";
    export const nofTableViewColumns = "x-ro-nof-tableViewColumns";
    export const nofMultipleLines = "x-ro-nof-multipleLines";
    export const nofWarnings = "x-ro-nof-warnings";
    export const nofMessages = "x-ro-nof-messages";
        
    Angular.app.config(($routeProvider: ng.route.IRouteProvider) => {
    

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
        //mask.setMaskMapping( "d", "date" ,"d MMM yyyy", "+0000");
 
        mask.setMaskMapping("C", "number", "2", "");
        mask.setMaskMapping("c", "number", "2", "");


    });
}