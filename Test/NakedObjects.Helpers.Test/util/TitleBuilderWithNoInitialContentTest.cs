// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects {
    [TestClass]
    public class TitleBuilderWithNoIntialContentTest {
        private TitleBuilder builder;

        [TestInitialize]
        public void NewBuilder() {
            builder = new TitleBuilder();
        }

        private void AssertTitleIs(string expected) {
            Assert.AreEqual(expected, builder.ToString());
        }

        [TestMethod]
        public void TestNewBuilderContainsUnmodifiedText() {
            AssertTitleIs("");
        }


        [TestMethod]
        public void TestConcatAddsText() {
            builder.Concat("added");
            AssertTitleIs("added");
        }

        [TestMethod]
        public void TestConcatNoJoiner() {
            builder.Concat(":", null, "d", null);
            AssertTitleIs("");
        }

        [TestMethod]
        public void TestConcatAddsTextWithoutJoiner() {
            builder.Concat("+", "added");
            AssertTitleIs("added");
        }

        [TestMethod]
        public void TestAppendNullAddsNoTextAndNoSpace() {
            builder.Append(null);
            AssertTitleIs("");
        }

        [TestMethod]
        public void TestAppendFormatWithDefault() {
            builder.Append(":", null, "d", "no date");
            AssertTitleIs("no date");
        }

        [TestMethod]
        public void TestAppendFormat() {
            builder.Append(new DateTime(2007, 4, 2), "d", null);
            AssertTitleIs("02/04/2007");
        }


        [TestMethod]
        public void TestAppendWithJoiner() {
            builder.Append(1).Append("~", 2).Append(" !", 3);
            AssertTitleIs("1~ 2 ! 3");
        }

        [TestMethod]
        public void TestJoinerOmmittedIfNullIsAppended() {
            builder.Append(1).Append("~", null);
            AssertTitleIs("1");
        }

        [TestMethod]
        public void TestJoinerOmmittedIfBeingAppendedToNothing() {
            builder.Append("~", 1);
            AssertTitleIs("1");
        }

        [TestMethod]
        public void TestSecondNullDoesNotNegateFirstOne() {
            builder.Append(1).Append("~", 2).Append(" !", null);
            AssertTitleIs("1~ 2");
        }

        [TestMethod]
        public void EnumAsArgumentWithSingleWord() {
            var t = new TitleBuilder();
            t.Append(Sex.Female);

            Assert.AreEqual("Female", t.ToString());
        }

        [TestMethod]
        public void EnumAsArgumentWithMultipleWordsFormatted() {
            var t = new TitleBuilder();
            t.Append(Sex.NotSpecified);

            Assert.AreEqual("Not Specified", t.ToString());
        }
    }

    internal enum Sex {
        Male = 1,
        Female = 2,
        Unknown = 3,
        NotSpecified = 4
    }
}