// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace NakedObjects.SystemTest.Repositories {


    [TestClass]
    public class TestSimpleRepository : AbstractSystemTest<SimpleRepositoryDbContext> {
                #region Setup/Teardown
        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            InitializeNakedObjectsFramework(new TestSimpleRepository());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestSimpleRepository());
            Database.Delete(SimpleRepositoryDbContext.DatabaseName);
        }

        private Customer cust1;
        private Customer cust2;

        [TestInitialize()]
        public void TestInitialize()
        {
            StartTest();
            ITestObject cust1To = NewTestObject<Customer>();
            cust1 = (Customer)cust1To.GetDomainObject();
            cust1.Id = 1;
            cust1To.Save();

            ITestObject cust2To = NewTestObject<Customer>();
            cust2 = (Customer)cust2To.GetDomainObject();
            cust2.Id = 2;
            cust2To.Save();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                           new SimpleRepository<Customer>()
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
            var result = find.InvokeReturnObject(1000);
            Assert.IsNull(result);
        }
    }

    #region Classes used in tests

    public class SimpleRepositoryDbContext : DbContext
    {
        public const string DatabaseName = "TestSimpleRepository";
        public SimpleRepositoryDbContext() : base(DatabaseName) { }

        public DbSet<Customer> Customer { get; set; }
    }

    public class Customer 
    {
        public virtual int Id { get; set; }

    }


    #endregion
}