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
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    [TestFixture]
    public class DefaultedFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DefaultedFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private DefaultedFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INamedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        public void TestDefaultedMustBeADefaultsProvider() {
            // no test, because compiler prevents us from nominating a class that doesn't
            // implement DefaultsProvider
        }

        public abstract class DefaultsProviderNoop<T> : IDefaultsProvider<T> {
            #region IDefaultsProvider<T> Members

            public abstract T DefaultValue { get; }

            #endregion
        }

        [Defaulted(DefaultsProviderName = "NakedObjects.Reflector.DotNet.Facets.Objects.Defaults.DefaultedFacetFactoryTest+MyDefaultedUsingDefaultsProvider")]
        public class MyDefaultedUsingDefaultsProvider : DefaultsProviderNoop<MyDefaultedUsingDefaultsProvider> {
            // Required since is a DefaultsProvider.

            public override MyDefaultedUsingDefaultsProvider DefaultValue {
                get { return new MyDefaultedUsingDefaultsProvider(); }
            }
        }

        [Defaulted(DefaultsProviderClass = typeof (MyDefaultedUsingDefaultsProviderClass))]
        public class MyDefaultedUsingDefaultsProviderClass : DefaultsProviderNoop<MyDefaultedUsingDefaultsProviderClass> {
            // Required since is a DefaultsProvider.

            public override MyDefaultedUsingDefaultsProviderClass DefaultValue {
                get { return new MyDefaultedUsingDefaultsProviderClass(); }
            }
        }

        [Defaulted]
        public class MyDefaultedWithDefaultsProviderSpecifiedUsingConfiguration : DefaultsProviderNoop<MyDefaultedWithDefaultsProviderSpecifiedUsingConfiguration> {
            // Required since is a DefaultsProvider.

            public override MyDefaultedWithDefaultsProviderSpecifiedUsingConfiguration DefaultValue {
                get { return new MyDefaultedWithDefaultsProviderSpecifiedUsingConfiguration(); }
            }
        }

        [Defaulted(DefaultsProviderClass = typeof (MyDefaultedWithoutNoArgConstructor))]
        public class MyDefaultedWithoutNoArgConstructor : DefaultsProviderNoop<MyDefaultedWithoutNoArgConstructor> {
            // no no-arg constructor

            public MyDefaultedWithoutNoArgConstructor(int value) {}

            public override MyDefaultedWithoutNoArgConstructor DefaultValue {
                get { return new MyDefaultedWithoutNoArgConstructor(0); }
            }
        }

        [Defaulted(DefaultsProviderClass = typeof (MyDefaultedWithoutPublicNoArgConstructor))]
        public class MyDefaultedWithoutPublicNoArgConstructor : DefaultsProviderNoop<MyDefaultedWithoutPublicNoArgConstructor> {
            // no public no-arg constructor
            private MyDefaultedWithoutPublicNoArgConstructor() {}

            public MyDefaultedWithoutPublicNoArgConstructor(int value) {}


            public override MyDefaultedWithoutPublicNoArgConstructor DefaultValue {
                get { return new MyDefaultedWithoutPublicNoArgConstructor(); }
            }
        }

        public class NonAnnotatedDefaultedDefaultsProviderSpecifiedUsingConfiguration : DefaultsProviderNoop<NonAnnotatedDefaultedDefaultsProviderSpecifiedUsingConfiguration> {
            // Required since is a DefaultsProvider.

            public override NonAnnotatedDefaultedDefaultsProviderSpecifiedUsingConfiguration DefaultValue {
                get { return new NonAnnotatedDefaultedDefaultsProviderSpecifiedUsingConfiguration(); }
            }
        }

        [Test]
        public void TestDefaultedHaveAPublicNoArgConstructor() {
            facetFactory.Process(typeof (MyDefaultedWithoutPublicNoArgConstructor), MethodRemover, Specification);
            var facet = (DefaultedFacetAbstract<MyDefaultedWithoutPublicNoArgConstructor>) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestDefaultedMustHaveANoArgConstructor() {
            facetFactory.Process(typeof (MyDefaultedWithoutNoArgConstructor), MethodRemover, Specification);
            var facet = (DefaultedFacetAbstract<MyDefaultedWithoutNoArgConstructor>) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestDefaultedUsingDefaultsProviderClass() {
            facetFactory.Process(typeof (MyDefaultedUsingDefaultsProviderClass), MethodRemover, Specification);
            var facet = (DefaultedFacetAbstract<MyDefaultedUsingDefaultsProviderClass>) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.AreEqual(typeof (MyDefaultedUsingDefaultsProviderClass), facet.GetDefaultsProviderClass());
        }

        [Test]
        public void TestDefaultedUsingDefaultsProviderName() {
            facetFactory.Process(typeof (MyDefaultedUsingDefaultsProvider), MethodRemover, Specification);
            var facet = (DefaultedFacetAbstract<MyDefaultedUsingDefaultsProvider>) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.AreEqual(typeof (MyDefaultedUsingDefaultsProvider), facet.GetDefaultsProviderClass());
        }

        [Test]
        public void TestFacetFacetHolderStored() {
            facetFactory.Process(typeof (MyDefaultedUsingDefaultsProvider), MethodRemover, Specification);
            var valueFacet = (DefaultedFacetAbstract<MyDefaultedUsingDefaultsProvider>) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.AreEqual(Specification, valueFacet.Specification);
        }

        [Test]
        public void TestFacetPickedUp() {
            facetFactory.Process(typeof (MyDefaultedUsingDefaultsProvider), MethodRemover, Specification);

            var facet = (IDefaultedFacet) Specification.GetFacet(typeof (IDefaultedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DefaultedFacetAbstract<MyDefaultedUsingDefaultsProvider>);
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.ActionParameter));
        }

        [Test]
        public void TestNoMethodsRemoved() {
            facetFactory.Process(typeof (MyDefaultedUsingDefaultsProvider), MethodRemover, Specification);
            AssertNoMethodsRemoved();
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}