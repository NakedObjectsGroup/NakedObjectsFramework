// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Adapter;


using NakedObjects.Reflector.DotNet.Value;
using NUnit.Framework;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet {
    public abstract class ValueSemanticsProviderAbstractTestCase<T> {
        private EncodeableFacetUsingEncoderDecoder<T> encodeableFacet;
        private ParseableFacetUsingParser<T> parseableFacet;
        protected ILifecycleManager LifecycleManager = new Mock<ILifecycleManager>().Object;
        protected IObjectPersistor Persistor = new Mock<IObjectPersistor>().Object;
        protected IReflector Reflector = new Mock<IReflector>().Object;
        protected IMetamodelManager Metamodel = new Mock<IMetamodelManager>().Object;
        private ValueSemanticsProviderAbstract<T> value;
        protected INakedObjectManager Manager = new Mock<INakedObjectManager>().Object;

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
           
            return new PocoAdapter(Metamodel, session, Persistor, LifecycleManager, Manager, obj, null);
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
            object newValue = encodeableFacet.FromEncodedString(EncodeableFacetUsingEncoderDecoder<object>.EncodedNull, Manager);
            Assert.IsNull(newValue);
        }

        [Test]
        public void TestEmptyEncoding() {
            Assert.AreEqual(EncodeableFacetUsingEncoderDecoder<object>.EncodedNull, encodeableFacet.ToEncodedString(null));
        }
    }
}