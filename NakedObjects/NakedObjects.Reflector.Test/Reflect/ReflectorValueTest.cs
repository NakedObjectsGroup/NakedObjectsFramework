// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.ParallelReflector.Component;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Reflect;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Component;
using NakedObjects.ParallelReflector.Component;

namespace NakedObjects.Reflector.Test.Reflect {
    [TestClass]
    public class ReflectorValueTest : AbstractReflectorTest {
        protected override IReflector Reflector(Metamodel metamodel, ILoggerFactory lf) {
            var config = new CoreConfiguration();
            ClassStrategy = new SystemTypeClassStrategy(config);
            var systemTypeFacetFactorySet = new SystemTypeFacetFactorySet(FacetFactories.OfType<IObjectFacetFactoryProcessor>());
            var mockLogger1 = new Mock<ILogger<AbstractParallelReflector>>().Object;
            return new SystemTypeReflector(systemTypeFacetFactorySet, (SystemTypeClassStrategy) ClassStrategy, metamodel, config, System.Array.Empty<IFacetDecorator>(), lf, mockLogger1);
        }

        protected override (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(IReflector reflector) {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
            (_, metamodel) = reflector.LoadSpecification(typeof(IEnumerable<char>), metamodel);
            (_, metamodel) = reflector.LoadSpecification(typeof(string), metamodel);

            (_, metamodel) = ((AbstractParallelReflector) reflector).IntrospectSpecification(typeof(IEnumerable<char>), metamodel);
            return ((AbstractParallelReflector) reflector).IntrospectSpecification(typeof(string), metamodel);
        }

        [TestMethod]
        public void TestCollectionFacet() {
            var facet = Specification.GetFacet(typeof(ICollectionFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestDescriptionFaced() {
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestFacets() {
            Assert.AreEqual(28, Specification.FacetTypes.Length);
        }

        [TestMethod]
        public void TestIsParseable() {
            Assert.IsTrue(Specification.IsParseable);
        }

        [TestMethod]
        public void TestName() {
            Assert.AreEqual(typeof(string).FullName, Specification.FullName);
        }

        [TestMethod]
        public void TestNamedFaced() {
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestPluralFaced() {
            var facet = Specification.GetFacet(typeof(IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestType() {
            Assert.IsTrue(Specification.IsCollection);
        }

        [TestMethod]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}