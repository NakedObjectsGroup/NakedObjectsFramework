// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local
    // ReSharper disable ClassNeverInstantiated.Local
    public class CollectionFieldMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private CollectionFieldMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof (IPropertyAccessorFacet),
                    typeof (ITypeOfFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public void TestCannotInferTypeOfFacetIfNoExplicitAddToOrRemoveFromMethods() {
            PropertyInfo property = FindProperty(typeof (Customer6), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (ITypeOfFacet)));
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestInstallsDisabledAlways() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAlways);
        }

        [TestMethod]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledForArrayListAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledForIListAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledForObjectArrayAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer3), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledForOrderArrayAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledForSetAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestSetFacetAddedToSet() {
            Type type = typeof (Customer18);
            PropertyInfo property = FindProperty(type, "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIsASetFacet));
            Assert.IsNotNull(facet);
            Assert.IsInstanceOfType(facet, typeof (IsASetFacet));
        }

        [TestMethod]
        public void TestSetFacetNoAddedToList() {
            Type type = typeof (Customer17);
            PropertyInfo property = FindProperty(type, "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIsASetFacet));
            Assert.IsNull(facet);
        }

        #region Nested type: Customer

        private class Customer {
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            public ArrayList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer10

        private class Customer10 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}
        }

        #endregion

        #region Nested type: Customer11

        private class Customer11 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}
        }

        #endregion

        #region Nested type: Customer12

        private class Customer12 {
            public IList Orders {
                get { return null; }
            }

            public void ClearOrders() {}
        }

        #endregion

        #region Nested type: Customer13

        private class Customer13 {
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer14

        private class Customer14 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}

            public string ValidateAddToOrders(Order o) {
                return null;
            }
        }

        #endregion

        #region Nested type: Customer15

        private class Customer15 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}

            public string ValidateRemoveFromOrders(Order o) {
                return null;
            }
        }

        #endregion

        #region Nested type: Customer16

        private class Customer16 {
            public IList<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        #endregion

        #region Nested type: Customer17

        private class Customer17 {
            public IList<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Customer c) {}
            public void RemoveFromOrders(Customer c) {}
        }

        #endregion

        #region Nested type: Customer18

        private class Customer18 {
            public ISet<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Customer c) {}
            public void RemoveFromOrders(Customer c) {}
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public ArrayList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            public object[] Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
            public Order[] Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer5

        private class Customer5 {
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer6

        private class Customer6 {
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer7

        private class Customer7 {
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested type: Customer8

        private class Customer8 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        #endregion

        #region Nested type: Customer9

        private class Customer9 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        #endregion

        #region Nested type: CustomerStatic

        public class CustomerStatic {
            public IList Orders {
                get { return null; }
            }

            public static string NameOrders() {
                return "Most Recent Orders";
            }

            public static string DescriptionOrders() {
                return "Some old description";
            }

            public static bool AlwaysHideOrders() {
                return true;
            }

            public static bool ProtectOrders() {
                return true;
            }

            public static bool HideOrders(IPrincipal principal) {
                return true;
            }

            public static string DisableOrders(IPrincipal principal) {
                return "disabled for this user";
            }

            public static void OtherOrders() {}

            public static bool AlwaysHideOtherOrders() {
                return false;
            }

            public static bool ProtectOtherOrders() {
                return false;
            }
        }

        #endregion

        #region Nested type: Order

        private class Order {}

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new CollectionFieldMethodsFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
    // ReSharper restore ClassNeverInstantiated.Local
}