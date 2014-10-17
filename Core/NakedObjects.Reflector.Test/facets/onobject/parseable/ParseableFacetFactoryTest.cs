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
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
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