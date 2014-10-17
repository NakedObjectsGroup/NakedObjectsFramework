// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class IteratorFilteringFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new IteratorFilteringFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private IteratorFilteringFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        // ReSharper disable UnusedMember.Local
        // ReSharper disable InconsistentNaming
        // ReSharper disable AssignNullToNotNullAttribute
        private class Customer : IEnumerable {
            #region IEnumerable Members

            public IEnumerator GetEnumerator() {
                return null;
            }

            #endregion

            public void someAction() {}
        }

        private class Customer1 {
            public void someAction() {}
        }

        // ReSharper restore AssignNullToNotNullAttribute
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local

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
        public void TestRequestsRemoverToRemoveIteratorMethods() {
            MethodInfo enumeratorMethod = FindMethod(typeof (Customer), "GetEnumerator");
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            AssertMethodRemoved(enumeratorMethod);
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}