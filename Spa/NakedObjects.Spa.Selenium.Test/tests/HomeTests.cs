// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium
{
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class HomeTestsRoot : AWTest
    {
        public virtual void WaitForSingleHome()
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

            //AssertHasFocus(menus[0]); //TODO: Test all focus separately
        }

        #region Clicking on menus and opening/closing dialogs

        public virtual void ClickOnVariousMenus()
        {
            GoToMenuFromHomePage("Customers");
            OpenSubMenu("Stores");
            OpenSubMenu("Individuals");
            AssertAction(0,"Find Customer By Account Number");
            AssertAction(1,"Find Store By Name");
            AssertAction(2,"Create New Store Customer");
            AssertAction(3,"Random Store");
            AssertAction(4,"Find Individual Customer By Name");
            AssertAction(5,"Create New Individual Customer");
            AssertAction(6,"Random Individual");
            AssertAction(7,"Customer Dashboard");
            AssertAction(8,"Throw Domain Exception");
            AssertAction(9,"Find Customer");


            GoToMenuFromHomePage("Sales");
            AssertAction(0, "Create New Sales Person");
            AssertAction(1, "Find Sales Person By Name");
            AssertAction(2, "List Accounts For Sales Person");
            AssertAction(3, "Random Sales Person");

            //AssertHasFocus(actions[0]);
        }


        public virtual void OpenAndCloseSubMenus()
        {
            GoToMenuFromHomePage("Customers");
            AssertActionNotDisplayed("Random Store");
            AssertActionNotDisplayed("Random Individual");
            OpenSubMenu("Stores");
            GetObjectAction("Random Store");
            AssertActionNotDisplayed("Random Individual");
            OpenSubMenu("Individuals");
            GetObjectAction("Random Store");
            GetObjectAction("Random Individual");
            CloseSubMenu("Stores");
            AssertActionNotDisplayed("Random Store");
            GetObjectAction("Random Individual");
            OpenActionDialog("Find Individual Customer By Name");
            AssertActionNotDisplayed("Random Store");

            // to check this behaviour has changed submenu remain open when dialog opened
            //AssertActionNotDisplayed("Random Individual");
        }
        public virtual void SelectSuccessiveDialogActionsThenCancel()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            OpenActionDialog("Customer Dashboard");
            OpenActionDialog("Find Customer By Account Number");
            CancelDialog();
        }
        #endregion

        #region Invoking main menu actions        
        public virtual void ZeroParamReturnsObject()
        {
            Url(CustomersMenuUrl);
            OpenSubMenu("Stores");
            Click(GetObjectAction("Random Store"));
            WaitForView(Pane.Single, PaneType.Object);

            var title = WaitForCss(".header .title");
            //AssertHasFocus(title);
        }

        public virtual void ZeroParamReturnsCollection()
        {
            Url(OrdersMenuUrl);
            WaitForCss(".actions .action", OrderServiceActions);
            Click(GetObjectAction("Highest Value Orders"));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
            WaitForCss(".reference", 20);

            var first = WaitForCssNo(".reference", 0);
            //AssertHasFocus(first); //TODO: test all focus separately
        }

        public virtual void ZeroParamThrowsError()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            Click(GetObjectAction("Throw Domain Exception"));
            var msg = WaitForCss(".error .message");
            Assert.AreEqual("Message: Foo", msg.Text);
        }

        public virtual void ZeroParamReturnsEmptyCollection()
        {
            Url(CustomersMenuUrl);
            OpenSubMenu("Individuals");
            Click(GetObjectAction("Find Individual Customer By Name"));
            ClearFieldThenType("#lastname1", "zzz");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Individual Customer By Name");
            var rows = br.FindElements(By.CssSelector("td"));
            Assert.AreEqual(0, rows.Count);
        }

        //Failing due to focus issue 
        public virtual void DialogActionOK()
        {
            Url(CustomersMenuUrl);
            WaitForCss(".actions .action", CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            var fieldCss = ".parameter:nth-child(1) input";
            //TODO: Test focus in separate tests; unreliable here
            //AssertHasFocus(WaitForCss(fieldCss));

            ClearFieldThenType(fieldCss, "AW00022262");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
        }
        #endregion
    }
    public abstract class HomeTests : HomeTestsRoot
    {

        [TestMethod]
        public override void WaitForSingleHome()
        {
            base.WaitForSingleHome();
        }

        #region Clicking on menus and opening/closing dialogs
        [TestMethod]
        public override void ClickOnVariousMenus()
        {
            base.ClickOnVariousMenus();
        }
        [TestMethod]
        public override void OpenAndCloseSubMenus()
        {
            base.OpenAndCloseSubMenus();
        }

        [TestMethod]
        public override void SelectSuccessiveDialogActionsThenCancel()
        {
            base.SelectSuccessiveDialogActionsThenCancel();
        }
        #endregion

        #region Invoking main menu actions
        [TestMethod]
        public override void ZeroParamReturnsObject()
        {
            base.ZeroParamReturnsObject();
        }

        [TestMethod]
        public override void ZeroParamReturnsCollection()
        {
            base.ZeroParamReturnsCollection();
        }

        [TestMethod]
        public override void ZeroParamThrowsError()
        {
            base.ZeroParamThrowsError();
        }

        [TestMethod]
        public override void ZeroParamReturnsEmptyCollection()
        {
            base.ZeroParamReturnsEmptyCollection();
        }

        [TestMethod] //Failing due to focus issue 
        public override void DialogActionOK()
        {
            base.DialogActionOK();
        }
        #endregion
    }
    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class HomeTestsIe : HomeTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class HomeTestsFirefox : HomeTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    // [TestClass, Ignore]
    public class HomeTestsChrome : HomeTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion

    [TestClass]
    public class MegaHomeTestFirefox : HomeTestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        [TestMethod]
        public virtual void MegaHomeTest()
        {
            WaitForSingleHome();
            ClickOnVariousMenus();
            OpenAndCloseSubMenus();
            SelectSuccessiveDialogActionsThenCancel();
            ZeroParamReturnsObject();
            ZeroParamReturnsCollection();
            ZeroParamThrowsError();
            ZeroParamReturnsEmptyCollection();
            DialogActionOK();
        }
    }
}