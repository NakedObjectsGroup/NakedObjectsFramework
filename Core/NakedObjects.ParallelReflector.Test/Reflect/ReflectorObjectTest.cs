// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.ParallelReflector.Component;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.ParallelReflect.Test {
    public class TestDomainObject {
        public void Action(DateTime? test) { }
    }

    [TestClass]
    public class ReflectorObjectTest : AbstractReflectorTest {
        protected override (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(AbstractParallelReflector reflector) {
            var objectReflector = reflector as ObjectReflector;
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
            (_, metamodel) = reflector.LoadSpecification(typeof(TestDomainObject), metamodel);
            return reflector.IntrospectSpecification(typeof(TestDomainObject), metamodel, () => new ObjectIntrospector(reflector, null));
        }

        [TestMethod]
        public void TestCollectionFacet() {
            var facet = Specification.GetFacet(typeof(ICollectionFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestDescriptionFaced() {
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestFacets() {
            Assert.AreEqual(22, Specification.FacetTypes.Length);
        }

        [TestMethod]
        public void TestName() {
            Assert.AreEqual(typeof(TestDomainObject).FullName, Specification.FullName);
        }

        [TestMethod]
        public void TestNamedFaced() {
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestNoCollectionFacet() {
            var facet = Specification.GetFacet(typeof(ICollectionFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestNoTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestPluralFaced() {
            var facet = Specification.GetFacet(typeof(IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestType() {
            Assert.IsTrue(Specification.IsObject);
        }

        [TestMethod]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}