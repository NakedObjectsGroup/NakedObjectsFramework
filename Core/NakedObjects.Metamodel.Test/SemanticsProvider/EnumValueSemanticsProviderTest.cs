// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;
using NUnit.Framework;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    public enum TestEnum {
        London,
        Paris,
        NewYork
    };


    [TestFixture]
    public class EnumValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TestEnum> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new EnumValueSemanticsProvider<TestEnum>(spec, holder));
        }

        #endregion

        private ISpecification holder;
        private EnumValueSemanticsProvider<TestEnum> value;


        public enum TestEnumSb : sbyte {
            London = sbyte.MinValue,
            Paris,
            NewYork = sbyte.MaxValue
        }

        public enum TestEnumB : byte {
            London = byte.MinValue,
            Paris,
            NewYork = byte.MaxValue
        }

        public enum TestEnumUs : ushort {
            London = ushort.MinValue,
            Paris,
            NewYork = ushort.MaxValue
        }

        public enum TestEnumS : short {
            London = short.MinValue,
            Paris,
            NewYork = short.MaxValue
        }

        public enum TestEnumUi : uint {
            London = uint.MinValue,
            Paris,
            NewYork = uint.MaxValue
        }

        public enum TestEnumI {
            London = int.MinValue,
            Paris,
            NewYork = int.MaxValue
        }

        public enum TestEnumUl : ulong {
            London = ulong.MinValue,
            Paris,
            NewYork = ulong.MaxValue
        }

        public enum TestEnumL : long {
            London = long.MinValue,
            Paris,
            NewYork = long.MaxValue
        }

        private static INakedObject MockNakedObject(object toWrap) {
            var mock = new Mock<INakedObject>();
            mock.Setup(no => no.Object).Returns(toWrap);
            return mock.Object;
        }

        [Test]
        public void TestDecode() {
            TestEnum decoded = GetValue().FromEncodedString("NakedObjects.Meta.Test.SemanticsProvider.TestEnum:Paris");
            Assert.AreEqual(TestEnum.Paris, decoded);
        }


        [Test]
        public void TestDefault() {
            INakedObject nakedObject = MockNakedObject(null);
            object def = value.GetDefault(nakedObject);
            Assert.AreEqual(TestEnum.London, def);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(TestEnum.Paris);
            Assert.AreEqual("NakedObjects.Meta.Test.SemanticsProvider.TestEnum:Paris", encoded);
        }


        [Test]
        public void TestIntegralValue() {
            Assert.AreEqual(sbyte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>(null).IntegralValue(MockNakedObject(TestEnumSb.London)));
            Assert.AreEqual(byte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>(null).IntegralValue(MockNakedObject(TestEnumB.London)));
            Assert.AreEqual(ushort.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>(null).IntegralValue(MockNakedObject(TestEnumUs.London)));
            Assert.AreEqual(short.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>(null).IntegralValue(MockNakedObject(TestEnumS.London)));
            Assert.AreEqual(uint.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>(null).IntegralValue(MockNakedObject(TestEnumUi.London)));
            Assert.AreEqual(int.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>(null).IntegralValue(MockNakedObject(TestEnumI.London)));
            Assert.AreEqual(ulong.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>(null).IntegralValue(MockNakedObject(TestEnumUl.London)));
            Assert.AreEqual(long.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>(null).IntegralValue(MockNakedObject(TestEnumL.London)));

            Assert.AreEqual(sbyte.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>(null).IntegralValue(MockNakedObject(TestEnumSb.NewYork)));
            Assert.AreEqual(byte.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>(null).IntegralValue(MockNakedObject(TestEnumB.NewYork)));
            Assert.AreEqual(ushort.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>(null).IntegralValue(MockNakedObject(TestEnumUs.NewYork)));
            Assert.AreEqual(short.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>(null).IntegralValue(MockNakedObject(TestEnumS.NewYork)));
            Assert.AreEqual(uint.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>(null).IntegralValue(MockNakedObject(TestEnumUi.NewYork)));
            Assert.AreEqual(int.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>(null).IntegralValue(MockNakedObject(TestEnumI.NewYork)));
            Assert.AreEqual(ulong.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>(null).IntegralValue(MockNakedObject(TestEnumUl.NewYork)));
            Assert.AreEqual(long.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>(null).IntegralValue(MockNakedObject(TestEnumL.NewYork)));

            Assert.AreEqual(sbyte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>(null).IntegralValue(MockNakedObject(sbyte.MinValue)));
            Assert.AreEqual(byte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>(null).IntegralValue(MockNakedObject(byte.MinValue)));
            Assert.AreEqual(ushort.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>(null).IntegralValue(MockNakedObject(ushort.MinValue)));
            Assert.AreEqual(short.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>(null).IntegralValue(MockNakedObject(short.MinValue)));
            Assert.AreEqual(uint.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>(null).IntegralValue(MockNakedObject(uint.MinValue)));
            Assert.AreEqual(int.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>(null).IntegralValue(MockNakedObject(int.MinValue)));
            Assert.AreEqual(ulong.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>(null).IntegralValue(MockNakedObject(ulong.MinValue)));
            Assert.AreEqual(long.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>(null).IntegralValue(MockNakedObject(long.MinValue)));

            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumSb>(null).IntegralValue(MockNakedObject((sbyte) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumB>(null).IntegralValue(MockNakedObject((byte) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUs>(null).IntegralValue(MockNakedObject((ushort) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumS>(null).IntegralValue(MockNakedObject((short) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUi>(null).IntegralValue(MockNakedObject((uint) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumI>(null).IntegralValue(MockNakedObject(2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUl>(null).IntegralValue(MockNakedObject((ulong) 2)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumL>(null).IntegralValue(MockNakedObject((long) 2)));
        }

        [Test]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("fail");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParse() {
            object newValue = value.ParseTextEntry("0");
            Assert.AreEqual(TestEnum.London, newValue);
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
            const TestEnum c1 = TestEnum.London;
            string s1 = c1.ToString();
            object c2 = value.ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseOverflow() {
            try {
                object newValue = value.ParseTextEntry(long.MaxValue.ToString());
                Assert.Fail("Expect Exception");
            }
            catch (InvalidEntryException e) {
                Assert.AreEqual("Value was either too large or too small for an Int32.", e.Message);
            }
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("New York", value.DisplayTitleOf(TestEnum.NewYork));
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}