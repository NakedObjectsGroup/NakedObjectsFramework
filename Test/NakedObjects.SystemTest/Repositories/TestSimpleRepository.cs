// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Repositories {


    [TestClass, Ignore]
    public class TestSimpleRepository : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize()]
        public void SetUp() {
            InitializeNakedObjectsFramework(this);
            ITestObject cust1To = NewTestObject<Customer>();
            cust1 = (Customer) cust1To.GetDomainObject();
            cust1.Id = 1;
            cust1To.Save();

            ITestObject cust2To = NewTestObject<Customer>();
            cust2 = (Customer) cust2To.GetDomainObject();
            cust2.Id = 2;
            cust2To.Save();

            ITestObject sup1To = NewTestObject<Supplier>();
            sup1 = (Supplier) sup1To.GetDomainObject();
            sup1.Id = 1;
            sup1To.Save();

        }

        [TestCleanup()]
        public void TearDown() {
            CleanupNakedObjectsFramework(this);
            cust1 = null;
            cust2 = null;
            sup1 = null;
        }

        #endregion




        private Customer cust1;
        private Customer cust2;
        private Supplier sup1;


        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                           new SimpleRepository<Customer>(),
                           new SimpleRepository<Supplier>(),
                });
            }
        }

        [TestMethod]
        public void FindByKey() {
            var find = GetTestService("Customers").GetAction("Find By Key");
            var result = find.InvokeReturnObject(1);
            result.GetPropertyByName("Id").AssertValueIsEqual("1");
            result = find.InvokeReturnObject(2);
            result.GetPropertyByName("Id").AssertValueIsEqual("2");
        }

        [TestMethod]
        public void KeyValueDoesNotExist() {
            var find = GetTestService("Customers").GetAction("Find By Key");
            var result = find.InvokeReturnObject(5);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestKeyNotSpecified() {
            var find = GetTestService("Suppliers").GetAction("Find By Key");
            try
            {
                var result = find.InvokeReturnObject(1);
                Assert.Fail();
            }
            catch (NakedObjectDomainException e)
            {
                Assert.AreEqual("Cannot find key for NakedObjects.SystemTest.Repositories.Supplier", e.Message);
            }
        }

    }

    #region Classes used in tests
    public interface IPayee { }

    public class Customer : IPayee
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

    }

    public class Supplier : IPayee
    {
        public int Id { get; set; }
    }

    #endregion
}