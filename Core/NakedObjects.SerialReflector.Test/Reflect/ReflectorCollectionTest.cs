// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.Component;

namespace NakedObjects.Reflect.Test {
    [TestClass]
    public class ReflectorCollectionTest : AbstractReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) => reflector.LoadSpecification<IObjectSpecImmutable>(typeof(ArrayList));

        [TestMethod]
        public void TestCollectionFacet() {
            var facet = Specification.GetFacet(typeof(ICollectionFacet));
            Assert.IsNotNull(facet);
            AssertIsInstanceOfType<CollectionFacet>(facet);
        }

        [TestMethod]
        public void TestDescriptionFaced() {
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            AssertIsInstanceOfType<DescribedAsFacetNone>(facet);
        }

        [TestMethod]
        public void TestElementTypeFacet() {
            var facet = (IElementTypeFacet) Specification.GetFacet(typeof(IElementTypeFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestFacets() {
            Assert.AreEqual(23, Specification.FacetTypes.Length);
        }

        [TestMethod]
        public void TestName() {
            Assert.AreEqual(typeof(ArrayList).FullName, Specification.FullName);
        }

        [TestMethod]
        public void TestNamedFaced() {
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            AssertIsInstanceOfType<NamedFacetInferred>(facet);
        }

        [TestMethod]
        public void TestPluralFaced() {
            var facet = Specification.GetFacet(typeof(IPluralFacet));
            Assert.IsNotNull(facet);
            AssertIsInstanceOfType<PluralFacetInferred>(facet);
        }

        [TestMethod]
        public void TestType() {
            Assert.IsTrue(Specification.IsCollection);
        }

        [TestMethod]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(facet);
            AssertIsInstanceOfType<TypeOfFacetDefaultToType>(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}