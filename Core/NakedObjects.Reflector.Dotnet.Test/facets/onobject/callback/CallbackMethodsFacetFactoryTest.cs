// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Testing;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    [TestFixture]
    public class CallbackMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private CallbackMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new Type[] {
                                      typeof (ICreatedCallbackFacet),
                                      typeof (IPersistingCallbackFacet),
                                      typeof (IPersistedCallbackFacet),
                                      typeof (IUpdatingCallbackFacet),
                                      typeof (IUpdatedCallbackFacet),
                                      typeof (ILoadingCallbackFacet),
                                      typeof (ILoadedCallbackFacet),
                                      typeof (IDeletingCallbackFacet),
                                      typeof (IDeletedCallbackFacet)
                                  };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new CallbackMethodsFacetFactory { Reflector = reflector };
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestCreatedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer), "Created");
            facetFactory.Process(typeof(Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(ICreatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is CreatedCallbackFacetViaMethod);
            CreatedCallbackFacetViaMethod createdCallbackFacetViaMethod = (CreatedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, createdCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestPersistingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer1), "Persisting");
            facetFactory.Process(typeof(Customer1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetViaMethod);
            PersistingCallbackFacetViaMethod persistingCallbackFacetViaMethod = (PersistingCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, persistingCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestPersistedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer2), "Persisted");
            facetFactory.Process(typeof(Customer2), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetViaMethod);
            PersistedCallbackFacetViaMethod persistedCallbackFacetViaMethod = (PersistedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, persistedCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestSavingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer9), "Saving");
            facetFactory.Process(typeof(Customer9), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetViaMethod);
            PersistingCallbackFacetViaMethod persistingCallbackFacetViaMethod = (PersistingCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, persistingCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestSavedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer10), "Saved");
            facetFactory.Process(typeof(Customer10), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetViaMethod);
            PersistedCallbackFacetViaMethod persistedCallbackFacetViaMethod = (PersistedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, persistedCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestUpdatingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer3), "Updating");
            facetFactory.Process(typeof(Customer3), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IUpdatingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatingCallbackFacetViaMethod);
            UpdatingCallbackFacetViaMethod updatingCallbackFacetViaMethod = (UpdatingCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, updatingCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestUpdatedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer4), "Updated");
            facetFactory.Process(typeof(Customer4), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IUpdatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatedCallbackFacetViaMethod);
            UpdatedCallbackFacetViaMethod updatedCallbackFacetViaMethod = (UpdatedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, updatedCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestLoadingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer5), "Loading");
            facetFactory.Process(typeof(Customer5), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(ILoadingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadingCallbackFacetViaMethod);
            LoadingCallbackFacetViaMethod loadingCallbackFacetViaMethod = (LoadingCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, loadingCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestLoadedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer6), "Loaded");
            facetFactory.Process(typeof(Customer6), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(ILoadedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadedCallbackFacetViaMethod);
            LoadedCallbackFacetViaMethod loadedCallbackFacetViaMethod = (LoadedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, loadedCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestDeletingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer7), "Deleting");
            facetFactory.Process(typeof(Customer7), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDeletingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletingCallbackFacetViaMethod);
            DeletingCallbackFacetViaMethod deletingCallbackFacetViaMethod = (DeletingCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, deletingCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestDeletedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer8), "Deleted");
            facetFactory.Process(typeof(Customer8), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDeletedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletedCallbackFacetViaMethod);
            DeletedCallbackFacetViaMethod deletedCallbackFacetViaMethod = (DeletedCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, deletedCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));
        }

        [Test]
        public void TestOnUpdatingErrorLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer11), "OnUpdatingError", new[] { typeof(Exception) });
            facetFactory.Process(typeof(Customer11), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetViaMethod);
            var onUpdatingErrorCallbackFacetViaMethod = (OnUpdatingErrorCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, onUpdatingErrorCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));

            // and test exception is passed through (assert in Customer11)
            var adapter = new ProgrammableTestSystem().AdapterFor(new Customer11());
            onUpdatingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [Test]
        public void TestOnPersistingErrorLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof(Customer11), "OnPersistingError", new[] {typeof(Exception)});
            facetFactory.Process(typeof(Customer11), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetViaMethod);
            var onPersistingErrorCallbackFacetViaMethod = (OnPersistingErrorCallbackFacetViaMethod)facet;
            Assert.AreEqual(method, onPersistingErrorCallbackFacetViaMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(method));

            // and test exception is passed through (assert in Customer11)
            var adapter = new ProgrammableTestSystem().AdapterFor(new Customer11());
            onPersistingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [Test]
        public void TestOnUpdatingErrorLifecycleNullFacet() {
            MethodInfo method = FindMethod(typeof(Customer10), "OnUpdatingError", new[] {typeof(Exception)});
            Assert.IsNull(method);
            facetFactory.Process(typeof(Customer10), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetNull);      
        }

        [Test]
        public void TestOnPersistingErrorLifecycleMethodNullFacet() {
            MethodInfo method = FindMethod(typeof(Customer10), "OnPersistingError", new[] { typeof(Exception) });
            Assert.IsNull(method);
            facetFactory.Process(typeof(Customer10), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetNull);
        }

        #region Nested Type: Customer

        private class Customer {
            public void Created() { }
        }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            public void Persisting() { }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            public void Persisted() { }
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            public void Updating() { }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public void Updated() { }
        }

        #endregion

        #region Nested Type: Customer5

        private class Customer5 {
            public void Loading() { }
        }

        #endregion

        #region Nested Type: Customer6

        private class Customer6 {
            public void Loaded() { }
        }

        #endregion

        #region Nested Type: Customer7

        private class Customer7 {
            public void Deleting() { }
        }

        #endregion

        #region Nested Type: Customer8

        private class Customer8 {
            public void Deleted() { }
        }

        #endregion

        #region Nested Type: Customer9

        private class Customer9 {
            public void Saving() { }
        }

        #endregion

        #region Nested Type: Customer10

        private class Customer10 {
            public void Saved() { }
        }

        #endregion

        #region Nested Type: Customer11

        private class Customer11 {
            public string OnPersistingError(Exception e) {
                Assert.IsNotNull(e);
                return string.Empty;
            }
            public string OnUpdatingError(Exception e) {
                Assert.IsNotNull(e);
                return string.Empty;
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}