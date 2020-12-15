// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Profile;
using NakedObjects.Profile;

namespace NakedObjects.Meta.Test.Profile {
    [TestClass]
    public class ProfileManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() { }

        #endregion

        private static Mock<ISpecification> SetupMocks(out Mock<ISpecification> testHolder) {
            var testSpec = new Mock<ISpecification>();
            testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);
            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);
            return testSpec;
        }

        private static void TestDecorated<TFacet, TResult>(ProfileManager manager) where TFacet : class, IFacet {
            var testSpec = SetupMocks(out var testHolder);
            var testFacet = new Mock<TFacet>();

            testFacet.Setup(n => n.FacetType).Returns(typeof(TFacet));
            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof(TResult));
        }

        private static void TestNotDecorated<TFacet, TResult>(ProfileManager manager) where TFacet : class, IFacet {
            var testSpec = SetupMocks(out var testHolder);
            var testFacet = new Mock<TFacet>();

            testFacet.Setup(n => n.FacetType).Returns(typeof(TFacet));
            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsNotInstanceOfType(facet, typeof(TResult));
            Assert.IsInstanceOfType(facet, typeof(TFacet));
        }

        private static void TestDecorateFacet<TFacet, TResult>(ProfileEvent eventToTest) where TFacet : class, IFacet {
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());
            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent> {eventToTest});

            var manager = new ProfileManager(config.Object);

            TestDecorated<TFacet, TResult>(manager);
        }

        private static void TestDecorateCallbackFacet<T>(ProfileEvent eventToTest) where T : class, ICallbackFacet {
            TestDecorateFacet<T, ProfileCallbackFacet>(eventToTest);
        }

        [TestMethod]
        public void TestCreateOk() {
            var config = new ProfileConfiguration<IProfiler>();

            // ReSharper disable once UnusedVariable
            var sink = new ProfileManager(config);
        }

        [TestMethod]
        public void TestCreateWrongDefaultProfilerType() {
            var config = new Mock<IProfileConfiguration>();

            config.Setup(c => c.Profiler).Returns(typeof(object));

            try {
                // ReSharper disable once UnusedVariable
                var sink = new ProfileManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("System.Object is not an IProfiler", expected.Message);
            }
        }

        [TestMethod]
        public void TestDecorateActionInvocationFacet() {
            TestDecorateFacet<IActionInvocationFacet, ProfileActionInvocationFacet>(ProfileEvent.ActionInvocation);
        }

        [TestMethod]
        public void TestDecoratePropertySetFacet() {
            TestDecorateFacet<IPropertySetterFacet, ProfilePropertySetterFacet>(ProfileEvent.PropertySet);
        }

        [TestMethod]
        public void TestDecorateCreatedFacet() {
            TestDecorateCallbackFacet<ICreatedCallbackFacet>(ProfileEvent.Created);
        }

        [TestMethod]
        public void TestDecorateDeletedFacet() {
            TestDecorateCallbackFacet<IDeletedCallbackFacet>(ProfileEvent.Deleted);
        }

        [TestMethod]
        public void TestDecorateDeletingFacet() {
            TestDecorateCallbackFacet<IDeletingCallbackFacet>(ProfileEvent.Deleting);
        }

        [TestMethod]
        public void TestDecorateLoadedFacet() {
            TestDecorateCallbackFacet<ILoadedCallbackFacet>(ProfileEvent.Loaded);
        }

        [TestMethod]
        public void TestDecorateLoadingFacet() {
            TestDecorateCallbackFacet<ILoadingCallbackFacet>(ProfileEvent.Loading);
        }

        [TestMethod]
        public void TestDecoratePersistedFacet() {
            TestDecorateCallbackFacet<IPersistedCallbackFacet>(ProfileEvent.Persisted);
        }

        [TestMethod]
        public void TestDecoratePersistingFacet() {
            TestDecorateCallbackFacet<IPersistingCallbackFacet>(ProfileEvent.Persisting);
        }

        [TestMethod]
        public void TestDecorateUpdatedFacet() {
            TestDecorateCallbackFacet<IUpdatedCallbackFacet>(ProfileEvent.Updated);
        }

        [TestMethod]
        public void TestDecorateUpdatingFacet() {
            TestDecorateCallbackFacet<IUpdatingCallbackFacet>(ProfileEvent.Updating);
        }

        [TestMethod]
        public void TestDecorateAllFacets() {
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());
            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent> {
                ProfileEvent.ActionInvocation,
                ProfileEvent.PropertySet,
                ProfileEvent.Created,
                ProfileEvent.Deleted,
                ProfileEvent.Deleting,
                ProfileEvent.Loaded,
                ProfileEvent.Loading,
                ProfileEvent.Persisted,
                ProfileEvent.Persisting,
                ProfileEvent.Updated,
                ProfileEvent.Updating
            });

            var manager = new ProfileManager(config.Object);

            TestDecorated<IActionInvocationFacet, ProfileActionInvocationFacet>(manager);
            TestDecorated<IPropertySetterFacet, ProfilePropertySetterFacet>(manager);
            TestDecorated<ICreatedCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IDeletedCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IDeletingCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<ILoadedCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<ILoadingCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IPersistedCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IPersistingCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IUpdatedCallbackFacet, ProfileCallbackFacet>(manager);
            TestDecorated<IUpdatingCallbackFacet, ProfileCallbackFacet>(manager);
        }

        [TestMethod]
        public void TestDecorateNoFacets() {
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());
            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent>());

            var manager = new ProfileManager(config.Object);

            TestNotDecorated<IActionInvocationFacet, ProfileActionInvocationFacet>(manager);
            TestNotDecorated<IPropertySetterFacet, ProfilePropertySetterFacet>(manager);
            TestNotDecorated<ICreatedCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IDeletedCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IDeletingCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<ILoadedCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<ILoadingCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IPersistedCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IPersistingCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IUpdatedCallbackFacet, ProfileCallbackFacet>(manager);
            TestNotDecorated<IUpdatingCallbackFacet, ProfileCallbackFacet>(manager);
        }
    }
}