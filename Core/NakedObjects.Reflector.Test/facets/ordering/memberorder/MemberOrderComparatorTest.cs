// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedObjects.Architecture.Component;
using NUnit.Framework;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Utils;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    [TestFixture]
    public class MemberOrderComparatorTest {
        #region Setup/Teardown

        [SetUp]
        public virtual void SetUp() {
            comparator = new MemberOrderComparator<MemberPeerStub>(true);
            laxComparator = new MemberOrderComparator<MemberPeerStub>(false);
        }

        #endregion

        private MemberOrderComparator<MemberPeerStub> comparator;
        private MemberOrderComparator<MemberPeerStub> laxComparator;

        private MemberPeerStub m1;
        private MemberPeerStub m2;

        public MemberOrderComparatorTest() {
            Reset();
        }

        private void Reset() {
            var p = new Mock<ILifecycleManager>().Object;
            m1 = new MemberPeerStub("abc", p);
            m2 = new MemberPeerStub("abc", p);
        }


        [Test]
        public virtual void TestDefaultGroupOneComponent() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneComponentOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "2", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneComponentSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.4", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.4", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirst() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.1", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirstOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsTwoComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsTwoComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("", "1.2", m1));
            m2.AddFacet(new MemberOrderFacet("", "1.1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestEnsuresInSameGroup() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("abc", "1", m1));
            m2.AddFacet(new MemberOrderFacet("def", "2", m2));
            try {
                Assert.AreEqual(-1, comparator.Compare(m1, m2));
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException /*expected*/) {}
        }

        [Test]
        public void TestEnsuresInSameGroupCanBeDisabled() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("abc", "1", m1));
            m2.AddFacet(new MemberOrderFacet("def", "2", m2));
            Assert.AreEqual(-1, laxComparator.Compare(m1, m2));
        }

        [Test]
        public void TestNamedGroupOneSideRunsLotsOfComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacet("abc", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacet("abc", "1.2.5.8.3.4", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestNonAnnotatedAfterAnnotated() {
            Reset();
            // don't annotate m1
            m2.AddFacet(new MemberOrderFacet("def", "2", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}