// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Util {
#pragma warning disable 618
    [TestClass]
    public class TitleBuilderWithInitialContentTest {
        private TitleBuilder builder;
        private CultureInfo culture;

        [TestInitialize]
        public void NewBuilder() {
            culture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            builder = new TitleBuilder("Text");
        }

        [TestCleanup]
        public void Cleanup() {
            Thread.CurrentThread.CurrentCulture = culture;
        }

        private void AssertTitleIs(string expected) {
            Assert.AreEqual(expected, builder.ToString());
        }

        [TestMethod]
        public void TestNewBuilderContainsUnmodifiedText() {
            builder = new TitleBuilder("Text");
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestConcatAddsText() {
            builder.Concat("added");
            AssertTitleIs("Textadded");
        }

        [TestMethod]
        public void TestConcatAddsTextWithJoiner() {
            builder.Concat("+", "added");
            AssertTitleIs("Text+added");
        }

        [TestMethod]
        public void TestConcatAddsTitleAttribute() {
            builder.Concat(new ObjectWithTitleAttribute());
            AssertTitleIs("Textfrom property");
        }

        [TestMethod]
        public void TestAppendAddsTextAndSpace() {
            builder.Append("added");
            AssertTitleIs("Text added");
        }

        [TestMethod]
        public void TestAppendNullAddsNoTextAndNoSpace() {
            builder.Append(null);
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestAppendAddsToString() {
            builder.Append(new ObjectWithToString());
            AssertTitleIs("Text from ToString");
        }

        [TestMethod]
        public void TestAppendAddsToStringWithJoiner() {
            builder.Append("-", new ObjectWithToString());
            AssertTitleIs("Text- from ToString");
        }

        [TestMethod]
        public void TestAppendAddsTitleMethod() {
            builder.Append(new ObjectWithTitleMethod());
            AssertTitleIs("Text from Title method");
        }

        [TestMethod]
        public void TestAppendAddsTitleAttribute() {
            builder.Append(new ObjectWithTitleAttribute());
            AssertTitleIs("Text from property");
        }

        [TestMethod]
        public void TestAppendAddsNullTitleAttribute() {
            builder.Append(new ObjectWithNullTitleAttribute());
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestAppendAddsTitleAttributeThatIsAReference() {
            builder.Append(new ObjectWithTitleAttributeThatIsAReference());
            AssertTitleIs("Text from Title method");
        }

        [TestMethod]
        public void TestConcatNoJoiner() {
            builder.Concat(":", null, "d", null);
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestAppendNoJoiner() {
            builder.Append(":", null, "d", null);
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestAppendNoJoinerNoDefault() {
            builder.Append(":", null, "d", null);
            AssertTitleIs("Text");
        }

        [TestMethod]
        public void TestAppendFormatWithDefault() {
            builder.Append(":", null, "d", "no date");
            AssertTitleIs("Text: no date");
        }

        [TestMethod]
        public void TestTitleTruncated() {
            builder.Append("no date");
            builder.Truncate(3);
            AssertTitleIs("Text no date");
            builder.Truncate(2);
            AssertTitleIs("Text no ...");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void TestTitleTruncateLenghtChecked() {
            builder.Truncate(0);
        }

        [TestMethod]
        public void TestAppendFormat() {
            builder.Append(":", new DateTime(2007, 4, 2), "d", null);
            AssertTitleIs("Text: 04/02/2007");
        }

        [TestMethod]
        public void Test() {
            builder.Append("added");
            AssertTitleIs("Text added");
        }
    }
#pragma warning restore 618
}