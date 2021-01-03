using System;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel
{
    public static class Helpers
    {
        //TODO: Elimiate this version
        public static (T, T) DisplayAndPersist<T>(T obj) => (obj, obj);

        public static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));

        public static Action<IAlert> WarnUser(string message) =>(IAlert ua) => ua.WarnUser(message);

        public static Action<IAlert> InformUser(string message) => (IAlert ua) => ua.InformUser(message);


        //TODO: Elimiate this version
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

        public static (T, IContext) SingleObjectWarnIfNoMatch<T>(this IQueryable<T> query, IContext context) =>
            (query.FirstOrDefault(), query.Any() ? context : context.WithAction(WarnUser("There is no matching object")));

        //TODO: Eliminate this version
        public static T Random<T>(IQueryable<T> query, int random) where T : class
        {
            //The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            return query.OrderBy(n => "").Skip(random % query.Count()).FirstOrDefault();
        }

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T
        /// </summary>
        public static T Random<T>(IContext context) where T : class =>  Random<T>(context, context.RandomSeed());

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T, where random number is specified (to allow repeated calling)
        /// </summary>
        public static T Random<T>(IContext context, IRandom random) where T : class
        {
            //The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            var instances = context.Instances<T>().OrderBy(n => "");
            return instances.Skip(random.ValueInRange(instances.Count())).FirstOrDefault();
        }

    }
}