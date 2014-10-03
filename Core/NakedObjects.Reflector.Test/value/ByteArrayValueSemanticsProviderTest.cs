// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using System.Linq;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class ByteArrayValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<byte[]> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            byteArray = new byte[0];
            byteArrayNakedObject = CreateAdapter(byteArray);
            facetHolder = new FacetHolderImpl();
            var spec = new Mock<IIntrospectableSpecification>().Object;
            SetValue(value = new ArrayValueSemanticsProvider<byte>(spec, facetHolder));
        }

        #endregion

        private INakedObject byteArrayNakedObject;
        private object byteArray;
        private IFacetHolder facetHolder;
        private ArrayValueSemanticsProvider<byte> value;


        public void TestEncodeDecode(byte[] toTest) {
            byte[] originalValue = toTest;
            string encodedValue = value.ToEncodedString(originalValue);
            byte[] decodedValue = value.FromEncodedString(encodedValue);

            Assert.AreEqual(decodedValue, originalValue);
        }


        [Test]
        public void TestEncodeDecode() {
            TestEncodeDecode(new byte[] {1, 2, 100});
        }

        [Test]
        public void TestEncodeDecodeEmpty() {
            TestEncodeDecode(new byte[] {});
        }

        [Test]
        public void TestEncodeDecodeNull() {
            TestEncodeDecode(null);
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
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("fred");
                Assert.Fail("Invalid string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParseInvariant() {
            var b1 = new byte[] {1, 2, 3, 4};
            string s1 = b1.Aggregate("", (s, t) => s + ' ' + t.ToString(CultureInfo.InvariantCulture));
            object b2 = value.ParseInvariant(s1);
            Assert.AreEqual(b1, b2);
        }

        [Test]
        public void TestParseOutOfRangeString() {
            try {
                value.ParseTextEntry("1 2 1000");
                Assert.Fail("out of range string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParseString() {
            object parsed = value.ParseTextEntry("0 0 1 100 255");
            Assert.AreEqual(new byte[] {0, 0, 1, 100, 255}, parsed);
        }

        [Test]
        public void TestTitle() {
            Assert.AreEqual("1 2 100", value.DisplayTitleOf(new byte[] {1, 2, 100}));
        }

        [Test]
        public void TestTitleEmpty() {
            Assert.AreEqual("", value.DisplayTitleOf(new byte[] {}));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}