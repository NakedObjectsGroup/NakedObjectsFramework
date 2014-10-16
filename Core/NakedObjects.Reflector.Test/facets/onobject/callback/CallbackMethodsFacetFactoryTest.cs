// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks {
    [TestFixture]
    public class CallbackMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new CallbackMethodsFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private CallbackMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
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

        private INakedObject AdapterFor(object obj) {
            ISession session = new Mock<ISession>().Object;
            ILifecycleManager lifecycleManager = new Mock<ILifecycleManager>().Object;
            IObjectPersistor persistor = new Mock<IObjectPersistor>().Object;
            INakedObjectManager manager = new Mock<INakedObjectManager>().Object;
            return new PocoAdapter(Metamodel, session, persistor, lifecycleManager, manager, obj, null);
        }

        // ReSharper disable UnusedMember.Local

        private class Customer {
            public void Created() {}
        }

        private class Customer1 {
            public void Persisting() {}
        }

        private class Customer2 {
            public void Persisted() {}
        }

        private class Customer3 {
            public void Updating() {}
        }

        private class Customer4 {
            public void Updated() {}
        }

        private class Customer5 {
            public void Loading() {}
        }

        private class Customer6 {
            public void Loaded() {}
        }

        private class Customer7 {
            public void Deleting() {}
        }

        private class Customer8 {
            public void Deleted() {}
        }

        private class Customer9 {
            public void Saving() {}
        }

        private class Customer10 {
            public void Saved() {}
        }

        // ReSharper disable UnusedParameter.Local
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

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local

        [Test]
        public void TestCreatedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer), "Created");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ICreatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is CreatedCallbackFacetViaMethod);
            var createdCallbackFacetViaMethod = (CreatedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, createdCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestDeletedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer8), "Deleted");
            facetFactory.Process(typeof (Customer8), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDeletedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletedCallbackFacetViaMethod);
            var deletedCallbackFacetViaMethod = (DeletedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, deletedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestDeletingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer7), "Deleting");
            facetFactory.Process(typeof (Customer7), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDeletingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletingCallbackFacetViaMethod);
            var deletingCallbackFacetViaMethod = (DeletingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, deletingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestLoadedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer6), "Loaded");
            facetFactory.Process(typeof (Customer6), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ILoadedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadedCallbackFacetViaMethod);
            var loadedCallbackFacetViaMethod = (LoadedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, loadedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestLoadingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer5), "Loading");
            facetFactory.Process(typeof (Customer5), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ILoadingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadingCallbackFacetViaMethod);
            var loadingCallbackFacetViaMethod = (LoadingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, loadingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestOnPersistingErrorLifecycleMethodNullFacet() {
            MethodInfo method = FindMethod(typeof (Customer10), "OnPersistingError", new[] {typeof (Exception)});
            Assert.IsNull(method);
            facetFactory.Process(typeof (Customer10), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetNull);
        }

        [Test]
        public void TestOnPersistingErrorLifecycleMethodPickedUpOn() {
            MethodInfo method1 = FindMethod(typeof (Customer11), "OnUpdatingError", new[] {typeof (Exception)});
            MethodInfo method2 = FindMethod(typeof (Customer11), "OnPersistingError", new[] {typeof (Exception)});
            facetFactory.Process(typeof (Customer11), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetViaMethod);
            var onPersistingErrorCallbackFacetViaMethod = (OnPersistingErrorCallbackFacetViaMethod) facet;
            Assert.AreEqual(method2, onPersistingErrorCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method1, method2});
            // and test exception is passed through (assert in Customer11)
            INakedObject adapter = AdapterFor(new Customer11());
            onPersistingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [Test]
        public void TestOnUpdatingErrorLifecycleMethodPickedUpOn() {
            MethodInfo method1 = FindMethod(typeof (Customer11), "OnUpdatingError", new[] {typeof (Exception)});
            MethodInfo method2 = FindMethod(typeof (Customer11), "OnPersistingError", new[] {typeof (Exception)});
            facetFactory.Process(typeof (Customer11), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetViaMethod);
            var onUpdatingErrorCallbackFacetViaMethod = (OnUpdatingErrorCallbackFacetViaMethod) facet;
            Assert.AreEqual(method1, onUpdatingErrorCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method1, method2});
            // and test exception is passed through (assert in Customer11)
            INakedObject adapter = AdapterFor(new Customer11());
            onUpdatingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [Test]
        public void TestOnUpdatingErrorLifecycleNullFacet() {
            MethodInfo method = FindMethod(typeof (Customer10), "OnUpdatingError", new[] {typeof (Exception)});
            Assert.IsNull(method);
            facetFactory.Process(typeof (Customer10), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetNull);
        }

        [Test]
        public void TestPersistedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer2), "Persisted");
            facetFactory.Process(typeof (Customer2), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetViaMethod);
            var persistedCallbackFacetViaMethod = (PersistedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestPersistingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer1), "Persisting");
            facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetViaMethod);
            var persistingCallbackFacetViaMethod = (PersistingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestSavedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer10), "Saved");
            facetFactory.Process(typeof (Customer10), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetViaMethod);
            var persistedCallbackFacetViaMethod = (PersistedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestSavingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer9), "Saving");
            facetFactory.Process(typeof (Customer9), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetViaMethod);
            var persistingCallbackFacetViaMethod = (PersistingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestUpdatedLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer4), "Updated");
            facetFactory.Process(typeof (Customer4), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IUpdatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatedCallbackFacetViaMethod);
            var updatedCallbackFacetViaMethod = (UpdatedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, updatedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [Test]
        public void TestUpdatingLifecycleMethodPickedUpOn() {
            MethodInfo method = FindMethod(typeof (Customer3), "Updating");
            facetFactory.Process(typeof (Customer3), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IUpdatingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatingCallbackFacetViaMethod);
            var updatingCallbackFacetViaMethod = (UpdatingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, updatingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}