// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Core.Persist;
using NUnit.Framework;

namespace NakedObjects.Core {
    [TestFixture]
    public class SerialOidTest {
        [Test]
        public void TestEquals() {
            SerialOid oid1 = SerialOid.CreateTransient(123, typeof(object).FullName);
            SerialOid oid2 = SerialOid.CreateTransient(123, typeof(object).FullName);
            SerialOid oid3 = SerialOid.CreateTransient(321, typeof(object).FullName);
            SerialOid oid4 = SerialOid.CreatePersistent(321, typeof(object).FullName);
            SerialOid oid5 = SerialOid.CreatePersistent(456, typeof(object).FullName);
            SerialOid oid6 = SerialOid.CreatePersistent(456, typeof(object).FullName);

            Assert.IsTrue(oid1.Equals(oid2));
            Assert.IsTrue(oid2.Equals(oid1));

            Assert.IsFalse(oid1.Equals(oid3));
            Assert.IsFalse(oid3.Equals(oid1));

            Assert.IsTrue(oid5.Equals(oid6));
            Assert.IsTrue(oid6.Equals(oid5));

            Assert.IsFalse(oid4.Equals(oid5));
            Assert.IsFalse(oid5.Equals(oid4));

            Assert.IsFalse(oid3.Equals((Object) oid4));
            Assert.IsFalse(oid4.Equals((Object) oid3));

            Assert.IsFalse(oid3.Equals(oid4));
            Assert.IsFalse(oid4.Equals(oid3));
        }
    }
}