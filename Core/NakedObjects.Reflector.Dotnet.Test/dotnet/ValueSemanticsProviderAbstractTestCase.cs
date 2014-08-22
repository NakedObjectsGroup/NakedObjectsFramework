// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Parseable;
using NakedObjects.Reflector.DotNet.Value;
using NakedObjects.Testing;
using NakedObjects.TestSystem;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet {
    
    public abstract class ValueSemanticsProviderAbstractTestCase<T> : TestProxyTestCase {
        private EncodeableFacetUsingEncoderDecoder<T> encodeableFacet;
        private ParseableFacetUsingParser<T> parseableFacet;
        private ValueSemanticsProviderAbstract<T> value;
        protected ProgrammableReflector reflector;

        protected void SetValue(ValueSemanticsProviderAbstract<T> newValue) {
            value = newValue;
            encodeableFacet = new EncodeableFacetUsingEncoderDecoder<T>(newValue, null);
            parseableFacet = new ParseableFacetUsingParser<T>(newValue, null);
            reflector = new ProgrammableReflector(new ProgrammableTestSystem());
        }

        protected ValueSemanticsProviderAbstract<T> GetValue() {
            return value;
        }

        protected IEncodeableFacet GetEncodeableFacet() {
            return encodeableFacet;
        }

        protected IParseableFacet GetParseableFacet() {
            return parseableFacet;
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            //TestClock.initialize();
        }

        [TearDown]
        public override void TearDown() {
            value = null;
            encodeableFacet = null;
            parseableFacet = null;
            base.TearDown();
        }


        protected void SetupSpecification(Type type) {
            TestProxySpecification specification = system.GetSpecification(type);
            specification.SetupHasNoIdentity(true);
        }

        protected INakedObject CreateAdapter(object obj) {
            return system.CreateAdapterForTransient(obj);
        }


       [Test]
        public void TestParseNull() {
            try {
                value.ParseTextEntry(null);
                Assert.Fail();
            }
            catch (ArgumentException /*expected*/) {}
        }

       [Test]
        public void TestParseEmptyString() {
            object newValue = value.ParseTextEntry("");
            Assert.IsNull(newValue);
        }

       [Test]
        public void TestDecodeNull() {
            object newValue = encodeableFacet.FromEncodedString(EncodeableFacetUsingEncoderDecoder<object>.ENCODED_NULL, NakedObjectsContext.ObjectPersistor);
            Assert.IsNull(newValue);
        }

       [Test]
        public void TestEmptyEncoding() {
            Assert.AreEqual(EncodeableFacetUsingEncoderDecoder<object>.ENCODED_NULL, encodeableFacet.ToEncodedString(null));
        }


        //[Test]
        //public void TestEmptyTitle() {
        //    Assert.AreEqual("", value.DisplayTitleOf(default(T)));
        //}
    }
}