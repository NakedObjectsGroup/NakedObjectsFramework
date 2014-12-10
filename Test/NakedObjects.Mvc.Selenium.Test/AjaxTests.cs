// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class AjaxTests : AWWebTest {
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        public abstract void RemoteValidationProperty();

        public void DoRemoteValidationProperty() {
            Login();
            br.ClickAction("WorkOrderRepository-RandomWorkOrder");
            br.AssertContainsObjectView();
            br.ClickEdit();
            IWebElement qty = br.GetField("WorkOrder-OrderQty");
            qty.AssertInputValueNotEquals("0");
            qty.TypeText("0", br);
            qty.AppendText(Keys.Tab, br);
            br.GetField("WorkOrder-ScrappedQty").TypeText("0", br);
            br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // to move focus off field - tab doesn't seem to work on all browsers 
            br.WaitForAjaxComplete();
            IWebElement valMsg = br.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Order Quantity must be > 0", valMsg.Text);
        }

        public abstract void RemoteValidationParameter();

        public void DoRemoteValidationParameter() {
            Login();
            br.TogglePopups(false);
            FindProduct("LW-1000");
            br.ClickAction("Product-BestSpecialOffer");
            IWebElement qty = br.GetField("Product-BestSpecialOffer-Quantity");
            qty.AssertInputValueNotEquals("0");
            qty.TypeText("0", br);
            qty.AppendText(Keys.Tab, br);
            br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // to move focus off field - tab doesn't seem to work on all browsers 
            br.WaitForAjaxComplete();
            IWebElement valMsg = br.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Quantity must be > 0", valMsg.Text);
        }

        public abstract void RemoteValidationParameterPopup();


        public void DoRemoteValidationParameterPopup() {
            Login();
            br.TogglePopups(true);
            FindProduct("LW-1000");
            br.ClickAction("Product-BestSpecialOffer");
            IWebElement qty = br.GetField("Product-BestSpecialOffer-Quantity");
            qty.AssertInputValueNotEquals("0");
            qty.TypeText("0", br);
            qty.AppendText(Keys.Tab, br);
            //br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // to move focus off field - tab doesn't seem to work on all browsers 
            br.WaitForAjaxComplete();
            IWebElement valMsg = br.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Quantity must be > 0", valMsg.Text);
        }


        public abstract void ActionChoices();

        public void DoActionChoices() {
            Login();
            br.TogglePopups(false);
            FindOrder("SO63557");
            br.ClickAction("SalesOrderHeader-AddNewSalesReason");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReason-Reason").AssertIsEmpty();
            reason.SelectDropDownItem("Price", br);
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionChoicesPopup();

        public void DoActionChoicesPopup() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO63557");
            br.ClickAction("SalesOrderHeader-AddNewSalesReason");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReason-Reason").AssertIsEmpty();
            reason.SelectDropDownItem("Price", br);
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionMultipleChoices();

        public void DoActionMultipleChoices() {
            Login();
            br.TogglePopups(false);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasons");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasons-Reasons").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Price", "Other");
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionConditionalMultipleChoices();

        public void DoActionConditionalMultipleChoices() {
            Login();
            br.TogglePopups(true);
            br.ClickAction("ProductRepository-FindProductsByCategory");
            IWebElement categories = br.GetField("ProductRepository-FindProductsByCategory-Categories").AssertIsEmpty();
            //categories.SelectListBoxItems(br, "Bikes");
            // selected by default
            br.WaitForAjaxComplete();

            IWebElement subcategories = br.GetField("ProductRepository-FindProductsByCategory-Subcategories").AssertIsEmpty();
            ReadOnlyCollection<IWebElement> options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(4, options.Count);

            Assert.IsTrue(options.Any(we => we.Text == ""));
            Assert.IsTrue(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Road Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Touring Bikes"));

            categories.SelectListBoxItems(br, "Bikes", "Components"); // unselect bikes select components 

            options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(15, options.Count);

            Assert.IsFalse(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Handlebars"));

            categories.SelectListBoxItems(br, "Components", "Clothing", "Accessories"); // unselect components 

            options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(21, options.Count);

            Assert.IsFalse(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsFalse(options.Any(we => we.Text == "Handlebars"));
            Assert.IsTrue(options.Any(we => we.Text == "Caps"));
            Assert.IsTrue(options.Any(we => we.Text == "Lights"));

            subcategories.SelectListBoxItems(br, "Jerseys", "Shorts", "Socks", "Tights", "Vests");

            br.ClickOk();

            br.AssertPageTitleEquals("20 Products");
            Assert.AreEqual("Find Products By Category: Query Result: Viewing 20 of 25 Products", br.GetTopObject().Text);

            br.ClickLast();
            br.AssertPageTitleEquals("5 Products");
            Assert.AreEqual("Find Products By Category: Query Result: Viewing 5 of 25 Products", br.GetTopObject().Text);
            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 2 of 2", pageNo.Text);
        }

        public abstract void ActionMultipleChoicesValidateFail();

        public void DoActionMultipleChoicesValidateFail() {
            Login();
            br.TogglePopups(false);
            FindOrder("SO47185");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasons");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasons-Reasons").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Review");
            br.ClickOk();
            IWebElement valMsg = br.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Review already exists in Sales Reasons", valMsg.Text);
        }

        public abstract void ActionCrossValidateFail();

        public void DoActionCrossValidateFail() {
            Login();
            br.TogglePopups(false);
            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-SearchForOrders");

            IWebElement fromDate = br.GetField("OrderContributedActions-SearchForOrders-FromDate");
            fromDate.TypeText("30/6/2013", br);
            IWebElement toDate = br.GetField("OrderContributedActions-SearchForOrders-ToDate");
            toDate.TypeText("1/6/2013" + Keys.Tab, br);

            Thread.Sleep(1000); // wait for datepicker to close

            br.ClickOk();
            IWebElement valMsg = br.FindElement(By.ClassName("validation-summary-errors"));
            const string expected = "Action was unsuccessful. Please correct the errors and try again.\r\n'From Date' must be before 'To Date'";
            Assert.AreEqual(expected, valMsg.Text);
        }

        public abstract void ActionCrossValidateFailPopup();

        public void DoActionCrossValidateFailPopup() {
            Login();
            br.TogglePopups(true);
            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-SearchForOrders");

            IWebElement fromDate = br.GetField("OrderContributedActions-SearchForOrders-FromDate");
            fromDate.TypeText("30/6/2013", br);
            IWebElement toDate = br.GetField("OrderContributedActions-SearchForOrders-ToDate");
            toDate.TypeText("1/6/2013" + Keys.Escape, br);

            Thread.Sleep(1000); // wait for datepicker to close
            br.ClickOk();
            Thread.Sleep(1000);
            IWebElement valMsg = br.FindElement(By.ClassName("validation-summary-errors"));

            const string expected = "Action was unsuccessful. Please correct the errors and try again.\r\n'From Date' must be before 'To Date'";
            Assert.AreEqual(expected, valMsg.Text);

            fromDate = br.GetField("OrderContributedActions-SearchForOrders-FromDate");
            fromDate.TypeText("1/6/2013", br);
            toDate = br.GetField("OrderContributedActions-SearchForOrders-ToDate");
            toDate.TypeText("30/6/2013" + Keys.Tab, br);

            Thread.Sleep(1000); // wait for datepicker to close
            br.ClickApply();

            ReadOnlyCollection<IWebElement> errors = br.FindElements(By.ClassName("validation-summary-errors"));
            Assert.AreEqual(0, errors.Count, "No errors expected");

            fromDate = br.GetField("OrderContributedActions-SearchForOrders-FromDate");
            fromDate.TypeText("28/6/2013", br);
            toDate = br.GetField("OrderContributedActions-SearchForOrders-ToDate");
            toDate.TypeText("2/6/2013" + Keys.Tab, br);

            Thread.Sleep(1000); // wait for datepicker to close
            br.ClickApply();

            valMsg = br.FindElement(By.ClassName("validation-summary-errors"));
            Assert.AreEqual(expected, valMsg.Text);

            fromDate = br.GetField("OrderContributedActions-SearchForOrders-FromDate");
            fromDate.TypeText("1/6/2013", br);
            toDate = br.GetField("OrderContributedActions-SearchForOrders-ToDate");
            toDate.TypeText("30/6/2013" + Keys.Tab, br);

            Thread.Sleep(1000); // wait for datepicker to close
            br.ClickOk();

            br.AssertPageTitleEquals("No Sales Orders");
            Assert.AreEqual("Search For Orders: Query Result: Viewing 0 of 0 Sales Orders", br.GetTopObject().Text);

            errors = br.FindElements(By.ClassName("validation-summary-errors"));
            Assert.AreEqual(0, errors.Count, "No errors expected");
        }


        public abstract void ActionMultipleChoicesPopup();

        public void DoActionMultipleChoicesPopup() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasons");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasons-Reasons").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Price", "Other");
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionMultipleChoicesPopupDefaults();

        public void DoActionMultipleChoicesPopupDefaults() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-AddStandardComments");
            IWebElement comments = br.GetField("SalesOrderHeader-AddStandardComments-Comments").AssertIsEmpty();

            Assert.AreEqual(4, comments.FindElements(By.TagName("option")).Count());
            Assert.AreEqual(2, comments.FindElements(By.CssSelector("option[selected=selected]")).Count());
        }


        public abstract void ActionMultipleChoicesPopupValidateFail();

        public void DoActionMultipleChoicesPopupValidateFail() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO47185");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasons");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasons-Reasons").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Review");
            br.ClickOk();
            IWebElement valMsg = br.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Review already exists in Sales Reasons", valMsg.Text);
        }

        public abstract void ActionMultipleChoicesEnum();

        public void DoActionMultipleChoicesEnum() {
            Login();
            br.TogglePopups(false);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasonsByCategories");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Marketing", "Other");
            br.ClickOk();
            br.AssertContainsObjectView();
        }


        public abstract void ActionMultipleChoicesPopupEnum();

        public void DoActionMultipleChoicesPopupEnum() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-AddNewSalesReasonsByCategories");
            IWebElement reason = br.GetField("SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories").AssertIsEmpty();
            reason.SelectListBoxItems(br, "Marketing", "Other");
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionMultipleChoicesPopupConditionalEnum();

        public void DoActionMultipleChoicesPopupConditionalEnum() {
            Login();
            br.TogglePopups(true);
            br.ClickAction("ProductRepository-FindByProductLinesAndClasses");
            IWebElement productLine = br.GetField("ProductRepository-FindByProductLinesAndClasses-ProductLine").AssertIsEmpty();
            IWebElement productClass = br.GetField("ProductRepository-FindByProductLinesAndClasses-ProductClass").AssertIsEmpty();
            // unselect defaults 
            productLine.SelectListBoxItems(br, "M", "S");
            productClass.SelectListBoxItems(br, "H");
            // then select these
            productLine.SelectListBoxItems(br, "M");
            productClass.SelectListBoxItems(br, "L");
            br.ClickOk();
            br.AssertPageTitleEquals("20 Products");
            Assert.AreEqual("Find By Product Lines And Classes: Query Result: Viewing 20 of 26 Products", br.GetTopObject().Text);
        }

        public abstract void ActionMultipleChoicesDomainObject();

        public void DoActionMultipleChoicesDomainObject() {
            Login();
            br.TogglePopups(false);
            FindOrder("SO72847");
            br.ClickAction("SalesOrderHeader-RemoveDetails");
            IWebElement reason = br.GetField("SalesOrderHeader-RemoveDetails-DetailsToRemove").AssertIsEmpty();
            reason.SelectListBoxItems(br, "1 x Touring-2000 Blue, 46");
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ActionMultipleChoicesPopupDomainObject();

        public void DoActionMultipleChoicesPopupDomainObject() {
            Login();
            br.TogglePopups(true);
            FindOrder("SO72848");
            br.ClickAction("SalesOrderHeader-RemoveDetails");
            IWebElement reason = br.GetField("SalesOrderHeader-RemoveDetails-DetailsToRemove").AssertIsEmpty();
            reason.SelectListBoxItems(br, "1 x Touring-1000 Yellow, 54");
            br.ClickOk();
            br.AssertContainsObjectView();
        }

        public abstract void ClientSideValidation();

        public void DoClientSideValidation() {
            Login();
            FindProduct("LW-1000");
            br.ClickEdit();
            IWebElement days = br.GetField("Product-DaysToManufacture");
            days.AssertInputValueEquals("0");
            days.TypeText("100", br);
            days.AppendText(Keys.Tab, br);
            br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // to move focus off field - tab doesn't seem to work on all browsers 
            br.WaitForAjaxComplete();
            IWebElement valMsg = days.FindElement(By.ClassName("field-validation-error"));
            Assert.AreEqual("Value is outside the range 1 to 90", valMsg.Text);
        }

        public abstract void CanGoBackToDialog();

        public void DoCanGoBackToDialog() {
            Login();
            br.TogglePopups(false);
            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-CreateNewOrder");
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            br.ClickOk();
            br.Navigate().Back();
            br.WaitForAjaxComplete();
            br.AssertElementExists(By.Id("Store-CreateNewOrder-Dialog"));
            br.TogglePopups(false);
        }

        public abstract void GoingBackToDialogPreservesEnteredValues();

        public void DoGoingBackToDialogPreservesEnteredValues() {
            Login();
            br.TogglePopups(false);
            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-CreateNewOrder");
            IWebElement checkBox = br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            Assert.IsTrue(checkBox.Selected);
            //OK the action with checkbox checked
            br.ClickOk();
            br.Navigate().Back();
            br.WaitForAjaxComplete();

            br.AssertElementExists(By.Id("Store-CreateNewOrder-Dialog"));
            checkBox = br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            Assert.IsTrue(checkBox.Selected);

            //Now repeat with checkbox unchecked
            checkBox.SendKeys(Keys.Space);
            Assert.IsFalse(checkBox.Selected);
            br.ClickOk();
            br.Navigate().Back();
            br.WaitForAjaxComplete();

            br.AssertElementExists(By.Id("Store-CreateNewOrder-Dialog"));
            checkBox = br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-CopyHeaderFromLastOrder-Input"));
            Assert.IsFalse(checkBox.Selected);
            br.TogglePopups(false);
        }

        public abstract void DragDropOk();

        public void DoDragDropOk() {
            Login();
            FindSalesPerson("Ito");
            FindCustomerByAccountNumber("AW00000546");
            br.ClickEdit();
            IWebElement toDrag = br.GetHistory().FindElements(By.ClassName("nof-object")).First().FindElement(By.TagName("a"));
            IWebElement toDrop = br.GetField("Store-SalesPerson").FindElements(By.ClassName("nof-object")).First();
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Linda Mitchell");

            (new Actions(br)).DragAndDrop(toDrag, toDrop).Build().Perform();

            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Shu Ito");
        }

        public abstract void DragDropWrongType();

        public void DoDragDropWrongType() {
            Login();
            FindSalesPerson("Ito");
            FindCustomerByAccountNumber("AW00000546");
            br.ClickEdit();
            IWebElement toDrag = br.GetHistory().FindElements(By.ClassName("nof-object")).Last();
            IWebElement toDrop = br.GetField("Store-SalesPerson").FindElements(By.TagName("div.nof-object")).First();
            toDrop.AssertObjectHasTitle("Linda Mitchell");

            (new Actions(br)).DragAndDrop(toDrag, toDrop);

            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Linda Mitchell");
        }
    }

    [TestClass, Ignore]
    public class AjaxTestsIE : AjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void RemoteValidationProperty() {
            DoRemoteValidationProperty();
        }

        [TestMethod]
        public override void RemoteValidationParameter() {
            DoRemoteValidationParameter();
        }

        [TestMethod]
        public override void RemoteValidationParameterPopup() {
            DoRemoteValidationParameterPopup();
        }

        [TestMethod]
        public override void ActionChoices() {
            DoActionChoices();
        }

        [TestMethod]
        public override void ActionChoicesPopup() {
            DoActionChoicesPopup();
        }

        [TestMethod]
        public override void ActionMultipleChoices() {
            DoActionMultipleChoices();
        }

        [TestMethod]
        public override void ActionConditionalMultipleChoices() {
            DoActionConditionalMultipleChoices();
        }

        [TestMethod]
        public override void ActionCrossValidateFail() {
            DoActionCrossValidateFail();
        }

        [TestMethod]
        public override void ActionCrossValidateFailPopup() {
            DoActionCrossValidateFailPopup();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopup() {
            DoActionMultipleChoicesPopup();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupDefaults() {
            DoActionMultipleChoicesPopupDefaults();
        }

        [TestMethod]
        public override void ActionMultipleChoicesEnum() {
            DoActionMultipleChoicesEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupEnum() {
            DoActionMultipleChoicesPopupEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupConditionalEnum() {
            DoActionMultipleChoicesPopupConditionalEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesDomainObject() {
            DoActionMultipleChoicesDomainObject();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupDomainObject() {
            DoActionMultipleChoicesPopupDomainObject();
        }

        [TestMethod]
        public override void ActionMultipleChoicesValidateFail() {
            DoActionMultipleChoicesValidateFail();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupValidateFail() {
            DoActionMultipleChoicesPopupValidateFail();
        }

        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }

        [TestMethod]
        public override void CanGoBackToDialog() {
            DoCanGoBackToDialog();
        }

        [TestMethod]
        public override void GoingBackToDialogPreservesEnteredValues() {
            DoGoingBackToDialogPreservesEnteredValues();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropOk() {
            DoDragDropOk();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropWrongType() {
            DoDragDropWrongType();
        }
    }

    [TestClass]
    public class AjaxTestsFirefox : AjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
            //br.Manage().Window.Maximize();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }


        [TestMethod]
        public override void RemoteValidationProperty() {
            DoRemoteValidationProperty();
        }

        [TestMethod, Ignore]
        public override void RemoteValidationParameter() {
            DoRemoteValidationParameter();
        }

        [TestMethod]
        public override void RemoteValidationParameterPopup() {
            DoRemoteValidationParameterPopup();
        }

        [TestMethod, Ignore] // broken in firefox - retest later version selenium
        public override void ActionChoices() {
            DoActionChoices();
        }

        [TestMethod]
        public override void ActionChoicesPopup() {
            DoActionChoicesPopup();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoices() {
            DoActionMultipleChoices();
        }

        [TestMethod, Ignore]
        public override void ActionConditionalMultipleChoices() {
            DoActionConditionalMultipleChoices();
        }

        [TestMethod]
        public override void ActionCrossValidateFail() {
            DoActionCrossValidateFail();
        }

        [TestMethod]
        public override void ActionCrossValidateFailPopup() {
            DoActionCrossValidateFailPopup();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopup() {
            DoActionMultipleChoicesPopup();
        }


        [TestMethod]
        public override void ActionMultipleChoicesPopupDefaults() {
            DoActionMultipleChoicesPopupDefaults();
        }


        [TestMethod, Ignore]
        public override void ActionMultipleChoicesEnum() {
            DoActionMultipleChoicesEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupEnum() {
            DoActionMultipleChoicesPopupEnum();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopupConditionalEnum() {
            DoActionMultipleChoicesPopupConditionalEnum();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesValidateFail() {
            DoActionMultipleChoicesValidateFail();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopupValidateFail() {
            DoActionMultipleChoicesPopupValidateFail();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesDomainObject() {
            DoActionMultipleChoicesDomainObject();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupDomainObject() {
            DoActionMultipleChoicesPopupDomainObject();
        }


        [TestMethod]
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }

        [TestMethod]
        public override void CanGoBackToDialog() {
            DoCanGoBackToDialog();
        }

        [TestMethod]
        public override void GoingBackToDialogPreservesEnteredValues() {
            DoGoingBackToDialogPreservesEnteredValues();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropOk() {
            DoDragDropOk();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropWrongType() {
            DoDragDropWrongType();
        }
    }

    [TestClass]
    public class AjaxTestsChrome : AjaxTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AjaxTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        [Ignore] // temp ignore failing in chrome
        public override void RemoteValidationProperty() {
            DoRemoteValidationProperty();
        }

        [TestMethod]
        [Ignore] // temp ignore failing in chrome
        public override void RemoteValidationParameter() {
            DoRemoteValidationParameter();
        }

        [TestMethod]
        [Ignore] // temp ignore failing in chrome
        public override void RemoteValidationParameterPopup() {
            DoRemoteValidationParameterPopup();
        }

        [TestMethod]
        public override void ActionChoices() {
            DoActionChoices();
        }

        [TestMethod]
        public override void ActionChoicesPopup() {
            DoActionChoicesPopup();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoices() {
            DoActionMultipleChoices();
        }

        [TestMethod]
        [Ignore] // cannot select options in multiple choive boxes with chrome driver 
        public override void ActionConditionalMultipleChoices() {
            DoActionConditionalMultipleChoices();
        }

        [TestMethod]
        public override void ActionCrossValidateFail() {
            DoActionCrossValidateFail();
        }

        [TestMethod]
        [Ignore] // temp ignore failing in chrome
        public override void ActionCrossValidateFailPopup() {
            DoActionCrossValidateFailPopup();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopup() {
            DoActionMultipleChoicesPopup();
        }


        [TestMethod]
        public override void ActionMultipleChoicesPopupDefaults() {
            DoActionMultipleChoicesPopupDefaults();
        }


        [TestMethod, Ignore]
        public override void ActionMultipleChoicesEnum() {
            DoActionMultipleChoicesEnum();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupEnum() {
            DoActionMultipleChoicesPopupEnum();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopupConditionalEnum() {
            DoActionMultipleChoicesPopupConditionalEnum();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesValidateFail() {
            DoActionMultipleChoicesValidateFail();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesPopupValidateFail() {
            DoActionMultipleChoicesPopupValidateFail();
        }

        [TestMethod, Ignore]
        public override void ActionMultipleChoicesDomainObject() {
            DoActionMultipleChoicesDomainObject();
        }

        [TestMethod]
        public override void ActionMultipleChoicesPopupDomainObject() {
            DoActionMultipleChoicesPopupDomainObject();
        }

        [TestMethod]
        [Ignore] // temp ignore failing in chrome
        public override void ClientSideValidation() {
            DoClientSideValidation();
        }

        [TestMethod]
        public override void CanGoBackToDialog() {
            DoCanGoBackToDialog();
        }

        [TestMethod]
        public override void GoingBackToDialogPreservesEnteredValues() {
            DoGoingBackToDialogPreservesEnteredValues();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropOk() {
            DoDragDropOk();
        }

        [TestMethod, Ignore] // doesn't work try on later veersion of selenium
        public override void DragDropWrongType() {
            DoDragDropWrongType();
        }
    }
}