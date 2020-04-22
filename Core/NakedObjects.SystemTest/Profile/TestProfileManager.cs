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
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Profile;
using NakedObjects.Profile;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.SystemTest.Profile
{
    [TestFixture]
    public class TestProfileManager : AbstractSystemTest<ProfileDbContext>
    {
        [Test]
        public void TestActionInvocation()
        {
            ITestObject foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();

            bool beginCalled = false;
            bool endCalled = false;

            Action<IPrincipal, ProfileEvent, Type, string> testCallback = (p, e, t, s) =>
            {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual(ProfileEvent.ActionInvocation, e);
                Assert.AreEqual(typeof(Foo), t);
                Assert.AreEqual("TestAction", s);
            };

            MyProfiler.BeginCallback = (p, e, t, s) =>
            {
                beginCalled = true;
                testCallback(p, e, t, s);
            };

            MyProfiler.EndCallback = (p, e, t, s) =>
            {
                endCalled = true;
                testCallback(p, e, t, s);
            };

            foo.Actions.First().Invoke();

            Assert.IsTrue(beginCalled);
            Assert.IsTrue(endCalled);
        }

        [Test]
        [Ignore("investigate")]
        public void TestPropertySet()
        {
            ITestObject foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();

            int beginCalledCount = 0;
            int endCalledCount = 0;

            var callbackData = new List<CallbackData>();

            Action<CallbackData> testCallback0 = cbd =>
            {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.Loading, cbd.ProfileEvent);
                Assert.AreEqual(typeof(string), cbd.Type);
                Assert.AreEqual("", cbd.Member);
            };

            Action<CallbackData> testCallback1 = cbd =>
            {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.Loaded, cbd.ProfileEvent);
                Assert.AreEqual(typeof(string), cbd.Type);
                Assert.AreEqual("", cbd.Member);
            };

            Action<CallbackData> testCallback2 = cbd =>
            {
                Assert.AreEqual("sven", cbd.Principal.Identity.Name);
                Assert.AreEqual(ProfileEvent.PropertySet, cbd.ProfileEvent);
                Assert.AreEqual(typeof(Foo), cbd.Type);
                Assert.AreEqual("TestProperty", cbd.Member);
            };

            MyProfiler.BeginCallback = (p, e, t, s) =>
            {
                beginCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            MyProfiler.EndCallback = (p, e, t, s) =>
            {
                endCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            foo.Properties.Last().SetValue("avalue");

            Assert.AreEqual(3, beginCalledCount);
            Assert.AreEqual(3, endCalledCount);

            testCallback0(callbackData[0]);
            testCallback0(callbackData[1]);
            testCallback1(callbackData[2]);
            testCallback1(callbackData[3]);
            testCallback2(callbackData[4]);
            testCallback2(callbackData[5]);
        }

        [Test]
        [Ignore("investigate")]
        public void TestCallbacks()
        {
            int beginCalledCount = 0;
            int endCalledCount = 0;

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
                typeof (SimpleRepository<Foo>),
                typeof (SimpleRepository<Foo>),
                typeof (SimpleRepository<Foo>),
                typeof (SimpleRepository<Foo>),
                typeof (SimpleRepository<Foo>),
                typeof (int),
                typeof (int),
                typeof (int),
                typeof (int),
                typeof (Foo),
                typeof (Foo),
                typeof (SimpleRepository<Foo>),
                typeof (int),
                typeof (int),
                typeof (int),
                typeof (int),
                typeof (Foo),
                typeof (Foo),
                typeof (string),
                typeof (string),
                typeof (string),
                typeof (string),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (string),
                typeof (string),
                typeof (string),
                typeof (string),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo),
                typeof (Foo)
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

            Func<int, Action<CallbackData>> createTestFunc = i =>
            {
                return cbd =>
                {
                    Assert.AreEqual("sven", cbd.Principal.Identity.Name, i.ToString());
                    Assert.AreEqual(events[i], cbd.ProfileEvent, i.ToString());
                    Assert.AreEqual(types[i], cbd.Type, i.ToString());
                    Assert.AreEqual(members[i], cbd.Member, i.ToString());
                };
            };

            MyProfiler.BeginCallback = (p, e, t, s) =>
            {
                beginCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            MyProfiler.EndCallback = (p, e, t, s) =>
            {
                endCalledCount++;
                callbackData.Add(new CallbackData(p, e, t, s));
            };

            NakedObjectsFramework.TransactionManager.StartTransaction();

            ITestObject foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
            foo.Properties.First().SetValue("101");
            foo.Properties.Last().SetValue("avalue");
            foo.Save();

            NakedObjectsFramework.TransactionManager.EndTransaction();

            NakedObjectsFramework.TransactionManager.StartTransaction();

            foo.Properties.Last().SetValue("anothervalue");

            NakedObjectsFramework.TransactionManager.EndTransaction();

            Assert.AreEqual(20, beginCalledCount);
            Assert.AreEqual(20, endCalledCount);

            for (int i = 0; i < 40; i++)
            {
                createTestFunc(i)(callbackData[i]);
            }
        }

        #region Nested type: CallbackData

        private class CallbackData
        {
            public CallbackData(IPrincipal principal, ProfileEvent profileEvent, Type type, string member)
            {
                Principal = principal;
                ProfileEvent = profileEvent;
                Type = type;
                Member = member;
            }

            public IPrincipal Principal { get; private set; }
            public ProfileEvent ProfileEvent { get; private set; }
            public Type Type { get; private set; }
            public string Member { get; private set; }
        }

        #endregion

        #region Setup/Teardown


        [OneTimeSetUp]
        public  void ClassInitialize()
        {
            ProfileDbContext.Delete();
            var context = Activator.CreateInstance<ProfileDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        protected override Type[] Types => new[] {
            typeof(Foo),
            typeof(QueryableList<Foo>)
        };

        protected override Type[] Services => new[] {typeof(SimpleRepository<Foo>)};

        protected override string[] Namespaces => new[] { typeof(Foo).Namespace };

        protected override void RegisterTypes(IServiceCollection services)
        {
            base.RegisterTypes(services);
            var config = new ProfileConfiguration<MyProfiler>
            {
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





        [OneTimeTearDown]
        public  void ClassCleanup()
        {
            CleanupNakedObjectsFramework(this);
            ProfileDbContext.Delete();
        }

        [SetUp()]
        public void SetUp()
        {
            StartTest();
            SetUser("sven");
        }

        [TearDown()]
        public void TearDown()
        {
            base.EndTest();
            MyProfiler.BeginCallback = (p, e, t, s) => { };
            MyProfiler.EndCallback = (p, e, t, s) => { };
        }

        #endregion
    }

    #region Classes used by tests

    public class ProfileDbContext : DbContext
    {
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

        public static void Delete() => System.Data.Entity.Database.Delete(Cs);

        public const string DatabaseName = "TestProfile";
        public ProfileDbContext() : base(Cs) { }
        public DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
    }

    public class DatabaseInitializer : DropCreateDatabaseAlways<ProfileDbContext>
    {
        protected override void Seed(ProfileDbContext context)
        {
            context.Foos.Add(new Foo() { Id = 1 });
            context.SaveChanges();
        }
    }

    public class MyProfiler : IProfiler
    {
        public static Action<IPrincipal, ProfileEvent, Type, string> BeginCallback = (p, e, t, s) => { };
        public static Action<IPrincipal, ProfileEvent, Type, string> EndCallback = (p, e, t, s) => { };

        #region IProfiler Members

        public void Begin(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName)
        {
            BeginCallback(principal, profileEvent, onType, memberName);
        }

        public void End(IPrincipal principal, ProfileEvent profileEvent, Type onType, string memberName)
        {
            EndCallback(principal, profileEvent, onType, memberName);
        }

        #endregion
    }

    public class Foo
    {
        public virtual int Id { get; set; }
        public string TestProperty { get; set; }
        public void TestAction() { }
    }

    #endregion
}