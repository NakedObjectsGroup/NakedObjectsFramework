// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Services;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class NofFrameworkHelperTest : AcceptanceTestCase {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public void StartTest() {
            SetUser("sven");
            Fixtures.InstallFixtures(NakedObjectsContext.ObjectPersistor);
        }

        [TearDown]
        public void EndTest() {
            MemoryObjectStore.DiscardObjects();
            ((SimpleOidGenerator)NakedObjectsContext.ObjectPersistor.OidGenerator).ResetTo(100L); 
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions() }); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        protected override IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller { SimpleOidGeneratorStart = 100 }; }
        }

        [Test]
        public void ActionsForHelper() {
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            INakedObject adapter = NakedObjectsContext.GetNakedObject(claim);
            IEnumerable<INakedObjectAction> actions = NakedObjectsContext.GetActions(adapter);
            Assert.AreEqual(8, actions.Count());
        }

        private const string objectId = "NakedObjects.Proxy.Expenses.ExpenseClaims.Claim;132;False";
        private const string genericObjectId = @"NakedObjects.Services.SimpleRepository`1-MvcTestApp.Tests.Helpers.CustomHelperTestClass;5;False";

        [Test]
        public void GetObjectIdForObject() {
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string id = NakedObjectsContext.GetObjectId(claim);
            Assert.AreEqual(id, objectId);
        }

        [Test]
        public void GetObjectIdForGenericObject() {
            object repo = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            string id = NakedObjectsContext.GetObjectId(repo);
            Assert.AreEqual(id, genericObjectId);
        }

        [Test]
        public void GetObjectIdForNakedObjectObject() {
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            INakedObject adapter = NakedObjectsContext.GetNakedObject(claim);
            string id = NakedObjectsContext.GetObjectId(adapter);
            Assert.AreEqual(id, objectId);
        }

        [Test]
        public void GetObjectTypeForObject() {
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string typeId = NakedObjectsContext.GetObjectTypeName(claim);
            Assert.AreEqual(typeId, "Claim");
        }

        [Test]
        public void GetServiceId() {
            const string serviceName = "ClaimRepository";
            string serviceId = NakedObjectsContext.GetServiceId(serviceName);
            Assert.AreEqual("Expenses.ExpenseClaims.ClaimRepository;1;False", serviceId );
        }

        [Test]
        public void GetObjectFromId() {
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            object claim2 = NakedObjectsContext.GetObjectFromId(objectId);
            Assert.AreSame(claim1, claim2);
        }

        [Test]
        public void GetGenericObjectFromId() {
            var repo1 = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            var id = NakedObjectsContext.GetObjectId(repo1);

            object repo2 = NakedObjectsContext.GetObjectFromId(genericObjectId);
            Assert.AreSame(repo1, repo2);
        }


        [Test]
        public void GetNakedObjectFromId() {
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            INakedObject claim2 = NakedObjectsContext.GetNakedObjectFromId(objectId);
            Assert.AreSame(claim1, claim2.Object);
        }

        [Test]
        public void GetCollectionNakedObjectFromId() {
            IList<Claim> claims = NakedObjectsContext.GetService<ClaimRepository>().FindMyClaims(null, "");
            INakedObject no = NakedObjectsContext.ObjectPersistor.CreateAdapter(claims, null, null);

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Where(a => a.Id == "FindMyClaims").Single();
            INakedObject[] parms = new[] { null, "" }.Select(o => NakedObjectsContext.ObjectPersistor.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, NakedObjectsContext.Session, service, action, parms);
            no.SetATransientOid(cm);

            string id = NakedObjectsContext.GetObjectId(no);

            INakedObject no2 = NakedObjectsContext.GetNakedObjectFromId(id);

            List<Claim> claims2 = no2.GetDomainObject<IEnumerable<Claim>>().ToList();

            Assert.AreEqual(claims.Count(), claims2.Count());

            int index = 0;
            Dictionary<Claim, Claim> dict = claims.ToDictionary(x => x, y => claims2.Skip(index++).First());

            dict.ForEach(kvp => Assert.AreSame(kvp.Key, kvp.Value));
        }


        [Test]
        public void GetServices() {    
            var services = NakedObjectsContext.GetAllServices();
            Assert.AreEqual(6, services.Count());
        }


        [Test]
        public void GetServicesMatch() {
        

            var s1 = NakedObjectsContext.GetAdaptedService("EmployeeRepository");
            var s2 = NakedObjectsContext.GetAdaptedService("ClaimRepository");
            var s3 = NakedObjectsContext.GetAdaptedService("RecordedActionRepository");
            var s4 = NakedObjectsContext.GetAdaptedService("RecordActionService");
            var s5 = NakedObjectsContext.GetAdaptedService("RecordedActionContributedActions");
            var s6 = NakedObjectsContext.GetAdaptedService("DummyMailSender");
            var s7 = NakedObjectsContext.GetAdaptedService("repository#MvcTestApp.Tests.Helpers.CustomHelperTestClass");
            var s8 = NakedObjectsContext.GetAdaptedService("repository#MvcTestApp.Tests.Helpers.DescribedCustomHelperTestClass");

            var s11 = NakedObjectsContext.GetAdaptedService<EmployeeRepository>();
            var s12 = NakedObjectsContext.GetAdaptedService<ClaimRepository>();
            var s13 = NakedObjectsContext.GetAdaptedService<RecordedActionRepository>();
            var s14 = NakedObjectsContext.GetAdaptedService<RecordActionService>();
            var s15 = NakedObjectsContext.GetAdaptedService<RecordedActionContributedActions>();
            var s16 = NakedObjectsContext.GetAdaptedService<DummyMailSender>();
            var s17 = NakedObjectsContext.GetAdaptedService<SimpleRepository<CustomHelperTestClass>>();
            var s18 = NakedObjectsContext.GetAdaptedService<SimpleRepository<DescribedCustomHelperTestClass>>();

            var s21 = NakedObjectsContext.GetService<EmployeeRepository>();
            var s22 = NakedObjectsContext.GetService<ClaimRepository>();
            var s23 = NakedObjectsContext.GetService<RecordedActionRepository>();
            var s24 = NakedObjectsContext.GetService<RecordActionService>();
            var s25 = NakedObjectsContext.GetService<RecordedActionContributedActions>();
            var s26 = NakedObjectsContext.GetService<DummyMailSender>();
            var s27 = NakedObjectsContext.GetService<SimpleRepository<CustomHelperTestClass>>();
            var s28 = NakedObjectsContext.GetService<SimpleRepository<DescribedCustomHelperTestClass>>();

            var s31 = NakedObjectsContext.GetService("EmployeeRepository");
            var s32 = NakedObjectsContext.GetService("ClaimRepository");
            var s33 = NakedObjectsContext.GetService("RecordedActionRepository");
            var s34 = NakedObjectsContext.GetService("RecordActionService");
            var s35 = NakedObjectsContext.GetService("RecordedActionContributedActions");
            var s36 = NakedObjectsContext.GetService("DummyMailSender");
            var s37 = NakedObjectsContext.GetService("repository#MvcTestApp.Tests.Helpers.CustomHelperTestClass");
            var s38 = NakedObjectsContext.GetService("repository#MvcTestApp.Tests.Helpers.DescribedCustomHelperTestClass");


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
            var s51 = NakedObjectsContext.GetService<IUserFinder>();
            var s61 = NakedObjectsContext.GetAdaptedService<IUserFinder>();

            Assert.AreSame(s21, s51);
            Assert.AreSame(s11, s61);

        }
    }
}