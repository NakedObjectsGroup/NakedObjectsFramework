// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Value;
using RestfulObjects.Test.Data;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace MvcTestApp {
    public static class NakedObjectsSettings {
        //TODO: Add similar Configuration mechanisms for Authentication, Auditing
        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 

        private static Type[] Types {
            get { 
                return new Type[] {
                    typeof (MostSimple[]), 
                    typeof (Image), 
                    typeof (FileAttachment), 
                    typeof (EntityCollection<object>), 
                    typeof (ObjectQuery<object>)
                }; 
            }
        }

        private static Type[] MenuServices {
            get {
                return new[] {
                    typeof (RestDataRepository),
                    typeof (WithActionService)
                };
            }
        }

        private static Type[] ContributedActions {
            get {
                return new Type[] {
                    typeof (ContributorService)
                };
            }
        }

        private static Type[] SystemServices {
            get {
                return new[] {
                    typeof (TestTypeCodeMapper)
                };
            }
        }

        //private static Type[] AssociatedTypes() {
        //    var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
        //    return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        //}

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, MenuServices, ContributedActions, SystemServices, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingCodeFirstContext(() => new CodeFirstContext("RestTest"));
            return config;
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            var menu1 = factory.NewMenu<RestDataRepository>(true);
            var menu2 = factory.NewMenu<WithActionService>(true);
            var menu3 = factory.NewMenu<ContributorService>(true);
            var menu4 = factory.NewMenu<TestTypeCodeMapper>(true);


            return new IMenu[] { menu1, menu2, menu3, menu4 };
        }
    }
}