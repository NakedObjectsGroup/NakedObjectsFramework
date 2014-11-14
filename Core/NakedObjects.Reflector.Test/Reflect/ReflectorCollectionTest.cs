// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test {
    [TestFixture]
    public class ReflectorCollectionTest : AbstractReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return reflector.LoadSpecification(typeof (ArrayList));
        }

        [Test]
        public void TestCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNotNull(facet);
            //Assert.AreEqual(typeof(ArrayList).FullName, facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestElementTypeFacet() {
            var facet = (IElementTypeFacet) Specification.GetFacet(typeof (IElementTypeFacet));
            Assert.IsNotNull(facet);
            Assert.AreEqual(typeof (object), facet.Value);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(20, Specification.FacetTypes.Length);
        }

        [Test]
        public void TestName() {
            Assert.AreEqual(typeof (ArrayList).FullName, Specification.FullName);
        }


        [Test]
        public void TestNamedFaced() {
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = Specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestType() {
            Assert.IsTrue(Specification.IsCollection);
        }

        [Test]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}