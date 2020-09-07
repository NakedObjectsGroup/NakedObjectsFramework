using AdventureWorksModel;
using System;
using System.Collections.Generic;

namespace AdventureWorksFunctionalModel
{
    //Throwaway code exploring how the RunSettings should be configured
    public class NakedFunctionsRunSettings
    {


        //Main menu functions would be registered like this (with no need for IMenu or other type)
        //N.B. Assumes that all actions for any given menu are defined on one (static) class. 
        //Sub-menuing implemented using MemberOrder(1, "SubMenuName") within that class.
        public static IDictionary<string, Type> MenuFunctions() => new Dictionary<string, Type>()
        {
            ["Products"] = typeof(ProductRepository),
            ["Special Offers"] = typeof(SpecialOfferRepository),
        };

        //Under the covers, and within the server project, this could be converted to the NO programming model via this code,
        //which does use IMenu and IMenuFactory:
        //static IMenu[] MainMenus2(IMenuFactory factory) => MainMenus().Select(m => factory.NewMenu(m.Value, true, m.Key)).ToArray();
    }
}
