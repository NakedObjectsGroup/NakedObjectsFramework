// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFramework.Core.Util;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NakedFramework.Core.Test.Util;

[TestFixture]
public class CopyUtilsTest {
    [Test]
    public void TestAllClone() {
        var so1 = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var so2 = new SimpleObject { ValueOne = 3, ValueTwo = 4 };
        var ao = new AllObject { ValueOne = 5, ValueTwo = 6, ReferenceOne = so1 };
        ao.CollectionOne.Add(so2);

        var clone = (AllObject)CopyUtils.CloneObjectTest(ao);
        ClassicAssert.AreNotSame(ao, clone);
        ClassicAssert.AreSame(ao.GetType(), clone.GetType());
        ClassicAssert.AreEqual(ao.ValueOne, clone.ValueOne);
        ClassicAssert.AreEqual(ao.ValueTwo, clone.ValueTwo);
        ClassicAssert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
        ClassicAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
    }

    [Test]
    public void TestAllUpdate() {
        var so1 = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var so2 = new SimpleObject { ValueOne = 3, ValueTwo = 4 };
        var so3 = new SimpleObject { ValueOne = 5, ValueTwo = 6 };
        var so4 = new SimpleObject { ValueOne = 7, ValueTwo = 8 };
        var ao = new AllObject { ValueOne = 9, ValueTwo = 10, ReferenceOne = so1 };
        ao.CollectionOne.Add(so2);

        var clone = (AllObject)CopyUtils.CloneObjectTest(ao);

        clone.ValueTwo = 11;
        clone.ReferenceOne = so3;
        clone.CollectionOne.Add(so4);

        ClassicAssert.AreNotSame(ao, clone);
        ClassicAssert.AreSame(ao.GetType(), clone.GetType());
        ClassicAssert.AreEqual(ao.ValueOne, clone.ValueOne);
        ClassicAssert.AreNotEqual(ao.ValueTwo, clone.ValueTwo);
        ClassicAssert.AreNotSame(ao.ReferenceOne, clone.ReferenceOne);
        ClassicAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreNotEqual(ao.CollectionOne.Count, clone.CollectionOne.Count);

        CopyUtils.UpdateFromClone(ao, clone);

        ClassicAssert.AreNotSame(ao, clone);
        ClassicAssert.AreSame(ao.GetType(), clone.GetType());
        ClassicAssert.AreNotSame(ao.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreSame(ao.ReferenceOne, clone.ReferenceOne);
        ClassicAssert.AreEqual(ao.ValueOne, clone.ValueOne);
        ClassicAssert.AreEqual(ao.ValueTwo, clone.ValueTwo);
        ClassicAssert.AreEqual(ao.CollectionOne.Count, clone.CollectionOne.Count);
        ClassicAssert.AreSame(ao.CollectionOne.First(), clone.CollectionOne.First());
        ClassicAssert.AreSame(ao.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
    }

    [Test]
    public void TestCollectionClone() {
        var so = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var co = new CollectionObject();
        co.CollectionOne.Add(so);

        var clone = (CollectionObject)CopyUtils.CloneObjectTest(co);
        ClassicAssert.AreNotSame(co, clone);
        ClassicAssert.AreSame(co.GetType(), clone.GetType());
        ClassicAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
    }

    [Test]
    public void TestCollectionUpdate() {
        var so1 = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var so2 = new SimpleObject { ValueOne = 3, ValueTwo = 4 };
        var co = new CollectionObject();
        co.CollectionOne.Add(so1);

        var clone = (CollectionObject)CopyUtils.CloneObjectTest(co);

        clone.CollectionOne.Add(so2);

        ClassicAssert.AreNotSame(co, clone);
        ClassicAssert.AreSame(co.GetType(), clone.GetType());
        ClassicAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreNotEqual(co.CollectionOne.Count, clone.CollectionOne.Count);

        CopyUtils.UpdateFromClone(co, clone);

        ClassicAssert.AreNotSame(co, clone);
        ClassicAssert.AreSame(co.GetType(), clone.GetType());
        ClassicAssert.AreNotSame(co.CollectionOne, clone.CollectionOne);
        ClassicAssert.AreEqual(co.CollectionOne.Count, clone.CollectionOne.Count);
        ClassicAssert.AreSame(co.CollectionOne.First(), clone.CollectionOne.First());
        ClassicAssert.AreSame(co.CollectionOne.ElementAt(1), clone.CollectionOne.ElementAt(1));
    }

    [Test]
    public void TestReferenceClone() {
        var so = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var ro = new ReferenceObject { ReferenceOne = so };

        var clone = (ReferenceObject)CopyUtils.CloneObjectTest(ro);

        ClassicAssert.AreNotSame(ro, clone);
        ClassicAssert.AreSame(ro.GetType(), clone.GetType());

        ClassicAssert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
    }

    [Test]
    public void TestReferenceUpdate() {
        var so1 = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var so2 = new SimpleObject { ValueOne = 3, ValueTwo = 4 };
        var ro = new ReferenceObject { ReferenceOne = so1 };

        var clone = (ReferenceObject)CopyUtils.CloneObjectTest(ro);
        clone.ReferenceOne = so2;

        ClassicAssert.AreNotSame(ro, clone);
        ClassicAssert.AreSame(ro.GetType(), clone.GetType());
        ClassicAssert.AreNotSame(ro.ReferenceOne, clone.ReferenceOne);

        CopyUtils.UpdateFromClone(ro, clone);

        ClassicAssert.AreNotSame(ro, clone);
        ClassicAssert.AreSame(ro.GetType(), clone.GetType());
        ClassicAssert.AreSame(ro.ReferenceOne, clone.ReferenceOne);
    }

    [Test]
    public void TestSimpleClone() {
        var so = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var clone = (SimpleObject)CopyUtils.CloneObjectTest(so);

        ClassicAssert.AreNotSame(so, clone);
        ClassicAssert.AreSame(so.GetType(), clone.GetType());
        ClassicAssert.AreEqual(so.ValueOne, clone.ValueOne);
        ClassicAssert.AreEqual(so.ValueTwo, clone.ValueTwo);
    }

    [Test]
    public void TestSimpleUpdate() {
        var so = new SimpleObject { ValueOne = 1, ValueTwo = 2 };
        var clone = (SimpleObject)CopyUtils.CloneObjectTest(so);
        clone.ValueTwo = 3;

        ClassicAssert.AreNotSame(so, clone);
        ClassicAssert.AreSame(so.GetType(), clone.GetType());
        ClassicAssert.AreEqual(so.ValueOne, clone.ValueOne);
        ClassicAssert.AreNotEqual(so.ValueTwo, clone.ValueTwo);

        CopyUtils.UpdateFromClone(so, clone);

        ClassicAssert.AreNotSame(so, clone);
        ClassicAssert.AreSame(so.GetType(), clone.GetType());
        ClassicAssert.AreEqual(so.ValueOne, clone.ValueOne);
        ClassicAssert.AreEqual(so.ValueTwo, clone.ValueTwo);
    }

    public class AllObject {
        public int ValueOne { get; set; }
        public int ValueTwo { get; set; }
        public SimpleObject ReferenceOne { get; set; }

        public ICollection<SimpleObject> CollectionOne { get; set; } = new List<SimpleObject>();
    }

    public class CollectionObject {
        public ICollection<SimpleObject> CollectionOne { get; set; } = new List<SimpleObject>();
    }

    public class ReferenceObject {
        public SimpleObject ReferenceOne { get; set; }
    }

    public class SimpleObject {
        public int ValueOne { get; set; }
        public int ValueTwo { get; set; }
    }
}