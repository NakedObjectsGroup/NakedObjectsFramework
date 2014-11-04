// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta {
    public class ImmutableInMemorySpecCache : ISpecificationCache {
        private ImmutableList<IMenu> mainMenus = ImmutableList<IMenu>.Empty;

        private ImmutableDictionary<string, IObjectSpecImmutable> specs =
            ImmutableDictionary<string, IObjectSpecImmutable>.Empty;

        // constructor to use when reflecting
        public ImmutableInMemorySpecCache() {}

        // constructor to use when loading metadata from file
        public ImmutableInMemorySpecCache(string file) {
            using (FileStream fs = File.Open(file, FileMode.Open)) {
                IFormatter formatter = new BinaryFormatter();
                specs = ((IDictionary<string, IObjectSpecImmutable>) formatter.Deserialize(fs)).ToImmutableDictionary();
            }
        }

        #region ISpecificationCache Members

        public void Serialize(string file) {
            using (FileStream fs = File.Open(file, FileMode.OpenOrCreate)) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, specs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            }
        }

        public virtual IObjectSpecImmutable GetSpecification(string key) {
            return specs.ContainsKey(key) ? specs[key] : null;
        }

        public virtual void Cache(string key, IObjectSpecImmutable spec) {
            specs = specs.Add(key, spec);
        }

        public virtual void Clear() {
            specs = specs.Clear();
        }

        public virtual IObjectSpecImmutable[] AllSpecifications() {
            return specs.Values.ToArray();
        }

        public void Cache(IMenu mainMenu) {
            mainMenus = mainMenus.Add(mainMenu);
        }

        public IMenu[] MainMenus() {
            return mainMenus.ToArray();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}