// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NUnit.Framework;
using NakedObjects.Meta.Spec;

namespace NakedObjects.Reflect.DotNet.Value {
    [TestFixture]
    public class CharValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<char> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            character = 'r';
            holder = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new CharValueSemanticsProvider(spec, holder));
        }

        #endregion

        private Char character;
        private ISpecification holder;
        private CharValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            object restore = value.FromEncodedString("Y");
            Assert.AreEqual('Y', restore);
        }

        [Test]
        public void TestEncode() {
            Assert.AreEqual("r", value.ToEncodedString(character));
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestParseInvariant() {
            const char c1 = 'z';
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = value.ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseLongString() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestTitleOf() {
            Assert.AreEqual("r", value.DisplayTitleOf(character));
        }

        [Test]
        public void TestValidParse() {
            object parse = value.ParseTextEntry("t");
            Assert.AreEqual('t', parse);
        }

        // Copyright (c) Naked Objects Group Ltd.
    }
}