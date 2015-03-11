// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Util;
using NUnit.Framework;

namespace NakedObjects.Core.util {
    [TestFixture]
    public class QueryableUtilsTest {
        [Test]
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

            Assert.IsFalse(notOrdered.IsOrdered());
            Assert.IsTrue(simpleOrdered.IsOrdered());
            Assert.IsTrue(thenByOrdered.IsOrdered());
            Assert.IsTrue(complexOrdered1.IsOrdered());
            Assert.IsTrue(complexOrdered2.IsOrdered());
            Assert.IsTrue(complexOrdered3.IsOrdered());
            Assert.IsTrue(complexOrdered4.IsOrdered());

            Assert.IsFalse(distinctNotOrdered.IsOrdered());
            Assert.IsFalse(distinctNotOrdered1.IsOrdered());
            Assert.IsFalse(distinctNotOrdered2.IsOrdered());

            Assert.IsTrue(distinctOrdered.IsOrdered());
            Assert.IsTrue(distinctOrdered1.IsOrdered());
            Assert.IsTrue(distinctOrdered2.IsOrdered());
        }
    }
}