// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Util {
#pragma warning disable 618
    [TestFixture]
    public class NewTitleBuilderWithNoInitialContentTest {
        [SetUp]
        public void NewBuilder() {
            culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            initiallyEmptyBuilder = new NewTitleBuilder();
        }

        [TearDown]
        public void Cleanup() {
            Thread.CurrentThread.CurrentCulture = culture;
        }

        private CultureInfo culture;
        private NewTitleBuilder initiallyEmptyBuilder;

        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
            Assert.AreEqual(expected, builder.ToString());
        }

        // Appends

        [Test]
        public void AppendNullLeavesTextUnmodified() {
            initiallyEmptyBuilder.Append(null);
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        [Test]
        public void AppendNumberIncludesNoSpace() {
            initiallyEmptyBuilder.Append(4.5);
            AssertTitleIs("4.5", initiallyEmptyBuilder);
        }

        [Test]
        public void AppendNumbersWithAndWithFormats() {
            initiallyEmptyBuilder.Append(4.5).Format("00.00");
            initiallyEmptyBuilder.Append(90);
            AssertTitleIs("04.50 90", initiallyEmptyBuilder);
        }

        [Test]
        public void AppendStringIncludesNoSpace() {
            initiallyEmptyBuilder.Append("string");
            AssertTitleIs("string", initiallyEmptyBuilder);
        }

        [Test]
        public void Compound() {
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append("Order");
            initiallyEmptyBuilder.Append(34).Format("0000");
            initiallyEmptyBuilder.Separator(" -").Append(date).Format("d");
            //   builder.Default("no date");
            AssertTitleIs("Order 0034 - 04/01/2007", initiallyEmptyBuilder);
        }

        // complex

        [Test]
        public void Compound3() {
            var customer = new Customer();
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Concat(date).Format("d");
            AssertTitleIs("Harry Smith - 04/01/2007", initiallyEmptyBuilder);
        }

        [Test]
        public void Compound4() {
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(null).Separator(" - ").Concat(date).Format("d");
            AssertTitleIs("04/01/2007", initiallyEmptyBuilder);
        }

        [Test]
        public void Compound5() {
            var customer = new Customer();
            var date = new DateTime(2007, 4, 1);
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(null);
            AssertTitleIs("Harry Smith", initiallyEmptyBuilder);
        }

        [Test]
        public void Compound6() {
            var customer = new Customer();
            initiallyEmptyBuilder.Append(customer).Separator(" - ").Append(customer).Separator("!").Append(null);
            AssertTitleIs("Harry Smith -  Harry Smith", initiallyEmptyBuilder);
        }

        // Concat

        [Test]
        public void ConcatNullLeaveTextUnmodified() {
            initiallyEmptyBuilder.Concat(null);
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        [Test]
        public void ConcatNumber() {
            initiallyEmptyBuilder.Concat(4.5);
            AssertTitleIs("4.5", initiallyEmptyBuilder);
        }

        // Format

        [Test]
        public void ConcatNumberWithFormat() {
            initiallyEmptyBuilder.Concat(4.5).Format("00.00");
            AssertTitleIs("04.50", initiallyEmptyBuilder);
        }

        [Test]
        public void ConcatString() {
            initiallyEmptyBuilder.Concat("Text");
            AssertTitleIs("Text", initiallyEmptyBuilder);
        }

        [Test]
        public void ConcatTwice() {
            initiallyEmptyBuilder.Concat("Text");
            initiallyEmptyBuilder.Concat("ing");
            AssertTitleIs("Texting", initiallyEmptyBuilder);
        }

        [Test]
        public void ContainsEmptyString() {
            AssertTitleIs("", initiallyEmptyBuilder);
        }

        // default
        [Test]
        public void DefaultAppliedWhenNull() {
            initiallyEmptyBuilder.Default("zero");
            AssertTitleIs("zero", initiallyEmptyBuilder);
        }

        // joiner
        [Test]
        public void JoinerIgnoredWhenNoExistingText() {
            initiallyEmptyBuilder.Append("test");
            initiallyEmptyBuilder.Separator(",");
            AssertTitleIs("test", initiallyEmptyBuilder);
        }

        // truncate

        [Test]
        public void TestTruncate() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(14);
            AssertTitleIs("The quick brow", initiallyEmptyBuilder);
        }

        [Test]
        public void TestTruncateRemovesSpace() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(10);
            AssertTitleIs("The quick", initiallyEmptyBuilder);
        }

        [Test]
        public void TestTruncateRemovesSpaceWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(13, false, "...");
            AssertTitleIs("The quick...", initiallyEmptyBuilder);
        }

        [Test]
        public void TestTruncateToWordBoundary() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(18, true);
            AssertTitleIs("The quick brown", initiallyEmptyBuilder);
        }

        [Test]
        public void TestTruncateToWordBoundaryWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(18, true, "...");
            AssertTitleIs("The quick...", initiallyEmptyBuilder);
        }

        [Test]
        public void TestTruncateWithContinuation() {
            initiallyEmptyBuilder.Append("The quick brown fox jumped");
            initiallyEmptyBuilder.Truncate(16, false, "...");
            AssertTitleIs("The quick bro...", initiallyEmptyBuilder);
        }
    }

    public class Customer {
        public string Title() => "Harry Smith";
    }
#pragma warning restore 618
}