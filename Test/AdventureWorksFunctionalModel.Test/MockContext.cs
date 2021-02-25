using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventureWorksFunctionalModel.Test
{
    public record MockContext : IContext
    {
        #region Additional public methods
        public MockContext WithInstances(params object[] instances) =>
            this with { AllInstances = AllInstances.AddRange(instances) };

        public ImmutableArray<object> AllInstances { get; init; } = new object[] { }.ToImmutableArray();

        public ImmutableDictionary<object, object> Services { get; init; } 
            = new Dictionary<object, object>().ToImmutableDictionary();

        public ImmutableDictionary<object, object> UpdatedInstances { get; init; } 
            = new Dictionary<object, object>().ToImmutableDictionary();

        public MockContext WithService<T>(T service) where T : class =>
            this with { Services = Services.Add(typeof(T), service) };

        public MockContext WithService<I, T>(T service) where T : class, I =>
            this with { Services = Services.Add(typeof(I), service) };

        public Func<IContext, IContext> Deferred { get; init; }

        public MockContext ExecuteDeferred() => (MockContext)Deferred?.Invoke(this);
        #endregion

        #region Implementation of IContext
        public T GetService<T>() => (T) Services[typeof(T)];

        public IQueryable<T> Instances<T>() where T : class => AllInstances.OfType<T>().AsQueryable();

        public IContext WithDeferred(Func<IContext, IContext> function) => this with { Deferred = function };

        public IContext WithDeleted(object deleteObj) => this with { AllInstances = AllInstances.Remove(deleteObj) };

        public IContext WithNew(object newObj) => this with {AllInstances = AllInstances.Add(newObj)};

        public IContext WithUpdated<T>(T original, T updated) => this with
        {
            AllInstances = AllInstances.Remove(original).Add(updated),
            UpdatedInstances = UpdatedInstances.Add(original, updated)
        };

        public T Reload<T>(T original) where T : class => (T) UpdatedInstances[original];

        //Implementation just returns the object passed in, which is assumed to have all rquired references set up already by  the test
        public T Resolve<T>(T obj) where T : class => obj;
        #endregion


    }
}
