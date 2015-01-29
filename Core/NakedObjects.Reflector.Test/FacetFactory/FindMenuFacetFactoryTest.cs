// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class FindMenuFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new FindMenuFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private FindMenuFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IFindMenuFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {

            // ReSharper disable once UnusedMember.Local
            public void Action1(Foo param1, [FindMenu] Foo param2) { }

            // ReSharper disable once UnusedMember.Local
            public void Action2([FindMenu] string param1) { }

            // ReSharper disable once UnusedMember.Local
            public Foo Property1 { get; set; }

            // ReSharper disable once UnusedMember.Local
            [FindMenu]
            public Foo Property2 { get; set; }

            // ReSharper disable once UnusedMember.Local
            [FindMenu]
            public string Property3 { get; set; }
        }

        private class Foo { }

        [TestMethod]
        public void TestFindMenuFacetNotAddedToParameterByDefault() {
            MethodInfo method = FindMethod(typeof(Customer), "Action1", new[] { typeof(Foo), typeof(Foo)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof(IFindMenuFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestFindMenuAnnotationOnParameterPickedUp() {
            MethodInfo method = FindMethod(typeof(Customer), "Action1", new[] { typeof(Foo), typeof(Foo) });
            facetFactory.ProcessParams(Reflector, method, 1, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof(IFindMenuFacet)));
        }

        [TestMethod]
        public void TestFindMenuAnnotationIgnoredForPrimitiveParameter() {
            MethodInfo method = FindMethod(typeof(Customer), "Action2", new[] { typeof(string) });
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            Assert.IsNull(Specification.GetFacet(typeof(IFindMenuFacet)));
        }

        [TestMethod]
        public void TestFindMenuFacetNotAddedToPropertyByDefault() {
            PropertyInfo property = FindProperty(typeof(Customer), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof(IFindMenuFacet)));
        }

        [TestMethod]
        public void TestFindMenuAnnotationOnPropertyPickedUp() {
            PropertyInfo property = FindProperty(typeof(Customer), "Property2");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof(IFindMenuFacet)));
        }

        [TestMethod]
        public void TestFindMenuAnnotationIgnoredForPrimitiveProperty() {
            PropertyInfo property = FindProperty(typeof(Customer), "Property3");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof(IFindMenuFacet)));
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}