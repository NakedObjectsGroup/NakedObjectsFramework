// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class FindMenuFacetFactoryTest : AbstractFacetFactoryTest {
        private FindMenuFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IFindMenuFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public void TestFindMenuFacetNotAddedToParameterByDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer), "Action1", new[] {typeof (Foo), typeof (Foo)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IFindMenuFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestFindMenuAnnotationOnParameterPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer), "Action1", new[] {typeof (Foo), typeof (Foo)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 1, Specification, metamodel);
            Assert.IsNotNull(Specification.GetFacet(typeof (IFindMenuFacet)));
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestFindMenuAnnotationIgnoredForPrimitiveParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer), "Action2", new[] {typeof (string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof (IFindMenuFacet)));
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestFindMenuFacetNotAddedToPropertyByDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof (Customer), "Property1");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof (IFindMenuFacet)));
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestFindMenuAnnotationOnPropertyPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof (Customer), "Property2");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNotNull(Specification.GetFacet(typeof (IFindMenuFacet)));
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestFindMenuAnnotationIgnoredForPrimitiveProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof (Customer), "Property3");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof (IFindMenuFacet)));
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        private class Customer {
            // ReSharper disable UnusedParameter.Local
            // ReSharper disable UnusedMember.Local
            public void Action1(Foo param1, [FindMenu] Foo param2) {}

            public void Action2([FindMenu] string param1) {}

            public Foo Property1 { get; set; }

            [FindMenu]
            public Foo Property2 { get; set; }

            [FindMenu]
            public string Property3 { get; set; }

            // ReSharper restore UnusedParameter.Local
            // ReSharper restore UnusedMember.Local
        }

        #endregion

        #region Nested type: Foo

        private class Foo {}

        #endregion

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
    }

    // Copyright (c) Naked Objects Group Ltd.
}