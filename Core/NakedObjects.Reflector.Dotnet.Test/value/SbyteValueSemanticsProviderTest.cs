// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class SbyteValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<sbyte> {
        private sbyte byteObj;
        private IFacetHolder holder;
        private SbyteValueSemanticsProvider value;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            byteObj = 102;
            holder = new FacetHolderImpl();
            SetValue(value = new SbyteValueSemanticsProvider(holder));
        }

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
                Assert.IsInstanceOf( typeof (InvalidEntryException),e);
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
            } catch (Exception ) {
                Assert.Fail();
            }
        }

        // Copyright (c) Naked Objects Group Ltd.
    }
}