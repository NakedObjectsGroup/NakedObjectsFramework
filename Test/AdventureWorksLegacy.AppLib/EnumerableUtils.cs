using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksLegacy.AppLib
{
    public static class EnumerableUtils
    {
        private static Type ElementType(this IEnumerable e)
        {
            var t = e.GetType();

            if (!t.IsGenericType)
            {
                throw new Exception("Must be generic enumerable in order to use these helpers");
            }

            var args = t.GenericTypeArguments;

            if (Enumerable.Count(args) != 1)
            {
                throw new Exception("Must be only one generic arg in order to use these helpers");
            }

            return Enumerable.First(args);
        }

        public static int Count(this IEnumerable e)
        {
            var countMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Count" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = countMethod.MakeGenericMethod(e.ElementType());
            return (int)gm.Invoke(null, new object[] { e });
        }

        public static object First(this IEnumerable e)
        {
            var firstMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "First" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = firstMethod.MakeGenericMethod(e.ElementType());
            return gm.Invoke(null, new object[] { e });
        }

        public static IEnumerable Take(this IEnumerable e, int count)
        {
            static bool MatchParms(MethodInfo m)
            {
                var parms = m.GetParameters();
                return parms.Length == 2 && parms[1].ParameterType == typeof(int);
            }

            var takeMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Take" && MatchParms(m));
            var gm = takeMethod.MakeGenericMethod(e.ElementType());
            return (IEnumerable)gm.Invoke(null, new object[] { e, count });
        }

        public static IEnumerable Skip(this IEnumerable e, int count)
        {
            var takeMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Skip" && Enumerable.Count(m.GetParameters()) == 2);
            var gm = takeMethod.MakeGenericMethod(e.ElementType());
            return (IEnumerable)gm.Invoke(null, new object[] { e, count });
        }

        public static bool Contains(this IEnumerable e, object item)
        {
            var containsMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Contains" && Enumerable.Count(m.GetParameters()) == 2);
            var gm = containsMethod.MakeGenericMethod(e.ElementType());
            return (bool)gm.Invoke(null, new[] { e, item });
        }

        public static object[] ToArray(this IEnumerable e)
        {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToArray" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = toArrayMethod.MakeGenericMethod(e.ElementType());
            return (object[])gm.Invoke(null, new object[] { e });
        }

        public static IList ToList(this IEnumerable e)
        {
            var toArrayMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "ToList" && Enumerable.Count(m.GetParameters()) == 1);
            var gm = toArrayMethod.MakeGenericMethod(e.ElementType());
            return (IList)gm.Invoke(null, new object[] { e });
        }

        public static ArrayList ToArrayList(this IEnumerable e)
        {
            return new ArrayList(e.ToList());
        }
    }
}
