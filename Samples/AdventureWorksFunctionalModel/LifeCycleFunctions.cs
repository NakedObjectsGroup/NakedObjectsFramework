using System;
using NakedFunctions;


namespace AdventureWorksModel
{
    public static class LifeCycleFunctions
    {

        internal static T UpdateModified<T>(T obj, DateTime when) where T : IHasModifiedDate
        {
            return obj.With(x => x.ModifiedDate, when);
        }
    }
}
