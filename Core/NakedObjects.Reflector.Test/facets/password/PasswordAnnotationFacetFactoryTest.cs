// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Password;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Password {
    [TestFixture]
    public class PasswordAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PasswordAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PasswordAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPasswordFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            [DataType(DataType.Password)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void someAction([DataType(DataType.Password)] string foo) {}
        }

        private class Customer3 {
            [DataType(DataType.PhoneNumber)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer4 {
            public void someAction([DataType(DataType.PhoneNumber)] string foo) {}
        }


        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestPasswordAnnotationNotPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestPasswordAnnotationNotPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestPasswordAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacetAnnotation);
        }

        [Test]
        public void TestPasswordAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacetAnnotation);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}