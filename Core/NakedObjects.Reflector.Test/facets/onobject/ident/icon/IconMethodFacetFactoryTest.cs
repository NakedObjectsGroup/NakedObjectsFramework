// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon {
    [TestFixture]
    public class IconMethodFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new IconMethodFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private IconMethodFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IIconFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private INakedObject AdapterFor(object obj) {
            ILifecycleManager persistor = new Mock<ILifecycleManager>().Object;
            ISession session = new Mock<ISession>().Object;
            return new PocoAdapter(Reflector, session, persistor, persistor, obj, null);
        }

        private class Customer {
            public string IconName() {
                return "TestName";
            }
        }

        [IconName("AttributeName")]
        private class Customer1 {}

        [IconName("AttributeName")]
        private class Customer2 {
            public string IconName() {
                return "TestName";
            }
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
        public void TestIconNameFromAttribute() {
            facetFactory.Process(typeof (Customer1), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetAnnotation);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            INakedObject no = AdapterFor(new Customer1());
            Assert.AreEqual("AttributeName", facet.GetIconName(no));
        }

        [Test]
        public void TestIconNameFromMethod() {
            facetFactory.Process(typeof (Customer), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            Assert.IsNull(facet.GetIconName());
            INakedObject no = AdapterFor(new Customer());
            Assert.AreEqual("TestName", facet.GetIconName(no));
        }

        [Test]
        public void TestIconNameMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo iconNameMethod = FindMethod(typeof (Customer), "IconName");
            facetFactory.Process(typeof (Customer), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            AssertMethodRemoved(iconNameMethod);
        }

        [Test]
        public void TestIconNameWithFallbackAttribute() {
            facetFactory.Process(typeof (Customer2), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            INakedObject no = AdapterFor(new Customer2());
            Assert.AreEqual("TestName", facet.GetIconName(no));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}