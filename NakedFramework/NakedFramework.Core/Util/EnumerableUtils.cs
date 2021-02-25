// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Linq;
using NakedFramework.Core.Exception;

namespace NakedFramework.Core.Util {
    public static class EnumerableUtils {
        private static Type ElementType(this IEnumerable e) {
            var t = e.GetType();

            if (!t.IsGenericType) {
                throw new NakedObjectSystemException("Must be generic enumerable in order to use these helpers");
            }

            var args = t.GenericTypeArguments;

            if (Enumerable.Count(args) != 1) {
                throw new NakedObjectSystemException("Must be only one generic arg in order to use these helpers");
            }

            return Enumerable.First(args);
        }

        public static int Count(this IEnumerable e) {
            var countMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Count" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = countMethod.MakeGenericMethod(e.ElementType());
            return (int) gm.Invoke(null, new object[] {e});
        }

        public static object First(this IEnumerable e) {
            var firstMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "First" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = firstMethod.MakeGenericMethod(e.ElementType());
            return gm.Invoke(null, new object[] {e});
        }

        public static IEnumerable Take(this IEnumerable e, int count) {
            var takeMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Take" && Enumerable.Count(m.GetParameters()) == 2);
            var gm = takeMethod.MakeGenericMethod(e.ElementType());
            return (IEnumerable) gm.Invoke(null, new object[] {e, count});
        }

        public static IEnumerable Skip(this IEnumerable e, int count) {
            var takeMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Skip" && Enumerable.Count(m.GetParameters()) == 2);
            var gm = takeMethod.MakeGenericMethod(e.ElementType());
            return (IEnumerable) gm.Invoke(null, new object[] {e, count});
        }

        public static bool Contains(this IEnumerable e, object item) {
            var containsMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Contains" && Enumerable.Count(m.GetParameters()) == 2);
            var gm = containsMethod.MakeGenericMethod(e.ElementType());
            return (bool) gm.Invoke(null, new[] {e, item});
        }

        public static object[] ToArray(this IEnumerable e) {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToArray" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = toArrayMethod.MakeGenericMethod(e.ElementType());
            return (object[]) gm.Invoke(null, new object[] {e});
        }

        public static IList ToList(this IEnumerable e) {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToList" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = toArrayMethod.MakeGenericMethod(e.ElementType());
            return (IList) gm.Invoke(null, new object[] {e});
        }
    }
}