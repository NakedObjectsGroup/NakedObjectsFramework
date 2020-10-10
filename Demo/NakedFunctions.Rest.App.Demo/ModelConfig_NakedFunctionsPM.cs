using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedFunctions.Rest.App.Demo
{
    public static class ModelConfig_NakedFunctionsPM
    {
        public static Func<IConfiguration, DbContext> DbContextInstaller =>  c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static List<Type> DomainTypes() => new List<Type>
        {
            typeof(Product), typeof(SpecialOffer), typeof(Employee)
        };


        //Register static types that contain functions - all defined with 'extension method' syntax - that are to be contributed to a
        //specific domain type, IQueryable<DomainType>, or an interface implemented by domain types
        public static List<Type> ContributedFunctions() => new List<Type> {
            typeof(Product_MenuFunctions), 
            typeof(SpecialOffer_MenuFunctions),
            typeof(Employee_MenuFunctions),
            typeof(Customer_MenuFunctions),

            typeof(Product_Functions), 
            typeof(SpecialOffer_Functions), 
            typeof(Employee_Functions)
        };

        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)> {
            ("Products", typeof(Product_MenuFunctions)),
            ("Special Offers", typeof(SpecialOffer_MenuFunctions)),
           ("Employees", typeof(Employee_MenuFunctions)),
           ("Customers NF", typeof(Customer_MenuFunctions)),
        };
    }
}
