// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;

namespace NakedObjects.SystemTest.Container {
    [TestClass]
    public class TestContainer : AbstractSystemTest<ContainerDbContext> {
        protected override Type[] Types {
            get { return new Type[] {typeof (Object1), typeof(Object2), typeof(ViewModel2)}; }
        }

        protected override object[] MenuServices {
            get {
                return (new object[] {
                    new SimpleRepository<Object1>()
                });
            }
        }

        [TestMethod]
        public void DefaultsTransient() {
            var testObject = (Object1) NewTestObject<Object1>().GetDomainObject();
            Assert.IsNotNull(testObject.Container);

            var o2 = testObject.Container.NewTransientInstance<Object2>();

            Assert.AreEqual(o2.TestDateTime, new DateTime());
            Assert.IsNull(o2.TestNullableDateTime);
            Assert.AreEqual(o2.TestInt, 0);
            Assert.IsNull(o2.TestNullableInt);
        }

        [TestMethod]
        public void DefaultsViewModel() {
            var testObject = (Object1)NewTestObject<Object1>().GetDomainObject();
            Assert.IsNotNull(testObject.Container);

            var vm = testObject.NewViewModel();

            Assert.AreEqual(vm.TestDateTime, new DateTime());
            Assert.IsNull(vm.TestNullableDateTime);
            Assert.AreEqual(vm.TestInt, 0);
            Assert.IsNull(vm.TestNullableInt);
        }

        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestContainer());
            Database.Delete(ContainerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        #endregion
    }

    #region Domain classes used by tests

    public class ContainerDbContext : DbContext {
        public const string DatabaseName = "TestContainer";
        public ContainerDbContext() : base(DatabaseName) {}
        public DbSet<Object1> Object1 { get; set; }
    }

    public class Object1 {
        public IDomainObjectContainer Container { get; set; }
        public virtual int Id { get; set; }

        public ViewModel2 NewViewModel() {
            return Container.NewViewModel<ViewModel2>();
        }
    }

    public class Object2 {
        public virtual int Id { get; set; }

        public DateTime TestDateTime { get; set; }

        public DateTime? TestNullableDateTime { get; set; }

        public int TestInt { get; set; }

        public int? TestNullableInt { get; set; }

    }

    public class ViewModel2  : IViewModel{
        public virtual int Id { get; set; }

        public DateTime TestDateTime { get; set; }

        public DateTime? TestNullableDateTime { get; set; }

        public int TestInt { get; set; }

        public int? TestNullableInt { get; set; }

        public string[] DeriveKeys() {
          //  throw new NotImplementedException();
            return new string[]{};
        }

        public void PopulateUsingKeys(string[] keys) {
           // throw new NotImplementedException();
        }
    }


    #endregion
}