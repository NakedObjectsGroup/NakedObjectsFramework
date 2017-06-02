// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class FooterTestsRoot : AWTest {
        public virtual void Home() {
            Debug.WriteLine(nameof(Home));
            GeminiUrl("object?o1=___1.Product--968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(br.FindElement(By.CssSelector(".icon.home")));
            WaitForView(Pane.Single, PaneType.Home, "Home");
        }

        public virtual void BackAndForward() {
            Debug.WriteLine(nameof(BackAndForward));
            Url(BaseUrl);
            GoToMenuFromHomePage("Orders");
            Click(GetObjectAction("Random Order"));
            WaitForView(Pane.Single, PaneType.Object);
            var orderTitle = WaitForCss(".title").Text;
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Home);
            ClickForwardButton();
            WaitForView(Pane.Single, PaneType.Object, orderTitle);
            EditObject();
            WaitForView(Pane.Single, PaneType.Object, "Editing - " + orderTitle);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Home);
            ClickForwardButton();
            WaitForView(Pane.Single, PaneType.Object, "Editing - " + orderTitle);
            Click(GetCancelEditButton());
            WaitForView(Pane.Single, PaneType.Object, orderTitle);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Home);
            ClickForwardButton();
            WaitForView(Pane.Single, PaneType.Object, orderTitle);

            var link = GetReferenceFromProperty("Customer");
            var cusTitle = link.Text;
            Click(link);
            WaitForView(Pane.Single, PaneType.Object, cusTitle);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Object, orderTitle);
            ClickForwardButton();
            WaitForView(Pane.Single, PaneType.Object, cusTitle);
            OpenObjectActions();
            OpenSubMenu("Orders");
            OpenActionDialog("Create New Order");
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Object, cusTitle);
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Object, orderTitle);
        }

        public virtual void RecentObjects() {
            Debug.WriteLine(nameof(RecentObjects));
            GeminiUrl("home?m1=CustomerRepository&d1=FindCustomerByAccountNumber&f1_accountNumber=%22AW%22");
            ClearFieldThenType("#accountnumber1", "AW00000042");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Healthy Activity Store, AW00000042");
            ClickHomeButton();
            GoToMenuFromHomePage("Customers");
            OpenActionDialog("Find Customer By Account Number");
            ClearFieldThenType("#accountnumber1", "AW00000359");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Mechanical Sports Center, AW00000359");
            ClickHomeButton();
            GoToMenuFromHomePage("Customers");
            OpenActionDialog("Find Customer By Account Number");
            ClearFieldThenType("#accountnumber1", "AW00022262");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
            ClickHomeButton();
            GoToMenuFromHomePage("Products");
            Click(GetObjectAction("Find Product By Number"));
            ClearFieldThenType("#number1", "LJ-0192-S");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Long-Sleeve Logo Jersey, S");
            ClickRecentButton();
            WaitForView(Pane.Single, PaneType.Recent);
            var el = WaitForCssNo("tr td:nth-child(1)", 0);
            Assert.AreEqual("Long-Sleeve Logo Jersey, S", el.Text);
            el = WaitForCssNo("tr td:nth-child(1)", 1);
            Assert.AreEqual("Marcus Collins, AW00022262", el.Text);
            el = WaitForCssNo("tr td:nth-child(1)", 2);
            Assert.AreEqual("Mechanical Sports Center, AW00000359", el.Text);
            el = WaitForCssNo("tr td:nth-child(1)", 3);
            Assert.AreEqual("Healthy Activity Store, AW00000042", el.Text);

            //Test left- and right-click navigation from Recent
            el = WaitForCssNo("tr td:nth-child(1)", 0);
            Assert.AreEqual("Long-Sleeve Logo Jersey, S", el.Text);
            RightClick(el);
            WaitForView(Pane.Right, PaneType.Object, "Long-Sleeve Logo Jersey, S");
            WaitForView(Pane.Left, PaneType.Recent);
            el = WaitForCssNo("tr td:nth-child(1)", 1);
            Assert.AreEqual("Marcus Collins, AW00022262", el.Text);
            Click(el);
            WaitForView(Pane.Left, PaneType.Object, "Marcus Collins, AW00022262");

            //Test that clear button works
            ClickRecentButton();
            WaitForView(Pane.Left, PaneType.Recent);
            WaitForCss("tr td:nth-child(1)", 6);
            var clear = GetInputButton("Clear", Pane.Left).AssertIsEnabled();
            Click(clear);
            GetInputButton("Clear", Pane.Left).AssertIsDisabled();
            WaitForCss("tr td", 0);
        }

        public virtual void ApplicationProperties() {
            Debug.WriteLine(nameof(ApplicationProperties));
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            ClickPropertiesButton();
            WaitForView(Pane.Single, PaneType.Properties, "Application Properties");
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 5);
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[3].Text.StartsWith("Server API version: 8.1.0"));
            var properties = br.FindElements(By.CssSelector(".property"));
            Assert.IsTrue(properties[0].Text.StartsWith("Application Name:"), properties[0].Text);
            Assert.IsTrue(properties[1].Text.StartsWith("User Name:"), properties[1].Text);
            Assert.IsTrue(properties[2].Text.StartsWith("Server Url: http"), properties[2].Text); // maybe https
            Assert.IsTrue(properties[3].Text.StartsWith("Server API version: 8.1.0"), properties[3].Text);
            Assert.IsTrue(properties[4].Text.StartsWith("Client version:"), properties[4].Text);
        }

        public virtual void LogOff() {
            Debug.WriteLine(nameof(LogOff));
            GeminiUrl("home");
            ClickLogOffButton();
            wait.Until(d => br.FindElement(By.CssSelector(".title")).Text.StartsWith("Log Off"));
            var cancel = wait.Until(dr => dr.FindElement(By.CssSelector("button[value='Cancel']")));
            Click(cancel);
            WaitForView(Pane.Single, PaneType.Home);
        }

        #region WarningsAndInfo

        public virtual void ExplicitWarningsAndInfo() {
            Debug.WriteLine(nameof(ExplicitWarningsAndInfo));
            GeminiUrl("home?m1=WorkOrderRepository");
            Click(GetObjectAction("Generate Info And Warning"));
            var warn = WaitForCss(".footer .warnings");
            Assert.AreEqual("Warn User of something else", warn.Text);
            var msg = WaitForCss(".footer .messages");
            Assert.AreEqual("Inform User of something", msg.Text);

            //Test that both are cleared by next action
            Click(GetObjectAction("Random Work Order"));
            WaitUntilElementDoesNotExist(".footer .warnings");
            WaitUntilElementDoesNotExist(".footer .messages");
        }

        public virtual void ZeroParamActionReturningNullGeneratesGenericWarning() {
            Debug.WriteLine(nameof(ZeroParamActionReturningNullGeneratesGenericWarning));
            GeminiUrl("home?m1=EmployeeRepository");
            Click(GetObjectAction("Me"));
            WaitForTextEquals(".footer .warnings", "no result found");
            Click(GetObjectAction("My Departmental Colleagues"));
            WaitForTextEquals(".footer .warnings", "Current user unknown");
        }

        #endregion
    }

    public abstract class FooterTests : FooterTestsRoot {
        [TestMethod]
        public override void Home() {
            base.Home();
        }

        [TestMethod]
        public override void BackAndForward() {
            base.BackAndForward();
        }

        [TestMethod]
        public override void RecentObjects() {
            base.RecentObjects();
        }

        [TestMethod]
        public override void ApplicationProperties() {
            base.ApplicationProperties();
        }

        [TestMethod]
        public override void LogOff() {
            base.LogOff();
        }

        #region Warnings and Info

        [TestMethod]
        public override void ExplicitWarningsAndInfo() {
            base.ExplicitWarningsAndInfo();
        }

        [TestMethod]
        public override void ZeroParamActionReturningNullGeneratesGenericWarning() {
            base.ZeroParamActionReturningNullGeneratesGenericWarning();
        }

        #endregion
    }



    #region Mega tests

    public abstract class MegaFooterTestsRoot : FooterTestsRoot {
        [TestMethod] //Mega
        public void MegaFooterTest() {
            ExplicitWarningsAndInfo();
            ZeroParamActionReturningNullGeneratesGenericWarning();
            Home();
            BackAndForward();
            RecentObjects();
            ApplicationProperties();
            LogOff();
        }
    }

    //[TestClass]
    public class MegaFooterTestsFirefox : MegaFooterTestsRoot {
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
    public class MegaFooterTestsIe : MegaFooterTestsRoot {
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
    public class MegaFooterTestsChrome : MegaFooterTestsRoot {
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

    #endregion
}