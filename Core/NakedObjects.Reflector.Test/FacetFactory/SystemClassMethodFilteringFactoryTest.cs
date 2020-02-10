// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect.Component;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    // ReSharper disable UnusedMember.Local
    public class SystemClassMethodFilteringFactoryTest : AbstractFacetFactoryTest {
        private SystemClassMethodFilteringFactory facetFactory;

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
        public void TestFilterActions() {
            ReflectorConfiguration.NoValidate = true;

            var config = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new[] {typeof(Customer).Namespace});

            var classStrategy = new DefaultClassStrategy(config);

            var methods = typeof(Customer).GetMethods().ToList();
            var filteredActions = methods.Where(m => facetFactory.Filters(m, classStrategy)).ToArray();
            var notFilteredActions = methods.Where(m => !facetFactory.Filters(m, classStrategy)).ToArray();

            var filteredNames = new List<string> {
                "ToString",
                "Equals",
                "GetHashCode",
                "GetType",
            };

            var notFilteredNames = new List<string> {
                "ActionWithNoParameters",
                "ActionWithOneGoodParameter",
                "ActionWithTwoGoodParameter",
                "ActionWithOneBadParameter",
                "ActionWithOneGoodOneBadParameter",
                "ActionWithGenericParameter",
                "ActionWithNullableParameter",
                "ActionWithDictionaryParameter"
            };

            Assert.AreEqual(notFilteredNames.Count, notFilteredActions.Count());
            notFilteredNames.ForEach(n => Assert.IsTrue(notFilteredActions.Select(a => a.Name).Contains(n)));

            Assert.AreEqual(filteredNames.Count, filteredActions.Count());
            filteredNames.ForEach(n => Assert.IsTrue(filteredActions.Select(a => a.Name).Contains(n)));
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        // ReSharper disable UnusedParameter.Local
        private class Customer {
            public void ActionWithNoParameters() { }
            public void ActionWithOneGoodParameter(int i) { }
            public void ActionWithTwoGoodParameter(int i, Customer c) { }

            public void ActionWithOneBadParameter(out int c) {
                c = 0;
            }

            public void ActionWithOneGoodOneBadParameter(int i, ref int j) { }
            public void ActionWithGenericParameter(Predicate<int> p) { }
            public void ActionWithNullableParameter(int? i) { }
            public void ActionWithDictionaryParameter(string path, Dictionary<string, object> answers) { }
        }

        // ReSharper restore UnusedParameter.Local

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();

            var cache = new ImmutableInMemorySpecCache();
            ReflectorConfiguration.NoValidate = true;

            var config = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new[] {typeof(Customer).Namespace});
            var menuFactory = new NullMenuFactory();

            facetFactory = new SystemClassMethodFilteringFactory(0);
            var classStrategy = new DefaultClassStrategy(config);
            var metamodel = new Metamodel(classStrategy, cache);

            Reflector = new Reflector(classStrategy, metamodel, config, menuFactory, new IFacetDecorator[] { }, new IFacetFactory[] {facetFactory});
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // ReSharper restore UnusedMember.Local
}