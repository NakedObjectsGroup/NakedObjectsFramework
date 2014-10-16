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
    public class ShortValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<short> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            s = 32;
            holder = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new ShortValueSemanticsProvider(spec, holder));
        }

        #endregion

        private ISpecification holder;
        private short s;
        private ShortValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            long decoded = GetValue().FromEncodedString("30421");
            Assert.AreEqual(30421, decoded);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(21343);
            Assert.AreEqual("21343", encoded);
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
            Assert.AreEqual((short) 120, newValue);
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
            const short c1 = (short) 12346;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseOddlyFormedEntry() {
            object newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual((short) 120, newValue);
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(s));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}