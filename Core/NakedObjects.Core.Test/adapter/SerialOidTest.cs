// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NUnit.Framework;

namespace NakedObjects.Core.Test.Adapter {
    [TestFixture]
    public class SerialOidTest {
        [Test]
        public void TestEquals() {
            var r = new Mock<IMetamodelManager>().Object;
            SerialOid oid1 = SerialOid.CreateTransient(r, 123, typeof (object).FullName);
            SerialOid oid2 = SerialOid.CreateTransient(r, 123, typeof (object).FullName);
            SerialOid oid3 = SerialOid.CreateTransient(r, 321, typeof (object).FullName);
            SerialOid oid4 = SerialOid.CreatePersistent(r, 321, typeof (object).FullName);
            SerialOid oid5 = SerialOid.CreatePersistent(r, 456, typeof (object).FullName);
            SerialOid oid6 = SerialOid.CreatePersistent(r, 456, typeof (object).FullName);

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