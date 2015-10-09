// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class HomeTests : GeminiTest {

        [TestMethod]
        public void WaitForSingleHome()
        {
            WaitFor(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);

            Assert.IsNotNull(br.FindElement(By.CssSelector(".main-column")));

            ReadOnlyCollection<IWebElement> menus = br.FindElements(By.CssSelector(".menu"));
            Assert.AreEqual("Customers", menus[0].Text);
            Assert.AreEqual("Orders", menus[1].Text);
            Assert.AreEqual("Products", menus[2].Text);
            Assert.AreEqual("Employees", menus[3].Text);
            Assert.AreEqual("Sales", menus[4].Text);
            Assert.AreEqual("Special Offers", menus[5].Text);
            Assert.AreEqual("Contacts", menus[6].Text);
            Assert.AreEqual("Vendors", menus[7].Text);
            Assert.AreEqual("Purchase Orders", menus[8].Text);
            Assert.AreEqual("Work Orders", menus[9].Text);
            AssertFooterExists();
        }

        #region Clicking on menus and opening/closing dialogs
        [TestMethod]
        public virtual void ClickOnVariousMenus() {
            GoToMenuFromHomePage("Customers");

            wait.Until(d => d.FindElement(By.CssSelector(".actions")));
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == 9);
            var actions = br.FindElements(By.CssSelector(".action"));

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Assert.AreEqual("Create New Store Customer", actions[2].Text);
            Assert.AreEqual("Random Store", actions[3].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            Assert.AreEqual("Random Individual", actions[6].Text);
            Assert.AreEqual("Customer Dashboard", actions[7].Text);
            Assert.AreEqual("Throw Domain Exception", actions[8].Text);

            GoToMenuFromHomePage("Sales");

            wait.Until(d => d.FindElement(By.CssSelector(".actions")));
            actions = br.FindElements(By.CssSelector(".action"));
            Assert.AreEqual(4, actions.Count);

            Assert.AreEqual("Create New Sales Person", actions[0].Text);
            Assert.AreEqual("Find Sales Person By Name", actions[1].Text);
            Assert.AreEqual("List Accounts For Sales Person", actions[2].Text);
            Assert.AreEqual("Random Sales Person", actions[3].Text);
        }

        [TestMethod]
        public virtual void SelectSuccessiveDialogActionsThenCancel()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);

            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            OpenActionDialog("Find Store By Name");
            OpenActionDialog("Find Customer By Account Number");
            CancelDialog();
        }
        #endregion

        #region Invoking main menu actions
        [TestMethod, Ignore]
        public virtual void ZeroParamReturnsObject()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);
            Click(GetObjectAction("Random Store"));
            wait.Until(dr => dr.FindElement(By.CssSelector(".single .object")));
        }

        [TestMethod, Ignore]
        public virtual void ZeroParamReturnsCollection()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == OrderServiceActions);
            Click(GetObjectAction("Highest Value Orders"));
            WaitFor(Pane.Single, PaneType.Query, "Highest Value Orders");
            wait.Until(d => d.FindElements(By.CssSelector(".reference")).Count == 20);
        }

        [TestMethod, Ignore]
        public virtual void ZeroParamThrowsError()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            Click(GetObjectAction("Throw Domain Exception"));
            var msg = wait.Until(d => d.FindElement(By.CssSelector(".error .message")));
            Assert.AreEqual("Foo", msg.Text);
        }

        [TestMethod, Ignore]
        public virtual void ZeroParamReturnsEmptyCollection()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == OrderServiceActions);
            Click(GetObjectAction("Orders In Process"));
            WaitFor(Pane.Single, PaneType.Query, "Orders In Process");
            var rows = br.FindElements(By.CssSelector("td"));
            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod, Ignore]
        public virtual void DialogActionOK()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            FindElementByCss(".value  input").SendKeys("00022262");
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
        }
        #endregion
    }

    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class HomeTestsIe : HomeTests {
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

    [TestClass]
    public class HomeTestsFirefox : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

   // [TestClass, Ignore]
    public class HomeTestsChrome : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}