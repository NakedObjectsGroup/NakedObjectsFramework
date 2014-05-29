// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted {
    [TestFixture]
    public class NotPersistedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NotPersistedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INotPersistedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NotPersistedAnnotationFacetFactory { Reflector = reflector };
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
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

       [Test]
        public void TestNotPersistedAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacetAnnotation);
            AssertNoMethodsRemoved();
        }

       [Test]
        public void TestNotPersistedAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        #region Nested Type: Customer

        private class Customer {
            [NotPersisted]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [NotPersisted]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}