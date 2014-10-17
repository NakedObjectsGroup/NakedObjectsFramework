// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Adapter;
using NakedObjects.Reflector.FacetFactory;
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