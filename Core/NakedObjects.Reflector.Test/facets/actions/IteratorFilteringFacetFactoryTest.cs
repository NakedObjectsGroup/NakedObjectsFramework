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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new IteratorFilteringFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private IteratorFilteringFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        // ReSharper disable UnusedMember.Local
        // ReSharper disable InconsistentNaming
        // ReSharper disable AssignNullToNotNullAttribute
        private class Customer : IEnumerable {
            #region IEnumerable Members

            public IEnumerator GetEnumerator() {
                return null;
            }

            #endregion

            public void someAction() {}
        }

        private class Customer1 {
            public void someAction() {}
        }

        // ReSharper restore AssignNullToNotNullAttribute
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestRequestsRemoverToRemoveIteratorMethods() {
            MethodInfo enumeratorMethod = FindMethod(typeof (Customer), "GetEnumerator");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            AssertMethodRemoved(enumeratorMethod);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}