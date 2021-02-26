using NakedFunctions;
using System;
using System.Linq;

namespace NakedFunctions.Test
{
    // This abstract type exists only to permit the concrete implementation (MockContext) to
    // use covariant return types (i.e. return MockContext instead of IContext),
    // as this pattern does not yet (as of C#9) work for methods inherited directly
    // from interfaces.
    public abstract record AbstractMockContext : IContext
    {
        public abstract T GetService<T>();

        public abstract IQueryable<T> Instances<T>() where T : class; 

        public abstract T Reload<T>(T unsaved) where T : class;

        public abstract T Resolve<T>(T unResolved) where T : class;

        public abstract IContext WithDeferred(Func<IContext, IContext> function);

        public abstract IContext WithDeleted(object deleteObj);

        public abstract IContext WithNew(object newObj);

        public abstract IContext WithUpdated<T>(T original, T updated);
    }
}
