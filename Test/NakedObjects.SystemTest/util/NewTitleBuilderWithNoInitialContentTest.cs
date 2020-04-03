//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Globalization;
//using System.Threading;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace NakedObjects.SystemTest.Util {
//#pragma warning disable 618
//    [TestClass]
//    public class NewTitleBuilderWithNoInitialContentTest {
//        private NewTitleBuilder initiallyEmptyBuilder;
//        private CultureInfo culture;

//        [TestInitialize]
//        public void NewBuilder() {
//            culture = Thread.CurrentThread.CurrentCulture;
//            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
//            initiallyEmptyBuilder = new NewTitleBuilder();
//        }

//        [TestCleanup]
//        public void Cleanup() {
//            Thread.CurrentThread.CurrentCulture = culture;
//        }

//        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
//            Assert.AreEqual(expected, builder.ToString());
//        }

//        [TestMethod]
//        public void ContainsEmptyString() {
//            AssertTitleIs("", initiallyEmptyBuilder);
//        }

//        // Concat

//        [TestMethod]
//        public void ConcatNullLeaveTextUnmodified() {
//            initiallyEmptyBuilder.Concat(null);
//            AssertTitleIs("", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void ConcatString() {
//            initiallyEmptyBuilder.Concat("Text");
//            AssertTitleIs("Text", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void ConcatTwice() {
//            initiallyEmptyBuilder.Concat("Text");
//            initiallyEmptyBuilder.Concat("ing");
//            AssertTitleIs("Texting", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void ConcatNumber() {
//            initiallyEmptyBuilder.Concat(4.5);
//            AssertTitleIs("4.5", initiallyEmptyBuilder);
//        }

//        // Appends

//        [TestMethod]
//        public void AppendNullLeavesTextUnmodified() {
//            initiallyEmptyBuilder.Append(null);
//            AssertTitleIs("", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void AppendStringIncludesNoSpace() {
//            initiallyEmptyBuilder.Append("string");
//            AssertTitleIs("string", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void AppendNumberIncludesNoSpace() {
//            initiallyEmptyBuilder.Append(4.5);
//            AssertTitleIs("4.5", initiallyEmptyBuilder);
//        }

//        // Format

//        [TestMethod]
//        public void ConcatNumberWithFormat() {
//            initiallyEmptyBuilder.Concat(4.5).Format("00.00");
//            AssertTitleIs("04.50", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void AppendNumbersWithAndWithFormats() {
//            initiallyEmptyBuilder.Append(4.5).Format("00.00");
//            initiallyEmptyBuilder.Append(90);
//            AssertTitleIs("04.50 90", initiallyEmptyBuilder);
//        }

//        // default
//        [TestMethod]
//        public void DefaultAppliedWhenNull() {
//            initiallyEmptyBuilder.Default("zero");
//            AssertTitleIs("zero", initiallyEmptyBuilder);
//        }

//        // joiner
//        [TestMethod]
//        public void JoinerIgnoredWhenNoExistingText() {
//            initiallyEmptyBuilder.Append("test");
//            initiallyEmptyBuilder.Separator(",");
//            AssertTitleIs("test", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void Compound() {
//            var date = new DateTime(2007, 4, 1);
//            initiallyEmptyBuilder.Append("Order");
//            initiallyEmptyBuilder.Append(34).Format("0000");
//            initiallyEmptyBuilder.Separator(" -").Append(date).Format("d");
//            //   builder.Default("no date");
//            AssertTitleIs("Order 0034 - 04/01/2007", initiallyEmptyBuilder);
//        }

//        // truncate

//        [TestMethod]
//        public void TestTruncate() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(14);
//            AssertTitleIs("The quick brow", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void TestTruncateWithContinuation() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(16, false, "...");
//            AssertTitleIs("The quick bro...", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void TestTruncateRemovesSpace() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(10);
//            AssertTitleIs("The quick", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void TestTruncateRemovesSpaceWithContinuation() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(13, false, "...");
//            AssertTitleIs("The quick...", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void TestTruncateToWordBoundary() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(18, true);
//            AssertTitleIs("The quick brown", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void TestTruncateToWordBoundaryWithContinuation() {
//            initiallyEmptyBuilder.Append("The quick brown fox jumped");
//            initiallyEmptyBuilder.Truncate(18, true, "...");
//            AssertTitleIs("The quick...", initiallyEmptyBuilder);
//        }

//        // complex

//        [TestMethod]
//        public void Compound3() {
//            var customer = new Customer();
//            var date = new DateTime(2007, 4, 1);
//            initiallyEmptyBuilder.Append(customer).Separator(" - ").Concat(date).Format("d");
//            AssertTitleIs("Harry Smith - 04/01/2007", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void Compound4() {
//            var date = new DateTime(2007, 4, 1);
//            initiallyEmptyBuilder.Append(null).Separator(" - ").Concat(date).Format("d");
//            AssertTitleIs("04/01/2007", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void Compound5() {
//            var customer = new Customer();
//            var date = new DateTime(2007, 4, 1);
//            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(null);
//            AssertTitleIs("Harry Smith", initiallyEmptyBuilder);
//        }

//        [TestMethod]
//        public void Compound6() {
//            var customer = new Customer();
//            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(customer).Separator("!").Append(null);
//            AssertTitleIs("Harry Smith -  Harry Smith", initiallyEmptyBuilder);
//        }
//    }

//    public class Customer {
//        public String Title() {
//            return "Harry Smith";
//        }
//    }
//#pragma warning restore 618
//}