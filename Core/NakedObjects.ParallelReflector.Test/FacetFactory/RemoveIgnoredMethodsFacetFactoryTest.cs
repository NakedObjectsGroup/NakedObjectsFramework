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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Component;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class RemoveIgnoredMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private RemoveIgnoredMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof(INamedFacet),
                    typeof(IExecutedFacet),
                    typeof(IActionValidationFacet),
                    typeof(IActionInvocationFacet),
                    typeof(IActionDefaultsFacet),
                    typeof(IActionChoicesFacet),
                    typeof(IDescribedAsFacet),
                    typeof(IMandatoryFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public void TestMethodsMarkedIgnoredAreRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer1), MethodRemover, Specification, metamodel);
            AssertRemovedCalled(2);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNakedObjectsTypeReflectOverAll() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer2), MethodRemover, Specification, metamodel);
            AssertRemovedCalled(0);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod, Ignore] //Pending re-instatement of NakedObjectsType & Include attributes (PM 7.2)
        public void TestNakedObjectsTypeReflectOverTypeOnly() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer3), MethodRemover, Specification, metamodel);
            AssertRemovedCalled(7); //That's 3 from the class, and 4 inherited from objec (e.g. ToString())
            Assert.IsNotNull(metamodel);
        }

        [TestMethod, Ignore] //Pending re-instatement of NakedObjectsType & Include attributes (PM 7.2)
        public void TestNakedObjectsTypeReflectOverNone() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            try {
                metamodel = facetFactory.Process(Reflector, typeof(Customer4), MethodRemover, Specification, metamodel);
                Assert.Fail("Shouldn't get to here!");
            }
            catch (Exception e) {
                Assert.AreEqual("Attempting to introspect a class that has been marked with NakedObjectsType with ReflectOver.None", e.Message);
            }

            Assert.IsNotNull(metamodel);
        }

        [TestMethod, Ignore] //Pending re-instatement of NakedObjectsType & Include attributes (PM 7.2)
        public void TestNakedObjectsTypeReflectOverExplicitlyIncludedMembersOnly() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer5), MethodRemover, Specification, metamodel);
            AssertRemovedCalled(6); //That's 2 from the class, and 4 inherited from objec (e.g. ToString())
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region TestClasses

        private class Customer1 {
            [NakedObjectsIgnore]
            public void Method1() { }

            [NakedObjectsIgnore]
            public void Method2() { }

            public void Method3() { }
        }

        // [NakedObjectsType(ReflectOver.All)]
        private class Customer2 {
            public void Method1() { }

            public void Method2() { }

            public void Method3() { }
        }

        //[NakedObjectsType(ReflectOver.TypeOnlyNoMembers)]
        private class Customer3 {
            [NakedObjectsIgnore] //Irrelevant!
            public void Method1() { }

            public void Method2() { }

            public void Method3() { }
        }

        //[NakedObjectsType(ReflectOver.None)]
        private class Customer4 {
            public void Method1() { }

            public void Method2() { }

            public void Method3() { }
        }

        //[NakedObjectsType(ReflectOver.ExplicitlyIncludedMembersOnly)]
        private class Customer5 {
            public void Method1() { }

            // [NakedObjectsInclude]
            public void Method2() { }

            public void Method3() { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            var cache = new ImmutableInMemorySpecCache();
            ReflectorConfiguration.NoValidate = true;

            var reflectorConfiguration = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { });
            facetFactory = new RemoveIgnoredMethodsFacetFactory(0);
            var menuFactory = new NullMenuFactory();
            var classStrategy = new DefaultClassStrategy(reflectorConfiguration);
            var metamodel = new Metamodel(classStrategy, cache);

            Reflector = new ParallelReflector(classStrategy, metamodel, reflectorConfiguration, menuFactory, new IFacetDecorator[] { }, new IFacetFactory[] {facetFactory});
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }
}