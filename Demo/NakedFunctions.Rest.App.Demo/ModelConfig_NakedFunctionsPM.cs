using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedObjects.Rest.App.Demo
{
    public static class ModelConfig_NakedFunctionsPM
    {
        public static Func<IConfiguration, DbContext> ContextInstaller =>  c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static List<Type> DomainTypes() => new List<Type>
        {
            typeof(Product), typeof(SpecialOffer) //etc.
        };


        //Register static types that contain functions - all defined with 'extension method' syntax - that are to be contributed to a
        //specific domain type, IQueryable<DomainType>, or an interface implemented by domain types
        public static List<Type> ObjectFunctions() => new List<Type> { typeof(ProductMenuFunctions), typeof(SpecialOffer_MenuFunctions) };

        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)> {
            ("Products", typeof(ProductMenuFunctions)),
            ("Special Offers", typeof(SpecialOffer_MenuFunctions))
        };
    }
}
