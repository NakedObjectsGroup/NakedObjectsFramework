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
            Url(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            WaitForCss(".value  select").SendKeys("Ascending");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }

        [TestMethod]
        public virtual void TestCancelDialog()
        {
            Url(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            CancelDialog();
        }

        [TestMethod]
        public virtual void ScalarChoicesParmKeepsValue()
        {
            Url(OrdersMenuUrl);
            GetObjectActions(OrderServiceActions);
            OpenActionDialog("Orders By Value");

            WaitForCss(".value  select").SendKeys("Ascending");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }

        [TestMethod]
        public virtual void ScalarParmKeepsValue()
        {
            Url(CustomersMenuUrl);
            GetObjectActions(CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            WaitForCss(".value input").SendKeys("00000042");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Healthy Activity Store, AW00000042");
        }

        [TestMethod]
        public virtual void DateTimeParmKeepsValue()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Customer-555&actions1=open");
            OpenActionDialog("Search For Orders");
            TypeIntoField("#fromdate","1 Jan 2003");
            TypeIntoField("#todate", "1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Search For Orders");
        }

        [TestMethod]
        public virtual void RefChoicesParmKeepsValue()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Category");
            WaitForCss(".value  select").SendKeys("Forks");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "List Products By Sub Category");
            AssertTopItemInListIs("HL Fork");
        }

        [TestMethod]
        public virtual void MultipleRefChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");

            var selected = new SelectElement(WaitForCss("div#subcategories select"));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", selected.AllSelectedOptions.First().Text);
            Assert.AreEqual("Touring Bikes", selected.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "List Products By Sub Categories");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        [TestMethod]
        public virtual void MultipleRefChoicesChangeDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");

            WaitForCss(".value  select").SendKeys("Handlebars");
            IKeyboard kb = ((IHasInputDevices)br).Keyboard;

            kb.PressKey(Keys.Control);
            br.FindElement(By.CssSelector(".value  select option[label='Brakes']")).Click();
            kb.ReleaseKey(Keys.Control);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "List Products By Sub Categories");
            AssertTopItemInListIs("Front Brakes");
        }

        [TestMethod]
        public virtual void ChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            var slctPl = new SelectElement(WaitForCss("div#productline select"));
            var slctPc = new SelectElement(WaitForCss("div#productclass select"));

            Assert.AreEqual("M", slctPl.SelectedOption.Text);
            Assert.AreEqual("H", slctPc.SelectedOption.Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Find By Product Line And Class");
            AssertTopItemInListIs("Mountain-300 Black, 38");
        }

        [TestMethod]
        public virtual void ChoicesChangeDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            WaitForCss("div#productline .value  select").SendKeys("R");
            WaitForCss("div#productclass .value  select").SendKeys("L");

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Find By Product Line And Class");
            AssertTopItemInListIs("HL Road Frame - Black, 58");
        }

        [TestMethod]
        public virtual void ConditionalChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find Products By Category");
            var slctCs = new SelectElement(WaitForCss("div#categories select"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(WaitForCss("div#subcategories select")).AllSelectedOptions.Count == 2);

            var slct = new SelectElement(WaitForCss("div#subcategories select"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        [TestMethod]
        public virtual void ConditionalChoicesChangeDefaults()
        {
            Url(ProductServiceUrl);

            OpenActionDialog("Find Products By Category");

            var slctCs = new SelectElement(WaitForCss("div#categories select"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(WaitForCss("div#subcategories select")).AllSelectedOptions.Count == 2);


            var slct = new SelectElement(WaitForCss("div#subcategories select"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        #region Auto Complete
        [TestMethod]
        public virtual void AutoCompleteParmShow()
        {
            Url(SalesServiceUrl);
            OpenActionDialog("List Accounts For Sales Person");

            br.FindElement(By.CssSelector(".value input[type='text']")).SendKeys("Valdez");

            wait.Until(d => d.FindElement(By.CssSelector(".ui-menu-item")));

            Click(WaitForCss(".ui-menu-item"));

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "List Accounts For Sales Person");
        }

        [TestMethod]
        public virtual void AutoCompleteParmGo()
        {
            Url(SalesServiceUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("List Accounts For Sales Person");

            WaitForCss(".value input[type='text']").SendKeys("Valdez");

            wait.Until(d => d.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);

            Click(WaitForCss(".ui-menu-item"));

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Query, "List Accounts For Sales Person");

            try
            {
                WaitForCss(".value input[type='text']");
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
            Url(ProductServiceUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("Find Product");

            Assert.AreEqual("Adjustable Race", WaitForCss(".value input[type='text']").GetAttribute("value"));

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Adjustable Race");
        }

        [TestMethod]
        public virtual void AutoCompleteParmShowSingleItem()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find Product");

            var acElem = WaitForCss(".value input[type='text']");

            for (int i = 0; i < 15; i++)
            {
                acElem.SendKeys(Keys.Backspace);
            }
            acElem.SendKeys("BB");
            var item = wait.Until(dr => dr.FindElement(By.CssSelector(".ui-menu-item")));
            Assert.AreEqual("BB Ball Bearing", item.Text);
            Click(item);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "BB Ball Bearing");
        }
        #endregion

        #region Parameter validation
        [TestMethod]
        public virtual void MandatoryParameterEnforced()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-342&actions1=open&dialog1=BestSpecialOffer");
            var qty = WaitForCss(".parameter#quantity input");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameter#quantity .validation")).Text.Length > 0);
            var validation = WaitForCss(".parameter#quantity .validation");
            Assert.AreEqual("Mandatory", validation.Text);
            qty.SendKeys(Keys.Backspace + "1");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        [TestMethod]
        public virtual void ValidateSingleValueParameter()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-342&actions1=open&dialog1=BestSpecialOffer");
            var qty = WaitForCss(".parameter#quantity input");
            qty.SendKeys("0");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameter#quantity .validation")).Text.Length > 0);
            var validation = WaitForCss(".parameter#quantity .validation");
            Assert.AreEqual("Quantity must be > 0", validation.Text);
            qty.SendKeys(Keys.Backspace+"1");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }

        [TestMethod]
        public virtual void ValidateSingleRefParamFromChoices()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.SalesOrderHeader-71742&collection1_SalesOrderHeaderSalesReason=List&actions1=open&dialog1=AddNewSalesReason");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection")).Count == 2);
            var reason = WaitForCss(".parameter#reason select");
            reason.SendKeys("Price");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameter#reason .validation")).Text.Length > 0);
            var validation = WaitForCss(".parameter#reason .validation");
            Assert.AreEqual("Price already exists in Sales Reasons", validation.Text);
        }

        [TestMethod, Ignore]
        public virtual void CoValidationOfMultipleParameters()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.PurchaseOrderDetail-1632-3660&actions1=open&dialog1=ReceiveGoods");
            WaitForCss(".parameter#qtyreceived input").SendKeys("100");
            WaitForCss(".parameter#qtyrejected input").SendKeys("50");
            WaitForCss(".parameter#qtyintostock input").SendKeys("49");
            Click(OKButton());
            //TODO: Test for co-validation message of '"Qty Into Stock + Qty Rejected must add up to Qty Received"'
        }

        #endregion

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