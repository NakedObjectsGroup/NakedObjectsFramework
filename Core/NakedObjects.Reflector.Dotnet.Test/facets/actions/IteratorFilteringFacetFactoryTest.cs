// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class IteratorFilteringFacetFactoryTest : AbstractFacetFactoryTest {
        private IteratorFilteringFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new IteratorFilteringFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
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

        [Test]
        public void TestRequestsRemoverToRemoveIteratorMethods() {
            MethodInfo enumeratorMethod = FindMethod(typeof (Customer), "GetEnumerator");
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(enumeratorMethod));
        }

        [Test]
        public void TestIterableIteratorMethodFiltered() {
            MethodInfo enumeratorMethod = FindMethod(typeof (Customer), "GetEnumerator");
            //Assert.IsTrue(facetFactory.Recognizes(enumeratorMethod));
        }

        [Test]
        public void TestNoIteratorMethodFiltered() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "someAction");
           // Assert.IsFalse(facetFactory.Recognizes(actionMethod));
        }

        #region Nested Type: Customer

        private class Customer : IEnumerable {
            #region IEnumerable Members

            public IEnumerator GetEnumerator() {
                return null;
            }

            #endregion

            public void someAction() {}
        }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            public void someAction() {}
        }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}