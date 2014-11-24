// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class TableViewAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TableViewAnnotationFacetFactory();
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private TableViewAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (ITableViewFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            [TableView(true, "col1", "col2")]
            public Order[] Orders { get; set; }

            [TableView(false, "col3", "col4")]
            public ICollection<Order> Orders1 { get; set; }

            [TableView(false, "col5", "col6")]
            public Order[] OrdersAction() {
                return null;
            }

            [TableView(true, "col7", "col8")]
            public ICollection<Order> OrdersAction1() {
                return null;
            }

            [TableView(true)]
            public ICollection<Order> OrdersAction2() {
                return null;
            }

            [TableView(true, "col7", "col8")]
            public IQueryable<Order> OrdersAction3() {
                return null;
            }
        }

        private class Customer2 {
            public Order[] Orders { get; set; }

            public ICollection<Order> Orders1 { get; set; }

            public Order[] OrdersAction() {
                return null;
            }

            public ICollection<Order> OrdersAction1() {
                return null;
            }
        }


        private class Order {}

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestTableViewFacetNotPickedUpOnArray() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestTableViewFacetNotPickedUpOnArrayAction() {
            MethodInfo method = FindMethod(typeof (Customer2), "OrdersAction");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestTableViewFacetNotPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestTableViewFacetNotPickedUpOnCollectionAction() {
            MethodInfo method = FindMethod(typeof (Customer2), "OrdersAction1");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestTableViewFacetPickedUpOnArray() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col1", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col2", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTableViewFacetPickedUpOnArrayAction() {
            MethodInfo method = FindMethod(typeof (Customer1), "OrdersAction");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(false, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col5", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col6", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTableViewFacetPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(false, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col3", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col4", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTableViewFacetPickedUpOnCollectionAction() {
            MethodInfo method = FindMethod(typeof (Customer1), "OrdersAction1");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col7", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col8", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTableViewFacetPickedUpOnCollectionActionNoColumns() {
            MethodInfo method = FindMethod(typeof (Customer1), "OrdersAction2");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(0, tableViewFacetFromAnnotation.Columns.Length);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTableViewFacetPickedUpOnQueryableAction() {
            MethodInfo method = FindMethod(typeof (Customer1), "OrdersAction3");
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col7", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col8", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
        }


        // Copyright (c) Naked Objects Group Ltd.
    }
}