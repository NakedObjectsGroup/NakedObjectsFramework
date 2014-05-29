// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects {
    [TestClass]
    public class NewTitleBuilderWithInitialContentTest {
        private NewTitleBuilder builder;

        [TestInitialize]
        public void NewBuilder() {
            builder = new NewTitleBuilder("Text");
        }

        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
            Assert.AreEqual(expected, builder.ToString());
        }

        [TestMethod]
        public void ContainsConstructorString() {
            AssertTitleIs("Text", builder);
        }

        #region Concat

        [TestMethod]
        public void ConcatNullLeaveTextUnmodified() {
            builder.Concat(null);
            AssertTitleIs("Text", builder);
        }

        [TestMethod]
        public void ConcatString() {
            builder.Concat("ing");
            AssertTitleIs("Texting", builder);
        }

        [TestMethod]
        public void ConcatTwice() {
            builder.Concat("ing");
            builder.Concat("123");
            AssertTitleIs("Texting123", builder);
        }

        [TestMethod]
        public void ConcatNumber() {
            builder.Concat(4.5);
            AssertTitleIs("Text4.5", builder);
        }

        #endregion

        #region Append

        [TestMethod]
        public void AppendNullLeavesTextUnmodified() {
            builder.Append(null);
            AssertTitleIs("Text", builder);
        }

        [TestMethod]
        public void AppendStringIncludesSpace() {
            builder.Append("string");
            AssertTitleIs("Text string", builder);
        }

        [TestMethod]
        public void AppendNumberIncludesSpace() {
            builder.Append(4.5);
            AssertTitleIs("Text 4.5", builder);
        }

        [TestMethod]
        public void AppendAndConcatOnlyIncludesSpaceOnAppend() {
            builder.Append("Test");
            builder.Concat("ing");
            AssertTitleIs("Text Testing", builder);
        }

        // Format

        [TestMethod]
        public void AppendNumberWithFormat() {
            builder.Append(4.5).Format("00.00");
            AssertTitleIs("Text 04.50", builder);
        }

        #endregion

        #region Default

        [TestMethod]
        public void DefaultIgnoredWhenNotNull() {
            builder.Default("zero");
            AssertTitleIs("Text", builder);
        }

        #endregion

        #region Joiner

        [TestMethod]
        public void JoinerUsedWhenTextExists() {
            builder.Separator(",");
            builder.Append("test");
            AssertTitleIs("Text, test", builder);
        }

        #endregion

        #region Truncate

        [TestMethod]
        public void TestTruncateDoesNothingOnShortString() {
            builder.Truncate(13, false, "...");
            AssertTitleIs("Text", builder);
        }


        [TestMethod]
        public void TestTruncateOnlyTruncatesLatestEntry() {
            builder.Append("The quick brown fox jumped");
            builder.Truncate(13, false, "...");
            AssertTitleIs("Text The quick...", builder);
        }

        #endregion
    }
}