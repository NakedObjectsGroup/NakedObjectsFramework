
module Spiro.Angular {

    // Custom configuration for a particular implementation - example config included for 'Adventurework' application. 

    export interface ColorMapInterface {
        [index: string]: string;
    }

    // map of domain classes to colors  
    export var colorMap : ColorMapInterface = {
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
    };

    // array of colors for allocated colors by default
    export var defaultColorArray = [
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
    ];

    export var defaultColor : string = "darkBlue";

    export interface ILocalFilter {
        name: string;
        mask: string;
    }

    export interface IMaskMap {
        [index: string]: ILocalFilter;
    }

    // map to convert from mask representation in RO extension to client represention.
    export var maskMap : IMaskMap = {
        "d": { name: "date", mask: "d MMM yyyy" }
    };
}

  