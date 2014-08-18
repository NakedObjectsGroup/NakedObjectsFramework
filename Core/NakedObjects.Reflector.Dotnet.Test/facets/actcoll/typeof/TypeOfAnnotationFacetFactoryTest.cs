// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof {
    [TestFixture]
    public class TypeOfAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private TypeOfAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (ITypeOfFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypeOfAnnotationFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }


        //[Test]
        //public void TestTypeOfFacetInferredForActionWithCollectionReturnType() {
        //    MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
        //    facetFactory.Process(actionMethod, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}

        //[Test]
        //public void TestTypeOfFacetInferredForPropertyWithCollectionReturnType() {
        //    PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
        //    facetFactory.Process(property, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}

        [Test]
        public void TestTypeOfFacetInferredForActionWithGenericCollectionReturnType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypeOfFacetInferredFromGenerics);
            TypeOfFacetInferredFromGenerics typeOfFacetInferredFromGenerics = (TypeOfFacetInferredFromGenerics) facet;
            Assert.IsTrue(typeOfFacetInferredFromGenerics.IsInferred);
            Assert.AreEqual(typeof (Order), typeOfFacetInferredFromGenerics.Value);
        }

        [Test]
        public void TestTypeOfFacetInferredForCollectionWithGenericCollectionReturnType() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypeOfFacetInferredFromGenerics);
            TypeOfFacetInferredFromGenerics typeOfFacetInferredFromGenerics = (TypeOfFacetInferredFromGenerics) facet;
            Assert.IsTrue(typeOfFacetInferredFromGenerics.IsInferred);
            Assert.AreEqual(typeof (Order), typeOfFacetInferredFromGenerics.Value);
        }

        //[Test]
        //public void TestTypeOfFacetInferredForActionArrayListReturnType() {
        //    MethodInfo actionMethod = FindMethod(typeof (Customer5), "SomeAction");
        //    facetFactory.Process(actionMethod, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}

        //[Test]
        //public void TestTypeOfFacetInferredForCollectionWithArrayListReturnType() {
        //    PropertyInfo property = FindProperty(typeof (Customer6), "Orders");
        //    facetFactory.Process(property, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}

        //[Test]
        //public void TestTypeOfFacetInferredForActionWithSetReturnType() {
        //    MethodInfo actionMethod = FindMethod(typeof (Customer7), "SomeAction");
        //    facetFactory.Process(actionMethod, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}

        //[Test]
        //public void TestTypeOfFacetInferredForCollectionWithSetReturnType() {
        //    PropertyInfo property = FindProperty(typeof (Customer8), "Orders");
        //    facetFactory.Process(property, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TypeOfFacetViaAnnotation);
        //    TypeOfFacetViaAnnotation typeOfFacetViaAnnotation = (TypeOfFacetViaAnnotation) facet;
        //    Assert.AreEqual(typeof (Order), typeOfFacetViaAnnotation.Value);
        //    AssertNoMethodsRemoved();
        //}


        [Test]
        public void TestTypeOfFacetInferredForActionWithArrayReturnType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer9), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypeOfFacetInferredFromArray);
            TypeOfFacetInferredFromArray typeOfFacetInferredFromArray = (TypeOfFacetInferredFromArray) facet;
            Assert.AreEqual(typeof (Order), typeOfFacetInferredFromArray.Value);
            AssertNoMethodsRemoved();
        }


        [Test]
        public void TestTypeOfFacetIsInferredForCollectionFromOrderArray() {
            PropertyInfo property = FindProperty(typeof (Customer10), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypeOfFacetInferredFromArray);
            TypeOfFacetInferredFromArray typeOfFacetInferredFromArray = (TypeOfFacetInferredFromArray) facet;
            Assert.IsTrue(typeOfFacetInferredFromArray.IsInferred);
            Assert.AreEqual(typeof (Order), typeOfFacetInferredFromArray.Value);
        }

        //[Test]
        //public void TestTypeOfAnnotationIgnoredForActionIfReturnTypeIsntACollectionType() {
        //    MethodInfo actionMethod = FindMethod(typeof (Customer11), "SomeAction");
        //    facetFactory.Process(actionMethod, methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNull(facet);
        //    AssertNoMethodsRemoved();
        //}

     

        #region Nested Type: Customer10

        private class Customer10 {
            public Order[] Orders {
                get { return null; }
            }
        }

        #endregion

     

      

        #region Nested Type: Customer3

        private class Customer3 {
            public IList<Order> SomeAction() {
                return null;
            }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public IList<Order> Orders {
                get { return null; }
            }
        }

        #endregion

     

      

      

   

        #region Nested Type: Customer9

        private class Customer9 {
            public Order[] SomeAction() {
                return null;
            }
        }

        #endregion

        #region Nested Type: Order

        private class Order {}

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}