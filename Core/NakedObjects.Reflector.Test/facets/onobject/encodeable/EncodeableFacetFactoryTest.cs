// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    [TestFixture]
    public class EncodeableFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new EncodeableFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private EncodeableFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IEncodeableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        public abstract class EncoderDecoderNoop<T> : IEncoderDecoder<T> {
            #region IEncoderDecoder<T> Members

            public T FromEncodedString(string encodedString) {
                return default(T);
            }

            public string ToEncodedString(T toEncode) {
                return null;
            }

            #endregion
        }

        [Encodeable(EncoderDecoderClass = typeof (MyEncodeableUsingEncoderDecoderClass))]
        public class MyEncodeableUsingEncoderDecoderClass : EncoderDecoderNoop<MyEncodeableUsingEncoderDecoderClass> {
            // Required since is a EncoderDecoder.
        }

        [Encodeable(EncoderDecoderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable.EncodeableFacetFactoryTest+MyEncodeableUsingEncoderDecoderName")]
        public class MyEncodeableUsingEncoderDecoderName : EncoderDecoderNoop<MyEncodeableUsingEncoderDecoderName> {
            // Required since is an EncoderDecoder
        }

        [Encodeable]
        public class MyEncodeableWithEncoderDecoderSpecifiedUsingConfiguration : EncoderDecoderNoop<MyEncodeableWithEncoderDecoderSpecifiedUsingConfiguration> {
            // Required since is a EncoderDecoder.
        }

        [Encodeable(EncoderDecoderClass = typeof (MyEncodeableWithoutNoArgConstructor))]
        public class MyEncodeableWithoutNoArgConstructor : EncoderDecoderNoop<MyEncodeableWithoutNoArgConstructor> {
            // no no-arg constructor

            public MyEncodeableWithoutNoArgConstructor(int value) {}
        }

        [Encodeable(EncoderDecoderClass = typeof (MyEncodeableWithoutPublicNoArgConstructor))]
        public class MyEncodeableWithoutPublicNoArgConstructor : EncoderDecoderNoop<MyEncodeableWithoutPublicNoArgConstructor> {
            // no public no-arg constructor
            private MyEncodeableWithoutPublicNoArgConstructor() {}

            public MyEncodeableWithoutPublicNoArgConstructor(int value) {}
        }

        public class NonAnnotatedEncodeableEncoderDecoderSpecifiedUsingConfiguration : EncoderDecoderNoop<NonAnnotatedEncodeableEncoderDecoderSpecifiedUsingConfiguration> {
            // Required since is a EncoderDecoder.
        }

        [Test]
        public void TestEncodeableHaveANoArgConstructor() {
            facetFactory.Process(typeof (MyEncodeableWithoutNoArgConstructor), MethodRemover, Specification);
            var encodeableFacet = (EncodeableFacet<MyEncodeableWithoutNoArgConstructor>) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.IsNull(encodeableFacet);
        }

        [Test]
        public void TestEncodeableHaveAPublicNoArgConstructor() {
            facetFactory.Process(typeof (MyEncodeableWithoutPublicNoArgConstructor), MethodRemover, Specification);

            var encodeableFacet = (EncodeableFacet<MyEncodeableWithoutPublicNoArgConstructor>) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.IsNull(encodeableFacet);
        }

        [Test]
        public void TestEncodeableMustBeAEncoderDecoder() {
            // no test, because compiler prevents us from nominating a class that doesn't
            // implement EncoderDecoder
        }

        [Test]
        public void TestEncodeableUsingEncoderDecoderClass() {
            facetFactory.Process(typeof (MyEncodeableUsingEncoderDecoderClass), MethodRemover, Specification);

            var encodeableFacet = (EncodeableFacet<MyEncodeableUsingEncoderDecoderClass>) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.AreEqual(typeof (MyEncodeableUsingEncoderDecoderClass), encodeableFacet.GetEncoderDecoderClass());
        }

        [Test]
        public void TestEncodeableUsingEncoderDecoderName() {
            facetFactory.Process(typeof (MyEncodeableUsingEncoderDecoderName), MethodRemover, Specification);
            var encodeableFacet = (EncodeableFacet<MyEncodeableUsingEncoderDecoderName>) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.AreEqual(typeof (MyEncodeableUsingEncoderDecoderName), encodeableFacet.GetEncoderDecoderClass());
        }

        [Test]
        public void TestFacetFacetHolderStored() {
            facetFactory.Process(typeof (MyEncodeableUsingEncoderDecoderName), MethodRemover, Specification);

            var valueFacet = (EncodeableFacet<MyEncodeableUsingEncoderDecoderName>) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.AreEqual(Specification, valueFacet.Specification);
        }

        [Test]
        public void TestFacetPickedUp() {
            facetFactory.Process(typeof (MyEncodeableUsingEncoderDecoderName), MethodRemover, Specification);

            var facet = (IEncodeableFacet) Specification.GetFacet(typeof (IEncodeableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EncodeableFacet<MyEncodeableUsingEncoderDecoderName>);
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.ActionParameter));
        }

        [Test]
        public void TestNoMethodsRemoved() {
            facetFactory.Process(typeof (MyEncodeableUsingEncoderDecoderName), MethodRemover, Specification);
            AssertNoMethodsRemoved();
        }
    }
}