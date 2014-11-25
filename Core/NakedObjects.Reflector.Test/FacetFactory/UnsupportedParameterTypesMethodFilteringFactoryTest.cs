// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class UnsupportedParameterTypesMethodFilteringFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            var classStrategy = new DefaultClassStrategy();
            var cache = new ImmutableInMemorySpecCache();
            var metamodel = new Metamodel(classStrategy, cache);
            var config = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new Type[] {}, new Type[] {});
            var servicesConfig = new ServicesConfiguration();
            var menuDefinition = new ReflectorTest.NullMenuDefinition();
            var menuFactory = new NullMenuFactory();

            facetFactory = new UnsupportedParameterTypesMethodFilteringFactory(0);

            Reflector = new Reflector(classStrategy, metamodel, config, servicesConfig, menuDefinition, menuFactory, new IFacetDecorator[] {}, new IFacetFactory[]{facetFactory});

        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private UnsupportedParameterTypesMethodFilteringFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof (INamedFacet),
                    typeof (IExecutedFacet),
                    typeof (IActionValidationFacet),
                    typeof (IActionInvocationFacet),
                    typeof (IActionDefaultsFacet),
                    typeof (IActionChoicesFacet),
                    typeof (IDescribedAsFacet),
                    typeof (IMandatoryFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }


        private class Customer {
            public void ActionWithNoParameters() {}
            public void ActionWithOneGoodParameter(int i) {}
            public void ActionWithTwoGoodParameter(int i, Customer c) {}

            public void ActionWithOneBadParameter(out int c) {
                c = 0;
            }

            public void ActionWithOneGoodOneBadParameter(int i, ref int j) {}
            public void ActionWithGenericParameter(Predicate<int> p) {}
            public void ActionWithNullableParameter(int? i) {}
            public void ActionWithDictionaryParameter(string path, Dictionary<string, object> answers) {}
        }

        [Test]
        public void TestActionWithDictionaryParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithDictionaryParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }

        [Test]
        public void TestActionWithGenericParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithGenericParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }


        [Test]
        public void TestActionWithNoParameters() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithNoParameters");
            Assert.IsFalse(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }

        [Test]
        public void TestActionWithNullableParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithNullableParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }


        [Test]
        public void TestActionWithOneBadParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneBadParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }

        [Test]
        public void TestActionWithOneGoodOneBadParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneGoodOneBadParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }

        [Test]
        public void TestActionWithOneGoodParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneGoodParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }


        [Test]
        public void TestActionWithTwoGoodParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithTwoGoodParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod, Reflector.ClassStrategy));
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }
    }
}