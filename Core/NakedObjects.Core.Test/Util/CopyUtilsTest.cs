// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Core.Util;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace NakedObjects.Core.Test.Util {
    [TestFixture]
    public class CopyUtilsTest {
        [Test]
        public void TestAllClone() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var ao = new AllObject {ValueOne = 5, ValueTwo = 6, ReferenceOne = so1};
            ao.CollectionOne.Add(so2);

            var clone = (AllObject) CopyUtils.CloneObjectTest(ao);
            Assert.AreNotSame(ao, clone);
            Assert.AreSame(ao.GetType(), clone.GetType());
            Assert.AreEqual(ao.ValueOne, clone.ValueOne);
            Assert.AreEqual(ao.ValueTwo, clone.ValueTwo);
            Assert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
            Assert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            Assert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
        }

        [Test]
        public void TestAllUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var so3 = new SimpleObject {ValueOne = 5, ValueTwo = 6};
            var so4 = new SimpleObject {ValueOne = 7, ValueTwo = 8};
            var ao = new AllObject {ValueOne = 9, ValueTwo = 10, ReferenceOne = so1};
            ao.CollectionOne.Add(so2);

            var clone = (AllObject) CopyUtils.CloneObjectTest(ao);

            clone.ValueTwo = 11;
            clone.ReferenceOne = so3;
            clone.CollectionOne.Add(so4);

            Assert.AreNotSame(ao, clone);
            Assert.AreSame(ao.GetType(), clone.GetType());
            Assert.AreEqual(ao.ValueOne, clone.ValueOne);
            Assert.AreNotEqual(ao.ValueTwo, clone.ValueTwo);
            Assert.AreNotSame(ao.ReferenceOne, clone.ReferenceOne);
            Assert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            Assert.AreNotEqual(ao.CollectionOne.Count, clone.CollectionOne.Count);

            CopyUtils.UpdateFromClone(ao, clone);

            Assert.AreNotSame(ao, clone);
            Assert.AreSame(ao.GetType(), clone.GetType());
            Assert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
            Assert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
            Assert.AreEqual(ao.ValueOne, clone.ValueOne);
            Assert.AreEqual(ao.ValueTwo, clone.ValueTwo);
            Assert.AreEqual(ao.CollectionOne.Count, clone.CollectionOne.Count);
            Assert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
            Assert.AreSame(ao.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
        }

        [Test]
        public void TestCollectionClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var co = new CollectionObject();
            co.CollectionOne.Add(so);

            var clone = (CollectionObject) CopyUtils.CloneObjectTest(co);
            Assert.AreNotSame(co, clone);
            Assert.AreSame(co.GetType(), clone.GetType());
            Assert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            Assert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
        }

        [Test]
        public void TestCollectionUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var co = new CollectionObject();
            co.CollectionOne.Add(so1);

            var clone = (CollectionObject) CopyUtils.CloneObjectTest(co);

            clone.CollectionOne.Add(so2);

            Assert.AreNotSame(co, clone);
            Assert.AreSame(co.GetType(), clone.GetType());
            Assert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            Assert.AreNotEqual(co.CollectionOne.Count, clone.CollectionOne.Count);

            CopyUtils.UpdateFromClone(co, clone);

            Assert.AreNotSame(co, clone);
            Assert.AreSame(co.GetType(), clone.GetType());
            Assert.AreNotSame(co.CollectionOne, clone.CollectionOne);
            Assert.AreEqual(co.CollectionOne.Count, clone.CollectionOne.Count);
            Assert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
            Assert.AreSame(co.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
        }

        [Test]
        public void TestReferenceClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var ro = new ReferenceObject {ReferenceOne = so};

            var clone = (ReferenceObject) CopyUtils.CloneObjectTest(ro);

            Assert.AreNotSame(ro, clone);
            Assert.AreSame(ro.GetType(), clone.GetType());

            Assert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
        }

        [Test]
        public void TestReferenceUpdate() {
            var so1 = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var so2 = new SimpleObject {ValueOne = 3, ValueTwo = 4};
            var ro = new ReferenceObject {ReferenceOne = so1};

            var clone = (ReferenceObject) CopyUtils.CloneObjectTest(ro);
            clone.ReferenceOne = so2;

            Assert.AreNotSame(ro, clone);
            Assert.AreSame(ro.GetType(), clone.GetType());
            Assert.AreNotSame(ro.ReferenceOne, clone.ReferenceOne);

            CopyUtils.UpdateFromClone(ro, clone);

            Assert.AreNotSame(ro, clone);
            Assert.AreSame(ro.GetType(), clone.GetType());
            Assert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
        }

        [Test]
        public void TestSimpleClone() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var clone = (SimpleObject) CopyUtils.CloneObjectTest(so);

            Assert.AreNotSame(so, clone);
            Assert.AreSame(so.GetType(), clone.GetType());
            Assert.AreEqual(so.ValueOne, clone.ValueOne);
            Assert.AreEqual(so.ValueTwo, clone.ValueTwo);
        }

        [Test]
        public void TestSimpleUpdate() {
            var so = new SimpleObject {ValueOne = 1, ValueTwo = 2};
            var clone = (SimpleObject) CopyUtils.CloneObjectTest(so);
            clone.ValueTwo = 3;

            Assert.AreNotSame(so, clone);
            Assert.AreSame(so.GetType(), clone.GetType());
            Assert.AreEqual(so.ValueOne, clone.ValueOne);
            Assert.AreNotEqual(so.ValueTwo, clone.ValueTwo);

            CopyUtils.UpdateFromClone(so, clone);

            Assert.AreNotSame(so, clone);
            Assert.AreSame(so.GetType(), clone.GetType());
            Assert.AreEqual(so.ValueOne, clone.ValueOne);
            Assert.AreEqual(so.ValueTwo, clone.ValueTwo);
        }

        #region Nested type: AllObject

        public class AllObject {
            public int ValueOne { get; set; }
            public int ValueTwo { get; set; }
            public SimpleObject ReferenceOne { get; set; }

            public ICollection<SimpleObject> CollectionOne { get; set; } = new List<SimpleObject>();
        }

        #endregion

        #region Nested type: CollectionObject

        public class CollectionObject {
            public ICollection<SimpleObject> CollectionOne { get; set; } = new List<SimpleObject>();
        }

        #endregion

        #region Nested type: ReferenceObject

        public class ReferenceObject {
            public SimpleObject ReferenceOne { get; set; }
        }

        #endregion

        #region Nested type: SimpleObject

        public class SimpleObject {
            public int ValueOne { get; set; }
            public int ValueTwo { get; set; }
        }

        #endregion
    }
}