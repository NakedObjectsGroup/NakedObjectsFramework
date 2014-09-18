// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Helpers.Test.ViewModel;
using NakedObjects.Services;
using NakedObjects.SystemTest.PolymorphicAssociations;
using NakedObjects.SystemTest.TestObjectFinderWithCompoundKeysAndTypeCodeMapper;
using NakedObjects.Xat;
using NakedObjects.Helpers.Test;

namespace NakedObjects.SystemTest.PolymorphicNavigator {

    [TestClass]
    public class TestPolymorphicNavigator : TestPolymorphicNavigatorAbstract {

        #region Setup/Teardown

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(new TestPolymorphicNavigator());
        }

        [ClassCleanup]
        public static void TearDownTest() {
            CleanupNakedObjectsFramework(new TestPolymorphicNavigator());
        }

        #endregion

        #region Run configuration

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(
                    new SimpleRepository<PolymorphicPayment>(),
                    new SimpleRepository<CustomerAsPayee>(),
                    new SimpleRepository<SupplierAsPayee>(),
                    new SimpleRepository<InvoiceAsPayableItem>(),
                    new SimpleRepository<ExpenseClaimAsPayableItem>(),
                    new Services.PolymorphicNavigator());
            }
        }
        #endregion

        [TestMethod]
        public  void SetPolymorphicPropertyOnTransientObject() {
            base.SetPolymorphicPropertyOnTransientObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
        }

        [TestMethod]
        public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
            base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
        }

        [TestMethod]
        public  void SetPolymorphicPropertyOnPersistentObject()
        {
            SetPolymorphicPropertyOnPersistentObject("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
        }

        [TestMethod, Ignore]
        public override void ChangePolymorphicPropertyOnPersistentObject()
        {
            base.ChangePolymorphicPropertyOnPersistentObject();
        }

        [TestMethod]
        public override void ClearPolymorphicProperty()
        {
            base.ClearPolymorphicProperty();
        }

        [TestMethod]
        public void PolymorphicCollectionAddMutlipleItemsOfOneType()
        {
            base.PolymorphicCollectionAddMutlipleItemsOfOneType("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
        }

        [TestMethod]
        public void PolymorphicCollectionAddDifferentItems()
        {
            base.PolymorphicCollectionAddDifferentItems("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem", "NakedObjects.SystemTest.PolymorphicAssociations.ExpenseClaimAsPayableItem");
        }


        [TestMethod, Ignore]
        public override void AttemptToAddSameItemTwice()
        {
            base.AttemptToAddSameItemTwice();
        }

        [TestMethod, Ignore]
        public override void RemoveItem()
        {
            base.RemoveItem();
        }


        [TestMethod]
        public override void AttemptToRemoveNonExistentItem()
        {
            base.AttemptToRemoveNonExistentItem();
        }


        [TestMethod, Ignore]
        public override void FindOwnersForObject()
        {
            base.FindOwnersForObject();
        }
    }
}