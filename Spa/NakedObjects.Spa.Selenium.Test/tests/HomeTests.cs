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
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class HomeTests : GeminiTest {

        #region Clicking on menus and opening/closing dialogs
        [TestMethod]
        public virtual void ClickOnVariousMenus() {
            GoToMenuFromHomePage("Customers");

            wait.Until(d => d.FindElement(By.ClassName("actions")));
            var actions = br.FindElements(By.ClassName("action"));
            Assert.AreEqual(9, actions.Count);

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Assert.AreEqual("Create New Store Customer", actions[2].Text);
            Assert.AreEqual("Random Store", actions[3].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            Assert.AreEqual("Random Individual", actions[6].Text);
            Assert.AreEqual("Customer Dashboard", actions[7].Text);
            Assert.AreEqual("Throw Domain Exception", actions[8].Text);

            GoToMenuFromHomePage("Sales");

            wait.Until(d => d.FindElement(By.ClassName("actions")));
            actions = br.FindElements(By.ClassName("action"));
            Assert.AreEqual(4, actions.Count);

            Assert.AreEqual("Create New Sales Person", actions[0].Text);
            Assert.AreEqual("Find Sales Person By Name", actions[1].Text);
            Assert.AreEqual("List Accounts For Sales Person", actions[2].Text);
            Assert.AreEqual("Random Sales Person", actions[3].Text);
        }

        [TestMethod]
        public virtual void SelectSuccessiveDialogActionsThenCancel()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);
            var actions = br.FindElements(By.ClassName("action"));
            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Click(actions[0]);

            wait.Until(d => d.FindElement(By.ClassName("dialog")));
            string title = br.FindElement(By.CssSelector("div.dialog > div.title")).Text;
            Assert.AreEqual("Find Customer By Account Number", title);

            actions = br.FindElements(By.ClassName("action"));
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Click(actions[1]);

            wait.Until(d => d.FindElement(By.CssSelector("div.dialog > div.title")).Text == "Find Store By Name");

            // cancel dialog 
            Click(br.FindElement(By.CssSelector("div.dialog  .cancel")));

            wait.Until(d => {
                try
                {
                    br.FindElement(By.ClassName("dialog"));
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
            });
        }
        #endregion

        #region Invoking main menu actions
        [TestMethod]
        public virtual void ZeroParamReturnsObject()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            IWebElement action = br.FindElements(By.ClassName("action"))[3];
            Assert.AreEqual("Random Store", action.Text);

            // click on action to get object 
            Click(action);

            wait.Until(d => d.FindElement(By.ClassName("object")));
        }

        [TestMethod]
        public virtual void ZeroParamReturnsCollection()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
            var action = br.FindElements(By.ClassName("action"))[2];

            Assert.AreEqual("Highest Value Orders", action.Text);
            Click(action);

            wait.Until(d => d.FindElement(By.ClassName("query")));
            wait.Until(d => d.FindElements(By.ClassName("reference")).Count == 20);
        }

        [TestMethod]
        public virtual void ZeroParamThrowsError()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            IWebElement action = br.FindElements(By.ClassName("action"))[8];
            Assert.AreEqual("Throw Domain Exception", action.Text);

            // click on action to get object 
            Click(action);

            wait.Until(d => d.FindElement(By.ClassName("error")));

            var msg = br.FindElement(By.CssSelector(".message"));
            Assert.AreEqual("Foo", msg.Text);
        }

        [TestMethod]
        public virtual void ZeroParamReturnsEmptyCollection()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
            var actions = br.FindElements(By.ClassName("action"));

            Assert.AreEqual("Orders In Process", actions[0].Text);
            Click(actions[0]);

            wait.Until(d => d.FindElement(By.ClassName("query")));
            var rows = br.FindElements(By.CssSelector("td"));
            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public virtual void DialogActionOK()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[0]); // Find customer by account number

            wait.Until(d => d.FindElement(By.ClassName("dialog")));
            string title = br.FindElement(By.CssSelector("div.dialog > div.title")).Text;

            Assert.AreEqual("Find Customer By Account Number", title);

            br.FindElement(By.CssSelector(".value  input")).SendKeys("00022262");

            ClickOK();

            wait.Until(d => d.FindElement(By.ClassName("object")));
        }
        #endregion
    }

    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class HomeTestsIe : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class HomeTestsFirefox : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

   // [TestClass, Ignore]
    public class HomeTestsChrome : HomeTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            br.Navigate().GoToUrl(Url);
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