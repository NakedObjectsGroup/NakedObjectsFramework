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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class ViewModelFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ViewModelFacetFactory(Reflector);
        }

        [TearDown]
        public new void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ViewModelFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IViewModelFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

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

        private class Class2 {
            public string[] DeriveKeys() {
                throw new NotImplementedException();
            }

            public void PopulateUsingKeys(string[] instanceId) {
                throw new NotImplementedException();
            }
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestViewModelDerive() {
            facetFactory.Process(typeof (Class1), MethodRemover, Specification);
            var facet = Specification.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1 {Value1 = "testValue1", Value2 = "testValue2"};
            var mock = new Mock<INakedObject>();
            INakedObject value = mock.Object;
            mock.Setup(no => no.Object).Returns(testClass);


            string[] key = facet.Derive(value);

            Assert.AreEqual(2, key.Length);
            Assert.AreEqual(testClass.Value1, key[0]);
            Assert.AreEqual(testClass.Value2, key[1]);
        }

        [Test]
        public void TestViewModelNotPickedUp() {
            facetFactory.Process(typeof (Class2), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IViewModelFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestViewModelPickedUp() {
            facetFactory.Process(typeof (Class1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelFacetConvention);

            MethodInfo m1 = typeof (Class1).GetMethod("DeriveKeys");
            MethodInfo m2 = typeof (Class1).GetMethod("PopulateUsingKeys");

            AssertMethodsRemoved(new[] {m1, m2});
        }

        [Test]
        public void TestViewModelPopulate() {
            facetFactory.Process(typeof (Class1), MethodRemover, Specification);
            var facet = Specification.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1();
            var keys = new[] {"testValue1", "testValue2"};
            var mock = new Mock<INakedObject>();
            INakedObject value = mock.Object;
            mock.Setup(no => no.Object).Returns(testClass);

            facet.Populate(keys, value);


            Assert.AreEqual(keys[0], testClass.Value1);
            Assert.AreEqual(keys[1], testClass.Value2);
        }
    }
}