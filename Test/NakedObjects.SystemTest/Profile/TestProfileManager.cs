// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;
using NakedObjects.Meta.Profile;
using NakedObjects.Profile;

namespace NakedObjects.SystemTest.Profile {
    [TestClass]
    public class TestProfileManager : AbstractSystemTest<ProfileDbContext> {
        #region Run Configuration

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new ProfileConfiguration<MyProfiler>();

            config.EventsToProfile = new HashSet<ProfileEvent> {
                ProfileEvent.ActionInvocation,
                ProfileEvent.PropertySet,
                ProfileEvent.Created,
                ProfileEvent.Deleted,
                ProfileEvent.Deleting,
                ProfileEvent.Loaded,
                ProfileEvent.Loading,
                ProfileEvent.Persisted,
                ProfileEvent.Persisting,
                ProfileEvent.Updated,
                ProfileEvent.Updating
            };

            container.RegisterInstance<IProfileConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IFacetDecorator, ProfileManager>("ProfileManager", new ContainerControlledLifetimeManager());

            var reflectorConfig = new ReflectorConfiguration(new[] {
                typeof (Foo),
                typeof (QueryableList<Foo>) },
                new Type[] {},
                new string[] {typeof (Foo).Namespace});

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
        }

        #endregion

        [TestMethod]
        public void TestActionInvocation() {
            var foo = (Foo) NakedObjectsFramework.Persistor.Instances<Foo>().First();

            foo.TestAction();
        }

        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestProfileManager());
            Database.Delete(ProfileDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("sven");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion
    }

    #region Classes used by tests

    public class ProfileDbContext : DbContext {
        public const string DatabaseName = "TestProfile";
        public ProfileDbContext() : base(DatabaseName) {}
        public DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<ProfileDbContext> {
        protected override void Seed(ProfileDbContext context) {
            context.Foos.Add(new Foo() {Id = 1});
            context.SaveChanges();
        }
    }

    public class MyProfiler : IProfiler {
        #region IProfiler Members

        public void Begin(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName) {
            //throw new NotImplementedException();
        }

        public void End(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName) {
            //throw new NotImplementedException();
        }

        #endregion
    }

    public class Foo {
        public virtual int Id { get; set; }
        public void TestAction() {}
    }

    #endregion
}