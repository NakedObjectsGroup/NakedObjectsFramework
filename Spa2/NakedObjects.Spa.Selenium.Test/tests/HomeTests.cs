// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class HomeTestsRoot : AWTest {
        public virtual void WaitForSingleHome() {
            WaitForView(Pane.Single, PaneType.Home, "Home");
            WaitForCss(".main-column");
            var menus = WaitForCss("nof-menu-bar nof-action input", MainMenusCount);
            Assert.AreEqual("Customers", menus[0].GetAttribute("value"));
            Assert.AreEqual("Orders", menus[1].GetAttribute("value"));
            Assert.AreEqual("Products", menus[2].GetAttribute("value"));
            Assert.AreEqual("Employees", menus[3].GetAttribute("value"));
            Assert.AreEqual("Sales", menus[4].GetAttribute("value"));
            Assert.AreEqual("Special Offers", menus[5].GetAttribute("value"));
            Assert.AreEqual("Contacts", menus[6].GetAttribute("value"));
            Assert.AreEqual("Vendors", menus[7].GetAttribute("value"));
            Assert.AreEqual("Purchase Orders", menus[8].GetAttribute("value"));
            Assert.AreEqual("Work Orders", menus[9].GetAttribute("value"));
            AssertFooterExists();

            //AssertHasFocus(menus[0]); //TODO: Test all focus separately
        }

        #region Clicking on menus and opening/closing dialogs

        public virtual void ClickOnVariousMenus() {
            GoToMenuFromHomePage("Customers");
            OpenSubMenu("Stores");
            OpenSubMenu("Individuals");
            AssertAction(0, "Find Customer By Account Number");
            AssertAction(1, "Find Store By Name");
            AssertAction(2, "Create New Store Customer");
            AssertAction(3, "Random Store");
            AssertAction(4, "Find Individual Customer By Name");
            AssertAction(5, "Create New Individual Customer");
            AssertAction(6, "Random Individual");
            AssertAction(7, "Customer Dashboard");
            AssertAction(8, "Throw Domain Exception");
            AssertAction(9, "Find Customer");

            GoToMenuFromHomePage("Sales");
            AssertAction(0, "Create New Sales Person");
            AssertAction(1, "Find Sales Person By Name");
            AssertAction(2, "List Accounts For Sales Person");
            AssertAction(3, "Random Sales Person");

            //AssertHasFocus(actions[0]);
        }

        public virtual void OpenAndCloseSubMenus() {
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

            //Now for sub-sub-menus
            GoToMenuFromHomePage("Sales");
            AssertActionNotDisplayed("Action1");
            OpenSubMenu("Sub Menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            OpenSubMenu("Level 2 sub menu");
            GetObjectAction("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            OpenSubMenu("Level 3 sub menu");
            GetObjectAction("Action3");
            GetObjectAction("Action4");
            CloseSubMenu("Level 3 sub menu");
            GetObjectAction("Action1");
            GetObjectAction("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            CloseSubMenu("Level 2 sub menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            CloseSubMenu("Sub Menu");
            AssertActionNotDisplayed("Action1");
            AssertActionNotDisplayed("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
        }

        public virtual void SelectSuccessiveDialogActionsThenCancel() {
            Url(CustomersMenuUrl, true);
            WaitForCss("nof-action-list nof-action", CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            //CancelDialog();
            OpenActionDialog("Customer Dashboard");
            //CancelDialog();
            OpenActionDialog("Find Customer By Account Number");
            CancelDialog();
        }

        #endregion

        #region Invoking main menu actions        

        public virtual void ZeroParamReturnsObject() {
            Url(CustomersMenuUrl);
            OpenSubMenu("Stores");
            Click(GetObjectAction("Random Store"));
            WaitForView(Pane.Single, PaneType.Object);

            var title = WaitForCss(".header .title");
            //AssertHasFocus(title);
        }

        public virtual void ZeroParamReturnsCollection() {
            Url(OrdersMenuUrl);
            WaitForCss("nof-action-list nof-action", OrderServiceActions);
            Click(GetObjectAction("Highest Value Orders"));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
            WaitForCss(".reference", 20);

            var first = WaitForCssNo(".reference", 0);
            //AssertHasFocus(first); //TODO: test all focus separately
        }

        public virtual void ZeroParamReturnsEmptyCollection() {
            Url(CustomersMenuUrl);
            OpenSubMenu("Individuals");
            Click(GetObjectAction("Find Individual Customer By Name"));
            ClearFieldThenType("#lastname1", "zzz");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Individual Customer By Name");
            var rows = br.FindElements(By.CssSelector("td"));
            Assert.AreEqual(0, rows.Count);
        }

        public virtual void DialogActionOK() {
            Url(CustomersMenuUrl);
            WaitForCss("nof-action-list nof-action", CustomerServiceActions);
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

    public abstract class HomeTests : HomeTestsRoot {
        [TestMethod]
        public override void WaitForSingleHome() {
            base.WaitForSingleHome();
        }

        #region Clicking on menus and opening/closing dialogs

        [TestMethod]
        public override void ClickOnVariousMenus() {
            base.ClickOnVariousMenus();
        }

        [TestMethod]
        public override void OpenAndCloseSubMenus() {
            base.OpenAndCloseSubMenus();
        }

        [TestMethod]
        public override void SelectSuccessiveDialogActionsThenCancel() {
            base.SelectSuccessiveDialogActionsThenCancel();
        }

        #endregion

        #region Invoking main menu actions

        [TestMethod]
        public override void ZeroParamReturnsObject() {
            base.ZeroParamReturnsObject();
        }

        [TestMethod]
        public override void ZeroParamReturnsCollection() {
            base.ZeroParamReturnsCollection();
        }

        [TestMethod]
        public override void ZeroParamReturnsEmptyCollection() {
            base.ZeroParamReturnsEmptyCollection();
        }

        [TestMethod] //Failing due to focus issue 
        public override void DialogActionOK() {
            base.DialogActionOK();
        }

        #endregion
    }



    public class MegaHomeTestBase : HomeTestsRoot {
        [TestMethod] //Mega
        public virtual void MegaHomeTest() {
            WaitForSingleHome();
            ClickOnVariousMenus();
            OpenAndCloseSubMenus();
            SelectSuccessiveDialogActionsThenCancel();
            ZeroParamReturnsObject();
            ZeroParamReturnsCollection();
            ZeroParamReturnsEmptyCollection();
            DialogActionOK();
        }
    }

    //[TestClass]
    public class MegaHomeTestFirefox : MegaHomeTestBase {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }

    //[TestClass]
    public class MegaHomeTestIe : MegaHomeTestBase {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }

    [TestClass]
    public class MegaHomeTestChrome : MegaHomeTestBase {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }
}