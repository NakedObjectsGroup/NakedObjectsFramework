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
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class ValueFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ValueFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ValueFacetFactory facetFactory;


        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IValueFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Value]
        private class MyNumberEqualByContentDefault {}

        [Value]
        private class MyNumberImmutableDefault {}

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyParseableUsingParserName2")]
        public class MyParseableUsingParserName2 : ValueSemanticsProviderImpl<MyParseableUsingParserName2> {
            // Required since is a IParser.
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsADefaultsProvider")]
        public class MyValueSemanticsProviderThatIsADefaultsProvider : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsADefaultsProvider>, IDefaultsProvider<MyValueSemanticsProviderThatIsADefaultsProvider> {
            // Required since is a ValueSemanticsProvider.

            #region IDefaultsProvider<MyValueSemanticsProviderThatIsADefaultsProvider> Members

            public MyValueSemanticsProviderThatIsADefaultsProvider DefaultValue {
                get { return new MyValueSemanticsProviderThatIsADefaultsProvider(); }
            }

            #endregion
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsAnEncoderDecoder")]
        public class MyValueSemanticsProviderThatIsAnEncoderDecoder : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsAnEncoderDecoder>, IEncoderDecoder<MyValueSemanticsProviderThatIsAnEncoderDecoder> {
            // Required since is a ValueSemanticsProvider.

            #region IEncoderDecoder<MyValueSemanticsProviderThatIsAnEncoderDecoder> Members

            public MyValueSemanticsProviderThatIsAnEncoderDecoder FromEncodedString(string encodedString) {
                return null;
            }

            public string ToEncodedString(MyValueSemanticsProviderThatIsAnEncoderDecoder toEncode) {
                return null;
            }

            #endregion
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsAParser")]
        public class MyValueSemanticsProviderThatIsAParser : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsAParser>, IParser<MyValueSemanticsProviderThatIsAParser> {
            // Required since is a ValueSemanticsProvider.

            #region IParser<MyValueSemanticsProviderThatIsAParser> Members

            public object ParseTextEntry(string entry) {
                return null;
            }

            public object ParseInvariant(string entry) {
                return null;
            }

            public string InvariantString(MyValueSemanticsProviderThatIsAParser obj) {
                throw new NotImplementedException();
            }

            public string DisplayTitleOf(MyValueSemanticsProviderThatIsAParser obj) {
                return null;
            }

            public string EditableTitleOf(MyValueSemanticsProviderThatIsAParser existing) {
                return null;
            }

            public string TitleWithMaskOf(string mask, MyValueSemanticsProviderThatIsAParser obj) {
                return null;
            }

            public int TypicalLength {
                get { return 0; }
            }

            #endregion
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic> {
            // Required since is a ValueSemanticsProvider.


            public MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic()
                : base(true, true) {}
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesImmutableSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesImmutableSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesImmutableSemantic> {
            // Required since is a ValueSemanticsProvider.


            public MyValueSemanticsProviderThatSpecifiesImmutableSemantic()
                : base(true, true) {}
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic> {
            // Required since is a ValueSemanticsProvider.


            public MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic()
                : base(false, false) {}
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic> {
            // Required since is a ValueSemanticsProvider.


            public MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic()
                : base(false, true) {}
        }

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderUsingSemanticsProviderClass))]
        public class MyValueSemanticsProviderUsingSemanticsProviderClass : ValueSemanticsProviderImpl<MyValueSemanticsProviderUsingSemanticsProviderClass> {
            // Required since is a ValueSemanticsProvider.
        }

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderUsingSemanticsProviderName")]
        public class MyValueSemanticsProviderUsingSemanticsProviderName : ValueSemanticsProviderImpl<MyValueSemanticsProviderUsingSemanticsProviderName> {
            // Required since is a ValueSemanticsProvider.
        }

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderWithoutNoArgConstructor))]
        public class MyValueSemanticsProviderWithoutNoArgConstructor : ValueSemanticsProviderImpl<MyValueSemanticsProviderWithoutNoArgConstructor> {
            // no no-arg constructor

            // pass in false for an immutable, which isn't the default
            public MyValueSemanticsProviderWithoutNoArgConstructor(int value)
                : base(false, false) {}
        }

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderWithoutPublicNoArgConstructor))]
        public class MyValueSemanticsProviderWithoutPublicNoArgConstructor : ValueSemanticsProviderImpl<MyValueSemanticsProviderWithoutPublicNoArgConstructor> {
            // no public no-arg constructor

            // pass in false for an immutable, which isn't the default
            private MyValueSemanticsProviderWithoutPublicNoArgConstructor()
                : base(false, false) {}

            public MyValueSemanticsProviderWithoutPublicNoArgConstructor(int value) {}
        }

        [Value]
        public class MyValueWithSemanticsProviderSpecifiedUsingConfiguration : ValueSemanticsProviderImpl<MyValueWithSemanticsProviderSpecifiedUsingConfiguration>, IParser<MyValueWithSemanticsProviderSpecifiedUsingConfiguration> {
            // Required since is a SemanticsProvider.

            #region IParser<MyValueWithSemanticsProviderSpecifiedUsingConfiguration> Members

            public object ParseTextEntry(string entry) {
                return null;
            }

            public object ParseInvariant(string entry) {
                return null;
            }

            public string InvariantString(MyValueWithSemanticsProviderSpecifiedUsingConfiguration obj) {
                throw new NotImplementedException();
            }

            public string DisplayTitleOf(MyValueWithSemanticsProviderSpecifiedUsingConfiguration obj) {
                return null;
            }

            public string TitleWithMaskOf(string mask, MyValueWithSemanticsProviderSpecifiedUsingConfiguration obj) {
                return null;
            }

            public string EditableTitleOf(MyValueWithSemanticsProviderSpecifiedUsingConfiguration existing) {
                return null;
            }

            public int TypicalLength {
                get { return 0; }
            }

            #endregion
        }

        public class NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration : ValueSemanticsProviderImpl<NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration>, IParser<NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration> {
            // Required since is a SemanticsProvider.

            #region IParser<NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration> Members

            public object ParseTextEntry(string entry) {
                return null;
            }

            public object ParseInvariant(string entry) {
                return null;
            }

            public string InvariantString(NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration obj) {
                throw new NotImplementedException();
            }

            public string DisplayTitleOf(NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration obj) {
                return null;
            }

            public string TitleWithMaskOf(string mask, NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration obj) {
                return null;
            }

            public string EditableTitleOf(NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration existing) {
                return null;
            }

            public int TypicalLength {
                get { return 0; }
            }

            #endregion
        }

        [Test]
        public void TestEqualByContentFacetsIsInstalledIfNoSemanticsProviderSpecified() {
            facetFactory.Process(typeof (MyNumberEqualByContentDefault), MethodRemover, Specification);

            var facet = (IEqualByContentFacet) Specification.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNotNull(facet);
        }


        [Test]
        public void TestEqualByContentFacetsIsInstalledIfSpecifiesEqualByContent() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic), MethodRemover, Specification);

            var facet = (IEqualByContentFacet) Specification.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNotNull(facet);
        }


        [Test]
        public void TestEqualByContentFacetsIsNotInstalledIfSpecifiesNotEqualByContent() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic), MethodRemover, Specification);
            var facet = (IEqualByContentFacet) Specification.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestFacetFacetHolderStored() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), MethodRemover, Specification);

            var valueFacet = (ValueFacetAnnotation<MyParseableUsingParserName2>) Specification.GetFacet(typeof (IValueFacet));
            Assert.AreEqual(Specification, valueFacet.Specification);
        }

        [Test]
        public void TestFacetPickedUp() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), MethodRemover, Specification);

            var facet = (IValueFacet) Specification.GetFacet(typeof (IValueFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ValueFacetAnnotation<MyParseableUsingParserName2>);
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestImmutableFacetsIsInstalledIfNoSemanticsProviderSpecified() {
            facetFactory.Process(typeof (MyNumberImmutableDefault), MethodRemover, Specification);

            var facet = (IImmutableFacet) Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
        }


        [Test]
        public void TestImmutableFacetsIsInstalledIfSpecifiesImmutable() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesImmutableSemantic), MethodRemover, Specification);

            var facet = (IImmutableFacet) Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
        }


        [Test]
        public void TestImmutableFacetsIsNotInstalledIfSpecifiesNotImmutable() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic), MethodRemover, Specification);

            var facet = (IImmutableFacet) Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestNoMethodsRemoved() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), MethodRemover, Specification);

            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestPickUpSemanticsProviderViaClassAndInstallsValueFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), MethodRemover, Specification);

            Assert.IsNotNull(Specification.GetFacet(typeof (IValueFacet)));
        }

        [Test]
        public void TestPickUpSemanticsProviderViaNameAndInstallsValueFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderName), MethodRemover, Specification);

            Assert.IsNotNull(Specification.GetFacet(typeof (IValueFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderMustBeAValueSemanticsProvider() {
            // no test, because compiler prevents us from nominating a class that doesn't
            // implement ValueSemanticsProvider
        }


        [Test]
        public void TestValueSemanticsProviderMustHaveANoArgConstructor() {
            facetFactory.Process(typeof (MyValueSemanticsProviderWithoutNoArgConstructor), MethodRemover, Specification);

            // the fact that we have an immutable means that the provider wasn't picked up
            Assert.IsNotNull(Specification.GetFacet(typeof (IImmutableFacet)));
        }


        [Test]
        public void TestValueSemanticsProviderMustHaveAPublicNoArgConstructor() {
            facetFactory.Process(typeof (MyValueSemanticsProviderWithoutPublicNoArgConstructor), MethodRemover, Specification);

            // the fact that we have an immutable means that the provider wasn't picked up
            Assert.IsNotNull(Specification.GetFacet(typeof (IImmutableFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsADefaultsProviderInstallsDefaultedFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsADefaultsProvider), MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IDefaultedFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsParseableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IParseableFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsTitleFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (ITitleFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsTypicalLengthFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (ITypicalLengthFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsAnEncoderInstallsEncodeableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAnEncoderDecoder), MethodRemover, Specification);

            Assert.IsNotNull(Specification.GetFacet(typeof (IEncodeableFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsNotADefaultsProviderDoesNotInstallDefaultedFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IDefaultedFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsNotAParserDoesNotInstallParseableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IParseableFacet)));
        }

        [Test]
        public void TestValueSemanticsProviderThatIsNotAnEncoderDecoderDoesNotInstallEncodeableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), MethodRemover, Specification);

            Assert.IsNull(Specification.GetFacet(typeof (IEncodeableFacet)));
        }
    }
}