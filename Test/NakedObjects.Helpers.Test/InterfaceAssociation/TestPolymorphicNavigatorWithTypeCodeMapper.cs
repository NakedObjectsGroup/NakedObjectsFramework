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

namespace NakedObjects.SystemTest.PolymorphicNavigator {

    [TestClass]
    public class TestPolymorphicNavigatorWithTypeCodeMapper : TestPolymorphicNavigatorAbstract
    {
        #region Setup/Teardown


        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(new TestPolymorphicNavigatorWithTypeCodeMapper());
        }

        [ClassCleanup]
        public static void TearDownTest() {
            CleanupNakedObjectsFramework(new TestPolymorphicNavigatorWithTypeCodeMapper());
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
                    new SimpleRepository<ExpenseClaimAsPayableItem>());
            }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new Services.PolymorphicNavigator(), new SimpleTypeCodeMapper()); }
        }

        #endregion

        [TestMethod]
        public void SetPolymorphicPropertyOnTransientObject()
        {
            base.SetPolymorphicPropertyOnTransientObject("CUS");
        }

        [TestMethod]
        public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject()
        {
            base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
        }

        [TestMethod]
        public  void SetPolymorphicPropertyOnPersistentObject()
        {
            base.SetPolymorphicPropertyOnPersistentObject("CUS");
        }

        [TestMethod, Ignore]
        public override void ChangePolymorphicPropertyOnPersistentObject()
        {
            base.ChangePolymorphicPropertyOnPersistentObject();
        }

        [TestMethod, Ignore]
        public override void ClearPolymorphicProperty()
        {
            base.ClearPolymorphicProperty();
        }

        [TestMethod]
        public  void PolymorphicCollectionAddMutlipleItemsOfOneType()
        {
            base.PolymorphicCollectionAddMutlipleItemsOfOneType("INV");
        }

        [TestMethod]
        public  void PolymorphicCollectionAddDifferentItems()
        {
            base.PolymorphicCollectionAddDifferentItems("INV", "EXP");
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

    public class SimpleTypeCodeMapper : ITypeCodeMapper {
        #region ITypeCodeMapper Members

        public Type TypeFromCode(string code) {
            if (code == "CUS") return typeof (CustomerAsPayee);
            if (code == "SUP") return typeof (SupplierAsPayee);
            if (code == "INV") return typeof (InvoiceAsPayableItem);
            if (code == "EXP") return typeof (ExpenseClaimAsPayableItem);
            throw new DomainException("Code not recognised: " + code);
        }

        public string CodeFromType(Type type) {
            if (type == typeof (CustomerAsPayee)) return "CUS";
            if (type == typeof (SupplierAsPayee)) return "SUP";
            if (type == typeof (InvoiceAsPayableItem)) return "INV";
            if (type == typeof (ExpenseClaimAsPayableItem)) return "EXP";
            throw new DomainException("Type not recognised: " + type);
        }

        #endregion
    }
}