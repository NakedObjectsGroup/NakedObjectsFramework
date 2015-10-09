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
    //[TestClass]
    public abstract class ObjectViewTests : GeminiTest {
        [TestMethod]
        public virtual void FooterIcons() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            wait.Until(d => d.FindElement(By.CssSelector(".object")));

            Assert.IsTrue(br.FindElement(By.CssSelector(".view")).Displayed);
            WaitFor(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
        }

        [TestMethod]
        public virtual void Actions() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            Assert.IsTrue(br.FindElement(By.CssSelector(".view")).Displayed);
            var actions = GetObjectActions();

            Assert.AreEqual("Create New Address", actions[0].Text);
            Assert.AreEqual("Create New Contact", actions[1].Text);
            Assert.AreEqual("Create New Order", actions[2].Text);
            Assert.AreEqual("Quick Order", actions[3].Text);
            Assert.AreEqual("Search For Orders", actions[4].Text);
            Assert.AreEqual("Last Order", actions[5].Text);
            Assert.AreEqual("Open Orders", actions[6].Text);
            Assert.AreEqual("Recent Orders", actions[7].Text);
        }

        [TestMethod]
        public virtual void PropertiesAndCollections() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            Assert.IsTrue(br.FindElement(By.CssSelector(".view")).Displayed);

            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 6);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Store Name:\r\nTwin Cycles", properties[0].Text);
            Assert.AreEqual("Demographics:\r\nAnnualSales: 800000 AnnualRevenue: 80000 BankName: International Security BusinessType: BM YearOpened: 1988 Specialty: Touring SquareFeet: 21000 Brands: AW Internet: T1 NumberEmployees: 11", properties[1].Text);
            Assert.AreEqual("Sales Person:\r\nLynn Tsoflias", properties[2].Text);
            Assert.IsTrue(properties[3].Text.StartsWith("Modified Date:\r\n13 Oct 2004"));
            Assert.AreEqual("Account Number:\r\nAW00000555", properties[4].Text);
            Assert.AreEqual("Sales Territory:\r\nAustralia", properties[5].Text);

            ReadOnlyCollection<IWebElement> collections = br.FindElements(By.CssSelector(".collection"));

            Assert.AreEqual("Addresses:\r\n1-Customer Addresses", collections[0].Text);
            Assert.AreEqual("Contacts:\r\n1-Store Contacts", collections[1].Text);
        }

        [TestMethod]
        public virtual void ClickReferenceProperty() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            var reference = FindElementByCss(".property .reference", 0);
            Click(reference);
            WaitFor(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }

        [TestMethod]
        public virtual void OpenCollectionAsList() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);

            wait.Until(d => d.FindElements(By.CssSelector(".collection")).Count == StoreCollections);
            var iconList = FindElementByCss(".icon-list", 0);

            Click(iconList);

            wait.Until(d => d.FindElement(By.TagName("table")));

            // cancel table view 
            Click(br.FindElement(By.CssSelector(".icon-summary")));

            WaitUntilGone(d => d.FindElement(By.CssSelector(".table")));
        }

        [TestMethod]
        public virtual void ClickOnLineItemWithCollectionAsList()
        {
            var testUrl = Url + "#/gemini/object?object1=AdventureWorksModel.Store-555&collection1_Addresses=List";
            br.Navigate().GoToUrl(testUrl);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table .reference")));
            var title = row.Text;
            Click(row);
            WaitFor(Pane.Single, PaneType.Object, title);
        }

        [TestMethod]
        public virtual void ClickOnLineItemWithCollectionAsTable()
        {
            var testUrl = Url + "#/gemini/object?object1=AdventureWorksModel.Store-555&collection1_Addresses=Table";
            br.Navigate().GoToUrl(testUrl);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            wait.Until(dr => row.FindElements(By.CssSelector(".cell")).Count >= 2);

            var type = row.FindElements(By.CssSelector(".cell"))[0].Text;
            var addr = row.FindElements(By.CssSelector(".cell"))[1].Text;
            Click(row);
            WaitFor(Pane.Single, PaneType.Object, type+": "+addr);
        }

        [TestMethod]
        public virtual void AttachmentProperty() {
            br.Navigate().GoToUrl(Product968Url);
            wait.Until(d => d.FindElements(By.CssSelector(".property")).Count == ProductProperties);
            wait.Until(d => d.FindElements(By.CssSelector(".property  a > img")).Count == 1);
            Assert.IsTrue(br.FindElement(By.CssSelector(".property  a > img")).GetAttribute("src").Length > 0); 
        }

        [TestMethod]
        public virtual void DialogAction() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            OpenActionDialog("Search For Orders");
        }

        [TestMethod]
        public virtual void DialogActionOk() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);

            var dialog = OpenActionDialog("Search For Orders");

            dialog.FindElements(By.CssSelector(".parameter .value input"))[0].SendKeys("1 Jan 2003");
            dialog.FindElements(By.CssSelector(".parameter .value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Search For Orders");
        }

        [TestMethod]
        public virtual void ObjectAction() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            Click(GetObjectAction("Last Order"));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
        }

        [TestMethod]
        public virtual void CollectionAction() {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            Click(GetObjectAction("Recent Orders"));
            WaitFor(Pane.Single, PaneType.Query, "Recent Orders");
        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ObjectViewTestsIe : ObjectViewTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class ObjectViewTestsFirefox : ObjectViewTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
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
            GeminiTest.InitialiseClass(context);
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