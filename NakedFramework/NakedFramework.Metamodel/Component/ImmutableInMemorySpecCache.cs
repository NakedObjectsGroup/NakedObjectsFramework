// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
#pragma warning disable 618
#pragma warning disable SYSLIB0011

namespace NakedObjects.Meta.Component {
    [Serializable]
    public sealed class ImmutableInMemorySpecCache : ISpecificationCache, ISerializable, IDeserializationCallback {
        private readonly SerializedData tempData;
        private ImmutableList<IMenuImmutable> mainMenus = ImmutableList<IMenuImmutable>.Empty;

        private ImmutableDictionary<string, ITypeSpecImmutable> specs = ImmutableDictionary<string, ITypeSpecImmutable>.Empty;

        // constructor to use when reflecting
        public ImmutableInMemorySpecCache() { }

        // constructor to use when loading metadata from file
        public ImmutableInMemorySpecCache(string file) {
            using var fs = File.Open(file, FileMode.Open);
            IFormatter formatter = new BinaryFormatter();
            var data = (SerializedData) formatter.Deserialize(fs);

            specs = data.SpecKeys.Zip(data.SpecValues, (k, v) => new {k, v}).ToDictionary(a => a.k, a => a.v).ToImmutableDictionary();

            mainMenus = data.MenuValues.ToImmutableList();
        }

        public ImmutableInMemorySpecCache(string file, IFormatter formatter) {
            using var fs = File.Open(file, FileMode.Open);
            var data = (SerializedData) formatter.Deserialize(fs);
            specs = data.SpecKeys.Zip(data.SpecValues, (k, v) => new {k, v}).ToDictionary(a => a.k, a => a.v).ToImmutableDictionary();

            mainMenus = data.MenuValues.ToImmutableList();
        }

        // The special constructor is used to deserialize values. 
        public ImmutableInMemorySpecCache(SerializationInfo info, StreamingContext context) => tempData = info.GetValue<SerializedData>("data");

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender) {
            specs = tempData.SpecKeys.Zip(tempData.SpecValues, (k, v) => new {k, v}).ToDictionary(a => a.k, a => a.v).ToImmutableDictionary();
            mainMenus = tempData.MenuValues.ToImmutableList();
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            var data = new SerializedData {SpecKeys = specs.Keys.ToList(), SpecValues = specs.Values.ToList(), MenuValues = mainMenus.ToList()};
            info.AddValue<SerializedData>("data", data);
        }

        #endregion

        #region ISpecificationCache Members

        public void Serialize(string file) {
            using var fs = File.Open(file, FileMode.OpenOrCreate);
            IFormatter formatter = new BinaryFormatter();
            var data = new SerializedData {SpecKeys = specs.Keys.ToList(), SpecValues = specs.Values.ToList(), MenuValues = mainMenus.ToList()};
            formatter.Serialize(fs, data);
        }

        public void Serialize(string file, IFormatter formatter) {
            using var fs = File.Open(file, FileMode.OpenOrCreate);
            var data = new SerializedData {SpecKeys = specs.Keys.ToList(), SpecValues = specs.Values.ToList(), MenuValues = mainMenus.ToList()};
            formatter.Serialize(fs, data);
        }

        public ITypeSpecImmutable GetSpecification(string key) => specs.ContainsKey(key) ? specs[key] : null;

        public void Cache(string key, ITypeSpecImmutable spec) => specs = specs.Add(key, spec);

        public void Clear() => specs = specs.Clear();

        public ITypeSpecImmutable[] AllSpecifications() => specs.Values.ToArray();

        public void Cache(IMenuImmutable mainMenu) => mainMenus = mainMenus.Add(mainMenu);

        public IMenuImmutable[] MainMenus() => mainMenus.ToArray();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}