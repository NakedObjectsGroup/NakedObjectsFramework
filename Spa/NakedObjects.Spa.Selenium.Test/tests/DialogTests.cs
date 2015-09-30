// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium
{
    /// <summary>
    /// Tests for the detailed operation of dialogs, including parameter rendering,
    /// choices, auto-complete, default values, formatting, and validation
    /// </summary>
    public abstract class DialogTests : GeminiTest
    {
        [TestMethod]
        public virtual void ChoicesParm()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            FindElementByCss(".value  select").SendKeys("Ascending");
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }

        [TestMethod]
        public virtual void TestCancelDialog()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            CancelDialog();
        }

        [TestMethod]
        public virtual void ScalarChoicesParmKeepsValue()
        {
            br.Navigate().GoToUrl(OrdersMenuUrl);
            GetObjectActions(OrderServiceActions);
            OpenActionDialog("Orders By Value");

            FindElementByCss(".value  select").SendKeys("Ascending");
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }

        [TestMethod]
        public virtual void ScalarParmKeepsValue()
        {
            br.Navigate().GoToUrl(CustomersMenuUrl);
            GetObjectActions(CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            FindElementByCss(".value input").SendKeys("00000042");
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Object, "Healthy Activity Store, AW00000042");
        }

        [TestMethod]
        public virtual void DateTimeParmKeepsValue()
        {
            br.Navigate().GoToUrl(Store555UrlWithActionsMenuOpen);
            OpenActionDialog("Search For Orders");
            FindElementByCss(".value input",0).SendKeys("1 Jan 2003");
            FindElementByCss(".value input",1).SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Search For Orders");
        }

        [TestMethod]
        public virtual void RefChoicesParmKeepsValue()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Category");
            FindElementByCss(".value  select").SendKeys("Forks");
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "List Products By Sub Category");
            AssertTopItemInListIs("HL Fork");
        }

        [TestMethod]
        public virtual void MultipleRefChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");

            var selected = new SelectElement(FindElementByCss("div#subcategories select"));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", selected.AllSelectedOptions.First().Text);
            Assert.AreEqual("Touring Bikes", selected.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "List Products By Sub Categories");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        [TestMethod]
        public virtual void MultipleRefChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");

            FindElementByCss(".value  select").SendKeys("Handlebars");
            IKeyboard kb = ((IHasInputDevices)br).Keyboard;

            kb.PressKey(Keys.Control);
            br.FindElement(By.CssSelector(".value  select option[label='Brakes']")).Click();
            kb.ReleaseKey(Keys.Control);

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "List Products By Sub Categories");
            AssertTopItemInListIs("Front Brakes");
        }

        [TestMethod]
        public virtual void ChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            var slctPl = new SelectElement(FindElementByCss("div#productline select"));
            var slctPc = new SelectElement(FindElementByCss("div#productclass select"));

            Assert.AreEqual("M", slctPl.SelectedOption.Text);
            Assert.AreEqual("H", slctPc.SelectedOption.Text);

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Find By Product Line And Class");
            AssertTopItemInListIs("Mountain-300 Black, 38");
        }

        [TestMethod]
        public virtual void ChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            FindElementByCss("div#productline .value  select").SendKeys("R");
            FindElementByCss("div#productclass .value  select").SendKeys("L");

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Find By Product Line And Class");
            AssertTopItemInListIs("HL Road Frame - Black, 58");
        }

        [TestMethod]
        public virtual void ConditionalChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("Find Products By Category");
            var slctCs = new SelectElement(FindElementByCss("div#categories select"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(FindElementByCss("div#subcategories select")).AllSelectedOptions.Count == 2);

            var slct = new SelectElement(FindElementByCss("div#subcategories select"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        [TestMethod]
        public virtual void ConditionalChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            OpenActionDialog("Find Products By Category");

            var slctCs = new SelectElement(FindElementByCss("div#categories select"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(FindElementByCss("div#subcategories select")).AllSelectedOptions.Count == 2);


            var slct = new SelectElement(FindElementByCss("div#subcategories select"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        [TestMethod]
        public virtual void AutoCompleteParmShow()
        {
            br.Navigate().GoToUrl(SalesServiceUrl);
            OpenActionDialog("List Accounts For Sales Person");

            br.FindElement(By.CssSelector(".value input[type='text']")).SendKeys("Valdez");

            wait.Until(d => d.FindElement(By.CssSelector(".ui-menu-item")));

            Click(FindElementByCss(".ui-menu-item"));

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "List Accounts For Sales Person");
        }

        [TestMethod]
        public virtual void AutoCompleteParmGo()
        {
            br.Navigate().GoToUrl(SalesServiceUrl);
            WaitFor(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("List Accounts For Sales Person");

            FindElementByCss(".value input[type='text']").SendKeys("Valdez");

            wait.Until(d => d.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);

            Click(FindElementByCss(".ui-menu-item"));

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Query, "List Accounts For Sales Person");

            try
            {
                FindElementByCss(".value input[type='text']");
                // found so it fails
                Assert.Fail();
            }
            catch
            {
                // all OK 
            }
        }

        [TestMethod]
        public virtual void AutoCompleteParmDefault()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            WaitFor(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("Find Product");

            Assert.AreEqual("Adjustable Race", FindElementByCss(".value input[type='text']").GetAttribute("value"));

            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Object, "Adjustable Race");
        }

        [TestMethod]
        public virtual void AutoCompleteParmShowSingleItem()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);
            OpenActionDialog("Find Product");

            var acElem = FindElementByCss(".value input[type='text']");

            for (int i = 0; i < 15; i++)
            {
                acElem.SendKeys(Keys.Backspace);
            }
            acElem.SendKeys("BB");
            var item = wait.Until(dr => dr.FindElement(By.CssSelector(".ui-menu-item")));
            Assert.AreEqual("BB Ball Bearing", item.Text);
            Click(item);
            Click(OKButton());
            WaitFor(Pane.Single, PaneType.Object, "BB Ball Bearing");
        }
    }

    #region browsers specific subclasses

    // [TestClass, Ignore]
    public class DialogTestsIe : DialogTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class DialogTestsFirefox : DialogTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    // [TestClass, Ignore]
    public class DialogTestsChrome : DialogTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion
}