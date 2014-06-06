// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class DialogTests : SpiroTest
    {

        private const int CustomersFindCustomerByAccountNumber = 0;

        private const int OrdersOrdersByValue = 3;

        private const int StoresSearchForOrders = 4;

        private const int SalesListAccountsForSalesPerson = 2;

        private const int ProductsFindProductByName = 0;
        private const int ProductsFindProductByNumber = 1;
        private const int ProductsListProductsBySubcategory = 2;
        private const int ProductsListProductsBySubcategories = 3;
        private const int ProductsFindByProductLineAndClass = 4;
        private const int ProductsFindByProductLinesAndClasses = 5;
        private const int ProductsFindProduct = 6;
        private const int ProductsFindProductsByCategory = 7;
       

        [TestMethod]
        public virtual void ChoicesParm() {
            br.Navigate().GoToUrl(orderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 6);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            var showList = new Action<string, string>((type, test) => {
                // click on action to open dialog 
                Click(br.FindElements(By.ClassName("action"))[OrdersOrdersByValue]); // orders by value

                wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
                string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

                Assert.AreEqual("Orders By Value", title);

                br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys(type);

                Click(br.FindElement(By.ClassName("show")));

                wait.Until(d => d.FindElement(By.ClassName("list-view")));

                string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

                Assert.AreEqual(test, topItem);
            });

            var cancelList = new Action(() => {
                // cancel object 
                Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("list-view"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            var cancelDialog = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("action-dialog"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            showList("Ascending", "SO51782");
            cancelList();
            cancelDialog();

            showList("Descending", "SO51131");
            cancelDialog();
            cancelList();
        }

        [TestMethod]
        public virtual void ScalarChoicesParmKeepsValue() {
            br.Navigate().GoToUrl(orderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 6);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[OrdersOrdersByValue]); // orders by value

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Orders By Value", title);

            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("Ascending");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("SO51782", topItem);

            Assert.AreEqual("Ascending", br.FindElement(By.CssSelector("option[selected=selected]")).Text); 
        }

        [TestMethod]
        public virtual void ScalarParmKeepsValue() {
            br.Navigate().GoToUrl(customerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[CustomersFindCustomerByAccountNumber]); // find customer by account number

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Customer By Account Number", title);

            br.FindElement(By.CssSelector("div.parameter-value input")).SendKeys("00000042");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            Assert.AreEqual("AW00000042", br.FindElement(By.CssSelector("div.parameter-value input")).GetAttribute("value"));
        }

        [TestMethod]
        public virtual void DateTimeParmKeepsValue() {
            br.Navigate().GoToUrl(store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 8);


            // click on action to open dialog 
            Click(br.FindElements(By.CssSelector("div.action-button a"))[StoresSearchForOrders]); // Search for orders

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Search For Orders", title);

            br.FindElements(By.CssSelector(".parameter-value input"))[0].SendKeys("1 Jan 2003");
            br.FindElements(By.CssSelector(".parameter-value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(

            wait.Until(d => br.FindElement(By.ClassName("show")));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            Assert.AreEqual("1 Jan 2003", br.FindElements(By.CssSelector(".parameter-value input"))[0].GetAttribute("value"));
            Assert.AreEqual("1 Dec 2003", br.FindElements(By.CssSelector(".parameter-value input"))[1].GetAttribute("value"));
        }

        [TestMethod]
        public virtual void RefChoicesParmKeepsValue() {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategory]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Category", title);

            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("Forks");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("HL Fork", topItem);

            Assert.AreEqual("Forks", br.FindElement(By.CssSelector("option[selected=selected]")).Text);
        }

       
        [TestMethod]
        public virtual void MultipleRefChoicesDefaults()
        {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategories]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Categories", title);

            var selected = br.FindElements(By.CssSelector("option[selected=selected]"));

            Assert.AreEqual(2, selected.Count);
            Assert.AreEqual("Mountain Bikes", selected.First().Text);
            Assert.AreEqual("Touring Bikes", selected.Last().Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Mountain-100 Black, 38", topItem);

            selected = br.FindElements(By.CssSelector("option[selected=selected]"));

            Assert.AreEqual(2, selected.Count);
            Assert.AreEqual("Mountain Bikes", selected.First().Text);
            Assert.AreEqual("Touring Bikes", selected.Last().Text);
        }

        [TestMethod]
        public virtual void MultipleRefChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategories]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Categories", title);


            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("HandleBars");
            IKeyboard kb = ((IHasInputDevices)br).Keyboard;

            kb.PressKey(Keys.Control);
            br.FindElement(By.CssSelector(".parameter-value  select option[value='5']")).Click();
            kb.ReleaseKey(Keys.Control);


            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Front Brakes", topItem);

            var selected = br.FindElements(By.CssSelector("option[selected=selected]"));

            Assert.AreEqual(2, selected.Count);
            Assert.AreEqual("Handlebars", selected.First().Text);
            Assert.AreEqual("Brakes", selected.Last().Text);
        }




        [TestMethod]
        public virtual void ChoicesDefaults()
        {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindByProductLineAndClass]); // find by product line and class

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find By Product Line And Class", title);
  
            Assert.AreEqual("M", br.FindElement(By.CssSelector("div#productline option[selected=selected]")).Text);
            Assert.AreEqual("H", br.FindElement(By.CssSelector("div#productclass option[selected=selected]")).Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Mountain-300 Black, 38", topItem);

            Assert.AreEqual("M", br.FindElement(By.CssSelector("div#productline option[selected=selected]")).Text);
            Assert.AreEqual("H", br.FindElement(By.CssSelector("div#productclass option[selected=selected]")).Text);

        }

        [TestMethod]
        public virtual void ChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindByProductLineAndClass]); // find by product line and class

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find By Product Line And Class", title);

            br.FindElement(By.CssSelector("div#productline .parameter-value  select")).SendKeys("R");
            br.FindElement(By.CssSelector("div#productclass .parameter-value  select")).SendKeys("L");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("HL Road Frame - Black, 58", topItem);

            Assert.AreEqual("R", br.FindElement(By.CssSelector("div#productline option[selected=selected]")).Text);
            Assert.AreEqual("L", br.FindElement(By.CssSelector("div#productclass option[selected=selected]")).Text);

        }

        [TestMethod]
        public virtual void ConditionalChoicesDefaults()
        {
            br.Navigate().GoToUrl(productServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 11);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindProductsByCategory]); 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Products By Category", title);

            Assert.AreEqual("Bikes", br.FindElement(By.CssSelector("div#categories option[selected=selected]")).Text);


            var slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Road-150 Red, 62", topItem);

            Assert.AreEqual("Bikes", br.FindElement(By.CssSelector("div#categories option[selected=selected]")).Text);

            slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);
        }


        [TestMethod]
        public virtual void AutoCompleteParmKeepsValue() {
            br.Navigate().GoToUrl(salesServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 5);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[SalesListAccountsForSalesPerson]); // list accounts for sales person 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Accounts For Sales Person", title);

            br.FindElement(By.CssSelector(".parameter-value input[type='text']")).SendKeys("Valdez");

            wait.Until(d => d.FindElement(By.ClassName("ui-menu-item")));

            Click(br.FindElement(By.CssSelector(".ui-menu-item a")));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            Assert.AreEqual("Rachel Valdez", br.FindElement(By.CssSelector(".parameter-value input[type='text']")).GetAttribute("value"));
        }
    }

    #region browsers specific subclasses

    [TestClass, Ignore]
    public class DialogTestsIe : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
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
    public class DialogTestsFirefox : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            SpiroTest.InitialiseClass(context);
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

    [TestClass, Ignore]
    public class DialogTestsChrome : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            SpiroTest.InitialiseClass(context);
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