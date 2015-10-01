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
using System.Security.Principal;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Menu;
using NakedObjects.Architecture.Menu;
using NakedObjects.Audit;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;

namespace NakedObjects.Test.App {
    public class NakedObjectsRunSettings {

        private static Type[] Types {
            get {
                return new Type[] {
                    typeof (EnumerableQuery<object>),
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (ActionResultModelQ<object>),
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof( OrderStatus),
                    typeof (QuickOrderForm),
                    typeof (ActionResultModelQ<>),
                    typeof (ActionResultModel<>)
                };
            }
        }

        private static Type[] Services {
            get {
                return new Type[] {

                typeof(CustomerRepository),
                typeof(OrderRepository),
                typeof(ProductRepository),
                typeof(EmployeeRepository),
                typeof(SalesRepository),
                typeof(SpecialOfferRepository),
                typeof(ContactRepository),
                typeof(VendorRepository),
                typeof(PurchaseOrderRepository),
                typeof(WorkOrderRepository),
                typeof( OrderContributedActions),
                typeof( CustomerContributedActions)
                };
            }
        }

        private static Type[] AssociatedTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => (t.BaseType == typeof(AWDomainObject)) && !t.IsAbstract).ToArray();
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, AssociatedTypes().Select(t => t.Namespace).Distinct().ToArray(), MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingEdmxContext("Model").AssociateTypes(AssociatedTypes);
            config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });
            return config;
        }

        public class DefaultAuditor : IAuditor {
            public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {
                // do nothing
            }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {
                // do nothing
            }

            public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {
                // do nothing
            }

            public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {
                // do nothing
            }
        }

        public static IAuditConfiguration AuditConfig() {
            return new AuditConfiguration<DefaultAuditor>();
        }

        public static IAuthorizationConfiguration AuthorizationConfig() {
            return new AuthorizationConfiguration<DemoAuthorizer>();
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
                    factory.NewMenu<WorkOrderRepository>(true)
            };
        }
    }
}