// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Core.Adapter;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    public abstract class ValueSemanticsProviderAbstractTestCase<T> {
        private EncodeableFacetUsingEncoderDecoder<T> encodeableFacet;
        protected ILifecycleManager LifecycleManager = new Mock<ILifecycleManager>().Object;
        protected INakedObjectManager Manager = new Mock<INakedObjectManager>().Object;
        protected IMetamodelManager Metamodel = new Mock<IMetamodelManager>().Object;
        private ParseableFacetUsingParser<T> parseableFacet;
        protected IObjectPersistor Persistor = new Mock<IObjectPersistor>().Object;
        protected IReflector Reflector = new Mock<IReflector>().Object;
        private IValueSemanticsProvider<T> value;

        protected void SetValue(IValueSemanticsProvider<T> newValue) {
            value = newValue;
            encodeableFacet = new EncodeableFacetUsingEncoderDecoder<T>(newValue, null);
            parseableFacet = new ParseableFacetUsingParser<T>(newValue, null);
        }

        protected IValueSemanticsProvider<T> GetValue() {
            return value;
        }

        protected IEncodeableFacet GetEncodeableFacet() {
            return encodeableFacet;
        }

        protected IParseableFacet GetParseableFacet() {
            return parseableFacet;
        }

        public virtual void SetUp() {}

        public virtual void TearDown() {
            value = null;
            encodeableFacet = null;
            parseableFacet = null;
        }

        protected void SetupSpecification(Type type) {
            //TestProxySpecification specification = system.GetSpecification(type);
            //specification.SetupHasNoIdentity(true);
        }

        protected INakedObjectAdapter CreateAdapter(object obj) {
            ISession session = new Mock<ISession>().Object;

            return new NakedObjectAdapter(Metamodel, session, Persistor, LifecycleManager, Manager, obj, null);
        }

        public virtual void TestParseNull() {
            try {
                value.ParseTextEntry(null);
                Assert.Fail();
            }
            catch (ArgumentException /*expected*/) {}
        }

        public virtual void TestParseEmptyString() {
            object newValue = value.ParseTextEntry("");
            Assert.IsNull(newValue);
        }

        public virtual void TestDecodeNull() {
            object newValue = encodeableFacet.FromEncodedString(EncodeableFacetUsingEncoderDecoder<object>.EncodedNull, Manager);
            Assert.IsNull(newValue);
        }

        public virtual void TestEmptyEncoding() {
            Assert.AreEqual(EncodeableFacetUsingEncoderDecoder<object>.EncodedNull, encodeableFacet.ToEncodedString(null));
        }
    }
}