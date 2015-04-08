// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Web.Mvc.Models;
using System.Linq;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects;

namespace $rootnamespace$ {

    // Use this class to configure the application running under Naked Objects
    public class NakedObjectsRunSettings {

	     public static string RestRoot {
            get { return null; }
        }

		private static string[] ModelNamespaces { 
            get {
                return new string[] {}; //Add top-level namespace(s) that cover the domain model
            }			
		}
        
        private static Type[] Services {
            get {
                return new Type[] {
                };
            }
        }

		private static Type[] Types {
            get {
                return new Type[] {
                    //These types must be registered because they are defined in
                    //NakedObjects.Mvc, not in Core.
                    typeof (ActionResultModelQ<>),
                    typeof (ActionResultModel<>)
                    //Add any domain types that cannot be reached by traversing model from the registered services
                };
            }
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
			//config.UsingCodeFirstContext(() => new MyDbContext());
			return config;
        }

		public static IAuditConfiguration AuditConfig() {
            return null; //No auditing set up
			//Example:
            //var config = new AuditConfiguration<MyDefaultAuditor>();
            //config.AddNamespaceAuditor<FooAuditor>("MySpace.Foo");
            //return config;
        }

		public static IAuthorizationConfiguration AuthorizationConfig() {
            return null; //No authorization set up
			//Example:
			//var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
    		//config.AddTypeAuthorizer<Foo, FooAuthorizer>();
			//config.AddNamespaceAuthorizer<BarAuthorizer>("MySpace.Bar");
			//return config;
        }

		public static IProfileConfiguration ProfileConfig() {
            return null;
			//Example:
			//var events = new HashSet<ProfileEvent>() { ProfileEvent.ActionInvocation }; //etc
            //return new ProfileConfiguration<MyProfiler>() { EventsToProfile = events };
        }

		/// <summary>
        /// Return an array of IMenus (obtained via the factory, then configured) to
        /// specify the Main Menus for the application. If none are returned then
        /// the Main Menus will be derived automatically from the Services.
        /// </summary>
		public static IMenu[] MainMenus(IMenuFactory factory) {
            return new IMenu[] {};
        }
    }
}