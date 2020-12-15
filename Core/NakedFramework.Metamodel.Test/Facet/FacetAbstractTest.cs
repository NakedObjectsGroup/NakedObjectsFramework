﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Test.Facet {
    [TestClass]
    public class FacetAbstractTest {
        private ISpecification facetHolder2;
        private FacetAbstract fooFacet;
        private ISpecification specification;

        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() {
            specification = new Mock<ISpecificationBuilder>().Object;
            facetHolder2 = new Mock<ISpecification>().Object;
            fooFacet = new ConcreteFacet(typeof(IFooFacet), specification);
            FacetUtils.AddFacet(fooFacet);
        }

        #endregion

        [TestMethod]
        public void FacetType() {
            Assert.AreEqual(typeof(IFooFacet), fooFacet.FacetType);
        }

        [TestMethod]
        public void GetFacetHolder() {
            Assert.AreEqual(specification, fooFacet.Specification);
        }

        [TestMethod]
        public void SetFacetHolder() {
            fooFacet.Specification = facetHolder2;
            Assert.AreEqual(facetHolder2, fooFacet.Specification);
        }

        [TestMethod]
        public void TestToString() {
            Assert.AreEqual("FacetAbstractTest+ConcreteFacet[type=FacetAbstractTest+IFooFacet]", fooFacet.ToString());
        }

        #region Nested type: ConcreteFacet

        internal class ConcreteFacet : FacetAbstract, IFooFacet {
            public ConcreteFacet(Type facetType, ISpecification holder) : base(facetType, holder) { }
        }

        #endregion

        #region Nested type: IFooFacet

        public interface IFooFacet : IFacet { }

        #endregion
    }
}