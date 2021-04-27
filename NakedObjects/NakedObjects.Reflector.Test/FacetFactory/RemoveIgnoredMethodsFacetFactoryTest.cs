// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Component;
using NakedFramework.ParallelReflector.Component;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Reflect;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class RemoveIgnoredMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private RemoveIgnoredMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes =>
            new[] {
                typeof(INamedFacet),
                typeof(IActionValidationFacet),
                typeof(IActionInvocationFacet),
                typeof(IActionDefaultsFacet),
                typeof(IActionChoicesFacet),
                typeof(IDescribedAsFacet),
                typeof(IMandatoryFacet)
            };

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestMethodsMarkedIgnoredAreRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer1), MethodRemover, Specification, metamodel);
            AssertRemovedCalled(2);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer1

        #region TestClasses

        private class Customer1 {
            [NakedObjectsIgnore]
            public void Method1() { }

            [NakedObjectsIgnore]
            public void Method2() { }

            public void Method3() { }
        }

        #endregion

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            var cache = new ImmutableInMemorySpecCache();
            ObjectReflectorConfiguration.NoValidate = true;

            var reflectorConfiguration = new ObjectReflectorConfiguration(Array.Empty<Type>(), Array.Empty<Type>());
            facetFactory = new RemoveIgnoredMethodsFacetFactory(GetOrder<RemoveIgnoredMethodsFacetFactory>(), LoggerFactory);
            var objectFactFactorySet = new ObjectFacetFactorySet(new IObjectFacetFactoryProcessor[] {facetFactory});
            var classStrategy = new ObjectClassStrategy(reflectorConfiguration);
            var metamodel = new MetamodelHolder(cache, null);
            var mockLogger = new Mock<ILogger<AbstractParallelReflector>>().Object;
            var mockLoggerFactory = new Mock<ILoggerFactory>().Object;
            var order = new ObjectReflectorOrder<ObjectReflector>();
            Reflector = new ObjectReflector(objectFactFactorySet, classStrategy, reflectorConfiguration, Array.Empty<IFacetDecorator>(), order, mockLoggerFactory, mockLogger);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }
}