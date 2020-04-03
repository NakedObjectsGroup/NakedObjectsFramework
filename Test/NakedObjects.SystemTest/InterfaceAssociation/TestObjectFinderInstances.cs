//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NakedObjects.Services;
//using NakedObjects.Util;
//using NakedObjects.Xat;

//namespace NakedObjects.SystemTest.ObjectFinderInstances {
//    [TestClass]
//    public class TestObjectFinderInstances : AbstractSystemTest<PaymentContext> {
//        protected override string[] Namespaces {
//            get { return new[] {typeof (Customer).Namespace}; }
//        }

//        protected override object[] MenuServices {
//            get {
//                return new object[] {
//                    new ObjectFinder(),
//                    new SimpleRepository<Customer>(),
//                    new SimpleRepository<Supplier>(),
//                    new MyService()
//                };
//            }
//        }

//        [TestCleanup]
//        public void CleanUp() {}

//        [TestMethod]
//        public void FindInstances() {
//            string namesp = "NakedObjects.SystemTest.ObjectFinderInstances.";
//            ITestAction payees = GetTestService("My Service").GetAction("Payees");
//            var results = payees.InvokeReturnCollection(namesp + "Customer");
//            results.AssertCountIs(2);
//            results.ElementAt(0).AssertIsType(typeof (Customer));

//            results = payees.InvokeReturnCollection(namesp + "Supplier");
//            results.AssertCountIs(3);
//            results.ElementAt(0).AssertIsType(typeof (Supplier));
//        }

//        //This tests that the results are coming back as a Queryable<T>
//        [TestMethod]
//        public void FindInstancesFilteredByInterfaceProperty() {
//            string namesp = "NakedObjects.SystemTest.ObjectFinderInstances.";
//            ITestAction find = GetTestService("My Service").GetAction("Find Payee");
//            var result = find.InvokeReturnObject(namesp + "Customer", 2);
//            result.AssertIsType(typeof (Customer));
//            result.GetPropertyByName("Id").AssertValueIsEqual("2");

//            result = find.InvokeReturnObject(namesp + "Supplier", 3);
//            result.AssertIsType(typeof (Supplier));
//            result.GetPropertyByName("Id").AssertValueIsEqual("3");
//        }

//        #region Setup/Teardown

//        [ClassInitialize]
//        public static void SetupTestFixture(TestContext tc) {
//            Database.SetInitializer(new DatabaseInitializer());
//        }

//        [TestInitialize()]
//        public void TestInitialize() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//        }

//        #endregion
//    }

//    #region Classes used by test

//    public class PaymentContext : DbContext {
//        public const string DatabaseName = "ObjectFinderInstances";

//        public PaymentContext() : base(DatabaseName) {}

//        public DbSet<Customer> Customers { get; set; }
//        public DbSet<Supplier> Suppliers { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
//            Database.SetInitializer(new DatabaseInitializer());
//        }
//    }

//    public class DatabaseInitializer : DropCreateDatabaseAlways<PaymentContext> {
//        protected override void Seed(PaymentContext context) {
//            context.Customers.Add(new Customer());
//            context.Customers.Add(new Customer());
//            context.Suppliers.Add(new Supplier());
//            context.Suppliers.Add(new Supplier());
//            context.Suppliers.Add(new Supplier());
//            context.SaveChanges();
//        }
//    }

//    public interface IPayee {
//        int Id { get; }
//    }

//    public class Customer : IPayee {
//        #region IPayee Members

//        [Disabled]
//        public virtual int Id { get; set; }

//        #endregion
//    }

//    public class Supplier : IPayee {
//        #region IPayee Members

//        [Disabled]
//        public virtual int Id { get; set; }

//        #endregion
//    }

//    public class MyService {
//        public IObjectFinder ObjectFinder { set; protected get; }

//        public IList<IPayee> Payees(string ofType) {
//            Type type = TypeUtils.GetType(ofType);
//            return ObjectFinder.Instances<IPayee>(type).ToList();
//        }

//        public IPayee FindPayee(string ofType, int Id) {
//            Type type = TypeUtils.GetType(ofType);
//            return ObjectFinder.Instances<IPayee>(type).SingleOrDefault(p => p.Id == Id);
//        }
//    }

//    #endregion
//}