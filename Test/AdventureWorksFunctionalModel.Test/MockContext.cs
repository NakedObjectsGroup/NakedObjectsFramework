using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AdventureWorksFunctionalModel.Test
{
    public record MockContext : IContext
    {
        #region constructors
        public MockContext(object[] instances, IAlert a, IClock c, IRandomSeedGenerator r, IGuidGenerator g, IPrincipalProvider p)
        {
            AllInstances = instances.ToImmutableArray();
            Services = new Dictionary<object, object> {
                { typeof(IAlert), a },
                { typeof(IClock), c},
                { typeof(IRandomSeedGenerator),r },
                { typeof(IGuidGenerator), g},
                { typeof(IPrincipalProvider),p }
            }.ToImmutableDictionary();
        }

        public MockContext(object[] instances) :
            this(instances, new MockAlert(), new MockClock(), new MockRandomSeedGenerator(), new MockGuidGenerator(), new MockPrincipalProvider())
        { }
        #endregion

        #region Additional public methods
        public ImmutableArray<object> AllInstances { get; init; }

        public ImmutableDictionary<object, object> Services { get; init; }

        public ImmutableDictionary<object, object> UpdatedInstances { get; init; }

        public MockContext WithService<T>(T service) where T : class =>
            this with { Services = Services.Add(typeof(T), service) };

        public MockContext WithService<I, T>(T service) where T : class, I =>
            this with { Services = Services.Add(typeof(I), service) };

        public Func<IContext, IContext> Deferred { get; init; }

        public MockContext ExecuteDeferred() => (MockContext)Deferred?.Invoke(this);

        #endregion

        #region Implementation of IContext
        public T GetService<T>() => Services.OfType<T>().FirstOrDefault();

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
