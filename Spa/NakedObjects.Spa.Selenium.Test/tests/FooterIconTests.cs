// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class FooterIconTests : GeminiTest {

        [TestMethod]
        public virtual void Home() {
            br.Navigate().GoToUrl(Product968Url);
            WaitFor(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(br.FindElement(By.CssSelector(".icon-home")));
            WaitFor(Pane.Single, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void BackAndForward() {
            br.Navigate().GoToUrl(Url);
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);
            GoToMenuFromHomePage("Customers");
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            Click(br.FindElement(By.CssSelector(".icon-back")));
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);
            Click(br.FindElement(By.CssSelector(".icon-forward")));
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
        }
    }

    #region browsers specific subclasses 

   // [TestClass, Ignore]
    public class MenuBarTestsIe : FooterIconTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class MenuBarTestsFirefox : FooterIconTests {
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
    }

    //[TestClass, Ignore]
    public class MenuBarTestsChrome : FooterIconTests {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}