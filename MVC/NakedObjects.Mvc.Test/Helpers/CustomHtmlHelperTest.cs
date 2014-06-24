// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class CustomHtmlHelperTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
            SetUser("sven");
        }

        [TearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller  ContributedActions {
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions() }); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        protected override IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller {SimpleOidGeneratorStart = 100}; }
        }


        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();

        private static string GetTestData(string name) {
            string file = Path.Combine(@"..\..\Custom Html reference files", name) + ".htm";
            return File.ReadAllText(file);
        }

        // for testcreation 

        private static void WriteTestData(string name, string data) {
            string file = Path.Combine(@"..\..\Custom Html reference files", name) + ".htm";
            File.WriteAllText(file, data);
        }

        private static void CheckResults(string resultsFile, string s) {
            //string actionView = GetTestData(resultsFile).StripWhiteSpace();
            //Assert.AreEqual(actionView, s.StripWhiteSpace());
            WriteTestData(resultsFile, s);
        }


        private void CustomHelperTest(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            CheckResults(toCompare, s);
        }


        private void CustomHelperTestOtherObj(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = new object(); // placeholder
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            CheckResults(toCompare, s);
        }


        private void DescriptionCustomHelperTestCompareDirect(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            var tc = (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void DescriptionCustomHelperTestCompareDirectOtherObj(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = new object();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void CustomHelperTestCollection(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            var collection = new List<CustomHelperTestClass> {tc};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            mocks.ViewDataContainer.Object.ViewData.Model = collection;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            CheckResults(toCompare, s);
        }

        private void CustomHelperTestCompareDirect(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            var tc = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }

        private void CustomHelperTestCompareDirectOtherObj(Func<ContextMocks, string> func, string toCompare) {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = new object();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = func(mocks);
            Assert.AreEqual(toCompare, s);
        }


        private CustomHelperTestClass TestClass {
            get { return (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        private DescribedCustomHelperTestClass DescribedTestClass {
            get { return (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        [Test]
        public void CollectionExclusions() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout("TestCollectionOne", "TestInt").ToString(),
                                       "CollectionWithExclusions");
        }

        [Test]
        public void CollectionExclusionsGeneric() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                                       "CollectionWithExclusions");
        }

        [Test]
        public void CollectionExclusionsGenericOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = PersistorUtils.CreateAdapter(tc);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                                       "CollectionWithExclusions");
        }

        [Test]
        public void CollectionExclusionsOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = PersistorUtils.CreateAdapter(tc);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                                       "CollectionWithExclusions");
        }

        [Test]
        public void CollectionInclusions() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith("TestInt", "TestCollectionOne").ToString(),
                                       "CollectionWithInclusions");
        }

        [Test]
        public void CollectionInclusionsGeneric() {
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                                       "CollectionWithInclusions");
        }

        [Test]
        public void CollectionInclusionsGenericOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = PersistorUtils.CreateAdapter(tc);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                                       "CollectionWithInclusions");
        }

        [Test]
        public void CollectionInclusionsOtherObj() {
            var tc = new List<CustomHelperTestClass> {TestClass};
            INakedObject adapter = PersistorUtils.CreateAdapter(tc);
            adapter.SetATransientOid(new DummyOid());
            CustomHelperTestCollection(x => x.GetHtmlHelper<IEnumerable<CustomHelperTestClass>>().CollectionTableWith(tc, "TestInt", "TestCollectionOne").ToString(),
                                       "CollectionWithInclusions");
        }

        [Test]
        public void ContentsFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents("OneValueParameterAction", 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, "OneValueParameterAction", 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents("TestInt").ToString(),
                                          "0");
        }

        [Test]
        public void ContentsPropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents(y => y.TestInt).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsPropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, y => y.TestInt).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsPropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents(tc, "TestInt").ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Contents<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                                          "0");
        }

        [Test]
        public void ContentsTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void ContentsTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Contents<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                                                  "0");
        }

        [Test]
        public void CustomHelperTestAsDialog() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.ObjectActionAsDialog("FourValueParametersFunction").ToString();
            CheckResults("CustomHelperTestAsDialog", s);
        }

        [Test]
        public void CustomHelperTestAsDialogOnOtherObject() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = new object();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.ObjectActionAsDialog(tc, "FourValueParametersFunction").ToString();
            CheckResults("CustomHelperTestAsDialog", s);
        }

        [Test]
        public void CustomHelperTestStringId() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.HtmlHelper.ObjectAction("NoParameterAction").ToString();
        

            CheckResults("NoParameterAction", s);
        }

        [Test]
        public void CustomHelperTestStringIdOnOtherObject() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = new object(); // placeholder
            string s = mocks.HtmlHelper.ObjectAction(tc, "NoParameterAction").ToString();
          

            CheckResults("NoParameterAction", s);
        }

        [Test]
        public void CustomHelperTestStringIdHidden() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.HtmlHelper.ObjectAction("HiddenAction").ToString();
            Assert.AreEqual("", s);
        }

        [Test]
        public void CustomHelperTestStringIdOnOtherObjectHidden() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = new object(); // placeholder
            string s = mocks.HtmlHelper.ObjectAction(tc, "HiddenAction").ToString();
            Assert.AreEqual("", s);
        }

        [Test]
        public void CustomHelperTestStringIdDisabled() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.HtmlHelper.ObjectAction("DisabledAction").ToString();
         

            CheckResults("DisabledAction", s);
        }

        [Test]
        public void CustomHelperTestStringIdOnOtherObjectDisabled() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = new object(); // placeholder
            string s = mocks.HtmlHelper.ObjectAction(tc, "DisabledAction").ToString();
           

            CheckResults("DisabledAction", s);
        }


        [Test]
        public void CustomHelperTestStringIdOnOtherObjectEdit() {
            var mocks = new ContextMocks(controller);
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData.Model = new object();
            string s = mocks.HtmlHelper.ObjectActionOnTransient(tc, "NoParameterAction").ToString();
         

            CheckResults("NoParameterEditAction", s);
        }

        [Test]
        public void DescriptionFourParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionFourParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionFourParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionFourParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionOneParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionOneParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionOneParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionOneParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description("OneValueParameterAction", 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, "OneValueParameterAction", 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionProperty() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description("TestInt").ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionPropertyGeneric() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description(y => y.TestInt).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionPropertyGenericOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, y => y.TestInt).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionPropertyOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description(tc, "TestInt").ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionTest() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description().ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionThreeParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionThreeParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionThreeParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionThreeParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionTwoParm() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionTwoParmFunc() {
            DescriptionCustomHelperTestCompareDirect(x => x.GetHtmlHelper<DescribedCustomHelperTestClass>().Description<DescribedCustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                                                     "aDescription");
        }

        [Test]
        public void DescriptionTwoParmOtherObj() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void DescriptionTwoParmOtherObjFunc() {
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            DescriptionCustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Description<DescribedCustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                                                             "aDescription");
        }

        [Test]
        public void FourRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.FourRefParametersAction).ToString(),
                             "FourRefParametersAction");
        }

        [Test]
        public void FourRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.FourRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass}).ToString(),
                             "FourRefParametersActionWithParameters");
        }


        [Test]
        public void FourRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.FourRefParametersAction).ToString(),
                                     "FourRefParametersAction");
        }

        [Test]
        public void FourRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.FourRefParametersAction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass }).ToString(),
                                     "FourRefParametersActionWithParameters");
        }

        [Test]
        public void FourRefParametersEditActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionOnTransient<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.FourRefParametersAction).ToString(),
                                     "FourRefParametersEditAction");
        }

        [Test]
        public void FourRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.FourRefParametersFunction).ToString(),
                             "FourRefParametersFunction");
        }

        [Test]
        public void FourRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.FourRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass }).ToString(),
                             "FourRefParametersFunctionWithParameters");
        }

        [Test]
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

        [Test]
        public void FourRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              int>(tc, y => y.FourRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass, parm4 = TestClass }).ToString(),
                                     "FourRefParametersFunctionWithParameters");
        }


        [Test]
        public void FourValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction).ToString(),
                             "FourValueParametersAction");
        }

        [Test]
        public void FourValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction).ToString(),
                                     "FourValueParametersAction");
        }

        [Test]
        public void FourValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction).ToString(),
                             "FourValueParametersFunction");
        }

        [Test]
        public void FourValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction).ToString(),
                                     "FourValueParametersFunction");
        }

        [Test]
        public void FourValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, new { parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3 }).ToString(),
                             "FourValueParametersActionWithParameters");
        }

        [Test]
        public void FourValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, new { parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3 }).ToString(),
                                     "FourValueParametersActionWithParameters");
        }

        [Test]
        public void FourValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, new { parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3 }).ToString(),
                             "FourValueParametersFunctionWithParameters");
        }

        [Test]
        public void FourValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, new { parm1 = 0, parm2 = 1, parm3 = 2, parm4 = 3 }).ToString(),
                                     "FourValueParametersFunctionWithParameters");
        }







        [Test]
        public void IconNameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.IconName(tc).ToString(),
                                                  "Default");
        }

        [Test]
        public void IconNameTest() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().IconName().ToString(),
                                          "Default");
        }

        [Test]
        public void IntPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestInt).ToString(),
                             "TestIntEdit");
        }

        [Test]
        public void InlinePropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestInline).ToString(),
                             "TestInlineEdit");
        }


        [Test]
        public void IntPropertyDefault() {
          
            var mocks = new ContextMocks(controller);
            INakedObject adapter = FrameworkHelper.GetNakedObject(new CustomHelperTestClass());
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            mocks.ViewDataContainer.Object.ViewData["CustomHelperTestClass-TestIntDefault-Input"] = PersistorUtils.CreateAdapter(0);

            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestIntDefault).ToString();

            CheckResults("TestIntDefault", s);
        }

        [Test]
        public void DateTimePropertyEdit() {

            var mocks = new ContextMocks(controller);
            INakedObject adapter = FrameworkHelper.GetNakedObject(new NotPersistedTestClass());
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().ObjectPropertyEdit(y => y.TestDateTime).ToString();
       

            CheckResults("TestDateTime", s);
        }

        [Test]
        public void IntPropertyEditHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.HiddenTestInt).ToString(),
                             "");
        }

        [Test]
        public void IntPropertyEditDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.DisabledTestInt).ToString(),
                             "DisabledTestIntEdit");
        }

        [Test]
        public void IntPropertyEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestInt).ToString(),
                                     "TestIntEdit");
        }

        [Test]
        public void IntPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestInt").ToString(),
                             "TestIntEdit");
        }

        [Test]
        public void IntPropertyStringEditHidden() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("HiddenTestInt").ToString(),
                             "TestIntEditHidden");
        }

        [Test]
        public void IntPropertyStringEditDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("DisabledTestInt").ToString(),
                             "TestIntEditDisabled");
        }

        [Test]
        public void IntPropertyStringEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestInt").ToString(),
                                     "TestIntEdit");
        }

        [Test]
        public void IntPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestInt").ToString(),
                             "TestIntView");
        }

        [Test]
        public void IntPropertyStringViewHidden() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("HiddenTestInt").ToString(),
                             "TestIntViewHidden");
        }

        [Test]
        public void IntPropertyStringViewDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("DisabledTestInt").ToString(),
                             "TestIntViewDisabled");
        }

        [Test]
        public void IntPropertyStringViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestInt").ToString(),
                                     "TestIntView");
        }

        [Test]
        public void IntPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestInt).ToString(),
                             "TestIntView");
        }

        [Test]
        public void IntPropertyViewHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.HiddenTestInt).ToString(),
                             "");
        }

        [Test]
        public void IntPropertyViewDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.DisabledTestInt).ToString(),
                             "TestIntViewDisabled");
        }

        [Test]
        public void IntPropertyViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestInt).ToString(),
                                     "TestIntView");
        }

        [Test]
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

        [Test]
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

        [Test]
        public void NameFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NameFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NameOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                                          "Parm");
        }

        [Test]
        public void NameOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                                          "Parm");
        }

        [Test]
        public void NameOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                                                  "Parm");
        }

        [Test]
        public void NameOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                                                  "Parm");
        }

        [Test]
        public void NameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc).ToString(),
                                                  "Untitled Custom Helper Test Class");
        }

        [Test]
        public void NameParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name("OneValueParameterAction", 0).ToString(),
                                          "Parm");
        }

        [Test]
        public void NameParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, "OneValueParameterAction", 0).ToString(),
                                                  "Parm");
        }

        [Test]
        public void NameProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name("TestInt").ToString(),
                                          "Test Int");
        }

        [Test]
        public void NamePropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name(y => y.TestInt).ToString(),
                                          "Test Int");
        }

        [Test]
        public void NamePropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, y => y.TestInt).ToString(),
                                                  "Test Int");
        }

        [Test]
        public void NamePropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name(tc, "TestInt").ToString(),
                                                  "Test Int");
        }

        [Test]
        public void NameTest() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name().ToString(),
                                          "Untitled Custom Helper Test Class");
        }

        [Test]
        public void NameThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NameThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NameTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().Name<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                                          "Parm1");
        }

        [Test]
        public void NameTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NameTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.Name<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                                                  "Parm1");
        }

        [Test]
        public void NoParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction(y => y.NoParameterAction).ToString(),
                             "NoParameterAction");
        }

        [Test]
        public void NoParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction(tc, y => y.NoParameterAction).ToString(),
                                     "NoParameterAction");
        }

        [Test]
        public void NoValueParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.NoValueParameterFunction).ToString(),
                             "NoValueParameterFunction");
        }

        [Test]
        public void NoValueParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.NoValueParameterFunction).ToString(),
                                     "NoValueParameterFunction");
        }

        [Test]
        public void OneCollectionParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction).ToString(),
                             "OneCollectionParameterAction");
        }

        [Test]
        public void OneCollectionParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction).ToString(),
                             "OneCollectionParameterFunction");
        }

        [Test]
        public void OneCollectionParameterActionWithParameters() {
            try {
                CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction, new {collection = new List<CustomHelperTestClass>()}).ToString(),
                                 "OneCollectionParameterActionWithParameters");

                Assert.Fail("Expect not supported exception");
            }

            catch (NotSupportedException) {} // expected
        }

        [Test]
        public void OneCollectionParameterFunctionWithParameters() {
            try {
                CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction, new {collection = new List<CustomHelperTestClass>()}).ToString(),
                                 "OneCollectionParameterFunctionWithParameters");
                Assert.Fail("Expect not supported exception");
            }

            catch (NotSupportedException) {} // expected
        }

        [Test]
        public void OneRefParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.OneRefParameterAction).ToString(),
                             "OneRefParameterAction");
        }


        [Test]
        public void OneRefParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.OneRefParameterAction).ToString(),
                                     "OneRefParameterAction");
        }

        [Test]
        public void OneRefParameterActionHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.HiddenOneRefParameterAction).ToString(),
                             "");
        }

        [Test]
        public void OneRefParameterActionDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.DisabledOneRefParameterAction).ToString(),
                             "OneRefParameterActionDisabled");
        }


        [Test]
        public void OneRefParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass, int>(y => y.OneRefParameterFunction).ToString(),
                             "OneRefParameterFunction");
        }

        [Test]
        public void OneRefParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction).ToString(),
                                     "OneRefParameterFunction");
        }

        [Test]
        public void OneRefParameterActionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.OneRefParameterAction, new {parm = TestClass}).ToString(),
                             "OneRefParameterActionWithParameter");
        }


        [Test]
        public void OneRefParameterActionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.OneRefParameterAction, new { parm = TestClass }).ToString(),
                                     "OneRefParameterActionWithParameter");
        }

        [Test]
        public void OneRefParameterActionHiddenWithParameter() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.HiddenOneRefParameterAction, new { parm = TestClass }).ToString(),
                             "");
        }

        [Test]
        public void OneRefParameterActionDisabledWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.DisabledOneRefParameterAction, new { parm = TestClass }).ToString(),
                             "OneRefParameterActionDisabled");
        }


        [Test]
        public void OneRefParameterFunctionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass, int>(y => y.OneRefParameterFunction, new { parm = TestClass }).ToString(),
                             "OneRefParameterFunctionWithParameter");
        }

        [Test]
        public void OneRefParameterFunctionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction, new { parm = TestClass }).ToString(),
                                     "OneRefParameterFunctionWithParameter");
        }

        [Test]
        public void OneValueParameterAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.OneValueParameterAction).ToString(),
                             "OneValueParameterAction");
        }


        [Test]
        public void OneValueParameterActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction).ToString(),
                                     "OneValueParameterAction");
        }

        [Test]
        public void OneValueParameterFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction).ToString(),
                             "OneValueParameterFunction");
        }

        [Test]
        public void OneValueParameterFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction).ToString(),
                                     "OneValueParameterFunction");
        }

        //

        [Test]
        public void OneValueParameterActionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int>(y => y.OneValueParameterAction, new { parm = 1 }).ToString(),
                             "OneValueParameterActionWithParameter");
        }


        [Test]
        public void OneValueParameterActionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, new { parm = 1 }).ToString(),
                                     "OneValueParameterActionWithParameter");
        }

        [Test]
        public void OneValueParameterFunctionWithParameter() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, new { parm = 1 }).ToString(),
                             "OneValueParameterFunctionWithParameter");
        }

        [Test]
        public void OneValueParameterFunctionOnOtherObjectWithParameter() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, new { parm = 1 }).ToString(),
                                     "OneValueParameterFunctionWithParameter");
        }



        //in

        [Test]
        public void NoValueParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int>(y => y.NoValueParameterFunction).ToString(),
                             "NoValueParameterFunctionAsDialog");
        }

        [Test]
        public void NoValueParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int>(tc, y => y.NoValueParameterFunction).ToString(),
                                     "NoValueParameterFunctionAsDialog");
        }

        [Test]
        public void OneCollectionParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, List<CustomHelperTestClass>>(y => y.OneCollectionParameterAction).ToString(),
                             "OneCollectionParameterActionAsDialog");
        }

        [Test]
        public void OneCollectionParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, List<CustomHelperTestClass>, int>(y => y.OneCollectionParameterFunction).ToString(),
                             "OneCollectionParameterFunctionAsDialog");
        }

        [Test]
        public void OneRefParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.OneRefParameterAction).ToString(),
                             "OneRefParameterActionAsDialog");
        }

        [Test]
        public void OneRefParameterPopulatedActionAsDialog() {

            var mocks = new ContextMocks(controller);
            var tc = (CustomHelperTestClass)GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            mocks.ViewDataContainer.Object.ViewData["CustomHelperTestClass-OneRefParameterAction-Parm-Select"] = PersistorUtils.CreateAdapter(tc);

            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, CustomHelperTestClass>(y => y.OneRefParameterAction).ToString();
   

            CheckResults("OneRefParameterPopulatedActionAsDialog", s);
        }


        [Test]
        public void OneRefParameterActionAsDialogHidden() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.HiddenOneRefParameterAction).ToString(),
                             "");
        }

        [Test]
        public void OneRefParameterActionAsDialogDisabled() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.DisabledOneRefParameterAction).ToString(),
                             "OneRefParameterActionAsDialogDisabled");
        }


        [Test]
        public void OneRefParameterActionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.OneRefParameterAction).ToString(),
                                     "OneRefParameterActionAsDialog");
        }

        [Test]
        public void OneRefParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass,
                                      CustomHelperTestClass, int>(y => y.OneRefParameterFunction).ToString(),
                             "OneRefParameterFunctionAsDialog");
        }

        [Test]
        public void OneRefParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass,
                                              CustomHelperTestClass, int>(tc, y => y.OneRefParameterFunction).ToString(),
                                     "OneRefParameterFunctionAsDialog");
        }

        [Test]
        public void OneValueParameterActionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int>(y => y.OneValueParameterAction).ToString(),
                             "OneValueParameterActionasDialog");
        }


        [Test]
        public void OneValueParameterActionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction).ToString(),
                                     "OneValueParameterActionAsDialog");
        }

        [Test]
        public void OneValueParameterFunctionAsDialog() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectActionAsDialog<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction).ToString(),
                             "OneValueParameterFunctionAsDialog");
        }

        [Test]
        public void OneValueParameterFunctionOnOtherObjectAsDialog() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectActionAsDialog<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction).ToString(),
                                     "OneValueParameterFunctionAsDialog");
        }

        //out



        [Test]
        public void PropertyListEditExclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWithout("TestCollectionOne", "TestInt").ToString(),
                             "PropertyListEditWithExclusions");
        }

        [Test]
        public void PropertyListEditExclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                             "PropertyListEditWithExclusions");
        }

        [Test]
        public void PropertyListEditExclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                                     "PropertyListEditWithExclusions");
        }

        [Test]
        public void PropertyListEditExclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                                     "PropertyListEditWithExclusions");
        }

        [Test]
        public void PropertyListEditInclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWith("TestInt", "TestCollectionOne").ToString(),
                             "PropertyListEditWithInclusions");
        }

        [Test]
        public void PropertyListEditInclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEditWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                             "PropertyListEditWithInclusions");
        }

        [Test]
        public void PropertyListEditInclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                                     "PropertyListEditWithInclusions");
        }

        [Test]
        public void PropertyListEditInclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEditWith(tc, "TestInt", "TestCollectionOne").ToString(),
                                     "PropertyListEditWithInclusions");
        }

        [Test]
        public void PropertyListEditList() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit("TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                             "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                             "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                             "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                             "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                                     "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                                     "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                                     "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditListOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                                     "PropertyListEditList");
        }

        [Test]
        public void PropertyListEditTable() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit("TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                             "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                             "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                             "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListEdit(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                             "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                                     "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                                     "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                                     "PropertyListEditTable");
        }

        [Test]
        public void PropertyListEditTableOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListEdit(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                                     "PropertyListEditTable");
        }

        [Test]
        public void PropertyListExclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithout("TestCollectionOne", "TestInt").ToString(),
                             "PropertyListWithExclusions");
        }

        [Test]
        public void PropertyListExclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithout(y => y.TestCollectionOne, y => y.TestInt).ToString(),
                             "PropertyListWithExclusions");
        }

        [Test]
        public void PropertyListExclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWithout(tc, y => y.TestCollectionOne, y => y.TestInt).ToString(),
                                     "PropertyListWithExclusions");
        }

        [Test]
        public void PropertyListExclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWithout(tc, "TestCollectionOne", "TestInt").ToString(),
                                     "PropertyListWithExclusions");
        }

        [Test]
        public void PropertyListInclusions() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWith("TestInt", "TestCollectionOne").ToString(),
                             "PropertyListWithInclusions");
        }

        [Test]
        public void PropertyListInclusionsGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWith(y => y.TestInt, y => y.TestCollectionOne).ToString(),
                             "PropertyListWithInclusions");
        }

        [Test]
        public void PropertyListInclusionsGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWith(tc, y => y.TestInt, y => y.TestCollectionOne).ToString(),
                                     "PropertyListWithInclusions");
        }

        [Test]
        public void PropertyListInclusionsOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyListWith(tc, "TestInt", "TestCollectionOne").ToString(),
                                     "PropertyListWithInclusions");
        }

        [Test]
        public void PropertyListList() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList("TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                             "PropertyListList");
        }

        [Test]
        public void PropertyListListDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                             "PropertyListList");
        }

        [Test]
        public void PropertyListListDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                             "PropertyListList");
        }

        [Test]
        public void PropertyListListGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                             "PropertyListList");
        }

        [Test]
        public void PropertyListListOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.List).ToString(),
                                     "PropertyListList");
        }

        [Test]
        public void PropertyListListOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.List)).ToString(),
                                     "PropertyListList");
        }

        [Test]
        public void PropertyListListOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List)).ToString(),
                                     "PropertyListList");
        }

        [Test]
        public void PropertyListListOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.List).ToString(),
                                     "PropertyListList");
        }

        [Test]
        public void PropertyListTable() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList("TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                             "PropertyListTable");
        }

        [Test]
        public void PropertyListTableDict() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                             "PropertyListTable");
        }

        [Test]
        public void PropertyListTableDictGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                             "PropertyListTable");
        }

        [Test]
        public void PropertyListTableGeneric() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyList(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                             "PropertyListTable");
        }

        [Test]
        public void PropertyListTableOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, "TestCollectionOne", PropertyExtensions.CollectionFormat.Table).ToString(),
                                     "PropertyListTable");
        }

        [Test]
        public void PropertyListTableOtherObjDict() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<string, PropertyExtensions.CollectionFormat>("TestCollectionOne", PropertyExtensions.CollectionFormat.Table)).ToString(),
                                     "PropertyListTable");
        }

        [Test]
        public void PropertyListTableOtherObjDictGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, new Tuple<Expression<Func<CustomHelperTestClass, IEnumerable>>, PropertyExtensions.CollectionFormat>(y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table)).ToString(),
                                     "PropertyListTable");
        }

        [Test]
        public void PropertyListTableOtherObjGeneric() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestOtherObj(x => x.HtmlHelper.PropertyList(tc, y => y.TestCollectionOne, PropertyExtensions.CollectionFormat.Table).ToString(),
                                     "PropertyListTable");
        }

        [Test]
        public void RefPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestRef).ToString(),
                             "TestRefEdit");
        }

        [Test]
        public void RefPropertyEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestRef).ToString(),
                                     "TestRefEditOther");
        }

        [Test]
        public void RefPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestRef").ToString(),
                             "TestRefEdit");
        }

        [Test]
        public void RefPropertyStringEditExistingValue() {

            var mocks = new ContextMocks(controller);
            var tc = (CustomHelperTestClass)GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;
            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var id = FrameworkHelper.GetObjectId(tc);
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("CustomHelperTestClass-TestRef-Select", new ValueProviderResult(id, null, null));
            
            string s = mocks.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestRef").ToString();
       
            CheckResults("RefPropertyStringEditExistingValue", s);
        }


        [Test]
        public void RefPropertyStringEditOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestRef").ToString(),
                                     "TestRefEdit");
        }

        [Test]
        public void RefPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestRef").ToString(),
                             "TestRefView");
        }

        [Test]
        public void RefPropertyStringViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestRef").ToString(),
                                     "TestRefView");
        }

        [Test]
        public void RefPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestRef).ToString(),
                             "TestRefView");
        }

        [Test]
        public void RefPropertyViewOtherObj() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestRef).ToString(),
                                     "TestRefView");
        }

        [Test]
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

        [Test]
        public void SingleServiceMenu() {
            var mocks = new ContextMocks(controller);
            object ts = GetTestService("Custom Helper Test Classes").NakedObject.Object;
            string s = mocks.HtmlHelper.ServiceMenu(ts).ToString();
      

            CheckResults("SingleServiceMenu", s);
        }

        [Test]
        public void StringPropertyEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit(y => y.TestString).ToString(),
                             "TestStringEdit");
        }

        [Test]
        public void StringPropertyOtherObjEdit() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, y => y.TestString).ToString(),
                                     "TestStringEdit");
        }

        [Test]
        public void StringPropertyOtherObjView() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, y => y.TestString).ToString(),
                                     "TestStringView");
        }

        [Test]
        public void StringPropertyStringEdit() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyEdit("TestString").ToString(),
                             "TestStringEdit");
        }

        [Test]
        public void StringPropertyStringOtherObjEdit() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyEdit(tc, "TestString").ToString(),
                                     "TestStringEdit");
        }

        [Test]
        public void StringPropertyStringOtherObjView() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectPropertyView(tc, "TestString").ToString(),
                                     "TestStringView");
        }

        [Test]
        public void StringPropertyStringView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView("TestString").ToString(),
                             "TestStringView");
        }

        [Test]
        public void StringPropertyView() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectPropertyView(y => y.TestString).ToString(),
                             "TestStringView");
        }

        [Test]
        public void ThreeRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.ThreeRefParametersAction).ToString(),
                             "ThreeRefParametersAction");
        }

        [Test]
        public void ThreeRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.ThreeRefParametersAction).ToString(),
                                     "ThreeRefParametersAction");
        }

        [Test]
        public void ThreeRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.ThreeRefParametersFunction).ToString(),
                             "ThreeRefParametersFunction");
        }

        [Test]
        public void ThreeRefParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              int>(tc, y => y.ThreeRefParametersFunction).ToString(),
                                     "ThreeRefParametersFunction");
        }

        [Test]
        public void ThreeValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction).ToString(),
                             "ThreeValueParametersAction");
        }

        [Test]
        public void ThreeValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction).ToString(),
                                     "ThreeValueParametersAction");
        }

        [Test]
        public void ThreeValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction).ToString(),
                             "ThreeValueParametersFunction");
        }

        [Test]
        public void ThreeValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction).ToString(),
                                     "ThreeValueParametersFunction");
        }

        [Test]
        public void TwoRefParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.TwoRefParametersAction).ToString(),
                             "TwoRefParametersAction");
        }

        [Test]
        public void TwoRefParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.TwoRefParametersAction).ToString(),
                                     "TwoRefParametersAction");
        }

        [Test]
        public void TwoRefParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.TwoRefParametersFunction).ToString(),
                             "TwoRefParametersFunction");
        }

        [Test]
        public void TwoRefParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              int>(tc, y => y.TwoRefParametersFunction).ToString(),
                                     "TwoRefParametersFunction");
        }

        [Test]
        public void TwoValueParametersAction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction).ToString(),
                             "TwoValueParametersAction");
        }

        [Test]
        public void TwoValueParametersActionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction).ToString(),
                                     "TwoValueParametersAction");
        }

        [Test]
        public void TwoValueParametersFunction() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction).ToString(),
                             "TwoValueParametersFunction");
        }

        [Test]
        public void TwoValueParametersFunctionOnOtherObject() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction).ToString(),
                                     "TwoValueParametersFunction");
        }

        //

        [Test]
        public void ThreeRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.ThreeRefParametersAction, new {parm1 = TestClass, parm2 = TestClass, parm3 = TestClass}).ToString(),
                             "ThreeRefParametersActionWithParameters");
        }

        [Test]
        public void ThreeRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.ThreeRefParametersAction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass }).ToString(),
                                     "ThreeRefParametersActionWithParameters");
        }

        [Test]
        public void ThreeRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.ThreeRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass }).ToString(),
                             "ThreeRefParametersFunctionWithParameters");
        }

        [Test]
        public void ThreeRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              int>(tc, y => y.ThreeRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass, parm3 = TestClass }).ToString(),
                                     "ThreeRefParametersFunctionWithParameters");
        }

        [Test]
        public void ThreeValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, new { parm1 = 1, parm2 = 2, parm3 = 3 }).ToString(),
                             "ThreeValueParametersActionWithParameters");
        }

        [Test]
        public void ThreeValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, new { parm1 = 1, parm2 = 2, parm3 = 3 }).ToString(),
                                     "ThreeValueParametersActionWithParameters");
        }

        [Test]
        public void ThreeValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, new { parm1 = 1, parm2 = 2, parm3 = 3 }).ToString(),
                             "ThreeValueParametersFunctionWithParameters");
        }

        [Test]
        public void ThreeValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, new { parm1 = 1, parm2 = 2, parm3 = 3 }).ToString(),
                                     "ThreeValueParametersFunctionWithParameters");
        }

        [Test]
        public void TwoRefParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass>(y => y.TwoRefParametersAction, new { parm1 = TestClass, parm2 = TestClass }).ToString(),
                             "TwoRefParametersActionWithParameters");
        }

        [Test]
        public void TwoRefParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass>(tc, y => y.TwoRefParametersAction, new { parm1 = TestClass, parm2 = TestClass }).ToString(),
                                     "TwoRefParametersActionWithParameters");
        }

        [Test]
        public void TwoRefParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      CustomHelperTestClass,
                                      int>(y => y.TwoRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass }).ToString(),
                             "TwoRefParametersFunctionWithParameters");
        }

        [Test]
        public void TwoRefParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              CustomHelperTestClass,
                                              int>(tc, y => y.TwoRefParametersFunction, new { parm1 = TestClass, parm2 = TestClass }).ToString(),
                                     "TwoRefParametersFunctionWithParameters");
        }

        [Test]
        public void TwoValueParametersActionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, new { parm1 = 1, parm2 = 2 }).ToString(),
                             "TwoValueParametersActionWithParameters");
        }

        [Test]
        public void TwoValueParametersActionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, new { parm1 = 1, parm2 = 2 }).ToString(),
                                     "TwoValueParametersActionWithParameters");
        }

        [Test]
        public void TwoValueParametersFunctionWithParameters() {
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().ObjectAction<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, new { parm1 = 1, parm2 = 2 }).ToString(),
                             "TwoValueParametersFunctionWithParameters");
        }

        [Test]
        public void TwoValueParametersFunctionOnOtherObjectWithParameters() {
            CustomHelperTestClass tc = TestClass;

            CustomHelperTestOtherObj(x => x.HtmlHelper.ObjectAction<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, new { parm1 = 1, parm2 = 2 }).ToString(),
                                     "TwoValueParametersFunctionWithParameters");
        }




        // object

        [Test]
        public void TypeName() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName().ToString(),
                                          "CustomHelperTestClass");
        }

        [Test]
        public void TypeNameFourParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int>(y => y.FourValueParametersAction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameFourParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int, int>(y => y.FourValueParametersFunction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameFourParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int>(tc, y => y.FourValueParametersAction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameFourParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int, int>(tc, y => y.FourValueParametersFunction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameOneParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int>(y => y.OneValueParameterAction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameOneParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int>(y => y.OneValueParameterFunction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameOneParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int>(tc, y => y.OneValueParameterAction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameOneParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int>(tc, y => y.OneValueParameterFunction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc).ToString(),
                                                  "CustomHelperTestClass");
        }

        [Test]
        public void TypeNameParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName("OneValueParameterAction", 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, "OneValueParameterAction", 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameProperty() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName("TestInt").ToString(),
                                          "Int32");
        }

        // property

        [Test]
        public void TypeNamePropertyGeneric() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName(y => y.TestInt).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNamePropertyGenericOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, y => y.TestInt).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNamePropertyOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName(tc, "TestInt").ToString(),
                                                  "Int32");
        }

        // parm 


        [Test]
        public void TypeNameThreeParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int>(y => y.ThreeValueParametersAction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameThreeParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int, int>(y => y.ThreeValueParametersFunction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameThreeParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int>(tc, y => y.ThreeValueParametersAction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameThreeParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int, int>(tc, y => y.ThreeValueParametersFunction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameTwoParm() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int>(y => y.TwoValueParametersAction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameTwoParmFunc() {
            CustomHelperTestCompareDirect(x => x.GetHtmlHelper<CustomHelperTestClass>().TypeName<CustomHelperTestClass, int, int, int>(y => y.TwoValueParametersFunction, 0).ToString(),
                                          "Int32");
        }

        [Test]
        public void TypeNameTwoParmOtherObj() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int>(tc, y => y.TwoValueParametersAction, 0).ToString(),
                                                  "Int32");
        }

        [Test]
        public void TypeNameTwoParmOtherObjFunc() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTestCompareDirectOtherObj(x => x.HtmlHelper.TypeName<CustomHelperTestClass, int, int, int>(tc, y => y.TwoValueParametersFunction, 0).ToString(),
                                                  "Int32");
        }


        [Test]
        public void PropertyListWithoutCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListWithoutCollections(tc).ToString(), "PropertyListWithoutCollections");
        }

        [Test]
        public void PropertyListOnlyCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc).ToString(), "PropertyListOnlyCollections");
        }

        [Test]
        public void PropertyListOnlyCollectionsFormatList() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc, PropertyExtensions.CollectionFormat.List).ToString(), "PropertyListOnlyCollectionsFormatList");
        }

        [Test]
        public void PropertyListOnlyCollectionsFormatTable() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertyListOnlyCollections(tc, PropertyExtensions.CollectionFormat.Table).ToString(), "PropertyListOnlyCollectionsFormatTable");
        }

        [Test]
        public void PropertiesListOnlyCollections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().PropertiesListOnlyCollections(tc).Aggregate("", (s,t) => s + t.ToString()), "PropertiesListOnlyCollections");
        }

        [Test]
        public void Collections() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc).ToString(), "Collections");
        }

        [Test]
        public void CollectionsFormatList() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc, IdHelper.ListDisplayFormat).ToString(), "CollectionsFormatList");
        }

        [Test]
        public void CollectionsFormatTable() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().Collections(tc, IdHelper.TableDisplayFormat).ToString(), "CollectionsFormatTable");
        }

        [Test]
        public void CollectionTitles() {
            CustomHelperTestClass tc = TestClass;
            CustomHelperTest(x => x.GetHtmlHelper<CustomHelperTestClass>().CollectionTitles(tc, "{0} {1}").ToString(), "CollectionTitles");
        }

    }
}