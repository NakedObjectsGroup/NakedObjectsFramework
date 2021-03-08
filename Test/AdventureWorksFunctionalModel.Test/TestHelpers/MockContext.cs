using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NakedFunctions.Test
{
    public record MockContext : AbstractMockContext
    {
        #region Additional public methods
        public MockContext WithInstances(params object[] instances) =>
            this with { AllInstances = AllInstances.AddRange(instances) };

        public ImmutableArray<object> AllInstances { get; init; } = new object[] { }.ToImmutableArray();

        private ImmutableDictionary<object, object> Services { get; init; }
            = new Dictionary<object, object>().ToImmutableDictionary();

        public ImmutableDictionary<object, object> NewOrUpdated { get; init; }
            = new Dictionary<object, object>().ToImmutableDictionary();

        public MockContext WithService<T>(T service) where T : class =>
            this with { Services = Services.Add(typeof(T), service) };

        public MockContext WithService<I, T>(T service) where T : class, I =>
            this with { Services = Services.Add(typeof(I), service) };

        public Func<IContext, IContext> Deferred { get; init; }

        public MockContext ExecuteDeferred() => (MockContext)Deferred?.Invoke(this);

        private ImmutableDictionary<Type, Delegate> OnSavingNew { get; init; } = new Dictionary<Type, Delegate>().ToImmutableDictionary();

        //Called within a test to simulate the changes made to updated and/or associated objects by the persistor
        public MockContext WithOnSavingNew<T>(Func<T,T> f) => 
            this with { OnSavingNew = OnSavingNew.Add(typeof(T), f) };

        public MockContext WithReplacement<T>(T original, T replacement) =>          
            this with
            {
                NewOrUpdated = NewOrUpdated.ContainsKey(original) ? 
                    NewOrUpdated.Remove(original).Add(original, replacement)
                    : NewOrUpdated,
                AllInstances = AllInstances.Remove(original).Add(replacement)
            };

        #endregion

        #region Implementation of IContext
        public override T GetService<T>() => (T) Services[typeof(T)];

        public override IQueryable<T> Instances<T>() where T : class => AllInstances.OfType<T>().AsQueryable();

        public override MockContext WithDeferred(Func<IContext, IContext> function) => 
            this with { Deferred = function };

        public override MockContext WithDeleted<T>(T deleteObj) where T : class => 
            this with { AllInstances = AllInstances.Remove(deleteObj) };

        public override MockContext WithNew<T>(T newObj) where T : class
        {
            var repl = OnSavingNew.TryGetValue(typeof(T), out var f) ?
               ((Func<T, T>)f).Invoke(newObj) : newObj;
            return this with
            {            
                NewOrUpdated = NewOrUpdated.Add(newObj, repl),
                AllInstances = AllInstances.Add(repl),
            };
        }

        public override MockContext WithUpdated<T>(T original, T updated) where T : class => 
            this with
            {
                AllInstances = AllInstances.Remove(original).Add(updated),
                NewOrUpdated = NewOrUpdated.Add(original, updated)
            };

        public override T Reload<T>(T original) where T : class => (T) NewOrUpdated[original];

        //Implementation just returns the object passed in, which is assumed to have all rquired references set up already by  the test
        public override T Resolve<T>(T obj) where T : class => obj;
        #endregion


    }
}
