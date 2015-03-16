// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Chrome;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class ObjectViewAndActionTests : AWWebTest {
        public abstract void ViewPersistedObject();

        public void DoViewPersistedObject() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            br.AssertContainsObjectView();

            br.AssertPageTitleEquals("Field Trip Store, AW00000546");
            //IWebElement objectView = br.FindElement(By.ClassName("main-content"));
            //IWebElement titleObject = objectView.FindElement(By.ClassName("nof-object"));
            //Assert.AreEqual("Field Trip Store, AW00000546", titleObject.Text);

            br.FindElement(By.CssSelector("#Store-Name")).AssertValueEquals("Field Trip Store");
            br.AssertElementExists(By.CssSelector("[title=Edit]"));
            br.AssertElementDoesNotExist(By.CssSelector("[title=Save]"));
            Assert.AreEqual("nof-menu", br.FindElement(By.Id("Store-Actions")).GetAttribute("class"));

            //View: Via Link
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Linda Mitchell");
            br.FindElement(By.CssSelector("#SalesPerson-CommissionPct")).AssertValueEquals("1.50 %");
            br.AssertElementExists(By.CssSelector("[title=Edit]"));
            Assert.AreEqual("nof-menu", br.FindElement(By.Id("SalesPerson-Actions")).GetAttribute("class"));
            br.ClickTabLink(0);

            // rendered eagerly 
            Assert.AreEqual("nof-collection-table", br.GetInternalCollection("Store-Addresses").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            IWebElement table = br.GetInternalCollection("Store-Addresses").FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);

            // Collection Summary
            br.ViewAsSummary("Store-Addresses");
            br.GetInternalCollection("Store-Addresses").AssertSummaryEquals("1 Customer Address");

            // Collection List
            br.ViewAsList("Store-Addresses");
            Assert.AreEqual("nof-collection-list", br.GetInternalCollection("Store-Addresses").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            table = br.GetInternalCollection("Store-Addresses").FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElement(By.ClassName("nof-object")).Text);

            // Collection Table
            br.ViewAsTable("Store-Addresses");
            Assert.AreEqual("nof-collection-table", br.GetInternalCollection("Store-Addresses").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            table = br.GetInternalCollection("Store-Addresses").FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);
        }

        public abstract void ViewTableHeader();

        public void DoViewTableHeader() {
            Login();
            FindProduct("BK-M38S-46");
            br.AssertContainsObjectView();

            // Collection Table
            // br.ViewAsTable("Product-ProductInventory");
            Assert.AreEqual("nof-collection-table", br.GetInternalCollection("Product-ProductInventory").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            IWebElement table = br.GetInternalCollection("Product-ProductInventory").FindElement(By.TagName("table"));
            Assert.AreEqual(3, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th")).Count);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td")).Count);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td")).Count);

            Assert.AreEqual("Quantity", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual("Location", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text);
            Assert.AreEqual("Shelf", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text);
            Assert.AreEqual("Bin", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text);

            Assert.AreEqual("60", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("Finished Goods Storage", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1].Text);
            Assert.AreEqual("N/A", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual("0", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[3].Text);

            Assert.AreEqual("104", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("Final Assembly", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[1].Text);
            Assert.AreEqual("N/A", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual("0", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[3].Text);
        }

        public abstract void ViewViewModel();

        public void DoViewViewModel() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            wait.ClickAndWait("#Store-QuickOrder button", wd => wd.Title == "AW00000546");
        }

        public abstract void InvokeActionNoParmsNoReturn();

        public void DoInvokeActionNoParmsNoReturn() {
            Login();
            FindOrder("SO59000");
            wait.ClickAndWait("#SalesOrderHeader-Recalculate", wd => true);
            Thread.Sleep(2000);
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("SO59000");
            //No verification that method has actually run - only that it hasn't changed the view!
        }

        public abstract void InvokeActionParmsReturn();

        public void DoInvokeActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.Clear();
            quantity.SendKeys("12" + Keys.Tab);

            wait.ClickAndWait(".nof-ok", ".nof-objectview");

            br.AssertPageTitleEquals("Volume Discount 11 to 14");
        }

        public abstract void ShowActionParmsReturn();

        public void DoShowActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.Clear();
            quantity.SendKeys("12" + Keys.Tab);

            br.ClickApply();

            br.AssertPageTitleEquals("Volume Discount 11 to 14");
            br.AssertElementExists(By.CssSelector(".nof-apply")); // dialog still up 
            br.FindElement(By.CssSelector(".ui-dialog-titlebar-close")).Click();
        }

        public abstract void InvokeActionOnViewModel();

        public void DoInvokeActionOnViewModel() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            br.AssertContainsObjectView();
            wait.ClickAndWait("#Store-QuickOrder button", wd => wd.Title == "AW00000546");

            var number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.Clear();
            number.SendKeys("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
                    
            wait.ClickAndWait(".nof-ok", wd => wd.Title.StartsWith("33 x "));
        }

        public abstract void InvokeActionOnViewModelReturnCollection();

        public void DoInvokeActionOnViewModelReturnCollection() {
            Login();

            FindCustomerByAccountNumber("AW00000546");

            wait.ClickAndWait("#Store-QuickOrder button", wd => wd.Title == "AW00000546");

            var number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.Clear();
            number.SendKeys("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
            wait.ClickAndWaitGone(".nof-ok", "#QuickOrderForm-AddDetail-Number-Input");

            number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.Clear();
            number.SendKeys("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
            wait.ClickAndWaitGone(".nof-ok", "#QuickOrderForm-AddDetail-Number-Input");

            number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.Clear();
            number.SendKeys("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
            wait.ClickAndWait(".nof-ok", wd => wd.Title.StartsWith("33 x "));

            wait.ClickAndWait("#QuickOrderForm-GetOrders button", wd => wd.Title == "3 Order Lines");
        }

        public abstract void InvokeActionParmsMandatory();

        public void DoInvokeActionParmsMandatory() {
            Login();

            FindProduct("BK-M68S-42");

            var ok = wait.ClickAndWait("#Product-BestSpecialOffer button", ".nof-ok");
            var error = wait.ClickAndWait(ok, "span.field-validation-error");
            error.AssertTextEquals("Mandatory");
        }

        public abstract void InvokeActionParmsInvalid();

        public void DoInvokeActionParmsInvalid() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.Clear();
            quantity.SendKeys("not a number" + Keys.Tab);

            var error = wait.ClickAndWait(".nof-ok", "span.field-validation-error");
            error.AssertTextEquals("Invalid Entry");
        }

        public abstract void InvokeContributedActionNoParmsReturnTransient();

        public void DoInvokeContributedActionNoParmsReturnTransient() {
            Login();
            FindProduct("BK-M68S-42");

            var objedit = wait.ClickAndWait("#Product-CreateNewWorkOrder", ".nof-objectedit");
            br.AssertContainsObjectEditTransient();
            Assert.AreEqual("AdventureWorksModel-WorkOrder", objedit.GetAttribute("id"));
        }

        public abstract void InvokeContributedActionNoParmsReturnPersistent();

        public void DoInvokeContributedActionNoParmsReturnPersistent() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            var number = wait.ClickAndWait("#Store-LastOrder", "SalesOrderHeader-SalesOrderNumber-Input");
            number.AssertValueEquals("SO69561");
        }

        public abstract void InvokeContributedActionParmsReturn();

        public void DoInvokeContributedActionParmsReturn() {
            bool useEsc = GetType() == typeof (ObjectViewAndActionTestsChrome);
            string esc = useEsc ? Keys.Escape : "";

            Login();

            FindCustomerByAccountNumber("AW00000546");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");

            obj.AssertObjectHasTitle("Field Trip Store, AW00000546");

            br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-FromDate")).TypeText("01/01/2003", br);
            //  br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-FromDate").AppendText(Keys.Escape, br);

            br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-ToDate")).TypeText("12/12/2003" + Keys.Escape, br);
            //   br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-ToDate").AppendText(Keys.Escape, br);
            br.WaitForAjaxComplete();

            wait.ClickAndWait(".nof-ok", wd => wd.Title == "4 Sales Orders");   
        }

        public abstract void CancelActionDialog();

        public void DoCancelActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000546");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");

            obj.AssertObjectHasTitle("Field Trip Store, AW00000546");

            //br.ClickCancel();
            br.FindElement(By.CssSelector(".ui-dialog-titlebar-close")).Click();

            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Field Trip Store, AW00000546");
        }

        public abstract void EmptyCollectionDoesNotShowListOrTableButtons();

        public void DoEmptyCollectionDoesNotShowListOrTableButtons() {
            Login();
            FindProduct("LW-1000");
            IWebElement reviews = br.FindElement(By.CssSelector("#Product-ProductReviews"));
            reviews.AssertSummaryEquals("No Product Reviews");
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=List]"));
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=Table]"));
        }

        public abstract void RemoveFromActionDialog();

        public void DoRemoveFromActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000546");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");
            obj.AssertObjectHasTitle("Field Trip Store, AW00000546");

            br.FindElement(By.CssSelector("button[title=Remove]")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Mandatory");
        }

        public abstract void RecentlyViewedOnActionDialog();

        public void DoRecentlyViewedOnActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000547");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");
            obj.AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");

            br.FindElement(By.CssSelector("button[title='Recently Viewed']")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");
        }

        public abstract void RecentlyViewedOnActionDialogWithSelect();

        public void DoRecentlyViewedOnActionDialogWithSelect() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            FindCustomerByAccountNumber("AW00000547");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");
            obj.AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");

            br.FindElement(By.CssSelector("button[title='Recently Viewed']")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElements(By.CssSelector("button[title='Select']")).First().BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Field Trip Store, AW00000546");
        }

        public abstract void ActionFindOnActionDialog();

        public void DoActionFindOnActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000547");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");
            obj.AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");

            var accountNumber = wait.ClickAndWait("#Store-SearchForOrders-Customer-CustomerRepository-FindCustomerByAccountNumber", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            accountNumber.Clear();
            accountNumber.SendKeys("AW00000546" + Keys.Tab);

            var customer = wait.ClickAndWait(".nof-ok", "#OrderContributedActions-SearchForOrders-Customer");   

            customer.AssertObjectHasTitle("Field Trip Store, AW00000546");
        }

        public abstract void NewObjectOnActionDialog();

        public void DoNewObjectOnActionDialog() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");


            br.FindElement(By.CssSelector("#Product-Name")).TypeText("test-popup", br);
            br.FindElement(By.CssSelector("#Product-ProductNumber")).TypeText("test-popup", br);
            br.FindElement(By.CssSelector("#Product-ListPrice")).TypeText("10", br);

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-ReorderPoint")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-DaysToManufacture")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-SellStartDate")).TypeText("1/1/2020" + Keys.Escape, br);
            br.FindElement(By.CssSelector("#Product-StandardCost")).TypeText("1", br);
            br.ClickSave();
            br.WaitForAjaxComplete();

            br.FindElements(By.CssSelector("button[title='Select']")).First().BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.Id("WorkOrderRepository-CreateNewWorkOrder-Product")).AssertInputValueEquals("test-popup ");
        }

        public abstract void AutoCompleteOnActionDialog();

        public void DoAutoCompleteOnActionDialog() {
            Login();

            wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");

            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).Clear();
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys("HL");
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.ArrowDown);
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.ArrowDown);
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.Tab);
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product")).AssertInputValueEquals("HL Hub");
        }

        public abstract void NewObjectOnActionDialogFailMandatory();

        public void DoNewObjectOnActionDialogFailMandatory() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");

            br.FindElement(By.CssSelector("#Product-Name")).TypeText("test", br);
            br.FindElement(By.CssSelector("#Product-ProductNumber")).TypeText("test", br);
            br.FindElement(By.CssSelector("#Product-ListPrice")).TypeText("10", br);

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-ReorderPoint")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-DaysToManufacture")).TypeText("1", br);
            //br.FindElement(By.CssSelector("#Product-SellStartDate").TypeText(DateTime.Today.AddDays(1).ToShortDateString(), br); - missing mandatory
            br.FindElement(By.CssSelector("#Product-StandardCost")).TypeText("1", br);

            br.ClickSave();
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Mandatory");
            Assert.AreEqual("test", br.FindElement(By.CssSelector("#Product-Name-Input")).GetAttribute("value"));
        }

        public abstract void NewObjectOnActionDialogFailInvalid();

        public void DoNewObjectOnActionDialogFailInvalid() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");

            br.FindElement(By.CssSelector("#Product-Name")).TypeText("test", br);
            br.FindElement(By.CssSelector("#Product-ProductNumber")).TypeText("test", br);
            br.FindElement(By.CssSelector("#Product-ListPrice")).TypeText("10", br);

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-ReorderPoint")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-DaysToManufacture")).TypeText("1", br);
            br.FindElement(By.CssSelector("#Product-SellStartDate")).TypeText("1" + Keys.Escape, br); // invalid
            br.FindElement(By.CssSelector("#Product-StandardCost")).TypeText("1", br);
            br.ClickSave();
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Invalid Entry");
            Assert.AreEqual("test", br.FindElement(By.CssSelector("#Product-Name-Input")).GetAttribute("value"));
        }

        public abstract void ExpandAndCollapseNestedObject();

        public void DoExpandAndCollapseNestedObject() {
            Login();

            FindCustomerByAccountNumber("AW00000547");

            var label = wait.ClickAndWait("button.nof-maximize", "#Store-SalesPerson-SalesPerson-Employee div.nof-label");

            Assert.AreEqual("Employee Details:", label.Text);

            wait.ClickAndWait("button.nof-minimize", "#Store-SalesPerson-SalesPerson-Employee div.nof-label");


        }


    }
}