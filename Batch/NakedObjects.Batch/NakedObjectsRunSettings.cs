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
using NakedObjects.Core.Async;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;

namespace NakedObjects.Batch {
    /// <summary>
    /// Use this class to configure the application running under Naked Objects
    /// </summary>
    public static class NakedObjectsRunSettings {
        //TODO: Add similar Configuration mechanisms for Authentication, Auditing
        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 

        private static string[] ModelNamespaces
        {
            get
            {
                return new string[] { "AdventureWorksModel" };
            }
        }

        private static Type[] Types
        {
            get
            {
                return new[] {
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof (OrderStatus),
                    typeof (QuickOrderForm),
                    typeof (ProductProductPhoto),
                    typeof (ProductModelProductDescriptionCulture)
                };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                    typeof (PersonRepository),
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository),
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions),
                    typeof (SpecialOfferContributedActions),
                    typeof (ServiceWithNoVisibleActions)
                };
            }
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            return config;
        }

        public static IAuditConfiguration AuditConfig() {
            return null;
        }

        public static IAuthorizationConfiguration AuthorizationConfig() {
            return null;
        }

        /// <summary>
        /// Return an array of IMenus (obtained via the factory, then configured) to
        /// specify the Main Menus for the application. If none are returned then
        /// the Main Menus will be derived automatically from the MenuServices.
        /// </summary>
        public static IMenu[] MainMenus(IMenuFactory factory) {
            return new IMenu[] {};
        }
    }
}