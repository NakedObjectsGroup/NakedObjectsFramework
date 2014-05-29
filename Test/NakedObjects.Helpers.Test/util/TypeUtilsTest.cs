// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Util;

namespace NakedObjects {
    [TestClass]
    public class TypeUtilsTest {


        [TestMethod]
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

        [TestMethod]
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

            var oo = new Object();

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