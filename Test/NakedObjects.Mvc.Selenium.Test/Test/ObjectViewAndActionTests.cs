// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
            br.FindElement(By.ClassName("nof-objectview"));

            Assert.AreEqual("Field Trip Store, AW00000546", br.Title);

            Assert.AreEqual("Field Trip Store", br.FindElement(By.CssSelector("#Store-Name")).FindElement(By.ClassName("nof-value")).Text);
            br.FindElement(By.CssSelector("[title=Edit]"));
            br.AssertElementDoesNotExist(By.CssSelector("[title=Save]"));
            Assert.AreEqual("nof-menu", br.FindElement(By.CssSelector("#Store-Actions")).GetAttribute("class"));

            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.Title == "Linda Mitchell");

            br.FindElement(By.ClassName("nof-objectview"));
            Assert.AreEqual("1.50 %", br.FindElement(By.CssSelector("#SalesPerson-CommissionPct")).FindElement(By.ClassName("nof-value")).Text);
            br.FindElement(By.CssSelector("[title=Edit]"));
            Assert.AreEqual("nof-menu", br.FindElement(By.CssSelector("#SalesPerson-Actions")).GetAttribute("class"));

            // click history first tab
            wait.ClickAndWait(".nof-tab:first-of-type a", wd => wd.Title == "Field Trip Store, AW00000546");

            // rendered eagerly 
            Assert.AreEqual("nof-collection-table", br.FindElements(By.CssSelector("#Store-Addresses div"))[1].GetAttribute("class"));
            var table = br.FindElement(By.CssSelector("#Store-Addresses")).FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);
            // Collection Summary
            wait.ClickAndWait("#Store-Addresses .nof-summary", "#Store-Addresses .nof-collection-summary");

            IWebElement tempQualifier = br.FindElement(By.CssSelector("#Store-Addresses"));
            Assert.IsTrue(tempQualifier.FindElements(By.TagName("div"))[1].GetAttribute("class") == "nof-collection-summary", "Collection is not in Summary view");
            Assert.AreEqual("1 Customer Address", tempQualifier.FindElement(By.CssSelector("div.nof-object")).Text);

            // Collection List
            wait.ClickAndWait("#Store-Addresses .nof-list", "#Store-Addresses .nof-collection-list");

            Assert.AreEqual("nof-collection-list", br.FindElements(By.CssSelector("#Store-Addresses div"))[1].GetAttribute("class"));
            table = br.FindElement(By.CssSelector("#Store-Addresses")).FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElement(By.ClassName("nof-object")).Text);

            // Collection Table
            wait.ClickAndWait("#Store-Addresses .nof-table", "#Store-Addresses .nof-collection-table");

            Assert.AreEqual("nof-collection-table", br.FindElements(By.CssSelector("#Store-Addresses div"))[1].GetAttribute("class"));
            table = br.FindElement(By.CssSelector("#Store-Addresses")).FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);
        }

        public abstract void ViewTableHeader();

        public void DoViewTableHeader() {
            Login();
            FindProduct("BK-M38S-46");
            br.FindElement(By.ClassName("nof-objectview"));

            // Collection Table

            Assert.AreEqual("nof-collection-table", br.FindElements(By.CssSelector("#Product-ProductInventory div"))[1].GetAttribute("class"));
            var table = br.FindElement(By.CssSelector("#Product-ProductInventory")).FindElement(By.TagName("table"));
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
            br.FindElement(By.ClassName("nof-objectview"));
            Assert.AreEqual("SO59000", br.Title);
            //No verification that method has actually run - only that it hasn't changed the view!
        }

        public abstract void InvokeActionParmsReturn();

        public void DoInvokeActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.TypeText("12" + Keys.Tab);

            wait.ClickAndWait(".nof-ok", wd => wd.Title == "Volume Discount 11 to 14");
        }

        public abstract void ShowActionParmsReturn();

        public void DoShowActionParmsReturn() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.TypeText("12" + Keys.Tab);

            wait.ClickAndWait(".nof-apply", wd => wd.Title == "Volume Discount 11 to 14");
            wait.ClickAndWaitGone(".ui-dialog-titlebar-close", ".nof-apply");
        }

        public abstract void InvokeActionOnViewModel();

        public void DoInvokeActionOnViewModel() {
            Login();

            FindCustomerByAccountNumber("AW00000546");
            br.FindElement(By.ClassName("nof-objectview"));
            wait.ClickAndWait("#Store-QuickOrder button", wd => wd.Title == "AW00000546");

            var number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.TypeText("33" + Keys.Tab);

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
            wait.ClickAndWait(".nof-ok", wd => wd.FindElement(By.CssSelector("#QuickOrderForm-PropertyList .nof-object")).Text == "1 Order Line");

            number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.TypeText("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
            wait.ClickAndWait(".nof-ok", wd => wd.FindElement(By.CssSelector("#QuickOrderForm-PropertyList .nof-object")).Text == "2 Order Lines");

            number = wait.ClickAndWait("#QuickOrderForm-AddDetail button", "#QuickOrderForm-AddDetail-Number-Input");
            number.TypeText("33" + Keys.Tab);

            wait.ClickAndWait("#QuickOrderForm-AddDetail-Product-ProductRepository-RandomProduct", "#QuickOrderForm-AddDetail-Product img");
            wait.ClickAndWait(".nof-ok", wd => wd.FindElement(By.CssSelector("#QuickOrderForm-PropertyList .nof-object")).Text == "3 Order Lines");

            wait.ClickAndWait("#QuickOrderForm-GetOrders button", wd => wd.Title == "3 Order Lines");
        }

        public abstract void InvokeActionParmsMandatory();

        public void DoInvokeActionParmsMandatory() {
            Login();

            FindProduct("BK-M68S-42");

            var ok = wait.ClickAndWait("#Product-BestSpecialOffer button", ".nof-ok");
            var error = wait.ClickAndWait(ok, "span.field-validation-error");
            Assert.AreEqual("Mandatory", error.Text);
        }

        public abstract void InvokeActionParmsInvalid();

        public void DoInvokeActionParmsInvalid() {
            Login();

            FindProduct("BK-M68S-42");

            var quantity = wait.ClickAndWait("#Product-BestSpecialOffer button", "#Product-BestSpecialOffer-Quantity-Input");
            quantity.TypeText("not a number" + Keys.Tab);

            var error = wait.ClickAndWait(".nof-ok", "span.field-validation-error");
            Assert.AreEqual("Invalid Entry", error.Text);
        }

        public abstract void InvokeContributedActionNoParmsReturnTransient();

        public void DoInvokeContributedActionNoParmsReturnTransient() {
            Login();
            FindProduct("BK-M68S-42");

            var objedit = wait.ClickAndWait("#Product-CreateNewWorkOrder button", ".nof-objectedit");
            IWebElement elem = br.FindElement(By.CssSelector(".nof-objectedit"));
            var cls = elem.GetAttribute("class");
            Assert.IsTrue(cls.Contains("nof-objectedit") && cls.Contains("nof-transient") && cls.Replace("nof-transient", "").Replace("nof-objectedit", "").Trim().Length == 0);
            Assert.AreEqual("AdventureWorksModel-WorkOrder", objedit.GetAttribute("id"));
        }

        public abstract void InvokeContributedActionNoParmsReturnPersistent();

        public void DoInvokeContributedActionNoParmsReturnPersistent() {
            Login();
            FindCustomerByAccountNumber("AW00000546");
            var number = wait.ClickAndWait("#Store-LastOrder button", "#SalesOrderHeader-SalesOrderNumber");
            Assert.AreEqual("SO69561", number.FindElement(By.ClassName("nof-value")).Text);
        }

        public abstract void InvokeContributedActionParmsReturn();

        public void DoInvokeContributedActionParmsReturn() {
            var useEsc = GetType() == typeof (ObjectViewAndActionTestsChrome);
            var esc = useEsc ? Keys.Escape : "";

            Login();

            FindCustomerByAccountNumber("AW00000546");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");

            Assert.AreEqual("Field Trip Store, AW00000546", obj.FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text);

            br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-FromDate-Input")).TypeText("01/01/2003");
            br.FindElement(By.CssSelector("#OrderContributedActions-SearchForOrders-ToDate-Input")).TypeText("12/12/2003" + Keys.Escape);

            wait.ClickAndWait(".nof-ok", wd => wd.Title == "4 Sales Orders");
        }

        public abstract void CancelActionDialog();

        public void DoCancelActionDialog() {
            Login();

            FindCustomerByAccountNumber("AW00000546");

            var obj = wait.ClickAndWait("#Store-SearchForOrders button", "#OrderContributedActions-SearchForOrders-Customer");

            Assert.AreEqual("Field Trip Store, AW00000546", obj.FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text);

            //br.ClickCancel();
            br.FindElement(By.CssSelector(".ui-dialog-titlebar-close")).Click();

            br.FindElement(By.ClassName("nof-objectview"));
            Assert.AreEqual("Field Trip Store, AW00000546", br.Title);
        }

        public abstract void EmptyCollectionDoesNotShowListOrTableButtons();

        public void DoEmptyCollectionDoesNotShowListOrTableButtons() {
            Login();
            FindProduct("LW-1000");
            var reviews = br.FindElement(By.CssSelector("#Product-ProductReviews"));
            Assert.IsTrue(reviews.FindElements(By.TagName("div"))[1].GetAttribute("class") == "nof-collection-summary", "Collection is not in Summary view");
            Assert.AreEqual("No Product Reviews", reviews.FindElement(By.CssSelector("div.nof-object")).Text);
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=List]"));
            br.AssertElementDoesNotExist(By.CssSelector("div#Product-ProductReviews[title=Table]"));
        }

        public abstract void RemoveFromActionDialog();

        public void DoRemoveFromActionDialog() {
            Login();
            var randomEmployee = wait.ClickAndWait("#SalesRepository-CreateNewSalesPerson button", "#SalesRepository-CreateNewSalesPerson-Employee-EmployeeRepository-RandomEmployee");
            wait.ClickAndWait(randomEmployee, wd => wd.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee .nof-object a")).Text.Trim().Length > 0);
            var error = wait.ClickAndWait("button[title=Remove]", "span.field-validation-error");
            Assert.AreEqual("Mandatory", error.Text);
        }

        public abstract void RecentlyViewedOnActionDialog();

        public void DoRecentlyViewedOnActionDialog() {
            Login();

            FindEmployeeByLastName("Sánchez");

            var recentlyViewed = wait.ClickAndWait("#SalesRepository-CreateNewSalesPerson button", "button[title='Recently Viewed']");
            wait.ClickAndWait(recentlyViewed, wd => wd.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee .nof-object a")).Text.Trim().Length > 0);

            Assert.AreEqual("Ken Sánchez", br.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee")).FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text);
        }

        public abstract void RecentlyViewedOnActionDialogWithSelect();

        public void DoRecentlyViewedOnActionDialogWithSelect() {
            Login();

            FindEmployeeByLastName("Gubbels");
            FindEmployeeByLastName("Krebs");

            wait.Until(wd => wd.Title == "Peter Krebs");

            var recentlyViewed = wait.ClickAndWait("#SalesRepository-CreateNewSalesPerson button", "button[title='Recently Viewed']");
            var select = wait.ClickAndWait(recentlyViewed, "button[title='Select']:last-of-type");
            wait.ClickAndWait(select, wd => wd.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee .nof-object a")).Text.Trim().Length > 0);

            Assert.AreEqual("Eric Gubbels", br.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee")).FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text);
        }

        public abstract void ActionFindOnActionDialog();

        public void DoActionFindOnActionDialog() {
            Login();

            var findEmployee = wait.ClickAndWait("#SalesRepository-CreateNewSalesPerson button", "#SalesRepository-CreateNewSalesPerson-Employee-EmployeeRepository-FindEmployeeByName");
            var lastName = wait.ClickAndWait(findEmployee, "#EmployeeRepository-FindEmployeeByName-LastName-Input");

            lastName.TypeText("Krebs" + Keys.Tab);

            wait.ClickAndWait(".nof-ok", wd => wd.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee .nof-object a")).Text.Trim().Length > 0);

            Assert.AreEqual("Peter Krebs", br.FindElement(By.CssSelector("#SalesRepository-CreateNewSalesPerson-Employee")).FindElement(By.ClassName("nof-object")).FindElement(By.TagName("a")).Text);
        }

        public abstract void NewObjectOnActionDialog();

        public void DoNewObjectOnActionDialog() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");

            br.FindElement(By.CssSelector("#Product-Name-Input")).TypeText("test-popup");
            br.FindElement(By.CssSelector("#Product-ProductNumber-Input")).TypeText("test-popup");
            br.FindElement(By.CssSelector("#Product-ListPrice-Input")).TypeText("10");

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-ReorderPoint-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-DaysToManufacture-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-SellStartDate-Input")).TypeText("1/1/2020" + Keys.Escape);
            br.FindElement(By.CssSelector("#Product-StandardCost-Input")).TypeText("1");

            var select = wait.ClickAndWait(".nof-save", "button[title='Select']:first-of-type");
            var product = wait.ClickAndWait(select, "#WorkOrderRepository-CreateNewWorkOrder-Product");

            Assert.AreEqual("test-popup ", product.FindElement(By.TagName("input")).GetAttribute("value"));
        }

        public abstract void AutoCompleteOnActionDialog();

        public void DoAutoCompleteOnActionDialog() {
            Login();

            wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");

            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).TypeText("HL");

            wait.Until(wd => wd.FindElements(By.CssSelector(".ui-menu-item")).Count > 0);

            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.ArrowDown);
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.ArrowDown);
            br.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product-Select-AutoComplete")).SendKeys(Keys.Tab);

            wait.Until(wd => wd.FindElement(By.CssSelector("#WorkOrderRepository-CreateNewWorkOrder-Product input")).GetAttribute("value") == "HL Hub");
        }

        public abstract void NewObjectOnActionDialogFailMandatory();

        public void DoNewObjectOnActionDialogFailMandatory() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");

            br.FindElement(By.CssSelector("#Product-Name-Input")).TypeText("test");
            br.FindElement(By.CssSelector("#Product-ProductNumber-Input")).TypeText("test");
            br.FindElement(By.CssSelector("#Product-ListPrice-Input")).TypeText("10");

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-ReorderPoint-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-DaysToManufacture-Input")).TypeText("1");
            //br.FindElement(By.CssSelector("#Product-SellStartDate-Input").TypeText(DateTime.Today.AddDays(1).ToShortDateString(), br); - missing mandatory
            br.FindElement(By.CssSelector("#Product-StandardCost-Input")).TypeText("1");

            var error = wait.ClickAndWait(".nof-save", "span.field-validation-error");
            Assert.AreEqual("Mandatory", error.Text);

            Assert.AreEqual("test", br.FindElement(By.CssSelector("#Product-Name-Input")).GetAttribute("value"));
        }

        public abstract void NewObjectOnActionDialogFailInvalid();

        public void DoNewObjectOnActionDialogFailInvalid() {
            Login();

            var newProduct = wait.ClickAndWait("#WorkOrderRepository-CreateNewWorkOrder button", "#WorkOrderRepository-CreateNewWorkOrder-Product-ProductRepository-NewProduct");
            wait.ClickAndWait(newProduct, "#Product-Name-Input");

            br.FindElement(By.CssSelector("#Product-Name-Input")).TypeText("test");
            br.FindElement(By.CssSelector("#Product-ProductNumber-Input")).TypeText("test");
            br.FindElement(By.CssSelector("#Product-ListPrice-Input")).TypeText("10");

            br.FindElement(By.CssSelector("#Product-SafetyStockLevel-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-ReorderPoint-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-DaysToManufacture-Input")).TypeText("1");
            br.FindElement(By.CssSelector("#Product-SellStartDate-Input")).TypeText("1" + Keys.Escape); // invalid
            br.FindElement(By.CssSelector("#Product-StandardCost-Input")).TypeText("1");

            var error = wait.ClickAndWait(".nof-save", "span.field-validation-error");
            Assert.AreEqual("Invalid Entry", error.Text);

            Assert.AreEqual("test", br.FindElement(By.CssSelector("#Product-Name-Input")).GetAttribute("value"));
        }

        public abstract void ExpandAndCollapseNestedObject();

        public void DoExpandAndCollapseNestedObject() {
            Login();

            FindCustomerByAccountNumber("AW00000547");

            var label = wait.ClickAndWait("button.nof-maximize", "#Store-SalesPerson-SalesPerson-Employee div.nof-label");

            Assert.AreEqual("Employee Details:", label.Text);

            wait.ClickAndWaitGone("button.nof-minimize", "#Store-SalesPerson-SalesPerson-Employee div.nof-label");
        }
    }
}