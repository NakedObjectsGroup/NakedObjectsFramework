// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class PasswordAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private PasswordAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPasswordFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PasswordAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer1 {
            [DataType(DataType.Password)]
// ReSharper disable UnusedMember.Local
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([DataType(DataType.Password)] string foo) {}
        }

        private class Customer3 {
            [DataType(DataType.PhoneNumber)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer4 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([DataType(DataType.PhoneNumber)] string foo) {}
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestPasswordAnnotationNotPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestPasswordAnnotationNotPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof (Customer3), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestPasswordAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacet);
            Assert.IsNotNull(metamodel);

        }

        [TestMethod]
        public void TestPasswordAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IPasswordFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PasswordFacet);
            Assert.IsNotNull(metamodel);

        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}