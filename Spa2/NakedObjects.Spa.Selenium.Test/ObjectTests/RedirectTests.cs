// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Spa.Selenium.Test.ObjectTests {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class RedirectTestsRoot : AWTest {

        protected override string BaseUrl => TestConfig.BaseObjectUrl;

        public virtual void RedirectFromActionResult() {
            GeminiUrl("home?m1=SalesRepository");
            Click(GetObjectEnabledAction("Random Sales Tax Rate"));
            WaitForView(Pane.Single, PaneType.Object);
            //Redirected from a SalesTaxRate to corresponding StateProvice
            wait.Until(dr => dr.FindElement(By.CssSelector(".properties")).Text.Contains("Is Only State Province"));
        }

        public virtual void RedirectFromLink() {
            GeminiUrl("home?m1=SalesRepository");
            Click(GetObjectEnabledAction("Sales Tax Rates"));
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

    public class MegaRedirectTestBase : RedirectTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public virtual void RedirectTests() {
            // todo look into is it just config ?
            //RedirectFromActionResult();
            //RedirectFromLink();
        }

        //[TestMethod]
        [Priority(-1)]
        public void ProblematicTests() { }
    }

    //[TestClass]
    public class MegaRedirectTestFirefox : MegaRedirectTestBase {
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
    public class MegaRedirectTestIe : MegaRedirectTestBase {
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

    [TestClass] //toggle
    public class MegaRedirectTestChrome : MegaRedirectTestBase {
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