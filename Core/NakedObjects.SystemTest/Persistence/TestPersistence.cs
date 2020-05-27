// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using NakedObjects.Services;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Persistence {
    [TestFixture]
    public class TestPersistence : AbstractSystemTest<PersistenceDbContext> {
        private static bool triggerFail;

        protected override Type[] Types =>
            new[] {
                typeof(ObjectQuery<Qux1>),
                typeof(Foo1)
            };

        protected override Type[] Services => new[] {
            typeof(SimpleRepository<Foo1>),
            typeof(SimpleRepository<Bar1>),
            typeof(SimpleRepository<Qux1>)
        };

        protected override string[] Namespaces => new[] {typeof(Foo1).Namespace};

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            PersistenceDbContext.Delete();
            var context = Activator.CreateInstance<PersistenceDbContext>();

            context.Database.Create();
            MyDbInitialiser.Seed(context);
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            PersistenceDbContext.Delete();
        }

        internal static void FailAsRequired() {
            if (triggerFail) {
                throw new NotImplementedException();
            }
        }

        [Test]
        public virtual void ExceptionInPersistedCausesWholeTransactionToFail() {
            var bar = GetTestService("Bar1s").GetAction("New Instance").InvokeReturnObject();
            try {
                bar.Save();
            }
            catch {
                bar.AssertIsTransient();
            }
        }

        [Test]
        public virtual void ExceptionInUpdatedCausesWholeTransactionToFail() {
            var qs = GetTestService(typeof(SimpleRepository<Qux1>)).GetAction("All Instances");
            var q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
            var name = q.GetPropertyByName("Name");
            name.AssertValueIsEqual("Qux 1");
            try {
                triggerFail = true;
                name.SetValue("yyy");
            }
            catch {
                triggerFail = false;
                q.Refresh();
                q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
                name = q.GetPropertyByName("Name");
                name.AssertValueIsEqual("Qux 1");
            }
        }

        [Test]
        public virtual void IdIsSetByTheTimePersistedIsCalled() {
            var foo = GetTestService(typeof(SimpleRepository<Foo1>)).GetAction("New Instance").InvokeReturnObject();
            foo.AssertIsTransient();
            var id = foo.GetPropertyByName("Id").AssertValueIsEqual("0");
            var idPersisting = foo.GetPropertyByName("Id On Persisting").AssertValueIsEqual("0");
            var idPersisted = foo.GetPropertyByName("Id On Persisted").AssertValueIsEqual("0");
            foo.Save().AssertIsPersistent();
            id.AssertValueIsEqual("1");
            idPersisting.AssertValueIsEqual("0");
            idPersisted.AssertValueIsEqual("1");
        }
    }

    #region Classes used by tests

    public class PersistenceDbContext : DbContext {
        public const string DatabaseName = "TestPersistence";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public PersistenceDbContext() : base(Cs) { }

        public DbSet<Foo1> Foos { get; set; }
        public DbSet<Bar1> Bars { get; set; }
        public DbSet<Qux1> Quxes { get; set; }

        public static void Delete() => Database.Delete(Cs);
    }

    public class MyDbInitialiser {
        public static void Seed(PersistenceDbContext context) {
            var q1 = NewQux("Qux 1", context);
            context.SaveChanges();
        }

        private static Qux1 NewQux(string name, PersistenceDbContext context) {
            var q = new Qux1 {Name = name};
            context.Quxes.Add(q);
            return q;
        }
    }

    public class Foo1 {
        private int myIdOnPersisted;
        public virtual int Id { get; set; }

        public virtual int IdOnPersisting { get; set; }

        public virtual int IdOnPersisted => myIdOnPersisted;

        public void Persisting() {
            IdOnPersisting = Id;
        }

        public void Persisted() {
            myIdOnPersisted = Id;
        }
    }

    public class Bar1 {
        public virtual int Id { get; set; }

        public void Persisted() {
            throw new NotImplementedException();
        }
    }

    public class Qux1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Name { get; set; }

        public void Updated() {
            TestPersistence.FailAsRequired();
        }
    }

    #endregion
}