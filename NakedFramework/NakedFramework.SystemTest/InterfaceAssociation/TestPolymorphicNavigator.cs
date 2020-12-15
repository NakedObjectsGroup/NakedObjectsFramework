// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.SystemTest.PolymorphicAssociations;
using NUnit.Framework;

namespace NakedObjects.SystemTest.PolymorphicNavigator {
    [TestFixture]
    public class TestPolymorphicNavigator : TestPolymorphicNavigatorAbstract {
        private const string DatabaseName = "TestPolymorphicNavigator";

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] { config => new PolymorphicNavigationContext(DatabaseName) };



        protected override object[] Fixtures => new object[] {new FixtureEntities(), new FixtureLinksUsingTypeName()};

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            InitializeNakedObjectsFramework(this);
            RunFixtures();
        }

        [OneTimeTearDown]
        public void DeleteDatabase() {
            CleanupNakedObjectsFramework(this);
        }

        [Test]
        public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
            base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
        }

        [Test]
        public override void AttemptToAddSameItemTwice() {
            base.AttemptToAddSameItemTwice();
        }

        [Test]
        public override void AttemptToRemoveNonExistentItem() {
            base.AttemptToRemoveNonExistentItem();
        }

        [Test]
        public void ChangePolymorphicPropertyOnPersistentObject() {
            ChangePolymorphicPropertyOnPersistentObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee", "NakedObjects.SystemTest.PolymorphicAssociations.SupplierAsPayee");
        }

        [Test]
        public override void ClearPolymorphicProperty() {
            base.ClearPolymorphicProperty();
        }

        [Test]
        public override void FindOwnersForObject() {
            base.FindOwnersForObject();
        }

        [Test]
        public void PolymorphicCollectionAddDifferentItems() {
            PolymorphicCollectionAddDifferentItems("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem", "NakedObjects.SystemTest.PolymorphicAssociations.ExpenseClaimAsPayableItem");
        }

        [Test]
        public void PolymorphicCollectionAddMutlipleItemsOfOneType() {
            PolymorphicCollectionAddMutlipleItemsOfOneType("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
        }

        [Test]
        public override void RemoveItem() {
            base.RemoveItem();
        }

        [Test]
        public void SetPolymorphicPropertyOnPersistentObject() {
            SetPolymorphicPropertyOnPersistentObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
        }

        [Test]
        public void SetPolymorphicPropertyOnTransientObject() {
            SetPolymorphicPropertyOnTransientObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
        }
    }
}