// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.DotNet.Facets;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {

    public enum TestEnum {
        London,
        Paris,
        NewYork
    } ;


    [TestFixture]
    public class EnumValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TestEnum> {
        private IFacetHolder holder;
        private EnumValueSemanticsProvider<TestEnum> value;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            holder = new FacetHolderImpl();
            SetValue(value = new EnumValueSemanticsProvider<TestEnum>(holder));
        }

        [Test]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry(0, "fail");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf( typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("New York", value.DisplayTitleOf(TestEnum.NewYork));
        }

        [Test]
        public void TestParse() {
            object newValue = value.ParseTextEntry(0, "0");
            Assert.AreEqual(TestEnum.London, newValue);
        }

        [Test]
        public void TestParseOverflow() {
            try {
                object newValue = value.ParseTextEntry(0, long.MaxValue.ToString());
                Assert.Fail("Expect Exception");
            }
            catch (InvalidEntryException e) { 
                Assert.AreEqual("Value was either too large or too small for an Int32.", e.Message);
            }
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry(0, "");
                Assert.IsNull(newValue);
            } catch (Exception ) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(TestEnum.Paris);
            Assert.AreEqual("NakedObjects.Reflector.DotNet.Value.TestEnum:Paris", encoded);
        }

        [Test]
        public void TestDecode() {
            TestEnum decoded = GetValue().FromEncodedString("NakedObjects.Reflector.DotNet.Value.TestEnum:Paris");
            Assert.AreEqual(TestEnum.Paris, decoded);
        }

        [Test]
        public void TestDefault() {
            var def = value.GetDefault(new ProgrammableNakedObject(null, null));
            Assert.AreEqual(TestEnum.London, def);
        }


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

        public enum TestEnumI  {
            London = int.MinValue,
            Paris,
            NewYork = int.MaxValue
        }

        public enum TestEnumUl : ulong {
            London = ulong.MinValue,
            Paris,
            NewYork = ulong.MaxValue
        }

        public enum TestEnumL: long {
            London = long.MinValue,
            Paris,
            NewYork = long.MaxValue
        } 



        [Test]
        public void TestIntegralValue() {

            Assert.AreEqual(sbyte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>().IntegralValue(new ProgrammableNakedObject(TestEnumSb.London, null)));
            Assert.AreEqual(byte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>().IntegralValue(new ProgrammableNakedObject(TestEnumB.London, null)));
            Assert.AreEqual(ushort.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>().IntegralValue(new ProgrammableNakedObject(TestEnumUs.London, null)));
            Assert.AreEqual(short.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>().IntegralValue(new ProgrammableNakedObject(TestEnumS.London, null)));
            Assert.AreEqual(uint.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>().IntegralValue(new ProgrammableNakedObject(TestEnumUi.London, null)));
            Assert.AreEqual(int.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>().IntegralValue(new ProgrammableNakedObject(TestEnumI.London, null)));
            Assert.AreEqual(ulong.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>().IntegralValue(new ProgrammableNakedObject(TestEnumUl.London, null)));
            Assert.AreEqual(long.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>().IntegralValue(new ProgrammableNakedObject(TestEnumL.London, null)));

            Assert.AreEqual(sbyte.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>().IntegralValue(new ProgrammableNakedObject(TestEnumSb.NewYork, null)));
            Assert.AreEqual(byte.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>().IntegralValue(new ProgrammableNakedObject(TestEnumB.NewYork, null)));
            Assert.AreEqual(ushort.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>().IntegralValue(new ProgrammableNakedObject(TestEnumUs.NewYork, null)));
            Assert.AreEqual(short.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>().IntegralValue(new ProgrammableNakedObject(TestEnumS.NewYork, null)));
            Assert.AreEqual(uint.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>().IntegralValue(new ProgrammableNakedObject(TestEnumUi.NewYork, null)));
            Assert.AreEqual(int.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>().IntegralValue(new ProgrammableNakedObject(TestEnumI.NewYork, null)));
            Assert.AreEqual(ulong.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>().IntegralValue(new ProgrammableNakedObject(TestEnumUl.NewYork, null)));
            Assert.AreEqual(long.MaxValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>().IntegralValue(new ProgrammableNakedObject(TestEnumL.NewYork, null)));

            Assert.AreEqual(sbyte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumSb>().IntegralValue(new ProgrammableNakedObject(sbyte.MinValue, null)));
            Assert.AreEqual(byte.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumB>().IntegralValue(new ProgrammableNakedObject(byte.MinValue, null)));
            Assert.AreEqual(ushort.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUs>().IntegralValue(new ProgrammableNakedObject(ushort.MinValue, null)));
            Assert.AreEqual(short.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumS>().IntegralValue(new ProgrammableNakedObject(short.MinValue, null)));
            Assert.AreEqual(uint.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUi>().IntegralValue(new ProgrammableNakedObject(uint.MinValue, null)));
            Assert.AreEqual(int.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumI>().IntegralValue(new ProgrammableNakedObject(int.MinValue, null)));
            Assert.AreEqual(ulong.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumUl>().IntegralValue(new ProgrammableNakedObject(ulong.MinValue, null)));
            Assert.AreEqual(long.MinValue.ToString(), new EnumValueSemanticsProvider<TestEnumL>().IntegralValue(new ProgrammableNakedObject(long.MinValue, null)));

            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumSb>().IntegralValue(new ProgrammableNakedObject((sbyte)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumB>().IntegralValue(new ProgrammableNakedObject((byte)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUs>().IntegralValue(new ProgrammableNakedObject((ushort)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumS>().IntegralValue(new ProgrammableNakedObject((short)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUi>().IntegralValue(new ProgrammableNakedObject((uint)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumI>().IntegralValue(new ProgrammableNakedObject((int)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumUl>().IntegralValue(new ProgrammableNakedObject((ulong)2, null)));
            Assert.AreEqual(2.ToString(), new EnumValueSemanticsProvider<TestEnumL>().IntegralValue(new ProgrammableNakedObject((long)2, null)));


        }

    }


    // Copyright (c) Naked Objects Group Ltd.
}