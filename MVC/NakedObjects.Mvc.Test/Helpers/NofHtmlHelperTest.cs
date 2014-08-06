// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using Expenses.Currencies;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Moq;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

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
    public class NofHtmlHelperTest : AcceptanceTestCase {
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
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
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions(),
                                                              new NotContributedTestService(),
                                                              new ViewModelTestService()}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        protected override IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller { SimpleOidGeneratorStart = 100 }; }
        }

        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();

        private static string GetTestData(string name) {
            string file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            return File.ReadAllText(file);
        }

        private static void WriteTestData(string name, string data) {
            string file = Path.Combine(@"..\..\Generated Html reference files", name) + ".htm";
            File.WriteAllText(file, data);
        }

        private static void CheckResults(string resultsFile, string s) {
            string actionView = GetTestData(resultsFile);
            Assert.AreEqual(actionView, s);
            //WriteTestData(resultsFile, s);
        }

        private DescribedCustomHelperTestClass DescribedTestClass {
            get { return (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        private NotPersistedTestClass NotPersistedTestClass {
            get { return new NotPersistedTestClass(); }
        }

     

        [Test]
        public void ActionDialogId() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            INakedObjectAction action = FrameworkHelper.GetNakedObject(claim).Specification.GetObjectActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            Assert.AreEqual(@"Claim-CopyAllExpenseItemsFromAnotherClaim-Dialog", mocks.HtmlHelper.ObjectActionDialogId(claim, action).ToString());
        }

        [Test]
        public void ActionName() {
            var mocks = new ContextMocks(controller);
            Assert.AreEqual(@"<div class=""nof-actionname"">Test</div>", mocks.HtmlHelper.ObjectActionName("Test").ToString());
        }

        [Test]
        public void BoolPropertyEdit() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyListEdit(testBool).ToString();

            CheckResults("BoolPropertyEdit", s);
        }



        [Test]
        public void DuplicateAction() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testBool = (BoolTestClass)GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.GetHtmlHelper<BoolTestClass>().Menu(testBool).ToString();
            CheckResults("DuplicateAction", s);
        }


        [Test]
        public void BoolPropertyView() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testBool = (BoolTestClass) GetBoundedInstance<BoolTestClass>("BoolClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testBool).ToString();
            CheckResults("BoolPropertyView", s);
        }

        [Test]
        public void EnumPropertyView() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testEnum = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(testEnum).ToString();
            CheckResults("EnumPropertyView", s);
        }


        [Test]
        public void ChoicesParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameter", s);
        }

        [Test]
        public void MultipleChoicesParameterDomainObject1() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesDomainObject1");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject", s);
        }

        [Test]
        public void MultipleChoicesParameterDomainObject2() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesDomainObject2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterDomainObject2", s);
        }

        [Test]
        public void MultipleChoicesParameterString() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesString");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterString", s);
        }

        [Test]
        public void MultipleChoicesParameterInt() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesInt");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterInt", s);
        }

        [Test]
        public void MultipleChoicesParameterBounded() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            //testChoices.TestChoicesProperty = testChoices;
            //testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesBounded");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterBounded", s);
        }


        [Test]
        public void ChoicesParameterWithDefault() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm1-Select"] = FrameworkHelper.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction4-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }

       
        [Test]
        public void MultipleChoicesParameterWithDefault() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm1-Select"] = FrameworkHelper.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestMultipleChoicesAction4-Parm2-Select"] = PersistorUtils.CreateAdapter(new List<string> {"test1", "test2"});

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestMultipleChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("MultipleChoicesParameterWithDefault", s);
        }


        [Test]
        public void ChoicesParameterWithExistingValues() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
          

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm1-Select", new ValueProviderResult(FrameworkHelper.GetNakedObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction4-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction4");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterWithDefault", s);
        }



        [Test]
        public void ChoicesParameterAlternativeSyntax() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction2");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntax", s);
        }

        [Test]
        public void ChoicesParameterAlternativeSyntaxWithDefault() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm1-Select"] = FrameworkHelper.GetNakedObject(testChoices);
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestChoicesAction3-Parm2-Input"] = "test1";

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }

        [Test]
        public void ChoicesParameterAlternativeSyntaxWithExistingValues() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testChoices = (ChoicesTestClass)GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
           

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm1-Select", new ValueProviderResult( FrameworkHelper.GetNakedObject(testChoices), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("ChoicesTestClass-TestChoicesAction3-Parm2-Input", new ValueProviderResult("test1", null, null));

            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestChoicesAction3");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("ChoicesParameterAlternativeSyntaxWithDefault", s);
        }


        [Test]
        public void AutoCompleteParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testAC = (AutoCompleteTestClass)GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testAC).Specification.GetObjectActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameter", s);
        }

        [Test]
        public void AutoCompleteParameterWithDefault() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testAC = (AutoCompleteTestClass)GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select"] = FrameworkHelper.GetNakedObject(testAC);
            mocks.ViewDataContainer.Object.ViewData["AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input"] = "test1";

            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testAC).Specification.GetObjectActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithDefault", s);
        }

        [Test]
        public void AutoCompleteParameterWithExistingValues() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            var testAC = (AutoCompleteTestClass)GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm1-Select", new ValueProviderResult(FrameworkHelper.GetNakedObject(testAC), null, null));
            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue("AutoCompleteTestClass-TestAutoCompleteAction-Parm2-Input", new ValueProviderResult("test1", null, null));

            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";
            INakedObjectAction action = FrameworkHelper.GetNakedObject(testAC).Specification.GetObjectActions().Single(p => p.Id == "TestAutoCompleteAction");

            string s = mocks.HtmlHelper.ParameterList(testAC, action).ToString();

            CheckResults("AutoCompleteParameterWithExistingValues", s);
        }



        [Test]
        public void EnumParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameter", s);
        }

        [Test]
        public void EnumParameterWithDefault() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            mocks.ViewDataContainer.Object.ViewData["EnumTestClass-TestActualEnumParm-Parm-Input"] = TestEnum.Paris;
            var testChoices = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestActualEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterWithDefault", s);
        }


        [Test]
        public void EnumParameterAnnotation() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestAnnotationEnumParm");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotation", s);
        }

        [Test]
        public void EnumParameterChoices() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestActualEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterChoices", s);
        }

        [Test]
        public void EnumParameterAnnotationChoices() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestAnnotationEnumParmChoices");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

            CheckResults("EnumParameterAnnotationChoices", s);
        }


        [Test]
        public void EmptyQueryableParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = PersistorUtils.CreateAdapter(new List<ChoicesTestClass>().AsQueryable());
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = collectionAdapter;

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

         
            CheckResults("EmptyQueryableParameter", s);
        }

        [Test]
        public void EmptyEnumerableParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = PersistorUtils.CreateAdapter(new List<ChoicesTestClass>());
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = collectionAdapter;

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

      

            CheckResults("EmptyEnumerableParameter", s);
        }

        [Test]
        public void QueryableParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();


            INakedObject collectionAdapter = PersistorUtils.CreateAdapter(new List<ChoicesTestClass> {testChoices}.AsQueryable());
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestQueryableAction-Parm1-Select"] = collectionAdapter;

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestQueryableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

        

            CheckResults("QueryableParameter", s);
        }

        [Test]
        public void EnumerableParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();

            INakedObject collectionAdapter = PersistorUtils.CreateAdapter(new List<ChoicesTestClass> {testChoices});
            collectionAdapter.SetATransientOid(new DummyOid());
            mocks.ViewDataContainer.Object.ViewData["ChoicesTestClass-TestEnumerableAction-Parm1-Select"] = collectionAdapter;

            INakedObjectAction action = FrameworkHelper.GetNakedObject(testChoices).Specification.GetObjectActions().Single(p => p.Id == "TestEnumerableAction");

            string s = mocks.HtmlHelper.ParameterList(testChoices, action).ToString();

         

            CheckResults("EnumerableParameter", s);
        }


        [Test]
        public void ChoicesProperty() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testChoices = (ChoicesTestClass) GetBoundedInstance<ChoicesTestClass>("Class1").GetDomainObject();
            testChoices.TestChoicesProperty = testChoices;
            testChoices.TestChoicesStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testChoices).ToString();

         

            CheckResults("ChoicesProperty", s);
        }

        [Test]
        public void AutoCompleteProperty() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testAC = (AutoCompleteTestClass)GetBoundedInstance<AutoCompleteTestClass>("Class4").GetDomainObject();
            testAC.TestAutoCompleteProperty = DescribedTestClass;
            testAC.TestAutoCompleteStringProperty = "test2";

            string s = mocks.HtmlHelper.PropertyListEdit(testAC).ToString();

            CheckResults("AutoCompleteProperty", s);
        }

        [Test]
        public void EnumProperty() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            var testEnum = (EnumTestClass)GetBoundedInstance<EnumTestClass>("EnumClass").GetDomainObject();
  
            string s = mocks.HtmlHelper.PropertyListEdit(testEnum).ToString();

            CheckResults("EnumProperty", s);
        }

        [Test]
        public void CollectionViewForEmptyCollection() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] {};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
         

            CheckResults("CollectionViewForEmptyCollection", s);
        }

        [Test]
        public void QueryableViewForEmptyCollection() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] { }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
          

            CheckResults("QueryableViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionViewForOneElementCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] {claim};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
        

            CheckResults("CollectionViewForOneElementCollectionNoPage", s);
        }

        [Test]
        public void CollectionViewForOneElementCollectionWithMultiline() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] {pc};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
           

            CheckResults("CollectionViewForOneElementCollectionWithMultiline", s);
        }


        [Test]
        public void CollectionViewForPagedCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void CollectionViewForPagedCollectionPage1() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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


        [Test]
        public void CollectionViewForPagedCollectionPage2() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] {claim1};
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        //

        [Test]
        public void QueryableViewForOneElementCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] { claim }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
        

            CheckResults("QueryableViewForOneElementCollection", s);
        }

        [Test]
        public void QueryableViewForOneElementCollectionWithMultiline() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] { pc }.AsQueryable(); 
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

           

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();
         

            CheckResults("QueryableViewForOneElementCollectionWithMultiline", s);
        }


        [Test]
        public void QueryableViewForPagedCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable(); 
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void QueryableViewForPagedCollectionPage1() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable(); 
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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


        [Test]
        public void QueryableViewForPagedCollectionPage2() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable(); 
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        // new

        [Test]
        public void CollectionListViewForEmptyCollection() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] { };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionListViewForEmptyCollectionTableView() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] { };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForEmptyCollection", s);
        }

        [Test]
        public void CollectionListViewForEagerlyCollectionTableView() {
            var mocks = new ContextMocks(controller);
         
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            var collection = new[] { claim };

            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionList(collection, action).ToString();

            CheckResults("CollectionListViewForEagerlyCollectionTableView", s);
        }



        [Test]
        public void QueryableListViewForEmptyCollection() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] { }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForEmptyCollection", s);
        }

        [Test]
        public void QueryableListViewForEmptyCollectionTableView() {
            var mocks = new ContextMocks(controller);
            var collection = new object[] { }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForEmptyCollectionTableView", s);
        }


        [Test]
        public void CollectionListViewForOneElementCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] { claim };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionNoPage", s);
        }

        [Test]
        public void CollectionListViewForOneElementCollectionTableView() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] { claim };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();

            CheckResults("CollectionListViewForOneElementCollectionNoPageTableView", s);
        }

        [Test]
        public void CollectionlistViewForOneElementCollectionWithMultiline() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] { pc };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("CollectionListViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void CollectionlistViewForOneElementCollectionWithMultilineTableView() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] { pc };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionWithMultilineTableView", s);
        }



        [Test]
        public void CollectionListViewForPagedCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void CollectionListViewForPagedCollectionTableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 1},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 1}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionTableView", s);
        }



        [Test]
        public void CollectionListViewForPagedCollectionPage1() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void CollectionListViewForPagedCollectionPage1TableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 1},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 2}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage1TableView", s);
        }


        [Test]
        public void CollectionListViewForPagedCollectionPage2() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void CollectionListViewForPagedCollectionPage2TableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 };
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 2},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 2}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");


            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("CollectionListViewForOneElementCollectionPage2TableView", s);
        }


        //

        [Test]
        public void QueryableListViewForOneElementCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] { claim }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();

            CheckResults("QueryableListViewForOneElementCollection", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollectionTableView() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            var collection = new[] { claim }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForOneElementCollectionTableView", s);
        }


        [Test]
        public void QueryableListViewForOneElementCollectionWithMultiline() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] { pc }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());



            string s = mocks.HtmlHelper.CollectionTable(collection, null).ToString();


            CheckResults("QueryableListViewForOneElementCollectionWithMultiline", s);
        }

        [Test]
        public void QueryableListViewForOneElementCollectionWithMultilineTableView() {
            var mocks = new ContextMocks(controller);
            ProjectCode pc = NakedObjectsContext.ObjectPersistor.Instances<ProjectCode>().Single(c => c.Code == "005");


            var collection = new[] { pc }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForOneElementCollectionWithMultilineTableView", s);
        }


        [Test]
        public void QueryableListViewForPagedCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void QueryableListViewForPagedCollectionTableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 1},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 1}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionTableView", s);
        }


        [Test]
        public void QueryableListViewForPagedCollectionPage1() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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

        [Test]
        public void QueryableListViewForPagedCollectionPage1TableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 1},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 2}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage1TableView", s);
        }


        [Test]
        public void QueryableListViewForPagedCollectionPage2() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
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


        [Test]
        public void QueryableListViewForPagedCollectionPage2TableView() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();


            var collection = new[] { claim1 }.AsQueryable();
            INakedObject adapter = PersistorUtils.CreateAdapter(collection);
            adapter.SetATransientOid(new DummyOid());

            var pagingData = new Dictionary<string, int> {
                                                             {IdHelper.PagingCurrentPage, 2},
                                                             {IdHelper.PagingPageSize, 1},
                                                             {IdHelper.PagingTotal, 2}
                                                         };

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = pagingData;

            // use FindMyClaims action for TableView
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "FindMyClaims");

            string s = mocks.HtmlHelper.CollectionTable(collection, action).ToString();


            CheckResults("QueryableListViewForPagedCollectionPage2TableView", s);
        }


        //



        [Test]
        public void DisplayName() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().Menu(DescribedTestClass).ToString();
         
            CheckResults("DisplayName", s);
        }

        [Test]
        public void GenericAction() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", null, claim).ToString();
        
            CheckResults("GenericAction", s);
        }

        [Test]
        public void GenericActionWithController() {
            var mocks = new ContextMocks(controller);
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller").ToString();
       
            CheckResults("GenericActionWithController", s);
        }

        [Test]
        public void GenericActionWithRVDict() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerAction("Text", "Action", "Controller", new RouteValueDictionary(new {
                                                                                                                          id = FrameworkHelper.GetObjectId(claim)
                                                                                                                      })).ToString();
          
            CheckResults("GenericAction", s);
        }

        [Test]
        public void GenericEditAction() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", claim).ToString();
          
            CheckResults("GenericEditAction", s);
        }

        [Test]
        public void GenericEditActionWithController() {
            var mocks = new ContextMocks(controller);
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller").ToString();
         
            CheckResults("GenericEditActionWithController", s);
        }

        [Test]
        public void GenericEditActionWithRVDict() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.ControllerActionOnTransient("Text", "Action", "Controller", new RouteValueDictionary(new {
                                                                                                                                     id = FrameworkHelper.GetObjectId(claim)
                                                                                                                                 })).ToString();
           

            CheckResults("GenericEditAction", s);
        }

        [Test]
        public void MultiLineField() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEditWith(DescribedTestClass, x => x.TestMultiLineString).ToString();
       

            CheckResults("MultilineField", s);
        }

        [Test]
        public void MultiLineFieldView() {
            var mocks = new ContextMocks(controller);
            DescribedCustomHelperTestClass tc = DescribedTestClass;
            tc.TestMultiLineString = "Test String";

            mocks.ViewDataContainer.Object.ViewData.Model = tc;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListWith(tc, x => x.TestMultiLineString).ToString();
        

            CheckResults("MultilineFieldView", s);
        }


        [Test]
        public void MultiLineParameter() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string>(DescribedTestClass, x => x.TestMultiLineFunction).ToString();
           

            CheckResults("MultilineParameter", s);
        }

        [Test]
        public void NotPersistedMenu() {
            var mocks = new ContextMocks(controller);
            INakedObject adapter = FrameworkHelper.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().MenuOnTransient(adapter.Object).ToString();
      

            CheckResults("NotPersistedMenu", s);
        }

        [Test]
        public void NotPersistedWithoutButton() {
            var mocks = new ContextMocks(controller);
            INakedObject adapter = FrameworkHelper.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyListEditHidden(adapter.Object).ToString();
      

            CheckResults("NotPersistedWithoutButton", s);
           
        }

        [Test]
        public void NotPersistedPropertyList() {
            


            var mocks = new ContextMocks(controller);
            INakedObject adapter = FrameworkHelper.GetNakedObject(NotPersistedTestClass);
            mocks.ViewDataContainer.Object.ViewData.Model = adapter.Object;
            string s = mocks.GetHtmlHelper<NotPersistedTestClass>().PropertyList(adapter.Object).ToString();
          

            CheckResults("NotPersistedPropertyList", s);
        }


        [Test]
        public void Object() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object(claim).ToString();
         

            CheckResults("Object", s);
        }

        [Test]
        public void ViewModel() {
            var mocks = new ContextMocks(controller);
            Employee employee = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();

            var no = FrameworkHelper.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);

            string s = mocks.HtmlHelper.Object(no.Object).ToString();

            CheckResults("ViewModel", s);
        }


        [Test]
        public void ObjectActions() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Menu(claim).ToString();
         
            CheckResults("ObjectActions", s);
        }

        [Test]
        public void ObjectActionsWithHints() {
            var mocks = new ContextMocks(controller);
            var hint = (HintTestClass)GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            string s = mocks.HtmlHelper.Menu(hint).ToString();

            CheckResults("ObjectActionsWithHints", s);
        }

        [Test]
        public void ObjectPropertiesWithHints() {
            var mocks = new ContextMocks(controller);
            var hint = (HintTestClass)GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyList(hint).ToString();

            CheckResults("ObjectPropertiesWithHints", s);
        }

        [Test]
        public void ObjectEditPropertiesWithHints() {
            var mocks = new ContextMocks(controller);
            var hint = (HintTestClass)GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            string s = mocks.HtmlHelper.PropertyListEdit(hint).ToString();

            CheckResults("ObjectEditPropertiesWithHints", s);
        }



        [Test]
        public void ViewModelActions() {
            var mocks = new ContextMocks(controller);
            Employee employee = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();

            var no = FrameworkHelper.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.Menu(no.Object).ToString();

            CheckResults("ViewModelActions", s);
        }

        [Test]
        public void ViewModelProperties() {
            var mocks = new ContextMocks(controller);
            Employee employee = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();

            var no = FrameworkHelper.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyList(no.Object).ToString();

            CheckResults("ViewModelProperties", s);
        }

        [Test]
        public void ViewModelPropertiesEdit() {
            var mocks = new ContextMocks(controller);
            Employee employee = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();

            var no = FrameworkHelper.GetNakedObjectFromId("MvcTestApp.Tests.Helpers.ViewModelTestClass;1;" + employee.Name);
            string s = mocks.HtmlHelper.PropertyListEdit(no.Object).ToString();

            CheckResults("ViewModelPropertiesEdit", s);
        }


        [Test]
        public void ObjectActionsTestNotContributed1() {
            var mocks = new ContextMocks(controller);

            var nc1 = (NotContributedTestClass1) GetBoundedInstance<NotContributedTestClass1>("NC1Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc1).ToString();


            CheckResults("ObjectActionsTestNotContributed1", s);
        }

        [Test]
        public void ObjectActionsTestNotContributed2() {
            var mocks = new ContextMocks(controller);

            var nc2 = (NotContributedTestClass2) GetBoundedInstance<NotContributedTestClass2>("NC2Class").GetDomainObject();
            string s = mocks.HtmlHelper.Menu(nc2).ToString();

            CheckResults("ObjectActionsTestNotContributed2", s);
        }


        [Test]
        public void ObjectActionsWithConcurrency() {
            var mocks = new ContextMocks(controller);
            RecordedAction recordedAction = NakedObjectsContext.ObjectPersistor.Instances<RecordedAction>().First();
            string s = mocks.HtmlHelper.Menu(recordedAction).ToString();
     

            CheckResults("ObjectActionsWithConcurrency", s);
        }

        [Test]
        public void ObjectEditFieldsWithActionAsFind() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject employeeRepo = FrameworkHelper.GetAdaptedService("EmployeeRepository");
            INakedObjectAction action = employeeRepo.Specification.GetObjectActions().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim, employeeRepo.Object, action, "Approver", null).ToString();
          

            CheckResults("ObjectEditFieldsWithActionAsFind", s);
        }

        [Test]
        public void ObjectEditFieldsWithFinder() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Employee emp = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.PropertyListEdit(claim, null, null, "Approver", new[] {emp}).ToString();
        

            CheckResults("ObjectEditFieldsWithFinder", s);
        }

        [Test]
        public void ObjectEditFieldsWithInlineObject() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            var claim2 = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 19);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject employeeRepo = FrameworkHelper.GetAdaptedService("EmployeeRepository");
            INakedObjectAction action = employeeRepo.Specification.GetObjectActions().Single(a => a.Id == "FindEmployeeByName");

            string s = mocks.HtmlHelper.PropertyListEdit(claim1, employeeRepo.Object, action, "Approver", new[] {claim2}).ToString();
         

            CheckResults("ObjectEditFieldsWithInlineObject", s);
        }


        [Test]
        public void ObjectEditFieldsWithListCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();
         

            CheckResults("ObjectEditFieldsWithListCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithListCollectionAndRemove() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

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
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();
          

            CheckResults("ObjectEditFieldsWithSummaryCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithSummaryCollectionForTransient() {
            var mocks = new ContextMocks(controller);
            var claim = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();
            claim.DateCreated = new DateTime(2010, 3, 25);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();
          

            CheckResults("ObjectEditFieldsWithSummaryCollectionForTransient", s);
        }

        [Test]
        public void ObjectEditFieldsWithTableCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.PropertyListEdit(claim).ToString();
         

            CheckResults("ObjectEditFieldsWithTableCollection", s);
        }

        [Test]
        public void ObjectEditFieldsWithTableCollectionAndRemove() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

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
        public void ObjectFieldsWithListCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "list";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();
          

            CheckResults("ObjectFieldsWithListCollection", s);
        }

        [Test]
        public void ObjectFieldsWithSummaryCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();
       
            CheckResults("ObjectFieldsWithSummaryCollection", s);
        }

        [Test]
        public void ObjectFieldsWithTableCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData["ExpenseItems"] = "table";
            string s = mocks.HtmlHelper.PropertyList(claim).ToString();
          

            CheckResults("ObjectFieldsWithTableCollection", s);
        }


        [Test]
        public void ObjectForEnumerable() {
            var mocks = new ContextMocks(controller);
            IList<Claim> claims = NakedObjectsContext.ObjectPersistor.Instances<Claim>().Take(2).ToList();

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = new Dictionary<string, int> {
                                                                                                           {IdHelper.PagingCurrentPage, 1},
                                                                                                           {IdHelper.PagingPageSize, 2},
                                                                                                           {IdHelper.PagingTotal, 2}
                                                                                                       };
            var claimAdapter = PersistorUtils.CreateAdapter(claims.First());
            var adapter = PersistorUtils.CreateAdapter(claims);
            var mockOid = new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            adapter.SetATransientOid(mockOid);


            string s = mocks.HtmlHelper.Object(claims).ToString();
         

            CheckResults("ObjectForEnumerable", s);
        }

        [Test]
        public void ObjectForQueryable() {
            var mocks = new ContextMocks(controller);
            IQueryable<Claim> claims = NakedObjectsContext.ObjectPersistor.Instances<Claim>().Take(2);

            mocks.ViewDataContainer.Object.ViewData[IdHelper.PagingData] = new Dictionary<string, int> {
                                                                                                           {IdHelper.PagingCurrentPage, 1},
                                                                                                           {IdHelper.PagingPageSize, 2},
                                                                                                           {IdHelper.PagingTotal, 2}
                                                                                                       };

            var claimAdapter = PersistorUtils.CreateAdapter(claims.First());
            var adapter = PersistorUtils.CreateAdapter(claims);
            var mockOid = new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            adapter.SetATransientOid(mockOid);

            string s = mocks.HtmlHelper.Object(claims).ToString();
          

            CheckResults("ObjectForQueryable", s);
        }

        [Test]
        public void ObjectHasVisibleFields() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Assert.IsTrue(mocks.HtmlHelper.ObjectHasVisibleFields(claim));
        }

        [Test]
        public void ObjectLinkAndIcon() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            string s = mocks.HtmlHelper.Object("Text", "Action", claim).ToString();
       

            CheckResults("ObjectLinkAndIcon", s);
        }

        [Test]
        public void ObjectTitle() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            Assert.AreEqual(@"28th Mar - Sales call, London", mocks.HtmlHelper.ObjectTitle(claim).ToString());
        }

        [Test]
        public void ObjectTypeAsCssId() {
            var mocks = new ContextMocks(controller);
            Employee emp = NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            List<Employee> allEmployees = NakedObjectsContext.ObjectPersistor.Instances<Employee>().ToList();

            string empId = mocks.HtmlHelper.ObjectTypeAsCssId(emp).ToString();
            string allEmpId = mocks.HtmlHelper.ObjectTypeAsCssId(allEmployees).ToString();

            Assert.AreEqual("Expenses-ExpenseEmployees-Employee", empId);
            Assert.AreEqual("System-Collections-Generic-List`1[[Expenses-ExpenseEmployees-Employee]]", allEmpId);
        }

        [Test]
        public void ObjectWithImage() {
            var mocks = new ContextMocks(controller);
            var currency = (Currency) GetBoundedInstance<Currency>("EUR").GetDomainObject();

            string s = mocks.HtmlHelper.PropertyList(currency).ToString();
          

            CheckResults("ObjectWithImage", s);
        }


        [Test]
        public void DialogWithAjaxDisabled() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            INakedObject adapter = FrameworkHelper.GetNakedObject(claim);

            INakedObjectAction action = adapter.Specification.GetObjectActions().Single(p => p.Id == "RejectItems");
   
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

            CheckResults("DialogWithAjaxDisabled", s);
        }

        [Test]
        public void ParameterEdit() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            INakedObject adapter = FrameworkHelper.GetNakedObject(claim);

            INakedObjectAction action = adapter.Specification.GetObjectActions().First();
            INakedObjectActionParameter parm = action.Parameters.First();

            string keyToSelect = IdHelper.GetParameterInputId(action, parm);
            INakedObject objToSelect = FrameworkHelper.GetNakedObject("Expenses.ExpenseClaims.ExpenseType;4;False");

            mocks.ViewDataContainer.Object.ViewData.ModelState.SetModelValue(keyToSelect, new ValueProviderResult(objToSelect, null, null));

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();
           

            CheckResults("ParameterEdit", s);
        }

        [Test]
        public void ParameterListWithHint() {
            var mocks = new ContextMocks(controller);

            var hint = (HintTestClass)GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();

            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            INakedObject adapter = FrameworkHelper.GetNakedObject(hint);

            INakedObjectAction action = adapter.Specification.GetObjectActions().Single(a => a.Id == "ActionWithParms");
         
            string s = mocks.HtmlHelper.ParameterList(hint, action).ToString();

            CheckResults("ParameterListWithHint", s);
        }



        [Test]
        public void ParameterEditCollection() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            var tc1 = (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object;

            INakedObject adapter = FrameworkHelper.GetNakedObject(tc1);

            INakedObjectAction action = adapter.Specification.GetObjectActions().Single(a => a.Id == "OneCollectionParameterAction");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();

           

            CheckResults("ParameterEditCollection", s);
        }

        [Test]
        public void ParameterEditWithActionAsFind() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject adapter = FrameworkHelper.GetNakedObject(claim);
            INakedObjectAction action = FrameworkHelper.GetNakedObject(claim).Specification.GetObjectActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction targetAction = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", null).ToString();
        

            CheckResults("ParameterEditWithActionAsFind", s);
        }

        [Test]
        public void ParameterEditWithFinders() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject adapter = FrameworkHelper.GetNakedObject(claim);
            INakedObjectAction action = FrameworkHelper.GetNakedObject(claim).Specification.GetObjectActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, action).ToString();
         

            CheckResults("ParameterEditWithFinders", s);
        }

        [Test]
        public void ParameterEditWithInlineObject() {
            var mocks = new ContextMocks(controller);
            Claim claim1 = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            var claim2 = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            claim2.DateCreated = new DateTime(2010, 5, 18);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject adapter = FrameworkHelper.GetNakedObject(claim1);
            INakedObjectAction action = FrameworkHelper.GetNakedObject(claim1).Specification.GetObjectActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");

            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction targetAction = claimRepo.Specification.GetActionLeafNodes().First(a => a.Id == "CreateNewClaim");

            string s = mocks.HtmlHelper.ParameterList(adapter.Object, claimRepo.Object, action, targetAction, "otherClaim", new[] {claim2}).ToString();
       

            CheckResults("ParameterEditWithInlineObject", s);
        }

        [Test]
        public void ParameterEditWithSelection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

            INakedObject adapter = FrameworkHelper.GetNakedObject(claim);
            INakedObjectAction action = FrameworkHelper.GetNakedObject(claim).Specification.GetObjectActions().Single(a => a.Id == "CopyAllExpenseItemsFromAnotherClaim");
            string s = mocks.HtmlHelper.ParameterList(adapter.Object, null, action, null, "otherClaim", new[] {claim}).ToString();
        

            CheckResults("ParameterEditWithSelection", s);
        }

        [Test]
        public void ParameterEditForCollection() {
            var mocks = new ContextMocks(controller);
            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();

       
            INakedObject claimRepo = FrameworkHelper.GetAdaptedService("ClaimRepository");
            INakedObjectAction action = claimRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "MyRecentClaims");

            var selected = claimRepo.GetDomainObject<ClaimRepository>().MyRecentClaims().First();

            INakedObject target = PersistorUtils.CreateAdapter(new[] {claim}.AsQueryable());
            target.SetATransientOid(new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, new CollectionMemento(NakedObjectsContext.ObjectPersistor, NakedObjectsContext.Reflector, claimRepo, action, new INakedObject[] { }), new object[] { selected }));

            INakedObjectAction targetAction = claimRepo.Specification.GetActionLeafNodes().Single(a => a.Id == "ApproveClaims");

            string s = mocks.HtmlHelper.ParameterList(target.Object, null, targetAction, null, "claims", null).ToString();
        

            CheckResults("ParameterEditForCollection", s);
        }



//        [Test]
//        public void PasswordField() {
//            var mocks = new ContextMocks(controller);
//            PasswordTestClass ptc = PasswordTestClass;
//            mocks.ViewDataContainer.Object.ViewData.Model = ptc;
//            PersistorUtils.CreateAdapterForTransient(ptc, true);
//            string s = mocks.GetHtmlHelper<PasswordTestClass>().PropertyListEditWith(ptc, x => x.Password).ToString();
       

//            CheckResults("PasswordField", s);
//        }

//        [Test]
//        public void PasswordParameter() {
//            var mocks = new ContextMocks(controller);
//            PasswordTestClass ptc = PasswordTestClass;
//            mocks.ViewDataContainer.Object.ViewData.Model = ptc;
//            PersistorUtils.CreateAdapterForTransient(ptc, true);
//#pragma warning disable 612,618
//            string s = mocks.GetHtmlHelper<PasswordTestClass>().ObjectActionAsDialog<PasswordTestClass, Password>(ptc, x => x.ChangePassword).ToString();
//#pragma warning restore 612,618
          

//            CheckResults("PasswordParameter", s);
//        }

        [Test]
        public void BoolParameter() {
            var mocks = new ContextMocks(controller);
            var btc = new BoolTestClass();
            mocks.ViewDataContainer.Object.ViewData.Model = btc;
            PersistorUtils.CreateAdapter(btc);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass,bool>(btc, x => x.TestBoolAction).ToString();
          

            CheckResults("BoolParameter", s);
        }

        [Test]
        public void NullableBoolParameter() {
            var mocks = new ContextMocks(controller);
            var btc = new BoolTestClass();
            mocks.ViewDataContainer.Object.ViewData.Model = btc;
            PersistorUtils.CreateAdapter(btc);
            string s = mocks.GetHtmlHelper<BoolTestClass>().ObjectActionAsDialog<BoolTestClass, bool?>(btc, x => x.TestNullableBoolAction).ToString();


            CheckResults("NullableBoolParameter", s);
        }

        [Test]
        public void ParameterWithHint() {
            var mocks = new ContextMocks(controller);

            var hint = (HintTestClass)GetBoundedInstance<HintTestClass>("HintTestClass").GetDomainObject();
            mocks.ViewDataContainer.Object.ViewData.Model = hint;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            FrameworkHelper.GetNakedObject(hint);

            string s = mocks.GetHtmlHelper<HintTestClass>().ObjectActionAsDialog<HintTestClass, int, int>(hint, x => x.ActionWithParms).ToString();

            CheckResults("ParameterWithHint", s);
        }



        [Test]
        public void ServiceHasNoVisibleFields() {
            var mocks = new ContextMocks(controller);
            INakedObject service = FrameworkHelper.GetAdaptedService("ClaimRepository");
            Assert.IsFalse(mocks.HtmlHelper.ObjectHasVisibleFields(service.Object));
        }

        [Test]
        public void ServiceList() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            string s = mocks.HtmlHelper.Services().ToString();
          

            CheckResults("ServiceList", s);
        }

        [Test]
        public void TransientWithCollection() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();


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
            var mocks = new ContextMocks(controller);


            ITestObject testObject = GetBoundedInstance<Currency>("EUR");
            var currency = (Currency) testObject.GetDomainObject();
            INakedObjectAction action1 = testObject.NakedObject.Specification.GetObjectActions().Single(a => a.Id == "UploadImage");
            INakedObjectAction action2 = testObject.NakedObject.Specification.GetObjectActions().Single(a => a.Id == "UploadFile");
            INakedObjectAction action3 = testObject.NakedObject.Specification.GetObjectActions().Single(a => a.Id == "UploadByteArray");

            string s = mocks.HtmlHelper.ParameterList(currency, action1).ToString() +
                       mocks.HtmlHelper.ParameterList(currency, action2) +
                       mocks.HtmlHelper.ParameterList(currency, action3);


          

            CheckResults("ObjectWithUploads", s);
        }

        [Test]
        public void UserMessages() {
            var mocks = new ContextMocks(controller);

            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofWarnings] = new[] {"Warning1", "Warning2"};
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofMessages] = new[] {"Message1", "Message2"};

            string s = mocks.HtmlHelper.UserMessages().ToString();
         

            CheckResults("NofValidationSummary", s);
        }

        [Test]
        public void TestClientValidationHtml() {

            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().PropertyListEdit(mocks.ViewDataContainer.Object.ViewData.Model).ToString();

            CheckResults("ClientValidationHtml", s);
        }

        [Test]
        public void TestClientValidationHtmlDialog() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData.Model = DescribedTestClass;
            string s = mocks.GetHtmlHelper<DescribedCustomHelperTestClass>().ObjectActionAsDialog<DescribedCustomHelperTestClass, string, int, string, string>(DescribedTestClass, x => x.TestClientValidationFunction).ToString();

            CheckResults("ClientValidationHtmlDialog", s);
        }

    }
}