// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {

    [TestFixture]
    public class SystemHelperTest : AcceptanceTestCase {
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
            ((NakedObjectPersistorAbstract)NakedObjectsContext.ObjectPersistor).OidGenerator = new SimpleOidGenerator(100L);
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

        private class DummyController : Controller { }
        private readonly Controller controller = new DummyController();

        private static string GetTestData(string name) {
            var file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            return File.ReadAllText(file);
        }

        private static void WriteTestData(string name, string data) {
            string file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            File.WriteAllText(file, data);
        }

        private static void CheckResults(string resultsFile, string s) {
            string actionView = GetTestData(resultsFile).StripWhiteSpace();
            Assert.AreEqual(actionView, s.StripWhiteSpace());
            //WriteTestData(resultsFile, s);
        }

        [Test]
        public void History() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp);     
            string s = mocks.HtmlHelper.History().StripWhiteSpace();
            CheckResults("History", s);
        }

        [Test]
        public void HistoryWithCount1() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            Employee emp2 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().Last();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);
            
            string s = mocks.HtmlHelper.History(3).StripWhiteSpace();
            CheckResults("HistoryWithCount", s);
        }

        [Test]
        public void HistoryWithCount2() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            Employee emp2 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().Last();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);
           
            string s = mocks.HtmlHelper.History(2).StripWhiteSpace();
            CheckResults("History", s); 
        }

        // too hard to mock appropriately - rely on selenium tests
        [Test, Ignore]
        public void TabbedHistory() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp);
            string s = mocks.HtmlHelper.TabbedHistory().StripWhiteSpace();
            CheckResults("TabbedHistory", s);
        }

        [Test, Ignore]
        public void TabbedHistoryWithCount1() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            Employee emp2 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().Last();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.HtmlHelper.TabbedHistory(emp2);
            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp1);

            string s = mocks.HtmlHelper.TabbedHistory(3).StripWhiteSpace();
            CheckResults("TabbedHistoryWithCount", s);
        }

        [Test, Ignore]
        public void TabbedHistoryWithCount2() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            Employee emp2 = NakedObjectsContext.ObjectPersistor.Instances<Employee>().Last();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.HtmlHelper.TabbedHistory(emp2);
            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp1);

            string s = mocks.HtmlHelper.TabbedHistory(2).StripWhiteSpace();
            CheckResults("TabbedHistory", s);
        }

        [Test]
        [Ignore] // doesn't work now uses urls which are empty in tests
        public void Cancel() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp);
            string s = mocks.HtmlHelper.CancelButton(null).ToString();
            string fieldView = GetTestData("Cancel");
            Assert.AreEqual(fieldView, s);
        }
    }
}
