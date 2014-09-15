// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Helpers.Test.ViewModel;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.ObjectFinderCompoundKeys {
    public class DatabaseInitializer : DropCreateDatabaseAlways<PaymentContext> {}

    [TestClass, Ignore]
    public class TestObjectFinderWithCompoundKeys : AcceptanceTestCase {
        private int countCustomerOnes;
        private int countCustomerThrees;
        private int countCustomerTwos;
        private int countEmployees;
        private int countPayments;
        private int countSuppliers;
        private ITestObject customer1;
        private ITestObject customer2a;
        private ITestObject customer2b;
        private ITestObject customer3;
        private ITestObject emp1;
        private ITestProperty key1;
        private ITestProperty payee1;
        private ITestObject payment1;
        private ITestObject supplier1;

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                    new ObjectFinder(),
                    new SimpleRepository<Payment>(),
                    new SimpleRepository<CustomerOne>(),
                    new SimpleRepository<CustomerTwo>(),
                    new SimpleRepository<CustomerThree>(),
                    new SimpleRepository<CustomerFour>(),
                    new SimpleRepository<Supplier>(),
                    new SimpleRepository<Employee>()
                });
            }
        }


        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new PaymentContext("HelpersTest"));
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
        }

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(new TestViewModel());
        }

        [ClassCleanup]
        public static void TearDownTest() {
            CleanupNakedObjectsFramework(new TestViewModel());
        }

        [TestInitialize]
        public void Initialize() {
            StartTest();
            payment1 = CreatePayment();
            customer1 = CreateCustomerOnes();
            customer2a = CreateCustomerTwos();
            customer2b = CreateCustomerTwos();
            customer3 = CreateCustomerThrees();
            supplier1 = CreateSupplier();
            payee1 = payment1.GetPropertyByName("Payee");
            key1 = payment1.GetPropertyByName("Payee Compound Key");
            emp1 = CreateEmployee("foo");
        }

        [TestCleanup]
        public void CleanUp() {
            countPayments = 0;
            countCustomerTwos = 0;
            countSuppliers = 0;
            countEmployees = 0;
            payment1 = null;
            customer1 = null;
            customer2a = null;
            customer2b = null;
            customer3 = null;
            payee1 = null;
            key1 = null;
            emp1 = null;
        }


        private ITestObject CreatePayment() {
            ITestObject pay = GetTestService("Payments").GetAction("New Instance").InvokeReturnObject();
            countPayments++;
            pay.GetPropertyByName("Id").SetValue(countPayments.ToString());
            pay.Save();
            return pay;
        }

        private ITestObject CreateCustomerOnes() {
            ITestObject cust = GetTestService("Customer Ones").GetAction("New Instance").InvokeReturnObject();
            countCustomerOnes++;
            cust.GetPropertyByName("Id").SetValue(countCustomerOnes.ToString());
            cust.Save();
            return cust;
        }

        private ITestObject CreateCustomerTwos() {
            ITestObject cust = GetTestService("Customer Twos").GetAction("New Instance").InvokeReturnObject();
            countCustomerTwos++;
            cust.GetPropertyByName("Id").SetValue(countCustomerTwos.ToString());
            cust.GetPropertyByName("Id2").SetValue((countCustomerTwos + 1000).ToString());
            cust.Save();
            return cust;
        }

        private ITestObject CreateCustomerThrees() {
            ITestObject cust = GetTestService("Customer Threes").GetAction("New Instance").InvokeReturnObject();
            countCustomerThrees++;
            cust.GetPropertyByName("Id").SetValue(countCustomerThrees.ToString());
            cust.GetPropertyByName("Id2").SetValue((countCustomerThrees + 1000).ToString());
            cust.GetPropertyByName("Number").SetValue((countCustomerThrees + 2000).ToString());
            cust.Save();
            return cust;
        }

        private ITestObject CreateSupplier() {
            ITestObject sup = GetTestService("Suppliers").GetAction("New Instance").InvokeReturnObject();
            countSuppliers++;
            sup.GetPropertyByName("Id").SetValue(countSuppliers.ToString());
            sup.GetPropertyByName("Id2").SetValue((countSuppliers + 2000).ToString());
            sup.Save();
            return sup;
        }


        private ITestObject CreateEmployee(string stringKey) {
            ITestObject emp = GetTestService("Employees").GetAction("New Instance").InvokeReturnObject();
            countEmployees++;
            emp.GetPropertyByName("Id").SetValue(countEmployees.ToString());
            emp.GetPropertyByName("Id2").SetValue(stringKey);
            emp.Save();
            return emp;
        }


        [TestMethod]
        public void SetAssociatedObject() {
            payee1.SetObject(customer2a);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");

            payee1.SetObject(customer2b);
            Assert.AreEqual(payee1.ContentAsObject, customer2b);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|2|1002");
        }

        [TestMethod]
        public void FailsIfAssociatedObjectHasNoKeys() {
            ITestObject cust = GetTestService("Customer Fours").GetAction("New Instance").InvokeReturnObject();
            cust.GetPropertyByName("Id").SetValue("1");
            cust.Save();
            try {
                payee1.SetObject(cust);
                throw new AssertFailedException("Exception should have been thrown");
            }
            catch (Exception e) {
                Assert.AreEqual("Object: NakedObjects.Proxy.NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerFour has no Keys defined", e.Message);
            }
        }

        [TestMethod]
        public void WorksWithASingleIntegerKey() {
            payee1.SetObject(customer1);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerOne|1");
            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerOne|1");
            payee1.AssertIsNotEmpty();
            payee1.AssertObjectIsEqual(customer1);
        }

        [TestMethod]
        public void WorksWithTripleIntegerKey() {
            payee1.SetObject(customer3);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001|2001");
            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001|2001");
            payee1.AssertIsNotEmpty();
            payee1.AssertObjectIsEqual(customer3);
        }

        [TestMethod]
        public void FailsIfTypeNameIsEmpty() {
            key1.SetValue("|1|1001|2001");
            try {
                payee1.AssertIsNotEmpty();
                throw new AssertFailedException("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Compound key: |1|1001|2001 does not contain an object type", ex.Message);
            }
        }

        [TestMethod]
        public void FailsIfTypeNameIsWrong() {
            key1.SetValue("CustomerThree|1|1001|2001");
            try {
                payee1.AssertIsNotEmpty();
                throw new AssertFailedException("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Type: CustomerThree cannot be found", ex.Message);
            }
        }


        [TestMethod]
        public void FailsIfTooFewKeysSupplied() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001");
            try {
                payee1.AssertIsNotEmpty();
                throw new AssertFailedException("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree", ex.Message);
            }
        }


        [TestMethod]
        public void FailsIfTooManyKeysSupplied() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001|2001");
            try {
                payee1.AssertIsNotEmpty();
                throw new AssertFailedException("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo", ex.Message);
            }
        }


        [TestMethod]
        public void ChangeAssociatedObjectType() {
            payee1.SetObject(customer2a);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");
            payee1.SetObject(supplier1);
            Assert.AreEqual(payee1.ContentAsObject, supplier1);

            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.Supplier|1|2001");
        }


        [TestMethod]
        public void ClearAssociatedObject() {
            payee1.SetObject(customer2a);
            payee1.ClearObject();
            key1.AssertIsEmpty();
        }


        [TestMethod]
        public void GetAssociatedObject() {
            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("1");

            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|2|1002");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("2");
        }

        [TestMethod]
        public void NoAssociatedObject() {
            key1.AssertIsEmpty();
        }
    }

    #region Classes used by test

    public class PaymentContext : DbContext {
        public PaymentContext(string name) : base(name) {}

        public DbSet<Payment> Payments { get; set; }
        public DbSet<CustomerOne> CustomerOnes { get; set; }
        public DbSet<CustomerTwo> CustomerTwos { get; set; }
        public DbSet<CustomerThree> CustomerThrees { get; set; }
        public DbSet<CustomerFour> CustomerFours { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }


    public class Payment {
        public IDomainObjectContainer Container { protected get; set; }
        public virtual int Id { get; set; }

        #region Payee Property (Interface Association)

        private IPayee myPayee;
        public IObjectFinder ObjectFinder { set; protected get; }

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

    public class CustomerOne : IPayee {
        [Key]
        public virtual int Id { get; set; }
    }

    public class CustomerTwo : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }


    public class CustomerThree : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }

        [Key]
        [Column(Order = 3)]
        public virtual int Number { get; set; }
    }

    // No Key field
    public class CustomerFour : IPayee {
        public virtual int Id { get; set; }
    }

    public class Supplier : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual short Id2 { get; set; }
    }


    public class Employee : IPayee {
        [Key]
        [Column(Order = 1)]
        public virtual int Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public virtual string Id2 { get; set; }
    }

    #endregion
}