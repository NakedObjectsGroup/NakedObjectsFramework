using AdventureWorksModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace AdventureWorksFunctionalModel
{
    public class AWNakedFunctionsConfiguration
    {
        public static List<Type> DomainTypes() => new List<Type>
        {
            typeof(Product), typeof(SpecialOffer) //etc.
        };

        //Main menu functions would be registered like this (with no need for IMenu or other type)
        //N.B. Assumes that all actions for any given menu are defined on one (static) class. 
        //Sub-menuing implemented using MemberOrder(1, "SubMenuName") within that class.
        //Under the covers, and within the server project, this could be converted to the NO programming model via this code,
        //which does use IMenu and IMenuFactory:
        //static IMenu[] MainMenus2(IMenuFactory factory) => MainMenus().Select(m => factory.NewMenu(m.Value, true, m.Key)).ToArray();
        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)> {
            ("Products", typeof(ProductMenuFunctions)),
            ("Special Offers", typeof(SpecialOffer_MenuFunctions))
        };

        //Register static types that contain functions - all defined with 'extension method' syntax - that are to be contributed to a
        //specific domain type, IQueryable<DomainType>, or an interface implemented by domain types
        public static  List<Type> ObjectFunctions() => new List<Type> { typeof(ProductMenuFunctions), typeof(SpecialOffer_MenuFunctions) };
    }
}
