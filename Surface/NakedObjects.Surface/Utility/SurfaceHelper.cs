// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface.Utility.Restricted {
    public static class SurfaceHelper {
        public static void ForEach<T>(this T[] toIterate, Action<T> action) {
            Array.ForEach<T>(toIterate, action);
        }

        public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T> action) {
            foreach (T obj in toIterate) {
                action(obj);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T, int> action) {
            int num = 0;
            foreach (T obj in toIterate) {
                action(obj, num++);
            }
        }
    }
}