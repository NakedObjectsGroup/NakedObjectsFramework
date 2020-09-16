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
using AdventureWorksFunctionalModel;
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
        private static string[] ModelNamespaces {
            get { return new string[] {"AdventureWorksModel"}; }
        }

        private static Type[] Types =>   new Type [] {       };

        private static Type[] Services => AWModelConfiguration.Services().ToArray();
        

        private static Type[] FunctionalTypes => AWModelConfiguration.DomainTypes().ToArray();
      

        private static Type[] Functions => AWModelConfiguration.ObjectFunctions().ToArray();
        

        public static ReflectorConfiguration ReflectorConfig() {
            ReflectorConfiguration.NoValidate = true;
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static FunctionalReflectorConfiguration FunctionalReflectorConfig() => new FunctionalReflectorConfiguration(FunctionalTypes, Functions);

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration) {
            var config = new EntityObjectStoreConfiguration();
            var cs = configuration.GetConnectionString("AdventureWorksContext");
            config.UsingContext(() => new AdventureWorksContext(cs));
            return config;
        }

        public static IAuditConfiguration AuditConfig() => null;

        public static IAuthorizationConfiguration AuthorizationConfig() => null;

        public static IMenu[] MainMenus(IMenuFactory factory) => AWModelConfiguration.MainMenus().Select(kvp => factory.NewMenu(kvp.Value, true, kvp.Key)).ToArray();
    }
}