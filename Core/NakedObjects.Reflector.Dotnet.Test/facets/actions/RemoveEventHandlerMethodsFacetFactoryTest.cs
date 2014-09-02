// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Facets.Actions.Validate;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class RemoveEventHandlerMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            var reflector = new DotNetReflector(new DefaultClassStrategy(), new FacetFactorySetImpl(), new FacetDecoratorSet());
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
            facetFactory.Process(typeof (Customer), MethodRemover, FacetHolder);

            AssertRemovedCalled(2);

            EventInfo eInfo = typeof (Customer).GetEvent("AnEventHandler");

            var eventMethods = new[] {eInfo.GetAddMethod(), eInfo.GetRemoveMethod()};

            foreach (MethodInfo removedMethod in eventMethods) {
                AssertMethodRemoved(removedMethod);
            }
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }
    }
}