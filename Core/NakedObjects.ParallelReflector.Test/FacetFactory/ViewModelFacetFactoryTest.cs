// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class ViewModelFacetFactoryTest : AbstractFacetFactoryTest {
        private ViewModelFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IViewModelFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestViewModelDerive() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Class1), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1 {Value1 = "testValue1", Value2 = "testValue2"};
            var mock = new Mock<INakedObjectAdapter>();
            var value = mock.Object;
            mock.Setup(no => no.Object).Returns(testClass);

            var key = facet.Derive(value, null, null);

            Assert.AreEqual(2, key.Length);
            Assert.AreEqual(testClass.Value1, key[0]);
            Assert.AreEqual(testClass.Value2, key[1]);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestViewModelNotPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Class2), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IViewModelFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestViewModelPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var class1Type = typeof(Class1);
            metamodel = facetFactory.Process(Reflector, class1Type, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelFacetConvention);

            var m1 = class1Type.GetMethod("DeriveKeys");
            var m2 = class1Type.GetMethod("PopulateUsingKeys");

            AssertMethodsRemoved(new[] {m1, m2});
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestViewModelEditPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var class3Type = typeof(Class3);
            metamodel = facetFactory.Process(Reflector, class3Type, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelEditFacetConvention);

            var m1 = class3Type.GetMethod("DeriveKeys");
            var m2 = class3Type.GetMethod("PopulateUsingKeys");

            AssertMethodsRemoved(new[] {m1, m2});
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestViewModelSwitchablePickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var class4Type = typeof(Class4);
            metamodel = facetFactory.Process(Reflector, class4Type, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelSwitchableFacetConvention);

            var m1 = class4Type.GetMethod("DeriveKeys");
            var m2 = class4Type.GetMethod("PopulateUsingKeys");
            var m3 = class4Type.GetMethod("IsEditView");

            AssertMethodsRemoved(new[] {m1, m2, m3});
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestViewModelPopulate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Class1), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1();
            var keys = new[] {"testValue1", "testValue2"};
            var mock = new Mock<INakedObjectAdapter>();
            var value = mock.Object;
            mock.Setup(no => no.Object).Returns(testClass);

            facet.Populate(keys, value, null, null);

            Assert.AreEqual(keys[0], testClass.Value1);
            Assert.AreEqual(keys[1], testClass.Value2);
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Class1

        private class Class1 : IViewModel {
            public string Value1 { get; set; }
            public string Value2 { get; set; }

            #region IViewModel Members

            public string[] DeriveKeys() {
                return new[] {Value1, Value2};
            }

            public void PopulateUsingKeys(string[] instanceId) {
                Value1 = instanceId[0];
                Value2 = instanceId[1];
            }

            #endregion
        }

        #endregion

        #region Nested type: Class2

        private class Class2 {
            // ReSharper disable once UnusedMember.Local
            public string[] DeriveKeys() => null;

            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void PopulateUsingKeys(string[] instanceId) { }
        }

        #endregion

        #region Nested type: Class3

        private class Class3 : IViewModelEdit {
            private string Value1 { get; set; }
            private string Value2 { get; set; }

            #region IViewModelEdit Members

            public string[] DeriveKeys() {
                return new[] {Value1, Value2};
            }

            public void PopulateUsingKeys(string[] instanceId) {
                Value1 = instanceId[0];
                Value2 = instanceId[1];
            }

            #endregion
        }

        #endregion

        #region Nested type: Class4

        private class Class4 : IViewModelSwitchable {
            private string Value1 { get; set; }
            private string Value2 { get; set; }

            #region IViewModelSwitchable Members

            public string[] DeriveKeys() {
                return new[] {Value1, Value2};
            }

            public void PopulateUsingKeys(string[] instanceId) {
                Value1 = instanceId[0];
                Value2 = instanceId[1];
            }

            public bool IsEditView() => false;

            #endregion
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ViewModelFacetFactory(0, null);
        }

        [TestCleanup]
        public new void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }
}