// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Linq;
using System.Reflection;

namespace NakedObjects.Core.Util.Reflection {
    public static class QueryableUtils {
        public static int Count(this IQueryable q) {
            MethodInfo countMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Count" && Enumerable.Count(m.GetParameters()) == 1);
            MethodInfo gm = countMethod.MakeGenericMethod(q.ElementType);
            return (int) gm.Invoke(null, new object[] {q});
        }

        public static object First(this IQueryable q) {
            MethodInfo firstMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "First" && Enumerable.Count(m.GetParameters()) == 1);
            MethodInfo gm = firstMethod.MakeGenericMethod(q.ElementType);
            return gm.Invoke(null, new object[] {q});
        }

        public static IQueryable Take(this IQueryable q, int count) {
            MethodInfo takeMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Take" && Enumerable.Count(m.GetParameters()) == 2);
            MethodInfo gm = takeMethod.MakeGenericMethod(q.ElementType);
            return (IQueryable) gm.Invoke(null, new object[] {q, count});
        }

        public static IQueryable Skip(this IQueryable q, int count) {
            MethodInfo takeMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Skip" && Enumerable.Count(m.GetParameters()) == 2);
            MethodInfo gm = takeMethod.MakeGenericMethod(q.ElementType);
            return (IQueryable) gm.Invoke(null, new object[] {q, count});
        }

        public static bool Contains(this IQueryable q, object item) {
            MethodInfo containsMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Contains" && Enumerable.Count(m.GetParameters()) == 2);
            MethodInfo gm = containsMethod.MakeGenericMethod(q.ElementType);
            return (bool) gm.Invoke(null, new object[] {q, item});
        }

        public static object[] ToArray(this IQueryable q) {
            MethodInfo toArrayMethod = typeof (Enumerable).GetMethods().Single(m => m.Name == "ToArray" && Enumerable.Count(m.GetParameters()) == 1);
            MethodInfo gm = toArrayMethod.MakeGenericMethod(q.ElementType);
            return (object[]) gm.Invoke(null, new object[] {q});
        }

        public static IList ToList(this IQueryable q) {
            MethodInfo toArrayMethod = typeof (Enumerable).GetMethods().Single(m => m.Name == "ToList" && Enumerable.Count(m.GetParameters()) == 1);
            MethodInfo gm = toArrayMethod.MakeGenericMethod(q.ElementType);
            return (IList) gm.Invoke(null, new object[] {q});
        }
    }
}