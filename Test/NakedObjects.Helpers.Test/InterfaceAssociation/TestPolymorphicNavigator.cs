// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.SystemTest.PolymorphicAssociations;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.PolymorphicNavigator {
    [TestClass]
    public class TestPolymorphicNavigator : AcceptanceTestCase {
        #region Setup/Teardown

        // to get EF SqlServer Dll in memory
        public SqlProviderServices instance = SqlProviderServices.Instance;

        [TestInitialize]
        public void Initialize() {
           
            InitializeNakedObjectsFramework();
        }

        [TestCleanup]
        public void CleanUp() {
            CleanupNakedObjectsFramework();
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


        protected override IObjectPersistorInstaller Persistor {
            get {
                var p = new EntityPersistorInstaller();
                p.UsingCodeFirstContext(() => new MyContext());
                return p;
            }
        }

        #endregion

        [TestMethod]
        public void SetPolymorphicPropertyOnTransientObject() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);
            payment1.Save();
            ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
            ITestProperty associatedType = payeeLink.GetPropertyByName("Associated Role Object Type").AssertIsUnmodifiable();
            associatedType.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
            ITestProperty associatedId = payeeLink.GetPropertyByName("Associated Role Object Id").AssertIsUnmodifiable();
            associatedId.AssertValueIsEqual(cusId);
        }

        [TestMethod]
        public void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            try {
                payeeProp.SetObject(customer1);
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.IsTrue(e.Message.Contains("Can't set field of persistent with a transient reference"));
            }
        }


        [TestMethod]
        public void SetPolymorphicPropertyOnPersistentObject() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);

            ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
            ITestProperty associatedType = payeeLink.GetPropertyByName("Associated Role Object Type").AssertIsUnmodifiable();
            associatedType.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.CustomerAsPayee");
            ITestProperty associatedId = payeeLink.GetPropertyByName("Associated Role Object Id").AssertIsUnmodifiable();
            associatedId.AssertValueIsEqual(cusId);
        }

        [TestMethod]
        public void ChangePolymorphicPropertyOnPersistentObject() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            string cusId = customer1.GetPropertyByName("Id").Title;

            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
            payeeProp.SetObject(customer1);
            payment1.Save();
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


        [TestMethod]
        public void ClearPolymorphicProperty() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

            ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link");
            payeeProp.SetObject(customer1);
            payment1.Save();
            payeeLinkProp.AssertIsNotEmpty();
            payeeProp.AssertIsNotEmpty();

            payeeProp.ClearObject();
            payeeLinkProp.AssertIsEmpty();
            payeeProp.AssertIsEmpty();
        }

        [TestMethod]
        public void PolymorphicCollectionAddMutlipleItemsOfOneType() {
            ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
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
            link1.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
            link1.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(invId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestObject item = items.AssertCountIs(1).ElementAt(0);
            item.AssertIsType(typeof (InvoiceAsPayableItem));

            //Add an expense claim
            payment.GetAction("Add Payable Item").InvokeReturnObject(inv2);
            links.AssertCountIs(2);
            ITestObject link2 = links.ElementAt(1);
            link2.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
            link2.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(inv2Id);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            item = items.AssertCountIs(2).ElementAt(1);
            item.AssertIsType(typeof (InvoiceAsPayableItem));
        }

        [TestMethod]
        public void PolymorphicCollectionAddDifferentItems() {
            ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
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
            link1.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.InvoiceAsPayableItem");
            link1.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(invId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            ITestObject item = items.AssertCountIs(1).ElementAt(0);
            item.AssertIsType(typeof (InvoiceAsPayableItem));

            //Add an expense claim
            payment.GetAction("Add Payable Item").InvokeReturnObject(exp);
            links.AssertCountIs(2);
            ITestObject link2 = links.ElementAt(1);
            link2.GetPropertyByName("Associated Role Object Type").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicAssociations.ExpenseClaimAsPayableItem");
            link2.GetPropertyByName("Associated Role Object Id").AssertValueIsEqual(expId);
            items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
            item = items.AssertCountIs(2).ElementAt(1);
            item.AssertIsType(typeof (ExpenseClaimAsPayableItem));
        }


        [TestMethod]
        public void AttemptToAddSameItemTwice() {
            ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
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

        [TestMethod]
        public void RemoveItem() {
            ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
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


        [TestMethod]
        public void AttemptToRemoveNonExistentItem() {
            ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
            ITestObject inv2 = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
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


        [TestMethod]
        public void FindOwnersForObject() {
            ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();
            ITestObject payment2 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();
            ITestObject payment3 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();


            ITestObject cus = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
            ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();


            ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
            payeeProp.SetObject(cus);
            payeeProp = payment3.GetPropertyByName("Payee");
            payeeProp.SetObject(cus);

            ITestCollection results = cus.GetAction("Payments To This Payee").InvokeReturnCollection();
            results.AssertCountIs(2);
            Assert.AreEqual(payment1, results.ElementAt(0));
            Assert.AreEqual(payment3, results.ElementAt(1));


            //Add invoice to two payments
            payment1.GetAction("Add Payable Item").InvokeReturnObject(inv);
            payment2.GetAction("Add Payable Item").InvokeReturnObject(inv);

            results = inv.GetAction("Payments Containing This").InvokeReturnCollection();
            results.AssertCountIs(2);
            Assert.AreEqual(payment1, results.ElementAt(0));
            Assert.AreEqual(payment2, results.ElementAt(1));
        }
    }
}