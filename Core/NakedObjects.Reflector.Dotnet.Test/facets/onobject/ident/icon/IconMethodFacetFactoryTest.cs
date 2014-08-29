// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon {
    [TestFixture]
    public class IconMethodFacetFactoryTest : AbstractFacetFactoryTest {
        private IconMethodFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IIconFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new IconMethodFacetFactory(reflector);
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
        public void TestIconNameMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo iconNameMethod = FindMethod(typeof (Customer), "IconName");
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            //Assert.AreEqual(iconNameMethod.Name, methodRemover.GetRemoveMethodMethodCalls()[0].Name);
            Assert.Fail(); // fix this 
        }

        [Test]
        public void TestIconNameFromMethod() {
            facetFactory.Process(typeof(Customer), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);      
            Assert.IsNull(facet.GetIconName());

            //var no = ((ProgrammableReflector)reflector).TestSystem.AdapterFor(new Customer());
            //Assert.AreEqual("TestName", facet.GetIconName(no));
            Assert.Fail(); // fix this 
        }

        [Test]
        public void TestIconNameFromAttribute() {         
            facetFactory.Process(typeof(Customer1), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetAnnotation);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            //var no = ((ProgrammableReflector) reflector).TestSystem.AdapterFor(new Customer1());
            //Assert.AreEqual("AttributeName", facet.GetIconName(no));
            Assert.Fail(); // fix this 
        }

        [Test]
        public void TestIconNameWithFallbackAttribute() {
            facetFactory.Process(typeof(Customer2), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            //var no = ((ProgrammableReflector)reflector).TestSystem.AdapterFor(new Customer2());
            //Assert.AreEqual("TestName", facet.GetIconName(no));

            Assert.Fail(); // fix this 
        }

        #region Nested Type: Customer

        private class Customer {
            public string IconName() {
                return "TestName";
            }
        }

        [IconName("AttributeName")]
        private class Customer1 {
           
        }

        [IconName("AttributeName")]
        private class Customer2 {
            public string IconName() {
                return "TestName";
            }
        }


        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}