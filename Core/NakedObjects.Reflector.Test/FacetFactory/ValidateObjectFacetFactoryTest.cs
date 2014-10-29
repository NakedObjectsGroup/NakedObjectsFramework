// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    // TODO place holder for validate facet factory tests !!!
    [TestFixture]
    public class ValidateObjectFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ValidateObjectFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ValidateObjectFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IValidateObjectFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
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

        //[Test]
        //public void TestNoExplicitTitleOrToStringMethod() {
        //    facetFactory.Process(typeof (Customer2), MethodRemover, Specification);
        //    Assert.IsNull(Specification.GetFacet(typeof (ITitleFacet)));
        //    AssertNoMethodsRemoved();
        //}

        //[Test]
        //public void TestTitleMethodPickedUpOnClassAndMethodRemoved() {
        //    MethodInfo titleMethod = FindMethod(typeof (Customer), "Title");
        //    facetFactory.Process(typeof (Customer), MethodRemover, Specification);
        //    IFacet facet = Specification.GetFacet(typeof (ITitleFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TitleFacetViaTitleMethod);
        //    var titleFacetViaTitleMethod = (TitleFacetViaTitleMethod) facet;
        //    Assert.AreEqual(titleMethod, titleFacetViaTitleMethod.GetMethod());
        //    AssertMethodRemoved(titleMethod);
        //}

        //[Test]
        //public void TestToStringMethodPickedUpOnClassAndMethodRemoved() {
        //    MethodInfo toStringMethod = FindMethod(typeof (Customer1), "ToString");
        //    facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
        //    IFacet facet = Specification.GetFacet(typeof (ITitleFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is TitleFacetViaToStringMethod);
        //    var titleFacetViaTitleMethod = (TitleFacetViaToStringMethod) facet;
        //    Assert.AreEqual(toStringMethod, titleFacetViaTitleMethod.GetMethod());
        //    AssertMethodRemoved(toStringMethod);
        //}
    }

    // Copyright (c) Naked Objects Group Ltd.
}