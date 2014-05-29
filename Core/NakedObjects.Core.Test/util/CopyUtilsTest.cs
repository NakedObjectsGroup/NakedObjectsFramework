// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NAssert = NUnit.Framework.Assert;

namespace NakedObjects.Core.Util {
    [TestFixture]
    public class CopyUtilsTest {
        public class SimpleObject {
            public int ValueOne { get; set; }
            public int ValueTwo { get; set; }
        }

        public class ReferenceObject {
            public SimpleObject ReferenceOne { get; set; }
        }

        public class CollectionObject {
            private ICollection<SimpleObject> collectionOne = new List<SimpleObject>();

            public ICollection<SimpleObject> CollectionOne {
                get { return collectionOne; }
                set { collectionOne = value; }
            }
        }

        public class AllObject {
            private ICollection<SimpleObject> collectionOne = new List<SimpleObject>();
            public int ValueOne { get; set; }
            public int ValueTwo { get; set; }
            public SimpleObject ReferenceOne { get; set; }

            public ICollection<SimpleObject> CollectionOne {
                get { return collectionOne; }
                set { collectionOne = value; }
            }
        }

        [Test]
        public void TestAllClone() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var ao = new AllObject {ValueOne = 5, ValueTwo = 6, ReferenceOne = so1};
            ao.CollectionOne.Add(so2);

            AllObject clone = (AllObject) CopyUtils.CloneObjectTest(ao);
            NAssert.AreNotSame(ao, clone);
            NAssert.AreSame(ao.GetType(), clone.GetType());
            NAssert.AreEqual(ao.ValueOne, clone.ValueOne);
            NAssert.AreEqual(ao.ValueTwo, clone.ValueTwo);
            NAssert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
            NAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            NAssert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
        }

        [Test]
        public void TestAllUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var so3 = new SimpleObject {ValueOne = 5, ValueTwo = 6};
            var so4 = new SimpleObject {ValueOne = 7, ValueTwo = 8};
            var ao = new AllObject {ValueOne = 9, ValueTwo = 10, ReferenceOne = so1};
            ao.CollectionOne.Add(so2);

            AllObject clone = (AllObject) CopyUtils.CloneObjectTest(ao);

            clone.ValueTwo = 11;
            clone.ReferenceOne = so3;
            clone.CollectionOne.Add(so4);

            NAssert.AreNotSame(ao, clone);
            NAssert.AreSame(ao.GetType(), clone.GetType());
            NAssert.AreEqual(ao.ValueOne, clone.ValueOne);
            NAssert.AreNotEqual(ao.ValueTwo, clone.ValueTwo);
            NAssert.AreNotSame(ao.ReferenceOne, clone.ReferenceOne);
            NAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            NAssert.AreNotEqual(ao.CollectionOne.Count(), clone.CollectionOne.Count());

            CopyUtils.UpdateFromClone(ao, clone);

            NAssert.AreNotSame(ao, clone);
            NAssert.AreSame(ao.GetType(), clone.GetType());
            NAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            NAssert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
            NAssert.AreEqual(ao.ValueOne, clone.ValueOne);
            NAssert.AreEqual(ao.ValueTwo, clone.ValueTwo);
            NAssert.AreEqual(ao.CollectionOne.Count(), clone.CollectionOne.Count());
            NAssert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
            NAssert.AreSame(ao.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
        }

        [Test]
        public void TestCollectionClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var co = new CollectionObject();
            co.CollectionOne.Add(so);

            CollectionObject clone = (CollectionObject) CopyUtils.CloneObjectTest(co);
            NAssert.AreNotSame(co, clone);
            NAssert.AreSame(co.GetType(), clone.GetType());
            NAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            NAssert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
        }

        [Test]
        public void TestCollectionUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var co = new CollectionObject();
            co.CollectionOne.Add(so1);

            CollectionObject clone = (CollectionObject) CopyUtils.CloneObjectTest(co);

            clone.CollectionOne.Add(so2);

            NAssert.AreNotSame(co, clone);
            NAssert.AreSame(co.GetType(), clone.GetType());
            NAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            NAssert.AreNotEqual(co.CollectionOne.Count(), clone.CollectionOne.Count());

            CopyUtils.UpdateFromClone(co, clone);

            NAssert.AreNotSame(co, clone);
            NAssert.AreSame(co.GetType(), clone.GetType());
            NAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            NAssert.AreEqual(co.CollectionOne.Count(), clone.CollectionOne.Count());
            NAssert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
            NAssert.AreSame(co.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
        }

        [Test]
        public void TestReferenceClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var ro = new ReferenceObject {ReferenceOne = so};

            ReferenceObject clone = (ReferenceObject) CopyUtils.CloneObjectTest(ro);

            NAssert.AreNotSame(ro, clone);
            NAssert.AreSame(ro.GetType(), clone.GetType());

            NAssert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
        }

        [Test]
        public void TestReferenceUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var ro = new ReferenceObject {ReferenceOne = so1};

            ReferenceObject clone = (ReferenceObject) CopyUtils.CloneObjectTest(ro);
            clone.ReferenceOne = so2;

            NAssert.AreNotSame(ro, clone);
            NAssert.AreSame(ro.GetType(), clone.GetType());
            NAssert.AreNotSame(ro.ReferenceOne, clone.ReferenceOne);

            CopyUtils.UpdateFromClone(ro, clone);

            NAssert.AreNotSame(ro, clone);
            NAssert.AreSame(ro.GetType(), clone.GetType());
            NAssert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
        }

        [Test]
        public void TestSimpleClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            SimpleObject clone = (SimpleObject) CopyUtils.CloneObjectTest(so);

            NAssert.AreNotSame(so, clone);
            NAssert.AreSame(so.GetType(), clone.GetType());
            NAssert.AreEqual(so.ValueOne, clone.ValueOne);
            NAssert.AreEqual(so.ValueTwo, clone.ValueTwo);
        }

        [Test]
        public void TestSimpleUpdate() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            SimpleObject clone = (SimpleObject) CopyUtils.CloneObjectTest(so);
            clone.ValueTwo = 3;

            NAssert.AreNotSame(so, clone);
            NAssert.AreSame(so.GetType(), clone.GetType());
            NAssert.AreEqual(so.ValueOne, clone.ValueOne);
            NAssert.AreNotEqual(so.ValueTwo, clone.ValueTwo);

            CopyUtils.UpdateFromClone(so, clone);

            NAssert.AreNotSame(so, clone);
            NAssert.AreSame(so.GetType(), clone.GetType());
            NAssert.AreEqual(so.ValueOne, clone.ValueOne);
            NAssert.AreEqual(so.ValueTwo, clone.ValueTwo);
        }
    }
}