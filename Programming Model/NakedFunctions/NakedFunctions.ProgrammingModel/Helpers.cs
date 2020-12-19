using System;
using System.Linq;
using System.Security.Principal;

namespace NakedFunctions
{
    public static class Helpers
    {
        //TODO: Elimiate this version
        public static (T, T) DisplayAndPersist<T>(T obj) => (obj, obj);

        public static (T, IContainer) DisplayAndSave<T>(T obj, IContainer container) => (obj, container.WithPendingSave(obj));

        public static Action<IAlert> WarnUser(string message) =>(IAlert ua) => ua.WarnUser(message);

        public static IContainer WithWarnUser(this IContainer container, string message) => container.WithOutput((IAlert ua) => ua.WarnUser(message));

        public static Action<IAlert> InformUser(string message) => (IAlert ua) => ua.InformUser(message);

        public static IContainer WithInformUser(this IContainer container, string message) => container.WithOutput((IAlert ua) => ua.InformUser(message));

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

        public static (T, IContainer) SingleObjectWarnIfNoMatch<T>(this IQueryable<T> query, IContainer container) =>
            (query.FirstOrDefault(), query.Any() ? container : container.WithOutput(WarnUser("There is no matching object")));

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
        public static T Random<T>(IContainer container) where T : class =>  Random<T>(container, container.RandomSeed());

        /// <summary>
        ///     Returns a random instance from the set of all instance of type T, where random number is specified (to allow repeated calling)
        /// </summary>
        public static T Random<T>(IContainer container, IRandom random) where T : class
        {
            //The OrderBy(...) doesn't change the ordering, but is a necessary precursor to using .Skip
            //which in turn is needed because LINQ to Entities doesn't support .ElementAt(x)
            var instances = container.Instances<T>().OrderBy(n => "");
            return instances.Skip(random.ValueInRange(instances.Count())).FirstOrDefault();
        }

        public static DateTime Now(this IContainer container) => container.GetService<IClock>().Now();

        public static DateTime Today(this IContainer container) => container.GetService<IClock>().Today();

        public static IPrincipal CurrentUser(this IContainer container) => container.GetService<IPrincipalProvider>().CurrentUser;

        public static IRandom RandomSeed(this IContainer container) => container.GetService<IRandomSeedGenerator>().Random;

        public static Guid NewGuid(this IContainer container) => container.GetService<IGuidGenerator>().NewGuid();
    }
}