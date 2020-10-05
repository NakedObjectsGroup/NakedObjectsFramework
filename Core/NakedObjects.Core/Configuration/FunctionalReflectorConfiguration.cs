// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace NakedObjects.Core.Configuration {
    public class FunctionalReflectorConfiguration : IFunctionalReflectorConfiguration {
        public FunctionalReflectorConfiguration(Type[] types,
                                                Type[] functions,
                                                string[] modelNamespaces = null,
                                                List<(Type rootType, string name, bool allActions, Action<IMenu> action)> mainMenus = null,
                                                bool concurrencyChecking = true) {
            Types = types;
            Functions = functions;
            MainMenus = mainMenus;
            ConcurrencyChecking = concurrencyChecking;
            IgnoreCase = false;
        }

        #region IFunctionalReflectorConfiguration Members

        public Type[] Types { get; }
        public Type[] Functions { get; }

        public Type[] Services => HasConfig() ? new[] {typeof(MenuFunctions)} : new Type[] { };
        public bool ConcurrencyChecking { get;  }
        public bool IgnoreCase { get; }
        public List<(Type rootType, string name, bool allActions, Action<IMenu> action)> MainMenus { get; }

        #endregion

        public bool HasConfig() => Types?.Any() == true || Functions?.Any() == true;
    }
}