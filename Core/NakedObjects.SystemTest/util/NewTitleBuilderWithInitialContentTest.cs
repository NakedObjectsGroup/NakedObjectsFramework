//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System.Globalization;
//using System.Threading;
//using NUnit.Framework;

//namespace NakedObjects.SystemTest.Util {
//#pragma warning disable 618
//    [TestFixture]
//    public class NewTitleBuilderWithInitialContentTest {
//        [SetUp]
//        public void NewBuilder() {
//            culture = Thread.CurrentThread.CurrentCulture;
//            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
//            builder = new NewTitleBuilder("Text");
//        }

//        [TearDown]
//        public void Cleanup() {
//            Thread.CurrentThread.CurrentCulture = culture;
//        }

//        private NewTitleBuilder builder;
//        private CultureInfo culture;

//        private void AssertTitleIs(string expected, NewTitleBuilder builder) {
//            Assert.AreEqual(expected, builder.ToString());
//        }

//        [Test]
//        public void AppendAndConcatOnlyIncludesSpaceOnAppend() {
//            builder.Append("Test");
//            builder.Concat("ing");
//            AssertTitleIs("Text Testing", builder);
//        }

//        [Test]
//        public void AppendNullLeavesTextUnmodified() {
//            builder.Append(null);
//            AssertTitleIs("Text", builder);
//        }

//        [Test]
//        public void AppendNumberIncludesSpace() {
//            builder.Append(4.5);
//            AssertTitleIs("Text 4.5", builder);
//        }

//        // Format

//        [Test]
//        public void AppendNumberWithFormat() {
//            builder.Append(4.5).Format("00.00");
//            AssertTitleIs("Text 04.50", builder);
//        }

//        [Test]
//        public void AppendStringIncludesSpace() {
//            builder.Append("string");
//            AssertTitleIs("Text string", builder);
//        }

//        [Test]
//        public void ConcatNullLeaveTextUnmodified() {
//            builder.Concat(null);
//            AssertTitleIs("Text", builder);
//        }

//        [Test]
//        public void ConcatNumber() {
//            builder.Concat(4.5);
//            AssertTitleIs("Text4.5", builder);
//        }

//        [Test]
//        public void ConcatString() {
//            builder.Concat("ing");
//            AssertTitleIs("Texting", builder);
//        }

//        [Test]
//        public void ConcatTwice() {
//            builder.Concat("ing");
//            builder.Concat("123");
//            AssertTitleIs("Texting123", builder);
//        }

//        [Test]
//        public void ContainsConstructorString() {
//            AssertTitleIs("Text", builder);
//        }

//        [Test]
//        public void DefaultIgnoredWhenNotNull() {
//            builder.Default("zero");
//            AssertTitleIs("Text", builder);
//        }

//        [Test]
//        public void JoinerUsedWhenTextExists() {
//            builder.Separator(",");
//            builder.Append("test");
//            AssertTitleIs("Text, test", builder);
//        }

//        [Test]
//        public void TestTruncateDoesNothingOnShortString() {
//            builder.Truncate(13, false, "...");
//            AssertTitleIs("Text", builder);
//        }

//        [Test]
//        public void TestTruncateOnlyTruncatesLatestEntry() {
//            builder.Append("The quick brown fox jumped");
//            builder.Truncate(13, false, "...");
//            AssertTitleIs("Text The quick...", builder);
//        }
//    }
//#pragma warning restore 618
//}