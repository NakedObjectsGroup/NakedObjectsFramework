/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />

module NakedObjects {

    app.run((color: IColor) => {

        color.addType("AdventureWorksModel.Customer", 1);
        color.addType("AdventureWorksModel.Individual", 1);
        color.addType("AdventureWorksModel.Store", 1);
        color.addType("AdventureWorksModel.SalesOrderHeader", 2);
        color.addType("AdventureWorksModel.SalesOrderDetail", 3);
        color.addType("AdventureWorksModel.Product", 4);
        color.addType("AdventureWorksModel.Employee", 5);
        color.addType("AdventureWorksModel.SalesPerson", 6);
        color.addType("AdventureWorksModel.SpecialOffer", 7);
        color.addType("AdventureWorksModel.Person", 8);
        color.addType("AdventureWorksModel.Vendor", 9);
        color.addType("AdventureWorksModel.PurchaseOrderHeader", 10);
        color.addType("AdventureWorksModel.PurchaseOrderHeader", 11);
        color.addType("AdventureWorksModel.WorkOrder", 12);

        //const matchBusiness = /AdventureWorksModel.Business*/;

        //color.addMatch(matchBusiness, 8);

        //// matches Person and Store 
        //color.addSubtype("AdventureWorksModel.BusinessEntity", 6);

        //// matches SalesOrderHeader
        //color.addSubtype("AdventureWorksModel.ICreditCardCreator", 5);

        color.setDefault(0);
    });
}