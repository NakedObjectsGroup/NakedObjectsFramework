// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Facets.Properties.Set;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.Access;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Collections {
    [TestFixture]
    public class CollectionFieldMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new CollectionFieldMethodsFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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


        private class Customer {
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer1 {
            public ArrayList Orders {
                get { return null; }
            }
        }

        private class Customer10 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}
        }

        private class Customer11 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}
        }

        private class Customer12 {
            public IList Orders {
                get { return null; }
            }

            public void ClearOrders() {}
        }

        private class Customer13 {
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer14 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}

            public string ValidateAddToOrders(Order o) {
                return null;
            }
        }

        private class Customer15 {
            public IList Orders {
                get { return null; }
            }

            public void RemoveFromOrders(Order o) {}

            public string ValidateRemoveFromOrders(Order o) {
                return null;
            }
        }

        private class Customer16 {
            public IList<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        private class Customer17 {
            public IList<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Customer c) {}

            public void RemoveFromOrders(Customer c) {}
        }

        private class Customer18 {
            public ISet<Order> Orders {
                get { return null; }
            }

            public void AddToOrders(Customer c) {}

            public void RemoveFromOrders(Customer c) {}
        }


        private class Customer2 {
            public ArrayList Orders {
                get { return null; }
            }
        }

        private class Customer3 {
            public object[] Orders {
                get { return null; }
            }
        }

        private class Customer4 {
            public Order[] Orders {
                get { return null; }
            }
        }

        private class Customer5 {
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer6 {
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer7 {
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer8 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        private class Customer9 {
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

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

        private class Order {}


        [Test]
        public void TestCannotInferTypeOfFacetIfNoExplicitAddToOrRemoveFromMethods() {
            PropertyInfo property = FindProperty(typeof (Customer6), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (ITypeOfFacet)));
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestInstallsDisabledAlways() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAlways);
        }

        [Test]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
        }

        [Test]
        public void TestPropertyAccessorFacetIsInstalledForArrayListAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacetViaAccessor);
            var propertyAccessorFacetViaAccessor = (PropertyAccessorFacetViaAccessor) facet;
            Assert.AreEqual(property.GetGetMethod(), propertyAccessorFacetViaAccessor.GetMethod());
        }

        [Test]
        public void TestPropertyAccessorFacetIsInstalledForIListAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacetViaAccessor);
            var propertyAccessorFacetViaAccessor = (PropertyAccessorFacetViaAccessor) facet;
            Assert.AreEqual(property.GetGetMethod(), propertyAccessorFacetViaAccessor.GetMethod());
        }

        [Test]
        public void TestPropertyAccessorFacetIsInstalledForObjectArrayAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer3), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacetViaAccessor);
            var propertyAccessorFacetViaAccessor = (PropertyAccessorFacetViaAccessor) facet;
            Assert.AreEqual(property.GetGetMethod(), propertyAccessorFacetViaAccessor.GetMethod());
        }

        [Test]
        public void TestPropertyAccessorFacetIsInstalledForOrderArrayAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacetViaAccessor);
            var propertyAccessorFacetViaAccessor = (PropertyAccessorFacetViaAccessor) facet;
            Assert.AreEqual(property.GetGetMethod(), propertyAccessorFacetViaAccessor.GetMethod());
        }

        [Test]
        public void TestPropertyAccessorFacetIsInstalledForSetAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacetViaAccessor);
            var propertyAccessorFacetViaAccessor = (PropertyAccessorFacetViaAccessor) facet;
            Assert.AreEqual(property.GetGetMethod(), propertyAccessorFacetViaAccessor.GetMethod());
        }

        [Test]
        public void TestSetFacetAddedToSet() {
            Type type = typeof (Customer18);
            PropertyInfo property = FindProperty(type, "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIsASetFacet));
            Assert.IsNotNull(facet);
            Assert.IsInstanceOf(typeof (IsASetFacet), facet);
        }

        [Test]
        public void TestSetFacetNoAddedToList() {
            Type type = typeof (Customer17);
            PropertyInfo property = FindProperty(type, "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIsASetFacet));
            Assert.IsNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}