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
    public class SbyteValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<sbyte> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            byteObj = 102;
            holder = new FacetHolderImpl();
            SetValue(value = new SbyteValueSemanticsProvider(reflector, holder));
        }

        #endregion

        private sbyte byteObj;
        private IFacetHolder holder;
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