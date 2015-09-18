// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests applied from a Query view.
    /// </summary>
    public abstract class QueryTests : GeminiTest {
        [TestMethod]
        public virtual void QueryActionReturnsListView()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            WaitForClass("actions");
            ClickAction("Highest Value Orders");
            WaitForClass("query");
            WaitForSingleQuery();
            //Test content of collection
            Assert.AreEqual("20-Objects", br.FindElement(By.CssSelector(".collection .summary .details")).Text);
            WaitForClass("icon-table");
            AssertClassDoesNotExist("icon-list");
            AssertClassDoesNotExist("icon-summary");
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.reference")).Count);
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.checkbox")).Count);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only

        }

        [TestMethod]
        public virtual void SwitchToTableViewAndBackToList()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            WaitForClass("actions");
            ClickAction("Highest Value Orders");
            var iconTable = WaitForClass("icon-table");
            Click(iconTable);
            var iconList = WaitForClass("icon-list");
            AssertClassDoesNotExist("icon-table");
            AssertClassDoesNotExist("icon-summary");
            //TODO: Clean this up below  -  reduce to what we actually want to test
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table tbody tr")).Count ==20);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table thead tr")).Count == 1);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table thead tr th")).Count == 25);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table tbody tr .cell")).Count > 50);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table tbody tr"))[1].FindElements(By.CssSelector(".cell")).Count == 25);

            //Switch back to List view
            Click(iconList);
            WaitForClass("icon-table");
            AssertClassDoesNotExist("icon-list");
            AssertClassDoesNotExist("icon-summary");
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.reference")).Count);
            Assert.AreEqual(20, br.FindElements(By.CssSelector(".collection table tbody tr td.checkbox")).Count);
            Assert.AreEqual(0, br.FindElements(By.CssSelector(".cell")).Count); //Cells are in Table view only
        }

        [TestMethod]
        public virtual void NavigateToItemFromListView()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            WaitForClass("actions");
            ClickAction("Highest Value Orders");
            WaitForClass("query");
            WaitForSingleQuery();

            // select item
            var row = wait.Until( dr => dr.FindElement(By.CssSelector("table .reference")));
            Assert.AreEqual("SO51131", row.Text);
            Click(row);
            WaitForSingleObject("SO51131");
        }

        [TestMethod]
        public virtual void NavigateToItemFromTableView()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            WaitForClass("actions");
            ClickAction("Highest Value Orders");
            var iconTable = WaitForClass("icon-table");
            Click(iconTable);

            // select item
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection table tbody tr .cell")).Count > 25);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            Click(row);

            var title = wait.Until(dr => dr.FindElement(By.CssSelector(".object .header .title")));
            Assert.AreEqual("SO51131", title.Text);
        }
        
        //TODO: Collection contributed Actions
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class QueryTestsIe : QueryTests
    {
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
    public class QueryTestsFirefox : QueryTests
    {
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
    public class QueryTestsChrome : QueryTests {
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
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}