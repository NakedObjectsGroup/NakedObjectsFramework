// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class CiceroMenu : AWTest
    {
        [TestMethod]
        public void MenuWithoutArguments()
        {
            CiceroUrl("home");
            WaitForOutput("home");
            EnterCommand("Menu");
            WaitForOutput("Menus: Customers; Orders; Products; Employees; Sales; Special Offers; Contacts; Vendors; Purchase Orders; Work Orders;");
        }

        [TestMethod]
        public void MenuWithArgument()
        {
            CiceroUrl("home");
            WaitForOutput("home");
            EnterCommand("Menu Customers");
            WaitForOutput("Customers menu.");
            EnterCommand("Menu pro");
            WaitForOutput("Products menu.");
            EnterCommand("Menu off");
            WaitForOutput("Special Offers menu.");
            EnterCommand("Menu ord");
            WaitForOutput("Menus matching ord: Orders; Purchase Orders; Work Orders;");
            EnterCommand("Menu foo");
            WaitForOutput("foo does not match any menu");
            EnterCommand("Menu cust prod");
            WaitForOutput("cust prod does not match any menu");
            EnterCommand("Menu cus, ord");
            WaitForOutput("Wrong number of arguments provided.");
        }

        [TestMethod]
        public void MenuInInvalidContext()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            EnterCommand("Menu");
            WaitForOutput("The command: menu is not available in the current context");
        }
    }
    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class CiceroMenuIe : CiceroMenu {
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
    public class CiceroMenuFirefox : CiceroMenu
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

   // [TestClass, Ignore]
    public class CiceroMenuChrome : CiceroMenu
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
}