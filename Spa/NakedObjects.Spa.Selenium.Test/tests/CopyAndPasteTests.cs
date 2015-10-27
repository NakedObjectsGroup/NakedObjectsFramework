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

        [TestMethod]
        public virtual void CopyTitleOrPropertyIntoClipboard()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.Product-990&object2=AdventureWorksModel.Customer-652");
            WaitForView(Pane.Left, PaneType.Object, "Mountain-500 Black, 42");
            WaitForView(Pane.Right, PaneType.Object, "Selected Distributors, AW00000652");

            //Copy title from left pane
            var title = WaitForCss("#pane1 .header .title");
            title.Click();
            CopyToClipboard(title);

            //Copy title right pane
            title = WaitForCss("#pane2 .header .title");
            title.Click();
            CopyToClipboard(title);

            //Copy embedded reference from left pane
            WaitForCss("#pane1 .header .title").Click();
            var target = Tab(4);
            Assert.AreEqual("Mountain-500", target.Text);
            CopyToClipboard(target);

            //Copy embedded reference from right pane
            WaitForCss("#pane2 .header .title").Click();
            target = Tab(3);
            Assert.AreEqual("Southeast", target.Text);
            CopyToClipboard(target);
        }
                    [TestMethod]
        public virtual void CopyListItemIntoClipboard()
        {
            GeminiUrl("list/list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&menu2=PersonRepository&action2=ValidCountries");
            WaitForView(Pane.Left, PaneType.List, "Current Special Offers");
            WaitForView(Pane.Right, PaneType.List, "Valid Countries");

            //Copy item from list, left pane
            WaitForCss("#pane1 .header .title").Click();
            var item = Tab(3);
            Assert.AreEqual("No Discount", item.Text);
            CopyToClipboard(item);

            //Copy item from list, right pane

            WaitForCss("#pane2 .header .menu").Click();
            Assert.AreEqual("Actions", br.SwitchTo().ActiveElement().Text);

             item = Tab(4);
            Assert.AreEqual("Australia", item.Text);
            CopyToClipboard(item);

        }

        private void CopyToClipboard(IWebElement element)
        {
            var title = element.Text;
            element.SendKeys(Keys.Control + "c");
            wait.Until(dr => dr.FindElement(By.CssSelector(".footer .currentcopy .reference")).Text == title);
        }

        private IWebElement Tab(int numberIfTabs = 1)
        {
            for (int i = 1; i <= numberIfTabs; i++)
            {
                br.SwitchTo().ActiveElement().SendKeys(Keys.Tab);
            }
            return br.SwitchTo().ActiveElement();
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