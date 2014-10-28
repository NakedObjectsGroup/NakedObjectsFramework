// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof {
    [TestFixture]
    public class TypeOfAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypeOfAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private TypeOfAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (ITypeOfFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        //[Test]
        //public void TestTypeOfAnnotationIgnoredForActionIfReturnTypeIsntACollectionType() {
        //    MethodInfo actionMethod = FindMethod(typeof (Customer11), "SomeAction");
        //    facetFactory.Process(actionMethod, methodRemover, specification);
        //    IFacet facet = specification.GetFacet(typeof (ITypeOfFacet));
        //    Assert.IsNull(facet);
        //    AssertNoMethodsRemoved();
        //}

        private class Customer10 {
            public Order[] Orders {
                get { return null; }
            }
        }

        private class Customer3 {
            public IList<Order> SomeAction() {
                return null;
            }
        }

        private class Customer4 {
            public IList<Order> Orders {
                get { return null; }
            }
        }

        private class Customer9 {
            public Order[] SomeAction() {
                return null;
            }
        }

        private class Order {}

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.ActionParameter));
        }

        [Test]
        public void TestTypeOfFacetInferredForActionWithArrayReturnType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer9), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet typeOfFacet = Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromArray);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof (Order), elementTypeFacet.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTypeOfFacetInferredForActionWithGenericCollectionReturnType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet typeOfFacet = Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromGenerics);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof (Order), elementTypeFacet.Value);
        }

        [Test]
        public void TestTypeOfFacetInferredForCollectionWithGenericCollectionReturnType() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet typeOfFacet = Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromGenerics);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof (Order), elementTypeFacet.Value);
        }

        [Test]
        public void TestTypeOfFacetIsInferredForCollectionFromOrderArray() {
            PropertyInfo property = FindProperty(typeof (Customer10), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet typeOfFacet = Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromArray);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof (Order), elementTypeFacet.Value);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}