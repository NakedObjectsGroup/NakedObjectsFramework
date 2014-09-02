// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
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
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestViewModelDerive() {
            facetFactory.Process(typeof (Class1), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1 {Value1 = "testValue1", Value2 = "testValue2"};
            string[] key = facet.Derive(new ProgrammableNakedObject(testClass, null));

            Assert.AreEqual(2, key.Length);
            Assert.AreEqual(testClass.Value1, key[0]);
            Assert.AreEqual(testClass.Value2, key[1]);
        }

        [Test]
        public void TestViewModelNotPickedUp() {
            facetFactory.Process(typeof (Class2), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IViewModelFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestViewModelPickedUp() {
            facetFactory.Process(typeof (Class1), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IViewModelFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ViewModelFacetConvention);

            MethodInfo m1 = typeof (Class1).GetMethod("DeriveKeys");
            MethodInfo m2 = typeof (Class1).GetMethod("PopulateUsingKeys");

            AssertMethodsRemoved(new[] {m1, m2});
        }

        [Test]
        public void TestViewModelPopulate() {
            facetFactory.Process(typeof (Class1), MethodRemover, FacetHolder);
            var facet = FacetHolder.GetFacet<IViewModelFacet>();
            Assert.IsNotNull(facet);

            var testClass = new Class1();
            var keys = new[] {"testValue1", "testValue2"};
            facet.Populate(keys, new ProgrammableNakedObject(testClass, null));


            Assert.AreEqual(keys[0], testClass.Value1);
            Assert.AreEqual(keys[1], testClass.Value2);
        }
    }
}