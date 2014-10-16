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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NotPersistedAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private NotPersistedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INotPersistedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [NotPersisted]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer1 {
            [NotPersisted]
            public IList Orders {
                get { return null; }
            }
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestNotPersistedAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNotPersistedAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacetAnnotation);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}