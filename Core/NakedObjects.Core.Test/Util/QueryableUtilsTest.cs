//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace NakedObjects.Core.Util.Query {
//    [TestClass]
//    public class QueryableUtilsTest {
//        [TestMethod]
//        public void TestCount() {
//            var list = Enumerable.Range(0, 100);
//            var queryable = (IQueryable) list.AsQueryable();

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(100, queryable.Count());
//        }

//        [TestMethod]
//        public void TestFirst() {
//            var list = Enumerable.Range(0, 100);
//            var queryable = (IQueryable) list.AsQueryable();

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, queryable.First());
//        }

//        [TestMethod]
//        public void TestTake() {
//            var list = Enumerable.Range(0, 100);
//            var queryable = (IQueryable) list.AsQueryable();

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(queryable.Take(9).Cast<object>().ToArray()));
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Enumerable.Range(0, 9).Cast<object>().SequenceEqual(queryable.Take(9).Cast<object>().ToList().Cast<object>()));
//        }

//        [TestMethod]
//        public void TestSkip() {
//            var list = Enumerable.Range(10, 100);
//            var queryable = (IQueryable) list.AsQueryable();

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(47, queryable.Skip(37).First());
//        }

//        [TestMethod]
//        public void TestContains() {
//            var list = Enumerable.Range(10, 100);
//            var queryable = (IQueryable) list.AsQueryable();

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(queryable.Contains(49));
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(queryable.Contains(201));
//        }

//        [TestMethod]
//        // ReSharper disable PossibleMultipleEnumeration
//        public void TestIsOrdered() {
//            var list = Enumerable.Range(10, 100);

//            var notOrdered = list.AsQueryable();
//            var simpleOrdered = notOrdered.OrderBy(i => "");
//            var thenByOrdered = notOrdered.OrderBy(i => "").ThenBy(i => "");
//            var complexOrdered1 = notOrdered.OrderBy(i => "").Select(i => i.ToString());
//            var complexOrdered2 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));
//            var complexOrdered3 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).OfType<string>().Select(s => int.Parse(s));
//            var complexOrdered4 = notOrdered.OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).OfType<string>().Select(s => int.Parse(s)).Where(i => i > 50);
//            var distinctNotOrdered = list.AsQueryable().Distinct();
//            var distinctNotOrdered1 = list.AsQueryable().OrderBy(i => "").Distinct();
//            var distinctNotOrdered2 = list.AsQueryable().OrderBy(i => "").Distinct().Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));
//            var distinctOrdered = list.AsQueryable().Distinct().OrderBy(i => "");
//            var distinctOrdered1 = list.AsQueryable().OrderBy(i => "").Distinct().OrderBy(i => "");
//            var distinctOrdered2 = list.AsQueryable().Distinct().OrderBy(i => "").Select(i => i.ToString()).Where(s => s.Length > 0).Select(s => int.Parse(s));

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(notOrdered.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(simpleOrdered.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(thenByOrdered.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(complexOrdered1.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(complexOrdered2.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(complexOrdered3.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(complexOrdered4.IsOrdered());

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(distinctNotOrdered.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(distinctNotOrdered1.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(distinctNotOrdered2.IsOrdered());

//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(distinctOrdered.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(distinctOrdered1.IsOrdered());
//            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(distinctOrdered2.IsOrdered());
//        }

//        // ReSharper restore PossibleMultipleEnumeration
//    }
//}