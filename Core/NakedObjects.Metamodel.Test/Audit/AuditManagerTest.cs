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
using NakedObjects.Audit;
using NakedObjects.Meta.Audit;

namespace NakedObjects.Meta.Test.Audit {
    [TestClass]
    public class AuditManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() { }

        #endregion

        [TestMethod]
        public void TestCreateOk() {
            IAuditConfiguration config = new AuditConfiguration<IAuditor>();

            config.AddNamespaceAuditor<IAuditor>("namespace");

            // ReSharper disable once UnusedVariable
            var sink = new AuditManager(config);
        }

        [TestMethod]
        public void TestCreateWrongDefaultAuditorType() {
            var config = new Mock<IAuditConfiguration>();

            config.Setup(c => c.DefaultAuditor).Returns(typeof(object));
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type>());

            try {
                // ReSharper disable once UnusedVariable
                var sink = new AuditManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("System.Object is not an IAuditor", expected.Message);
            }
        }

        [TestMethod]
        public void TestCreateWrongNamespaceAuditorType() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type> {{"", typeof(object)}});

            try {
                // ReSharper disable once UnusedVariable
                var sink = new AuditManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("System.Object is not an IAuditor", expected.Message);
            }
        }

        [TestMethod]
        public void TestDecorateActionInvocationFacet() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type> {{"", auditor.Object.GetType()}});

            var manager = new AuditManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IActionInvocationFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof(IActionInvocationFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof(AuditActionInvocationFacet));
        }

        [TestMethod]
        public void TestDecorateUpdatedFacet() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type> {{"", auditor.Object.GetType()}});

            var manager = new AuditManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IUpdatedCallbackFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof(IUpdatedCallbackFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof(AuditUpdatedFacet));
        }

        [TestMethod]
        public void TestDecoratePersistedFacet() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type> {{"", auditor.Object.GetType()}});

            var manager = new AuditManager(config.Object);

            var testSpec = new Mock<ISpecification>();
            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();
            var testFacet = new Mock<IPersistedCallbackFacet>();

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

            testFacet.Setup(n => n.FacetType).Returns(typeof(IPersistedCallbackFacet));

            testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

            var facet = manager.Decorate(testFacet.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof(AuditPersistedFacet));
        }
    }
}