// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class CallbackMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private CallbackMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof(ICreatedCallbackFacet),
                    typeof(IPersistingCallbackFacet),
                    typeof(IPersistedCallbackFacet),
                    typeof(IUpdatingCallbackFacet),
                    typeof(IUpdatedCallbackFacet),
                    typeof(ILoadingCallbackFacet),
                    typeof(ILoadedCallbackFacet),
                    typeof(IDeletingCallbackFacet),
                    typeof(IDeletedCallbackFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        private INakedObjectAdapter AdapterFor(object obj) {
            var session = new Mock<ISession>().Object;
            var lifecycleManager = new Mock<ILifecycleManager>().Object;
            var persistor = new Mock<IObjectPersistor>().Object;
            var manager = new Mock<INakedObjectManager>().Object;
            return new NakedObjectAdapter(Metamodel, session, persistor, lifecycleManager, manager, obj, null);
        }

        [TestMethod]
        public void TestCreatedLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer), "Created");
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(ICreatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is CreatedCallbackFacetViaMethod);
            var createdCallbackFacetViaMethod = (CreatedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, createdCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestDeletedLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer8), "Deleted");
            facetFactory.Process(Reflector, typeof(Customer8), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDeletedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletedCallbackFacetViaMethod);
            var deletedCallbackFacetViaMethod = (DeletedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, deletedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestDeletingLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer7), "Deleting");
            facetFactory.Process(Reflector, typeof(Customer7), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDeletingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DeletingCallbackFacetViaMethod);
            var deletingCallbackFacetViaMethod = (DeletingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, deletingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestLoadedLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer6), "Loaded");
            facetFactory.Process(Reflector, typeof(Customer6), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(ILoadedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadedCallbackFacetViaMethod);
            var loadedCallbackFacetViaMethod = (LoadedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, loadedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestLoadingLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer5), "Loading");
            facetFactory.Process(Reflector, typeof(Customer5), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(ILoadingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is LoadingCallbackFacetViaMethod);
            var loadingCallbackFacetViaMethod = (LoadingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, loadingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestOnPersistingErrorLifecycleMethodNullFacet() {
            var method = FindMethod(typeof(Customer10), "OnPersistingError", new[] {typeof(Exception)});
            Assert.IsNull(method);
            facetFactory.Process(Reflector, typeof(Customer10), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetNull);
        }

        [TestMethod]
        public void TestOnPersistingErrorLifecycleMethodPickedUpOn() {
            var method1 = FindMethod(typeof(Customer11), "OnUpdatingError", new[] {typeof(Exception)});
            var method2 = FindMethod(typeof(Customer11), "OnPersistingError", new[] {typeof(Exception)});
            facetFactory.Process(Reflector, typeof(Customer11), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IOnPersistingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnPersistingErrorCallbackFacetViaMethod);
            var onPersistingErrorCallbackFacetViaMethod = (OnPersistingErrorCallbackFacetViaMethod) facet;
            Assert.AreEqual(method2, onPersistingErrorCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method1, method2});
            // and test exception is passed through (assert in Customer11)
            var adapter = AdapterFor(new Customer11());
            onPersistingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [TestMethod]
        public void TestOnUpdatingErrorLifecycleMethodPickedUpOn() {
            var method1 = FindMethod(typeof(Customer11), "OnUpdatingError", new[] {typeof(Exception)});
            var method2 = FindMethod(typeof(Customer11), "OnPersistingError", new[] {typeof(Exception)});
            facetFactory.Process(Reflector, typeof(Customer11), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetViaMethod);
            var onUpdatingErrorCallbackFacetViaMethod = (OnUpdatingErrorCallbackFacetViaMethod) facet;
            Assert.AreEqual(method1, onUpdatingErrorCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method1, method2});
            // and test exception is passed through (assert in Customer11)
            var adapter = AdapterFor(new Customer11());
            onUpdatingErrorCallbackFacetViaMethod.Invoke(adapter, new Exception());
        }

        [TestMethod]
        public void TestOnUpdatingErrorLifecycleNullFacet() {
            var method = FindMethod(typeof(Customer10), "OnUpdatingError", new[] {typeof(Exception)});
            Assert.IsNull(method);
            facetFactory.Process(Reflector, typeof(Customer10), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IOnUpdatingErrorCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OnUpdatingErrorCallbackFacetNull);
        }

        [TestMethod]
        public void TestPersistedLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer2), "Persisted");
            facetFactory.Process(Reflector, typeof(Customer2), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetViaMethod);
            var persistedCallbackFacetViaMethod = (PersistedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestPersistingLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer1), "Persisting");
            facetFactory.Process(Reflector, typeof(Customer1), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetViaMethod);
            var persistingCallbackFacetViaMethod = (PersistingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, persistingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestSavedLifecycleMethodNotPickedUpOn() {
            facetFactory.Process(Reflector, typeof(Customer10), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IPersistedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistedCallbackFacetNull);
        }

        [TestMethod]
        public void TestSavingLifecycleMethodNotPickedUpOn() {
            facetFactory.Process(Reflector, typeof(Customer9), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IPersistingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PersistingCallbackFacetNull);
        }

        [TestMethod]
        public void TestUpdatedLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer4), "Updated");
            facetFactory.Process(Reflector, typeof(Customer4), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IUpdatedCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatedCallbackFacetViaMethod);
            var updatedCallbackFacetViaMethod = (UpdatedCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, updatedCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        [TestMethod]
        public void TestUpdatingLifecycleMethodPickedUpOn() {
            var method = FindMethod(typeof(Customer3), "Updating");
            facetFactory.Process(Reflector, typeof(Customer3), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IUpdatingCallbackFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is UpdatingCallbackFacetViaMethod);
            var updatingCallbackFacetViaMethod = (UpdatingCallbackFacetViaMethod) facet;
            Assert.AreEqual(method, updatingCallbackFacetViaMethod.GetMethod());
            AssertMethodsRemoved(new[] {method});
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new CallbackMethodsFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        

        private class Customer {
            public void Created() { }
        }

        private class Customer1 {
            public void Persisting() { }
        }

        private class Customer2 {
            public void Persisted() { }
        }

        private class Customer3 {
            public void Updating() { }
        }

        private class Customer4 {
            public void Updated() { }
        }

        private class Customer5 {
            public void Loading() { }
        }

        private class Customer6 {
            public void Loaded() { }
        }

        private class Customer7 {
            public void Deleting() { }
        }

        private class Customer8 {
            public void Deleted() { }
        }

        private class Customer9 {
            public void Saving() { }
        }

        private class Customer10 {
            public void Saved() { }
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}