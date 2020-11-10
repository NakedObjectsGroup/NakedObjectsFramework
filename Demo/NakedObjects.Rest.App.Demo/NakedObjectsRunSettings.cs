﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Extensions.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Persistor.Entity.Configuration;

namespace NakedObjects.Rest.App.Demo {
    public static class NakedObjectsRunSettings {
        public static string[] ModelNamespaces {
            get { return new string[] {"AdventureWorksModel"}; }
        }

        public static Type[] Types {
            get {
                return new[] {
                    typeof(EntityCollection<object>),
                    typeof(ObjectQuery<object>),
                    typeof(CustomerCollectionViewModel),
                    typeof(OrderLine),
                    typeof(OrderStatus),
                    typeof(QuickOrderForm),
                    typeof(ProductProductPhoto),
                    typeof(ProductModelProductDescriptionCulture)
                };
            }
        }

        public static Type[] Services {
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

        public static ObjectReflectorConfiguration ReflectorConfig() => new ObjectReflectorConfiguration(Types, Services, ModelNamespaces);

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration) {
            var config = new EntityObjectStoreConfiguration();
            var cs = configuration.GetConnectionString("AdventureWorksContext");
            config.UsingContext(() => new AdventureWorksContext(cs));
            return config;
        }

        public static Func<IConfiguration, DbContext> DbContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static IAuditConfiguration AuditConfig() => null;

        public static IAuthorizationConfiguration AuthorizationConfig() => null;

        /// <summary>
        /// Return an array of IMenus (obtained via the factory, then configured) to
        /// specify the Main Menus for the application. If none are returned then
        /// the Main Menus will be derived automatically from the Services.
        /// </summary>
        public static IMenu[] MainMenus(IMenuFactory factory) {
            var customerMenu = factory.NewMenu<CustomerRepository>(false);
            CustomerRepository.Menu(customerMenu);
            var salesMenu = factory.NewMenu<SalesRepository>(false);
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