using AdventureWorksModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace AdventureWorksFunctionalModel
{
    public class AWNakedObjectsConfiguration
    {
        public static IList<string> ModelNamedspaces() => new[]
        {
            "AdventureWorksModel"
        };

        public static List<Type> DomainTypes() => new List<Type>
        {
            //Empty
        };

        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)> {
            //("Products", typeof(ProductMenuFunctions)),
            //("Special Offers", typeof(SpecialOffer_MenuFunctions))
        };

        public static List<Type> Services() => new List<Type> {  };


    }
}
