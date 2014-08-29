// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NUnit.Framework;
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
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class RemoveEventHandlerMethodsFacetFactoryTest : AbstractFacetFactoryTest {
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


        private class Customer {
#pragma warning disable 67
            public event EventHandler AnEventHandler;
#pragma warning restore 67
        }

        [Test]
        public void TestActionWithNoParameters() {
            //facetFactory.Process(typeof (Customer), methodRemover, facetHolder);

            //Assert.AreEqual(3, methodRemover.GetRemoveMethodMethodCalls().Count);

            //IList<MethodInfo> removedMethods = methodRemover.GetRemoveMethodMethodCalls();

            //EventInfo eInfo = typeof (Customer).GetEvent("AnEventHandler");

            //var eventMethods = new[] {eInfo.GetAddMethod(), eInfo.GetRaiseMethod(), eInfo.GetRemoveMethod()};

            //Assert.AreEqual(removedMethods.Count(), eventMethods.Count());

            //foreach (MethodInfo removedMethod in removedMethods) {
            //    Assert.IsTrue(eventMethods.Contains(removedMethod));
            //}
            Assert.Fail(); // fix this 
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