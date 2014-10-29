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
    public class SbyteValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<sbyte> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            byteObj = 102;
            holder = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new SbyteValueSemanticsProvider(spec, holder));
        }

        #endregion

        private sbyte byteObj;
        private ISpecification holder;
        private SbyteValueSemanticsProvider value;

        public void TestParseValidString() {
            Object parsed = value.ParseTextEntry("21");
            Assert.AreEqual((sbyte) 21, parsed);
        }

        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("xs21z4xxx23");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        public void TestTitleOf() {
            Assert.AreEqual("102", value.DisplayTitleOf(byteObj));
        }

        public void TestEncode() {
            Assert.AreEqual("102", value.ToEncodedString(byteObj));
        }

        public void TestDecode() {
            Object parsed = value.FromEncodedString("-91");
            Assert.AreEqual((sbyte) -91, parsed);
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
            const sbyte c1 = (sbyte) 11;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        // Copyright (c) Naked Objects Group Ltd.
    }
}