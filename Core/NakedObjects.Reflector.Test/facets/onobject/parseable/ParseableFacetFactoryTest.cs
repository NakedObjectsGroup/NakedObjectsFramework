// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Capabilities;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    [TestFixture]
    public class ParseableFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ParseableFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ParseableFacetFactory facetFactory;


        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IParseableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Parseable(ParserClass = typeof (MyParseableUsingParserClass))]
        public class MyParseableUsingParserClass : ParserNoop<MyParseableUsingParserClass> {
            // Required since is a IParser.
        }

        [Parseable(ParserName = "NakedObjects.Reflector.DotNet.Facets.Objects.Parseable.ParseableFacetFactoryTest+MyParseableUsingParserName")]
        public class MyParseableUsingParserName : ParserNoop<MyParseableUsingParserName> {
            // Required since is a IParser.
        }

        [Parseable(ParserClass = typeof (MyParseableWithoutNoArgConstructor))]
        public class MyParseableWithoutNoArgConstructor : ParserNoop<MyParseableWithoutNoArgConstructor> {
            // no no-arg constructor

            public MyParseableWithoutNoArgConstructor(int value) {}
        }

        [Parseable(ParserClass = typeof (MyParseableWithoutPublicNoArgConstructor))]
        public class MyParseableWithoutPublicNoArgConstructor : ParserNoop<MyParseableWithoutPublicNoArgConstructor> {
            // no public no-arg constructor
            private MyParseableWithoutPublicNoArgConstructor()
                : this(0) {}

            public MyParseableWithoutPublicNoArgConstructor(int value) {}
        }

        [Parseable]
        public class MyParseableWithParserSpecifiedUsingConfiguration : ParserNoop<MyParseableWithParserSpecifiedUsingConfiguration> {
            // Required since is a IParser.
        }

        public class NonAnnotatedParseableParserSpecifiedUsingConfiguration : ParserNoop<NonAnnotatedParseableParserSpecifiedUsingConfiguration> {
            // Required since is a IParser.
        }

        public class ParserNoop<T> : IParser<T> {
            #region IParser<T> Members

            public object ParseTextEntry(string entry) {
                return null;
            }

            public object ParseInvariant(string entry) {
                return null;
            }

            public string InvariantString(T obj) {
                throw new NotImplementedException();
            }

            public int TypicalLength {
                get { return 0; }
            }

            public string DisplayTitleOf(T obj) {
                return null;
            }

            public string EditableTitleOf(T existing) {
                return null;
            }

            public string TitleWithMaskOf(string mask, T obj) {
                return null;
            }

            #endregion
        }

        [Test]
        public void TestFacetFacetHolderStored() {
            facetFactory.Process(typeof (MyParseableUsingParserName), MethodRemover, Specification);

            var parseableFacet = (IParseableFacet) Specification.GetFacet(typeof (IParseableFacet));
            Assert.AreEqual(Specification, parseableFacet.Specification);
        }

        [Test]
        public void TestFacetPickedUp() {
            facetFactory.Process(typeof (MyParseableUsingParserName), MethodRemover, Specification);

            var facet = (IParseableFacet) Specification.GetFacet(typeof (IParseableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ParseableFacetAbstract<MyParseableUsingParserName>);
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
        public void TestNoMethodsRemoved() {
            facetFactory.Process(typeof (MyParseableUsingParserName), MethodRemover, Specification);

            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestParseableHaveANoArgConstructor() {
            facetFactory.Process(typeof (MyParseableWithoutNoArgConstructor), MethodRemover, Specification);

            var parseableFacet = (ParseableFacetAnnotation<MyParseableUsingParserName>) Specification.GetFacet(typeof (IParseableFacet));
            Assert.IsNull(parseableFacet);
        }


        [Test]
        public void TestParseableHaveAPublicNoArgConstructor() {
            facetFactory.Process(typeof (MyParseableWithoutPublicNoArgConstructor), MethodRemover, Specification);

            var parseableFacet = (IParseableFacet) Specification.GetFacet(typeof (IParseableFacet));
            Assert.IsNull(parseableFacet);
        }

        [Test]
        public void TestParseableMustBeAParser() {
            // no test, because compiler prevents us from nominating a class that doesn't
            // implement IParser
        }

        [Test]
        public void TestParseableUsingParserClass() {
            facetFactory.Process(typeof (MyParseableUsingParserClass), MethodRemover, Specification);

            var parseableFacet = (ParseableFacetAnnotation<MyParseableUsingParserClass>) Specification.GetFacet(typeof (IParseableFacet));
            Assert.AreEqual(typeof (MyParseableUsingParserClass), parseableFacet.GetParserClass());
        }

        [Test]
        public void TestParseableUsingParserName() {
            facetFactory.Process(typeof (MyParseableUsingParserName), MethodRemover, Specification);

            var parseableFacet = (ParseableFacetAnnotation<MyParseableUsingParserName>) Specification.GetFacet(typeof (IParseableFacet));
            Assert.AreEqual(typeof (MyParseableUsingParserName), parseableFacet.GetParserClass());
        }
    }
}