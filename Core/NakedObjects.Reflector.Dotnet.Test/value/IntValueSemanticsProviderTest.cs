// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class IntValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<int> {
        private IFacetHolder holder;
        private int integer;
        private IntValueSemanticsProvider value;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            integer = 32;
            holder = new FacetHolderImpl();
            SetValue(value = new IntValueSemanticsProvider(holder));
        }

        [Test]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf( typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(integer));
        }

        [Test]
        public void TestParse() {
            object newValue = value.ParseTextEntry("120");
            Assert.AreEqual(120, newValue);
        }

        [Test]
        public void TestParseOddlyFormedEntry() {
            object newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual(120, newValue);
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            } catch (Exception ) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(213434790);
            Assert.AreEqual("213434790", encoded);
        }

        [Test]
        public void TestDecode() {
            int decoded = GetValue().FromEncodedString("304211223");
            Assert.AreEqual(304211223, decoded);
        }

        [Test]
        public void TestParseInvariant() {
            const int c1 = 123;
            var s1 = c1.ToString(CultureInfo.InvariantCulture);
            var c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}