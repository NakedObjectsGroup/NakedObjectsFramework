using System;
using System.Linq;
using NakedFunctions;

namespace AW
{
    public static class Helpers
    {
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

        //TODO: Temporary DUMMY extension method, pending native new method on IContext.
        public static IContext WithDeleted(this IContext context, object toDelete) =>
            context.WithInformUser($"object {toDelete} scheduled for deletion.");

        //TODO: Temporary DUMMY extension method, pending native new method on IContext.
        public static T GetNewlySavedVersion<T>(this IContext context, T unsaved) =>
            unsaved; //NOT what the real method will do!


    }
}