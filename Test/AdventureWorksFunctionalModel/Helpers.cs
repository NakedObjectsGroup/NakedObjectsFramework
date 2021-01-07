using System;
using System.Linq;
using NakedFunctions;

namespace AW
{
    public static class Helpers
    {
        public static (T, IContext) DisplayAndSave<T>(T obj, IContext context) => (obj, context.WithPendingSave(obj));

        public static Action<IAlert> WarnUser(string message) =>(IAlert ua) => ua.WarnUser(message);

        public static Action<IAlert> InformUser(string message) => (IAlert ua) => ua.InformUser(message);

        public static (T, IContext) SingleObjectWarnIfNoMatch<T>(this IQueryable<T> query, IContext context) =>
            (query.FirstOrDefault(), query.Any() ? context : context.WithAction(WarnUser("There is no matching object")));

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T
        /// </summary>
        public static T Random<T>(IContext context) where T : class 
        {
            //The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            var instances = context.Instances<T>().OrderBy(n => "");
            return instances.Skip(context.RandomSeed().ValueInRange(instances.Count())).FirstOrDefault();
        }

    }
}