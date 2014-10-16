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
using Assert = NUnit.Framework.Assert;

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
            ILifecycleManager lifecycleManager = new Mock<ILifecycleManager>().Object;
            IObjectPersistor persistor = new Mock<IObjectPersistor>().Object;
            ISession session = new Mock<ISession>().Object;
            INakedObjectManager manager = new Mock<INakedObjectManager>().Object;
            return new PocoAdapter(Metamodel, session, persistor, lifecycleManager, manager, obj, null);
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
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestIconNameFromAttribute() {
            facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            var facet = Specification.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetAnnotation);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            INakedObject no = AdapterFor(new Customer1());
            Assert.AreEqual("AttributeName", facet.GetIconName(no));
        }

        [Test]
        public void TestIconNameFromMethod() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            Assert.IsNull(facet.GetIconName());
            INakedObject no = AdapterFor(new Customer());
            Assert.AreEqual("TestName", facet.GetIconName(no));
        }

        [Test]
        public void TestIconNameMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo iconNameMethod = FindMethod(typeof (Customer), "IconName");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            AssertMethodRemoved(iconNameMethod);
        }

        [Test]
        public void TestIconNameWithFallbackAttribute() {
            facetFactory.Process(typeof (Customer2), MethodRemover, Specification);
            var facet = Specification.GetFacet<IIconFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IconFacetViaMethod);
            Assert.AreEqual("AttributeName", facet.GetIconName());
            INakedObject no = AdapterFor(new Customer2());
            Assert.AreEqual("TestName", facet.GetIconName(no));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}