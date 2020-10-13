using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedFunctions.Rest.App.Demo
{
    public static class ModelConfig_NakedFunctionsPM
    {
        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static Type[] DomainTypes() => new[]
        {
            //Human Resources
            typeof(Employee),

            //Production 
            typeof(BillOfMaterial),
            typeof(Culture),
            typeof(Document),
            typeof(Illustration),
            typeof(Location),
            typeof(Product),
            typeof(ProductCategory),
            typeof(ProductCostHistory),
            typeof(ProductDescription),
            typeof(ProductDocument),
            typeof(ProductInventory),
            typeof(ProductListPriceHistory),
            typeof(ProductModel),
            typeof(ProductModelIllustration),
            typeof(ProductModelProductDescriptionCulture),
            typeof(ProductPhoto),
            typeof(ProductProductPhoto),
            typeof(ProductReview),
            typeof(ProductSubcategory),
            typeof(ScrapReason),
            typeof(TransactionHistory),
            typeof(UnitMeasure),
            typeof(WorkOrder),
            typeof(WorkOrderRouting),
            typeof(ProductLineEnum),
            typeof(ProductClassEnum),


            //Sales
            typeof(SpecialOffer),
        };


        //Register static types that contain functions - all defined with 'extension method' syntax - that are to be contributed to a
        //specific domain type, IQueryable<DomainType>, or an interface implemented by domain types
        public static Type[] ContributedFunctions() => new[] {
            typeof(Product_MenuFunctions),
            typeof(SpecialOffer_MenuFunctions),
            typeof(Employee_MenuFunctions),
            typeof(Customer_MenuFunctions),

            typeof(Product_Functions),
            typeof(SpecialOffer_Functions),
            typeof(Employee_Functions),
            typeof(Vendor_Functions)  //Testing a function contributed to an NO type
        };

        public static (string name, Type rootType)[] MainMenus() => new[] {
            ("Products", typeof(Product_MenuFunctions)),
            ("Special Offers", typeof(SpecialOffer_MenuFunctions)),
           ("Employees", typeof(Employee_MenuFunctions)),
           ("Customers NF", typeof(Customer_MenuFunctions)),
        };
    }
}
