//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System.Globalization;
//using System.Threading;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace NakedObjects.SystemTest.Util {
//#pragma warning disable 618
//    [TestClass]
//    public class NewTitleBuilderWithInitialContentTest {
//        private NewTitleBuilder builder;
//        private CultureInfo culture;

//        [TestInitialize]
//        public void NewBuilder() {
//            culture = Thread.CurrentThread.CurrentCulture;
//            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
//            builder = new NewTitleBuilder("Text");
//        }

//        [TestCleanup]
//        public void Cleanup() {
//            Thread.CurrentThread.CurrentCulture = culture;
//        }

//        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
//            Assert.AreEqual(expected, builder.ToString());
//        }

//        [TestMethod]
//        public void ContainsConstructorString() {
//            AssertTitleIs("Text", builder);
//        }

//        #region Default

//        [TestMethod]
//        public void DefaultIgnoredWhenNotNull() {
//            builder.Default("zero");
//            AssertTitleIs("Text", builder);
//        }

//        #endregion

//        #region Joiner

//        [TestMethod]
//        public void JoinerUsedWhenTextExists() {
//            builder.Separator(",");
//            builder.Append("test");
//            AssertTitleIs("Text, test", builder);
//        }

//        #endregion

//        #region Concat

//        [TestMethod]
//        public void ConcatNullLeaveTextUnmodified() {
//            builder.Concat(null);
//            AssertTitleIs("Text", builder);
//        }

//        [TestMethod]
//        public void ConcatString() {
//            builder.Concat("ing");
//            AssertTitleIs("Texting", builder);
//        }

//        [TestMethod]
//        public void ConcatTwice() {
//            builder.Concat("ing");
//            builder.Concat("123");
//            AssertTitleIs("Texting123", builder);
//        }

//        [TestMethod]
//        public void ConcatNumber() {
//            builder.Concat(4.5);
//            AssertTitleIs("Text4.5", builder);
//        }

//        #endregion

//        #region Append

//        [TestMethod]
//        public void AppendNullLeavesTextUnmodified() {
//            builder.Append(null);
//            AssertTitleIs("Text", builder);
//        }

//        [TestMethod]
//        public void AppendStringIncludesSpace() {
//            builder.Append("string");
//            AssertTitleIs("Text string", builder);
//        }

//        [TestMethod]
//        public void AppendNumberIncludesSpace() {
//            builder.Append(4.5);
//            AssertTitleIs("Text 4.5", builder);
//        }

//        [TestMethod]
//        public void AppendAndConcatOnlyIncludesSpaceOnAppend() {
//            builder.Append("Test");
//            builder.Concat("ing");
//            AssertTitleIs("Text Testing", builder);
//        }

//        // Format

//        [TestMethod]
//        public void AppendNumberWithFormat() {
//            builder.Append(4.5).Format("00.00");
//            AssertTitleIs("Text 04.50", builder);
//        }

//        #endregion

//        #region Truncate

//        [TestMethod]
//        public void TestTruncateDoesNothingOnShortString() {
//            builder.Truncate(13, false, "...");
//            AssertTitleIs("Text", builder);
//        }

//        [TestMethod]
//        public void TestTruncateOnlyTruncatesLatestEntry() {
//            builder.Append("The quick brown fox jumped");
//            builder.Truncate(13, false, "...");
//            AssertTitleIs("Text The quick...", builder);
//        }

//        #endregion
//    }
//#pragma warning restore 618
//}