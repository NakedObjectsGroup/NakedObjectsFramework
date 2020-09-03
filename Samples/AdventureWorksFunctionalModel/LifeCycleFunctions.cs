using System;
using NakedFunctions;


namespace AdventureWorksModel
{
    public static class LifeCycleFunctions
    {
        //TODO: this should be able to work in future, when 'with' works with classes
        internal static T UpdateModified<T>(T obj, DateTime when) where T :  IHasModifiedDate
        {
            throw new NotImplementedException();
            //return obj with {ModifiedDate =  when};
        }
    }
}
