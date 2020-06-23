// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Profile;
using NakedObjects.Profile;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.Profile {
    [TestFixture]
    public class TestProfileManager : AbstractSystemTest<ProfileDbContext> {
        protected override Type[] Types => new[] {
            typeof(Foo),
            typeof(QueryableList<Foo>)
        };

        protected override Type[] Services => new[] {typeof(SimpleRepository<Foo>)};

        protected override string[] Namespaces => new[] {typeof(Foo).Namespace};

        [SetUp]
        public void SetUp() {
            StartTest();
            SetUser("sven");
        }

        [TearDown]
        public void TearDown() {
            base.EndTest();
            MyProfiler.BeginCallback = (p, e, t, s) => { };
            MyProfiler.EndCallback = (p, e, t, s) => { };
        }

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ProfileDbContext.Delete();
            var context = Activator.CreateInstance<ProfileDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            ProfileDbContext.Delete();
        }

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            var config = new ProfileConfiguration<MyProfiler> {
                EventsToProfile = new HashSet<ProfileEvent> {
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
                }
            };

            services.AddSingleton<IProfileConfiguration>(config);
            services.AddSingleton<IFacetDecorator, ProfileManager>();
        }

        [Test]
        public void TestActionInvocation() {
            var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();

            var beginCalled = false;
            var endCalled = false;

            void TestCallback(IPrincipal p, ProfileEvent e, Type t, string s) {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual(ProfileEvent.ActionInvocation, e);
                Assert.AreEqual(typeof(Foo), t);
                Assert.AreEqual("TestAction", s);
            }

            MyProfiler.BeginCallback = (p, e, t, s) => {
                beginCalled = true;
                TestCallback(p, e, t, s);
            };

            MyProfiler.EndCallback = (p, e, t, s) => {
                endCalled = true;
                TestCallback(p, e, t, s);
            };

            foo.Actions.First().Invoke();

            Assert.IsTrue(beginCalled);
            Assert.IsTrue(endCalled);
        }

        [Test]
        public void TestCallbacks() {
            var beginCalledCount = 0;
            var endCalledCount = 0;

            var callbackData = new List<CallbackData>();

            var events = new[] {
                ProfileEvent.Loading,
                ProfileEvent.Loading,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.ActionInvocation,
                ProfileEvent.Loading,
                ProfileEvent.Loading,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.Created,
                ProfileEvent.Created,
                ProfileEvent.ActionInvocation,
                ProfileEvent.Loading,
                ProfileEvent.Loading,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.PropertySet,
                ProfileEvent.PropertySet,
                ProfileEvent.Loading,
                ProfileEvent.Loading,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.PropertySet,
                ProfileEvent.PropertySet,
                ProfileEvent.Persisting,
                ProfileEvent.Persisting,
                ProfileEvent.Persisted,
                ProfileEvent.Persisted,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.Loading,
                ProfileEvent.Loading,
                ProfileEvent.Loaded,
                ProfileEvent.Loaded,
                ProfileEvent.PropertySet,
                ProfileEvent.PropertySet,
                ProfileEvent.Updating,
                ProfileEvent.Updating,
                ProfileEvent.Updated,
                ProfileEvent.Updated
            };

            var types = new[] {
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo>),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(Foo),
                typeof(Foo),
                typeof(SimpleRepository<Foo>),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(Foo),
                typeof(Foo),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(string),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo),
                typeof(Foo)
            };

            var members = new[] {
                "", // 0
                "",
                "",
                "",
                "NewInstance",
                "",
                "",
                "",
                "",
                "",
                "", // 10
                "NewInstance",
                "",
                "",
                "",
                "",
                "Id",
                "Id",
                "",
                "",
                "", // 20
                "",
                "TestProperty",
                "TestProperty",
                "",
                "",
                "",
                "",
                "",
                "",
                "", // 30
                "",
                "",
                "",
                "TestProperty",
                "TestProperty",
                "",
                "",
                "",
                ""
            };

            Action<CallbackData> CreateTestFunc(int i) {
                return cbd => {
                    Assert.AreEqual("sven", cbd.Principal.Identity.Name, i.ToString());
                    Assert.AreEqual(events[i], cbd.ProfileEvent, i.ToString());
                    Assert.AreEqual(types[i], cbd.Type, i.ToString());
                    Assert.AreEqual(members[i], cbd.Member, i.ToString());
                };
            }

            MyProfiler.BeginCallback = (p, e, t, s) => {
                beginCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            MyProfiler.EndCallback = (p, e, t, s) => {
                endCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            NakedObjectsFramework.TransactionManager.StartTransaction();

            var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
            foo.Properties.First().SetValue("101");
            foo.Properties.Last().SetValue("avalue");
            foo.Save();

            NakedObjectsFramework.TransactionManager.EndTransaction();

            NakedObjectsFramework.TransactionManager.StartTransaction();

            foo.Properties.Last().SetValue("anothervalue");

            NakedObjectsFramework.TransactionManager.EndTransaction();

            Assert.AreEqual(20, beginCalledCount);
            Assert.AreEqual(20, endCalledCount);

            for (var i = 0; i < 40; i++) {
                CreateTestFunc(i)(callbackData[i]);
            }
        }

        [Test]
        public void TestPropertySet() {
            var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();

            var beginCalledCount = 0;
            var endCalledCount = 0;

            var callbackData = new List<CallbackData>();

            void TestCallback0(CallbackData cbd) {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.Loading, cbd.ProfileEvent);
                Assert.AreEqual(typeof(string), cbd.Type);
                Assert.AreEqual("", cbd.Member);
            }

            void TestCallback1(CallbackData cbd) {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.Loaded, cbd.ProfileEvent);
                Assert.AreEqual(typeof(string), cbd.Type);
                Assert.AreEqual("", cbd.Member);
            }

            void TestCallback2(CallbackData cbd) {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.PropertySet, cbd.ProfileEvent);
                Assert.AreEqual(typeof(Foo), cbd.Type);
                Assert.AreEqual("TestProperty", cbd.Member);
            }

            MyProfiler.BeginCallback = (p, e, t, s) => {
                beginCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            MyProfiler.EndCallback = (p, e, t, s) => {
                endCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            foo.Properties.Last().SetValue("avalue");

            Assert.AreEqual(3, beginCalledCount);
            Assert.AreEqual(3, endCalledCount);

            TestCallback0(callbackData[0]);
            TestCallback0(callbackData[1]);
            TestCallback1(callbackData[2]);
            TestCallback1(callbackData[3]);
            TestCallback2(callbackData[4]);
            TestCallback2(callbackData[5]);
        }

        #region Nested type: CallbackData

        private class CallbackData {
            public CallbackData(IPrincipal principal, ProfileEvent profileEvent, Type type, string member) {
                Principal = principal;
                ProfileEvent = profileEvent;
                Type = type;
                Member = member;
            }

            public IPrincipal Principal { get; }
            public ProfileEvent ProfileEvent { get; }
            public Type Type { get; }
            public string Member { get; }
        }

        #endregion
    }

    #region Classes used by tests

    public class ProfileDbContext : DbContext {
        public const string DatabaseName = "TestProfile";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public ProfileDbContext() : base(Cs) { }
        public DbSet<Foo> Foos { get; set; }

        public static void Delete() => Database.Delete(Cs);

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<ProfileDbContext> {
        protected override void Seed(ProfileDbContext context) {
            context.Foos.Add(new Foo {Id = 1});
            context.SaveChanges();
        }
    }

    public class MyProfiler : IProfiler {
        public static Action<IPrincipal, ProfileEvent, Type, string> BeginCallback = (p, e, t, s) => { };
        public static Action<IPrincipal, ProfileEvent, Type, string> EndCallback = (p, e, t, s) => { };

        #region IProfiler Members

        public void Begin(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName) {
            BeginCallback(principal, profileEvent, onType, memberName);
        }

        public void End(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName) {
            EndCallback(principal, profileEvent, onType, memberName);
        }

        #endregion
    }

    public class Foo {
        public virtual int Id { get; set; }
        public string TestProperty { get; set; }
        public void TestAction() { }
    }

    #endregion
}