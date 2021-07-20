// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Core.Util;
using NUnit.Framework;

namespace NakedFramework.Core.Test.Util {
    [TestFixture]

    // cast ensures correct extension is called 
    // ReSharper disable RedundantCast
    public class EnumerableUtilsTest {
        [Test]
        public void TestCount() {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(0, 100));

            Assert.AreEqual(100, ((IEnumerable) list).Count());
        }

        [Test]
        public void TestFirst() {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(0, 100));

            Assert.AreEqual(0, ((IEnumerable) list).First());
        }

        [Test]
        public void TestTake() {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(0, 100));

            Assert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(((IEnumerable) list).Take(9).Cast<object>().ToArray()));
            Assert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(((IEnumerable) list).Take(9).Cast<object>().ToList()));
        }

        [Test]
        public void TestSkip() {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(10, 100));

            Assert.AreEqual(47, ((IEnumerable) list).Skip(37).First());
        }

        [Test]
        public void TestContains() {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(10, 100));

            Assert.IsTrue(((IEnumerable) list).Contains(49));
            Assert.IsFalse(((IEnumerable) list).Contains(201));
        }
    }

    // ReSharper restore RedundantCast
}