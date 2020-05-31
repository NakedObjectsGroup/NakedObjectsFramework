// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Component;
using NakedObjects.Reflect.Component;
using NakedObjects.Reflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class RemoveEventHandlerMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private RemoveEventHandlerMethodsFacetFactory facetFactory;

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

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestActionWithNoParameters() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);

            AssertRemovedCalled(2);

            var eInfo = typeof(Customer).GetEvent("AnEventHandler");

            var eventMethods = new[] {eInfo.GetAddMethod(), eInfo.GetRemoveMethod()};

            foreach (var removedMethod in eventMethods) {
                AssertMethodRemoved(removedMethod);
            }
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

        #region Nested type: Customer

        #region TestClass

#pragma warning disable 67
        // ReSharper disable EventNeverSubscribedTo.Local
        private class Customer {
            public event EventHandler AnEventHandler;
        }

        // ReSharper restore EventNeverSubscribedTo.Local
#pragma warning restore 67

        #endregion

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            var cache = new ImmutableInMemorySpecCache();
            ReflectorConfiguration.NoValidate = true;

            var reflectorConfiguration = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new string[] { });
            facetFactory = new RemoveEventHandlerMethodsFacetFactory(0);
            var menuFactory = new NullMenuFactory();
            var classStrategy = new DefaultClassStrategy(reflectorConfiguration);
            var metamodel = new Metamodel(classStrategy, cache);

            Reflector = new Reflector(classStrategy, metamodel, reflectorConfiguration, menuFactory, new IFacetDecorator[] { }, new IFacetFactory[] {facetFactory});
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }
}