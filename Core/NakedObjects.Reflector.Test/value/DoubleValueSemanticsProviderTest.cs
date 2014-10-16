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
    public class DoubleValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<double> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            holder = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(new DoubleValueSemanticsProvider(spec, holder));

            doubleObj = 32.5;
        }

        #endregion

        private Double doubleObj;

        private ISpecification holder;

        [Test]
        public void TestDecode() {
            Double decoded = GetValue().FromEncodedString("3.042112234E6");
            Assert.AreEqual(3042112.234, decoded);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(0.0000454566);
            Assert.AreEqual("4.54566E-05", encoded);
        }

        [Test]
        public void TestInvalidParse() {
            try {
                GetValue().ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParse() {
            object newValue = GetValue().ParseTextEntry("120.56");
            Assert.AreEqual(120.56, newValue);
        }

        [Test]
        public void TestParse2() {
            object newValue = GetValue().ParseTextEntry("1,20.0");
            Assert.AreEqual(120D, newValue);
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = GetValue().ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestParseInvariant() {
            const double c1 = 123.456;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestTitleOf() {
            Assert.AreEqual("35000000", GetValue().DisplayTitleOf(35000000.0));
        }

        [Test]
        public void TestValue() {
            Assert.AreEqual("32.5", GetValue().DisplayTitleOf(doubleObj));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}