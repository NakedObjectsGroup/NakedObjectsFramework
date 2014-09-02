// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class LongValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<long> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            l = 32;
            holder = new FacetHolderImpl();
            SetValue(value = new LongValueSemanticsProvider(reflector, holder));
        }

        #endregion

        private IFacetHolder holder;
        private long l;
        private LongValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            long decoded = GetValue().FromEncodedString("304211223");
            Assert.AreEqual(304211223L, decoded);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(213434790L);
            Assert.AreEqual("213434790", encoded);
        }

        [Test]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParse() {
            object newValue = value.ParseTextEntry("120");
            Assert.AreEqual(120L, newValue);
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
            const long c1 = 123456L;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseOddlyFormedEntry() {
            object newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual(120L, newValue);
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(l));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}