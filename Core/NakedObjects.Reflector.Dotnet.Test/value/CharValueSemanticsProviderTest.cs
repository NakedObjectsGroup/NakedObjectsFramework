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
    public class CharValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<char> {
        private Char character;
        private IFacetHolder holder;
        private CharValueSemanticsProvider value;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            character = 'r';
            holder = new FacetHolderImpl();
            SetValue(value = new CharValueSemanticsProvider(holder));
        }

        [Test]
        public void TestParseLongString() {
            try {
                value.ParseTextEntry(' ', "one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf( typeof (InvalidEntryException),e);
            }
        }

        [Test]
        public void TestTitleOf() {
            Assert.AreEqual("r", value.DisplayTitleOf(character));
        }

        [Test]
        public void TestValidParse() {
            object parse = value.ParseTextEntry(' ', "t");
            Assert.AreEqual('t', parse);
        }

        [Test]
        public void TestEncode() {
            Assert.AreEqual("r", value.ToEncodedString(character));
        }

        [Test]
        public void TestDecode() {
            object restore = value.FromEncodedString("Y");
            Assert.AreEqual('Y', restore);
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry(' ', "");
                Assert.IsNull(newValue);
            } catch (Exception ) {
                Assert.Fail();
            }
        }

        // Copyright (c) Naked Objects Group Ltd.
    }
}