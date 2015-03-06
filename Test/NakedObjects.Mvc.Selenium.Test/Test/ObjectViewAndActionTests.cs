// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
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

            br.GetField("Store-Name").AssertValueEquals("Field Trip Store");
            br.AssertElementExists(By.CssSelector("[title=Edit]"));
            br.AssertElementDoesNotExist(By.CssSelector("[title=Save]"));
            Assert.AreEqual("nof-menu", br.FindElement(By.Id("Store-Actions")).GetAttribute("class"));

            //View: Via Link
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Linda Mitchell");
            br.GetField("SalesPerson-CommissionPct").AssertValueEquals("1.50 %");
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
            br.AssertContainsObjectView();
            br.ClickAction("Store-QuickOrder");
            br.AssertPageTitleEquals("AW00000546");
        }

        public abstract void InvokeActionNoParmsNoReturn();

        public void DoInvokeActionNoParmsNoReturn() {
            Login();
            FindOrder("SO59000");
            br.GetAction("SalesOrderHeader-Recalculate").BrowserSpecificClick(br);
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("SO59000");
            //TODO:  No verification that method has actually run - only that it hasn't changed the view!
        }

        public abstract void InvokeActionParmsReturn();

        public void DoInvokeActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");
            br.ClickAction("Product-BestSpecialOffer").GetField("Product-BestSpecialOffer-Quantity").TypeText("12", br);
            //br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // workaround for not picking up ok click 
            br.ClickOk();
            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Volume Discount 11 to 14");
        }

        public abstract void ShowActionParmsReturn();

        public void DoShowActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");
            br.ClickAction("Product-BestSpecialOffer").GetField("Product-BestSpecialOffer-Quantity").TypeText("12", br);
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
            br.ClickAction("Store-QuickOrder");
            br.AssertPageTitleEquals("AW00000546");
            br.ClickAction("QuickOrderForm-AddDetail");
            br.GetField("QuickOrderForm-AddDetail-Number").TypeText("33", br);
            br.ClickAction("QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct");
            br.ClickOk();
            Assert.IsTrue(br.Title.StartsWith("33 x "));
        }

        public abstract void InvokeActionOnViewModelReturnCollection();

        public void DoInvokeActionOnViewModelReturnCollection() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            br.AssertContainsObjectView();
            br.ClickAction("Store-QuickOrder");
            br.AssertPageTitleEquals("AW00000546");
            br.ClickAction("QuickOrderForm-AddDetail");
            br.GetField("QuickOrderForm-AddDetail-Number").TypeText("33", br);
            br.ClickAction("QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct");
            br.ClickOk();
            br.ClickAction("QuickOrderForm-AddDetail");
            br.GetField("QuickOrderForm-AddDetail-Number").TypeText("33", br);
            br.ClickAction("QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct");
            br.ClickOk();
            br.ClickAction("QuickOrderForm-AddDetail");
            br.GetField("QuickOrderForm-AddDetail-Number").TypeText("33", br);
            br.ClickAction("QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct");
            br.ClickOk();
            Assert.IsTrue(br.Title.StartsWith("33 x "));

            br.ClickAction("QuickOrderForm-GetOrders");

            br.AssertPageTitleEquals("3 Order Lines");
        }

        public abstract void InvokeActionParmsMandatory();

        public void DoInvokeActionParmsMandatory() {
            Login();

            FindProduct("BK-M68S-42");
            br.ClickAction("Product-BestSpecialOffer");
            br.ClickOk();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Mandatory");
        }

        public abstract void InvokeActionParmsInvalid();

        public void DoInvokeActionParmsInvalid() {
            Login();

            FindProduct("BK-M68S-42");
            br.ClickAction("Product-BestSpecialOffer").GetField("Product-BestSpecialOffer-Quantity").TypeText("not a number", br);
            br.FindElement(By.CssSelector("#body")).BrowserSpecificClick(br); // workaround for not picking up ok click 
            br.ClickOk();

            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Invalid Entry");
        }

        public abstract void InvokeContributedActionNoParmsReturnTransient();

        public void DoInvokeContributedActionNoParmsReturnTransient() {
            Login();
            FindProduct("BK-M68S-42");
            br.ClickAction("Product-CreateNewWorkOrder");
            br.AssertContainsObjectEditTransient();
            Assert.AreEqual("AdventureWorksModel-WorkOrder", br.FindElement(By.ClassName("nof-objectedit")).GetAttribute("id"));
        }

        public abstract void InvokeContributedActionNoParmsReturnPersistent();

        public void DoInvokeContributedActionNoParmsReturnPersistent() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-LastOrder");
            br.GetField("SalesOrderHeader-SalesOrderNumber").AssertValueEquals("SO69561");
        }

        public abstract void InvokeContributedActionParmsReturn();

        public void DoInvokeContributedActionParmsReturn() {
            bool useEsc = GetType() == typeof (ObjectViewAndActionTestsChrome);
            string esc = useEsc ? Keys.Escape : "";

            Login();

            FindCustomerByAccountNumber("AW00000546");

            br.ClickAction("Store-SearchForOrders");

            br.GetField("OrderContributedActions-SearchForOrders-Customer").AssertObjectHasTitle("Field Trip Store, AW00000546");

            br.GetField("OrderContributedActions-SearchForOrders-FromDate").TypeText("01/01/2003", br);
            //  br.GetField("OrderContributedActions-SearchForOrders-FromDate").AppendText(Keys.Escape, br);

            br.GetField("OrderContributedActions-SearchForOrders-ToDate").TypeText("12/12/2003" + Keys.Escape, br);
            //   br.GetField("OrderContributedActions-SearchForOrders-ToDate").AppendText(Keys.Escape, br);
            br.WaitForAjaxComplete();

            br.ClickOk();

            br.AssertPageTitleEquals("4 Sales Orders");
        }

        public abstract void CancelActionDialog();

        public void DoCancelActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000546");

            br.ClickAction("Store-SearchForOrders");

            br.AssertPageTitleEquals("Field Trip Store, AW00000546");

            //br.ClickCancel();
            br.FindElement(By.CssSelector(".ui-dialog-titlebar-close")).Click();

            br.AssertContainsObjectView();
            br.AssertPageTitleEquals("Field Trip Store, AW00000546");
        }

        public abstract void EmptyCollectionDoesNotShowListOrTableButtons();

        public void DoEmptyCollectionDoesNotShowListOrTableButtons() {
            Login();
            FindProduct("LW-1000");
            IWebElement reviews = br.GetField("Product-ProductReviews");
            reviews.AssertSummaryEquals("No Product Reviews");
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=List]"));
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=Table]"));
        }

        public abstract void RemoveFromActionDialog();

        public void DoRemoveFromActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            br.ClickAction("Store-CreateNewOrder");
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Field Trip Store, AW00000546");

            br.FindElement(By.CssSelector("button[title=Remove]")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Mandatory");
        }

        public abstract void RecentlyViewedOnActionDialog();

        public void DoRecentlyViewedOnActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000547");
            br.ClickAction("Store-CreateNewOrder");
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");
            br.FindElement(By.CssSelector("button[title='Recently Viewed']")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");
        }

        public abstract void RecentlyViewedOnActionDialogWithSelect();

        public void DoRecentlyViewedOnActionDialogWithSelect() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            FindCustomerByAccountNumber("AW00000547");
            br.ClickAction("Store-CreateNewOrder");
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");
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
            br.ClickAction("Store-CreateNewOrder");
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Curbside Sporting Goods, AW00000547");
            br.FindElement(By.Id("Store-CreateNewOrder-Customer-CustomerRepository-FindCustomerByAccountNumber")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.GetField("CustomerRepository-FindCustomerByAccountNumber-AccountNumber").TypeText("AW00000546", br);
            br.ClickOk();
            br.FindElement(By.Id("OrderContributedActions-CreateNewOrder-Customer")).AssertObjectHasTitle("Field Trip Store, AW00000546");
        }

        public abstract void NewObjectOnActionDialog();

        public void DoNewObjectOnActionDialog() {
            Login();

            br.ClickAction("WorkOrderRepository-CreateNewWorkOrder");
            br.FindElement(By.Id("WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.GetField("Product-Name").TypeText("test-popup", br);
            br.GetField("Product-ProductNumber").TypeText("test-popup", br);
            br.GetField("Product-ListPrice").TypeText("10", br);

            br.GetField("Product-SafetyStockLevel").TypeText("1", br);
            br.GetField("Product-ReorderPoint").TypeText("1", br);
            br.GetField("Product-DaysToManufacture").TypeText("1", br);
            br.GetField("Product-SellStartDate").TypeText("1/1/2020" + Keys.Escape, br);
            br.GetField("Product-StandardCost").TypeText("1", br);
            br.ClickSave();
            br.WaitForAjaxComplete();

            br.FindElements(By.CssSelector("button[title='Select']")).First().BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElement(By.Id("WorkOrderRepository-CreateNewWorkOrder-Product")).AssertInputValueEquals("test-popup ");
        }

        public abstract void AutoCompleteOnActionDialog();

        public void DoAutoCompleteOnActionDialog() {
            Login();

            br.ClickAction("WorkOrderRepository-CreateNewWorkOrder");

            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete").Clear();
            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete").SendKeys("HL");
            br.WaitForAjaxComplete();
            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete").SendKeys(Keys.ArrowDown);
            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete").SendKeys(Keys.ArrowDown);
            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete").SendKeys(Keys.Tab);
            br.GetField("WorkOrderRepository-CreateNewWorkOrder-Product").AssertInputValueEquals("HL Hub");
        }

        public abstract void NewObjectOnActionDialogFailMandatory();

        public void DoNewObjectOnActionDialogFailMandatory() {
            Login();

            br.ClickAction("WorkOrderRepository-CreateNewWorkOrder");
            br.FindElement(By.Id("WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.GetField("Product-Name").TypeText("test", br);
            br.GetField("Product-ProductNumber").TypeText("test", br);
            br.GetField("Product-ListPrice").TypeText("10", br);

            br.GetField("Product-SafetyStockLevel").TypeText("1", br);
            br.GetField("Product-ReorderPoint").TypeText("1", br);
            br.GetField("Product-DaysToManufacture").TypeText("1", br);
            //br.GetField("Product-SellStartDate").TypeText(DateTime.Today.AddDays(1).ToShortDateString(), br); - missing mandatory
            br.GetField("Product-StandardCost").TypeText("1", br);

            br.ClickSave();
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Mandatory");
            Assert.AreEqual("test", br.GetField("Product-Name-Input").GetAttribute("value"));
        }

        public abstract void NewObjectOnActionDialogFailInvalid();

        public void DoNewObjectOnActionDialogFailInvalid() {
            Login();

            br.ClickAction("WorkOrderRepository-CreateNewWorkOrder");
            br.FindElement(By.Id("WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.GetField("Product-Name").TypeText("test", br);
            br.GetField("Product-ProductNumber").TypeText("test", br);
            br.GetField("Product-ListPrice").TypeText("10", br);

            br.GetField("Product-SafetyStockLevel").TypeText("1", br);
            br.GetField("Product-ReorderPoint").TypeText("1", br);
            br.GetField("Product-DaysToManufacture").TypeText("1", br);
            br.GetField("Product-SellStartDate").TypeText("1" + Keys.Escape, br); // invalid
            br.GetField("Product-StandardCost").TypeText("1", br);
            br.ClickSave();
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Invalid Entry");
            Assert.AreEqual("test", br.GetField("Product-Name-Input").GetAttribute("value"));
        }
    }
}