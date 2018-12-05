// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class NotNavigableAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NotNavigableAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(INotNavigableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof(Customer1), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof(Customer), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnType() {
            PropertyInfo property = FindProperty(typeof(Customer2), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
        }

        #region Nested type: Customer

        private class Customer {
            [NotNavigable]
// ReSharper disable once UnusedMember.Local
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [NotNavigable]
// ReSharper disable once UnusedMember.Local
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public NotNavigable FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: NotNavigable

        [NotNavigable]
        private class NotNavigable { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NotNavigableAnnotationFacetFactory(0);
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