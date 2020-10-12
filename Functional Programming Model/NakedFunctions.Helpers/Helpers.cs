using System;
using System.Linq;

namespace NakedFunctions
{
    public static class Helpers
    {
        public static (T, T) DisplayAndPersist<T>(T obj)
        {
            return (obj, obj);
        }

        public static Action<IAlert> WarnUser(string message)
        {
            return (IAlert ua) => ua.WarnUser(message);
        }

        public static Action<IAlert> InformUser(string message)
        {
            return (IAlert ua) => ua.InformUser(message);
        }

        public static (T, Action<IAlert>) SingleObjectWarnIfNoMatch<T>(IQueryable<T> query)
        {
            T result = default(T);
            string message = "";
            if (!query.Any())
            {
                message = "There is no matching object";

            }
            else
            {
                result = query.First();
            }
            return (result, WarnUser(message));
        }

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T
        /// </summary>
        public static T Random<T>(IQueryable<T> query, int random) where T : class
        {
            //The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return query.OrderBy(n => "").Skip(random % query.Count()).FirstOrDefault();
        }
    }
}
