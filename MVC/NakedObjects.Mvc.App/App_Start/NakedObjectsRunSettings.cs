// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using System.Collections.Generic;
using NakedObjects.Meta.Authorization;

namespace NakedObjects.Mvc.App {
    /// <summary>
    /// Use this class to configure the application running under Naked Objects
    /// </summary>
    public static class NakedObjectsRunSettings {

        private static string[] ModelNamespaces {
            get {
                return AllPersistedTypesInMainModel().Select(t => t.Namespace).Distinct().ToArray();
            }
        }

        private static Type[] Services {
            get {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                    typeof (ContactRepository),
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository),
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions),
                    typeof (SimpleEncryptDecrypt)
                };
            }
        }

        private static Type[] Types {
            get {
                return new[] {
                    //These types must be registered because they are defined in
                    //NakedObjects.Mvc, not in Core.
                    typeof (ActionResultModelQ<>),
                    typeof (ActionResultModel<>),
                    //Add any types here that cannot be reached by traversing model from the registered services
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof (QuickOrderForm)
                };
            }
        }

        private static Type[] AllPersistedTypesInMainModel() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => (t.BaseType == typeof(AWDomainObject)) && !t.IsAbstract).ToArray();
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingEdmxContext("Model").AssociateTypes(AllPersistedTypesInMainModel);
            config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] {typeof (AWDomainObject)});
            return config;
        }

        public static IAuditConfiguration AuditConfig() {
            return null;
        }

        public static IAuthorizationConfiguration AuthorizationConfig() {
                return null;
        }

        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 


        /// <summary>
        /// Return an array of IMenus (obtained via the factory, then configured) to
        /// specify the Main Menus for the application. If none are returned then
        /// the Main Menus will be derived automatically from the Services.
        /// </summary>
        public static IMenu[] MainMenus(IMenuFactory factory) {
            var customerMenu = factory.NewMenu<CustomerRepository>(false);
            CustomerRepository.Menu(customerMenu);
            return new IMenu[] {
                    customerMenu,
                    factory.NewMenu<OrderRepository>(true),
                    factory.NewMenu<ProductRepository>(true),
                    factory.NewMenu<EmployeeRepository>(true),
                    factory.NewMenu<SalesRepository>(true),
                    factory.NewMenu<SpecialOfferRepository>(true),
                    factory.NewMenu<ContactRepository>(true),
                    factory.NewMenu<VendorRepository>(true),
                    factory.NewMenu<PurchaseOrderRepository>(true),
                    factory.NewMenu<WorkOrderRepository>(true),
                    factory.NewMenu<object>(false, "Empty")
            };
        }
    }
}