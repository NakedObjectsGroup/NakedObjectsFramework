// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class ObjectViewTests : AWTest {

        [TestMethod]
        public virtual void Actions() {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            var actions = GetObjectActions(6);

            //Assert.AreEqual("Create New Address", actions[0].Text);
           // Assert.AreEqual("Create New Contact", actions[1].Text);
            Assert.AreEqual("Create New Order", actions[0].Text);
            Assert.AreEqual("Quick Order", actions[1].Text);
            Assert.AreEqual("Search For Orders", actions[2].Text);
            Assert.AreEqual("Last Order", actions[3].Text);
            Assert.AreEqual("Open Orders", actions[4].Text);
            Assert.AreEqual("Recent Orders", actions[5].Text);
        }

        [TestMethod]
        public virtual void PropertiesAndCollections() {
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&actions1=open");
            wait.Until(dr => dr.FindElement(By.CssSelector(".object")));
            wait.Until(dr => dr.FindElement(By.CssSelector(".view")).Displayed == true);
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 4);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Store Name:\r\nTwin Cycles", properties[0].Text);
            Assert.AreEqual("Demographics:\r\nAnnualSales: 800000 AnnualRevenue: 80000 BankName: International Security BusinessType: BM YearOpened: 1988 Specialty: Touring SquareFeet: 21000 Brands: AW Internet: T1 NumberEmployees: 11", properties[1].Text);
            Assert.AreEqual("Sales Person:\r\nLynn Tsoflias", properties[2].Text);
            Assert.IsTrue(properties[3].Text.StartsWith("Modified Date:\r\n13 Oct 2008"));

            wait.Until(d => br.FindElements(By.CssSelector(".collection")).Count >= 2);
            ReadOnlyCollection<IWebElement> collections = br.FindElements(By.CssSelector(".collection"));
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[0].Text == "Addresses:\r\n1 Item(s)");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[1].Text == "Contacts:\r\n1 Item(s)");
        }

        [TestMethod]
        public virtual void ClickReferenceProperty() {
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&actions1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
            Click(reference);
            WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }

        [TestMethod]
        public virtual void OpenCollectionAsList() {
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&actions1=open");
            WaitForCss(".collection", 2);
            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);

            WaitForCss("table");

            // cancel table view 
            Click(WaitForCss(".icon-summary"));
            WaitUntilGone(d => d.FindElement(By.CssSelector(".table")));
        }

        [TestMethod]
        public virtual void ClickOnLineItemWithCollectionAsList()
        {
            var testUrl = GeminiBaseUrl + "object?object1=AdventureWorksModel.Store-350&actions1=open" +"&collection1_Addresses=List";
            Url(testUrl);
            var row = WaitForCss("table .reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "Main Office: 2253-217 Palmer Street ...");
        }

        [TestMethod]
        public virtual void ClickOnLineItemWithCollectionAsTable()
        {
            var testUrl = GeminiBaseUrl + "object?object1=AdventureWorksModel.Store-350&actions1=open" + "&collection1_Addresses=Table";
            Url(testUrl);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            wait.Until(dr => row.FindElements(By.CssSelector(".cell")).Count >= 2);

            var type = row.FindElements(By.CssSelector(".cell"))[0].Text;
            var addr = row.FindElements(By.CssSelector(".cell"))[1].Text;
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, type+": "+addr);
        }

        [TestMethod]
        public virtual void AttachmentProperty() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-968");
            wait.Until(d => d.FindElements(By.CssSelector(".property")).Count == 23);
            wait.Until(d => d.FindElements(By.CssSelector(".property  a > img")).Count == 1);
            Assert.IsTrue(br.FindElement(By.CssSelector(".property  a > img")).GetAttribute("src").Length > 0); 
        }

        #region Actions
        [TestMethod]
        public virtual void DialogAction() {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");
            OpenActionDialog("Search For Orders");
        }

        [TestMethod]
        public virtual void DialogActionOk() {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");

            var dialog = OpenActionDialog("Search For Orders");

            dialog.FindElements(By.CssSelector(".parameter .value input"))[0].SendKeys("1 Jan 2003");
            dialog.FindElements(By.CssSelector(".parameter .value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Search For Orders");
        }

        [TestMethod]
        public virtual void ObjectAction() {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");
            Click(GetObjectAction("Last Order"));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
        }

        [TestMethod]
        public virtual void CollectionAction() {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");
            Click(GetObjectAction("Recent Orders"));
            WaitForView(Pane.Single, PaneType.List, "Recent Orders");
        }

        [TestMethod]
        public virtual void DescriptionRenderedAsTooltip()
        {
            GeminiUrl("home?menu1=SalesRepository");
            var a = GetObjectAction("Create New Sales Person");
            Assert.AreEqual("... from an existing Employee", a.GetAttribute("title"));
        }

        [TestMethod]
        public void DisabledAction()
        {
            GeminiUrl("object?object1=AdventureWorksModel.SalesOrderHeader-43893&actions1=open");
            //First the control test
            GetObjectAction("Add New Sales Reason").AssertIsEnabled();

            //Then the real test
            GetObjectAction("Add New Detail").AssertIsDisabled("Can only add to 'In Process' order");
        }


        [TestMethod]
        public void ActionsMenuDisabledOnObjectWithNoActions()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Address-21467");
            WaitForView(Pane.Single, PaneType.Object, "3022 Terra Calitina ...");
            var actions =wait.Until(dr => dr.FindElement(By.CssSelector(".header .menu")));
            Assert.AreEqual("true", actions.GetAttribute("disabled"));
        }
        #endregion



        [TestMethod, Ignore] //Failing on server
        public void QueryOnlyActionDoesNotReloadAutomatically()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-8410&actions1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(6) .value").Text;   
            var dialog = OpenActionDialog("Update Suffix"); //This is deliberately wrongly marked up as QueryOnly
            var field1 = WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue );
            Click(OKButton()); //This will have updated server, but not client-cached object
            //Go and do something else, so screen changes, then back again
            wait.Until(dr => dr.FindElements(By.CssSelector(".dialog")).Count == 0);
            GeminiUrl("");
            WaitForView(Pane.Single, PaneType.Home);
            Click(br.FindElement(By.CssSelector(".icon-back")));
            WaitForView(Pane.Single, PaneType.Object);
            var valueNow = WaitForCss(".property:nth-child(6) .value");
            Assert.AreEqual(original, valueNow.Text);
            Reload();
            GeminiUrl("");
            WaitForView(Pane.Single, PaneType.Home);
            Click(br.FindElement(By.CssSelector(".icon-back")));
            valueNow = WaitForCss(".property:nth-child(6) .value");
            Assert.AreEqual(newValue, valueNow.Text);
       }

        [TestMethod]
        public void PotentActionDoesReloadAutomatically()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Person-8410&actions1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(3) .value").Text;
            var dialog = OpenActionDialog("Update Middle Name"); //This is deliberately wrongly marked up as QueryOnly
            var field1 = WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue);
            Click(OKButton()); 
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(3) .value")).Text == newValue);
        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ObjectViewTestsIe : ObjectViewTests {
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
    public class ObjectViewTestsFirefox : ObjectViewTests {
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
    public class ObjectViewTestsChrome : ObjectViewTests {
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