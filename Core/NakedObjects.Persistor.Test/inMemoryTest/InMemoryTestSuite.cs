// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using System.Linq;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Persistor.TestData;
using NakedObjects.Persistor.TestSuite;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;
using TestData;

namespace NakedObjects.Persistor.InMemoryTest {
    [TestFixture, Ignore]
    public class InMemoryTestSuite : AcceptanceTestCase {

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            // replace INakedObjectStore types

            container.RegisterType<IOidGenerator, SimpleOidGenerator>(new InjectionConstructor(typeof(INakedObjectReflector), 0L));
            container.RegisterType<IPersistAlgorithm, DefaultPersistAlgorithm>();
            container.RegisterType<INakedObjectStore, MemoryObjectStore>();
            container.RegisterType<IIdentityMap, IdentityMapImpl>();
        }


        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetupFixture() {
            InitializeNakedObjectsFramework();
        }

        [TestFixtureTearDown]
        public void TearDownFixture() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public void Setup() {
            StartTest();

            // reset events from fixtures running
            NakedObjectsFramework.ObjectPersistor.Instances<Person>().ForEach(p => p.ResetEvents());
            NakedObjectsFramework.ObjectPersistor.Instances<Person>().Select(p => p.Address).ForEach(a => a.ResetEvents());
            NakedObjectsFramework.ObjectPersistor.Instances<Product>().ForEach(p => p.ResetEvents());

            NakedObjectsFramework.ObjectPersistor.Reset();

            tests = new PersistorTestSuite(NakedObjectsFramework);
        }

        [TearDown]
        public void TearDown() {

        }

        private PersistorTestSuite tests;

        #endregion

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {new TestDataFixture()}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Person>()}); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new SimpleRepository<Product>() }); }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new object[] { new SimpleRepository<Address>() }); }
        }

        [Test]
        public void AddToCollectionOnPersistent() {
            tests.AddToCollectionOnPersistent();
        }

        [Test]
        public void CanAccessCollectionProperty() {
            tests.CanAccessCollectionProperty();
        }

        [Test]
        public void CanAccessReferenceProperty() {
            tests.CanAccessReferenceProperty();
        }

        [Test]
        public void ChangeReferenceOnPersistentCallsUpdatingUpdated() {
            tests.ChangeReferenceOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeReferenceOnPersistentNotifiesUi() {
            tests.ChangeReferenceOnPersistentNotifiesUi();
        }

        [Test]
        public void ChangeScalarOnInlineObjectCallsUpdatingUpdated() {
            tests.ChangeScalarOnInlineObjectCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeScalarOnPersistentCallsUpdatingUpdated() {
            tests.ChangeScalarOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeScalarOnPersistentNotifiesUi() {
            tests.ChangeScalarOnPersistentNotifiesUi();
        }

        [Test]
        public void ClearCollectionOnPersistent() {
            tests.ClearCollectionOnPersistent();
        }

        [Test]
        public void ClearCollectionOnPersistentCallsUpdatingUpdated() {
            tests.ClearCollectionOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ClearCollectionOnPersistentNotifiesUi() {
            tests.ClearCollectionOnPersistentNotifiesUi();
        }

        [Test]
        public void CollectionPropertyCollectionResolveStateIsPersistent() {
            tests.CollectionPropertyCollectionResolveStateIsPersistent();
        }

        [Test]
        public void CollectionPropertyHasLoadingLoadedCalled() {
            tests.CollectionPropertyHasLoadingLoadedCalled();
        }

        [Test]
        public void CollectionPropertyObjectHasContainerInjected() {
            tests.CollectionPropertyObjectHasContainerInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasMenuServiceInjected() {
            tests.CollectionPropertyObjectHasMenuServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasContributedServiceInjected() {
            tests.CollectionPropertyObjectHasContributedServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasSystemServiceInjected() {
            tests.CollectionPropertyObjectHasSystemServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasVersion() {
            tests.CollectionPropertyObjectHasVersion();
        }

        [Test]
        public void CollectionPropertyObjectResolveStateIsPersistent() {
            tests.CollectionPropertyObjectResolveStateIsPersistent();
        }

        [Test]
        public void CreateTransientInlineInstance() {
            tests.CreateTransientInlineInstance();
        }

        [Test]
        public void EmptyCollectionPropertyCollectionResolveStateIsPersistent() {
            tests.EmptyCollectionPropertyCollectionResolveStateIsPersistent();
        }

        [Test]
        public void GetInlineInstance() {
            tests.GetInlineInstance();
        }

        [Test]
        public void GetInstanceFromInstancesOfSpecification() {
            tests.GetInstanceFromInstancesOfSpecification();
        }

        [Test]
        public void GetInstanceFromInstancesOfT() {
            tests.GetInstanceFromInstancesOfT();
        }

        [Test]
        public void GetInstanceFromInstancesOfType() {
            tests.GetInstanceFromInstancesOfType();
        }

        [Test]
        public void GetInstanceHasVersion() {
            tests.GetInstanceHasVersion();
        }

        [Test]
        public void GetInstanceIsAlwaysSameObject() {
            tests.GetInstanceIsAlwaysSameObject();
        }

        [Test]
        public void GetInstanceResolveStateIsPersistent() {
            tests.GetInstanceResolveStateIsPersistent();
        }

        [Test]
        public void InlineObjectCallsCreated() {
            tests.InlineObjectCallsCreated();
        }

        [Test]
        public void InlineObjectHasContainerInjected() {
            tests.InlineObjectHasContainerInjected();
        }

        [Test]
        public void InlineObjectHasLoadingLoadedCalled() {
            tests.InlineObjectHasLoadingLoadedCalled();
        }

        [Test]
        public void InlineObjectHasParentInjected() {
            tests.InlineObjectHasParentInjected();
        }

        [Test]
        public void InlineObjectHasServiceInjected() {
            tests.InlineObjectHasServiceInjected();
        }

        [Test]
        public void InlineObjectHasVersion() {
            tests.InlineObjectHasVersion();
        }

        [Test]
        public void LoadObjectReturnSameObject() {
            tests.LoadObjectReturnSameObject();
        }

        [Test]
        public void NewObjectHasContainerInjected() {
            tests.NewObjectHasContainerInjected();
        }

        [Test]
        public void NewObjectHasCreatedCalled() {
            tests.NewObjectHasCreatedCalled();
        }

        [Test]
        public void NewObjectHasServiceInjected() {
            tests.NewObjectHasServiceInjected();
        }

        [Test]
        public void NewObjectHasVersion() {
            tests.NewObjectHasVersion();
        }

        [Test]
        public void NewObjectIsCreated() {
            tests.NewObjectIsCreated();
        }

        [Test]
        public void NewObjectIsTransient() {
            tests.NewObjectIsTransient();
        }

        [Test]
        public void PersistentObjectHasContainerInjected() {
            tests.PersistentObjectHasContainerInjected();
        }

        [Test]
        public void PersistentObjectHasLoadingLoadedCalled() {
            tests.PersistentObjectHasLoadingLoadedCalled();
        }

        [Test]
        public void PersistentObjectHasServiceInjected() {
            tests.PersistentObjectHasServiceInjected();
        }

        [Test]
        public void ReferencePropertyHasLoadingLoadedCalled() {
            tests.ReferencePropertyHasLoadingLoadedCalled();
        }

        [Test]
        public void ReferencePropertyObjectHasContainerInjected() {
            tests.ReferencePropertyObjectHasContainerInjected();
        }

        [Test]
        public void ReferencePropertyObjectHasServiceInjected() {
            tests.ReferencePropertyObjectHasServiceInjected();
        }

        [Test]
        public void ReferencePropertyObjectHasVersion() {
            tests.ReferencePropertyObjectHasVersion();
        }

        [Test]
        public void ReferencePropertyObjectResolveStateIsPersistent() {
            tests.ReferencePropertyObjectResolveStateIsPersistent();
        }

        [Test]
        public void RemoveFromCollectionOnPersistent() {
            tests.RemoveFromCollectionOnPersistent();
        }

        [Test]
        public void SaveInlineObjectCallsPersistingPersisted() {
            tests.SaveInlineObjectCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectCallsPersistingPersisted() {
            tests.SaveNewObjectCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectCallsPersistingPersistedRecursively() {
            tests.SaveNewObjectCallsPersistingPersistedRecursively();
        }

        [Test]
        public void SaveNewObjectTransientCollectionItemCallsPersistingPersisted() {
            tests.SaveNewObjectTransientCollectionItemCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectTransientReferenceCallsPersistingPersisted() {
            tests.SaveNewObjectTransientReferenceCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectWithPersistentItemCollectionItem() {
            tests.SaveNewObjectWithPersistentItemCollectionItem();
        }

        [Test]
        public void SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() {
            tests.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction();
        }

        [Test]
        public void SaveNewObjectWithPersistentReference() {
            tests.SaveNewObjectWithPersistentReference();
        }

        [Test]
        public void SaveNewObjectWithPersistentReferenceInSeperateTransaction() {
            tests.SaveNewObjectWithPersistentReferenceInSeperateTransaction();
        }

        [Test]
        public void SaveNewObjectWithScalars() {
            tests.SaveNewObjectWithScalars();
        }

        [Test]
        public void SaveNewObjectWithValidate() {
            tests.SaveNewObjectWithValidate();
        }

        [Test]
        public void ChangeObjectWithValidate() {
            tests.ChangeObjectWithValidate();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceInvalid() {
            tests.SaveNewObjectWithTransientReferenceInvalid();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceObjectInvalid() {
            tests.SaveNewObjectWithTransientReferenceObjectInvalid();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceValidateAssocInvalid() {
            tests.SaveNewObjectWithTransientReferenceValidateAssocInvalid();
        }


        [Test]
        public void SaveNewObjectWithTransientCollectionItem() {
            tests.SaveNewObjectWithTransientCollectionItem();
        }

        [Test]
        public void SaveNewObjectWithTransientReference() {
            tests.SaveNewObjectWithTransientReference();
        }

        [Test]
        public void TrainsientInlineObjectHasVersion() {
            tests.TrainsientInlineObjectHasVersion();
        }

        [Test]
        public void TransientInlineObjectHasContainerInjected() {
            tests.TransientInlineObjectHasContainerInjected();
        }

        [Test]
        public void TransientInlineObjectHasParentInjected() {
            tests.TransientInlineObjectHasParentInjected();
        }

        [Test]
        public void TransientInlineObjectHasServiceInjected() {
            tests.TransientInlineObjectHasServiceInjected();
        }

        [Test]
        public void UpdateInlineObjectUpdatesUi() {
            tests.UpdateInlineObjectUpdatesUi();
        }

        [Test]
        public void GetKeysReturnsKeys() {
            tests.GetKeysReturnsKeys();
        }

        [Test]
        public void FindByKey() {
            tests.FindByKey();
        }

        [Test]
        public void CreateAndDeleteNewObjectWithScalars() {
            tests.CreateAndDeleteNewObjectWithScalars();
        }

        [Test]
        public void DeleteObjectCallsDeletingDeleted() {
            tests.DeleteObjectCallsDeletingDeleted();
        }

        [Test]
        public void CountCollectionOnPersistent() {
            tests.CountCollectionOnPersistent();
        }

        [Test]
        public void CountUnResolvedCollectionOnPersistent() {
            tests.CountUnResolvedCollectionOnPersistent();
        }

        [Test]
        public void CountEmptyCollectionOnTransient() {
            tests.CountEmptyCollectionOnTransient();
        }

        [Test]
        public void CountCollectionOnTransient() {
            tests.CountCollectionOnTransient();
        } 

        [Test]
        [Ignore("#973")]
        public void RefreshResetsObject() {
            tests.RefreshResetsObject();
        }
    }
}