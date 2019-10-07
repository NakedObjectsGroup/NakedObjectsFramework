// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace NakedObjects.Meta.Utils {
    public static class SerializationUtils {
        public static void AddValue<T>(this SerializationInfo info, string id, T value) {
            info.AddValue(id, value, typeof(T));
        }

        public static T GetValue<T>(this SerializationInfo info, string id) {
            return (T) info.GetValue(id, typeof(T));
        }

        public static void AddValue<TKey, TValue>(this SerializationInfo info, string id, IImmutableDictionary<TKey, TValue> immutableDict) {
            Dictionary<TKey, TValue> dict = immutableDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            info.AddValue(id, dict, typeof(Dictionary<TKey, TValue>));
        }

        public static Dictionary<TKey, TValue> GetValue<TKey, TValue>(this SerializationInfo info, string id) {
            return (Dictionary<TKey, TValue>) info.GetValue(id, typeof(Dictionary<TKey, TValue>));
        }
    }
}