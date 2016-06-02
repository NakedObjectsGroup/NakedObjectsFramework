// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;

namespace NakedObjects.Web.UnitTests.Selenium
{
    public abstract class FooterTestsRoot : AWTest
    {
        public virtual void Home()
        {
            GeminiUrl("object?o1=___1.Product--968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(br.FindElement(By.CssSelector(".icon-home")));
            WaitForView(Pane.Single, PaneType.Home, "Home");
        }
        public virtual void BackAndForward()
        {
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
            WaitForView(Pane.Single, PaneType.Object, "Editing - "+orderTitle);
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
            WaitForView(Pane.Single, PaneType.Object, orderTitle);

        }
        public virtual void Cicero()
        {
            GeminiUrl("object?o1=___1.Product--968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54"); //Cicero
            GeminiUrl("object/list?o1=___1.Store--350&m2=OrderRepository&a2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Store: Twin Cycles"); //Cicero

            GeminiUrl("object?o1=___1.Product--968&as1=open&d1=BestSpecialOffer&f1_quantity=%22%22");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            WaitForCss("#quantity1"); //i.e. dialog open
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54\r\nAction dialog: Best Special Offer\r\nQuantity: empty");
        }
        public virtual void WarningsAndInfo()
        {
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
        public virtual void RecentObjects()
        {
            GeminiUrl("home?m1=CustomerRepository&d1=FindCustomerByAccountNumber&f1_accountNumber=%22AW%22");
            ClearFieldThenType("#accountnumber1", "AW00000042");
                Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Healthy Activity Store, AW00000042");
            ClickBackButton();
            ClearFieldThenType("#accountnumber1", "AW00000359");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Mechanical Sports Center, AW00000359");
            ClickBackButton();
            ClearFieldThenType("#accountnumber1", "AW00022262");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Marcus Collins, AW00022262");
            ClickBackButton();
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
        }
        public virtual void ApplicationProperties()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            ClickPropertiesButton();
            WaitForView(Pane.Single, PaneType.Properties, "Application Properties");
        }
    }
    public abstract class FooterTests : FooterTestsRoot
    {

        [TestMethod]
        public override void Home() { base.Home(); }

        [TestMethod]
        public override void BackAndForward() { base.BackAndForward(); }

        [TestMethod]
        public override void Cicero() { base.Cicero(); }

        [TestMethod] 
        public override void WarningsAndInfo() { base.WarningsAndInfo(); }

        [TestMethod]
        public override void RecentObjects() { base.RecentObjects(); }
        [TestMethod]
        public override void ApplicationProperties() { base.ApplicationProperties(); }
    }

    #region browsers specific subclasses 

    // [TestClass, Ignore]
    public class FooterIconTestsIe : FooterTests
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
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class FooterIconTestsFirefox : FooterTests
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
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass, Ignore]
    public class FooterIconTestsChrome : FooterTests
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

    #region Mega tests
    public abstract class MegaFooterTestsRoot : FooterTestsRoot
    {
        [TestMethod]
        public void MegaFooterTest()
        {
            base.Home();
            base.BackAndForward();
            base.Cicero();
            base.WarningsAndInfo();
            base.RecentObjects();
            base.ApplicationProperties();
        }
    }

    [TestClass]
    public class MegaFooterTestsFirefox : MegaFooterTestsRoot
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
    #endregion
}