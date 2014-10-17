// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength {
    [TestFixture]
    public class TypicalLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypicalLengthAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private TypicalLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (ITypicalLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TypicalLength(16)]
        private class Customer {}

        private class Customer1 {
            [TypicalLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void someAction([TypicalLength(20)] int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(20, typicalLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(16, typicalLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(30, typicalLengthFacetAnnotation.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}