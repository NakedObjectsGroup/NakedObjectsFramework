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
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class AjaxTests : AWWebTest {

        public void DoRemoteValidationProperty() {
            Login();

            // click random work order and wait for object view 
            var edit = wait.ClickAndWait("#WorkOrderRepository-RandomWorkOrder button", ".nof-edit");

            // click edit and wait for order qrt input 
            var qty = wait.ClickAndWait(edit, "#WorkOrder-OrderQty-Input");

            // validate not 0
            Assert.AreNotEqual("0", qty.GetAttribute("value"));

            // enter 0
            qty.Clear();
            qty.SendKeys("0" + Keys.Tab);

            // find scrapped qty 
            var scrappedQty = br.FindElement(By.CssSelector("#WorkOrder-ScrappedQty-Input"));

            // enter 0 
            scrappedQty.Clear();
            scrappedQty.SendKeys("0" + Keys.Tab);

            // wait for error message 
            var error = wait.Until(wd => wd.FindElement(By.CssSelector("#WorkOrder-OrderQty  span.field-validation-error")));

            // validate error message 
            Assert.AreEqual("Order Quantity must be > 0", error.Text);
        }

        public void DoRemoteValidationParameter() {
            Login();
            // click find product by number and wait for product number input 
            var pn = wait.ClickAndWait("#ProductRepository-FindProductByNumber button", "#ProductRepository-FindProductByNumber-Number-Input");

            // enter product number 
            pn.Clear();
            pn.SendKeys("LW-1000" + Keys.Tab);

            // click ok and wait for best special offer button 
            var action = wait.ClickAndWait(".nof-ok", "#Product-BestSpecialOffer button");

            // click best special offer and wait for quantity input 
            var qty = wait.ClickAndWait(action, "#Product-BestSpecialOffer-Quantity-Input");

            // validate no 0 
            Assert.AreNotEqual("0", qty.GetAttribute("value"));

            // enter 0
            qty.Clear();
            qty.SendKeys("0" + Keys.Tab);

            // wait for error message 
            var error = wait.Until(wd => wd.FindElement(By.CssSelector("#Product-BestSpecialOffer-Quantity  span.field-validation-error")));

            Assert.AreEqual("Quantity must be > 0", error.Text);
        }

        public void DoActionChoices() {
            Login();

            // first test so get everything started with a longer timeout

            try {

                wait = new SafeWebDriverWait(br, new TimeSpan(0, 0, 30));

                var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

                orderNumber.Clear();
                orderNumber.SendKeys("SO63557" + Keys.Tab);

                var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddNewSalesReason button");

                var reason = wait.ClickAndWait(action, "#SalesOrderHeader-AddNewSalesReason-Reason");

                reason.AssertIsEmpty();
                reason.SelectDropDownItem("Price", br);

                wait.ClickAndWait(".nof-ok", ".nof-objectview");
            }
            finally {
                // make sure we put time out back 
                wait = new SafeWebDriverWait(br, DefaultTimeOut);
            }
        }

        public void DoActionMultipleChoices() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO72847" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddNewSalesReasons button");

            var reason = wait.ClickAndWait(action, "#SalesOrderHeader-AddNewSalesReasons-Reasons");

            reason.AssertIsEmpty();

            reason.SelectListBoxItems(br, "Price", "Other");
            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoActionMultipleChoicesEnum() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO72847" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddNewSalesReasonsByCategories button");

            var cats = wait.ClickAndWait(action, "#SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories");

            cats.AssertIsEmpty();

            cats.SelectListBoxItems(br, "Marketing", "Promotion");
            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoActionConditionalMultipleChoices() {
            Login();

            var categories = wait.ClickAndWait("#ProductRepository-FindProductsByCategory button", "#ProductRepository-FindProductsByCategory-Categories");

            categories.AssertIsEmpty();

            IWebElement subcategories = br.FindElement(By.CssSelector("#ProductRepository-FindProductsByCategory-Subcategories"));

            subcategories.AssertIsEmpty();

            wait.Until(wd => subcategories.FindElements(By.TagName("option")).Count >= 4);

            ReadOnlyCollection<IWebElement> options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(4, subcategories.FindElements(By.TagName("option")).Count);

            Assert.IsTrue(options.Any(we => we.Text == ""));
            Assert.IsTrue(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Road Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Touring Bikes"));

            categories.SelectListBoxItems(br, "Bikes", "Components"); // unselect bikes select components 

            wait.Until(wd => subcategories.FindElements(By.TagName("option")).Count >= 15);

            options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(15, options.Count);

            Assert.IsFalse(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsTrue(options.Any(we => we.Text == "Handlebars"));

            categories.SelectListBoxItems(br, "Components", "Clothing", "Accessories"); // unselect components 

            wait.Until(wd => subcategories.FindElements(By.TagName("option")).Count >= 21);

            options = subcategories.FindElements(By.TagName("option"));
            Assert.AreEqual(21, options.Count);

            Assert.IsFalse(options.Any(we => we.Text == "Mountain Bikes"));
            Assert.IsFalse(options.Any(we => we.Text == "Handlebars"));
            Assert.IsTrue(options.Any(we => we.Text == "Caps"));
            Assert.IsTrue(options.Any(we => we.Text == "Lights"));

            subcategories.SelectListBoxItems(br, "Jerseys", "Shorts", "Socks", "Tights", "Vests");

            br.FindElement(By.CssSelector(".nof-ok")).Click();

            wait.Until(wd => wd.Title == "20 Products");

            Assert.AreEqual("Find Products By Category: Query Result: Viewing 20 of 25 Products", br.FindElement(By.CssSelector("div.nof-object")).Text);

            br.FindElement(By.CssSelector("button[title=Last]")).Click();
            wait.Until(wd => wd.Title == "5 Products");

            Assert.AreEqual("Find Products By Category: Query Result: Viewing 5 of 25 Products", br.FindElement(By.CssSelector("div.nof-object")).Text);
            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 2 of 2", pageNo.Text);
        }

        private void DoActionValidateFail() {
            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            f.Clear();
            f.SendKeys("AW00000546" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#Store-SearchForOrders button");

            SetDates(action, "30/6/2013", "1/6/2013");

            wait.Until(wd => wd.FindElement(By.CssSelector(".nof-ok")) != null);

            var error = wait.ClickAndWait(".nof-ok", ".validation-summary-errors");

            const string expected = "Action was unsuccessful. Please correct the errors and try again.\r\n'From Date' must be before 'To Date'";
            Assert.AreEqual(expected, error.Text);
        }

        private void SetDates(IWebElement action, string fromDate, string toDate) {
            var from = wait.ClickAndWait(action, "#OrderContributedActions-SearchForOrders-FromDate-Input");

            from.Clear();
            from.SendKeys(fromDate + Keys.Tab);

            var to = br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-ToDate-Input"));

            to.Clear();
            to.SendKeys(toDate + Keys.Tab);
        }

        private void SetDates(string fromDate, string toDate) {
            var from = br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-FromDate-Input"));

            from.Clear();
            from.SendKeys(fromDate + Keys.Tab);

            var to = br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-ToDate-Input"));

            to.Clear();
            to.SendKeys(toDate + Keys.Tab);

            Thread.Sleep(1000);
        }

        public void DoActionCrossValidateFail() {
            Login();
            DoActionValidateFail();

            SetDates("1/6/2013", "30/6/2013");

            var apply = wait.Until(wd => wd.FindElement(By.CssSelector(".nof-apply")));
            apply.Click();

            var errors = wait.Until(wd => wd.FindElements(By.CssSelector(".validation-summary-errors")));

            Assert.AreEqual(0, errors.Count, "No errors expected");

            SetDates("28/6/2013", "2/6/2013");

            apply = wait.Until(wd => wd.FindElement(By.CssSelector(".nof-apply")));
            apply.Click();

            wait.Until(wd => wd.FindElements(By.CssSelector(".validation-summary-errors")).Count > 0);

            var error = wait.Until(wd => wd.FindElement(By.CssSelector(".validation-summary-errors")));
            const string expected = "Action was unsuccessful. Please correct the errors and try again.\r\n'From Date' must be before 'To Date'";

            Assert.AreEqual(expected, error.Text);

            SetDates("1/6/2013", "30/6/2013");

            var ok = wait.Until(wd => wd.FindElement(By.CssSelector(".nof-ok")));

            wait.ClickAndWait(ok, ".validation-summary-errors");

            br.AssertPageTitleEquals("No Sales Orders");
            Assert.AreEqual("Search For Orders: Query Result: Viewing 0 of 0 Sales Orders", br.GetTopObject().Text);

            wait.Until(wd => wd.FindElements(By.CssSelector(".validation-summary-errors")).Count == 0);

            errors = br.FindElements(By.CssSelector(".validation-summary-errors"));
            Assert.AreEqual(0, errors.Count, "No errors expected");
        }

        public void DoActionMultipleChoicesDefaults() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO72847" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddStandardComments button");
            var comments = wait.ClickAndWait(action, "#SalesOrderHeader-AddStandardComments-Comments");

            Assert.AreEqual(4, comments.FindElements(By.TagName("option")).Count());
            Assert.AreEqual(2, comments.FindElements(By.CssSelector("option[selected=selected]")).Count());
        }

        public void DoActionMultipleChoicesValidateFail() {
            Login();

            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO47185" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddNewSalesReasons button");
            var reason = wait.ClickAndWait(action, "#SalesOrderHeader-AddNewSalesReasons-Reasons");

            reason.SelectListBoxItems(br, "Review");

            var ok = wait.Until(wd => wd.FindElement(By.CssSelector(".nof-ok")));
            var valMsg = wait.ClickAndWait(ok, ".field-validation-error");

            Assert.AreEqual("Review already exists in Sales Reasons", valMsg.Text);
        }

        public void DoActionChoicesEnum() {
            Login();
            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO72847" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-AddNewSalesReasonsByCategories button");

            var reason = wait.ClickAndWait(action, "#SalesOrderHeader-AddNewSalesReasonsByCategories-ReasonCategories");

            reason.SelectListBoxItems(br, "Marketing", "Other");
            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoActionMultipleChoicesConditionalEnum() {
            Login();

            var productLine = wait.ClickAndWait("#ProductRepository-FindByProductLinesAndClasses button", "#ProductRepository-FindByProductLinesAndClasses-ProductLine");
            var productClass = br.FindElement(By.CssSelector("#ProductRepository-FindByProductLinesAndClasses-ProductClass"));

            productLine.AssertIsEmpty();
            productClass.AssertIsEmpty();
            // unselect defaults 
            productLine.SelectListBoxItems(br, "M", "S");
            productClass.SelectListBoxItems(br, "H");
            // then select these
            productLine.SelectListBoxItems(br, "M");
            productClass.SelectListBoxItems(br, "L");

            var ok = wait.Until(wd => wd.FindElement(By.CssSelector(".nof-ok")));

            ok.Click();

            wait.Until(wd => wd.Title == "20 Products");

            Assert.AreEqual("Find By Product Lines And Classes: Query Result: Viewing 20 of 26 Products", br.GetTopObject().Text);
        }

        public void DoActionMultipleChoicesDomainObject() {
            Login();
            var orderNumber = wait.ClickAndWait("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input");

            orderNumber.Clear();
            orderNumber.SendKeys("SO72847" + Keys.Tab);

            var action = wait.ClickAndWait(".nof-ok", "#SalesOrderHeader-RemoveDetails button");
            var reason = wait.ClickAndWait(action, "#SalesOrderHeader-RemoveDetails-DetailsToRemove");

            reason.AssertIsEmpty();

            reason.SelectListBoxItems(br, "1 x Touring-2000 Blue, 46");

            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoClientSideValidation() {
            Login();

            var pn = wait.ClickAndWait("#ProductRepository-FindProductByNumber button", "#ProductRepository-FindProductByNumber-Number-Input");

            // enter product number 
            pn.Clear();
            pn.SendKeys("LW-1000" + Keys.Tab);

            // click ok and wait for best special offer button 
            var edit = wait.ClickAndWait(".nof-ok", ".nof-edit");

            var days = wait.ClickAndWait(edit, "#Product-DaysToManufacture-Input");

            Assert.AreEqual("0", days.GetAttribute("value"));

            days.Clear();
            days.SendKeys("100" + Keys.Tab);

            var valMsg = wait.Until(wd => wd.FindElement(By.CssSelector("#Product-DaysToManufacture .field-validation-error")));

            Assert.AreEqual("Value is outside the range 1 to 90", valMsg.Text);
        }

        #region abstract

        public abstract void RemoteValidationProperty();

        public abstract void RemoteValidationParameter();

        public abstract void ActionChoices();

        public abstract void ActionMultipleChoices();

        public abstract void ActionConditionalMultipleChoices();

        public abstract void ActionMultipleChoicesValidateFail();

        public abstract void ActionCrossValidateFail();

        public abstract void ActionMultipleChoicesDefaults();

        public abstract void ActionMultipleChoicesEnum();

        public abstract void ActionMultipleChoicesConditionalEnum();

        public abstract void ActionMultipleChoicesDomainObject();

        public abstract void ClientSideValidation();

        #endregion
    }
}