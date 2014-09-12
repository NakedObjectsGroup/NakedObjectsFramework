// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.ObjectFinderGuid {
    [TestClass]
    public class TestObjectFinderWithGuids : AcceptanceTestCase {
        private int countPayments;
        private ITestObject customer1;
        private ITestObject customer2;
        private ITestProperty key1;
        private ITestProperty payee1;
        private ITestObject payment1;
        private ITestObject supplier1;

        #region Setup/Teardown

        [TestInitialize]
        public void Setup() {
            InitializeNakedObjectsFramework(this);
            payment1 = CreatePayment();
            customer1 = CreateCustomer("0c1ced04-7016-11e0-9c44-78544824019b");
            customer2 = CreateCustomer("3d9d6ca0-7016-11e0-b12a-9e544824019b");

            supplier1 = CreateSupplier("89bc90ec-7017-11e0-a08c-57564824019b");
            payee1 = payment1.GetPropertyByName("Payee");
            key1 = payment1.GetPropertyByName("Payee Compound Key");
        }

        [TestCleanup]
        public void TearDown() {
            CleanupNakedObjectsFramework(this);
            countPayments = 0;
            payment1 = null;
            customer1 = null;
            payee1 = null;
            key1 = null;
        }

        #endregion

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                    new ObjectFinder(),
                    new SimpleRepository<Payment>(),
                    new SimpleRepository<Customer>(),
                    new SimpleRepository<Supplier>()
                });
            }
        }

        private ITestObject CreatePayment() {
            ITestObject pay = GetTestService("Payments").GetAction("New Instance").InvokeReturnObject();
            countPayments++;
            pay.GetPropertyByName("Id").SetValue(countPayments.ToString());
            pay.Save();
            return pay;
        }

        private ITestObject CreateCustomer(string guid) {
            ITestObject cust = GetTestService("Customers").GetAction("New Instance").InvokeReturnObject();
            cust.GetPropertyById("Guid").SetValue(guid);
            cust.Save();
            return cust;
        }


        private ITestObject CreateSupplier(string guid) {
            ITestObject sup = GetTestService("Suppliers").GetAction("New Instance").InvokeReturnObject();
            sup.GetPropertyById("Guid").SetValue(guid);
            sup.Save();
            return sup;
        }

        [TestMethod]
        public void SetAssociatedObject() {
            payee1.SetObject(customer1);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Customer|0c1ced04-7016-11e0-9c44-78544824019b");

            payee1.SetObject(customer2);
            Assert.AreEqual(payee1.ContentAsObject, customer2);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Customer|3d9d6ca0-7016-11e0-b12a-9e544824019b");
        }


        [TestMethod]
        public void ChangeAssociatedObjectType() {
            payee1.SetObject(customer1);
            payee1.ClearObject();
            payee1.SetObject(supplier1);
            Assert.AreEqual(payee1.ContentAsObject, supplier1);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Supplier|89bc90ec-7017-11e0-a08c-57564824019b");
        }


        [TestMethod]
        public void ClearAssociatedObject() {
            payee1.SetObject(customer1);
            payee1.ClearObject();
            key1.AssertIsEmpty();
        }


        [TestMethod]
        public void GetAssociatedObject() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderGuid.Customer|0c1ced04-7016-11e0-9c44-78544824019b");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Guid").AssertValueIsEqual("0c1ced04-7016-11e0-9c44-78544824019b");

            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderGuid.Customer|3d9d6ca0-7016-11e0-b12a-9e544824019b");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Guid").AssertValueIsEqual("3d9d6ca0-7016-11e0-b12a-9e544824019b");
        }

        [TestMethod]
        public void NoAssociatedObject() {
            key1.AssertIsEmpty();
        }
    }

    #region Classes used by test

    public class Payment {
        public IDomainObjectContainer Container { protected get; set; }
        public virtual int Id { get; set; }

        #region Payee Property (Interface Association)

        //IMPORTANT:  Register an implementation of IObjectFinder
        //Suggestion: Move this property into an 'Injected Services' region
        private IPayee myPayee;
        public IObjectFinder ObjectFinder { set; protected get; }

        //Holds a compound key that represents both the
        //actual type and the identity of the associated object.
        //NOTE: If working Model First, an equivalent property should be added to the
        //Entity, and this line of code moved into the 'buddy class'.
        [Optionally]
        public virtual string PayeeCompoundKey { get; set; }

        [NotPersisted, Optionally]
        public IPayee Payee {
            get {
                if (myPayee == null & !String.IsNullOrEmpty(PayeeCompoundKey)) {
                    myPayee = ObjectFinder.FindObject<IPayee>(PayeeCompoundKey);
                }
                return myPayee;
            }
            set {
                myPayee = value;
                if (value == null) {
                    PayeeCompoundKey = null;
                }
                else {
                    PayeeCompoundKey = ObjectFinder.GetCompoundKey(value);
                }
            }
        }

        #endregion
    }

    public interface IPayee : IHasGuid {}


    public class Customer : IPayee {
        public virtual Guid Guid { get; set; }
    }


    public class Supplier : IPayee {
        public virtual Guid Guid { get; set; }
    }

    #endregion
}