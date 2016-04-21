/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />
/// <reference path="nakedobjects.app.ts" />
var NakedObjects;
(function (NakedObjects) {
    NakedObjects.app.run(function (color) {
        //Note: colour is determined by the FIRST matching rule
        var awm = "AdventureWorksModel.";
        //Match specific class
        color.addType(awm + "Customer", 1);
        color.addType(awm + "Product", 4);
        color.addType(awm + "Employee", 5);
        color.addType(awm + "SalesPerson", 6);
        color.addType(awm + "SpecialOffer", 7);
        color.addType(awm + "Vendor", 9);
        color.addType(awm + "WorkOrder", 12);
        //Match Regex on name
        color.addMatch(/.*SalesOrder.*/, 2); //Matches ..Header & ..Detail
        color.addMatch(/.*PurchaseOrder.*/, 10); //Matches ..Header & ..Detail
        //Match on sub-type
        color.addSubtype(awm + "BusinessEntity", 8); //Matches Person and Store
        //Default colour -  must be specified last
        color.setDefault(0);
    });
})(NakedObjects || (NakedObjects = {}));
//# sourceMappingURL=nakedobjects.services.color.config.js.map