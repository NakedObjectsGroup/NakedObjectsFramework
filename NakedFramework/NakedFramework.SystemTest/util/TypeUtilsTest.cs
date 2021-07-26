// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Util {
    [TestFixture]
    public class TypeUtilsTest {
        [Test]
        public void TestMatch() {
            var m = new Match();
            Assert.IsTrue(m.IsPropertyMatch("Prop1", mm => mm.Prop1));
            Assert.IsTrue(m.IsPropertyMatch("Prop2", mm => mm.Prop2));
            Assert.IsTrue(m.IsPropertyMatch("Prop3", mm => mm.Prop3));
            Assert.IsTrue(m.IsPropertyMatch("Prop4", mm => mm.Prop4));

            Assert.IsFalse(m.IsPropertyMatch("Prop2", mm => mm.Prop1));
            Assert.IsFalse(m.IsPropertyMatch("Something", mm => mm.Prop2));
            Assert.IsFalse(m.IsPropertyMatch("", mm => mm.Prop2));
            Assert.IsFalse(m.IsPropertyMatch(null, mm => mm.Prop2));
        }

        [Test]
        public void TestMatchObject() {
            var o = new Match() as object;

            Assert.IsTrue(o.IsPropertyMatch<Match, string>("Prop1", mm => mm.Prop1));
            Assert.IsTrue(o.IsPropertyMatch<Match, int>("Prop2", mm => mm.Prop2));
            Assert.IsTrue(o.IsPropertyMatch<Match, Match>("Prop3", mm => mm.Prop3));
            Assert.IsTrue(o.IsPropertyMatch<Match, int?>("Prop4", mm => mm.Prop4));

            Assert.IsFalse(o.IsPropertyMatch<Match, string>("Prop2", mm => mm.Prop1));
            Assert.IsFalse(o.IsPropertyMatch<Match, int>("Something", mm => mm.Prop2));
            Assert.IsFalse(o.IsPropertyMatch<Match, int>("", mm => mm.Prop2));
            Assert.IsFalse(o.IsPropertyMatch<Match, int>(null, mm => mm.Prop2));

            var oo = new object();

            Assert.IsFalse(oo.IsPropertyMatch<Match, string>("Prop1", mm => mm.Prop1));
            Assert.IsFalse(oo.IsPropertyMatch<Match, int>("Prop2", mm => mm.Prop2));
            Assert.IsFalse(oo.IsPropertyMatch<Match, Match>("Prop3", mm => mm.Prop3));
            Assert.IsFalse(oo.IsPropertyMatch<Match, int?>("Prop4", mm => mm.Prop4));

            Assert.IsFalse(oo.IsPropertyMatch<Match, string>("Prop2", mm => mm.Prop1));
            Assert.IsFalse(oo.IsPropertyMatch<Match, int>("Something", mm => mm.Prop2));
            Assert.IsFalse(oo.IsPropertyMatch<Match, int>("", mm => mm.Prop2));
            Assert.IsFalse(oo.IsPropertyMatch<Match, int>(null, mm => mm.Prop2));
        }

        public class Match {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public Match Prop3 { get; set; }
            public int? Prop4 { get; set; }
        }
    }
}