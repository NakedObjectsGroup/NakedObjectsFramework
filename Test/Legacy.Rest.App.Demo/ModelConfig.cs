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
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Extensions.Configuration;
using NakedFramework.Menu;

namespace Legacy.Rest.App.Demo {
    public static class ModelConfig {

       // Unintrospected specs: AdventureWorksModel.SalesOrderHeader+SalesReasonCategories,AdventureWorksModel.Sales.QuickOrderForm,

        private static Type[] AllAdventureWorksTypes => 
            Assembly.GetAssembly(typeof(AssemblyHook)).GetTypes().
                     Where(t => t.IsPublic).
                     Where(t => t.Namespace == "AdventureWorksModel").
                     Where(t => !t.Name.EndsWith("Map")).
                     Append(typeof(SalesOrderHeader.SalesReasonCategories)).
                     ToArray();


        public static Type[] NOTypes => AllAdventureWorksTypes.Where(t =>! t.IsDefined(typeof(LegacyType), false)).ToArray();

        public static Type[] NOServices {
            get {
                return new[] {
                    typeof(CustomerRepository),
                    typeof(OrderRepository),
                    typeof(ProductRepository),
                    typeof(EmployeeRepository),
                    typeof(SalesRepository),
                    typeof(SpecialOfferRepository),
                    typeof(PersonRepository),
                    typeof(VendorRepository),
                    typeof(PurchaseOrderRepository),
                    typeof(WorkOrderRepository),
                    typeof(OrderContributedActions),
                    typeof(CustomerContributedActions),
                    typeof(SpecialOfferContributedActions),
                    typeof(ServiceWithNoVisibleActions)
                };
            }
        }


        public static Type[] LegacyTypes => AllAdventureWorksTypes.Where(t => t.IsDefined(typeof(LegacyType), false)).ToArray();

        public static Type[] LegacyServices => new Type[] { };


    public static Func<IConfiguration, DbContext> DbContextCreator => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static Func<IConfiguration, Microsoft.EntityFrameworkCore.DbContext> EFDbContextCreator => c => new AdventureWorksEFCoreContext(c.GetConnectionString("AdventureWorksContext"));


        /// <summary>
        ///     Return an array of IMenus (obtained via the factory, then configured) to
        ///     specify the Main Menus for the application. If none are returned then
        ///     the Main Menus will be derived automatically from the Services.
        /// </summary>
        public static IMenu[] MainMenus(IMenuFactory factory) {
            var customerMenu = factory.NewMenu<CustomerRepository>();
            CustomerRepository.Menu(customerMenu);
            var salesMenu = factory.NewMenu<SalesRepository>();
            SalesRepository.Menu(salesMenu);
            return new[] {
                customerMenu,
                factory.NewMenu<OrderRepository>(true),
                factory.NewMenu<ProductRepository>(true),
                factory.NewMenu<EmployeeRepository>(true),
                salesMenu,
                factory.NewMenu<SpecialOfferRepository>(true),
                factory.NewMenu<PersonRepository>(true),
                factory.NewMenu<VendorRepository>(true),
                factory.NewMenu<PurchaseOrderRepository>(true),
                factory.NewMenu<WorkOrderRepository>(true),
                factory.NewMenu<ServiceWithNoVisibleActions>(true, "Empty")
            };
        }
    }
}