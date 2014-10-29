// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_ValueTest : AbstractDotNetReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return reflector.LoadSpecification(typeof (string));
        }

        [Test, Ignore]
        public void TestCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(24, Specification.FacetTypes.Length);
        }

        [Test]
        public void TestIsParseable() {
            Assert.IsTrue(Specification.IsParseable);
        }

        [Test]
        public void TestName() {
            Assert.AreEqual(typeof (string).FullName, Specification.FullName);
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

        [Test, Ignore]
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