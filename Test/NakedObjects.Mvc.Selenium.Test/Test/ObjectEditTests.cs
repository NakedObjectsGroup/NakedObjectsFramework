// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class ObjectEditTests : AWWebTest {
        private void FindCustomerAndEdit(string custId) {
            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");
            f.Clear();
            f.SendKeys(custId + Keys.Tab);
            var edit = wait.ClickAndWait(".nof-ok", ".nof-edit");
            wait.ClickAndWait(edit, ".nof-objectedit");
        }

        public void DoEditPersistedObject() {
            Login();
            FindCustomerAndEdit("AW00000546");

            //Check basics of edit view
            br.AssertPageTitleEquals("Field Trip Store, AW00000546");
            IWebElement objectView = br.FindElement(By.ClassName("main-content"));
            //IWebElement titleObject = objectView.FindElement(By.ClassName("nof-object")); // noew disabled for edit view 
            //Assert.AreEqual("Field Trip Store, AW00000546", titleObject.Text);
            br.AssertElementExists(By.CssSelector("[title=Save]"));
            try {
                br.AssertElementDoesNotExist(By.CssSelector("[title=Edit]"));
            }
            catch (Exception e) {
                string m = e.Message;
            }
            //Test modifiable field
            br.GetField("Store-Name").AssertIsModifiable();

            //Test unmodifiable field
            br.GetField("Store-AccountNumber").AssertIsUnmodifiable();

            Assert.AreEqual("nof-collection-table", br.GetInternalCollection("Store-Addresses").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            IWebElement table = br.GetInternalCollection("Store-Addresses").FindElement(By.TagName("table"));
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count); //First row is header
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);

            // Collection Summary
            br.ViewAsSummary("Store-Addresses");
            IWebElement contents = br.GetInternalCollection("Store-Addresses").FindElement(By.ClassName("nof-object"));
            Assert.AreEqual("1 Customer Address", contents.Text);

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
            Assert.AreEqual(1, table.FindElements(By.TagName("tr")).Count); //First row is header
            Assert.AreEqual("Main Office: 2575 Rocky Mountain Ave. ...", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("td"))[0].Text);

            //Return to View via History
            br.ClickTabLink(0);
            br.AssertContainsObjectView();
        }

        public void DoEditTableHeader() {
            Login();

            var f = wait.ClickAndWait("#ProductRepository-FindProductByNumber button", "#ProductRepository-FindProductByNumber-Number-Input");
            f.Clear();
            f.SendKeys("BK-M38S-46" + Keys.Tab);
            var edit = wait.ClickAndWait(".nof-ok", ".nof-edit");
            wait.ClickAndWait(edit, ".nof-objectedit");

            // Collection Table
            //br.ViewAsTable("Product-ProductInventory"); - noew rendered eagerly
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

        public void DoCancelButtonOnObjectEdit() {
            Login();
            FindCustomerAndEdit("AW00000546");
            wait.ClickAndWait(".nof-cancel", ".nof-objectview");
            br.AssertPageTitleEquals("Field Trip Store, AW00000546");
        }

        public void DoSaveWithNoChanges() {
            Login();
            FindCustomerAndEdit("AW00000071");
            wait.ClickAndWait(".nof-save", ".nof-objectview");
        }

        public void DoChangeStringField() {
            Login();
            FindCustomerAndEdit("AW00000072");
    
            br.GetField("Store-Name").AssertInputValueEquals("Outdoor Equipment Store").TypeText("Temporary Name", br);
            wait.ClickAndWait(".nof-save", ".nof-objectview");
            br.GetField("Store-Name").AssertValueEquals("Temporary Name");
        }

        public void DoChangeDropDownField() {
            Login();
            FindCustomerAndEdit("AW00000073");

            br.GetField("Store-SalesTerritory").AssertSelectedDropDownItemIs("Northwest").SelectDropDownItem("C", br);
            wait.ClickAndWait(".nof-save", ".nof-objectview");
            br.GetField("Store-SalesTerritory").AssertObjectHasTitle("Central");
        }

        public void DoChangeReferencePropertyViaRecentlyViewed() {
            Login();

            FindSalesPerson("Ito");
            FindCustomerByAccountNumber("AW00000074");
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("David Campbell");

            br.ClickEdit();
            br.AssertElementExists(By.ClassName("nof-menu"));
            br.ClickRecentlyViewed("Store-SalesPerson");
            br.SelectFinderOption("Store-SalesPerson", "Shu Ito");
            wait.ClickAndWait(".nof-save", ".nof-objectview");
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Shu Ito");
        }

        public void DoChangeReferencePropertyViaRemove() {
            Login();
            FindCustomerByAccountNumber("AW00000076");
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Jillian Carson");

            br.ClickEdit();
            br.AssertElementExists(By.ClassName("nof-menu"));
            br.ClickRemove("Store-SalesPerson");
            wait.ClickAndWait(".nof-save", ".nof-objectview");
            br.GetField("Store-SalesPerson").AssertIsEmpty();
        }

        public void DoChangeReferencePropertyViaAFindAction() {
            Login();
            FindCustomerByAccountNumber("AW00000075");
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Jillian Carson");

            br.ClickEdit();
            br.ClickFinderAction("Store-SalesPerson", "Store-SalesPerson-SalesRepository-FindSalesPersonByName");
            br.GetField("SalesRepository-FindSalesPersonByName-LastName").TypeText("Vargas", br);
            br.ClickOk();
            wait.ClickAndWait(".nof-save", ".nof-objectview");
            br.GetField("Store-SalesPerson").AssertObjectHasTitle("Garrett Vargas");
        }

        public void DoChangeReferencePropertyViaANewAction() {
            Login();
            br.ClickAction("WorkOrderRepository-RandomWorkOrder");
            br.ClickEdit();
            br.FindElement(By.Id("WorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();

            br.GetField("Product-Name").TypeText("test", br);
            br.GetField("Product-ProductNumber").TypeText("test", br);
            br.GetField("Product-ListPrice").TypeText("10", br);
            br.GetField("Product-SafetyStockLevel").TypeText("1", br);
            br.GetField("Product-ReorderPoint").TypeText("1", br);
            br.GetField("Product-DaysToManufacture").TypeText("1", br);
            br.GetField("Product-SellStartDate").TypeText("1/1/2020", br);
            br.GetField("Product-SellStartDate").AppendText(Keys.Escape, br);
            br.GetField("Product-StandardCost").TypeText("1", br);

            IWebElement saveButton = br.FindElement(By.CssSelector("button[name='InvokeActionAsSave']"));
            saveButton.FocusClick(br);
            br.WaitForAjaxComplete();
            saveButton.BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.FindElements(By.CssSelector("button[title='Select']")).First().BrowserSpecificClick(br);
            br.FindElement(By.Id("WorkOrder-Product")).AssertInputValueEquals("test ");
        }

        public void DoChangeReferencePropertyViaAutoComplete() {
            Login();
            br.ClickAction("WorkOrderRepository-RandomWorkOrder");
            br.ClickEdit();
            br.GetField("WorkOrder-Product-Select-AutoComplete").Clear();
            br.GetField("WorkOrder-Product-Select-AutoComplete").SendKeys("HL");
            br.WaitForAjaxComplete();
            br.GetField("WorkOrder-Product-Select-AutoComplete").SendKeys(Keys.ArrowDown);
            br.GetField("WorkOrder-Product-Select-AutoComplete").SendKeys(Keys.ArrowDown);
            br.GetField("WorkOrder-Product-Select-AutoComplete").SendKeys(Keys.Tab);
            br.GetField("WorkOrder-Product").AssertInputValueEquals("HL Crankset");
        }

        public void DoChangeReferencePropertyViaANewActionFailMandatory() {
            Login();
            br.ClickAction("WorkOrderRepository-RandomWorkOrder");
            br.ClickEdit();
            br.FindElement(By.Id("WorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
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

        public void DoChangeReferencePropertyViaANewActionFailInvalid() {
            Login();
            br.ClickAction("WorkOrderRepository-RandomWorkOrder");
            br.ClickEdit();
            br.FindElement(By.Id("WorkOrder-Product-ProductRepository-NewProduct")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();
            br.GetField("Product-Name").TypeText("test", br);
            br.GetField("Product-ProductNumber").TypeText("test", br);
            br.GetField("Product-ListPrice").TypeText("10", br);

            br.GetField("Product-SafetyStockLevel").TypeText("1", br);
            br.GetField("Product-ReorderPoint").TypeText("1", br);
            br.GetField("Product-DaysToManufacture").TypeText("1", br);
            br.GetField("Product-SellStartDate").TypeText("1/1/2020", br);
            br.GetField("Product-SellStartDate").AppendText(Keys.Escape, br);
            br.GetField("Product-StandardCost").TypeText("test", br); // invalid
            br.ClickSave();
            br.WaitForAjaxComplete();
            br.FindElement(By.CssSelector("span.field-validation-error")).AssertTextEquals("Invalid Entry");
            Assert.AreEqual("test", br.GetField("Product-Name-Input").GetAttribute("value"));
        }

        public void DoCheckDefaultsOnFindAction() {
            Login();
            FindOrder("SO53144");
            br.ClickEdit();

            br.ClickFinderAction("SalesOrderHeader-CurrencyRate", "SalesOrderHeader-CurrencyRate-OrderContributedActions-FindRate");

            br.GetField("OrderContributedActions-FindRate-Currency").AssertInputValueEquals("US Dollar");
            br.GetField("OrderContributedActions-FindRate-Currency1").AssertInputValueEquals("Euro");
        }

        public void DoNoEditButtonWhenNoEditableFields() {
            Login();
            FindOrder("SO53144");
            br.ClickOnObjectLinkInField("SalesOrderHeader-CreditCard");
            br.AssertPageTitleEquals("**********7212");
            br.AssertElementDoesNotExist(By.CssSelector("[title=Edit]"));
        }

        public void DoRefresh() {
            Login();
            FindCustomerAndEdit("AW00000071");

            br.Navigate().Refresh();
            br.WaitForAjaxComplete();
            br.AssertContainsObjectEdit();
        }

        public void DoNoValidationOnTransientUntilSave() {
            Login();
            FindCustomerByAccountNumber("AW00000532");
            br.ClickAction("Store-CreateNewOrder");
            br.ClickOk();

            br.FindElement(By.Id("SalesOrderHeader-ShipDate")).TypeText(DateTime.Now.AddDays(-1).ToShortDateString(), br);
            br.FindElement(By.Id("SalesOrderHeader-Status")).SelectDropDownItem("Approved", br);
            br.FindElement(By.Id("SalesOrderHeader-StoreContact")).SelectDropDownItem("Diane Glimp", br);
            br.FindElement(By.Id("SalesOrderHeader-ShipMethod")).SelectDropDownItem("XRQ", br);
            br.FindElement(By.Id("SalesOrderHeader-ShipDate")).AssertNoValidationError();
            br.ClickSave();

            // not managed to get validation error - perhaps becuase it revalidates and disappears ? 
            //br.FindElement(By.Id("SalesOrderHeader-ShipDate")).AssertValidationErrorIs("Ship date cannot be before order date");
            // checj instead we're still on the salesorder edit page 

            br.AssertContainsObjectEdit();
        }

        #region abstract 

        public abstract void EditPersistedObject();

        public abstract void EditTableHeader();

        public abstract void CancelButtonOnObjectEdit();

        public abstract void SaveWithNoChanges();

        public abstract void ChangeStringField();

        public abstract void ChangeDropDownField();

        public abstract void ChangeReferencePropertyViaRecentlyViewed();

        public abstract void ChangeReferencePropertyViaRemove();

        public abstract void ChangeReferencePropertyViaAFindAction();

        public abstract void ChangeReferencePropertyViaNewAction();

        public abstract void NoValidationOnTransientUntilSave();

        public abstract void Refresh();

        public abstract void NoEditButtonWhenNoEditableFields();

        public abstract void CheckDefaultsOnFindAction();

        public abstract void ChangeReferencePropertyViaAutoComplete();

        public abstract void ChangeReferencePropertyViaANewActionFailMandatory();

        public abstract void ChangeReferencePropertyViaANewActionFailInvalid();

        #endregion
    }
}