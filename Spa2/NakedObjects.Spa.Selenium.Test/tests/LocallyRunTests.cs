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
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium {
    /// <summary>
    /// These are all tests that should pass when run locally, but are unreliable on the server
    /// </summary>
    public abstract class LocallyRunTestsRoot : AWTest {
      

        #region Cicero

        //Note: Requires the Cicero icon to be made visible
        public virtual void LaunchCiceroFromIcon() {
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

        #endregion

       

        #region Copy & Paste

        public virtual void IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour() {
            GeminiUrl("home?m1=EmployeeRepository&d1=CreateNewEmployeeFromContact&f1_contactDetails=null");
            WaitForView(Pane.Single, PaneType.Home);
            var home = WaitForCss(".title");
            Actions action = new Actions(br);
            action.DoubleClick(home); //Should put "Home"into browser clipboard
            action.SendKeys(Keys.Control + "c");
            action.Perform();
            Thread.Sleep(500);
            //home.SendKeys(Keys.Control + "c");
            string selector = "input.value";
            var target = WaitForCss(selector);
            Assert.AreEqual("", target.GetAttribute("value"));
            target.Click();
            target.SendKeys(Keys.Control + "v");
            Assert.AreEqual("Home", target.GetAttribute("value"));
        }

        public virtual void CanClearADroppableReferenceField() {
            GeminiUrl("object?o1=___1.PurchaseOrderHeader--561&i1=Edit");
            WaitForView(Pane.Single, PaneType.Object);
            var fieldCss = ".property:nth-child(4) .value.droppable";
            var field = WaitForCss(fieldCss);
            Assert.AreEqual("Ben Miller", field.GetAttribute("value"));
            Thread.Sleep(100);

            var fieldBeforeCss = WaitForCss(".property:nth-child(3) input");
            fieldBeforeCss.SendKeys(Keys.Tab);
            field.SendKeys(Keys.Delete);
            wait.Until(dr => dr.FindElement(By.CssSelector(fieldCss)).GetAttribute("value") == "* (drop here)");
        }

        #endregion

        #region Object Edit


        #endregion

        #region Keyboard navigation

        public virtual void SelectFooterIconsWithAccessKeys() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            WaitForCss(".header .title").Click();
            var element = br.SwitchTo().ActiveElement();

            element.SendKeys(Keys.Alt + "h");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Home (Alt-h)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "b");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Back (Alt-b)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "f");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Forward (Alt-f)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "e");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Expand pane (Alt-e)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "s");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Swap panes (Alt-s)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "r");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Recent object (Alt-r)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "c");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Cicero - Speech Interface (Alt-c)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "p");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Application Properties (Alt-p)", element.GetAttribute("title"));

            element.SendKeys(Keys.Alt + "l");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Log off (Alt-l)", element.GetAttribute("title"));
        }

        public virtual void SelectObjectActionsWithAccessKey() {
            GeminiUrl("object?i1=View&o1=___1.Person--15748");
            WaitForView(Pane.Single, PaneType.Object);
            var element = br.SwitchTo().ActiveElement();
            element.SendKeys(Keys.Alt + "a");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Open actions (Alt-a)", element.GetAttribute("title"));
        }

        #endregion
    }

    public abstract class LocallyRunTests : LocallyRunTestsRoot {
        #region List tests

        #endregion

        #region Cicero

        //Note: Requires the Cicero icon to be made visible
        public override void LaunchCiceroFromIcon() {
            base.LaunchCiceroFromIcon();
        }

        #endregion

        #region Copy & Paste

        [TestMethod, Ignore] // doesn't work on chrome
        public override void CanClearADroppableReferenceField() {
            base.CanClearADroppableReferenceField();
        }

        [TestMethod, Ignore] // doesn't work on chrome
        public override void IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour() {
            base.IfNoObjectInClipboardCtrlVRevertsToBrowserBehaviour();
        }

        #endregion

        #region Object Edit

        #endregion

        #region Keyboard navigation

        [TestMethod, Ignore] //Doesn't work with Firefox or Chrome - behaviour is different
        public override void SelectFooterIconsWithAccessKeys() {
            base.SelectFooterIconsWithAccessKeys();
        }

        [TestMethod, Ignore] //Doesn't work with Firefox?
        public override void SelectObjectActionsWithAccessKey() {
            base.SelectObjectActionsWithAccessKey();
        }

        #endregion
    }

    #region browsers specific subclasses 

    //[TestClass, Ignore] //IE Individual
    public class LocallyRunTestsIe : LocallyRunTests {
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

    //[TestClass, Ignore] //Firefox Individual
    public class LocallyRunTestsFirefox : LocallyRunTests {
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

    [TestClass] //Chrome Individual
    public class LocallyRunTestsChrome : LocallyRunTests {
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