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
    public abstract class KeyboardNavigationTestsRoot : AWTest
    {
        public virtual void SelectFooterIconsWithAccessKeys()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            WaitForCss(".header .title").Click();
            var element = br.SwitchTo().ActiveElement();
            element.SendKeys(Keys.Alt + "h");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Home (Alt-h)", element.GetAttribute("title"));
        }

        public virtual void SelectObjectActionsWithAccessKey()
        {
            GeminiUrl("object?i1=View&o1=___1.Person--15748");
            WaitForView(Pane.Single, PaneType.Object);
            var element = br.SwitchTo().ActiveElement();
            element.SendKeys(Keys.Alt + "a");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Open actions (Alt-a)", element.GetAttribute("title"));
        }

        public virtual void EnterEquivalentToLeftClick()
        {
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
            reference.SendKeys(Keys.Enter);
            WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }

        public virtual void ShiftEnterEquivalentToRightClick()
        {
            Url(CustomersMenuUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            ClearFieldThenType(".value  input", "AW00022262");
            OKButton().SendKeys(Keys.Shift + Keys.Enter);
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Object, "Marcus Collins, AW00022262");
        }
    }
    public abstract class KeyboardNavigationTests : KeyboardNavigationTestsRoot
    {

        [TestMethod, Ignore] //Doesn't work with Firefox
        public override void SelectFooterIconsWithAccessKeys() { base.SelectFooterIconsWithAccessKeys(); }

        [TestMethod, Ignore] //Doesn't work with Firefox
        public override void SelectObjectActionsWithAccessKey() { base.SelectObjectActionsWithAccessKey(); }

        [TestMethod]
        public override void EnterEquivalentToLeftClick() { base.EnterEquivalentToLeftClick(); }

        [TestMethod] 
        public override void ShiftEnterEquivalentToRightClick() { base.ShiftEnterEquivalentToRightClick(); }
    }

    #region browsers specific subclasses 

    public class KeyboardNavigationTestsIe : KeyboardNavigationTests
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

    //[TestClass] //Firefox Individual
    public class KeyboardNavigationTestsFirefox : KeyboardNavigationTests
    {
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

    public class KeyboardNavigationTestsChrome : KeyboardNavigationTests
    {
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

    #region Mega tests
    public abstract class MegaKeyboardTestsRoot : KeyboardNavigationTestsRoot
    {
        [TestMethod] //Mega
        public void MegaKeyboardTest()
        {
            //SelectFooterIconsWithAccessKeys(); TODO: Does not work on Firefox in test mode
            //SelectObjectActionsWithAccessKey();  TODO: Does not work on Firefox in test mode
            EnterEquivalentToLeftClick();
            ShiftEnterEquivalentToRightClick();
        }
    }

    [TestClass]
    public class MegaKeyboardTestsFirefox : MegaKeyboardTestsRoot
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