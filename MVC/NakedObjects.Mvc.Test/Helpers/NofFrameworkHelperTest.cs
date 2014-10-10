// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Services;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class NofFrameworkHelperTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            SetUser("sven");
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("NofFrameworkHelperTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(this);
            RunFixtures();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            Database.Delete("NofFrameworkHelperTest");
        }


        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {new RecordedActionContributedActions()}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }


        private const string objectId = "Expenses.ExpenseClaims.Claim;1;System.Int32;1;False;;0";
        private const string genericObjectId = @"NakedObjects.Services.SimpleRepository`1-MvcTestApp.Tests.Helpers.CustomHelperTestClass;1;System.Int64;507;True;;0";

        [Test]
        public void ActionsForHelper() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);
            IEnumerable<INakedObjectAction> actions = NakedObjectsFramework.GetActions(adapter);
            Assert.AreEqual(8, actions.Count());
        }

        [Test]
        public void GetCollectionNakedObjectFromId() {
            IList<Claim> claims = NakedObjectsFramework.GetService<ClaimRepository>().FindMyClaims(null, "");
            INakedObject no = NakedObjectsFramework.LifecycleManager.CreateAdapter(claims, null, null);

            INakedObject service = NakedObjectsFramework.LifecycleManager.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Where(a => a.Id == "FindMyClaims").Single();
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            no.SetATransientOid(cm);

            string id = NakedObjectsFramework.GetObjectId(no);

            INakedObject no2 = NakedObjectsFramework.GetNakedObjectFromId(id);

            List<Claim> claims2 = no2.GetDomainObject<IEnumerable<Claim>>().ToList();

            Assert.AreEqual(claims.Count(), claims2.Count());

            int index = 0;
            Dictionary<Claim, Claim> dict = claims.ToDictionary(x => x, y => claims2.Skip(index++).First());

            dict.ForEach(kvp => Assert.AreSame(kvp.Key, kvp.Value));
        }

        [Test, Ignore] // fix later
        public void GetGenericObjectFromId() {
            var repo1 = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            var id = NakedObjectsFramework.GetObjectId(repo1);

            object repo2 = NakedObjectsFramework.GetObjectFromId(genericObjectId);
            Assert.AreSame(repo1, repo2);
        }


        [Test]
        public void GetNakedObjectFromId() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var id = NakedObjectsFramework.GetObjectId(claim1);

            INakedObject claim2 = NakedObjectsFramework.GetNakedObjectFromId(objectId);
            Assert.AreSame(claim1, claim2.Object);
        }

        [Test]
        public void GetObjectFromId() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            object claim2 = NakedObjectsFramework.GetObjectFromId(objectId);
            Assert.AreSame(claim1, claim2);
        }

        [Test, Ignore] // fix later
        public void GetObjectIdForGenericObject() {
            object repo = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            string id = NakedObjectsFramework.GetObjectId(repo);
            Assert.AreEqual(genericObjectId, id);
        }

        [Test]
        public void GetObjectIdForNakedObjectObject() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);
            string id = NakedObjectsFramework.GetObjectId(adapter);
            Assert.AreEqual(id, objectId);
        }

        [Test]
        public void GetObjectIdForObject() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string id = NakedObjectsFramework.GetObjectId(claim);
            Assert.AreEqual(id, objectId);
        }

        [Test]
        public void GetObjectTypeForObject() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string typeId = NakedObjectsFramework.GetObjectTypeName(claim);
            Assert.AreEqual(typeId, "Claim");
        }

        [Test]
        public void GetServiceId() {
            const string serviceName = "ClaimRepository";
            string serviceId = NakedObjectsFramework.GetServiceId(serviceName);
            Assert.AreEqual("Expenses.ExpenseClaims.ClaimRepository;1;System.Int32;0;False;;0", serviceId);
        }


        [Test]
        public void GetServices() {
            var services = NakedObjectsFramework.GetAllServices();
            Assert.AreEqual(6, services.Count());
        }


        [Test]
        public void GetServicesMatch() {
            var s1 = NakedObjectsFramework.GetAdaptedService("EmployeeRepository");
            var s2 = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            var s3 = NakedObjectsFramework.GetAdaptedService("RecordedActionRepository");
            var s4 = NakedObjectsFramework.GetAdaptedService("RecordActionService");
            var s5 = NakedObjectsFramework.GetAdaptedService("RecordedActionContributedActions");
            var s6 = NakedObjectsFramework.GetAdaptedService("DummyMailSender");
            var s7 = NakedObjectsFramework.GetAdaptedService("repository#MvcTestApp.Tests.Helpers.CustomHelperTestClass");
            var s8 = NakedObjectsFramework.GetAdaptedService("repository#MvcTestApp.Tests.Helpers.DescribedCustomHelperTestClass");

            var s11 = NakedObjectsFramework.GetAdaptedService<EmployeeRepository>();
            var s12 = NakedObjectsFramework.GetAdaptedService<ClaimRepository>();
            var s13 = NakedObjectsFramework.GetAdaptedService<RecordedActionRepository>();
            var s14 = NakedObjectsFramework.GetAdaptedService<RecordActionService>();
            var s15 = NakedObjectsFramework.GetAdaptedService<RecordedActionContributedActions>();
            var s16 = NakedObjectsFramework.GetAdaptedService<DummyMailSender>();
            var s17 = NakedObjectsFramework.GetAdaptedService<SimpleRepository<CustomHelperTestClass>>();
            var s18 = NakedObjectsFramework.GetAdaptedService<SimpleRepository<DescribedCustomHelperTestClass>>();

            var s21 = NakedObjectsFramework.GetService<EmployeeRepository>();
            var s22 = NakedObjectsFramework.GetService<ClaimRepository>();
            var s23 = NakedObjectsFramework.GetService<RecordedActionRepository>();
            var s24 = NakedObjectsFramework.GetService<RecordActionService>();
            var s25 = NakedObjectsFramework.GetService<RecordedActionContributedActions>();
            var s26 = NakedObjectsFramework.GetService<DummyMailSender>();
            var s27 = NakedObjectsFramework.GetService<SimpleRepository<CustomHelperTestClass>>();
            var s28 = NakedObjectsFramework.GetService<SimpleRepository<DescribedCustomHelperTestClass>>();

            var s31 = NakedObjectsFramework.GetService("EmployeeRepository");
            var s32 = NakedObjectsFramework.GetService("ClaimRepository");
            var s33 = NakedObjectsFramework.GetService("RecordedActionRepository");
            var s34 = NakedObjectsFramework.GetService("RecordActionService");
            var s35 = NakedObjectsFramework.GetService("RecordedActionContributedActions");
            var s36 = NakedObjectsFramework.GetService("DummyMailSender");
            var s37 = NakedObjectsFramework.GetService("repository#MvcTestApp.Tests.Helpers.CustomHelperTestClass");
            var s38 = NakedObjectsFramework.GetService("repository#MvcTestApp.Tests.Helpers.DescribedCustomHelperTestClass");


            Assert.AreSame(s1, s11);
            Assert.AreSame(s2, s12);
            Assert.AreSame(s3, s13);
            Assert.AreSame(s4, s14);
            Assert.AreSame(s5, s15);
            Assert.AreSame(s6, s16);
            Assert.AreSame(s7, s17);
            Assert.AreSame(s8, s18);

            Assert.AreSame(s1.Object, s11.Object);
            Assert.AreSame(s2.Object, s12.Object);
            Assert.AreSame(s3.Object, s13.Object);
            Assert.AreSame(s4.Object, s14.Object);
            Assert.AreSame(s5.Object, s15.Object);
            Assert.AreSame(s6.Object, s16.Object);
            Assert.AreSame(s7.Object, s17.Object);
            Assert.AreSame(s8.Object, s18.Object);

            Assert.AreSame(s1.Object, s21);
            Assert.AreSame(s2.Object, s22);
            Assert.AreSame(s3.Object, s23);
            Assert.AreSame(s4.Object, s24);
            Assert.AreSame(s5.Object, s25);
            Assert.AreSame(s6.Object, s26);
            Assert.AreSame(s7.Object, s27);
            Assert.AreSame(s8.Object, s28);

            Assert.AreSame(s1.Object, s31);
            Assert.AreSame(s2.Object, s32);
            Assert.AreSame(s3.Object, s33);
            Assert.AreSame(s4.Object, s34);
            Assert.AreSame(s5.Object, s35);
            Assert.AreSame(s6.Object, s36);
            Assert.AreSame(s7.Object, s37);
            Assert.AreSame(s8.Object, s38);


            // test getting by base class
            var s51 = NakedObjectsFramework.GetService<IUserFinder>();
            var s61 = NakedObjectsFramework.GetAdaptedService<IUserFinder>();

            Assert.AreSame(s21, s51);
            Assert.AreSame(s11, s61);
        }
    }
}