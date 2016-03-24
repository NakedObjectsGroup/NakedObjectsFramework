/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

module NakedObjects {

    const enum Colors {
        Blue,
        BlueLight,
        BlueDark,
        Green,
        GreenLight,
        GreenDark,
        Red,
        Yellow,
        Orange1,
        Orange2,
        OrangeDark,
        Pink,
        PinkDark,
        Purple,
        GrayDark,
        Magenta,
        Teal,
        RedLight,
        DarkBlue
    };


    app.run((color: IColor) => {
        //color.addType("AdventureWorksModel.CustomerRepository", 17);
        //color.addType("AdventureWorksModel.Individual", 1);
        //color.addType("AdventureWorksModel.OrderRepository", 3);
        //color.addType("AdventureWorksModel.SalesOrderDetail", 3);
        //color.addType("AdventureWorksModel.ProductRepository", 8);
        color.addType("AdventureWorksModel.Product", 1);
        //color.addType("AdventureWorksModel.ProductInventory", 8);
        //color.addType("AdventureWorksModel.ProductReview", 8);
        //color.addType("AdventureWorksModel.ProductModel", 7);
        //color.addType("AdventureWorksModel.ProductCategory", 17);
        //color.addType("AdventureWorksModel.ProductSubCategory", 6);
        //color.addType("AdventureWorksModel.EmployeeRepository", 0);
        //color.addType("AdventureWorksModel.Employee", 2);
        //color.addType("AdventureWorksModel.EmployeePayHistory", 0);
        //color.addType("AdventureWorksModel.EmployeeDepartmentHistory",0);
        //color.addType("AdventureWorksModel.SalesRepository", 13);
        color.addType("AdventureWorksModel.SalesPerson", 4);
        //color.addType("AdventureWorksModel.SpecialOfferRepository", 11);
        color.addType("AdventureWorksModel.SpecialOffer", 5);
        //color.addType("AdventureWorksModel.ContactRepository",16);
        //color.addType("AdventureWorksModel.Contact", 16);
        //color.addType("AdventureWorksModel.VendorRepository", 5);
        color.addType("AdventureWorksModel.Vendor", 2);
        //color.addType("AdventureWorksModel.PurchaseOrderRepository",14);
        //color.addType("AdventureWorksModel.PurchaseOrder", 14);
        //color.addType("AdventureWorksModel.WorkOrderRepository", 10);
        color.addType("AdventureWorksModel.WorkOrder", 3);
        //color.addType("AdventureWorksModel.OrderContributedActions", 18);
        //color.addType("AdventureWorksModel.CustomerContributedActions", 18);

        //const matchBusiness = /AdventureWorksModel.Business*/;

        //color.addMatch(matchBusiness, 8);

        //// matches Person and Store 
        //color.addSubtype("AdventureWorksModel.BusinessEntity", 6);

        //// matches SalesOrderHeader
        //color.addSubtype("AdventureWorksModel.ICreditCardCreator", 5);

        color.setDefault(0);
    });
}