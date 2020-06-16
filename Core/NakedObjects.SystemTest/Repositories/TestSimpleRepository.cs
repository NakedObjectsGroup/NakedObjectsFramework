// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using NakedObjects.Services;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Repositories {
    [TestFixture]
    public class TestSimpleRepository : AbstractSystemTest<SimpleRepositoryDbContext> {
        private Customer cust1;
        private Customer cust2;

        protected override Type[] Services =>
            new[] {
                typeof(SimpleRepository<Customer>)
            };

        protected override string[] Namespaces => new[] {typeof(Customer).Namespace};

        [SetUp]
        public void SetUp() {
            StartTest();
            var cust1To = NewTestObject<Customer>();
            cust1 = (Customer) cust1To.GetDomainObject();
            cust1.Id = 1;
            cust1To.Save();

            var cust2To = NewTestObject<Customer>();
            cust2 = (Customer) cust2To.GetDomainObject();
            cust2.Id = 2;
            cust2To.Save();
        }

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            SimpleRepositoryDbContext.Delete();
            var context = Activator.CreateInstance<SimpleRepositoryDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            SimpleRepositoryDbContext.Delete();
        }

        [Test]
        public void FindByKey() {
            var find = GetTestService("Customers").GetAction("Find By Key");
            var result = find.InvokeReturnObject(1);
            result.GetPropertyByName("Id").AssertValueIsEqual("1");
            result = find.InvokeReturnObject(2);
            result.GetPropertyByName("Id").AssertValueIsEqual("2");
        }

        [Test]
        public void KeyValueDoesNotExist() {
            var find = GetTestService("Customers").GetAction("Find By Key");
            var result = find.InvokeReturnObject(1000);
            Assert.IsNull(result);
        }
    }

    #region Classes used in tests

    public class SimpleRepositoryDbContext : DbContext {
        public const string DatabaseName = "TestSimpleRepository";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

        public SimpleRepositoryDbContext() : base(Cs) { }

        public DbSet<Customer> Customer { get; set; }

        public static void Delete() => Database.Delete(Cs);
    }

    public class Customer {
        public virtual int Id { get; set; }
    }

    #endregion
}