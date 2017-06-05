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
    public abstract class RedirectTestsRoot : AWTest {
        public virtual void RedirectFromActionResult() {
            GeminiUrl("home?m1=SalesRepository");
            Click(GetObjectAction("Random Sales Tax Rate"));
            WaitForView(Pane.Single, PaneType.Object);
            //Redirected from a SalesTaxRate to corresponding StateProvice
            wait.Until(dr => dr.FindElement(By.CssSelector(".properties")).Text.Contains("Is Only State Province"));
        }

        public virtual void RedirectFromLink() {
            GeminiUrl("home?m1=SalesRepository");
            Click(GetObjectAction("Sales Tax Rates"));
            WaitForView(Pane.Single, PaneType.List);
            WaitForCss(".reference", 20);
            var row = WaitForCssNo(".reference", 0);
            Assert.AreEqual("Tax Rate for: Alberta", row.Text);
            Click(row);
            //Redirected from a SalesTaxeRate to corresponding StateProvice
            WaitForView(Pane.Single, PaneType.Object, "Alberta");
        }
    }

    public abstract class RedirectTests : RedirectTestsRoot {
        [TestMethod]
        public override void RedirectFromActionResult() {
            base.RedirectFromActionResult();
        }

        [TestMethod]
        public override void RedirectFromLink() {
            base.RedirectFromLink();
        }
    }

    #region browsers specific subclasses 

    public class RedirectTestsIe : RedirectTests {
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
    public class RedirectTestsFirefox : RedirectTests {
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

    public class RedirectTestsChrome : RedirectTests {
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

    public class MegaRedirectTestBase : RedirectTestsRoot {
        [TestMethod] //Mega
        public virtual void MegaRedirectTest() {
            // todo look into is it just config ?
            //RedirectFromActionResult();
            //RedirectFromLink();
        }
    }

    //[TestClass]
    public class MegaRedirectTestFirefox : MegaRedirectTestBase {
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

    //[TestClass]
    public class MegaRedirectTestIe : MegaRedirectTestBase {
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

   //[TestClass] toggle
    public class MegaRedirectTestChrome : MegaRedirectTestBase {
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
    }
}