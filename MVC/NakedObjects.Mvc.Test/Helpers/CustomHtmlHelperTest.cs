// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;

namespace MvcTestApp.Tests.Helpers {
    [TestClass]
    public class CustomHtmlHelperTest : AcceptanceTestCase {
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
            InitializeNakedObjectsFrameworkOnceOnly();
            RunFixturesOnce();
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("CustomHtmlHelperTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());
        }

        [ClassCleanup]
        public static void TearDownTest() {
            Database.Delete("CustomHtmlHelperTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository")).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                var tempIt = new List<CustomHelperTestClass>().Where(i => true).Select(i => i);

                var types3 = new Type[] { tempIt.GetType() };


                return types1.Union(types2).Union(types3).ToArray();
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


        private class DummyController : Controller {}


        private static string GetTestData(string name) {
            return File.ReadAllText(GetFile(name));
        }

        private static string GetFile(string name) {
            return Path.Combine(CustomHtmlReferenceFiles, name) + ".htm";
        }

        // for testcreation 

        private static bool Writetests = false;
        private const string CustomHtmlReferenceFiles = @"..\..\Custom Html reference files";

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
                const string pattern = "System.Int64;\\d+;";
                const string replacement = "System.Int64;X;";
                var rgx = new Regex(pattern);
                actionView = rgx.Replace(actionView, replacement);
                s = rgx.Replace(s, replacement);

                Assert.AreEqual(actionView, s);
            }
        }


        private void CustomHelperTest(Func<ContextMocks, string> func, string toCompare) {
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            SetupViewData(tc);
            string s = func(mocks);
            CheckResults(toCompare, s);
        }


        private void CustomHelperTestOtherObj(Func<ContextMocks, string> func, string toCompare) {
            SetupViewData(new object());
            string s = func(mocks);
            CheckResults(toCompare, s);
        }


        private void DescriptionCustomHelperTestCompareDirect(Func<ContextMocks, string> func, string toCompare) {
            var tc = (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            SetupViewData(tc);
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void DescriptionCustomHelperTestCompareDirectOtherObj(Func<ContextMocks, string> func, string toCompare) {
            SetupViewData(new object());
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void CustomHelperTestCollection(Func<ContextMocks, string> func, string toCompare) {
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            var collection = new List<CustomHelperTestClass> {tc};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(collection, null, null);
            adapter.SetATransientOid(new DummyOid());


            SetupViewData(collection);
            string s = func(mocks);
            CheckResults(toCompare, s);
        }

        private void SetupViewData(object model) {
            mocks.ViewDataContainer.Object.ViewData.Model = model;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofMainMenus] = NakedObjectsFramework.Metamodel.MainMenus();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NoFramework] = NakedObjectsFramework;
        }

        private void CustomHelperTestCompareDirect(Func<ContextMocks, string> func, string toCompare) {
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            SetupViewData(tc);
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void CustomHelperTestCompareDirectOtherObj(Func<ContextMocks, string> func, string toCompare) {
            SetupViewData(new object());
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }


        private CustomHelperTestClass TestClass {
            get { return (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        private DescribedCustomHelperTestClass DescribedTestClass {
            get { return (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        [TestMethod]
        public void CollectionExclusions() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout("TestCollectionOne", "TestInt").ToString(),
                "CollectionWithExclusions");
        }

        [TestMethod]
        public void CollectionExclusionsGeneric() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "CollectionWithExclusions");
        }

        [TestMethod]
        public void CollectionExclusionsGenericOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(tc, null, null);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "CollectionWithExclusions");
        }

        [TestMethod]
        public void CollectionExclusionsOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(tc, null, null);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                "CollectionWithExclusions");
        }

        [TestMethod]
        public void CollectionInclusions() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith("TestInt", "TestCollectionOne").ToString(),
                "CollectionWithInclusions");
        }

        [TestMethod]
        public void CollectionInclusionsGeneric() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "CollectionWithInclusions");
        }

        [TestMethod]
        public void CollectionInclusionsGenericOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(tc, null, null);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "CollectionWithInclusions");
        }

        [TestMethod]
        public void CollectionInclusionsOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = NakedObjectsFramework.Manager.CreateAdapter(tc, null, null);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(tc, "TestInt", "TestCollectionOne").ToString(),
                "CollectionWithInclusions");
        }

        [TestMethod]
        public void CollectionTitles() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().CollectionTitles(tc, "{0} {1}").ToString(), "CollectionTitles");
        }

        [TestMethod, Ignore] // todo problem with specs needs thorough investigation
        public void Collections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc).ToString(), "Collections");
        }

        [TestMethod, Ignore] // todo problem with specs needs thorough investigation
        public void CollectionsFormatList() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc, IdHelper.ListDisplayFormat).ToString(), "CollectionsFormatList");
        }

        [TestMethod, Ignore] // todo problem with specs needs thorough investigation
        public void CollectionsFormatTable() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc, IdHelper.TableDisplayFormat).ToString(), "CollectionsFormatTable");
        }

        [TestMethod]
        public void ContentsFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents("OneValueParameterAction", 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, "OneValueParameterAction", 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents("TestInt").ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsPropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents(y => y.TestInt).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsPropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, y => y.TestInt).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsPropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, "TestInt").ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void ContentsTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                "0");
        }

        [TestMethod]
        public void CustomHelperTestAsDialog() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(tc);
            string s = mocks.HtmlHelper.ObjectActionAsDialog("FourValueParametersFunction").ToString();
            CheckResults("CustomHelperTestAsDialog", s);
        }

        [TestMethod]
        public void CustomHelperTestAsDialogOnOtherObject() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(tc);
            string s = mocks.HtmlHelper.ObjectActionAsDialog(tc, "FourValueParametersFunction").ToString();
            CheckResults("CustomHelperTestAsDialog", s);
        }

        [TestMethod]
        public void CustomHelperTestStringId() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(tc);
            string s = mocks.HtmlHelper.ObjectAction("NoParameterAction").ToString();


            CheckResults("NoParameterAction", s);
        }

        [TestMethod]
        public void CustomHelperTestStringIdDisabled() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(tc);
            string s = mocks.HtmlHelper.ObjectAction("DisabledAction").ToString();


            CheckResults("DisabledAction", s);
        }

        [TestMethod]
        public void CustomHelperTestStringIdHidden() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(tc);
            string s = mocks.HtmlHelper.ObjectAction("HiddenAction").ToString();
            Assert.AreEqual("", s);
        }

        [TestMethod]
        public void CustomHelperTestStringIdOnOtherObject() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(new object());

            string s = mocks.HtmlHelper.ObjectAction(tc, "NoParameterAction").ToString();


            CheckResults("NoParameterAction", s);
        }

        [TestMethod]
        public void CustomHelperTestStringIdOnOtherObjectDisabled() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(new object());
            string s = mocks.HtmlHelper.ObjectAction(tc, "DisabledAction").ToString();


            CheckResults("DisabledAction", s);
        }


        [TestMethod]
        public void CustomHelperTestStringIdOnOtherObjectEdit() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(new object());
            string s = mocks.HtmlHelper.ObjectActionOnTransient(tc, "NoParameterAction").ToString();


            CheckResults("NoParameterEditAction", s);
        }

        [TestMethod]
        public void CustomHelperTestStringIdOnOtherObjectHidden() {
            CustomHelperTestClass tc = TestClass;
            SetupViewData(new object());
            string s = mocks.HtmlHelper.ObjectAction(tc, "HiddenAction").ToString();
            Assert.AreEqual("", s);
        }

        [TestMethod]
        public void DateTimePropertyEdit() {
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(new NotPersistedTestClass());


            SetupViewData(adapter.Object);
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().ObjectPropertyEdit(y => y.TestDateTime).ToString();


            CheckResults("TestDateTime", s);
        }

        [TestMethod, Ignore]
        public void DescriptionFourParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionFourParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionFourParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionFourParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionOneParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionOneParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionOneParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionOneParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description("OneValueParameterAction", 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, "OneValueParameterAction", 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionProperty() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description("TestInt").ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionPropertyGeneric() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description(y => y.TestInt).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionPropertyGenericOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, y => y.TestInt).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionPropertyOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, "TestInt").ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionTest() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description().ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionThreeParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionThreeParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionThreeParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionThreeParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionTwoParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionTwoParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionTwoParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                "aDescription");
        }

        [TestMethod, Ignore]
        public void DescriptionTwoParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                "aDescription");
        }

        [TestMethod]
        public void FourRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.FourRefParametersAction).ToString(),
                "FourRefParametersAction");
        }


        [TestMethod]
        public void FourRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.FourRefParametersAction).ToString(),
                "FourRefParametersActionOnOtherObject");
        }

        [TestMethod]
        public void FourRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.FourRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass}).ToString(),
                "FourRefParametersActionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void FourRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.FourRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass}).ToString(),
                "FourRefParametersActionWithParameters");
        }

        [TestMethod]
        public void FourRefParametersEditActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionOnTransient<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.FourRefParametersAction).ToString(),
                "FourRefParametersEditAction");
        }

        [TestMethod]
        public void FourRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.FourRefParametersFunction).ToString(),
                "FourRefParametersFunction");
        }

        [TestMethod]
        public void FourRefParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.FourRefParametersFunction).ToString(),
                "FourRefParametersFunction");
        }

        [TestMethod]
        public void FourRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.FourRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass}).ToString(),
                "FourRefParametersFunctionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void FourRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.FourRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass}).ToString(),
                "FourRefParametersFunctionWithParameters");
        }


        [TestMethod]
        public void FourValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction).ToString(),
                "FourValueParametersAction");
        }

        [TestMethod]
        public void FourValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction).ToString(),
                "FourValueParametersAction");
        }

        [TestMethod]
        public void FourValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, new {parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3}).ToString(),
                "FourValueParametersActionWithParameters");
        }

        [TestMethod]
        public void FourValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, new {parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3}).ToString(),
                "FourValueParametersActionWithParameters");
        }

        [TestMethod]
        public void FourValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction).ToString(),
                "FourValueParametersFunction");
        }

        [TestMethod]
        public void FourValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction).ToString(),
                "FourValueParametersFunction");
        }

        [TestMethod]
        public void FourValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, new {parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3}).ToString(),
                "FourValueParametersFunctionWithParameters");
        }

        [TestMethod]
        public void FourValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, new {parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3}).ToString(),
                "FourValueParametersFunctionWithParameters");
        }


        [TestMethod]
        public void IconNameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.IconName(tc).ToString(),
                "Default");
        }

        [TestMethod]
        public void IconNameTest() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().IconName().ToString(),
                "Default");
        }

        [TestMethod]
        public void InlinePropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestInline).ToString(),
                "TestInlineEdit");
        }


        [TestMethod]
        public void IntPropertyDefault() {
            INakedObject adapter = NakedObjectsFramework.GetNakedObject(new CustomHelperTestClass());


            SetupViewData(adapter.Object);
            mocks.ViewDataContainer.Object.ViewData["CustomHelperTestClass-TestIntDefault-Input"] = NakedObjectsFramework.Manager.CreateAdapter(0, null, null);


            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestIntDefault).ToString();

            CheckResults("TestIntDefault", s);
        }

        [TestMethod]
        public void IntPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestInt).ToString(),
                "TestIntEdit");
        }

        [TestMethod]
        public void IntPropertyEditDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.DisabledTestInt).ToString(),
                "DisabledTestIntEdit");
        }

        [TestMethod]
        public void IntPropertyEditHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.HiddenTestInt).ToString(),
                "");
        }

        [TestMethod]
        public void IntPropertyEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestInt).ToString(),
                "TestIntEdit");
        }

        [TestMethod]
        public void IntPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestInt").ToString(),
                "TestIntEdit");
        }

        [TestMethod]
        public void IntPropertyStringEditDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("DisabledTestInt").ToString(),
                "TestIntEditDisabled");
        }

        [TestMethod]
        public void IntPropertyStringEditHidden() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("HiddenTestInt").ToString(),
                "TestIntEditHidden");
        }

        [TestMethod]
        public void IntPropertyStringEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestInt").ToString(),
                "TestIntEdit");
        }

        [TestMethod]
        public void IntPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestInt").ToString(),
                "TestIntView");
        }

        [TestMethod]
        public void IntPropertyStringViewDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("DisabledTestInt").ToString(),
                "TestIntViewDisabled");
        }

        [TestMethod]
        public void IntPropertyStringViewHidden() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("HiddenTestInt").ToString(),
                "TestIntViewHidden");
        }

        [TestMethod]
        public void IntPropertyStringViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestInt").ToString(),
                "TestIntView");
        }

        [TestMethod]
        public void IntPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestInt).ToString(),
                "TestIntView");
        }

        [TestMethod]
        public void IntPropertyViewDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.DisabledTestInt).ToString(),
                "TestIntViewDisabled");
        }

        [TestMethod]
        public void IntPropertyViewHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.HiddenTestInt).ToString(),
                "");
        }

        [TestMethod]
        public void IntPropertyViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestInt).ToString(),
                "TestIntView");
        }

        [TestMethod]
        public void MenuWithCustomItems() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Menu(TestClass,
                new CustomMenuItem {
                    Controller = "ControllerName",
                    Action = "ActionName",
                    Name = "Action Label",
                    RouteValues = new {id = "anId"}
                },
                new CustomMenuItem {
                    Controller = "ControllerName",
                    Action = "ActionName1",
                    MemberOrder = 5,
                }
                ).ToString(),
                "MenuWithCustomItems");
        }

        [TestMethod]
        public void MenuWithOnlyCustomItems() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Menu("Test Menu", new CustomMenuItem {
                Controller = "ControllerName",
                Action = "ActionName",
                Name = "Action Label",
                RouteValues = new {id = "anId"}
            },
                new CustomMenuItem {
                    Controller = "ControllerName",
                    Action = "ActionName1",
                    MemberOrder = 5,
                }
                ).ToString(),
                "MenuWithOnlyCustomItems");
        }

        [TestMethod]
        public void NameFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc).ToString(),
                "Untitled Custom Helper Test Class");
        }

        [TestMethod]
        public void NameParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name("OneValueParameterAction", 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, "OneValueParameterAction", 0).ToString(),
                "Parm");
        }

        [TestMethod]
        public void NameProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name("TestInt").ToString(),
                "Test Int");
        }

        [TestMethod]
        public void NamePropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name(y => y.TestInt).ToString(),
                "Test Int");
        }

        [TestMethod]
        public void NamePropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, y => y.TestInt).ToString(),
                "Test Int");
        }

        [TestMethod]
        public void NamePropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, "TestInt").ToString(),
                "Test Int");
        }

        [TestMethod]
        public void NameTest() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name().ToString(),
                "Untitled Custom Helper Test Class");
        }

        [TestMethod]
        public void NameThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NameTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                "Parm1");
        }

        [TestMethod]
        public void NoParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction(y => y.NoParameterAction).ToString(),
                "NoParameterAction");
        }

        [TestMethod]
        public void NoParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction(tc, y => y.NoParameterAction).ToString(),
                "NoParameterAction");
        }

        [TestMethod]
        public void NoValueParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.NoValueParameterFunction).ToString(),
                "NoValueParameterFunction");
        }

        [TestMethod]
        public void NoValueParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int>(y => y.NoValueParameterFunction).ToString(),
                "NoValueParameterFunctionAsDialog");
        }

        [TestMethod]
        public void NoValueParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.NoValueParameterFunction).ToString(),
                "NoValueParameterFunction");
        }

        [TestMethod]
        public void NoValueParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int>(tc, y => y.NoValueParameterFunction).ToString(),
                "NoValueParameterFunctionAsDialog");
        }

        [TestMethod]
        public void OneCollectionParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction).ToString(),
                "OneCollectionParameterAction");
        }

        [TestMethod]
        public void OneCollectionParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction).ToString(),
                "OneCollectionParameterActionAsDialog");
        }

        [TestMethod]
        public void OneCollectionParameterActionWithParameters() {
            try {
                CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction, new {collection = new List<CustomHelperTestClass>()}).ToString(),
                    "OneCollectionParameterActionWithParameters");

                Assert.Fail("Expect not supported exception");
            }

            catch (NotSupportedException) {} // expected
        }

        [TestMethod]
        public void OneCollectionParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction).ToString(),
                "OneCollectionParameterFunction");
        }

        [TestMethod]
        public void OneCollectionParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction).ToString(),
                "OneCollectionParameterFunctionAsDialog");
        }

        [TestMethod]
        public void OneCollectionParameterFunctionWithParameters() {
            try {
                CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction, new {collection = new List<CustomHelperTestClass>()}).ToString(),
                    "OneCollectionParameterFunctionWithParameters");
                Assert.Fail("Expect not supported exception");
            }

            catch (NotSupportedException) {} // expected
        }

        [TestMethod]
        public void OneRefParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.OneRefParameterAction).ToString(),
                "OneRefParameterAction");
        }


        [TestMethod]
        public void OneRefParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.OneRefParameterAction).ToString(),
                "OneRefParameterActionAsDialog");
        }

        [TestMethod]
        public void OneRefParameterActionAsDialogDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.DisabledOneRefParameterAction).ToString(),
                "OneRefParameterActionAsDialogDisabled");
        }

        [TestMethod]
        public void OneRefParameterActionAsDialogHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.HiddenOneRefParameterAction).ToString(),
                "");
        }

        [TestMethod]
        public void OneRefParameterActionDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.DisabledOneRefParameterAction).ToString(),
                "OneRefParameterActionDisabled");
        }

        [TestMethod]
        public void OneRefParameterActionDisabledWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.DisabledOneRefParameterAction, new {parm = TestClass}).ToString(),
                "OneRefParameterActionDisabledWithParameter");
        }

        [TestMethod]
        public void OneRefParameterActionHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.HiddenOneRefParameterAction).ToString(),
                "");
        }

        [TestMethod]
        public void OneRefParameterActionHiddenWithParameter() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.HiddenOneRefParameterAction, new {parm = TestClass}).ToString(),
                "");
        }

        [TestMethod]
        public void OneRefParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.OneRefParameterAction).ToString(),
                "OneRefParameterActionOnOtherObject");
        }


        [TestMethod]
        public void OneRefParameterActionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.OneRefParameterAction).ToString(),
                "OneRefParameterActionOnOtherObjectAsDialog");
        }

        [TestMethod]
        public void OneRefParameterActionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.OneRefParameterAction, new {parm = TestClass}).ToString(),
                "OneRefParameterActionOnOtherObjectWithParameter");
        }

        [TestMethod]
        public void OneRefParameterActionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass>(y => y.OneRefParameterAction, new {parm = TestClass}).ToString(),
                "OneRefParameterActionWithParameter");
        }

        [TestMethod]
        public void OneRefParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass, int>(y => y.OneRefParameterFunction).ToString(),
                "OneRefParameterFunction");
        }

        [TestMethod]
        public void OneRefParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass, int>(y => y.OneRefParameterFunction).ToString(),
                "OneRefParameterFunctionAsDialog");
        }

        [TestMethod]
        public void OneRefParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction).ToString(),
                "OneRefParameterFunctionOnOtherObject");
        }

        [TestMethod]
        public void OneRefParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass,
                CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction).ToString(),
                "OneRefParameterFunctionOnOtherObjectAsDialog");
        }

        [TestMethod]
        public void OneRefParameterFunctionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction, new {parm = TestClass}).ToString(),
                "OneRefParameterFunctionOnOtherObjectWithParameter");
        }

        [TestMethod]
        public void OneRefParameterFunctionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass, int>(y => y.OneRefParameterFunction, new {parm = TestClass}).ToString(),
                "OneRefParameterFunctionWithParameter");
        }

        [TestMethod]
        public void OneRefParameterPopulatedActionAsDialog() {
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;


            SetupViewData(tc);

            mocks.ViewDataContainer.Object.ViewData["CustomHelperTestClass-OneRefParameterAction-Parm-Select"] = NakedObjectsFramework.Manager.CreateAdapter(tc, null, null);

            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, CustomHelperTestClass>(y => y.OneRefParameterAction).ToString();


            CheckResults("OneRefParameterPopulatedActionAsDialog", s);
        }

        [TestMethod]
        public void OneValueParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.OneValueParameterAction).ToString(),
                "OneValueParameterAction");
        }

        [TestMethod]
        public void OneValueParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int>(y => y.OneValueParameterAction).ToString(),
                "OneValueParameterActionasDialog");
        }

        [TestMethod]
        public void OneValueParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction).ToString(),
                "OneValueParameterActionOnOtherObject");
        }


        [TestMethod]
        public void OneValueParameterActionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction).ToString(),
                "OneValueParameterActionOnOtherObjectAsDialog");
        }

        [TestMethod]
        public void OneValueParameterActionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, new {parm = 1}).ToString(),
                "OneValueParameterActionOnOtherObjectWithParameter");
        }

        [TestMethod]
        public void OneValueParameterActionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.OneValueParameterAction, new {parm = 1}).ToString(),
                "OneValueParameterActionWithParameter");
        }

        [TestMethod]
        public void OneValueParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction).ToString(),
                "OneValueParameterFunction");
        }

        [TestMethod]
        public void OneValueParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction).ToString(),
                "OneValueParameterFunctionAsDialog");
        }

        [TestMethod]
        public void OneValueParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction).ToString(),
                "OneValueParameterFunctionOnOtherObject");
        }

        [TestMethod]
        public void OneValueParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction).ToString(),
                "OneValueParameterFunctionAsDialog");
        }

        [TestMethod]
        public void OneValueParameterFunctionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, new {parm = 1}).ToString(),
                "OneValueParameterFunctionOnOtherObjectWithParameter");
        }

        [TestMethod]
        public void OneValueParameterFunctionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, new {parm = 1}).ToString(),
                "OneValueParameterFunctionWithParameter");
        }

        [TestMethod]
        public void PropertiesListOnlyCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertiesListOnlyCollections(tc).Aggregate("", (s, t) => s + t.ToString()), "PropertiesListOnlyCollections");
        }

        //out


        [TestMethod]
        public void PropertyListEditExclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWithout("TestCollectionOne", "TestInt").ToString(),
                "PropertyListEditWithExclusions");
        }

        [TestMethod]
        public void PropertyListEditExclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "PropertyListEditExclusionsGeneric");
        }

        [TestMethod]
        public void PropertyListEditExclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "PropertyListEditExclusionsGenericOtherObj");
        }

        [TestMethod]
        public void PropertyListEditExclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                "PropertyListEditExclusionsOtherObj");
        }

        [TestMethod, Ignore]
        public void PropertyListEditInclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWith("TestInt", "TestCollectionOne").ToString(),
                "PropertyListEditInclusions");
        }

        [TestMethod, Ignore]
        public void PropertyListEditInclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "PropertyListEditInclusionsGeneric");
        }

        [TestMethod, Ignore]
        public void PropertyListEditInclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "PropertyListEditInclusionsGenericOtherObj");
        }

        [TestMethod, Ignore]
        public void PropertyListEditInclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWith(tc, "TestInt", "TestCollectionOne").ToString(),
                "PropertyListEditInclusionsOtherObj");
        }

        [TestMethod]
        public void PropertyListEditList() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit("TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListEditList");
        }

        [TestMethod]
        public void PropertyListEditListDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListEditListDict");
        }

        [TestMethod]
        public void PropertyListEditListDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListEditListDictGeneric");
        }

        [TestMethod]
        public void PropertyListEditListGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListEditListGeneric");
        }

        [TestMethod]
        public void PropertyListEditListOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListEditListOtherObj");
        }

        [TestMethod]
        public void PropertyListEditListOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListEditListOtherObjDict");
        }

        [TestMethod]
        public void PropertyListEditListOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListEditListOtherObjDictGeneric");
        }

        [TestMethod]
        public void PropertyListEditListOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListEditListOtherObjGeneric");
        }

        [TestMethod]
        public void PropertyListEditTable() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit("TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListEditTable");
        }

        [TestMethod]
        public void PropertyListEditTableDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListEditTableDict");
        }

        [TestMethod]
        public void PropertyListEditTableDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListEditTableDictGeneric");
        }

        [TestMethod]
        public void PropertyListEditTableGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListEditTableGeneric");
        }

        [TestMethod]
        public void PropertyListEditTableOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListEditTableOtherObj");
        }

        [TestMethod]
        public void PropertyListEditTableOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListEditTableOtherObjDict");
        }

        [TestMethod]
        public void PropertyListEditTableOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListEditTableOtherObjDictGeneric");
        }

        [TestMethod]
        public void PropertyListEditTableOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListEditTableOtherObjGeneric");
        }

        [TestMethod]
        public void PropertyListExclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithout("TestCollectionOne", "TestInt").ToString(),
                "PropertyListExclusions");
        }

        [TestMethod]
        public void PropertyListExclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "PropertyListExclusionsGeneric");
        }

        [TestMethod]
        public void PropertyListExclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                "PropertyListExclusionsGenericOtherObj");
        }

        [TestMethod]
        public void PropertyListExclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                "PropertyListWithExclusions");
        }

        [TestMethod]
        public void PropertyListInclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWith("TestInt", "TestCollectionOne").ToString(),
                "PropertyListInclusions");
        }

        [TestMethod]
        public void PropertyListInclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "PropertyListInclusionsGeneric");
        }

        [TestMethod]
        public void PropertyListInclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                "PropertyListInclusionsGenericOtherObj");
        }

        [TestMethod]
        public void PropertyListInclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWith(tc, "TestInt", "TestCollectionOne").ToString(),
                "PropertyListWithInclusions");
        }

        [TestMethod]
        public void PropertyListList() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList("TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListList");
        }

        [TestMethod]
        public void PropertyListListDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListListDict");
        }

        [TestMethod]
        public void PropertyListListDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListListDictGeneric");
        }

        [TestMethod]
        public void PropertyListListGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListListGeneric");
        }

        [TestMethod]
        public void PropertyListListOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListListOtherObj");
        }

        [TestMethod]
        public void PropertyListListOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListListOtherObjDict");
        }

        [TestMethod]
        public void PropertyListListOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                "PropertyListListOtherObjDictGeneric");
        }

        [TestMethod]
        public void PropertyListListOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                "PropertyListListOtherObjGeneric");
        }

        [TestMethod]
        public void PropertyListOnlyCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc).ToString(), "PropertyListOnlyCollections");
        }

        [TestMethod]
        public void PropertyListOnlyCollectionsFormatList() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc, PropertyExtensions.CollectionFormat.List).ToString(), "PropertyListOnlyCollectionsFormatList");
        }

        [TestMethod]
        public void PropertyListOnlyCollectionsFormatTable() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc, PropertyExtensions.CollectionFormat.Table).ToString(), "PropertyListOnlyCollectionsFormatTable");
        }

        [TestMethod]
        public void PropertyListTable() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList("TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListTable");
        }

        [TestMethod]
        public void PropertyListTableDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListTableDict");
        }

        [TestMethod]
        public void PropertyListTableDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListTableDictGeneric");
        }

        [TestMethod]
        public void PropertyListTableGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListTableGeneric");
        }

        [TestMethod]
        public void PropertyListTableOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListTableOtherObj");
        }

        [TestMethod]
        public void PropertyListTableOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListTableOtherObjDict");
        }

        [TestMethod]
        public void PropertyListTableOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                "PropertyListTableOtherObjDictGeneric");
        }

        [TestMethod]
        public void PropertyListTableOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                "PropertyListTableOtherObjGeneric");
        }

        [TestMethod]
        public void PropertyListWithoutCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithoutCollections(tc).ToString(), "PropertyListWithoutCollections");
        }

        [TestMethod]
        public void RefPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestRef).ToString(),
                "RefPropertyEdit");
        }

        [TestMethod]
        public void RefPropertyEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestRef).ToString(),
                "TestRefEditOther");
        }

        [TestMethod]
        public void RefPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestRef").ToString(),
                "RefPropertyStringEdit");
        }

        [TestMethod]
        public void RefPropertyStringEditExistingValue() {
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            SetupViewData(tc);

            var id = NakedObjectsFramework.GetObjectId(tc);
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("CustomHelperTestClass-TestRef-Select", new ValueProviderResult(id, null, null));

            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestRef").ToString();

            CheckResults("RefPropertyStringEditExistingValue", s);
        }


        [TestMethod]
        public void RefPropertyStringEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestRef").ToString(),
                "TestRefEdit");
        }

        [TestMethod]
        public void RefPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestRef").ToString(),
                "TestRefView");
        }

        [TestMethod]
        public void RefPropertyStringViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestRef").ToString(),
                "TestRefView");
        }

        [TestMethod]
        public void RefPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestRef).ToString(),
                "TestRefView");
        }

        [TestMethod]
        public void RefPropertyViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestRef).ToString(),
                "TestRefView");
        }

        [TestMethod] //TODO: When transition to Menus complete, replace with test of adding CustomMenuItems to a menu
        public void ServiceWithCustomItems() {
            object ts = GetTestService("Custom Helper Test Classes").NakedObject.Object;

            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Service(ts,
                new CustomMenuItem {
                    Controller = "ControllerName",
                    Action = "ActionName",
                    Name = "Action Label",
                    RouteValues = new {id = "anId"}
                },
                new CustomMenuItem {
                    Controller = "ControllerName",
                    Action = "ActionName1",
                    MemberOrder = 5,
                }
                ).ToString(),
                "ServiceWithCustomItems");
        }

        [TestMethod] //TODO: When conversion to Menus complete, replace this test with call to MenuExtensions#MainMenu.
        public void SingleServiceMenu() {
            SetupViewData(new object());
            object ts = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            string s = mocks.HtmlHelper.ServiceMenu(ts).ToString();


            CheckResults("SingleServiceMenu", s);
        }

        [TestMethod]
        public void StringPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestString).ToString(),
                "TestStringEdit");
        }

        [TestMethod]
        public void StringPropertyOtherObjEdit() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestString).ToString(),
                "TestStringEdit");
        }

        [TestMethod]
        public void StringPropertyOtherObjView() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestString).ToString(),
                "TestStringView");
        }

        [TestMethod]
        public void StringPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestString").ToString(),
                "TestStringEdit");
        }

        [TestMethod]
        public void StringPropertyStringOtherObjEdit() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestString").ToString(),
                "TestStringEdit");
        }

        [TestMethod]
        public void StringPropertyStringOtherObjView() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestString").ToString(),
                "TestStringView");
        }

        [TestMethod]
        public void StringPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestString").ToString(),
                "TestStringView");
        }

        [TestMethod]
        public void StringPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestString).ToString(),
                "TestStringView");
        }

        [TestMethod]
        public void ThreeRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.ThreeRefParametersAction).ToString(),
                "ThreeRefParametersAction");
        }

        [TestMethod]
        public void ThreeRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.ThreeRefParametersAction).ToString(),
                "ThreeRefParametersAction");
        }

        [TestMethod]
        public void ThreeRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.ThreeRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass}).ToString(),
                "ThreeRefParametersActionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void ThreeRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.ThreeRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass}).ToString(),
                "ThreeRefParametersActionWithParameters");
        }

        [TestMethod]
        public void ThreeRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.ThreeRefParametersFunction).ToString(),
                "ThreeRefParametersFunction");
        }

        [TestMethod]
        public void ThreeRefParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.ThreeRefParametersFunction).ToString(),
                "ThreeRefParametersFunction");
        }

        [TestMethod]
        public void ThreeRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.ThreeRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass}).ToString(),
                "ThreeRefParametersFunctionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void ThreeRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.ThreeRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass}).ToString(),
                "ThreeRefParametersFunctionWithParameters");
        }

        [TestMethod]
        public void ThreeValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction).ToString(),
                "ThreeValueParametersAction");
        }

        [TestMethod]
        public void ThreeValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction).ToString(),
                "ThreeValueParametersAction");
        }

        [TestMethod]
        public void ThreeValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, new {parm1 = 1, parm2 = 2, parm3 = 3}).ToString(),
                "ThreeValueParametersActionWithParameters");
        }

        [TestMethod]
        public void ThreeValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, new {parm1 = 1, parm2 = 2, parm3 = 3}).ToString(),
                "ThreeValueParametersActionWithParameters");
        }

        [TestMethod]
        public void ThreeValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction).ToString(),
                "ThreeValueParametersFunction");
        }

        [TestMethod]
        public void ThreeValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction).ToString(),
                "ThreeValueParametersFunction");
        }

        [TestMethod]
        public void ThreeValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, new {parm1 = 1, parm2 = 2, parm3 = 3}).ToString(),
                "ThreeValueParametersFunctionWithParameters");
        }

        [TestMethod]
        public void ThreeValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, new {parm1 = 1, parm2 = 2, parm3 = 3}).ToString(),
                "ThreeValueParametersFunctionWithParameters");
        }

        [TestMethod]
        public void TwoRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.TwoRefParametersAction).ToString(),
                "TwoRefParametersAction");
        }

        [TestMethod]
        public void TwoRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.TwoRefParametersAction).ToString(),
                "TwoRefParametersAction");
        }

        [TestMethod]
        public void TwoRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(tc, y => y.TwoRefParametersAction, new {parm1 = TestClass, parm2 = TestClass}).ToString(),
                "TwoRefParametersActionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void TwoRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass>(y => y.TwoRefParametersAction, new {parm1 = TestClass, parm2 = TestClass}).ToString(),
                "TwoRefParametersActionWithParameters");
        }

        [TestMethod]
        public void TwoRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.TwoRefParametersFunction).ToString(),
                "TwoRefParametersFunction");
        }

        [TestMethod]
        public void TwoRefParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.TwoRefParametersFunction).ToString(),
                "TwoRefParametersFunction");
        }

        [TestMethod]
        public void TwoRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(tc, y => y.TwoRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass}).ToString(),
                "TwoRefParametersFunctionOnOtherObjectWithParameters");
        }

        [TestMethod]
        public void TwoRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                CustomHelperTestClass,
                CustomHelperTestClass,
                int>(y => y.TwoRefParametersFunction, new {parm1 = TestClass, parm2 = TestClass}).ToString(),
                "TwoRefParametersFunctionWithParameters");
        }

        [TestMethod]
        public void TwoValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction).ToString(),
                "TwoValueParametersAction");
        }

        [TestMethod]
        public void TwoValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction).ToString(),
                "TwoValueParametersAction");
        }

        [TestMethod]
        public void TwoValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, new {parm1 = 1, parm2 = 2}).ToString(),
                "TwoValueParametersActionWithParameters");
        }

        [TestMethod]
        public void TwoValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, new {parm1 = 1, parm2 = 2}).ToString(),
                "TwoValueParametersActionWithParameters");
        }

        [TestMethod]
        public void TwoValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction).ToString(),
                "TwoValueParametersFunction");
        }

        [TestMethod]
        public void TwoValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction).ToString(),
                "TwoValueParametersFunction");
        }

        //

        [TestMethod]
        public void TwoValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, new {parm1 = 1, parm2 = 2}).ToString(),
                "TwoValueParametersFunctionWithParameters");
        }

        [TestMethod]
        public void TwoValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, new {parm1 = 1, parm2 = 2}).ToString(),
                "TwoValueParametersFunctionWithParameters");
        }


        // object

        [TestMethod]
        public void TypeName() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName().ToString(),
                "CustomHelperTestClass");
        }

        [TestMethod]
        public void TypeNameFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc).ToString(),
                "CustomHelperTestClass");
        }

        [TestMethod]
        public void TypeNameParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName("OneValueParameterAction", 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, "OneValueParameterAction", 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName("TestInt").ToString(),
                "Int32");
        }

        // property

        [TestMethod]
        public void TypeNamePropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName(y => y.TestInt).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNamePropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, y => y.TestInt).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNamePropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, "TestInt").ToString(),
                "Int32");
        }

        // parm 


        [TestMethod]
        public void TypeNameThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                "Int32");
        }

        [TestMethod]
        public void TypeNameTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                "Int32");
        }
    }
}