// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using System.Linq;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Persistor.TestData;
using NakedObjects.Persistor.TestSuite;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;
using TestData;

namespace NakedObjects.Persistor.InMemoryTest {
    [TestFixture]
    public class InMemoryTestSuite : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            InitializeNakedObjectsFramework();

            // reset events from fixtures running
            NakedObjectsContext.ObjectPersistor.Instances<Person>().ForEach(p => p.ResetEvents());
            NakedObjectsContext.ObjectPersistor.Instances<Person>().Select( p => p.Address).ForEach(a => a.ResetEvents());
            NakedObjectsContext.ObjectPersistor.Instances<Product>().ForEach(p => p.ResetEvents());
           
            NakedObjectsContext.ObjectPersistor.Reset();

        }

        [TearDown]
        public void TearDown() {
            CleanupNakedObjectsFramework();
        }

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
            PersistorTestSuite.AddToCollectionOnPersistent();
        }

        [Test]
        public void CanAccessCollectionProperty() {
            PersistorTestSuite.CanAccessCollectionProperty();
        }

        [Test]
        public void CanAccessReferenceProperty() {
            PersistorTestSuite.CanAccessReferenceProperty();
        }

        [Test]
        public void ChangeReferenceOnPersistentCallsUpdatingUpdated() {
            PersistorTestSuite.ChangeReferenceOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeReferenceOnPersistentNotifiesUi() {
            PersistorTestSuite.ChangeReferenceOnPersistentNotifiesUi();
        }

        [Test]
        public void ChangeScalarOnInlineObjectCallsUpdatingUpdated() {
            PersistorTestSuite.ChangeScalarOnInlineObjectCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeScalarOnPersistentCallsUpdatingUpdated() {
            PersistorTestSuite.ChangeScalarOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ChangeScalarOnPersistentNotifiesUi() {
            PersistorTestSuite.ChangeScalarOnPersistentNotifiesUi();
        }

        [Test]
        public void ClearCollectionOnPersistent() {
            PersistorTestSuite.ClearCollectionOnPersistent();
        }

        [Test]
        public void ClearCollectionOnPersistentCallsUpdatingUpdated() {
            PersistorTestSuite.ClearCollectionOnPersistentCallsUpdatingUpdated();
        }

        [Test]
        public void ClearCollectionOnPersistentNotifiesUi() {
            PersistorTestSuite.ClearCollectionOnPersistentNotifiesUi();
        }

        [Test]
        public void CollectionPropertyCollectionResolveStateIsPersistent() {
            PersistorTestSuite.CollectionPropertyCollectionResolveStateIsPersistent();
        }

        [Test]
        public void CollectionPropertyHasLoadingLoadedCalled() {
            PersistorTestSuite.CollectionPropertyHasLoadingLoadedCalled();
        }

        [Test]
        public void CollectionPropertyObjectHasContainerInjected() {
            PersistorTestSuite.CollectionPropertyObjectHasContainerInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasMenuServiceInjected() {
            PersistorTestSuite.CollectionPropertyObjectHasMenuServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasContributedServiceInjected() {
            PersistorTestSuite.CollectionPropertyObjectHasContributedServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasSystemServiceInjected() {
            PersistorTestSuite.CollectionPropertyObjectHasSystemServiceInjected();
        }

        [Test]
        public void CollectionPropertyObjectHasVersion() {
            PersistorTestSuite.CollectionPropertyObjectHasVersion();
        }

        [Test]
        public void CollectionPropertyObjectResolveStateIsPersistent() {
            PersistorTestSuite.CollectionPropertyObjectResolveStateIsPersistent();
        }

        [Test]
        public void CreateTransientInlineInstance() {
            PersistorTestSuite.CreateTransientInlineInstance();
        }

        [Test]
        public void EmptyCollectionPropertyCollectionResolveStateIsPersistent() {
            PersistorTestSuite.EmptyCollectionPropertyCollectionResolveStateIsPersistent();
        }

        [Test]
        public void GetInlineInstance() {
            PersistorTestSuite.GetInlineInstance();
        }

        [Test]
        public void GetInstanceFromInstancesOfSpecification() {
            PersistorTestSuite.GetInstanceFromInstancesOfSpecification();
        }

        [Test]
        public void GetInstanceFromInstancesOfT() {
            PersistorTestSuite.GetInstanceFromInstancesOfT();
        }

        [Test]
        public void GetInstanceFromInstancesOfType() {
            PersistorTestSuite.GetInstanceFromInstancesOfType();
        }

        [Test]
        public void GetInstanceHasVersion() {
            PersistorTestSuite.GetInstanceHasVersion();
        }

        [Test]
        public void GetInstanceIsAlwaysSameObject() {
            PersistorTestSuite.GetInstanceIsAlwaysSameObject();
        }

        [Test]
        public void GetInstanceResolveStateIsPersistent() {
            PersistorTestSuite.GetInstanceResolveStateIsPersistent();
        }

        [Test]
        public void InlineObjectCallsCreated() {
            PersistorTestSuite.InlineObjectCallsCreated();
        }

        [Test]
        public void InlineObjectHasContainerInjected() {
            PersistorTestSuite.InlineObjectHasContainerInjected();
        }

        [Test]
        public void InlineObjectHasLoadingLoadedCalled() {
            PersistorTestSuite.InlineObjectHasLoadingLoadedCalled();
        }

        [Test]
        public void InlineObjectHasParentInjected() {
            PersistorTestSuite.InlineObjectHasParentInjected();
        }

        [Test]
        public void InlineObjectHasServiceInjected() {
            PersistorTestSuite.InlineObjectHasServiceInjected();
        }

        [Test]
        public void InlineObjectHasVersion() {
            PersistorTestSuite.InlineObjectHasVersion();
        }

        [Test]
        public void LoadObjectReturnSameObject() {
            PersistorTestSuite.LoadObjectReturnSameObject();
        }

        [Test]
        public void NewObjectHasContainerInjected() {
            PersistorTestSuite.NewObjectHasContainerInjected();
        }

        [Test]
        public void NewObjectHasCreatedCalled() {
            PersistorTestSuite.NewObjectHasCreatedCalled();
        }

        [Test]
        public void NewObjectHasServiceInjected() {
            PersistorTestSuite.NewObjectHasServiceInjected();
        }

        [Test]
        public void NewObjectHasVersion() {
            PersistorTestSuite.NewObjectHasVersion();
        }

        [Test]
        public void NewObjectIsCreated() {
            PersistorTestSuite.NewObjectIsCreated();
        }

        [Test]
        public void NewObjectIsTransient() {
            PersistorTestSuite.NewObjectIsTransient();
        }

        [Test]
        public void PersistentObjectHasContainerInjected() {
            PersistorTestSuite.PersistentObjectHasContainerInjected();
        }

        [Test]
        public void PersistentObjectHasLoadingLoadedCalled() {
            PersistorTestSuite.PersistentObjectHasLoadingLoadedCalled();
        }

        [Test]
        public void PersistentObjectHasServiceInjected() {
            PersistorTestSuite.PersistentObjectHasServiceInjected();
        }

        [Test]
        public void ReferencePropertyHasLoadingLoadedCalled() {
            PersistorTestSuite.ReferencePropertyHasLoadingLoadedCalled();
        }

        [Test]
        public void ReferencePropertyObjectHasContainerInjected() {
            PersistorTestSuite.ReferencePropertyObjectHasContainerInjected();
        }

        [Test]
        public void ReferencePropertyObjectHasServiceInjected() {
            PersistorTestSuite.ReferencePropertyObjectHasServiceInjected();
        }

        [Test]
        public void ReferencePropertyObjectHasVersion() {
            PersistorTestSuite.ReferencePropertyObjectHasVersion();
        }

        [Test]
        public void ReferencePropertyObjectResolveStateIsPersistent() {
            PersistorTestSuite.ReferencePropertyObjectResolveStateIsPersistent();
        }

        [Test]
        public void RemoveFromCollectionOnPersistent() {
            PersistorTestSuite.RemoveFromCollectionOnPersistent();
        }

        [Test]
        public void SaveInlineObjectCallsPersistingPersisted() {
            PersistorTestSuite.SaveInlineObjectCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectCallsPersistingPersisted() {
            PersistorTestSuite.SaveNewObjectCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectCallsPersistingPersistedRecursively() {
            PersistorTestSuite.SaveNewObjectCallsPersistingPersistedRecursively();
        }

        [Test]
        public void SaveNewObjectTransientCollectionItemCallsPersistingPersisted() {
            PersistorTestSuite.SaveNewObjectTransientCollectionItemCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectTransientReferenceCallsPersistingPersisted() {
            PersistorTestSuite.SaveNewObjectTransientReferenceCallsPersistingPersisted();
        }

        [Test]
        public void SaveNewObjectWithPersistentItemCollectionItem() {
            PersistorTestSuite.SaveNewObjectWithPersistentItemCollectionItem();
        }

        [Test]
        public void SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction() {
            PersistorTestSuite.SaveNewObjectWithPersistentItemCollectionItemInSeperateTransaction();
        }

        [Test]
        public void SaveNewObjectWithPersistentReference() {
            PersistorTestSuite.SaveNewObjectWithPersistentReference();
        }

        [Test]
        public void SaveNewObjectWithPersistentReferenceInSeperateTransaction() {
            PersistorTestSuite.SaveNewObjectWithPersistentReferenceInSeperateTransaction();
        }

        [Test]
        public void SaveNewObjectWithScalars() {
            PersistorTestSuite.SaveNewObjectWithScalars();
        }

        [Test]
        public void SaveNewObjectWithValidate() {
            PersistorTestSuite.SaveNewObjectWithValidate();
        }

        [Test]
        public void ChangeObjectWithValidate() {
            PersistorTestSuite.ChangeObjectWithValidate();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceInvalid() {
            PersistorTestSuite.SaveNewObjectWithTransientReferenceInvalid();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceObjectInvalid() {
            PersistorTestSuite.SaveNewObjectWithTransientReferenceObjectInvalid();
        }

        [Test]
        public void SaveNewObjectWithTransientReferenceValidateAssocInvalid() {
            PersistorTestSuite.SaveNewObjectWithTransientReferenceValidateAssocInvalid();
        }


        [Test]
        public void SaveNewObjectWithTransientCollectionItem() {
            PersistorTestSuite.SaveNewObjectWithTransientCollectionItem();
        }

        [Test]
        public void SaveNewObjectWithTransientReference() {
            PersistorTestSuite.SaveNewObjectWithTransientReference();
        }

        [Test]
        public void TrainsientInlineObjectHasVersion() {
            PersistorTestSuite.TrainsientInlineObjectHasVersion();
        }

        [Test]
        public void TransientInlineObjectHasContainerInjected() {
            PersistorTestSuite.TransientInlineObjectHasContainerInjected();
        }

        [Test]
        public void TransientInlineObjectHasParentInjected() {
            PersistorTestSuite.TransientInlineObjectHasParentInjected();
        }

        [Test]
        public void TransientInlineObjectHasServiceInjected() {
            PersistorTestSuite.TransientInlineObjectHasServiceInjected();
        }

        [Test]
        public void UpdateInlineObjectUpdatesUi() {
            PersistorTestSuite.UpdateInlineObjectUpdatesUi();
        }

        [Test]
        public void GetKeysReturnsKeys() {
            PersistorTestSuite.GetKeysReturnsKeys();
        }

        [Test]
        public void FindByKey() {
            PersistorTestSuite.FindByKey();
        }

        [Test]
        public void CreateAndDeleteNewObjectWithScalars() {
            PersistorTestSuite.CreateAndDeleteNewObjectWithScalars();
        }

        [Test]
        public void DeleteObjectCallsDeletingDeleted() {
            PersistorTestSuite.DeleteObjectCallsDeletingDeleted();
        }

        [Test]
        public void CountCollectionOnPersistent() {
            PersistorTestSuite.CountCollectionOnPersistent();
        }

        [Test]
        public void CountUnResolvedCollectionOnPersistent() {
            PersistorTestSuite.CountUnResolvedCollectionOnPersistent();
        }

        [Test]
        public void CountEmptyCollectionOnTransient() {
            PersistorTestSuite.CountEmptyCollectionOnTransient();
        }

        [Test]
        public void CountCollectionOnTransient() {
            PersistorTestSuite.CountCollectionOnTransient();
        } 

        [Test]
        [Ignore("#973")]
        public void RefreshResetsObject() {
            PersistorTestSuite.RefreshResetsObject();
        }
    }
}