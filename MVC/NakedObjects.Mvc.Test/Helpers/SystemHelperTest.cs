// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Component;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    //[Ignore]

    public class SystemHelperTest : AcceptanceTestCase {
        private static readonly bool writeTest = false;
        private DummyController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get { return Types.Select(t => t.Namespace).Distinct().ToArray(); }
        }

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository")).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                return types1.Union(types2).ToArray();
            }
        }

        protected override object[] MenuServices {
            get { return (DemoServicesSet.ServicesSet()); }
        }

        protected override object[] ContributedActions {
            get { return (new object[] {new RecordedActionContributedActions()}); }
        }

        protected override object[] Fixtures {
            get { return (DemoFixtureSet.FixtureSet()); }
        }

        private IIdHelper IdHelper { get { return new IdHelper(); } }
        protected INakedObjectsSurface Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("SystemHelperTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, MVCOid>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            Database.Delete("SystemHelperTest");
        }

        private void SetupViewData() {
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoFramework] = NakedObjectsFramework;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoSurface] = Surface;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.IdHelper] = new IdHelper();
        }

        private static string GetTestData(string name) {
            var file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            return File.ReadAllText(file);
        }

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
        public void History() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp);
            string s = mocks.HtmlHelper.History().StripWhiteSpace();
            CheckResults("History", s);
        }

        [Test]
        public void HistoryWithCount1() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            Employee emp1 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);

            string s = mocks.HtmlHelper.History(3).StripWhiteSpace();
            CheckResults("HistoryWithCount", s);
        }

        [Test]
        public void HistoryWithCount2() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Employee emp1 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderBy(c => c.Id).First();
            Employee emp2 = NakedObjectsFramework.Persistor.Instances<Employee>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.History(emp2);
            mocks.HtmlHelper.History(claim);
            mocks.HtmlHelper.History(emp1);

            string s = mocks.HtmlHelper.History(2).StripWhiteSpace();
            CheckResults("History", s);
        }

        #region Nested type: DummyController

        private class DummyController : Controller {}

        #endregion

        #region Setup/Teardown

        private static bool runFixtures;

        private void RunFixturesOnce() {
            if (!runFixtures) {
                RunFixtures();
                runFixtures = true;
            }
        }

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            RunFixturesOnce();
            StartTest();
            Surface = this.GetConfiguredContainer().Resolve<INakedObjectsSurface>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
            MessageBroker = NakedObjectsFramework.MessageBroker;
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
            SetupViewData();
        }

        #endregion
    }
}