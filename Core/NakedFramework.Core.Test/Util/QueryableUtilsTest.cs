// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NUnit.Framework;
using NAssert = NUnit.Framework.Assert;

namespace NakedObjects.Core.Util.Query {
    [TestFixture]
    public class QueryableUtilsTest {
        [Test]
        public void TestCount() {
            var list = Enumerable.Range(0, 100);
            var queryable = (IQueryable) list.AsQueryable();

            NAssert.AreEqual(100, queryable.Count());
        }

        [Test]
        public void TestFirst() {
            var list = Enumerable.Range(0, 100);
            var queryable = (IQueryable) list.AsQueryable();

            NAssert.AreEqual(0, queryable.First());
        }

        [Test]
        public void TestTake() {
            var list = Enumerable.Range(0, 100);
            var queryable = (IQueryable) list.AsQueryable();

            NAssert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(queryable.Take(9).Cast<object>().ToArray()));
            NAssert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(queryable.Take(9).Cast<object>().ToList().Cast<object>()));
        }

        [Test]
        public void TestSkip() {
            var list = Enumerable.Range(10, 100);
            var queryable = (IQueryable) list.AsQueryable();

            NAssert.AreEqual(47, queryable.Skip(37).First());
        }

        [Test]
        public void TestContains() {
            var list = Enumerable.Range(10, 100);
            var queryable = (IQueryable) list.AsQueryable();

            NAssert.IsTrue(queryable.Contains(49));
            NAssert.IsFalse(queryable.Contains(201));
        }

        [Test]
        // ReSharper disable PossibleMultipleEnumeration
        public void TestIsOrdered() {
            var list = Enumerable.Range(10, 100);

            var notOrdered = list.AsQueryable();
            var simpleOrdered = notOrdered.OrderBy(i => "");
            var thenByOrdered = notOrdered.OrderBy(i => "").ThenBy(i => "");
            var complexOrdered1 = notOrdered.OrderBy(i => "").Select(i => i.ToString());
            var complexOrdered2 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));
            var complexOrdered3 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).OfType<string>().Select(s => int.Parse(s));
            var complexOrdered4 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).OfType<string>().Select(s => int.Parse(s)).Where(i => i > 50);
            var distinctNotOrdered = list.AsQueryable().Distinct();
            var distinctNotOrdered1 = list.AsQueryable().OrderBy(i => "").Distinct();
            var distinctNotOrdered2 = list.AsQueryable().OrderBy(i => "").Distinct().Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));
            var distinctOrdered = list.AsQueryable().Distinct().OrderBy(i => "");
            var distinctOrdered1 = list.AsQueryable().OrderBy(i => "").Distinct().OrderBy(i => "");
            var distinctOrdered2 = list.AsQueryable().Distinct().OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));

            NAssert.IsFalse(notOrdered.IsOrdered());
            NAssert.IsTrue(simpleOrdered.IsOrdered());
            NAssert.IsTrue(thenByOrdered.IsOrdered());
            NAssert.IsTrue(complexOrdered1.IsOrdered());
            NAssert.IsTrue(complexOrdered2.IsOrdered());
            NAssert.IsTrue(complexOrdered3.IsOrdered());
            NAssert.IsTrue(complexOrdered4.IsOrdered());

            NAssert.IsFalse(distinctNotOrdered.IsOrdered());
            NAssert.IsFalse(distinctNotOrdered1.IsOrdered());
            NAssert.IsFalse(distinctNotOrdered2.IsOrdered());

            NAssert.IsTrue(distinctOrdered.IsOrdered());
            NAssert.IsTrue(distinctOrdered1.IsOrdered());
            NAssert.IsTrue(distinctOrdered2.IsOrdered());
        }

        // ReSharper restore PossibleMultipleEnumeration
    }
}