// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class SystemHelperTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
            SetupViewData();
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("SystemHelperTest"));
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
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
            Database.Delete("SystemHelperTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        private void SetupViewData() {
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NoFramework] = NakedObjectsFramework;
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

        protected override IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller {SimpleOidGeneratorStart = 100}; }
        }

        private class DummyController : Controller {}


        private static string GetTestData(string name) {
            var file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            return File.ReadAllText(file);
        }

        private static bool writeTest = false;

        private static void WriteTestData(string name, string data) {
            string file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            File.WriteAllText(file, data);
        }

        private static void CheckResults(string resultsFile, string s) {
            if (writeTest) {
                WriteTestData(resultsFile, s);
            }
            else {
                string actionView = GetTestData(resultsFile).StripWhiteSpace();
                Assert.AreEqual(actionView, s.StripWhiteSpace());
            }
        }

        [Test]
        [Ignore] // doesn't work now uses urls which are empty in tests
        public void Cancel() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().First();


            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp);
            string s = mocks.HtmlHelper.CancelButton(null).ToString();
            string fieldView = GetTestData("Cancel");
            Assert.AreEqual(fieldView, s);
        }

        [Test]
        public void History() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().First();

            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp);
            string s = mocks.HtmlHelper.History().StripWhiteSpace();
            CheckResults("History", s);
        }

        [Test]
        public void HistoryWithCount1() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();

            Employee emp1 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderByDescending(c => c.Id).First();


            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);

            string s = mocks.HtmlHelper.History(3).StripWhiteSpace();
            CheckResults("HistoryWithCount", s);
        }

        [Test]
        public void HistoryWithCount2() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderByDescending(c => c.Id).First();


            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);

            string s = mocks.HtmlHelper.History(2).StripWhiteSpace();
            CheckResults("History", s);
        }

        // too hard to mock appropriately - rely on selenium tests
        [Test, Ignore]
        public void TabbedHistory() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().First();

            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp);
            string s = mocks.HtmlHelper.TabbedHistory().StripWhiteSpace();
            CheckResults("TabbedHistory", s);
        }

        [Test, Ignore]
        public void TabbedHistoryWithCount1() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderByDescending(c => c.Id).First();


            mocks.HtmlHelper.TabbedHistory(emp2);
            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp1);

            string s = mocks.HtmlHelper.TabbedHistory(3).StripWhiteSpace();
            CheckResults("TabbedHistoryWithCount", s);
        }

        [Test, Ignore]
        public void TabbedHistoryWithCount2() {
            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.ObjectPersistor.Instances<Employee>().OrderByDescending(c => c.Id).First();


            mocks.HtmlHelper.TabbedHistory(emp2);
            mocks.HtmlHelper.TabbedHistory(claim);
            mocks.HtmlHelper.TabbedHistory(emp1);

            string s = mocks.HtmlHelper.TabbedHistory(2).StripWhiteSpace();
            CheckResults("TabbedHistory", s);
        }
    }
}