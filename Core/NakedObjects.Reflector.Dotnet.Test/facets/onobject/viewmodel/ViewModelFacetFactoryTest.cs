// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    [TestFixture]
    public class ViewModelFacetFactoryTest : AbstractFacetFactoryTest {
        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ViewModelFacetFactory {Reflector = reflector};
        }

        [TearDown]
        public new void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        private ViewModelFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IViewModelFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Class1 : IViewModel {
            public string[] DeriveKeys() {
                return new[] {Value1, Value2};
            }

            public string Value1 { get; set; }
            public string Value2 { get; set; }

            public void PopulateUsingKeys(string[] instanceId) {
                Value1 = instanceId[0];
                Value2 = instanceId[1];

            }
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
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestViewModelNotPickedUp() {
            facetFactory.Process(typeof (Class2), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IViewModelFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestViewModelPickedUp() {
            facetFactory.Process(typeof (Class1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelFacetConvention);

            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(typeof(Class1).GetMethod("DeriveKeys")));
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(typeof(Class1).GetMethod("PopulateUsingKeys")));
        }

        [Test]
        public void TestViewModelDerive() {
            facetFactory.Process(typeof (Class1), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1 {Value1 = "testValue1", Value2 = "testValue2"};
            string[] key = facet.Derive(new ProgrammableNakedObject(testClass, null));

            Assert.AreEqual(2, key.Length);
            Assert.AreEqual(testClass.Value1, key[0]);
            Assert.AreEqual(testClass.Value2, key[1]);
        }

        [Test]
        public void TestViewModelPopulate() {
            facetFactory.Process(typeof (Class1), methodRemover, facetHolder);
            var facet = facetHolder.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1();
            var keys = new[] {"testValue1", "testValue2"};
            facet.Populate(keys, new ProgrammableNakedObject(testClass, null));


            Assert.AreEqual(keys[0], testClass.Value1);
            Assert.AreEqual(keys[1], testClass.Value2);
        }

    }
}