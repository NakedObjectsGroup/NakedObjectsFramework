// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class CopyAndPasteTests : AWTest {

        [TestMethod, Ignore]
        public virtual void CopiedObjectShowsInFooter()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.Product-990&object2=AdventureWorksModel.Customer-652");
            WaitForView(Pane.Left, PaneType.Object, "Mountain-500 Black, 42");
            WaitForView(Pane.Right, PaneType.Object, "Selected Distributors, AW00000652");

            WaitForCss("#pane2 .header .title").Click();
            IWebElement active = br.SwitchTo().ActiveElement();
            Assert.AreEqual("div", active.TagName);
            Assert.AreEqual("Selected Distributors, AW00000652", active.Text);
            active.SendKeys(Keys.Control + "c");
            var copying = WaitForCss(".footer .currentclass .reference");
            Assert.AreEqual("Selected Distributors, AW00000652", copying.Text);
            
        }

    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class CopyAndPasteTestsIe : CopyAndPasteTests
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

    [TestClass]
    public class CopyAndPasteTestsFirefox : CopyAndPasteTests
    {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    //[TestClass, Ignore]
    public class CopyAndPasteTestsChrome : CopyAndPasteTests
    {
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
    }

    #endregion
}