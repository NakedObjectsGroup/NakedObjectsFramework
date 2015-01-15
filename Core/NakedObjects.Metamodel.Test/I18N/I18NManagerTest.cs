// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
        public void TestDecorateName() {
            var manager = new I18NManager();

            var testName = new Mock<INamedFacet>();
            testName.Setup(n => n.FacetType).Returns(typeof (INamedFacet));

            var testHolder = new Mock<IActionParameterSpec>();
            var identifier = new Mock<IIdentifier>();
            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "action_parameter1_name"))).Returns("I18N name");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testName.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof (NamedFacetI18N));
            Assert.AreEqual("I18N name", ((NamedFacetI18N)facet).Value);
        }

        [TestMethod]
        public void TestDecorateDescription() {
            var manager = new I18NManager();

            var testDescribed = new Mock<IDescribedAsFacet>();
            testDescribed.Setup(n => n.FacetType).Returns(typeof(IDescribedAsFacet));

            var testHolder = new Mock<IActionParameterSpec>();
            var identifier = new Mock<IIdentifier>();
            var testResources = new Mock<ResourceManager>();

            testResources.Setup(r => r.GetString(It.Is<string>(s => s == "action_parameter1_description"))).Returns("I18N description");

            manager.Resources = testResources.Object;

            testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

            var facet = manager.Decorate(testDescribed.Object, testHolder.Object);

            Assert.IsInstanceOfType(facet, typeof(DescribedAsFacetI18N));
            Assert.AreEqual("I18N description", ((DescribedAsFacetI18N)facet).Value);
        }
    }
}