// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;
using System.ComponentModel;
using NakedObjects.Core.Context;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.Named {
    [TestFixture]
    public class NamedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NamedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (INamedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NamedAnnotationFacetFactory(reflector);
            //BasicConfigurator.Configure(new WarningAppender());
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
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        #region named tests

        [Test]
        public void TestNamedAnnotationPickedUpOnClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "someAction");
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationOnPropertyMarksDuplicate() {
            PropertyInfo property = FindProperty(typeof(Customer11), "NumberOfOrders");
            PropertyInfo property1 = FindProperty(typeof(Customer11), "NumberOfOrders1");
            var facetHolder1 = new FacetHolderImpl();

            facetFactory.Process(property, methodRemover, facetHolder);
            facetFactory.Process(property1, methodRemover, facetHolder1);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

           // Assert.Contains("Duplicate name: some name found on type: NakedObjects.Reflector.DotNet.Facets.Naming.Named.NamedAnnotationFacetFactoryTest+Customer11", NakedObjectsContext.InitialisationWarnings);
        }


        [Test]
        public void TestNamedAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test] 
        public void ATestNamedAnnotationOnActionIgnoresDuplicate() { // these need to run before logs are added by other tests 
            MethodInfo actionMethod = FindMethod(typeof(Customer13), "SomeAction");
            MethodInfo actionMethod1 = FindMethod(typeof(Customer13), "SomeAction1");
            var facetHolder1 = new FacetHolderImpl();


            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(actionMethod1, methodRemover, facetHolder1);
            facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            Assert.IsEmpty(NakedObjectsContext.InitialisationWarnings);
        }


        [Test]
        public void TestNamedAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction", new Type[] {typeof (int)});
            facetFactory.ProcessParams(actionMethod, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
        }

        #endregion

        #region displayname tests

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnClass() {
            MethodInfo actionMethod = FindMethod(typeof(Customer5), "someAction");
            facetFactory.Process(typeof(Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof(Customer6), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationOnPropertyMarksDuplicate() {
            PropertyInfo property = FindProperty(typeof(Customer16), "NumberOfOrders");
            PropertyInfo property1 = FindProperty(typeof(Customer16), "NumberOfOrders1");
            var facetHolder1 = new FacetHolderImpl();

            facetFactory.Process(property, methodRemover, facetHolder);
            facetFactory.Process(property1, methodRemover, facetHolder1);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            //Assert.Contains("Duplicate name: some name found on type: NakedObjects.Reflector.DotNet.Facets.Naming.Named.NamedAnnotationFacetFactoryTest+Customer16", NakedObjectsContext.InitialisationWarnings);
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof(Customer7), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void ATestDisplayNameAnnotationOnActionIgnoresDuplicate() { // these need to run before logs are added by other tests 
            MethodInfo actionMethod = FindMethod(typeof(Customer18), "SomeAction");
            MethodInfo actionMethod1 = FindMethod(typeof(Customer18), "SomeAction1");
            var facetHolder1 = new FacetHolderImpl();


            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(actionMethod1, methodRemover, facetHolder1);
            facet = facetHolder.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract)facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            Assert.IsEmpty(NakedObjectsContext.InitialisationWarnings);
        }

        #endregion

        #region named test data 

        #region Nested Type: Customer

        [Named("some name")]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [Named("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer11

        private class Customer11 {
            [Named("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }

            [Named("some name")]
            public int NumberOfOrders1 {
                get { return 0; }
            }
        }

        #endregion


        #region Nested Type: Customer2

        private class Customer2 {
            [Named("some name")]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            [Named("some name")]
            public void SomeAction() {}
        }

        #endregion

        #region Nested Type: Customer13

        private class Customer13 {
            [Named("some name")]
            public void SomeAction() { }

            [Named("some name")]
            public void SomeAction1() { }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public void SomeAction([Named("some name")] int x) {}
        }

        #endregion

        #endregion

        #region displayname test data

        #region Nested Type: Customer5

        [DisplayName("some name")]
        private class Customer5 { }

        #endregion

        #region Nested Type: Customer6

        private class Customer6 {
            [DisplayName("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer16

        private class Customer16 {
            [DisplayName("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }

            [DisplayName("some name")]
            public int NumberOfOrders1 {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer7

        private class Customer7 {
            [DisplayName("some name")]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer8

        private class Customer8 {
            [DisplayName("some name")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested Type: Customer18

        private class Customer18 {
            [DisplayName("some name")]
            public void SomeAction() { }

            [DisplayName("some name")]
            public void SomeAction1() { }
        }

        #endregion
       

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}