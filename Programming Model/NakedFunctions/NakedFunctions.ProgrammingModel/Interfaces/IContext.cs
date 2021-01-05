using System;
using System.Linq;

namespace NakedFunctions
{
    public interface IContext
    {
        //Obtains a queryable of a given domain type, from the persistor.
        public IQueryable<T> Instances<T>() where T : class;

        //Gets a service that has been configured in services configuration.
        public T GetService<T>();

       //Returns a copy of this context but with one or more objects to be saved added.
        public IContext WithPendingSave(params object[] toBeSaved);

        //Returns a copy of this context but with an Action<T> (where T is a service registered in services configuration)
        //to be called by the framework after the function has completed.
        public IContext WithAction<T>(Action<T> action);
    }
}
