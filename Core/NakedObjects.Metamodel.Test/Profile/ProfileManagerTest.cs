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

namespace NakedObjects.Meta.Test.Profile {
    [TestClass]
    public class ProfileManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() {}

        #endregion

        [TestMethod]
        public void TestCreateOk() {
            var config = new ProfileConfiguration<IProfiler>();

            // ReSharper disable once UnusedVariable
            var sink = new ProfileManager(config);
        }

        [TestMethod]
        public void TestCreateWrongDefaultProfilerType() {
            var config = new Mock<IProfileConfiguration>();

            config.Setup(c => c.Profiler).Returns(typeof (object));

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
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());

            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent>() {ProfileEvent.ActionInvocation});

            var manager = new ProfileManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IActionInvocationFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof (IActionInvocationFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (ProfileActionInvocationFacet));
        }

        [TestMethod]
        public void TestDecorateUpdatedFacet() {
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());

            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent>() { ProfileEvent.Updated });


            var manager = new ProfileManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IUpdatedCallbackFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof (IUpdatedCallbackFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (ProfileCallbackFacet));
        }

        [TestMethod]
        public void TestDecoratePersistedFacet() {
            var config = new Mock<IProfileConfiguration>();
            var auditor = new Mock<IProfiler>();

            config.Setup(c => c.Profiler).Returns(auditor.Object.GetType());

            config.Setup(c => c.EventsToProfile).Returns(new HashSet<ProfileEvent>() { ProfileEvent.Persisted });


            var manager = new ProfileManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IPersistedCallbackFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof (IPersistedCallbackFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (ProfileCallbackFacet));
        }
    }
}