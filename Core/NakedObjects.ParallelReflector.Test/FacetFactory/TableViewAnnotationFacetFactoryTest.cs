// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    
    public class TableViewAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private TableViewAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(ITableViewFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestTableViewFacetNotPickedUpOnArray() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer2), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetNotPickedUpOnArrayAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "OrdersAction");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetNotPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer2), "Orders1");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetNotPickedUpOnCollectionAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "OrdersAction1");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnArray() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col1", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col2", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnArrayAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer1), "OrdersAction");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(false, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col5", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col6", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders1");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(false, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col3", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col4", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnCollectionAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer1), "OrdersAction1");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col7", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col8", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnCollectionActionNoColumns() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer1), "OrdersAction2");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(0, tableViewFacetFromAnnotation.Columns.Length);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetPickedUpOnQueryableAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer1), "OrdersAction3");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            Assert.IsNotNull(metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col7", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col8", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetIgnoresDuplicatesOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer1), "OrdersAction4");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(true, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col7", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col8", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTableViewFacetIgnoresDuplicatesOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders2");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITableViewFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TableViewFacet);
            var tableViewFacetFromAnnotation = (TableViewFacet) facet;
            Assert.AreEqual(false, tableViewFacetFromAnnotation.Title);
            Assert.AreEqual(2, tableViewFacetFromAnnotation.Columns.Length);
            Assert.AreEqual("col3", tableViewFacetFromAnnotation.Columns[0]);
            Assert.AreEqual("col4", tableViewFacetFromAnnotation.Columns[1]);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer1

        private class Customer1 {
            [TableView(true, "col1", "col2")]
            public Order[] Orders { get; set; }

            [TableView(false, "col3", "col4")]
            public ICollection<Order> Orders1 { get; set; }

            [TableView(false, "col3", "col4", "col3", "col4")]
            public ICollection<Order> Orders2 { get; set; }

            [TableView(false, "col5", "col6")]
            public Order[] OrdersAction() => null;

            [TableView(true, "col7", "col8")]
            public ICollection<Order> OrdersAction1() => null;

            [TableView(true)]
            public ICollection<Order> OrdersAction2() => null;

            [TableView(true, "col7", "col8")]
            public IQueryable<Order> OrdersAction3() => null;

            [TableView(true, "col7", "col8", "col7", "col8")]
            public IQueryable<Order> OrdersAction4() => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public Order[] Orders { get; set; }
            public ICollection<Order> Orders1 { get; set; }

            public Order[] OrdersAction() => null;

            public ICollection<Order> OrdersAction1() => null;
        }

        #endregion

        #region Nested type: Order

// ReSharper disable once ClassNeverInstantiated.Local
        private class Order { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TableViewAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        // Copyright (c) Naked Objects Group Ltd.
    }

    // ReSharper restore UnusedMember.Local
}