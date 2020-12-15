// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace NakedObjects.Core.Util.Query {
    public static class QueryableUtils {
        public static int Count(this IQueryable q) {
            var countMethod = typeof(Queryable).GetMethods().Single(m => m.Name == "Count" && m.GetParameters().Length == 1);
            var gm = countMethod.MakeGenericMethod(q.ElementType);
            return (int) gm.Invoke(null, new object[] {q});
        }

        public static object First(this IQueryable q) {
            var firstMethod = typeof(Queryable).GetMethods().Single(m => m.Name == "First" && m.GetParameters().Length == 1);
            var gm = firstMethod.MakeGenericMethod(q.ElementType);
            return gm.Invoke(null, new object[] {q});
        }

        public static IQueryable Take(this IQueryable q, int count) {
            var takeMethod = typeof(Queryable).GetMethods().Single(m => m.Name == "Take" && m.GetParameters().Length == 2);
            var gm = takeMethod.MakeGenericMethod(q.ElementType);
            return (IQueryable) gm.Invoke(null, new object[] {q, count});
        }

        public static IQueryable Skip(this IQueryable q, int count) {
            var takeMethod = typeof(Queryable).GetMethods().Single(m => m.Name == "Skip" && m.GetParameters().Length == 2);
            var gm = takeMethod.MakeGenericMethod(q.ElementType);
            return (IQueryable) gm.Invoke(null, new object[] {q, count});
        }

        public static bool Contains(this IQueryable q, object item) {
            var containsMethod = typeof(Queryable).GetMethods().Single(m => m.Name == "Contains" && m.GetParameters().Length == 2);
            var gm = containsMethod.MakeGenericMethod(q.ElementType);
            return (bool) gm.Invoke(null, new[] {q, item});
        }

        public static object[] ToArray(this IQueryable q) {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToArray" && m.GetParameters().Length == 1);
            var gm = toArrayMethod.MakeGenericMethod(q.ElementType);
            return (object[]) gm.Invoke(null, new object[] {q});
        }

        public static IList ToList(this IQueryable q) {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToList" && m.GetParameters().Length == 1);
            var gm = toArrayMethod.MakeGenericMethod(q.ElementType);
            return (IList) gm.Invoke(null, new object[] {q});
        }

        private static bool IsOrderExpression(Expression expr) {
            var expression = expr as MethodCallExpression;
            if (expression != null) {
                var method = expression.Method;
                return !method.Name.StartsWith("Distinct") && (method.Name.StartsWith("OrderBy") || method.Name.StartsWith("ThenBy") || expression.Arguments.Any(IsOrderExpression));
            }

            return false;
        }

        public static bool IsOrdered(this IQueryable queryable) => IsOrderExpression(queryable.Expression);
    }
}