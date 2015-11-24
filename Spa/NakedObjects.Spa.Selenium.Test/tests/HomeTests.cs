// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class HomeTests : AWTest
    {

        [TestMethod]
        public void WaitForSingleHome()
        {
            WaitForView(Pane.Single, PaneType.Home, "Home");
            WaitForCss(".main-column");
            var menus = WaitForCss(".menu", MainMenusCount);
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

            AssertHasFocus(menus[0]);
        }

        #region Clicking on menus and opening/closing dialogs
        [TestMethod]
        public virtual void ClickOnVariousMenus()
        {
            GoToMenuFromHomePage("Customers");
            var actions = WaitForCss(".actions .action", CustomerServiceActions);

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Assert.AreEqual("Create New Store Customer", actions[2].Text);
            Assert.AreEqual("Random Store", actions[3].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            Assert.AreEqual("Random Individual", actions[6].Text);
            Assert.AreEqual("Customer Dashboard", actions[7].Text);
            Assert.AreEqual("Throw Domain Exception", actions[8].Text);

            AssertHasFocus(actions[0]);

            GoToMenuFromHomePage("Sales");
            actions = WaitForCss(".actions .action", 4); ;
            Assert.AreEqual("Create New Sales Person", actions[0].Text);
            Assert.AreEqual("Find Sales Person By Name", actions[1].Text);
            Assert.AreEqual("List Accounts For Sales Person", actions[2].Text);
            Assert.AreEqual("Random Sales Person", actions[3].Text);

            AssertHasFocus(actions[0]);
        }

        [TestMethod]
        public virtual void SelectSuccessiveDialogActionsThenCancel()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            OpenActionDialog("Find Store By Name");
            OpenActionDialog("Find Customer By Account Number");
            CancelDialog();
        }
        #endregion

        #region Invoking main menu actions
        [TestMethod]
        public virtual void ZeroParamReturnsObject()
        {
            Url(CustomersMenuUrl);
            Click(GetObjectAction("Random Store"));
            WaitForView(Pane.Single, PaneType.Object);

            var title = WaitForCss(".header .title");
            AssertHasFocus(title);
        }

        [TestMethod]
        public virtual void ZeroParamReturnsCollection()
        {
            Url(OrdersMenuUrl);
            WaitForCss(".actions .action", OrderServiceActions);
            Click(GetObjectAction("Highest Value Orders"));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
            WaitForCss(".reference", 20);

            var first = WaitForCssNo(".reference", 0);
            AssertHasFocus(first);
        }

        [TestMethod]
        public virtual void ZeroParamThrowsError()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            Click(GetObjectAction("Throw Domain Exception"));
            var msg = WaitForCss(".error .message");
            Assert.AreEqual("Foo", msg.Text);
        }

        [TestMethod]
        public virtual void ZeroParamReturnsEmptyCollection()
        {
            Url(OrdersMenuUrl);
            WaitForCss(".actions .action", OrderServiceActions);
            Click(GetObjectAction("Orders In Process"));
            WaitForView(Pane.Single, PaneType.List, "Orders In Process");
            var rows = br.FindElements(By.CssSelector("td"));
            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public virtual void DialogActionOK()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");

            //Check focus
            var fieldCss = ".parameter:nth-child(1) input";
           AssertHasFocus(WaitForCss(fieldCss));

            TypeIntoField(fieldCss, Keys.ArrowRight+Keys.ArrowRight+"00022262");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
        }
        #endregion
    }
    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class HomeTestsIe : HomeTests {
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
    public class HomeTestsFirefox : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
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
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
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