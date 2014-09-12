// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.ObjectFinderSingleKey {
    [TestClass]
    public class TestObjectFinderWithSingleKeys : AcceptanceTestCase {
        private int countCustomers;
        private int countPayments;
        private int countSuppliers;
        private ITestObject customer1;
        private ITestObject customer2;
        private ITestObject emp1;
        private ITestObject emp2;
        private ITestProperty key1;
        private ITestProperty payee1;
        private ITestObject payment1;
        private ITestObject supplier1;

        #region Setup/Teardown

        [TestInitialize]
        public void Setup() {
            InitializeNakedObjectsFramework(this);
            payment1 = CreatePayment();
            customer1 = CreateCustomer();
            customer2 = CreateCustomer();
            supplier1 = CreateSupplier();
            payee1 = payment1.GetPropertyByName("Payee");
            key1 = payment1.GetPropertyByName("Payee Compound Key");
            emp1 = CreateEmployee("foo");
            emp2 = CreateEmployee("bar");
        }

        [TestCleanup]
        public void TearDown() {
            CleanupNakedObjectsFramework(this);
            countPayments = 0;
            countCustomers = 0;
            payment1 = null;
            customer1 = null;
            payee1 = null;
            key1 = null;
            emp1 = null;
            emp2 = null;
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
                    new SimpleRepository<Supplier>(),
                    new SimpleRepository<Employee>()
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

        private ITestObject CreateCustomer() {
            ITestObject cust = GetTestService("Customers").GetAction("New Instance").InvokeReturnObject();
            countCustomers++;
            cust.GetPropertyByName("Id").SetValue(countCustomers.ToString());
            cust.Save();
            return cust;
        }

        private ITestObject CreateSupplier() {
            ITestObject sup = GetTestService("Suppliers").GetAction("New Instance").InvokeReturnObject();
            countSuppliers++;
            sup.GetPropertyByName("Id").SetValue(countCustomers.ToString());
            sup.Save();
            return sup;
        }

        private ITestObject CreateEmployee(string key) {
            ITestObject emp = GetTestService("Employees").GetAction("New Instance").InvokeReturnObject();
            emp.GetPropertyByName("Id").SetValue(key);
            emp.Save();
            return emp;
        }


        [TestMethod]
        public void SetAssociatedObject() {
            payee1.SetObject(customer1);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|1");

            payee1.SetObject(customer2);
            Assert.AreEqual(payee1.ContentAsObject, customer2);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|2");
        }


        [TestMethod]
        public void ChangeAssociatedObjectType() {
            payee1.SetObject(customer1);
            payee1.ClearObject();
            payee1.SetObject(supplier1);
            Assert.AreEqual(payee1.ContentAsObject, supplier1);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Supplier|2");
        }


        [TestMethod]
        public void ClearAssociatedObject() {
            payee1.SetObject(customer1);
            payee1.ClearObject();
            key1.AssertIsEmpty();
        }


        [TestMethod]
        public void GetAssociatedObject() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|1");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("1");

            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|2");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("2");
        }

        [TestMethod]
        public void NoAssociatedObject() {
            key1.AssertIsEmpty();
        }


        [TestMethod]
        public void SetAssociatedObjectObjectWithAStringKey() {
            payee1.SetObject(emp1);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|foo");

            payee1.SetObject(emp2);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|bar");
        }

        [TestMethod]
        public void GetAssociatedObjectWithAStringKey() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|foo");
            payee1.AssertObjectIsEqual(emp1);

            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|bar");
            payee1.AssertObjectIsEqual(emp2);
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

    public interface IPayee {}


    public class Customer : IPayee {
        [Key]
        public virtual int Id { get; set; }
    }


    public class Supplier : IPayee {
        [Key]
        public virtual int Id { get; set; }
    }


    public class Employee : IPayee {
        [Key]
        public virtual string Id { get; set; }
    }

    #endregion
}