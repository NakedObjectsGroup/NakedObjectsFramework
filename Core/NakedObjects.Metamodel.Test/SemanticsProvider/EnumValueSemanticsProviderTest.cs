// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Meta.SemanticsProvider;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.Meta.Test.SemanticsProvider {
    public enum TestEnum {
        London,
        Paris,
        NewYork
    }

    [TestClass]
    public class EnumValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TestEnum> {
        #region TestEnumB enum

        public enum TestEnumB : byte {
            London = byte.MinValue,
            Paris,
            NewYork = byte.MaxValue
        }

        #endregion

        #region TestEnumI enum

        public enum TestEnumI {
            London = int.MinValue,
            Paris,
            NewYork = int.MaxValue
        }

        #endregion

        #region TestEnumL enum

        public enum TestEnumL : long {
            London = long.MinValue,
            Paris,
            NewYork = long.MaxValue
        }

        #endregion

        #region TestEnumS enum

        public enum TestEnumS : short {
            London = short.MinValue,
            Paris,
            NewYork = short.MaxValue
        }

        #endregion

        #region TestEnumSb enum

        public enum TestEnumSb : sbyte {
            London = sbyte.MinValue,
            Paris,
            NewYork = sbyte.MaxValue
        }

        #endregion

        #region TestEnumUi enum

        public enum TestEnumUi : uint {
            London = uint.MinValue,
            Paris,
            NewYork = uint.MaxValue
        }

        #endregion

        #region TestEnumUl enum

        public enum TestEnumUl : ulong {
            London = ulong.MinValue,
            Paris,
            NewYork = ulong.MaxValue
        }

        #endregion

        #region TestEnumUs enum

        public enum TestEnumUs : ushort {
            London = ushort.MinValue,
            Paris,
            NewYork = ushort.MaxValue
        }

        #endregion

        private ISpecification holder;
        private EnumValueSemanticsProvider<TestEnum> value;

        private static INakedObjectAdapter MockNakedObject(object toWrap) {
            var mock = new Mock<INakedObjectAdapter>();
            mock.Setup(no => no.Object).Returns(toWrap);
            return mock.Object;
        }

        [TestMethod]
        public void TestDecode() {
            var decoded = GetValue().FromEncodedString("NakedObjects.Meta.Test.SemanticsProvider.TestEnum:Paris");
            Assert.AreEqual(TestEnum.Paris, decoded);
        }

        [TestMethod]
        public void TestDefault() {
            object def = value.DefaultValue;
            Assert.AreEqual(TestEnum.London, def);
        }

        [TestMethod]
        public void TestEncode() {
            var encoded = GetValue().ToEncodedString(TestEnum.Paris);
            Assert.AreEqual("NakedObjects.Meta.Test.SemanticsProvider.TestEnum:Paris", encoded);
        }

        [TestMethod]
        public void TestIntegralValue() {
            Assert.AreEqual(sbyte.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumSb>(null, null).IntegralValue(MockNakedObject(TestEnumSb.London)));
            Assert.AreEqual(byte.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumB>(null, null).IntegralValue(MockNakedObject(TestEnumB.London)));
            Assert.AreEqual(ushort.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUs>(null, null).IntegralValue(MockNakedObject(TestEnumUs.London)));
            Assert.AreEqual(short.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumS>(null, null).IntegralValue(MockNakedObject(TestEnumS.London)));
            Assert.AreEqual(uint.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUi>(null, null).IntegralValue(MockNakedObject(TestEnumUi.London)));
            Assert.AreEqual(int.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumI>(null, null).IntegralValue(MockNakedObject(TestEnumI.London)));
            Assert.AreEqual(ulong.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUl>(null, null).IntegralValue(MockNakedObject(TestEnumUl.London)));
            Assert.AreEqual(long.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumL>(null, null).IntegralValue(MockNakedObject(TestEnumL.London)));

            Assert.AreEqual(sbyte.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumSb>(null, null).IntegralValue(MockNakedObject(TestEnumSb.NewYork)));
            Assert.AreEqual(byte.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumB>(null, null).IntegralValue(MockNakedObject(TestEnumB.NewYork)));
            Assert.AreEqual(ushort.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUs>(null, null).IntegralValue(MockNakedObject(TestEnumUs.NewYork)));
            Assert.AreEqual(short.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumS>(null, null).IntegralValue(MockNakedObject(TestEnumS.NewYork)));
            Assert.AreEqual(uint.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUi>(null, null).IntegralValue(MockNakedObject(TestEnumUi.NewYork)));
            Assert.AreEqual(int.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumI>(null, null).IntegralValue(MockNakedObject(TestEnumI.NewYork)));
            Assert.AreEqual(ulong.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUl>(null, null).IntegralValue(MockNakedObject(TestEnumUl.NewYork)));
            Assert.AreEqual(long.MaxValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumL>(null, null).IntegralValue(MockNakedObject(TestEnumL.NewYork)));

            Assert.AreEqual(sbyte.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumSb>(null, null).IntegralValue(MockNakedObject(sbyte.MinValue)));
            Assert.AreEqual(byte.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumB>(null, null).IntegralValue(MockNakedObject(byte.MinValue)));
            Assert.AreEqual(ushort.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUs>(null, null).IntegralValue(MockNakedObject(ushort.MinValue)));
            Assert.AreEqual(short.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumS>(null, null).IntegralValue(MockNakedObject(short.MinValue)));
            Assert.AreEqual(uint.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUi>(null, null).IntegralValue(MockNakedObject(uint.MinValue)));
            Assert.AreEqual(int.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumI>(null, null).IntegralValue(MockNakedObject(int.MinValue)));
            Assert.AreEqual(ulong.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUl>(null, null).IntegralValue(MockNakedObject(ulong.MinValue)));
            Assert.AreEqual(long.MinValue.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumL>(null, null).IntegralValue(MockNakedObject(long.MinValue)));

            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumSb>(null, null).IntegralValue(MockNakedObject((sbyte) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumB>(null, null).IntegralValue(MockNakedObject((byte) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUs>(null, null).IntegralValue(MockNakedObject((ushort) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumS>(null, null).IntegralValue(MockNakedObject((short) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUi>(null, null).IntegralValue(MockNakedObject((uint) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumI>(null, null).IntegralValue(MockNakedObject(2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumUl>(null, null).IntegralValue(MockNakedObject((ulong) 2)));
            Assert.AreEqual(2.ToString(CultureInfo.InvariantCulture), new EnumValueSemanticsProvider<TestEnumL>(null, null).IntegralValue(MockNakedObject((long) 2)));
        }

        [TestMethod]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("fail");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
            }
        }

        [TestMethod]
        public void TestParse() {
            var newValue = value.ParseTextEntry("0");
            Assert.AreEqual(TestEnum.London, newValue);
        }

        [TestMethod]
        public override void TestParseEmptyString() {
            try {
                var newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestParseInvariant() {
            const TestEnum c1 = TestEnum.London;
            var s1 = c1.ToString();
            var c2 = value.ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void TestParseOverflow() {
            try {
                // ReSharper disable once UnusedVariable
                var newValue = value.ParseTextEntry(long.MaxValue.ToString(CultureInfo.InvariantCulture));
                Assert.Fail("Expect Exception");
            }
            catch (InvalidEntryException e) {
                Assert.AreEqual("Value was either too large or too small for an Int32.", e.Message);
            }
        }

        [TestMethod]
        public void TestTitleString() {
            Assert.AreEqual("New York", value.DisplayTitleOf(TestEnum.NewYork));
        }

        [TestMethod]
        public override void TestParseNull() {
            base.TestParseNull();
        }

        [TestMethod]
        public override void TestDecodeNull() {
            base.TestDecodeNull();
        }

        [TestMethod]
        public override void TestEmptyEncoding() {
            base.TestEmptyEncoding();
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new EnumValueSemanticsProvider<TestEnum>(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}