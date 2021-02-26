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

        public ImmutableDictionary<object, object> Services { get; init; }
            = new Dictionary<object, object>().ToImmutableDictionary();

        public ImmutableDictionary<object, object> NewOrUpdated { get; init; }
            = new Dictionary<object, object>().ToImmutableDictionary();

        public MockContext WithService<T>(T service) where T : class =>
            this with { Services = Services.Add(typeof(T), service) };

        public MockContext WithService<I, T>(T service) where T : class, I =>
            this with { Services = Services.Add(typeof(I), service) };

        public Func<IContext, IContext> Deferred { get; init; }

        public MockContext ExecuteDeferred() => (MockContext)Deferred?.Invoke(this);

        //Called within the test to simulate the changes made to a new or updated object by the persistor
        public MockContext WithSavedNew<T>(T unsaved, T saved) => this with
        {
            NewOrUpdated = NewOrUpdated.Remove(unsaved).Add(unsaved, saved),
            AllInstances = AllInstances.Add(saved)
        };

        public MockContext WithSavedUpdated<T>(T original, T updated, T saved) => this with
        {
            //Leaves two entries for the object in NewOrUpdated, so that Reload will work on either original or updated
            NewOrUpdated = NewOrUpdated.Remove(original).Add(original, saved).Add(updated, saved),
            AllInstances = AllInstances.Remove(updated).Add(saved)
        };

        //Note: this method should only be used to replace objects that have NOT been registered as New or Updated
        //An example would be to add or update a REFERENCE to an object that HAS just been saved (i.e. is New or Updated)
        public MockContext WithReplacementForAnAssociatedObject<T>(T original, T replacement) => this with
        {
            AllInstances = AllInstances.Remove(original).Add(replacement)
        };

        #endregion

        #region Implementation of IContext
        public override T GetService<T>() => (T) Services[typeof(T)];

        public override IQueryable<T> Instances<T>() where T : class => AllInstances.OfType<T>().AsQueryable();

        public override MockContext WithDeferred(Func<IContext, IContext> function) => this with { Deferred = function };

        public override MockContext WithDeleted(object deleteObj) => this with { AllInstances = AllInstances.Remove(deleteObj) };

        public override MockContext WithNew(object newObj) => this with {
            NewOrUpdated = NewOrUpdated.Add(newObj, null)
        };

        public override MockContext WithUpdated<T>(T original, T updated) => this with
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
