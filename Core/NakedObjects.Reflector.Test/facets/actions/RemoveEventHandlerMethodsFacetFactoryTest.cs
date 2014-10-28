// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Configuration;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class RemoveEventHandlerMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            var classStrategy = new DefaultClassStrategy();
            var cache = new ImmutableInMemorySpecCache();
            var metamodel = new Reflect.Metamodel(classStrategy, cache);
            var config = new ReflectorConfiguration(new Type[] { }, new Type[] { }, new Type[] { }, new Type[] { });
            var servicesConfig = new ServicesConfiguration();
            var reflector = new Reflect.Reflector(classStrategy, new FacetFactorySet(), new FacetDecoratorSet(), metamodel, config, servicesConfig);
            facetFactory = new RemoveEventHandlerMethodsFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private RemoveEventHandlerMethodsFacetFactory facetFactory;

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

        #region TestClass

#pragma warning disable 67
        // ReSharper disable EventNeverSubscribedTo.Local
        private class Customer {
            public event EventHandler AnEventHandler;
        }

        // ReSharper restore EventNeverSubscribedTo.Local
#pragma warning restore 67

        #endregion

        [Test]
        public void TestActionWithNoParameters() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);

            AssertRemovedCalled(2);

            EventInfo eInfo = typeof (Customer).GetEvent("AnEventHandler");

            var eventMethods = new[] {eInfo.GetAddMethod(), eInfo.GetRemoveMethod()};

            foreach (MethodInfo removedMethod in eventMethods) {
                AssertMethodRemoved(removedMethod);
            }
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.ActionParameter));
        }
    }
}