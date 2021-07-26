// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using NakedFramework;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.Container {
    [TestFixture]
    public class TestContainer : AbstractSystemTest<ContainerDbContext> {
        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ContainerDbContext.Delete();
            var context = Activator.CreateInstance<ContainerDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            ContainerDbContext.Delete();
        }

        protected override Type[] ObjectTypes => new[] {typeof(Object1), typeof(Object2), typeof(ViewModel2), typeof(TestEnum)};

        protected override Type[] Services =>
            new[] {
                typeof(SimpleRepository<Object1>)
            };

        [Test]
        public void DefaultsTransient() {
            var testObject = (Object1) NewTestObject<Object1>().GetDomainObject();
            Assert.IsNotNull(testObject.Container);

            var o2 = testObject.Container.NewTransientInstance<Object2>();

            Assert.AreEqual(new DateTime(), o2.TestDateTime);
            Assert.IsNull(o2.TestNullableDateTime);
            Assert.AreEqual(0, o2.TestInt);
            Assert.IsNull(o2.TestNullableInt);

            Assert.AreEqual(TestEnum.Value1, o2.TestEnum);
            Assert.IsNull(o2.TestNullableEnum);

            Assert.AreEqual(0, o2.TestEnumDt);
            Assert.IsNull(o2.TestNullableEnumDt);
        }

        [Test]
        public void DefaultsViewModel() {
            var testObject = (Object1) NewTestObject<Object1>().GetDomainObject();
            Assert.IsNotNull(testObject.Container);

            var vm = testObject.NewViewModel();

            Assert.AreEqual(new DateTime(), vm.TestDateTime);
            Assert.IsNull(vm.TestNullableDateTime);
            Assert.AreEqual(0, vm.TestInt);
            Assert.IsNull(vm.TestNullableInt);

            Assert.AreEqual(TestEnum.Value1, vm.TestEnum);
            Assert.IsNull(vm.TestNullableEnum);

            Assert.AreEqual(0, vm.TestEnumDt);
            Assert.IsNull(vm.TestNullableEnumDt);
        }
    }

    #region Domain classes used by tests

    public class ContainerDbContext : DbContext {
        public const string DatabaseName = "TestContainer";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public ContainerDbContext() : base(Cs) { }
        public DbSet<Object1> Object1 { get; set; }
        public static void Delete() => Database.Delete(Cs);
    }

    public class Object1 {
        [NakedObjectsIgnore] // as container is public for test
        public IDomainObjectContainer Container { get; set; }

        public virtual int Id { get; set; }

        public ViewModel2 NewViewModel() => Container.NewViewModel<ViewModel2>();
    }

    public class Object2 {
        public virtual int Id { get; set; }

        public DateTime TestDateTime { get; set; }

        public DateTime? TestNullableDateTime { get; set; }

        public int TestInt { get; set; }

        public int? TestNullableInt { get; set; }

        public TestEnum TestEnum { get; set; }

        public TestEnum? TestNullableEnum { get; set; }

        [EnumDataType(typeof(TestEnum))]
        public int TestEnumDt { get; set; }

        [EnumDataType(typeof(TestEnum))]
        public int? TestNullableEnumDt { get; set; }
    }

    public enum TestEnum {
        Value1,
        Value2
    }

    public class ViewModel2 : IViewModel {
        public virtual int Id { get; set; }

        public DateTime TestDateTime { get; set; }

        public DateTime? TestNullableDateTime { get; set; }

        public int TestInt { get; set; }

        public int? TestNullableInt { get; set; }

        public TestEnum TestEnum { get; set; }

        public TestEnum? TestNullableEnum { get; set; }

        [EnumDataType(typeof(TestEnum))]
        public int TestEnumDt { get; set; }

        [EnumDataType(typeof(TestEnum))]
        public int? TestNullableEnumDt { get; set; }

        #region IViewModel Members

        public string[] DeriveKeys() {
            return new string[] { };
        }

        public void PopulateUsingKeys(string[] keys) { }

        #endregion
    }

    #endregion
}