using System;
using System.Linq;
using System.Security.Principal;

namespace NakedFunctions
{
    public interface IContainer
    {
        //Obtains a queryable of a given domain type, from the persistor.
        public IQueryable<T> Instances<T>();

        //Gets a service that has been configured in services configuration.
        public T GetService<T>();

        //Returns the current DateTime. (Used to avoid creating an external system dependency in a function.)
        public DateTime Now { get; }

        //Gets the IPrincipal representing the logged on user, if any.
        public IPrincipal Principal { get; }

        //Random  double value in the range 0 <= r < 1.0
        public double Random { get;  }

        //A new Guid. (Used to avoid creating an external system dependency in a function.)
        public Guid Guid { get; }

        //Returns a copy of this container but with new random number in the Random property (deterministic pseudo-random process).
        public IContainer Next();

        //Returns a copy of this container but with one or more objects to be saved added.
        public IContainer WithSaved(params object[] obj);

        //Returns a copy of this container but with an Action<T> (where T is a service registered in services configuration) to be called by the framework after the method. has exited.
        public IContainer WithAction<T>(Action<T> action);
    }
}
