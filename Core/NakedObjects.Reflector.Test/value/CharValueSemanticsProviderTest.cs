// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Spec;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class CharValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<char> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            character = 'r';
            holder = new SpecificationImpl();
            var spec = new Mock<IIntrospectableSpecification>().Object;
            SetValue(value = new CharValueSemanticsProvider(spec, holder));
        }

        #endregion

        private Char character;
        private ISpecification holder;
        private CharValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            object restore = value.FromEncodedString("Y");
            Assert.AreEqual('Y', restore);
        }

        [Test]
        public void TestEncode() {
            Assert.AreEqual("r", value.ToEncodedString(character));
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
            const char c1 = 'z';
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = value.ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseLongString() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestTitleOf() {
            Assert.AreEqual("r", value.DisplayTitleOf(character));
        }

        [Test]
        public void TestValidParse() {
            object parse = value.ParseTextEntry("t");
            Assert.AreEqual('t', parse);
        }

        // Copyright (c) Naked Objects Group Ltd.
    }
}