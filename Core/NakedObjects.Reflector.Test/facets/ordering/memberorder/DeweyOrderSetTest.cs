// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Moq;
using NakedObjects.Architecture.Component;
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
        private MemberPeerStub[] lastNameAndFirstName;
        private MemberPeerStub[] lastNameFirstNameAndPostalTown;
        private MemberPeerStub lastNameMember;
        private MemberPeerStub[] nameAndAddressMembers;
        private MemberPeerStub postalTownMember;
        private MemberPeerStub streetNameMember;

        [Test]
        public void TestDefaultGroup() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual("", orderSet.GroupName);
            Assert.AreEqual("", orderSet.GroupFullName);
            Assert.AreEqual("", orderSet.GroupPath);
        }

        [Test]
        public void TestDefaultGroupMixOfAnnotatedAndNotOrderedWithAnnotatedFirst() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("", "2", postalTownMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameFirstNameAndPostalTown);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(postalTownMember, orderSet.ElementList()[1]);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[2]);
        }

        [Test]
        public void TestDefaultGroupMixOfAnnotatedAndNotSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", postalTownMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameFirstNameAndPostalTown);
            Assert.AreEqual(3, orderSet.ElementList().Count);
        }

        [Test]
        public void TestDefaultGroupNeitherAnnotatedOrderedByName() {
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[1]);
        }

        [Test]
        public void TestDefaultGroupNeitherAnnotatedSize() {
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(2, orderSet.ElementList().Count);
        }

        [Test]
        public void TestDefaultGroupSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(2, orderSet.Size());
            Assert.AreEqual(2, orderSet.ElementList().Count);
            Assert.AreEqual(0, orderSet.Children.Count);
        }

        [Test]
        public void TestDefaultGroupTwoMembersSorted() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[1]);
        }

        [Test]
        public void TestTwoMembersAtDefaultGroupOtherWay() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", firstNameMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(lastNameAndFirstName);
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
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
            var childOrderSet = orderSet.Children[0].Set;
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
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
            var children = orderSet.Children;
            var childOrderSet = children[0].Set;
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
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
            var childOrderSet = orderSet.Children[0].Set;
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
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
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
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
            Assert.AreEqual(firstNameMember, orderSet.ElementList()[0]);
            Assert.AreEqual(lastNameMember, orderSet.ElementList()[1]);
            Assert.IsTrue(orderSet.ElementList()[2] is OrderSet<MemberPeerStub>);
        }

        [Test]
        public void TestWithChildGroupSize() {
            lastNameMember.AddFacet(new MemberOrderFacetAnnotation("", "1", lastNameMember));
            firstNameMember.AddFacet(new MemberOrderFacetAnnotation("", "2", firstNameMember));
            houseNumberMember.AddFacet(new MemberOrderFacetAnnotation("address", "1", houseNumberMember));
            streetNameMember.AddFacet(new MemberOrderFacetAnnotation("address", "2", streetNameMember));
            postalTownMember.AddFacet(new MemberOrderFacetAnnotation("address", "3", postalTownMember));
            var orderSet = DeweyOrderSet<MemberPeerStub>.CreateOrderSet(nameAndAddressMembers);
            Assert.AreEqual(1, orderSet.Children.Count);
            Assert.AreEqual(3, orderSet.Size());
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}