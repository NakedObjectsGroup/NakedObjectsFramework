// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
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
    public class UnsupportedParameterTypesMethodFilteringFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            Reflector = new DotNetReflector(new DefaultClassStrategy(), new FacetFactorySetImpl(), new FacetDecoratorSet());

            facetFactory = new UnsupportedParameterTypesMethodFilteringFactory(Reflector);
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
            Assert.IsTrue(facetFactory.Filters(actionMethod));
        }

        [Test]
        public void TestActionWithGenericParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithGenericParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod));
        }


        [Test]
        public void TestActionWithNoParameters() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithNoParameters");
            Assert.IsFalse(facetFactory.Filters(actionMethod));
        }

        [Test]
        public void TestActionWithNullableParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithNullableParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod));
        }


        [Test]
        public void TestActionWithOneBadParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneBadParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod));
        }

        [Test]
        public void TestActionWithOneGoodOneBadParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneGoodOneBadParameter");
            Assert.IsTrue(facetFactory.Filters(actionMethod));
        }

        [Test]
        public void TestActionWithOneGoodParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithOneGoodParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod));
        }


        [Test]
        public void TestActionWithTwoGoodParameter() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer), "ActionWithTwoGoodParameter");
            Assert.IsFalse(facetFactory.Filters(actionMethod));
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }
    }
}