using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using NakedObjects.Menu;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NakedFunctions.Rest.App.Demo
{
    public static class ModelConfig_NakedFunctionsPM
    {
        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        //public static Type[] DomainTypes() =>  typeof(Product).Assembly.GetTypes().Where(t => t.IsClass && !t.IsSealed).ToArray();
     
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

        //How to register ALL static types from an Assembly (getting the assembly via one type ensures it is loaded).
        //public static Type[] DomainFunctions() =>typeof(Product_MenuFunctions).Assembly.GetTypes().Where(t => t.IsClass && t.IsSealed && t.IsAbstract).ToArray();


        //Register static types that contain functions intended as user actions
        public static Type[] DomainFunctions() => new[] {
            typeof(Product_MenuFunctions),
            typeof(SpecialOffer_MenuFunctions),
            typeof(Employee_MenuFunctions),
            typeof(Customer_MenuFunctions),

            typeof(Product_Functions),
            typeof(SpecialOffer_Functions),
            typeof(Employee_Functions),
            typeof(Vendor_Functions)  //Testing a function contributed to an NO type
        };

        public static IMenu[] MainMenus(IMenuFactory mf) => new[] {
            mf.NewMenu("Products", "products", typeof(Product_MenuFunctions), true),
            mf.NewMenu("Special Offers", "offers", typeof(SpecialOffer_MenuFunctions)),
            mf.NewMenu("Employees", "employees", typeof(Employee_MenuFunctions)),
            mf.NewMenu("Customers NF","customersnf", typeof(Customer_MenuFunctions))
        };

        //Register services that can be used in returned Action<T> 
        public static Type[] Services() => new Type[] { };
    }
}
