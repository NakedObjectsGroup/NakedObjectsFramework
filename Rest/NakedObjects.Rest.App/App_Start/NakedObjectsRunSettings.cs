// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using RestfulObjects.Test.Data;

namespace MvcTestApp {
    public static class NakedObjectsRunSettings {

        public static string RestRoot {
            get { return ""; }
        }

        private static string[] ModelNamespaces {
            get {
                return new[] { "MvcTestApp" };
            }
        }

        private static Type[] Services {
            get {
                return new[] {
                    typeof (RestDataRepository),
                    typeof (WithActionService),
                    typeof (ContributorService),
                    typeof (TestTypeCodeMapper),
                    typeof(WithGuidKey)
                };
            }
        }

        private static Type[] Types {
            get {
                return new[] { typeof(Immutable),
                   typeof(WithActionViewModel),
                   typeof(WithCollectionViewModel),
                   typeof(WithValueViewModel),
                   typeof(WithNestedViewModel),
                   typeof(WithValueViewModelEdit),
                   typeof(WithNestedViewModelEdit),
                   typeof(RedirectedObject),
                   typeof(WithScalars),
                   typeof(VerySimple),
                   typeof(VerySimpleEager),
                   typeof(WithAction),
                   typeof(WithActionObject),
                   typeof(WithAttachments),
                   typeof(WithCollection),
                   typeof(WithDateTimeKey),           
                   typeof(WithError),
                   typeof(WithGetError),
                   typeof(WithNestedViewModel),
                   typeof(WithReference),
                   typeof(WithReferenceViewModel),
                   typeof(WithReferenceViewModelEdit),
                   typeof(MostSimple),
                   typeof(MostSimpleViewModel),
                   typeof(WithValue),
                   typeof(TestEnum),
                   typeof(MostSimple[]),
                   typeof(FormViewModel),
                   typeof(SwitchableViewModel),
                   typeof(SetWrapper<MostSimple>) };
            }
        }

        //private static Type[] AssociatedTypes() {
        //    var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
        //    return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        //}

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus, true);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {


            CodeFirstContext.Seed();

            var config = new EntityObjectStoreConfiguration();
            config.UsingCodeFirstContext(() => new CodeFirstContext("RestTest"));
            return config;
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            var menu1 = factory.NewMenu<RestDataRepository>(true);
            var menu2 = factory.NewMenu<WithActionService>(true);
            var menu3 = factory.NewMenu<ContributorService>(true);
            var menu4 = factory.NewMenu<TestTypeCodeMapper>(true);


            return new[] { menu1, menu2, menu3, menu4 };
        }
    }
}