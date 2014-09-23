// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using Moq;
using NakedObjects.Architecture.Persist;
using NakedObjects.Reflector.Peer;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    [TestFixture]
    public class DeweyOrderSetTest {
        #region Setup/Teardown

        [SetUp]
        public void Initialize() {
            var p = new Mock<ILifecycleManager>().Object;
            lastNameMember = new MemberPeerStub("LastName", p);
            firstNameMember = new MemberPeerStub("FirstName", p);
            houseNumberMember = new MemberPeerStub("HouseNumber", p);
            streetNameMember = new MemberPeerStub("StreetName", p);
            postalTownMember = new MemberPeerStub("PostalTown", p);
            lastNameAndFirstName = new[] {lastNameMember, firstNameMember};
            nameAndAddressMembers = new[] {lastNameMember, firstNameMember, houseNumberMember, streetNameMember, postalTownMember};
            lastNameFirstNameAndPostalTown = new[] {lastNameMember, firstNameMember, postalTownMember};
        }

        #endregion

        private MemberPeerStub firstNameMember;
        private MemberPeerStub houseNumberMember;
        private INakedObjectMemberPeer[] lastNameAndFirstName;
        private INakedObjectMemberPeer[] lastNameFirstNameAndPostalTown;
        private MemberPeerStub lastNameMember;
        private INakedObjectMemberPeer[] nameAndAddressMembers;
        private MemberPeerStub postalTownMember;
        private MemberPeerStub streetNameMember;

        [Test]
        public void TestDefaultGroup() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual("", orderSet.GroupName);
            Assert.AreEqual("", orderSet.GroupFullName);
            Assert.AreEqual("", orderSet.GroupPath);
        }

        [Test]
        public void TestDefaultGroupMixOfAnnotatedAndNotOrderedWithAnnotatedFirst() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("", "2", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameFirstNameAndPostalTown);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(postalTownMember, orderSet.ElementList()[1]);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[2]);
        }

        [Test]
        public void TestDefaultGroupMixOfAnnotatedAndNotSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameFirstNameAndPostalTown);
            Assert.AreEqual(3, orderSet.ElementList().Count);
        }

        [Test]
        public void TestDefaultGroupNeitherAnnotatedOrderedByName() {
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[1]);
        }

        [Test]
        public void TestDefaultGroupNeitherAnnotatedSize() {
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(2, orderSet.ElementList().Count);
        }

        [Test]
        public void TestDefaultGroupSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(2, orderSet.Size());
            Assert.AreEqual(2, orderSet.ElementList().Count);
            Assert.AreEqual(0, orderSet.Children.Count);
        }

        [Test]
        public void TestDefaultGroupTwoMembersSorted() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[1]);
        }

        [Test]
        public void TestTwoMembersAtDefaultGroupOtherWay() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", firstNameMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[1]);
        }

        [Test]
        public void TestWithChildGroupChildsGroupElementOrdering() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "6", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "5", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "4", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            var childOrderSet = (OrderSet) orderSet.Children[0];
            Assert.AreEqual(postalTownMember, childOrderSet.ElementList()[0]);
            Assert.AreEqual(streetNameMember, childOrderSet.ElementList()[1]);
            Assert.AreEqual(houseNumberMember, childOrderSet.ElementList()[2]);
        }

        [Test]
        public void TestWithChildGroupChildsGroupName() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "1", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "3", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            IList<IOrderableElement> children = orderSet.Children;
            var childOrderSet = (OrderSet) children[0];
            Assert.AreEqual("Address", childOrderSet.GroupName);
            Assert.AreEqual("address", childOrderSet.GroupFullName);
            Assert.AreEqual("", childOrderSet.GroupPath);
        }

        [Test]
        public void TestWithChildGroupChildsGroupSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "1", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "3", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            var childOrderSet = (OrderSet) orderSet.Children[0];
            Assert.AreEqual(3, childOrderSet.Size());
            Assert.AreEqual(0, childOrderSet.Children.Count);
        }

        [Test]
        public void TestWithChildGroupDefaultGroupName() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "1", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "3", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            Assert.AreEqual("", orderSet.GroupName);
            Assert.AreEqual("", orderSet.GroupFullName);
            Assert.AreEqual("", orderSet.GroupPath);
        }

        [Test]
        public void TestWithChildGroupOrderedAtEnd() {
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "6", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "5", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "4", postalTownMember));
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "3", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[1]);
            Assert.IsTrue(orderSet.ElementList()[2] is OrderSet);
        }

        [Test]
        public void TestWithChildGroupSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "1", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "3", postalTownMember));
            DeweyOrderSet orderSet = DeweyOrderSet.CreateOrderSet(nameAndAddressMembers);
            Assert.AreEqual(1, orderSet.Children.Count);
            Assert.AreEqual(3, orderSet.Size());
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}