// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects {
    [TestClass]
    public class NewTitleBuilderWithNoInitialContentTest {
        private NewTitleBuilder initiallyEmptyBuilder;

        [TestInitialize]
        public void NewBuilder() {
            initiallyEmptyBuilder = new NewTitleBuilder();
        }

        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
            Assert.AreEqual(expected, builder.ToString());
        }

        [TestMethod]
        public void ContainsEmptyString() {
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        // Concat

        [TestMethod]
        public void ConcatNullLeaveTextUnmodified() {
            initiallyEmptyBuilder.Concat(null);
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void ConcatString() {
            initiallyEmptyBuilder.Concat("Text");
            AssertTitleIs("Text", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void ConcatTwice() {
            initiallyEmptyBuilder.Concat("Text");
            initiallyEmptyBuilder.Concat("ing");
            AssertTitleIs("Texting", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void ConcatNumber() {
            initiallyEmptyBuilder.Concat(4.5);
            AssertTitleIs("4.5", initiallyEmptyBuilder);
        }


        // Appends

        [TestMethod]
        public void AppendNullLeavesTextUnmodified() {
            initiallyEmptyBuilder.Append(null);
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void AppendStringIncludesNoSpace() {
            initiallyEmptyBuilder.Append("string");
            AssertTitleIs("string", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void AppendNumberIncludesNoSpace() {
            initiallyEmptyBuilder.Append(4.5);
            AssertTitleIs("4.5", initiallyEmptyBuilder);
        }


        // Format

        [TestMethod]
        public void ConcatNumberWithFormat() {
            initiallyEmptyBuilder.Concat(4.5).Format("00.00");
            AssertTitleIs("04.50", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void AppendNumbersWithAndWithFormats() {
            initiallyEmptyBuilder.Append(4.5).Format("00.00");
            initiallyEmptyBuilder.Append(90);
            AssertTitleIs("04.50 90", initiallyEmptyBuilder);
        }

        // default
        [TestMethod]
        public void DefaultAppliedWhenNull() {
            initiallyEmptyBuilder.Default("zero");
            AssertTitleIs("zero", initiallyEmptyBuilder);
        }

        // joiner
        [TestMethod]
        public void JoinerIgnoredWhenNoExistingText() {
            initiallyEmptyBuilder.Append("test");
            initiallyEmptyBuilder.Separator(",");
            AssertTitleIs("test", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void Compound() {
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append("Order");
            initiallyEmptyBuilder.Append(34).Format("0000");
            initiallyEmptyBuilder.Separator(" -").Append(date).Format("d");
            //   builder.Default("no date");
            AssertTitleIs("Order 0034 - 01/04/2007", initiallyEmptyBuilder);
        }

        // truncate

        [TestMethod]
        public void TestTruncate() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(14);
            AssertTitleIs("The quick brow", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void TestTruncateWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(16, false, "...");
            AssertTitleIs("The quick bro...", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void TestTruncateRemovesSpace() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(10);
            AssertTitleIs("The quick", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void TestTruncateRemovesSpaceWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(13, false, "...");
            AssertTitleIs("The quick...", initiallyEmptyBuilder);
        }


        [TestMethod]
        public void TestTruncateToWordBoundary() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(18, true);
            AssertTitleIs("The quick brown", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void TestTruncateToWordBoundaryWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(18, true, "...");
            AssertTitleIs("The quick...", initiallyEmptyBuilder);
        }

        // complex

        [TestMethod]
        public void Compound3() {
            var customer = new Customer();
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Concat(date).Format("d");
            AssertTitleIs("Harry Smith - 01/04/2007", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void Compound4() {
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(null).Separator(" - ").Concat(date).Format("d");
            AssertTitleIs("01/04/2007", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void Compound5() {
            var customer = new Customer();
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(null);
            AssertTitleIs("Harry Smith", initiallyEmptyBuilder);
        }

        [TestMethod]
        public void Compound6() {
            var customer = new Customer();
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(customer).Separator("!").Append(null);
            AssertTitleIs("Harry Smith -  Harry Smith", initiallyEmptyBuilder);
        }
    }

    public class Customer {
        public String Title() {
            return "Harry Smith";
        }
    }
}