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
    public abstract class TestPolymorphicNavigatorAbstract : AbstractHelperTest<PolymorphicNavigationContext> {

        #region Setup/Teardown

        // to get EF SqlServer Dll in memory
        public SqlProviderServices instance = SqlProviderServices.Instance;

        [TestInitialize]
        public void Initialize() {
            StartTest();
     
        }

        [TestCleanup]
        public void CleanUp() {
        }

        #endregion

        public virtual void SetPolymorphicPropertyOnTransientObject(string roleObjectType) {
            ITestObject transPayment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = transPayment.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = transPayment.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);
            transPayment.Save();
            ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
            ITestProperty associatedType = payeeLink.GetPropertyByName("Associated Role Object Type").AssertIsUnmodifiable();
            associatedType.AssertValueIsEqual(roleObjectType);
            ITestProperty associatedId = payeeLink.GetPropertyByName("Associated Role Object Id").AssertIsUnmodifiable();
            associatedId.AssertValueIsEqual(cusId);
        }
 
        public virtual void AttemptSetPolymorphicPropertyWithATransientAssociatedObject()
        {
            ITestObject transPayment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = transPayment.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = transPayment.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            try {
                payeeProp.SetObject(customer1);
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.IsTrue(e.Message.Contains("Can't set field of persistent with a transient reference"));
            }
        }

        public virtual void SetPolymorphicPropertyOnPersistentObject(string roleObjectType)
        {
            var payment1 = FindById("Polymorphic Payments", 1);
            var customer1 = FindById("Customer As Payees", 1);    
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment1.GetPropertyByName("Payee").AssertIsEmpty();
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);

            ITestObject payeeLink = payment1.GetPropertyByName("Payee Link").AssertIsNotEmpty().ContentAsObject;
            ITestProperty associatedType = payeeLink.GetPropertyByName("Associated Role Object Type").AssertIsUnmodifiable();
            associatedType.AssertValueIsEqual(roleObjectType);
            ITestProperty associatedId = payeeLink.GetPropertyByName("Associated Role Object Id").AssertIsUnmodifiable();
            associatedId.AssertValueIsEqual(cusId);
        }

        public virtual  void ChangePolymorphicPropertyOnPersistentObject() {
            var payment2 = FindById("Polymorphic Payments", 3);
            var customer1 = FindById("Customer As Payees", 1);  
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment2.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment2.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);

            ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
            ITestProperty associatedType = payeeLink.GetPropertyByName("Associated Role Object Type").AssertIsUnmodifiable();
            associatedType.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
            ITestProperty associatedId = payeeLink.GetPropertyByName("Associated Role Object Id").AssertIsUnmodifiable();
            associatedId.AssertValueIsEqual(cusId);

            ITestObject sup1 = GetTestService("Supplier As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            string supId = sup1.GetPropertyByName("Id").Title;

            payeeProp.SetObject(sup1);
            associatedType.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.SupplierAsPayee");
            associatedId.AssertValueIsEqual(supId);

            payeeProp.ClearObject();
            payeeLinkProp.AssertIsEmpty();
            payeeProp.AssertIsEmpty();
        }

        public virtual void ClearPolymorphicProperty()
        {
            var payment3 = FindById("Polymorphic Payments", 2);
            
            var payeeProp = payment3.GetPropertyByName("Payee");
            var payeeLinkProp = payment3.GetPropertyByName("Payee Link");

            payeeLinkProp.AssertIsNotEmpty();
            payeeProp.AssertIsNotEmpty();

            payeeProp.ClearObject();
            payeeLinkProp.AssertIsEmpty();
            payeeProp.AssertIsEmpty();
        }

        public virtual void PolymorphicCollectionAddMutlipleItemsOfOneType(string roleObjectType)
        {
            var payment = FindById("Polymorphic Payments", 4);
            var inv = FindById("Invoice As Payable Items", 1);  

            string invId = inv.GetPropertyByName("Id").Title;
            ITestObject inv2 = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
            string inv2Id = inv2.GetPropertyByName("Id").Title;


            ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
            ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

            links.AssertCountIs(0);
            items.AssertCountIs(0);

            //Add an Invoice
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1);
            ITestObject link1 = links.ElementAt(0);
            link1.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual(roleObjectType);
            link1.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(invId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestObject item = items.AssertCountIs(1).ElementAt(0);
            item.AssertIsType(typeof (InvoiceAsPayableItem));

            //Add an expense claim
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv2);
            links.AssertCountIs(2);
            ITestObject link2 = links.ElementAt(1);
            link2.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual(roleObjectType);
            link2.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(inv2Id);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            item = items.AssertCountIs(2).ElementAt(1);
            item.AssertIsType(typeof (InvoiceAsPayableItem));
        }

        public virtual void PolymorphicCollectionAddDifferentItems(string roleType1, string roleType2)
        {
            var payment = FindById("Polymorphic Payments", 5);
            var inv = FindById("Invoice As Payable Items", 1);

            string invId = inv.GetPropertyByName("Id").Title;
            ITestObject exp = GetTestService("Expense Claim As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
            string expId = exp.GetPropertyByName("Id").Title;

            ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
            ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

            links.AssertCountIs(0);
            items.AssertCountIs(0);

            //Add an Invoice
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1);
            ITestObject link1 = links.ElementAt(0);
            link1.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual(roleType1);
            link1.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(invId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestObject item = items.AssertCountIs(1).ElementAt(0);
            item.AssertIsType(typeof (InvoiceAsPayableItem));

            //Add an expense claim
            payment.GetAction("Add Payable Item").InvokeReturnObject(exp);
            links.AssertCountIs(2);
            ITestObject link2 = links.ElementAt(1);
            link2.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual(roleType2);
            link2.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(expId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            item = items.AssertCountIs(2).ElementAt(1);
            item.AssertIsType(typeof (ExpenseClaimAsPayableItem));
        }

        public virtual void AttemptToAddSameItemTwice()
        {
            var payment = FindById("Polymorphic Payments", 6);
            var inv = FindById("Invoice As Payable Items", 1); 
            string invId = inv.GetPropertyByName("Id").Title;

            ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
            ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

            links.AssertCountIs(0);
            items.AssertCountIs(0);

            //Add an Invoice
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            items.AssertCountIs(1);

            //Try adding same expense claim again
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1); //Should still be 1
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            items.AssertCountIs(1);
        }

        public virtual void RemoveItem()
        {
            var payment = FindById("Polymorphic Payments", 7);
            var inv = FindById("Invoice As Payable Items", 1); 
            string invId = inv.GetPropertyByName("Id").Title;

            ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
            ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

            links.AssertCountIs(0);
            items.AssertCountIs(0);

            //Add an Invoice
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestCollection item = items.AssertCountIs(1);

            //Now remove the invoice
            payment.GetAction("Remove Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(0);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            items.AssertCountIs(0);
        }

        public virtual void AttemptToRemoveNonExistentItem()
        {
            var payment = FindById("Polymorphic Payments", 8);
            var inv = FindById("Invoice As Payable Items", 1);
            var inv2 = FindById("Invoice As Payable Items", 2); 

            string invId = inv.GetPropertyByName("Id").Title;

            ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
            ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

            links.AssertCountIs(0);
            items.AssertCountIs(0);

            //Add Invoice 1
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
            links.AssertCountIs(1);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestObject item = items.AssertCountIs(1).ElementAt(0);
            item.AssertIsType(typeof (InvoiceAsPayableItem));

            //Now attempt to remove invoice 2
            payment.GetAction("Remove Payable Item").InvokeReturnObject(inv2);
            links.AssertCountIs(1);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            item = items.AssertCountIs(1).ElementAt(0);
        }

        public virtual void FindOwnersForObject()
        {
            var payment9 = FindById("Polymorphic Payments", 9);
            var payment10 = FindById("Polymorphic Payments", 10);
            var payment11 = FindById("Polymorphic Payments", 11);

            var cus = FindById("Customer As Payees", 1);
            var inv = FindById("Invoice As Payable Items", 1);

            var payeeProp = payment9.GetPropertyByName("Payee");
            payeeProp.SetObject(cus);
            payeeProp = payment11.GetPropertyByName("Payee");
            payeeProp.SetObject(cus);

            var results = cus.GetAction("Payments To This Payee").InvokeReturnCollection();
            results.AssertCountIs(2);
            Assert.AreEqual(payment9, results.ElementAt(1));
            Assert.AreEqual(payment11, results.ElementAt(0));


            //Add invoice to two payments
            payment9.GetAction("Add Payable Item").InvokeReturnObject(inv);
            payment10.GetAction("Add Payable Item").InvokeReturnObject(inv);

            results = inv.GetAction("Payments Containing This").InvokeReturnCollection();
            results.AssertCountIs(2);
            Assert.AreEqual(payment9, results.ElementAt(0));
            Assert.AreEqual(payment10, results.ElementAt(1));
        }
    }
}