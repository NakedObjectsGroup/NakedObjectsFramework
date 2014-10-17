// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Defaults {
    [TestFixture]
    public class ActionDefaultAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ActionDefaultAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ActionDefaultAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IActionDefaultsFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer2 {
            public void someAction([DefaultValue(1)] int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }


        [Test]
        public void TestPropertyDefaultAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IActionDefaultsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionDefaultsFacetAnnotation);
            var actionDefaultFacetAnnotation = (ActionDefaultsFacetAnnotation) facet;
            Assert.AreEqual(1, actionDefaultFacetAnnotation.GetDefault(null).Item1);
            Assert.AreEqual(TypeOfDefaultValue.Explicit, actionDefaultFacetAnnotation.GetDefault(null).Item2);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}