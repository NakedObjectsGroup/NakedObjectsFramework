using System;
using NakedFunctions;


namespace AdventureWorksModel
{
    public static class LifeCycleFunctions
    {

        internal static T UpdateModified<T>(T obj, DateTime when) where T : IHasModifiedDate
        {
            return obj with {ModifiedDate =  when};
        }
    }
}
