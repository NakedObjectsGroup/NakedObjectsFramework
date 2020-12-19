using System;
using System.Collections.Immutable;
using System.Linq;

namespace NakedFunctions
{
    public interface IContainer
    {
        //Obtains a queryable of a given domain type, from the persistor.
        public IQueryable<T> Instances<T>();

        //Gets a service that has been configured in services configuration.
        public T GetService<T>();

       //Returns a copy of this container but with one or more objects to be saved added to the PendingSave array.
        public IContainer WithPendingSave(params object[] toBeSaved);

        //TODO: Do we need to consider possibility that a chained action modifies the same object more than once? What happend in NakedObjects now?
        public ImmutableArray<object> PendingSave { get; }

        //Returns a copy of this container but with an Action<T> (where T is a service registered in services configuration) to be called by the framework after the method. has exited.
        public IContainer WithOutput<T>(Action<T> action);
    }
}
