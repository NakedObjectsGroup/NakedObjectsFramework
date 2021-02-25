// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Configuration;

namespace NakedFramework.Core.Configuration {
    public class CoreConfiguration : ICoreConfiguration {
        public CoreConfiguration(Func<IMenuFactory, IMenu[]> mainMenus = null) => MainMenus = mainMenus;

        public Func<IMenuFactory, IMenu[]> MainMenus { get; }

        /// <summary>
        ///     Standard implementation of this contains system value and collection types recognized by the Framework.
        ///     The list is exposed so that types can be added or removed before reflection. Generic collection types should be
        ///     specified
        ///     without type parameters.
        /// </summary>
        /// <remarks>
        ///     These types will always be introspected and so are implicitly 'whitelisted'
        /// </remarks>
        ///
        public List<Type> SupportedSystemTypes { get; set; } = ReflectorDefaults.DefaultSystemTypes.ToList();

        public int HashMapCapacity { get; set; }
    }
}