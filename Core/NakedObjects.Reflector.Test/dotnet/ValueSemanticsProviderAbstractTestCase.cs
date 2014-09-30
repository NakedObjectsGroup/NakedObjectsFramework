// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter;
using NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Parseable;
using NakedObjects.Reflector.DotNet.Value;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet {
    public abstract class ValueSemanticsProviderAbstractTestCase<T> {
        private EncodeableFacetUsingEncoderDecoder<T> encodeableFacet;
        private ParseableFacetUsingParser<T> parseableFacet;
        protected ILifecycleManager persistor = new Mock<ILifecycleManager>().Object;
        protected INakedObjectReflector reflector = new Mock<INakedObjectReflector>().Object;
        private ValueSemanticsProviderAbstract<T> value;

        protected void SetValue(ValueSemanticsProviderAbstract<T> newValue) {
            value = newValue;
            encodeableFacet = new EncodeableFacetUsingEncoderDecoder<T>(newValue, null);
            parseableFacet = new ParseableFacetUsingParser<T>(newValue, null);
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
        public virtual void SetUp() {}

        [TearDown]
        public virtual void TearDown() {
            value = null;
            encodeableFacet = null;
            parseableFacet = null;
        }

        protected void SetupSpecification(Type type) {
            //TestProxySpecification specification = system.GetSpecification(type);
            //specification.SetupHasNoIdentity(true);
        }

        protected INakedObject CreateAdapter(object obj) {
            ISession session = new Mock<ISession>().Object;
            return new PocoAdapter(reflector, session, persistor, persistor, obj, null);
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
            object newValue = encodeableFacet.FromEncodedString(EncodeableFacetUsingEncoderDecoder<object>.ENCODED_NULL, persistor);
            Assert.IsNull(newValue);
        }

        [Test]
        public void TestEmptyEncoding() {
            Assert.AreEqual(EncodeableFacetUsingEncoderDecoder<object>.ENCODED_NULL, encodeableFacet.ToEncodedString(null));
        }
    }
}