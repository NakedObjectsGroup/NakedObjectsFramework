// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksFunctionalModel;
using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;

namespace NakedObjects.Rest.App.Demo {
    public static class NakedObjectsRunSettings {
        private static string[] ModelNamespaces => new[] {"AdventureWorksModel"};

        private static Type[] Types => new Type[] { };

        private static Type[] Services => AWModelConfiguration.Services().ToArray();

        public static List<(Type rootType, string name, bool allActions, Action<IMenu> customConstruction)> MainMenus() =>
            AWModelConfiguration.MainMenus().Select(t => (t.rootType, t.name, true, (Action<IMenu>) null)).ToList();

        public static IObjectReflectorConfiguration ObjectReflectorConfig() {
            ObjectReflectorConfiguration.NoValidate = true;
            return new ObjectReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus());
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration) {
            var config = new EntityObjectStoreConfiguration();
            var cs = configuration.GetConnectionString("AdventureWorksContext");
            config.UsingContext(() => new AdventureWorksContext(cs));
            return config;
        }
    }
}