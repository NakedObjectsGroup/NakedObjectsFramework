// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using Expenses.Currencies;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Component;
using NakedObjects.Core.Util;
using NakedObjects.Menu;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Helpers {
    public static class StringHelper {
        public static string StripWhiteSpace(this string s) {
            return s.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        public static string StripWhiteSpace(this MvcHtmlString s) {
            return s.ToString().StripWhiteSpace();
        }
    }

    [TestFixture]
    //[Ignore]

    public class NofHtmlHelperTest : AcceptanceTestCase {
        private const string GeneratedHtmlReferenceFiles = @"..\..\Generated Html reference files";
        private static readonly bool Writetests = false;
        private DummyController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get { return null; }
        }

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository")).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                var types3 = new Type[] {
                    typeof (EnumerableQuery<string>),
                    typeof (ObjectQuery<Claim>),
                    typeof (Claim),
                    typeof (Employee),
                    typeof (Employee[]),
                    typeof (Claim[]),
                    typeof (ProjectCode[]),
                    typeof (Object[]),
                };

                return types1.Union(types2).Union(types3).ToArray();
            }
        }

        protected override object[] MenuServices {
            get { return (DemoServicesSet.ServicesSet()); }
        }

        protected override object[] ContributedActions {
            get {
                return (new object[] {
                    new RecordedActionContributedActions(),
                    new NotContributedTestService(),
                    new ViewModelTestService()
                });
            }
        }

        protected override object[] Fixtures {
            get { return (DemoFixtureSet.FixtureSet()); }
        }

        private DescribedCustomHelperTestClass DescribedTestClass {
            get {
                var no =   GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject;
                NakedObjectsFramework.TransactionManager.StartTransaction();
                NakedObjectsFramework.LifecycleManager.MakePersistent(no);
                NakedObjectsFramework.TransactionManager.EndTransaction();
                return no.GetDomainObject<DescribedCustomHelperTestClass>();

            }
        }

        private NotPersistedTestClass NotPersistedTestClass {
            get { return new NotPersistedTestClass(); }
        }

        private IIdHelper IdHelper { get { return  new IdHelper();} }
        protected INakedObjectsSurface Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            Surface = this.GetConfiguredContainer().Resolve<INakedObjectsSurface>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
            MessageBroker = NakedObjectsFramework.MessageBroker;
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            SetUser("sven");
            SetupViewData(new object());
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("NofHtmlHelperTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, MVCOid>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBrokerSurface, MessageBrokerWrapper>(new PerResolveLifetimeManager());
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
            Database.Delete("NofHtmlHelperTest");
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) {
            return DemoServicesSet.MainMenus(factory);
        }

        private void SetupViewData(object model) {
            mocks.ViewDataContainer.Object.ViewData.Model = model;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofMainMenus] = NakedObjectsFramework.MetamodelManager.MainMenus();
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoFramework] = NakedObjectsFramework;
            mocks.ViewDataContainer.Object.ViewData["IdHelper"] = new IdHelper();
            mocks.ViewDataContainer.Object.ViewData["Surface"] = Surface;
        }

        private static string GetTestData(string name) {
            return File.ReadAllText(GetFile(name));
        }

        private static string GetFile(string name) {
            return Path.Combine(GeneratedHtmlReferenceFiles, name) + ".htm";
        }

        private static void WriteTestData(string name, string data) {
            File.WriteAllText(GetFile(name), data);
        }

        private static void CheckResults(string resultsFile, string s) {
            if (Writetests) {
                WriteTestData(resultsFile, s);
            }
            else {
                string actionView = GetTestData(resultsFile);

                // ignore keys 
                string pattern = "System.Int64;\\d+;";
                string replacement = "System.Int64;X;";
                var rgx = new Regex(pattern);
                actionView = rgx.Replace(actionView, replacement);
                s = rgx.Replace(s, replacement);

                pattern = "System.Int32;\\d+;";
                replacement = "System.Int32;X;";
                rgx = new Regex(pattern);
                actionView = rgx.Replace(actionView, replacement);
                s = rgx.Replace(s, replacement);

                // normalize new lines 

                actionView = actionView.Replace("\r\n", "\n");
                s = s.Replace("\r\n", "\n");

                //Assert.AreEqual(actionView, s);
                Compare(actionView, s);
            }
        }

        private static void Compare(string expected, string actual) {
            if (expected == actual) {
                return;
            }
            for (int i = 0; i < expected.Length; i++) {
                if (expected.Substring(i, 1) != actual.Substring(i, 1)) {
                    int start = i > 10 ? i - 10 : 0;
                    int maxLength = Math.Min(actual.Length, expected.Length);
                    int length = start + 50 < maxLength ? 50 : maxLength - start;

                    Assert.Fail("Strings unequal at " + i + ". Expected: " + expected.Substring(start, length) + " Actual: " + actual.Substring(start, length));
                }
            }
        }

        [Test]
        public void ActionDialogId() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var action = Surface.GetObject(claim).Specification.GetActionLeafNodes().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            Assert.AreEqual(@"Claim-CopyAllExpenseItemsFromAnotherClaim-Dialog", mocks.HtmlHelper.ObjectActionDialogId(claim, action).ToString());
        }

        [Test]
        public void ActionName() {
            Assert.AreEqual(@"<div class=""nof-actionname"">Test</div>", mocks.HtmlHelper.ObjectActionName("Test").ToString());
        }

        [Test]
        public void AutoCompleteParameter() {
            var testAC = (AutoCompleteTestClass)GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";

            var action = Surface.GetObject(testAC).Specification.GetActionLeafNodes().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameter", s);
        }

        [Test]      
        public void AutoCompleteParameterWithDefault() {
          
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;

            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select"] = Surface.GetObject(testAC);
            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input"] = "test1";

            
            testAC.TestAutoCompleteStringProperty = "test2";
            var action = Surface.GetObject(testAC).Specification.GetActionLeafNodes().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithDefault", s);
        }

        [Test]
       
        public void AutoCompleteParameterWithExistingValues() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select", new ValueProviderResult(Surface.GetObject(testAC), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input", new ValueProviderResult("test1", null, null));

          
            var action = Surface.GetObject(testAC).Specification.GetActionLeafNodes().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithExistingValues", s);
        }

        [Test]
       
        public void AutoCompleteProperty() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testAC).ToString();

            CheckResults("AutoCompleteProperty", s);
        }

        [Test]
        public void BoolParameter() {
            var btc = new BoolTestClass();

            SetupViewData(btc);

            NakedObjectsFramework.NakedObjectManager.CreateAdapter(btc, null, null);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass, bool>(btc, x => x.TestBoolAction).ToString();

            CheckResults("BoolParameter", s);
        }

        [Test]
        public void BoolPropertyEdit() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyListEdit(testBool).ToString();

            CheckResults("BoolPropertyEdit", s);
        }

        [Test]
        public void BoolPropertyView() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testBool).ToString();
            CheckResults("BoolPropertyView", s);
        }

        [Test]
        public void ChoicesParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameter", s);
        }

        [Test]
        public void ChoicesParameterAlternativeSyntax() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntax", s);
        }

        [Test]
        public void ChoicesParameterAlternativeSyntaxWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm1-Select"] = Surface.GetObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }

        [Test]
        public void ChoicesParameterAlternativeSyntaxWithExistingValues() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm1-Select", new ValueProviderResult(Surface.GetObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }

        [Test]
        public void ChoicesParameterWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm1-Select"] = Surface.GetObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }

        [Test]
        public void ChoicesParameterWithExistingValues() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm1-Select", new ValueProviderResult(Surface.GetObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }

        [Test]
        public void ChoicesProperty() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testChoices).ToString();

            CheckResults("ChoicesProperty", s);
        }

        [Test]
        public void CollectionListViewForEagerlyCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var collection = new[] {claim};

            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionList(collection, action).ToString();

            CheckResults("CollectionListViewForEagerlyCollectionTableView", s);
        }

        // new

        [Test]
        public void CollectionListViewForEmptyCollection() {
            var collection = new object[] {};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionListViewForEmptyCollectionTableView() {
            var collection = new object[] {};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionListViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForOneElementCollectionNoPage", s);
        }

        [Test]
        public void CollectionListViewForOneElementCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionNoPageTableView", s);
        }

        [Test]
        public void CollectionListViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForOneElementCollection", s);
        }

        [Test]
        public void CollectionListViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForOneElementCollectionPage1", s);
        }

        [Test]
        public void CollectionListViewForPagedCollectionPage1TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionPage1TableView", s);
        }

        [Test]
        public void CollectionListViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForOneElementCollectionPage2", s);
        }

        [Test]
        public void CollectionListViewForPagedCollectionPage2TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionPage2TableView", s);
        }

        [Test]
        public void CollectionListViewForPagedCollectionTableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionTableView", s);
        }

        [Test]
        public void CollectionViewForEmptyCollection() {
            var collection = new object[] {};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForOneElementCollectionNoPage", s);
        }

        [Test]
        public void CollectionViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void CollectionViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForOneElementCollection", s);
        }

        [Test]
        public void CollectionViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForOneElementCollectionPage1", s);
        }

        [Test]
        public void CollectionViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionViewForOneElementCollectionPage2", s);
        }

        [Test]
        public void CollectionlistViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("CollectionListViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void CollectionlistViewForOneElementCollectionWithMultilineTableView() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc};
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionWithMultilineTableView", s);
        }

        [Test]
        public void DialogWithAjaxDisabled() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var adapter = Surface.GetObject(claim);

            var action = adapter.Specification.GetActionLeafNodes().Single(p => p.Id == "RejectItems");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("DialogWithAjaxDisabled", s);
        }

        [Test]
        public void DisplayName() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().Menu(DescribedTestClass).ToString();

            CheckResults("DisplayName", s);
        }

        [Test]
        public void DuplicateAction() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();
            mocks.ViewDataContainer.Object.ViewData.Model = testBool;
            string s = mocks.GetHtmlHelper<BoolTestClass>().Menu(testBool).ToString();
            CheckResults("DuplicateAction", s);
        }

        [Test]
        public void EmptyEnumerableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            var collectionAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new List<ChoicesTestClass>(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());


            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = Surface.GetObject(collectionAdapter.Object);

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EmptyEnumerableParameter", s);
        }

        [Test]
        public void EmptyQueryableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            var collectionAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new List<ChoicesTestClass>().AsQueryable(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = Surface.GetObject(collectionAdapter.Object); 

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EmptyQueryableParameter", s);
        }

        [Test]
        public void EnumParameter() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameter", s);
        }

        [Test]
        public void EnumParameterAnnotation() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestAnnotationEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotation", s);
        }

        [Test]
        public void EnumParameterAnnotationChoices() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestAnnotationEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotationChoices", s);
        }

        [Test]
        public void EnumParameterChoices() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestActualEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterChoices", s);
        }

        [Test]
        public void EnumParameterWithDefault() {
            mocks.ViewDataContainer.Object.ViewData["EnumTestClass-TestActualEnumParm-Parm-Input"] = TestEnum.Paris;
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterWithDefault", s);
        }

        [Test]
        public void EnumProperty() {
            var testEnum = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyListEdit(testEnum).ToString();

            CheckResults("EnumProperty", s);
        }

        [Test]
        public void EnumPropertyView() {
            var testEnum = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testEnum).ToString();
            CheckResults("EnumPropertyView", s);
        }

        [Test]
        public void EnumerableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            var collectionAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new List<ChoicesTestClass> {testChoices}, null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = collectionAdapter;

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumerableParameter", s);
        }

        [Test]
        public void GenericAction() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", null, claim).ToString();

            CheckResults("GenericAction", s);
        }

        [Test]
        public void GenericActionWithController() {
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller").ToString();

            CheckResults("GenericActionWithController", s);
        }

        [Test]
        public void GenericActionWithRVDict() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller", new RouteValueDictionary(new {
                id = NakedObjectsFramework.GetObjectId(claim)
            })).ToString();

            CheckResults("GenericAction", s);
        }

        [Test]
        public void GenericEditAction() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", claim).ToString();

            CheckResults("GenericEditAction", s);
        }

        [Test]
        public void GenericEditActionWithController() {
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller").ToString();

            CheckResults("GenericEditActionWithController", s);
        }

        [Test]
        public void GenericEditActionWithRVDict() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller", new RouteValueDictionary(new {
                id = NakedObjectsFramework.GetObjectId(claim)
            })).ToString();

            CheckResults("GenericEditAction", s);
        }

        [Test]
        public void MultiLineField() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEditWith(DescribedTestClass, x => x.TestMultiLineString).ToString();

            CheckResults("MultilineField", s);
        }

        [Test]
        public void MultiLineFieldView() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            tc.TestMultiLineString = "Test String";

            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListWith(tc, x => x.TestMultiLineString).ToString();

            CheckResults("MultilineFieldView", s);
        }

        [Test]
        public void MultiLineParameter() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string>(DescribedTestClass, x => x.TestMultiLineFunction).ToString();

            CheckResults("MultilineParameter", s);
        }

        [Test] // needs fixing
        public void MultipleChoicesParameterBounded() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesBounded");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterBounded", s);
        }

        [Test]
        public void MultipleChoicesParameterDomainObject1() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesDomainObject1");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject1", s);
        }

        [Test]
        public void MultipleChoicesParameterDomainObject2() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesDomainObject2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject2", s);
        }

        [Test]
        public void MultipleChoicesParameterInt() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesInt");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterInt", s);
        }

        [Test]
        public void MultipleChoicesParameterString() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesString");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterString", s);
        }

        [Test]
        public void MultipleChoicesParameterWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm1-Select"] = Surface.GetObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm2-Select"] = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new List<string> {"test1", "test2"}, null, null);

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestMultipleChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterWithDefault", s);
        }

        [Test]
        public void NotPersistedMenu() {
            var adapter = Surface.GetObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().MenuOnTransient(adapter.Object).ToString();

            CheckResults("NotPersistedMenu", s);
        }

        [Test]
        public void NotPersistedPropertyList() {
            var adapter = Surface.GetObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyList(adapter.Object).ToString();

            CheckResults("NotPersistedPropertyList", s);
        }

        [Test]
        public void NotPersistedWithoutButton() {
            var adapter = Surface.GetObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyListEditHidden(adapter.Object).ToString();

            CheckResults("NotPersistedWithoutButton", s);
        }

        [Test]
        public void NullableBoolParameter() {
            var btc = new BoolTestClass();
            SetupViewData(btc);
            NakedObjectsFramework.NakedObjectManager.CreateAdapter(btc, null, null);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass, bool?>(btc, x => x.TestNullableBoolAction).ToString();

            CheckResults("NullableBoolParameter", s);
        }

        [Test]
        public void Object() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object(claim).ToString();

            CheckResults("Object", s);
        }

        [Test]
        public void ObjectActions() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData.Model = claim;
            string s = mocks.HtmlHelper.Menu(claim).ToString();

            CheckResults("ObjectActions", s);
        }

        [Test]
        public void ObjectWithNoActions() {
            ExpenseType exp = NakedObjectsFramework.Persistor.Instances<ExpenseType>().First();
            mocks.ViewDataContainer.Object.ViewData.Model = exp;
            string s = mocks.HtmlHelper.Menu(exp).ToString();

            CheckResults("ObjectWithNoActions", s);
        }

        [Test]
        public void ObjectActionsTestNotContributed1() {
            var nc1 = (NotContributedTestClass1) GetBoundedInstance<NotContributedTestClass1>("NC1Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc1).ToString();

            CheckResults("ObjectActionsTestNotContributed1", s);
        }

        [Test]
        public void ObjectActionsTestNotContributed2() {
            var nc2 = (NotContributedTestClass2) GetBoundedInstance<NotContributedTestClass2>("NC2Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc2).ToString();

            CheckResults("ObjectActionsTestNotContributed2", s);
        }

        [Test]
        public void ObjectActionsWithConcurrency() {
            RecordedAction recordedAction = NakedObjectsFramework.Persistor.Instances<RecordedAction>().First();
            string s = mocks.HtmlHelper.Menu(recordedAction).ToString();

            CheckResults("ObjectActionsWithConcurrency", s);
        }

        [Test]
        public void ObjectActionsWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            
            string s = mocks.HtmlHelper.Menu(hint).ToString();

            CheckResults("ObjectActionsWithHints", s);
        }

        [Test]
        public void ObjectEditFieldsWithActionAsFind() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var er = NakedObjectsFramework.GetAdaptedService("EmployeeRepository").Object;
            var employeeRepo = Surface.GetObject(er);
            var action = employeeRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim, employeeRepo.Object, action, "Approver", null).ToString();

            CheckResults("ObjectEditFieldsWithActionAsFind", s);
        }

        [Test]
        public void ObjectEditFieldsWithFinder() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            string s = mocks.HtmlHelper.PropertyListEdit(claim, null, (INakedObjectActionSurface) null, "Approver", new[] {emp}).ToString();

            CheckResults("ObjectEditFieldsWithFinder", s);
        }

        [Test]
        public void ObjectEditFieldsWithInlineObject() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var claim2 = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 19);

            var er = NakedObjectsFramework.GetAdaptedService("EmployeeRepository").Object;
            var employeeRepo = Surface.GetObject(er);
            var action = employeeRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim1, employeeRepo.Object, action, "Approver", new[] {claim2}).ToString();

            CheckResults("ObjectEditFieldsWithInlineObject", s);
        }

        [Test]
        public void ObjectEditFieldsWithListCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();

            CheckResults("ObjectEditFieldsWithListCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithListCollectionAndRemove() {
            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc2 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc3 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc4 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            tc4.TestCollectionOne.Add(tc1);
            tc4.TestCollectionOne.Add(tc2);
            tc4.TestCollectionTwo.Add(tc3);

            mocks.ViewDataContainer.Object.ViewData["TestCollectionOne"] = "list";

            string s = mocks.HtmlHelper.PropertyListEdit(tc4).ToString();

            CheckResults("ObjectEditFieldsWithListCollectionAndRemove", s);
        }

        [Test]
        public void ObjectEditFieldsWithSummaryCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();

            CheckResults("ObjectEditFieldsWithSummaryCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithSummaryCollectionForTransient() {
            var claim = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();
            claim.DateCreated = new DateTime(2010, 3, 25);

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();

            CheckResults("ObjectEditFieldsWithSummaryCollectionForTransient", s);
        }

        [Test]
        public void ObjectEditFieldsWithTableCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();

            CheckResults("ObjectEditFieldsWithTableCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithTableCollectionAndRemove() {
            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc2 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc3 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            var tc4 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            tc4.TestCollectionOne.Add(tc1);
            tc4.TestCollectionOne.Add(tc2);
            tc4.TestCollectionTwo.Add(tc3);

            mocks.ViewDataContainer.Object.ViewData["TestCollectionOne"] = "table";

            string s = mocks.HtmlHelper.PropertyListEdit(tc4).ToString();

            CheckResults("ObjectEditFieldsWithTableCollectionAndRemove", s);
        }

        [Test]
        public void ObjectEditPropertiesWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyListEdit(hint).ToString();

            CheckResults("ObjectEditPropertiesWithHints", s);
        }

        [Test]
        public void ObjectFieldsWithListCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();

            CheckResults("ObjectFieldsWithListCollection", s);
        }

        [Test]
        public void ObjectFieldsWithSummaryCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();

            CheckResults("ObjectFieldsWithSummaryCollection", s);
        }

        [Test]
        public void ObjectFieldsWithTableCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();

            CheckResults("ObjectFieldsWithTableCollection", s);
        }

        [Test]
        public void ObjectForEnumerable() {
            IList<Claim> claims = NakedObjectsFramework.Persistor.Instances<Claim>().Take(2).ToList();

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 2},
                {IdConstants.PagingTotal, 2}
            };
            var claimAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(claims.First(), null, null);
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(claims, null, null);
            var mockOid = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObjectAdapter[] { });

            adapter.SetATransientOid(mockOid);

            string s = mocks.HtmlHelper.Object(claims).ToString();

            CheckResults("ObjectForEnumerable", s);
        }

        [Test]
        public void ObjectForQueryable() {
            IQueryable<Claim> claims = NakedObjectsFramework.Persistor.Instances<Claim>().Take(2);

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 2},
                {IdConstants.PagingTotal, 2}
            };

            var claimAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(claims.First(), null, null);
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(claims, null, null);
            var mockOid = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObjectAdapter[] { });

            adapter.SetATransientOid(mockOid);

            string s = mocks.HtmlHelper.Object(claims).ToString();

            CheckResults("ObjectForQueryable", s);
        }

        [Test]
        public void ObjectHasVisibleFields() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Assert.IsTrue(mocks.HtmlHelper.ObjectHasVisibleFields(claim));
        }

        [Test]
        public void ObjectLinkAndIcon() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object("Text", "Action", claim).ToString();

            CheckResults("ObjectLinkAndIcon", s);
        }

        [Test]
        public void ObjectPropertiesWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyList(hint).ToString();

            CheckResults("ObjectPropertiesWithHints", s);
        }

        [Test]
        public void ObjectTitle() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Assert.AreEqual(@"28th Mar - Sales call, London", mocks.HtmlHelper.ObjectTitle(claim).ToString());
        }

        [Test]
        public void ObjectTypeAsCssId() {
            Employee emp = NakedObjectsFramework.Persistor.Instances<Employee>().First();
            List<Employee> allEmployees = NakedObjectsFramework.Persistor.Instances<Employee>().ToList();

            string empId = mocks.HtmlHelper.ObjectTypeAsCssId(emp).ToString();
            string allEmpId = mocks.HtmlHelper.ObjectTypeAsCssId(allEmployees).ToString();

            Assert.AreEqual("Expenses-ExpenseEmployees-Employee", empId);
            Assert.AreEqual("System-Collections-Generic-List`1[[Expenses-ExpenseEmployees-Employee]]", allEmpId);
        }

        [Test]
        public void ObjectWithImage() {
            var currency = (Currency) GetBoundedInstance<Currency>("EUR").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(currency).ToString();

            CheckResults("ObjectWithImage", s);
        }

        [Test]
        public void ParameterEdit() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var adapter = Surface.GetObject(claim);

            var action = adapter.Specification.GetActionLeafNodes().First();
            var parm = action.Parameters.First();

            string keyToSelect = IdHelper.GetParameterInputId(action, parm);
            var objToSelect = Surface.GetObject("Expenses.ExpenseClaims.ExpenseType;4;False");

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue(keyToSelect, new ValueProviderResult(objToSelect, null, null));

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("ParameterEdit", s);
        }

        [Test]
        public void ParameterEditCollection() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            var adapter = Surface.GetObject(tc1);

            var action = adapter.Specification.GetActionLeafNodes().Single(a => a.Id == "OneCollectionParameterAction");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("ParameterEditCollection", s);
        }

        [Test]
        public void ParameterEditForCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var crNo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            var cr = crNo.Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "MyRecentClaims");
            var oAction = crNo.Spec.GetActionLeafNodes().Single(a => a.Id == "MyRecentClaims");


            var selected = claimRepo.GetDomainObject<ClaimRepository>().MyRecentClaims().First();

            var target = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new[] {claim}.AsQueryable(), null, null);
            var currentMemento = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, crNo, oAction, new INakedObjectAdapter[] { });
            var newMemento = currentMemento.NewSelectionMemento(new object[] {selected}, false);

            target.SetATransientOid(newMemento);

            var targetAction = claimRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "ApproveClaims");

            string s = mocks.HtmlHelper.ParameterList(target.Object, null, targetAction, null, "claims", null).ToString();

            CheckResults("ParameterEditForCollection", s);
        }

        [Test]
        public void ParameterEditWithActionAsFind() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var adapter = Surface.GetObject(claim);
            var action = Surface.GetObject(claim).Specification.GetActionLeafNodes().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var targetAction = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", null).ToString();

            CheckResults("ParameterEditWithActionAsFind", s);
        }

        [Test]
        public void ParameterEditWithFinders() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var adapter = Surface.GetObject(claim);
            var action = Surface.GetObject(claim).Specification.GetActionLeafNodes().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("ParameterEditWithFinders", s);
        }

        [Test]
        public void ParameterEditWithInlineObject() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var claim2 = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 18);

            var adapter = Surface.GetObject(claim1);
            var action = Surface.GetObject(claim1).Specification.GetActionLeafNodes().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var targetAction = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", new[] {claim2}).ToString();

            CheckResults("ParameterEditWithInlineObject", s);
        }

        [Test]
        public void ParameterEditWithSelection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var adapter = Surface.GetObject(claim);
            var action = Surface.GetObject(claim).Specification.GetActionLeafNodes().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, null, action, null, "otherClaim", new[] {claim}).ToString();

            CheckResults("ParameterEditWithSelection", s);
        }

        [Test]
        public void ParameterListWithHint() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            var adapter = Surface.GetObject(hint);

            var action = adapter.Specification.GetActionLeafNodes().Single(a => a.Id == "ActionWithParms");

            string s = mocks.HtmlHelper.ParameterList(hint, action).ToString();

            CheckResults("ParameterListWithHint", s);
        }

        [Test]
        public void ParameterWithHint() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            SetupViewData(hint);
            Surface.GetObject(hint);

            string s = mocks.GetHtmlHelper<HintTestClass>().ObjectActionAsDialog<HintTestClass, int, int>(hint, x => x.ActionWithParms).ToString();

            CheckResults("ParameterWithHint", s);
        }

        [Test]
        public void QueryableListViewForEmptyCollection() {
            var collection = new object[] {}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForEmptyCollection", s);
        }

        [Test]
        public void QueryableListViewForEmptyCollectionTableView() {
            var collection = new object[] {}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForEmptyCollectionTableView", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForOneElementCollection", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForOneElementCollectionTableView", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollectionWithMultilineTableView() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForOneElementCollectionWithMultilineTableView", s);
        }

        [Test]
        public void QueryableListViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForPagedCollection", s);
        }

        [Test]
        public void QueryableListViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForPagedCollectionPage1", s);
        }

        [Test]
        public void QueryableListViewForPagedCollectionPage1TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForPagedCollectionPage1TableView", s);
        }

        [Test]
        public void QueryableListViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableListViewForPagedCollectionPage2", s);
        }

        [Test]
        public void QueryableListViewForPagedCollectionPage2TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForPagedCollectionPage2TableView", s);
        }

        [Test]
        public void QueryableListViewForPagedCollectionTableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            var action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("QueryableListViewForPagedCollectionTableView", s);
        }

        [Test]
        public void QueryableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            var collectionAdapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(new List<ChoicesTestClass> {testChoices}.AsQueryable(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = collectionAdapter;

            var action = Surface.GetObject(testChoices).Specification.GetActionLeafNodes().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("QueryableParameter", s);
        }

        [Test]
        public void QueryableViewForEmptyCollection() {
            var collection = new object[] {}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableViewForEmptyCollection", s);
        }

        [Test]
        public void QueryableViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableViewForOneElementCollection", s);
        }

        [Test]
        public void QueryableViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");

            var collection = new[] {pc}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void QueryableViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableViewForPagedCollection", s);
        }

        [Test]
        public void QueryableViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 1},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface)null).ToString();

            CheckResults("QueryableViewForPagedCollectionPage1", s);
        }

        [Test]
        public void QueryableViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim1}.AsQueryable();
            var adapter = NakedObjectsFramework.NakedObjectManager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdConstants.PagingCurrentPage, 2},
                {IdConstants.PagingPageSize, 1},
                {IdConstants.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdConstants.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, (INakedObjectActionSurface) null).ToString();

            CheckResults("QueryableViewForPagedCollectionPage2", s);
        }

        [Test]
        public void ServiceHasNoVisibleFields() {
            var cr = NakedObjectsFramework.GetAdaptedService("ClaimRepository").Object;
            var claimRepo = Surface.GetObject(cr);
            Assert.IsFalse(mocks.HtmlHelper.ObjectHasVisibleFields(claimRepo.Object));
        }

        [Test]
        public void TestMainMenus() {
            string s = mocks.HtmlHelper.MainMenus().ToString();
            CheckResults("MainMenus", s);
        }

        [Test]
        public void ServiceList() {
            string s = mocks.HtmlHelper.MainMenus().ToString();
            CheckResults("ServiceList", s);
        }

        [Test]
        public void TestClientValidationHtml() {
            SetupViewData(DescribedTestClass);
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEdit(mocks.ViewDataContainer.Object.ViewData.Model).ToString();

            CheckResults("ClientValidationHtml", s);
        }

        [Test]
        public void TestClientValidationHtmlDialog() {
            SetupViewData(DescribedTestClass);
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string, int, string, string>(DescribedTestClass, x => x.TestClientValidationFunction).ToString();

            CheckResults("ClientValidationHtmlDialog", s);
        }

        [Test]
        public void TransientWithCollection() {
            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc2 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            var tc3 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            var tc4 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            tc4.TestCollectionOne.Add(tc1);
            tc4.TestCollectionOne.Add(tc2);
            tc4.TestCollectionTwo.Add(tc3);

            string s = mocks.HtmlHelper.PropertyListEdit(tc4).ToString();

            CheckResults("TransientWithCollection", s);
        }

        [Test]
        public void UploadActions() {
            ITestObject to = GetBoundedInstance<Currency>("EUR");

            var testObject = Surface.GetObject(to.NakedObject.Object);

            var currency = (Currency) testObject.Object;
            var action1 = testObject.Specification.GetActionLeafNodes().Single(a => a.Id == "UploadImage");
            var action2 = testObject.Specification.GetActionLeafNodes().Single(a => a.Id == "UploadFile");
            var action3 = testObject.Specification.GetActionLeafNodes().Single(a => a.Id == "UploadByteArray");

            string s = mocks.HtmlHelper.ParameterList(currency, action1).ToString() +
                       mocks.HtmlHelper.ParameterList(currency, action2) +
                       mocks.HtmlHelper.ParameterList(currency, action3);

            CheckResults("ObjectWithUploads", s);
        }

        [Test]
        public void UserMessages() {
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofWarnings] = new[] { "Warning1", "Warning2" };
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofMessages] = new[] { "Message1", "Message2" };

            string s = mocks.HtmlHelper.UserMessages().ToString();

            CheckResults("NofValidationSummary", s);
        }

        [Test]
        public void ViewModel() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);

            string s = mocks.HtmlHelper.Object(no.Object).ToString();

            CheckResults("ViewModel", s);
        }

        [Test]
        public void ViewModelActions() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.Menu(no.Object).ToString();

            CheckResults("ViewModelActions", s);
        }

        [Test]
        public void ViewModelProperties() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyList(no.Object).ToString();

            CheckResults("ViewModelProperties", s);
        }

        [Test]
        public void ViewModelPropertiesEdit() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyListEdit(no.Object).ToString();

            CheckResults("ViewModelPropertiesEdit", s);
        }

        #region Nested type: DummyController

        private class DummyController : Controller {}

        #endregion
    }
}