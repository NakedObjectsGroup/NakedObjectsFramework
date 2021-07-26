// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class MemberOrderComparatorTest {
        private MemberOrderComparator<MemberPeerStub> comparator;
        private MemberPeerStub m1;
        private MemberPeerStub m2;

        public MemberOrderComparatorTest() => Reset();

        #region Setup/Teardown

        [TestInitialize]
        public virtual void SetUp() {
            comparator = new MemberOrderComparator<MemberPeerStub>();
        }

        #endregion

        private void Reset() {
            m1 = new MemberPeerStub("abc");
            m2 = new MemberPeerStub("abc");
        }

        [TestMethod]
        public virtual void TestDefaultGroupOneComponent() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneComponentOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "2", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneComponentSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsLotsOfComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.4", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.4", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirst() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.1", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirstOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsTwoComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [TestMethod]
        public void TestDefaultGroupOneSideRunsTwoComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}