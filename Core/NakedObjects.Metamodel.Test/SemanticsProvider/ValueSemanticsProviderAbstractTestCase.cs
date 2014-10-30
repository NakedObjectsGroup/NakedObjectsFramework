// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Adapter;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SemanticsProvider;
using NUnit.Framework;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    public abstract class ValueSemanticsProviderAbstractTestCase<T> {
        protected ILifecycleManager LifecycleManager = new Mock<ILifecycleManager>().Object;
        protected INakedObjectManager Manager = new Mock<INakedObjectManager>().Object;
        protected IMetamodelManager Metamodel = new Mock<IMetamodelManager>().Object;
        protected IObjectPersistor Persistor = new Mock<IObjectPersistor>().Object;
        protected IReflector Reflector = new Mock<IReflector>().Object;
        private EncodeableFacetUsingEncoderDecoder<T> encodeableFacet;
        private ParseableFacetUsingParser<T> parseableFacet;
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