// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Util;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class BoolValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<bool> {
        private INakedObject booleanNO;
        private object booleanObj;
        private IFacetHolder facetHolder;
        private BooleanValueSemanticsProvider value;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            booleanObj = true;
            booleanNO = CreateAdapter(booleanObj);
            facetHolder = new FacetHolderImpl();
            SetValue(value = new BooleanValueSemanticsProvider(facetHolder));
        }

        [Test]
        public void TestParseInvariant() {
            new[] {true, false}.ForEach(b => {
                var b1 = b.ToString(CultureInfo.InvariantCulture);
                var b2 = value.ParseInvariant(b1);
                Assert.AreEqual(b, b2);
            });
        }

        [Test]
        public void TestParseFalseString() {
            object parsed = value.ParseTextEntry("faLSe");
            Assert.AreEqual(false, parsed);
        }

        [Test]
        public void TestParseTrueString() {
            object parsed = value.ParseTextEntry("TRue");
            Assert.AreEqual(true, parsed);
        }

        [Test]
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("yes");
                Assert.Fail("Invalid string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf( typeof (InvalidEntryException),e);
            }
        }

        [Test]
        public void TestTitleTrue() {
            Assert.AreEqual("True", value.DisplayTitleOf(true));
        }

        [Test]
        public void TestTitleFalse() {
            Assert.AreEqual("False", value.DisplayTitleOf(false));
        }

        [Test]
        public void TestEncodeTrue() {
            Assert.AreEqual("T", value.ToEncodedString(true));
        }

        [Test]
        public void TestEncodeFalse() {
            Assert.AreEqual("F", value.ToEncodedString(false));
        }

        [Test]
        public void TestDecodeTrue() {
            object parsed = value.FromEncodedString("T");
            Assert.AreEqual(true, parsed);
        }
        [Test]
        public void TestDecodeFalse() {
            object parsed = value.FromEncodedString("F");
            Assert.AreEqual(false, parsed);
        }

        [Test]
        public void TestIsSet() {
            Assert.AreEqual(true, value.IsSet(booleanNO));
        }

        [Test]
        public void TestIsNotSet() {
            Assert.AreEqual(false, value.IsSet(CreateAdapter(false)));
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception ) {
                Assert.Fail();
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}