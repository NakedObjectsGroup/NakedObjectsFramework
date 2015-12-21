// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class FooterIconTests : AWTest {

        [TestMethod]
        public virtual void Home() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(br.FindElement(By.CssSelector(".icon-home")));
            WaitForView(Pane.Single, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void BackAndForward() {
            Url(BaseUrl);
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);
            GoToMenuFromHomePage("Customers");
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            Click(br.FindElement(By.CssSelector(".icon-back")));
            wait.Until(d => d.FindElements(By.CssSelector(".menu")).Count == MainMenusCount);
            Click(br.FindElement(By.CssSelector(".icon-forward")));
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
        }

        [TestMethod]
        public virtual void Cicero()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Product-968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54."); //Cicero
            GeminiUrl("object/list?object1=AdventureWorksModel.Store-350&menu2=OrderRepository&action2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Store: Twin Cycles."); //Cicero

            GeminiUrl("object?object1=AdventureWorksModel.Product-968&actions1=open&dialog1=BestSpecialOffer&parm1_quantity=%2522%2522");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            WaitForCss("#quantity1"); //i.e. dialog open
            Click(WaitForCss(".icon-speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54. Action dialog: Best Special Offer. Quantity: empty,");
        }

    }

    #region browsers specific subclasses 

   // [TestClass, Ignore]
    public class FooterIconTestsIe : FooterIconTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
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
    public class FooterIconTestsFirefox : FooterIconTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
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
    public class FooterIconTestsChrome : FooterIconTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
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