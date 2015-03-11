using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NakedObjects.Architecture.Util {
    public static class QueryableUtils {
        public static int Count(this IQueryable q) {
            MethodInfo countMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Count" && m.GetParameters().Count() == 1);
            MethodInfo gm = countMethod.MakeGenericMethod(q.ElementType);
            return (int) gm.Invoke(null, new object[] {q});
        }

        public static object First(this IQueryable q) {
            MethodInfo firstMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "First" && m.GetParameters().Count() == 1);
            MethodInfo gm = firstMethod.MakeGenericMethod(q.ElementType);
            return gm.Invoke(null, new object[] {q});
        }

        public static IQueryable Take(this IQueryable q, int count) {
            MethodInfo takeMethod = typeof (Queryable).GetMethods().Single(m => m.Name == "Take" && m.GetParameters().Count() == 2);
            MethodInfo gm = takeMethod.MakeGenericMethod(q.ElementType);
            return (IQueryable) gm.Invoke(null, new object[] {q, count});
        }

        public static object[] ToArray(this IQueryable q) {
            MethodInfo toArrayMethod = typeof (Enumerable).GetMethods().Single(m => m.Name == "ToArray" && m.GetParameters().Count() == 1);
            MethodInfo gm = toArrayMethod.MakeGenericMethod(q.ElementType);
            return (object[]) gm.Invoke(null, new object[] {q});
        }

        private static bool IsOrderExpression(Expression expr) {
            var expression = expr as MethodCallExpression;
            if (expression != null) {
                MethodInfo method = expression.Method;
                return !method.Name.StartsWith("Distinct") && (method.Name.StartsWith("OrderBy") || method.Name.StartsWith("ThenBy") || expression.Arguments.Any(IsOrderExpression));
            }
            return false;
        }

        public static bool IsOrdered(this IQueryable queryable) {
            return IsOrderExpression(queryable.Expression);
        }
    }
}