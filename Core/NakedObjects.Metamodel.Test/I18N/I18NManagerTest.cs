// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.I18N;

namespace NakedObjects.Meta.Test.I18N {
    [TestClass]
    public class I18NManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() {}

        #endregion

        [TestMethod]
        public void TestDecoratePropertyName() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "_property_name"))).Returns("I18N property name");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N property name", ((NamedFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestDecorateActionName() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(false);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "_action_name"))).Returns("I18N action name");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N action name", ((NamedFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestDecorateParameterName() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<IActionParameterSpec>();
            var identifier = new Mock<IIdentifier>();
            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "action_parameter1_name"))).Returns("I18N parameter name");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N parameter name", ((NamedFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestDecoratePropertyDescription() {
            var manager = new I18NManager();

            var testDescribed = new Mock<IDescribedAsFacet>();
            testDescribed.Setup(n => n.FacetType).Returns(typeof (IDescribedAsFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "_property_description"))).Returns("I18N property description");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testDescribed.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (DescribedAsFacetI18N));
            Assert.AreEqual("I18N property description", ((DescribedAsFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestDecorateActionDescription() {
            var manager = new I18NManager();

            var testDescribed = new Mock<IDescribedAsFacet>();
            testDescribed.Setup(n => n.FacetType).Returns(typeof (IDescribedAsFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(false);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "_action_description"))).Returns("I18N action description");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testDescribed.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (DescribedAsFacetI18N));
            Assert.AreEqual("I18N action description", ((DescribedAsFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestDecorateParameterDescription() {
            var manager = new I18NManager();

            var testDescribed = new Mock<IDescribedAsFacet>();
            testDescribed.Setup(n => n.FacetType).Returns(typeof (IDescribedAsFacet));

            var testHolder = new Mock<IActionParameterSpec>();
            var identifier = new Mock<IIdentifier>();
            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "action_parameter1_description"))).Returns("I18N parameter description");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testDescribed.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (DescribedAsFacetI18N));
            Assert.AreEqual("I18N parameter description", ((DescribedAsFacetI18N) facet).Value);
        }

        [TestMethod]
        public void TestMissingResource() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);

            var testResources = new Mock<ResourceManager>();

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestSystemOrNakedObjectsResource() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);
            identifier.Setup(i => i.ToIdentityString(It.IsAny<IdentifierDepth>())).Returns("System.");

            var testResources = new Mock<ResourceManager>();

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsNull(facet);

            identifier.Setup(i => i.ToIdentityString(It.IsAny<IdentifierDepth>())).Returns("NakedObjects.");

            facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestMissingResourceException() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.IsAny<string>())).Throws<MissingManifestResourceException>();

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestFacetTypes() {
            var manager = new I18NManager();

            var facets = manager.ForFacetTypes;

            Assert.IsTrue(facets.SequenceEqual(new[] {typeof (INamedFacet), typeof (IDescribedAsFacet)}));
        }

        [TestMethod]
        public void TestUndecoratedFacet() {
            var manager = new I18NManager();

            var testTitle = new Mock<ITitleFacet>();
            testTitle.Setup(n => n.FacetType).Returns(typeof (ITitleFacet));

            var testHolder = new Mock<ISpecification>();

            var facet = manager.Decorate(testTitle.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (ITitleFacet));
            Assert.AreEqual(testTitle.Object, facet);
        }

        [TestMethod]
        public void TestCaching() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<ISpecification>();
            var identifier = new Mock<IIdentifier>();

            identifier.Setup(i => i.IsField).Returns(true);

            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "_property_name"))).Returns("I18N property name");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N property name", ((NamedFacetI18N) facet).Value);

            facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N property name", ((NamedFacetI18N) facet).Value);
        }
    }
}