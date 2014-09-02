// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    [TestFixture]
    public class MemberOrderComparatorTest {
        #region Setup/Teardown

        [SetUp]
        public virtual void SetUp() {
            comparator = new MemberOrderComparator(true);
            laxComparator = new MemberOrderComparator(false);
        }

        #endregion

        private MemberOrderComparator comparator;
        private MemberOrderComparator laxComparator;

        private MemberPeerStub m1 = new MemberPeerStub("abc");
        private MemberPeerStub m2 = new MemberPeerStub("abc");

        private void Reset() {
            m1 = new MemberPeerStub("abc");
            m2 = new MemberPeerStub("abc");
        }


        [Test]
        public virtual void TestDefaultGroupOneComponent() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneComponentOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "2", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneComponentSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.4", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.4", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsLotsOfComponentsSame() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.2.5.8.3.3", m2));
            Assert.AreEqual(0, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirst() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.1", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsOutOfComponentsFirstOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsTwoComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.2", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestDefaultGroupOneSideRunsTwoComponentsOtherWay() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("", "1.2", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("", "1.1", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestEnsuresInSameGroup() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("abc", "1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("def", "2", m2));
            try {
                Assert.AreEqual(-1, comparator.Compare(m1, m2));
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentException /*expected*/) {}
        }

        [Test]
        public void TestEnsuresInSameGroupCanBeDisabled() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("abc", "1", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("def", "2", m2));
            Assert.AreEqual(-1, laxComparator.Compare(m1, m2));
        }

        [Test]
        public void TestNamedGroupOneSideRunsLotsOfComponents() {
            Reset();
            m1.AddFacet(new MemberOrderFacetAnnotation("abc", "1.2.5.8.3.3", m1));
            m2.AddFacet(new MemberOrderFacetAnnotation("abc", "1.2.5.8.3.4", m2));
            Assert.AreEqual(-1, comparator.Compare(m1, m2));
        }

        [Test]
        public void TestNonAnnotatedAfterAnnotated() {
            Reset();
            // don't annotate m1
            m2.AddFacet(new MemberOrderFacetAnnotation("def", "2", m2));
            Assert.AreEqual(+1, comparator.Compare(m1, m2));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}