// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Eagerly;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Eagerly {
    [TestFixture]
    public class EagerlyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            annotationFacetFactory = new EagerlyAnnotationFacetFactory(Metadata);
        }

        [TearDown]
        public override void TearDown() {
            annotationFacetFactory = null;
            base.TearDown();
        }

        #endregion

        private EagerlyAnnotationFacetFactory annotationFacetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IEagerlyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return annotationFacetFactory; }
        }

        private class Customer1 {
            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public int Prop { get; set; }

            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public IList<Customer1> Coll { get; set; }

            [Eagerly(EagerlyAttribute.Do.Rendering)]
            public IList<Customer1> Act() {
                return new List<Customer1>();
            }
        }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        private class Customer2 {
            public int Prop { get; set; }

            public IList<Customer1> Coll { get; set; }

            public IList<Customer1> Act() {
                return new List<Customer1>();
            }
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnClass() {
            annotationFacetFactory.Process(typeof (Customer2), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacetAnnotation);
            var propertyDefaultFacetAnnotation = (EagerlyFacetAnnotation) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Coll");
            annotationFacetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacetAnnotation);
            var propertyDefaultFacetAnnotation = (EagerlyFacetAnnotation) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnMethod() {
            MethodInfo method = FindMethod(typeof (Customer1), "Act");
            annotationFacetFactory.Process(method, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacetAnnotation);
            var propertyDefaultFacetAnnotation = (EagerlyFacetAnnotation) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Prop");
            annotationFacetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EagerlyFacetAnnotation);
            var propertyDefaultFacetAnnotation = (EagerlyFacetAnnotation) facet;
            Assert.AreEqual(EagerlyAttribute.Do.Rendering, propertyDefaultFacetAnnotation.What);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnClass() {
            annotationFacetFactory.Process(typeof (Customer1), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Coll");
            annotationFacetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnMethod() {
            MethodInfo method = FindMethod(typeof (Customer2), "Act");
            annotationFacetFactory.Process(method, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestEagerlyNotPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Prop");
            annotationFacetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEagerlyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = annotationFacetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}