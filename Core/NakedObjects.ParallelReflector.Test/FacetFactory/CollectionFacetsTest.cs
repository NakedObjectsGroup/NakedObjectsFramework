// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class CollectionFacetsTest {
        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodel = new Mock<IMetamodelManager>().Object;
        private readonly Mock<ILifecycleManager> mockLifecycleManager = new Mock<ILifecycleManager>();
        private readonly Mock<INakedObjectManager> mockManager = new Mock<INakedObjectManager>();
        private readonly Mock<IObjectPersistor> mockPersistor = new Mock<IObjectPersistor>();
        private readonly IOid oid = new Mock<IOid>().Object;
        private readonly IObjectPersistor persistor;
        private readonly ISession session = new Mock<ISession>().Object;
        private readonly ISpecification specification = new Mock<ISpecification>().Object;

        public CollectionFacetsTest() {
            lifecycleManager = mockLifecycleManager.Object;
            persistor = mockPersistor.Object;
            manager = mockManager.Object;
            mockManager.Setup(mm => mm.CreateAdapter(It.IsAny<object>(), null, null)).Returns<object, IOid, IVersion>((obj, oid, ver) => AdapterFor(obj));
        }

        private INakedObjectAdapter AdapterFor(object obj) {
            return new NakedObjectAdapter(metamodel, session, persistor, lifecycleManager, manager, obj, oid);
        }

        private void Size(ICollectionFacet collectionFacet, INakedObjectAdapter collection) {
            Assert.AreEqual(2, collectionFacet.AsEnumerable(collection, manager).Count());
        }

        // ReSharper disable PossibleMultipleEnumeration

        private void ValidateCollection(ICollectionFacet collectionFacet, INakedObjectAdapter collection, IEnumerable<object> objects) {
            INakedObjectAdapter[] collectionAsEnumerable = collectionFacet.AsEnumerable(collection, manager).ToArray();
            Assert.AreEqual(collectionAsEnumerable.Count(), objects.Count());
            IEnumerable<Tuple<object, object>> zippedCollections = collectionAsEnumerable.Zip(objects, (no, o1) => new Tuple<object, object>(no.Object, o1));
            zippedCollections.ForEach(t => Assert.AreSame(t.Item1, t.Item2));
        }

        // ReSharper restore PossibleMultipleEnumeration

        private void FirstElement(ICollectionFacet collectionFacet, INakedObjectAdapter collection, object first) {
            Assert.AreSame(first, collectionFacet.AsEnumerable(collection, manager).First().Object);
        }

        private void Contains(ICollectionFacet collectionFacet, INakedObjectAdapter collection, object first, object second) {
            Assert.IsTrue(collectionFacet.Contains(collection, AdapterFor(first)));
            Assert.IsFalse(collectionFacet.Contains(collection, AdapterFor(second)));
        }

        // ReSharper disable PossibleMultipleEnumeration

        private void Init(ICollectionFacet collectionFacet, INakedObjectAdapter collection, IEnumerable<object> data1, IEnumerable<object> data2) {
            collectionFacet.Init(collection, data1.Select(AdapterFor).ToArray());
            ValidateCollection(collectionFacet, collection, data1);
            collectionFacet.Init(collection, data2.Select(AdapterFor).ToArray());
            ValidateCollection(collectionFacet, collection, data2);
        }

        // ReSharper restore PossibleMultipleEnumeration

        private void Page(ICollectionFacet testArrayFacet, INakedObjectAdapter collection, object first) {
            INakedObjectAdapter pagedCollection = testArrayFacet.Page(1, 1, collection, manager, false);

            Assert.IsTrue(((IEnumerable) pagedCollection.Object).Cast<string>().Count() == 1);
            Assert.AreSame(((IEnumerable) pagedCollection.Object).Cast<string>().First(), first);
        }

        [TestMethod]
        public void ArrayContains() {
            var testArray = new[] {"element1", "element2"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Contains(testArrayFacet, testAdaptedArray, "element1", "element3");
        }

        [TestMethod]
        public void ArrayFirstElement() {
            var testArray = new[] {"element1", "element2"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            FirstElement(testArrayFacet, testAdaptedArray, "element1");
        }

        [TestMethod]
        public void ArrayGetEnumeratorFor() {
            var testArray = new[] {"element1", "element2"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            ValidateCollection(testArrayFacet, testAdaptedArray, testArray);
        }

        [TestMethod]
        public void ArrayInit() {
            var testArray = new[] {"element1", "element2"};
            var testArray1 = new[] {"element2", "element3"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Init(testArrayFacet, testAdaptedArray, testArray, testArray1);
        }

        [TestMethod]
        public void ArrayInitAllEmpty() {
            var testArray = new string[] { };
            var testArray1 = new string[] { };
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Init(testArrayFacet, testAdaptedArray, testArray, testArray1);
        }

        [TestMethod]
        public void ArrayInitEmpty() {
            var testArray = new string[] { };
            var testArray1 = new[] {"element2", "element3"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Init(testArrayFacet, testAdaptedArray, testArray, testArray1);
        }

        [TestMethod]
        public void ArrayInitToEmpty() {
            var testArray = new[] {"element1", "element2"};
            var testArray1 = new string[] { };
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Init(testArrayFacet, testAdaptedArray, testArray, testArray1);
        }

        [TestMethod]
        public void ArrayPage() {
            var testArray = new[] {"element1", "element2"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Page(testArrayFacet, testAdaptedArray, "element1");
        }

        [TestMethod]
        public void ArraySize() {
            var testArray = new[] {"element1", "element2"};
            var testArrayFacet = new ArrayFacet(specification);
            INakedObjectAdapter testAdaptedArray = AdapterFor(testArray);
            Size(testArrayFacet, testAdaptedArray);
        }

        [TestMethod]
        public void CollectionContains() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Contains(testCollectionFacet, testAdaptedCollection, "element1", "element3");
        }

        [TestMethod]
        public void CollectionFirstElement() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            FirstElement(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void CollectionGetEnumeratorFor() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            ValidateCollection(testCollectionFacet, testAdaptedCollection, testCollection.Cast<object>());
        }

        [TestMethod]
        public void CollectionInit() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollection1 = new[] {"element2", "element3"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection.Cast<object>(), testCollection1);
        }

        [TestMethod]
        public void CollectionInitAllEmpty() {
            var testCollection = new ArrayList();
            var testCollection1 = new string[] { };
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection.Cast<object>(), testCollection1);
        }

        [TestMethod]
        public void CollectionInitEmpty() {
            var testCollection = new ArrayList();
            var testCollection1 = new[] {"element2", "element3"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection.Cast<object>(), testCollection1);
        }

        [TestMethod]
        public void CollectionInitToEmpty() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollection1 = new string[] { };
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection.Cast<object>(), testCollection1);
        }

        [TestMethod]
        public void CollectionNotIsSet() {
            ICollectionFacet testCollectionFacet = new CollectionFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
            testCollectionFacet = new ArrayFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
            testCollectionFacet = new CollectionFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
        }

        [TestMethod]
        public void CollectionPage() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Page(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void CollectionSize() {
            var testCollection = new ArrayList {"element1", "element2"};
            var testCollectionFacet = new CollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Size(testCollectionFacet, testAdaptedCollection);
        }

        [TestMethod]
        public void GenericCollectionContains() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Contains(testCollectionFacet, testAdaptedCollection, "element1", "element3");
        }

        [TestMethod]
        public void GenericCollectionFirstElement() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            FirstElement(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericCollectionGetEnumeratorFor() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            ValidateCollection(testCollectionFacet, testAdaptedCollection, testCollection);
        }

        [TestMethod]
        public void GenericCollectionInit() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollection1 = new[] {"element2", "element3"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection, testCollection1);
        }

        [TestMethod]
        public void GenericCollectionInitAllEmpty() {
            var testCollection = new List<string>();
            var testCollection1 = new string[] { };
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection, testCollection1);
        }

        [TestMethod]
        public void GenericCollectionInitEmpty() {
            var testCollection = new List<string>();
            var testCollection1 = new[] {"element2", "element3"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection, testCollection1);
        }

        [TestMethod]
        public void GenericCollectionInitToEmpty() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollection1 = new string[] { };
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Init(testCollectionFacet, testAdaptedCollection, testCollection, testCollection1);
        }

        [TestMethod]
        public void GenericCollectionIsSet() {
            ICollectionFacet testCollectionFacet = new GenericCollectionFacet(specification, true);
            Assert.IsTrue(testCollectionFacet.IsASet);
            testCollectionFacet = new GenericIEnumerableFacet(specification, true);
            Assert.IsTrue(testCollectionFacet.IsASet);
            testCollectionFacet = new GenericIQueryableFacet(specification, true);
            Assert.IsTrue(testCollectionFacet.IsASet);
        }

        [TestMethod]
        public void GenericCollectionNotIsSet() {
            ICollectionFacet testCollectionFacet = new GenericCollectionFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
            testCollectionFacet = new GenericIEnumerableFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
            testCollectionFacet = new GenericIQueryableFacet(specification);
            Assert.IsFalse(testCollectionFacet.IsASet);
        }

        [TestMethod]
        public void GenericCollectionPage() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Page(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericCollectionSize() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericCollectionFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Size(testCollectionFacet, testAdaptedCollection);
        }

        [TestMethod]
        public void GenericEnumerableContains() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericIEnumerableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Contains(testCollectionFacet, testAdaptedCollection, "element1", "element3");
        }

        [TestMethod]
        public void GenericEnumerableFirstElement() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericIEnumerableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            FirstElement(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericEnumerableGetEnumeratorFor() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericIEnumerableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            ValidateCollection(testCollectionFacet, testAdaptedCollection, testCollection);
        }

        [TestMethod]
        public void GenericEnumerablePage() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericIEnumerableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Page(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericEnumerableSize() {
            var testCollection = new List<string> {"element1", "element2"};
            var testCollectionFacet = new GenericIEnumerableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Size(testCollectionFacet, testAdaptedCollection);
        }

        [TestMethod]
        public void GenericQueryableContains() {
            IQueryable<string> testCollection = new List<string> {"element1", "element2"}.AsQueryable();
            var testCollectionFacet = new GenericIQueryableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Contains(testCollectionFacet, testAdaptedCollection, "element1", "element3");
        }

        [TestMethod]
        public void GenericQueryableFirstElement() {
            IQueryable<string> testCollection = new List<string> {"element1", "element2"}.AsQueryable();
            var testCollectionFacet = new GenericIQueryableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            FirstElement(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericQueryableGetEnumeratorFor() {
            IQueryable<string> testCollection = new List<string> {"element1", "element2"}.AsQueryable();
            var testCollectionFacet = new GenericIQueryableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            ValidateCollection(testCollectionFacet, testAdaptedCollection, testCollection);
        }

        [TestMethod]
        public void GenericQueryablePage() {
            IQueryable<string> testCollection = new List<string> {"element1", "element2"}.AsQueryable();
            var testCollectionFacet = new GenericIQueryableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Page(testCollectionFacet, testAdaptedCollection, "element1");
        }

        [TestMethod]
        public void GenericQueryableSize() {
            IQueryable<string> testCollection = new List<string> {"element1", "element2"}.AsQueryable();
            var testCollectionFacet = new GenericIQueryableFacet(specification);
            INakedObjectAdapter testAdaptedCollection = AdapterFor(testCollection);
            Size(testCollectionFacet, testAdaptedCollection);
        }
    }
}