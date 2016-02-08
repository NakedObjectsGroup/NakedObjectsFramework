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
    public abstract class DialogTestsRoot : AWTest
    {

        //This test is a hangover from when the button was named 'Get'
        //for query-only actions, and 'Do' for others.  This has since
        //been reverted to OK for both.

        public virtual void OKButtonNaming()
        {
            Url(OrdersMenuUrl);
            //Query only action
            OpenActionDialog("Orders By Value");
            Assert.AreEqual("OK", OKButton().GetAttribute("value"));
            GeminiUrl("home?menu1=SalesRepository");
            //Other action
            OpenActionDialog("Create New Sales Person");
            Assert.AreEqual("OK", OKButton().GetAttribute("value"));
        }

        public virtual void ChoicesParm()
        {
            Url(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            SelectDropDownOnField("#ordering1", "Ascending");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }

        public virtual void TestCancelDialog()
        {
            Url(OrdersMenuUrl);
            OpenActionDialog("Orders By Value");
            CancelDialog();
            WaitUntilElementDoesNotExist(".dialog");
        }


        public virtual void ScalarChoicesParmKeepsValue()
        {
            Url(OrdersMenuUrl);
            GetObjectActions(OrderServiceActions);
            OpenActionDialog("Orders By Value");
            SelectDropDownOnField("#ordering1", "Ascending");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Orders By Value");
            AssertTopItemInListIs("SO51782");
        }


        public virtual void ScalarParmShowsDefaultValue()
        {
            Url(CustomersMenuUrl);
            GetObjectActions(CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            var input = WaitForCss(".value input");
            Assert.AreEqual("AW", input.GetAttribute("value"));
        }


        public virtual void DateTimeParmKeepsValue()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Customer-29923&actions1=open");
            OpenActionDialog("Search For Orders");
            var fromDate = WaitForCss("#fromdate1");
            Assert.AreEqual("1 Jan 2000", fromDate.GetAttribute("value")); //Default field value
            ClearFieldThenType("#fromdate1", "1 Sep 2007");
            CancelDatePicker("#fromdate1");
            ClearFieldThenType("#todate1", "1 Apr 2008");
            CancelDatePicker("#todate1");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Search For Orders");
            var details = WaitForCss(".summary .details");
            Assert.AreEqual("Page 1 of 1; viewing 2 of 2 items", details.Text);
        }


        public virtual void RefChoicesParmKeepsValue()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Category");
            SelectDropDownOnField("#subcategory1", "Forks");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "List Products By Sub Category");
            AssertTopItemInListIs("HL Fork");
        }


        public virtual void MultipleRefChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");

            var selected = new SelectElement(WaitForCss("select#subcategories1"));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", selected.AllSelectedOptions.ElementAt(0).Text);
            Assert.AreEqual("Touring Bikes", selected.AllSelectedOptions.ElementAt(1).Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "List Products By Sub Categories");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }


        public virtual void MultipleRefChoicesChangeDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("List Products By Sub Categories");
            WaitForCss(".value  select").SendKeys("Road Bikes");
            IKeyboard kb = ((IHasInputDevices)br).Keyboard;

            kb.PressKey(Keys.Control);
            br.FindElement(By.CssSelector(".value  select option[label='Touring Bikes']")).Click();
            kb.ReleaseKey(Keys.Control);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "List Products By Sub Categories");
            wait.Until(dr => dr.FindElement(By.CssSelector(".summary .details")).Text == "Page 1 of 4; viewing 20 of 65 items");
            AssertTopItemInListIs("Road-150 Red, 44");
        }


        public virtual void ChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            var slctPl = new SelectElement(WaitForCss("select#productline1"));
            var slctPc = new SelectElement(WaitForCss("select#productclass1"));

            Assert.AreEqual("M", slctPl.SelectedOption.Text);
            Assert.AreEqual("H", slctPc.SelectedOption.Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find By Product Line And Class");
            AssertTopItemInListIs("Mountain-300 Black, 38");
        }


        public virtual void ChoicesChangeDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find By Product Line And Class");

            SelectDropDownOnField("#productline1", "R");
            SelectDropDownOnField("#productclass1", "L");

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find By Product Line And Class");
            AssertTopItemInListIs("HL Road Frame - Black, 58");
        }

        public virtual void ConditionalChoices() {
            GeminiUrl("home?menu1=ProductRepository");
            WaitForView(Pane.Single, PaneType.Home);
            OpenActionDialog("List Products");
            SelectDropDownOnField("#category1", "Clothing");
                var x = new SelectElement(WaitForCss("#subcategory1")).Options;
            wait.Until(d => new SelectElement(WaitForCss("#subcategory1")).Options.ElementAt(1).Text == "Bib-Shorts");

            SelectDropDownOnField("#category1", "Accessories");
            wait.Until(d => new SelectElement(WaitForCss("#subcategory1")).Options.ElementAt(1).Text == "Bike Racks");

        }

        public virtual void ConditionalChoicesDefaults()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find Products By Category");
            var slctCs = new SelectElement(WaitForCss("select#categories1"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(WaitForCss("select#subcategories1")).AllSelectedOptions.Count == 2);

            var slct = new SelectElement(WaitForCss("select#subcategories1"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        public virtual void ConditionalChoicesMultiple()
        {
            Url(ProductServiceUrl);

            OpenActionDialog("Find Products By Category");

            var slctCs = new SelectElement(WaitForCss("select#categories1"));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            wait.Until(d => new SelectElement(WaitForCss("select#subcategories1")).AllSelectedOptions.Count == 2);


            var slct = new SelectElement(WaitForCss("select#subcategories1"));

            //Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Products By Category");
            AssertTopItemInListIs("Mountain-100 Black, 38");
        }

        #region Auto Complete


        public virtual void AutoCompleteParm()
        {
            Url(SalesServiceUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("List Accounts For Sales Person");
            ClearFieldThenType("#sp1", "Valdez");
            wait.Until(d => d.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);
            Click(WaitForCss(".ui-menu-item"));

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "List Accounts For Sales Person");
        }


        public virtual void AutoCompleteParmDefault()
        {
            Url(ProductServiceUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            OpenActionDialog("Find Product");

            Assert.AreEqual("Adjustable Race", WaitForCss(".value input[type='text']").GetAttribute("value"));

            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Adjustable Race");
        }


        public virtual void AutoCompleteParmShowSingleItem()
        {
            Url(ProductServiceUrl);
            OpenActionDialog("Find Product");
            ClearFieldThenType("#product1", "BB");
            wait.Until(dr => dr.FindElement(By.CssSelector(".ui-menu-item")).Text == "BB Ball Bearing");
            var item = br.FindElement(By.CssSelector(".ui-menu-item"));
            Click(item);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "BB Ball Bearing");
        }
        #endregion

        #region Parameter validation

        public virtual void MandatoryParameterEnforced()
        {
            GeminiUrl("home?menu1=SalesRepository&dialog1=FindSalesPersonByName");
            wait.Until(dr => dr.FindElement(By.CssSelector("input#firstname1")).GetAttribute("placeholder") == "");
            wait.Until(dr => dr.FindElement(By.CssSelector("input#lastname1")).GetAttribute("placeholder") == "* ");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector("input#lastname1")).GetAttribute("placeholder") == "REQUIRED * ");
            ClearFieldThenType("input#lastname1", "a");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Find Sales Person By Name");
        }


        public virtual void ValidateSingleValueParameter()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Product-342&actions1=open&dialog1=BestSpecialOffer");
            var qty = WaitForCss("input#quantity1");
            qty.SendKeys("0");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameter .validation")).Text.Length > 0);
            var validation = WaitForCss(".parameter .validation");
            Assert.AreEqual("Quantity must be > 0", validation.Text);
            qty = WaitForCss("input#quantity1");
            qty.SendKeys(Keys.Backspace + "1");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "No Discount");
        }


        public virtual void ValidateSingleRefParamFromChoices()
        {
            GeminiUrl("object?object1=AdventureWorksModel.SalesOrderHeader-71742&collection1_SalesOrderHeaderSalesReason=List&actions1=open&dialog1=AddNewSalesReason");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection")).Count == 2);
            SelectDropDownOnField("#reason1", "Price");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameter .validation")).Text.Length > 0);
            var validation = WaitForCss(".parameter .validation");
            Assert.AreEqual("Price already exists in Sales Reasons", validation.Text);
        }


        public virtual void CoValidationOfMultipleParameters()
        {
            GeminiUrl("object?object1=AdventureWorksModel.PurchaseOrderDetail-1632-3660&actions1=open&dialog1=ReceiveGoods");
            ClearFieldThenType("#qtyreceived1", "100");
            ClearFieldThenType("#qtyrejected1", "50");
            ClearFieldThenType("#qtyintostock1", "49");
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".parameters .co-validation")).Text ==
                "Qty Into Stock + Qty Rejected must add up to Qty Received");
        }

        #endregion


        public virtual void ParameterDescriptionRenderedAsPlacholder()
        {
            GeminiUrl("home?menu1=CustomerRepository&dialog1=FindStoreByName");
            var name = WaitForCss("input#name1");
            Assert.AreEqual("* partial match", name.GetAttribute("placeholder"));
        }
    }
    public abstract class DialogTests : DialogTestsRoot
    {

        //This test is a hangover from when the button was named 'Get'
        //for query-only actions, and 'Do' for others.  This has since
        //been reverted to OK for both.
        [TestMethod]
        public override void OKButtonNaming() { base.OKButtonNaming(); }
        [TestMethod]
        public override void ChoicesParm() { base.ChoicesParm(); }
        [TestMethod]
        public override void TestCancelDialog() { base.TestCancelDialog(); }
        [TestMethod]
        public override void ScalarChoicesParmKeepsValue() { base.ScalarChoicesParmKeepsValue(); }
        [TestMethod]
        public override void ScalarParmShowsDefaultValue() { base.ScalarParmShowsDefaultValue(); }
        [TestMethod]
        public override void DateTimeParmKeepsValue() { base.DateTimeParmKeepsValue(); }
        [TestMethod]
        public override void RefChoicesParmKeepsValue() { base.RefChoicesParmKeepsValue(); }
        [TestMethod]
        public override void MultipleRefChoicesDefaults() { base.MultipleRefChoicesDefaults(); }
        [TestMethod]
        public override void MultipleRefChoicesChangeDefaults() { base.MultipleRefChoicesChangeDefaults(); }
        [TestMethod]
        public override void ChoicesDefaults() { base.ChoicesDefaults(); }
        [TestMethod]
        public override void ChoicesChangeDefaults() { base.ChoicesChangeDefaults(); }
        [TestMethod]
        public override void ConditionalChoices() { base.ConditionalChoices(); }
        [TestMethod]
        public override void ConditionalChoicesDefaults() { base.ConditionalChoicesDefaults(); }
        [TestMethod]
        public override void ConditionalChoicesMultiple() { base.ConditionalChoicesMultiple(); }

        #region Auto Complete
        [TestMethod]
        public override void AutoCompleteParm() { base.AutoCompleteParm(); }
        [TestMethod]
        public override void AutoCompleteParmDefault() { base.AutoCompleteParmDefault(); }
        [TestMethod]
        public override void AutoCompleteParmShowSingleItem() { base.AutoCompleteParmShowSingleItem(); }
        #endregion

        #region Parameter validation
        [TestMethod]
        public override void MandatoryParameterEnforced() { base.MandatoryParameterEnforced(); }
        [TestMethod]
        public override void ValidateSingleValueParameter() { base.ValidateSingleValueParameter(); }
        [TestMethod]
        public override void ValidateSingleRefParamFromChoices() { base.ValidateSingleRefParamFromChoices(); }
        [TestMethod]
        public override void CoValidationOfMultipleParameters() { base.CoValidationOfMultipleParameters(); }
        #endregion
        [TestMethod]
        public override void ParameterDescriptionRenderedAsPlacholder() { base.ParameterDescriptionRenderedAsPlacholder(); }
    }

    #region browsers specific subclasses

    // [TestClass, Ignore]
    public class DialogTestsIe : DialogTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
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

    //[TestClass]
    public class DialogTestsFirefox : DialogTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
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
            AWTest.InitialiseClass(context);
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

    #region Mega tests
    public abstract class MegaDialogTestsRoot : DialogTestsRoot
    {
        [TestMethod]
        public void MegaDialogTest()
        {
            base.OKButtonNaming();
            base.ChoicesParm();
            base.TestCancelDialog();
            base.ScalarChoicesParmKeepsValue();
            base.ScalarParmShowsDefaultValue();
            base.DateTimeParmKeepsValue();
            base.RefChoicesParmKeepsValue();
            base.MultipleRefChoicesDefaults();
            base.MultipleRefChoicesChangeDefaults();
            ConditionalChoices();
            base.ChoicesDefaults();
            base.ChoicesChangeDefaults();
            base.ConditionalChoicesDefaults();
            base.ConditionalChoicesMultiple();
            base.AutoCompleteParm();
            base.AutoCompleteParmDefault();
            base.AutoCompleteParmShowSingleItem();
            base.MandatoryParameterEnforced();
            base.ValidateSingleValueParameter();
            base.ValidateSingleRefParamFromChoices();
            base.CoValidationOfMultipleParameters();
            base.ParameterDescriptionRenderedAsPlacholder();
        }
    }

    [TestClass]
    public class MegaDialogTestsFirefox : MegaDialogTestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }
    #endregion
}