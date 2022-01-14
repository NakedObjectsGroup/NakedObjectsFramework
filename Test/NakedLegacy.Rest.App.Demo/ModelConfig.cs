// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NakedFramework.Menu;
using AdventureWorksModel;
using AW.Types;
using AW.Services;

namespace Legacy.Rest.App.Demo
{
    public static class ModelConfig
    {

        // Unintrospected specs: AdventureWorksModel.SalesOrderHeader+SalesReasonCategories,AdventureWorksModel.Sales.QuickOrderForm,

        private static Type[] AllAdventureWorksTypes =>
            Assembly.GetAssembly(typeof(AW.Types.Department)).GetTypes().
                     Where(t => t.IsPublic).
                     ToArray();


        public static Type[] DomainTypes => AllAdventureWorksTypes.Where(t => t.Namespace.StartsWith("AW")).ToArray();

        public static Type[] DomainServices => new Type[] {typeof(PersonRepository) };//AllAdventureWorksTypes.Where(t => t.Namespace.EndsWith("AW.Services")).ToArray();

        public static Func<IConfiguration, Microsoft.EntityFrameworkCore.DbContext> EFDbContextCreator => c => new AdventureWorksEFCoreContext(c.GetConnectionString("AdventureWorksContext"));


        /// <summary>
        ///     Return an array of IMenus (obtained via the factory, then configured) to
        ///     specify the Main Menus for the application. If none are returned then
        ///     the Main Menus will be derived automatically from the Services.
        /// </summary>
        //public static NakedLegacy.Menu.IMenu[] MainMenus(IMenuFactory factory) {
        //    return new NakedLegacy.Types.IMenu[] {
        //        customerMenu,
        //        factory.NewMenu<OrderRepository>(true),
        //        factory.NewMenu<ProductRepository>(true),
        //        factory.NewMenu<EmployeeRepository>(true),
        //        salesMenu,
        //        factory.NewMenu<SpecialOfferRepository>(true),
        //        factory.NewMenu<PersonRepository>(true),
        //        factory.NewMenu<VendorRepository>(true),
        //        factory.NewMenu<PurchaseOrderRepository>(true),
        //        factory.NewMenu<WorkOrderRepository>(true),
        //        factory.NewMenu<ServiceWithNoVisibleActions>(true, "Empty")
        //    };
        //}

        //For testing purposes only - not an elegant solution - TODO replace with proper menu creation
        private static NakedFramework.Menu.IMenu MakeMenu<T>(IMenuFactory factory)
        {
            var t = typeof(T);
            var m = factory.NewMenu(t, false, t.Name);
            var actions = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var action in actions)
            {
                m.AddAction(action.Name);
            }

            return m;
        }

        //public static NakedFramework.Menu.IMenu[] MainMenus(IMenuFactory factory) =>
        //    new[] {
        //            MakeMenu<Employees>(factory),
        //            MakeMenu<Persons>(factory),
        //            MakeMenu<Addresses>(factory),
        //            MakeMenu<Products>(factory),
        //            MakeMenu<Vendors>(factory),
        //            MakeMenu<SalesOrders>(factory),
        //            MakeMenu<PurchaseOrders>(factory),
        //            MakeMenu<SpecialOffers>(factory),
        //            MakeMenu<WorkOrders>(factory),
        //            MakeMenu<Cart>(factory),
        //    };
    }
}