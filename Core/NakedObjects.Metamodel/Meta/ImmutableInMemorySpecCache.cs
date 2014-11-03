// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta {
    [Serializable]
    public class ImmutableInMemorySpecCache : ISpecificationCache, IDeserializationCallback {
        private ImmutableList<IMenu> mainMenus = ImmutableList<IMenu>.Empty;
        private ImmutableDictionary<string, IObjectSpecImmutable> specs = ImmutableDictionary<string, IObjectSpecImmutable>.Empty;

        public ImmutableInMemorySpecCache() {
            
        }

        #region ISpecificationCache Members

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

        public IMenu[] AllMainMenus() {
            return mainMenus.ToArray();
        }

        #endregion

        #region ISerializable

        private readonly Dictionary<string, IObjectSpecImmutable> deserializeTempDict; 

        // The special constructor is used to deserialize values. 
        public ImmutableInMemorySpecCache(SerializationInfo info, StreamingContext context) {
            deserializeTempDict = (Dictionary<string, IObjectSpecImmutable>)info.GetValue("specs", typeof(IDictionary<string, IObjectSpecImmutable>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            var dict = specs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            info.AddValue("specs", dict);
        }

        #endregion

        // Have to do this because dictionary will not be populated until deserialization event. 
        // Have to do here rather than linline in the ctor because otherwise the values in the dictionary are null
        // presumably dictionary is populated then before values have been deserialized.
        public void OnDeserialization(object sender) {
            deserializeTempDict.OnDeserialization(sender);
            specs = deserializeTempDict.ToImmutableDictionary();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}