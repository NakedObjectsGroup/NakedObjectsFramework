// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace NakedObjects.Core.Configuration {
    public class ReflectorConfiguration : IReflectorConfiguration {
        public ReflectorConfiguration(
            Type[] typesToIntrospect,
            Type[] menuServices,
            Type[] contributedActions,
            Type[] systemServices,
            Func<IMenuFactory, IMenu[]> mainMenus = null) {
            TypesToIntrospect = typesToIntrospect;
            MenuServices = menuServices;
            ContributedActions = contributedActions;
            SystemServices = systemServices;
            IgnoreCase = false;
        }

        #region IReflectorConfiguration Members

        public Type[] TypesToIntrospect { get; private set; }
        public bool IgnoreCase { get; private set; }
        public Type[] MenuServices { get; private set; }
        public Type[] ContributedActions { get; private set; }
        public Type[] SystemServices { get; private set; }

        #endregion
    }
}