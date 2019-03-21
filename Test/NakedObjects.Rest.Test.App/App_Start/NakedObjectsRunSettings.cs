// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Value;
using RestfulObjects.Test.Data;

namespace NakedObjects.Rest.Test.App {
    public class NakedObjectsRunSettings {

	    //Returning e.g. "restapi" creates the Restful Objects API on that root.
		//Returning "" creates the Restful Objects API at the top level
	    //Returning null means the Restful Objects API will not be generated
		public static string RestRoot {
            get { return ""; }
        }

		private static string[] ModelNamespaces { 
            get {
                return new string[] {}; //Add top-level namespace(s) that cover the domain model
            }			
		}

        private static Type[] Types
        {
            get
            {
                return new Type[] {
                    typeof (WithValueViewModel),
                    typeof (WithReferenceViewModel),
                    typeof (WithCollectionViewModel),
                    typeof (WithActionViewModel),
                    typeof (FormViewModel),
                    typeof (VerySimple),
                    typeof (TestEnum),
                    typeof (SetWrapper<MostSimple>),
                    typeof (List<MostSimple>),
                    typeof (HashSet<MostSimple>),
                    typeof (WithScalars),
                    typeof (WithAttachments),
                    typeof (VerySimpleEager),
                    typeof (Immutable),
                    typeof (MostSimple[]),
                    typeof (Image),
                    typeof (FileAttachment),
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>)
            };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new Type[] {
                    typeof (RestDataRepository),
                    typeof (WithActionService),
                    typeof (ContributorService),
                      typeof (TestTypeCodeMapper)
                };
            }
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, Types.Select(t => t.Namespace).Distinct().ToArray(), null, false);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {

            CodeFirstContext.Seed("RestTest");
            var config = new EntityObjectStoreConfiguration();
            config.UsingCodeFirstContext(() => new CodeFirstContext("RestTest"));
            return config;
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            //e.g. var menu1 = factory.NewMenu<MyService1>(true); //then add to returned array
            return new IMenu[] {  };
        }
    }
}