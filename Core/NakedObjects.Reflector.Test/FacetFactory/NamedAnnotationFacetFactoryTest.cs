// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class NamedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NamedAnnotationFacetFactory(Reflector);
            //BasicConfigurator.Configure(new WarningAppender());
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private NamedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INamedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Named("some name")]
        private class Customer {}

        private class Customer1 {
            [Named("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer11 {
            [Named("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }

            [Named("some name")]
            public int NumberOfOrders1 {
                get { return 0; }
            }
        }

        private class Customer2 {
            [Named("some name")]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer3 {
            [Named("some name")]
            public void SomeAction() {}
        }

        private class Customer13 {
            [Named("some name")]
            public void SomeAction() {}

            [Named("some name")]
            public void SomeAction1() {}
        }

        private class Customer4 {
            public void SomeAction([Named("some name")] int x) {}
        }

        [DisplayName("some name")]
        private class Customer5 {}

        private class Customer6 {
            [DisplayName("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer16 {
            [DisplayName("some name")]
            public int NumberOfOrders {
                get { return 0; }
            }

            [DisplayName("some name")]
            public int NumberOfOrders1 {
                get { return 0; }
            }
        }

        private class Customer7 {
            [DisplayName("some name")]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer8 {
            [DisplayName("some name")]
            public void SomeAction() {}
        }

        private class Customer18 {
            [DisplayName("some name")]
            public void SomeAction() {}

            [DisplayName("some name")]
            public void SomeAction1() {}
        }

        private readonly ISpecificationBuilder facetHolder1 = new Mock<ISpecificationBuilder>().Object;


        [Test]
        public void ATestDisplayNameAnnotationOnActionIgnoresDuplicate() {
            // these need to run before logs are added by other tests 
            MethodInfo actionMethod = FindMethod(typeof (Customer18), "SomeAction");
            MethodInfo actionMethod1 = FindMethod(typeof (Customer18), "SomeAction1");


            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(actionMethod1, MethodRemover, facetHolder1);
            facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void ATestNamedAnnotationOnActionIgnoresDuplicate() {
            // these need to run before logs are added by other tests 
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "SomeAction");
            MethodInfo actionMethod1 = FindMethod(typeof (Customer13), "SomeAction1");


            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(actionMethod1, MethodRemover, facetHolder1);
            facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationOnPropertyMarksDuplicate() {
            PropertyInfo property = FindProperty(typeof (Customer16), "NumberOfOrders");
            PropertyInfo property1 = FindProperty(typeof (Customer16), "NumberOfOrders1");


            facetFactory.Process(property, MethodRemover, Specification);
            facetFactory.Process(property1, MethodRemover, facetHolder1);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            //Assert.Contains("Duplicate name: some name found on type: NakedObjects.Reflector.DotNet.Facets.Naming.Named.NamedAnnotationFacetFactoryTest+Customer16", NakedObjectsContext.InitialisationWarnings);
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer8), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer5), "someAction");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisplayNameAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer6), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestNamedAnnotationOnPropertyMarksDuplicate() {
            PropertyInfo property = FindProperty(typeof (Customer11), "NumberOfOrders");
            PropertyInfo property1 = FindProperty(typeof (Customer11), "NumberOfOrders1");


            facetFactory.Process(property, MethodRemover, Specification);
            facetFactory.Process(property1, MethodRemover, facetHolder1);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            // Assert.Contains("Duplicate name: some name found on type: NakedObjects.Reflector.DotNet.Facets.Naming.Named.NamedAnnotationFacetFactoryTest+Customer11", NakedObjectsContext.InitialisationWarnings);
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(actionMethod, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "someAction");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNamedAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}