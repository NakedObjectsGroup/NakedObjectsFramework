//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Globalization;
//using System.Threading;
//using NUnit.Framework;

//namespace NakedObjects.SystemTest.Util {
//
//    [TestFixture]
//    public class TitleBuilderWithNoIntialContentTest {
//        [SetUp]
//        public void NewBuilder() {
//            culture = Thread.CurrentThread.CurrentCulture;
//            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
//            builder = new TitleBuilder();
//        }

//        [TearDown]
//        public void Cleanup() {
//            Thread.CurrentThread.CurrentCulture = culture;
//        }

//        private TitleBuilder builder;
//        private CultureInfo culture;

//        private void AssertTitleIs(string expected) {
//            Assert.AreEqual(expected, builder.ToString());
//        }

//        [Test]
//        public void EnumAsArgumentWithMultipleWordsFormatted() {
//            var t = new TitleBuilder();
//            t.Append(Sex.NotSpecified);

//            Assert.AreEqual("Not Specified", t.ToString());
//        }

//        [Test]
//        public void EnumAsArgumentWithSingleWord() {
//            var t = new TitleBuilder();
//            t.Append(Sex.Female);

//            Assert.AreEqual("Female", t.ToString());
//        }

//        [Test]
//        public void TestAppendAddsTextWithJoiner() {
//            builder.Append("+", "1Add");
//            builder.Append("+", "2Add");
//            AssertTitleIs("1Add+ 2Add");
//        }

//        [Test]
//        public void TestAppendFormat() {
//            builder.Append(new DateTime(2007, 4, 2), "d", null);
//            AssertTitleIs("04/02/2007");
//        }

//        [Test]
//        public void TestAppendFormatWithDefault() {
//            builder.Append("x");
//            builder.Append(":", null, "d", "no date");
//            AssertTitleIs("x: no date");
//        }

//        [Test]
//        public void TestAppendNullAddsNoTextAndNoSpace() {
//            builder.Append(null);
//            AssertTitleIs("");
//        }

//        [Test]
//        public void TestAppendToEmptyFormatWithDefault() {
//            builder.Append(":", null, "d", "no date");
//            AssertTitleIs("no date");
//        }

//        [Test]
//        public void TestAppendWithJoiner() {
//            builder.Append(1).Append("~", 2).Append(" !", 3);
//            AssertTitleIs("1~ 2 ! 3");
//        }

//        [Test]
//        public void TestConcatAddsText() {
//            builder.Concat("added");
//            AssertTitleIs("added");
//        }

//        [Test]
//        public void TestConcatAddsTextWithJoiner() {
//            builder.Concat("+", "1Add");
//            builder.Concat("+", "2Add");
//            AssertTitleIs("1Add+2Add");
//        }

//        [Test]
//        public void TestConcatNoJoiner() {
//            builder.Concat(":", null, "d", null);
//            AssertTitleIs("");
//        }

//        [Test]
//        public void TestJoinerOmmittedIfBeingAppendedToNothing() {
//            builder.Append("~", 1);
//            AssertTitleIs("1");
//        }

//        [Test]
//        public void TestJoinerOmmittedIfNullIsAppended() {
//            builder.Append(1).Append("~", null);
//            AssertTitleIs("1");
//        }

//        [Test]
//        public void TestNewBuilderContainsUnmodifiedText() {
//            AssertTitleIs("");
//        }

//        [Test]
//        public void TestSecondNullDoesNotNegateFirstOne() {
//            builder.Append(1).Append("~", 2).Append(" !", null);
//            AssertTitleIs("1~ 2");
//        }
//    }

//    internal enum Sex {
//        Male = 1,
//        Female = 2,
//        Unknown = 3,
//        NotSpecified = 4
//    }
//}