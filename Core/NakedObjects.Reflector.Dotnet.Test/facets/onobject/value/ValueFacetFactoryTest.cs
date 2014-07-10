// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.EqualByContent;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    [TestFixture]
    public class ValueFacetFactoryTest : AbstractFacetFactoryTest {
        private ValueFacetFactory facetFactory;
       

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IValueFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ValueFacetFactory { Reflector = reflector };         
           
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

       [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }


       [Test]
        public void TestFacetPickedUp() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), methodRemover, facetHolder);

            var facet = (IValueFacet) facetHolder.GetFacet(typeof (IValueFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ValueFacetAnnotation<MyParseableUsingParserName2>);
        }

       [Test]
        public void TestFacetFacetHolderStored() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), methodRemover, facetHolder);

            var valueFacet = (ValueFacetAnnotation<MyParseableUsingParserName2>)facetHolder.GetFacet(typeof(IValueFacet));
            Assert.AreEqual(facetHolder, valueFacet.FacetHolder);
        }

       [Test]
        public void TestNoMethodsRemoved() {
            facetFactory.Process(typeof (MyParseableUsingParserName2), methodRemover, facetHolder);

            AssertNoMethodsRemoved();
        }


       [Test]
        public void TestPickUpSemanticsProviderViaNameAndInstallsValueFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderName), methodRemover, facetHolder);

            Assert.IsNotNull(facetHolder.GetFacet(typeof (IValueFacet)));
        }

       [Test]
        public void TestPickUpSemanticsProviderViaClassAndInstallsValueFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), methodRemover, facetHolder);

            Assert.IsNotNull(facetHolder.GetFacet(typeof (IValueFacet)));
        }

       [Test]
        public void TestValueSemanticsProviderMustBeAValueSemanticsProvider() {
            // no test, because compiler prevents us from nominating a class that doesn't
            // implement ValueSemanticsProvider
        }


       [Test]
        public void TestValueSemanticsProviderMustHaveANoArgConstructor() {
            facetFactory.Process(typeof (MyValueSemanticsProviderWithoutNoArgConstructor), methodRemover, facetHolder);

            // the fact that we have an immutable means that the provider wasn't picked up
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IImmutableFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderMustHaveAPublicNoArgConstructor() {
            facetFactory.Process(typeof (MyValueSemanticsProviderWithoutPublicNoArgConstructor), methodRemover, facetHolder);

            // the fact that we have an immutable means that the provider wasn't picked up
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IImmutableFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsNotAParserDoesNotInstallParseableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), methodRemover, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (IParseableFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsParseableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IParseableFacet)));
        }

       [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsTitleFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (ITitleFacet)));
        }

       [Test]
        public void TestValueSemanticsProviderThatIsAParserInstallsTypicalLengthFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAParser), methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (ITypicalLengthFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsNotADefaultsProviderDoesNotInstallDefaultedFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), methodRemover, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (IDefaultedFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsADefaultsProviderInstallsDefaultedFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsADefaultsProvider), methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IDefaultedFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsNotAnEncoderDecoderDoesNotInstallEncodeableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderUsingSemanticsProviderClass), methodRemover, facetHolder);

            Assert.IsNull(facetHolder.GetFacet(typeof (IEncodeableFacet)));
        }


       [Test]
        public void TestValueSemanticsProviderThatIsAnEncoderInstallsEncodeableFacet() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatIsAnEncoderDecoder), methodRemover, facetHolder);

            Assert.IsNotNull(facetHolder.GetFacet(typeof (IEncodeableFacet)));
        }


       [Test]
        public void TestImmutableFacetsIsInstalledIfNoSemanticsProviderSpecified() {
            facetFactory.Process(typeof (MyNumberImmutableDefault), methodRemover, facetHolder);

            var facet = (IImmutableFacet) facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
        }


       [Test]
        public void TestImmutableFacetsIsInstalledIfSpecifiesImmutable() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesImmutableSemantic), methodRemover, facetHolder);

            var facet = (IImmutableFacet) facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
        }


       [Test]
        public void TestImmutableFacetsIsNotInstalledIfSpecifiesNotImmutable() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic), methodRemover, facetHolder);

            var facet = (IImmutableFacet) facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNull(facet);
        }

       [Test]
        public void TestEqualByContentFacetsIsInstalledIfNoSemanticsProviderSpecified() {
            facetFactory.Process(typeof (MyNumberEqualByContentDefault), methodRemover, facetHolder);

            var facet = (IEqualByContentFacet) facetHolder.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNotNull(facet);
        }


       [Test]
        public void TestEqualByContentFacetsIsInstalledIfSpecifiesEqualByContent() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic), methodRemover, facetHolder);

            var facet = (IEqualByContentFacet) facetHolder.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNotNull(facet);
        }


       [Test]
        public void TestEqualByContentFacetsIsNotInstalledIfSpecifiesNotEqualByContent() {
            facetFactory.Process(typeof (MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic), methodRemover, facetHolder);
            var facet = (IEqualByContentFacet) facetHolder.GetFacet(typeof (IEqualByContentFacet));
            Assert.IsNull(facet);
        }

        #region Nested Type: MyNumberEqualByContentDefault

        [Value()]
        private class MyNumberEqualByContentDefault {}

        #endregion

        #region Nested Type: MyNumberImmutableDefault

        [Value()]
        private class MyNumberImmutableDefault {}

        #endregion

        #region Nested Type: MyParseableUsingParserName2

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyParseableUsingParserName2")]
        public class MyParseableUsingParserName2 : ValueSemanticsProviderImpl<MyParseableUsingParserName2> {
           
             // Required since is a IParser.
       
            public MyParseableUsingParserName2() {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatIsADefaultsProvider

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsADefaultsProvider")]
        public class MyValueSemanticsProviderThatIsADefaultsProvider : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsADefaultsProvider>, IDefaultsProvider<MyValueSemanticsProviderThatIsADefaultsProvider> {
           
             // Required since is a ValueSemanticsProvider.
            
            public MyValueSemanticsProviderThatIsADefaultsProvider() {}

            #region IDefaultsProvider Members

            public MyValueSemanticsProviderThatIsADefaultsProvider DefaultValue {
                get { return new MyValueSemanticsProviderThatIsADefaultsProvider(); }
            }

            #endregion
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatIsAnEncoderDecoder

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsAnEncoderDecoder")]
        public class MyValueSemanticsProviderThatIsAnEncoderDecoder : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsAnEncoderDecoder>, IEncoderDecoder<MyValueSemanticsProviderThatIsAnEncoderDecoder> {
        
             // Required since is a ValueSemanticsProvider.
            
            public MyValueSemanticsProviderThatIsAnEncoderDecoder() {}

            #region IEncoderDecoder Members

            public MyValueSemanticsProviderThatIsAnEncoderDecoder FromEncodedString(string encodedString) {
                return null;
            }

            public string ToEncodedString(MyValueSemanticsProviderThatIsAnEncoderDecoder toEncode) {
                return null;
            }

            #endregion
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatIsAParser

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatIsAParser")]
        public class MyValueSemanticsProviderThatIsAParser : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatIsAParser>, IParser<MyValueSemanticsProviderThatIsAParser> {
            
             // Required since is a ValueSemanticsProvider.
            
            public MyValueSemanticsProviderThatIsAParser() {}

            #region IParser Members

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

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic> {
          
             // Required since is a ValueSemanticsProvider.
            

            public MyValueSemanticsProviderThatSpecifiesEqualByContentSemantic()
                : base(true, true) {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatSpecifiesImmutableSemantic

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesImmutableSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesImmutableSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesImmutableSemantic> {
           
             // Required since is a ValueSemanticsProvider.
           

            public MyValueSemanticsProviderThatSpecifiesImmutableSemantic()
                : base(true, true) {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic> {
           
             // Required since is a ValueSemanticsProvider.
           

            public MyValueSemanticsProviderThatSpecifiesNotEqualByContentSemantic()
                : base(false, false) {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic")]
        public class MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic : ValueSemanticsProviderImpl<MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic> {
          
             // Required since is a ValueSemanticsProvider.
            

            public MyValueSemanticsProviderThatSpecifiesNotImmutableSemantic()
                : base(false, true) {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderUsingSemanticsProviderClass

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderUsingSemanticsProviderClass))]
        public class MyValueSemanticsProviderUsingSemanticsProviderClass : ValueSemanticsProviderImpl<MyValueSemanticsProviderUsingSemanticsProviderClass> {
           
             // Required since is a ValueSemanticsProvider.
             
            public MyValueSemanticsProviderUsingSemanticsProviderClass() {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderUsingSemanticsProviderName

        [Value(SemanticsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Value.ValueFacetFactoryTest+MyValueSemanticsProviderUsingSemanticsProviderName")]
        public class MyValueSemanticsProviderUsingSemanticsProviderName : ValueSemanticsProviderImpl<MyValueSemanticsProviderUsingSemanticsProviderName> {
           
             // Required since is a ValueSemanticsProvider.
            
            public MyValueSemanticsProviderUsingSemanticsProviderName() {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderWithoutNoArgConstructor

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderWithoutNoArgConstructor))]
        public class MyValueSemanticsProviderWithoutNoArgConstructor : ValueSemanticsProviderImpl<MyValueSemanticsProviderWithoutNoArgConstructor> {
            // no no-arg constructor

            // pass in false for an immutable, which isn't the default
            public MyValueSemanticsProviderWithoutNoArgConstructor(int value)
                : base(false, false) {}
        }

        #endregion

        #region Nested Type: MyValueSemanticsProviderWithoutPublicNoArgConstructor

        [Value(SemanticsProviderClass = typeof (MyValueSemanticsProviderWithoutPublicNoArgConstructor))]
        public class MyValueSemanticsProviderWithoutPublicNoArgConstructor : ValueSemanticsProviderImpl<MyValueSemanticsProviderWithoutPublicNoArgConstructor> {
            // no public no-arg constructor

            // pass in false for an immutable, which isn't the default
            private MyValueSemanticsProviderWithoutPublicNoArgConstructor()
                : base(false, false) {}

            public MyValueSemanticsProviderWithoutPublicNoArgConstructor(int value) {}
        }

        #endregion

        #region Nested Type: MyValueWithSemanticsProviderSpecifiedUsingConfiguration

        [Value()]
        public class MyValueWithSemanticsProviderSpecifiedUsingConfiguration : ValueSemanticsProviderImpl<MyValueWithSemanticsProviderSpecifiedUsingConfiguration>, IParser<MyValueWithSemanticsProviderSpecifiedUsingConfiguration> {
           
             // Required since is a SemanticsProvider.
            
            public MyValueWithSemanticsProviderSpecifiedUsingConfiguration() {}

            #region IParser Members

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

        #endregion

        #region Nested Type: NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration

        public class NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration : ValueSemanticsProviderImpl<NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration>, IParser<NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration> {
            
             // Required since is a SemanticsProvider.
             
            public NonAnnotatedValueSemanticsProviderSpecifiedUsingConfiguration() {}

            #region IParser Members

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

        #endregion
    }
}