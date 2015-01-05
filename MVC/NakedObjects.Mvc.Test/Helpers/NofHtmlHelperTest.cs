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
using System.Data.SqlTypes;
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NakedObjects.Core.Util;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MvcTestApp.Tests.Helpers {
    public static class StringHelper {
        public static string StripWhiteSpace(this string s) {
            return s.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }

        public static string StripWhiteSpace(this MvcHtmlString s) {
            return s.ToString().StripWhiteSpace();
        }
    }

    [TestClass]
    public class NofHtmlHelperTest : AcceptanceTestCase {
        #region Setup/Teardown

        private static bool runFixtures;

        private void RunFixturesOnce() {
            if (!runFixtures) {
                RunFixtures();
                runFixtures = true;
            }
        }


        [TestInitialize]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            RunFixturesOnce();
            StartTest();
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
        }

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());           
        }

        [ClassCleanup]
        public static void TearDownTest() {
            Database.Delete("NofHtmlHelperTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository")).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                var types3 = new Type[] {
                    typeof (EnumerableQuery<string>),
                    typeof (ObjectQuery<Claim>),
                    typeof(Claim[]),
                    typeof(Object[]),

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


        private class DummyController : Controller {}

        private void SetupViewData(object model) {
            mocks.ViewDataContainer.Object.ViewData.Model = model;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NoFramework] = NakedObjectsFramework;
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

        private static bool Writetests = false;
        private const string GeneratedHtmlReferenceFiles = @"..\..\Generated Html reference files";

        private static void CheckResults(string resultsFile, string s) {
            if (Writetests) {
                WriteTestData(resultsFile, s);
            }
            else {
                string actionView = GetTestData(resultsFile);

                // ignore keys 
                const string pattern = "System.Int64;\\d+;";
                const string replacement = "System.Int64;X;";
                var rgx = new Regex(pattern);
                actionView = rgx.Replace(actionView, replacement);
                s = rgx.Replace(s, replacement);


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

        private DescribedCustomHelperTestClass DescribedTestClass {
            get { return (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        private NotPersistedTestClass NotPersistedTestClass {
            get { return new NotPersistedTestClass(); }
        }


        [TestMethod]
        public void ActionDialogId() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            IActionSpec action = NakedObjectsFramework.GetNakedObject(claim).Spec.GetAllActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            Assert.AreEqual(@"Claim-CopyAllExpenseItemsFromAnotherClaim-Dialog", mocks.HtmlHelper.ObjectActionDialogId(claim, action).ToString());
        }

        [TestMethod]
        public void ActionName() {
            Assert.AreEqual(@"<div class=""nof-actionname"">Test</div>", mocks.HtmlHelper.ObjectActionName("Test").ToString());
        }

        [TestMethod]
        public void AutoCompleteParameter() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testAC).Spec.GetAllActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameter", s);
        }

        [TestMethod]
        public void AutoCompleteParameterWithDefault() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select"] = NakedObjectsFramework.GetNakedObject(testAC);
            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input"] = "test1";

            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testAC).Spec.GetAllActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithDefault", s);
        }

        [TestMethod]
        public void AutoCompleteParameterWithExistingValues() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select", new ValueProviderResult(NakedObjectsFramework.GetNakedObject(testAC), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input", new ValueProviderResult("test1", null, null));

            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testAC).Spec.GetAllActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithExistingValues", s);
        }

        [TestMethod]
        public void AutoCompleteProperty() {
            var testAC = (AutoCompleteTestClass) GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testAC).ToString();

            CheckResults("AutoCompleteProperty", s);
        }

        [TestMethod]
        public void BoolParameter() {
            var btc = new BoolTestClass();

            SetupViewData(btc);

            NakedObjectsFramework.Manager.CreateAdapter(btc, null, null);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass, bool>(btc, x => x.TestBoolAction).ToString();


            CheckResults("BoolParameter", s);
        }

        [TestMethod]
        public void BoolPropertyEdit() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyListEdit(testBool).ToString();

            CheckResults("BoolPropertyEdit", s);
        }


        [TestMethod]
        public void BoolPropertyView() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testBool).ToString();
            CheckResults("BoolPropertyView", s);
        }


        [TestMethod]
        public void ChoicesParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameter", s);
        }


        [TestMethod]
        public void ChoicesParameterAlternativeSyntax() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntax", s);
        }

        [TestMethod]
        public void ChoicesParameterAlternativeSyntaxWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm1-Select"] = NakedObjectsFramework.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }

        [TestMethod]
        public void ChoicesParameterAlternativeSyntaxWithExistingValues() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();


            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm1-Select", new ValueProviderResult(NakedObjectsFramework.GetNakedObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }

        [TestMethod]
        public void ChoicesParameterWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm1-Select"] = NakedObjectsFramework.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }

        [TestMethod]
        public void ChoicesParameterWithExistingValues() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();


            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm1-Select", new ValueProviderResult(NakedObjectsFramework.GetNakedObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }


        [TestMethod]
        public void ChoicesProperty() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testChoices).ToString();


            CheckResults("ChoicesProperty", s);
        }

        [TestMethod]
        public void CollectionListViewForEagerlyCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var collection = new[] {claim};

            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionList(collection, action).ToString();

            CheckResults("CollectionListViewForEagerlyCollectionTableView", s);
        }

        // new

        [TestMethod]
        public void CollectionListViewForEmptyCollection() {
            var collection = new object[] {};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }

        [TestMethod]
        public void CollectionListViewForEmptyCollectionTableView() {
            var collection = new object[] {};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }


        [TestMethod]
        public void CollectionListViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionNoPage", s);
        }

        [TestMethod]
        public void CollectionListViewForOneElementCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionNoPageTableView", s);
        }


        [TestMethod]
        public void CollectionListViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollection", s);
        }


        [TestMethod]
        public void CollectionListViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage1", s);
        }

        [TestMethod]
        public void CollectionListViewForPagedCollectionPage1TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage1TableView", s);
        }


        [TestMethod]
        public void CollectionListViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage2", s);
        }

        [TestMethod]
        public void CollectionListViewForPagedCollectionPage2TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage2TableView", s);
        }

        [TestMethod]
        public void CollectionListViewForPagedCollectionTableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionTableView", s);
        }

        [TestMethod]
        public void CollectionViewForEmptyCollection() {
            var collection = new object[] {};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForEmptyCollection", s);
        }

        [TestMethod]
        public void CollectionViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForOneElementCollectionNoPage", s);
        }

        [TestMethod]
        public void CollectionViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForOneElementCollectionWithMultiline", s);
        }


        [TestMethod]
        public void CollectionViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForOneElementCollection", s);
        }

        [TestMethod]
        public void CollectionViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForOneElementCollectionPage1", s);
        }


        [TestMethod]
        public void CollectionViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionViewForOneElementCollectionPage2", s);
        }

        [TestMethod]
        public void CollectionlistViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionWithMultiline", s);
        }

        [TestMethod]
        public void CollectionlistViewForOneElementCollectionWithMultilineTableView() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionWithMultilineTableView", s);
        }

        [TestMethod]
        public void DialogWithAjaxDisabled() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);

            IActionSpec action = adapter.Spec.GetAllActions().Single(p => p.Id == "RejectItems");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("DialogWithAjaxDisabled", s);
        }

        [TestMethod]
        public void DisplayName() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().Menu(DescribedTestClass).ToString();

            CheckResults("DisplayName", s);
        }

        [TestMethod]
        public void DuplicateAction() {
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();
            mocks.ViewDataContainer.Object.ViewData.Model = testBool;
            string s = mocks.GetHtmlHelper<BoolTestClass>().Menu(testBool).ToString();
            CheckResults("DuplicateAction", s);
        }

        [TestMethod]
        public void EmptyEnumerableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = NakedObjectsFramework.Manager.CreateAdapter(new List<ChoicesTestClass>(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = collectionAdapter;

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();


            CheckResults("EmptyEnumerableParameter", s);
        }

        [TestMethod]
        public void EmptyQueryableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = NakedObjectsFramework.Manager.CreateAdapter(new List<ChoicesTestClass>().AsQueryable(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = collectionAdapter;

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();


            CheckResults("EmptyQueryableParameter", s);
        }

        [TestMethod]
        public void EnumParameter() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameter", s);
        }

        [TestMethod]
        public void EnumParameterAnnotation() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestAnnotationEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotation", s);
        }

        [TestMethod]
        public void EnumParameterAnnotationChoices() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestAnnotationEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotationChoices", s);
        }

        [TestMethod]
        public void EnumParameterChoices() {
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestActualEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterChoices", s);
        }

        [TestMethod]
        public void EnumParameterWithDefault() {
            mocks.ViewDataContainer.Object.ViewData["EnumTestClass-TestActualEnumParm-Parm-Input"] = TestEnum.Paris;
            var testChoices = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterWithDefault", s);
        }

        [TestMethod]
        public void EnumProperty() {
            var testEnum = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyListEdit(testEnum).ToString();

            CheckResults("EnumProperty", s);
        }

        [TestMethod]
        public void EnumPropertyView() {
            var testEnum = (EnumTestClass) GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testEnum).ToString();
            CheckResults("EnumPropertyView", s);
        }

        [TestMethod]
        public void EnumerableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = NakedObjectsFramework.Manager.CreateAdapter(new List<ChoicesTestClass> {testChoices}, null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = collectionAdapter;

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();


            CheckResults("EnumerableParameter", s);
        }

        [TestMethod]
        public void GenericAction() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", null, claim).ToString();

            CheckResults("GenericAction", s);
        }

        [TestMethod]
        public void GenericActionWithController() {
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller").ToString();

            CheckResults("GenericActionWithController", s);
        }

        [TestMethod]
        public void GenericActionWithRVDict() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller", new RouteValueDictionary(new {
                id = NakedObjectsFramework.GetObjectId(claim)
            })).ToString();

            CheckResults("GenericAction", s);
        }

        [TestMethod]
        public void GenericEditAction() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", claim).ToString();

            CheckResults("GenericEditAction", s);
        }

        [TestMethod]
        public void GenericEditActionWithController() {
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller").ToString();

            CheckResults("GenericEditActionWithController", s);
        }

        [TestMethod]
        public void GenericEditActionWithRVDict() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller", new RouteValueDictionary(new {
                id = NakedObjectsFramework.GetObjectId(claim)
            })).ToString();


            CheckResults("GenericEditAction", s);
        }

        [TestMethod]
        public void MultiLineField() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEditWith(DescribedTestClass, x => x.TestMultiLineString).ToString();


            CheckResults("MultilineField", s);
        }

        [TestMethod]
        public void MultiLineFieldView() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            tc.TestMultiLineString = "Test String";

            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListWith(tc, x => x.TestMultiLineString).ToString();


            CheckResults("MultilineFieldView", s);
        }


        [TestMethod]
        public void MultiLineParameter() {
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string>(DescribedTestClass, x => x.TestMultiLineFunction).ToString();


            CheckResults("MultilineParameter", s);
        }

        [TestMethod, Ignore] // needs fixing
        public void MultipleChoicesParameterBounded() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesBounded");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterBounded", s);
        }

        [TestMethod]
        public void MultipleChoicesParameterDomainObject1() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesDomainObject1");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject1", s);
        }

        [TestMethod]
        public void MultipleChoicesParameterDomainObject2() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesDomainObject2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject2", s);
        }

        [TestMethod]
        public void MultipleChoicesParameterInt() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesInt");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterInt", s);
        }

        [TestMethod]
        public void MultipleChoicesParameterString() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesString");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterString", s);
        }

        [TestMethod]
        public void MultipleChoicesParameterWithDefault() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm1-Select"] = NakedObjectsFramework.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm2-Select"] = NakedObjectsFramework.Manager.CreateAdapter(new List<string> {"test1", "test2"}, null, null);

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestMultipleChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterWithDefault", s);
        }

        [TestMethod]
        public void NotPersistedMenu() {
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().MenuOnTransient(adapter.Object).ToString();


            CheckResults("NotPersistedMenu", s);
        }

        [TestMethod]
        public void NotPersistedPropertyList() {
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyList(adapter.Object).ToString();


            CheckResults("NotPersistedPropertyList", s);
        }

        [TestMethod]
        public void NotPersistedWithoutButton() {
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyListEditHidden(adapter.Object).ToString();


            CheckResults("NotPersistedWithoutButton", s);
        }

        [TestMethod]
        public void NullableBoolParameter() {
            var btc = new BoolTestClass();
            SetupViewData(btc);
            NakedObjectsFramework.Manager.CreateAdapter(btc, null, null);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass, bool?>(btc, x => x.TestNullableBoolAction).ToString();


            CheckResults("NullableBoolParameter", s);
        }


        [TestMethod]
        public void Object() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object(claim).ToString();


            CheckResults("Object", s);
        }


        [TestMethod]
        public void ObjectActions() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData.Model = claim;
            string s = mocks.HtmlHelper.Menu(claim).ToString();

            CheckResults("ObjectActions", s);
        }


        [TestMethod]
        public void ObjectActionsTestNotContributed1() {
            var nc1 = (NotContributedTestClass1) GetBoundedInstance<NotContributedTestClass1>("NC1Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc1).ToString();


            CheckResults("ObjectActionsTestNotContributed1", s);
        }

        [TestMethod]
        public void ObjectActionsTestNotContributed2() {
            var nc2 = (NotContributedTestClass2) GetBoundedInstance<NotContributedTestClass2>("NC2Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc2).ToString();

            CheckResults("ObjectActionsTestNotContributed2", s);
        }


        [TestMethod]
        public void ObjectActionsWithConcurrency() {
            RecordedAction recordedAction = NakedObjectsFramework.Persistor.Instances<RecordedAction>().First();
            string s = mocks.HtmlHelper.Menu(recordedAction).ToString();


            CheckResults("ObjectActionsWithConcurrency", s);
        }

        [TestMethod]
        public void ObjectActionsWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            string s = mocks.HtmlHelper.Menu(hint).ToString();

            CheckResults("ObjectActionsWithHints", s);
        }

        [TestMethod]
        public void ObjectEditFieldsWithActionAsFind() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            INakedObject employeeRepo = NakedObjectsFramework.GetAdaptedService("EmployeeRepository");
            IActionSpec action = employeeRepo.Spec.GetAllActions().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim, employeeRepo.Object, action, "Approver", null).ToString();


            CheckResults("ObjectEditFieldsWithActionAsFind", s);
        }

        [TestMethod]
        public void ObjectEditFieldsWithFinder() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Employee emp = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            string s = mocks.HtmlHelper.PropertyListEdit(claim, null, null, "Approver", new[] {emp}).ToString();


            CheckResults("ObjectEditFieldsWithFinder", s);
        }

        [TestMethod]
        public void ObjectEditFieldsWithInlineObject() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var claim2 = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 19);


            INakedObject employeeRepo = NakedObjectsFramework.GetAdaptedService("EmployeeRepository");
            IActionSpec action = employeeRepo.Spec.GetAllActions().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim1, employeeRepo.Object, action, "Approver", new[] {claim2}).ToString();


            CheckResults("ObjectEditFieldsWithInlineObject", s);
        }


        [TestMethod]
        public void ObjectEditFieldsWithListCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();


            CheckResults("ObjectEditFieldsWithListCollection", s);
        }

        [TestMethod, Ignore]
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

        [TestMethod]
        public void ObjectEditFieldsWithSummaryCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();


            CheckResults("ObjectEditFieldsWithSummaryCollection", s);
        }

        [TestMethod]
        public void ObjectEditFieldsWithSummaryCollectionForTransient() {
            var claim = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();
            claim.DateCreated = new DateTime(2010, 3, 25);

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();


            CheckResults("ObjectEditFieldsWithSummaryCollectionForTransient", s);
        }

        [TestMethod]
        public void ObjectEditFieldsWithTableCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";

            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();


            CheckResults("ObjectEditFieldsWithTableCollection", s);
        }

        [TestMethod, Ignore]
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

        [TestMethod]
        public void ObjectEditPropertiesWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyListEdit(hint).ToString();

            CheckResults("ObjectEditPropertiesWithHints", s);
        }

        [TestMethod]
        public void ObjectFieldsWithListCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();


            CheckResults("ObjectFieldsWithListCollection", s);
        }

        [TestMethod]
        public void ObjectFieldsWithSummaryCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();

            CheckResults("ObjectFieldsWithSummaryCollection", s);
        }

        [TestMethod]
        public void ObjectFieldsWithTableCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();


            CheckResults("ObjectFieldsWithTableCollection", s);
        }


        [TestMethod]
        public void ObjectForEnumerable() {
            IList<Claim> claims = NakedObjectsFramework.Persistor.Instances<Claim>().Take(2).ToList();

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 2},
                {IdHelper.PagingTotal, 2}
            };
            var claimAdapter = NakedObjectsFramework.Manager.CreateAdapter(claims.First(), null, null);
            var adapter = NakedObjectsFramework.Manager.CreateAdapter(claims, null, null);
            var mockOid = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            adapter.SetATransientOid(mockOid);


            string s = mocks.HtmlHelper.Object(claims).ToString();


            CheckResults("ObjectForEnumerable", s);
        }

        [TestMethod]
        public void ObjectForQueryable() {
            IQueryable<Claim> claims = NakedObjectsFramework.Persistor.Instances<Claim>().Take(2);

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 2},
                {IdHelper.PagingTotal, 2}
            };

            var claimAdapter = NakedObjectsFramework.Manager.CreateAdapter(claims.First(), null, null);
            var adapter = NakedObjectsFramework.Manager.CreateAdapter(claims, null, null);
            var mockOid = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            adapter.SetATransientOid(mockOid);

            string s = mocks.HtmlHelper.Object(claims).ToString();


            CheckResults("ObjectForQueryable", s);
        }

        [TestMethod]
        public void ObjectHasVisibleFields() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Assert.IsTrue(mocks.HtmlHelper.ObjectHasVisibleFields(claim));
        }

        [TestMethod]
        public void ObjectLinkAndIcon() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object("Text", "Action", claim).ToString();


            CheckResults("ObjectLinkAndIcon", s);
        }

        [TestMethod]
        public void ObjectPropertiesWithHints() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyList(hint).ToString();

            CheckResults("ObjectPropertiesWithHints", s);
        }

        [TestMethod]
        public void ObjectTitle() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            Assert.AreEqual(@"28th Mar - Sales call, London", mocks.HtmlHelper.ObjectTitle(claim).ToString());
        }

        [TestMethod]
        public void ObjectTypeAsCssId() {
            Employee emp = NakedObjectsFramework.Persistor.Instances<Employee>().First();
            List<Employee> allEmployees = NakedObjectsFramework.Persistor.Instances<Employee>().ToList();

            string empId = mocks.HtmlHelper.ObjectTypeAsCssId(emp).ToString();
            string allEmpId = mocks.HtmlHelper.ObjectTypeAsCssId(allEmployees).ToString();

            Assert.AreEqual("Expenses-ExpenseEmployees-Employee", empId);
            Assert.AreEqual("System-Collections-Generic-List`1[[Expenses-ExpenseEmployees-Employee]]", allEmpId);
        }

        [TestMethod]
        public void ObjectWithImage() {
            var currency = (Currency) GetBoundedInstance<Currency>("EUR").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(currency).ToString();


            CheckResults("ObjectWithImage", s);
        }


        [TestMethod]
        public void ParameterEdit() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);

            IActionSpec action = adapter.Spec.GetAllActions().First();
            IActionParameterSpec parm = action.Parameters.First();

            string keyToSelect = IdHelper.GetParameterInputId(action, parm);
            INakedObject objToSelect = NakedObjectsFramework.GetNakedObject("Expenses.ExpenseClaims.ExpenseType;4;False");

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue(keyToSelect, new ValueProviderResult(objToSelect, null, null));

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();


            CheckResults("ParameterEdit", s);
        }


        [TestMethod, Ignore]
        public void ParameterEditCollection() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            INakedObject adapter = NakedObjectsFramework.GetNakedObject(tc1);

            IActionSpec action = adapter.Spec.GetAllActions().Single(a => a.Id == "OneCollectionParameterAction");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();


            CheckResults("ParameterEditCollection", s);
        }

        [TestMethod]
        public void ParameterEditForCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().Single(a => a.Id == "MyRecentClaims");

            var selected = claimRepo.GetDomainObject<ClaimRepository>().MyRecentClaims().First();

            INakedObject target = NakedObjectsFramework.Manager.CreateAdapter(new[] {claim}.AsQueryable(), null, null);

            target.SetATransientOid(new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  claimRepo, action, new INakedObject[] { }), new object[] { selected }));

            IActionSpec targetAction = claimRepo.Spec.GetActionLeafNodes().Single(a => a.Id == "ApproveClaims");

            string s = mocks.HtmlHelper.ParameterList(target.Object, null, targetAction, null, "claims", null).ToString();


            CheckResults("ParameterEditForCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void ParameterEditWithActionAsFind() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);
            IActionSpec action = NakedObjectsFramework.GetNakedObject(claim).Spec.GetAllActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec targetAction = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", null).ToString();


            CheckResults("ParameterEditWithActionAsFind", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void ParameterEditWithFinders() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);
            IActionSpec action = NakedObjectsFramework.GetNakedObject(claim).Spec.GetAllActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();


            CheckResults("ParameterEditWithFinders", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void ParameterEditWithInlineObject() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            var claim2 = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 18);


            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim1);
            IActionSpec action = NakedObjectsFramework.GetNakedObject(claim1).Spec.GetAllActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec targetAction = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", new[] {claim2}).ToString();


            CheckResults("ParameterEditWithInlineObject", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void ParameterEditWithSelection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            INakedObject adapter = NakedObjectsFramework.GetNakedObject(claim);
            IActionSpec action = NakedObjectsFramework.GetNakedObject(claim).Spec.GetAllActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, null, action, null, "otherClaim", new[] {claim}).ToString();


            CheckResults("ParameterEditWithSelection", s);
        }

        [TestMethod]
        public void ParameterListWithHint() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();


            INakedObject adapter = NakedObjectsFramework.GetNakedObject(hint);

            IActionSpec action = adapter.Spec.GetAllActions().Single(a => a.Id == "ActionWithParms");

            string s = mocks.HtmlHelper.ParameterList(hint, action).ToString();

            CheckResults("ParameterListWithHint", s);
        }

        [TestMethod]
        public void ParameterWithHint() {
            var hint = (HintTestClass) GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            SetupViewData(hint);
            NakedObjectsFramework.GetNakedObject(hint);

            string s = mocks.GetHtmlHelper<HintTestClass>().ObjectActionAsDialog<HintTestClass, int, int>(hint, x => x.ActionWithParms).ToString();

            CheckResults("ParameterWithHint", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForEmptyCollection() {
            var collection = new object[] {}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForEmptyCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForEmptyCollectionTableView() {
            var collection = new object[] {}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForEmptyCollectionTableView", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();

            CheckResults("QueryableListViewForOneElementCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForOneElementCollectionTableView() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForOneElementCollectionTableView", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForOneElementCollectionWithMultiline", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForOneElementCollectionWithMultilineTableView() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForOneElementCollectionWithMultilineTableView", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForPagedCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage1", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollectionPage1TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage1TableView", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage2", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollectionPage2TableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage2TableView", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableListViewForPagedCollectionTableView() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            IActionSpec action = claimRepo.Spec.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionTableView", s);
        }

        [TestMethod]
        public void QueryableParameter() {
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();


            INakedObject collectionAdapter = NakedObjectsFramework.Manager.CreateAdapter(new List<ChoicesTestClass> {testChoices}.AsQueryable(), null, null);
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = collectionAdapter;

            IActionSpec action = NakedObjectsFramework.GetNakedObject(testChoices).Spec.GetAllActions().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();


            CheckResults("QueryableParameter", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForEmptyCollection() {
            var collection = new object[] {}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForEmptyCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForOneElementCollection() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            var collection = new[] {claim}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForOneElementCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForOneElementCollectionWithMultiline() {
            ProjectCode pc = NakedObjectsFramework.Persistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForOneElementCollectionWithMultiline", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForPagedCollection() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 1}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForPagedCollection", s);
        }

        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForPagedCollectionPage1() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 1},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForPagedCollectionPage1", s);
        }


        [TestMethod, Ignore] // todo investigate change in html
        public void QueryableViewForPagedCollectionPage2() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().First();


            var collection = new[] {claim1}.AsQueryable();
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                {IdHelper.PagingCurrentPage, 2},
                {IdHelper.PagingPageSize, 1},
                {IdHelper.PagingTotal, 2}
            };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;


            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableViewForPagedCollectionPage2", s);
        }


        [TestMethod]
        public void ServiceHasNoVisibleFields() {
            INakedObject service = NakedObjectsFramework.GetAdaptedService("ClaimRepository");
            Assert.IsFalse(mocks.HtmlHelper.ObjectHasVisibleFields(service.Object));
        }

        [TestMethod, Ignore] //Pending completion of Menus work
        public void MainMenus() {
            string s = mocks.HtmlHelper.MainMenus().ToString();


            CheckResults("MainMenus", s);
        }

        [TestMethod, Ignore] //TODO: Failing on title of menu derived from SimpleRepository<T>; 
            //Previously this was because menu was derived from the title of the service object.
        public void ServiceList() {
            string s = mocks.HtmlHelper.Services().ToString();


            CheckResults("ServiceList", s);
        }

        [TestMethod]
        public void TestClientValidationHtml() {
            SetupViewData(DescribedTestClass);
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEdit(mocks.ViewDataContainer.Object.ViewData.Model).ToString();

            CheckResults("ClientValidationHtml", s);
        }

        [TestMethod]
        public void TestClientValidationHtmlDialog() {
            SetupViewData(DescribedTestClass);
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string, int, string, string>(DescribedTestClass, x => x.TestClientValidationFunction).ToString();

            CheckResults("ClientValidationHtmlDialog", s);
        }

        [TestMethod, Ignore]
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

        [TestMethod]
        public void UploadActions() {
            ITestObject testObject = GetBoundedInstance<Currency>("EUR");
            var currency = (Currency) testObject.GetDomainObject();
            IActionSpec action1 = testObject.NakedObject.Spec.GetAllActions().Single(a => a.Id == "UploadImage");
            IActionSpec action2 = testObject.NakedObject.Spec.GetAllActions().Single(a => a.Id == "UploadFile");
            IActionSpec action3 = testObject.NakedObject.Spec.GetAllActions().Single(a => a.Id == "UploadByteArray");

            string s = mocks.HtmlHelper.ParameterList(currency, action1).ToString() +
                       mocks.HtmlHelper.ParameterList(currency, action2) +
                       mocks.HtmlHelper.ParameterList(currency, action3);


            CheckResults("ObjectWithUploads", s);
        }

        [TestMethod]
        public void UserMessages() {
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofWarnings] = new[] {"Warning1", "Warning2"};
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofMessages] = new[] {"Message1", "Message2"};

            string s = mocks.HtmlHelper.UserMessages().ToString();


            CheckResults("NofValidationSummary", s);
        }

        [TestMethod]
        public void ViewModel() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);

            string s = mocks.HtmlHelper.Object(no.Object).ToString();

            CheckResults("ViewModel", s);
        }

        [TestMethod]
        public void ViewModelActions() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.Menu(no.Object).ToString();

            CheckResults("ViewModelActions", s);
        }

        [TestMethod]
        public void ViewModelProperties() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyList(no.Object).ToString();

            CheckResults("ViewModelProperties", s);
        }

        [TestMethod]
        public void ViewModelPropertiesEdit() {
            Employee employee = NakedObjectsFramework.Persistor.Instances<Employee>().First();

            var no = NakedObjectsFramework.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyListEdit(no.Object).ToString();

            CheckResults("ViewModelPropertiesEdit", s);
        }
    }
}