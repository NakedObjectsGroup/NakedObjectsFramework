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
    public class FloatValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<float> {
        private Single floatObj;

        private IFacetHolder holder;

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            holder = new FacetHolderImpl();
            SetValue(new FloatValueSemanticsProvider(holder));

            floatObj = 32.5F;
        }

        [Test]
        public void TestValue() {
            Assert.AreEqual("32.5", GetValue().DisplayTitleOf(floatObj));
        }

        [Test]
        public void TestInvalidParse() {
            try {
                GetValue().ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf( typeof (InvalidEntryException),e);
            }
        }

        [Test]
        public void TestTitleOf() {
            Assert.AreEqual("3500000", GetValue().DisplayTitleOf(3500000.0F));
        }

        [Test]
        public void TestParse() {
            object newValue = GetValue().ParseTextEntry("120.56");
            Assert.AreEqual(120.56F, newValue);
        }

        [Test]
        public void TestParse2() {
            object newValue = GetValue().ParseTextEntry("1,20.0");
            Assert.AreEqual(120F, newValue);
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = GetValue().ParseTextEntry("");
                Assert.IsNull(newValue);
            } catch (Exception ) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(0.0000454566f);
            Assert.AreEqual("4.54566E-05", encoded);
        }

        [Test]
        public void TestDecode() {
            float decoded = GetValue().FromEncodedString("3.042112234E6");
            Assert.AreEqual(3042112.234f, decoded);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}