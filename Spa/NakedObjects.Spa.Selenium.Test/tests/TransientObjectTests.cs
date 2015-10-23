// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class TransientObjectTests : AWTest {

        [TestMethod] //Complete test & guard against duplicate keys?
        public void CreateAndSaveTransientObject()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-12043&actions1=open");
            Click(GetObjectAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype", "Vista");
            string number = DateTime.Now.Ticks.ToString(); //pseudo-random string
            TypeIntoField("#cardnumber", number);
            SelectDropDownOnField("#expmonth","12");
            SelectDropDownOnField("#expyear","2020");
            Click(GetSaveButton()); //TODO: Test that it has in fact saved the object
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        }

        [TestMethod]
        public void MissingMandatoryFieldsNotified()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-12043&actions1=open");
            Click(GetObjectAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype", "Vista");
            SelectDropDownOnField("#expyear", "2020");
            Click(GetSaveButton());
            wait.Until(dr =>dr.FindElements(By.CssSelector(".validation")).Count(e => e.Text == "Mandatory") ==2);
        }

        [TestMethod]
        public virtual void PropertyDescriptionAndRequiredRenderedAsPlacholder()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-12043&actions1=open");
            Click(GetObjectAction("Create New Credit Card"));
            var name = WaitForCss("input#cardnumber");
            Assert.AreEqual("* Without spaces", name.GetAttribute("placeholder"));
        }

        [TestMethod]
        public void CancelTransientObject()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-12043&actions1=open");
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
            Click(GetObjectAction("Create New Credit Card"));
            Click(GetCancelEditButton());
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        }

        [TestMethod, Ignore] //cross-field validation not working
        public void MultiFieldValidation()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-12043&actions1=open");
            Click(GetObjectAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype", "Vista");
            TypeIntoField("#cardnumber", "1111222233334444");
            SelectDropDownOnField("#expmonth", "1");
            SelectDropDownOnField("#expyear", "2008");
            Click(GetSaveButton());
            //TODO: Test that dave has failed and message is shown

        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class TransientObjectTestsIe : TransientObjectTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class TransientObjectTestsFirefox : TransientObjectTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    //[TestClass, Ignore]
    public class TransientObjectTestsChrome : TransientObjectTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    #endregion
}