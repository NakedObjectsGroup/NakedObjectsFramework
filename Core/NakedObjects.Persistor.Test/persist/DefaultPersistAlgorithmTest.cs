// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NUnit.Framework;

// TODO Write new tests for this 


namespace NakedObjects.Persistor.Persist {
    /// <summary>
    /// Summary description for DefaultPersistAlgorithmTest
    /// </summary>
    [TestFixture, Ignore]
    public class DefaultPersistAlgorithmTest  {
        //#region Setup/Teardown

        //[SetUp]
        //public void SetUp() {
        //    system = new ProgrammableTestSystem();
        
        //    Person person = new Person();
        //    Role role = new Role();
        //    role.Person = person;

        //    personAdapter = system.AdapterFor(person);
        //    roleAdapter = system.AdapterFor(role);

        //    adder = new PersistedObjectAdderSpy();
        //    algorithm = new DefaultPersistAlgorithm();

        //    Assert.IsFalse(roleAdapter.ResolveState.IsResolved());
        //    Assert.That(adder.PersistedObjects.Count, Is.EqualTo(0));

        //    roleAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(roleAdapter.Specification));
        //    roleAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(roleAdapter.Specification));
        //    roleAdapter.Specification.AddFacet(new LoadingCallbackFacetNull(roleAdapter.Specification));
        //    roleAdapter.Specification.AddFacet(new LoadedCallbackFacetNull(roleAdapter.Specification));
        //    personAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(personAdapter.Specification));
        //    personAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(personAdapter.Specification));
        //    personAdapter.Specification.AddFacet(new LoadingCallbackFacetNull(personAdapter.Specification));
        //    personAdapter.Specification.AddFacet(new LoadedCallbackFacetNull(personAdapter.Specification));
        //}

        //#endregion

        //private PersistedObjectAdderSpy adder;
        //private DefaultPersistAlgorithm algorithm;
        //private INakedObject roleAdapter;
        //private INakedObject personAdapter;
      
        //private class PersistedObjectAdderSpy : ILifecycleManager {
        //    private readonly IList<INakedObject> persistedObjects = new List<INakedObject>();

        //    public IList<INakedObject> PersistedObjects {
        //        get { return persistedObjects; }
        //    }

        //    public int PersistedCount {
        //        get { return persistedObjects.Count; }
        //    }

        //    #region IPersistedObjectAdder Members

        //    public void AddPersistedObject(INakedObject nakedObject) {
        //        persistedObjects.Add(nakedObject);
        //    }

        //    public void MadePersistent(INakedObject nakedObject) {
        //        if (nakedObject != null) {
        //            nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
        //            nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
        //        }
        //    }

        //    #endregion

        //    public ISession Session { get; set; }
        //    public object UpdateNotifier { get; set; }
        //    public bool IsInitialized { get; set; }
        //    public INakedObject[] ServiceAdapters { get; private set; }
        //    public IOidGenerator OidGenerator { get; private set; }
        //    public IContainerInjector Injector { get; set; }

        //    public void Reset() {
        //        persistedObjects.Clear();
        //    }

        //    public void AddServices(IEnumerable<ServiceWrapper> services) {
        //        throw new NotImplementedException();
        //    }

        //    public IQueryable<T> Instances<T>() where T : class {
        //        throw new NotImplementedException();
        //    }

        //    public IQueryable Instances(Type type) {
        //        throw new NotImplementedException();
        //    }

        //    public IQueryable Instances(INakedObjectSpecification specification) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject LoadObject(IOid oid, INakedObjectSpecification spec) {
        //        throw new NotImplementedException();
        //    }

        //    public void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
        //        throw new NotImplementedException();
        //    }

        //    public void LoadField(INakedObject nakedObject, string fieldName) {
        //        throw new NotImplementedException();
        //    }

        //    public void ResolveImmediately(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public void ObjectChanged(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public void MakePersistent(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public void DestroyObject(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject GetService(string id) {
        //        throw new NotImplementedException();
        //    }

        //    public ServiceTypes GetServiceType(INakedObjectSpecification spec) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject[] GetServices() {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType) {
        //        throw new NotImplementedException();
        //    }

        //    public PropertyInfo[] GetKeys(Type type) {
        //        throw new NotImplementedException();
        //    }

        //    public void Refresh(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public int CountField(INakedObject nakedObject, string fieldName) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject FindByKeys(Type type, object[] keys) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject[] GetServices(ServiceTypes serviceType) {
        //        throw new NotImplementedException();
        //    }

        //    public List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
        //        throw new NotImplementedException();
        //    }

        //    public IOid RestoreGenericOid(string[] encodedData) {
        //        throw new NotImplementedException();
        //    }

        //    public void PopulateViewModelKeys(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public void Init() {
        //        throw new NotImplementedException();
        //    }

        //    public void Shutdown() {
        //        throw new NotImplementedException();
        //    }

        //    public void StartTransaction() {
        //        throw new NotImplementedException();
        //    }

        //    public bool FlushTransaction() {
        //        throw new NotImplementedException();
        //    }

        //    public void AbortTransaction() {
        //        throw new NotImplementedException();
        //    }

        //    public void UserAbortTransaction() {
        //        throw new NotImplementedException();
        //    }

        //    public void EndTransaction() {
        //        throw new NotImplementedException();
        //    }

        //    public void AddCommand(IPersistenceCommand command) {
        //        throw new NotImplementedException();
        //    }

        //    public void Abort(ILifecycleManager objectManager, IFacetHolder holder) {
        //        throw new NotImplementedException();
        //    }

        //    public void InitDomainObject(object obj) {
        //        throw new NotImplementedException();
        //    }

        //    public void InitInlineObject(object root, object inlineObject) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject CreateInstance(INakedObjectSpecification specification) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject CreateViewModel(INakedObjectSpecification specification) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification) {
        //        throw new NotImplementedException();
        //    }

        //    public void Reload(INakedObject nakedObject) {
        //        throw new NotImplementedException();
        //    }

        //    public void RemoveAdapter(INakedObject objectToDispose) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject GetAdapterFor(object obj) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject GetAdapterFor(IOid oid) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
        //        throw new NotImplementedException();
        //    }

        //    public void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
        //        throw new NotImplementedException();
        //    }

        //    public object CreateObject(INakedObjectSpecification specification) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject GetViewModel(IOid oid) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
        //        throw new NotImplementedException();
        //    }

        //    public INakedObject NewAdapterForKnownObject(object domainObject, IOid transientOid) {
        //        throw new NotImplementedException();
        //    }
        //}

        //[Test]
        //public void TestMakePersistent() {
        //    algorithm.MakePersistent(roleAdapter, adder, NakedObjectsContext.Session);
        //    Assert.IsTrue(roleAdapter.ResolveState.IsResolved());
        //    Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
        //    Assert.IsTrue(adder.PersistedObjects.Contains(personAdapter));
        //    Assert.That(adder.PersistedCount, Is.EqualTo(2));
        //}

        //[Test]
        //public void TestMakePersistentFailsIfObjectAlreadyPersistent() {
        //    roleAdapter.ResolveState.Handle(Events.StartResolvingEvent);
        //    roleAdapter.ResolveState.Handle(Events.EndResolvingEvent);

        //    try {
        //        algorithm.MakePersistent(roleAdapter, adder, NakedObjectsContext.Session);
        //        Assert.Fail();
        //    }
        //    catch (NotPersistableException /*expected*/) {}
        //}

        //[Test]
        //public void TestMakePersistentFailsIfObjectMustBeTransient() {
        //    roleAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(roleAdapter.Specification));
        //    roleAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(roleAdapter.Specification));


        //    try {
        //        ((ProgrammableSpecification) roleAdapter.Specification).SetUpPersistable(Persistable.TRANSIENT);
        //        algorithm.MakePersistent(roleAdapter, adder, NakedObjectsContext.Session);
        //        Assert.Fail();
        //    }
        //    catch (NotPersistableException /*expected*/) {}
        //}

        //[Test]
        //public void TestMakePersistentSkipsAlreadyPersistedObjects() {
        //    algorithm.MakePersistent(personAdapter, adder, NakedObjectsContext.Session);
        //    adder.Reset();

        //    algorithm.MakePersistent(roleAdapter, adder, NakedObjectsContext.Session);

        //    Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
        //    Assert.That(adder.PersistedCount, Is.EqualTo(1));
        //}

        //[Test]
        //public void TestMakePersistentSkipsAggregatedObjects() {
        //    Person person = new Person();
        //    system.AdapterFor(person, Events.InitializeAggregateEvent);
        //    ((Role)roleAdapter.GetDomainObject()).Person = person;

        //    algorithm.MakePersistent(roleAdapter, adder, NakedObjectsContext.Session);

        //    Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
        //    Assert.That(adder.PersistedCount, Is.EqualTo(1));
        //    Assert.IsFalse(adder.PersistedObjects.Contains(personAdapter));
        //}
    }
}