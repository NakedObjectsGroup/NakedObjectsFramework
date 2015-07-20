// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class StandaloneCollectionTests : AWWebTest {
        public abstract void ViewStandaloneCollection();
        public abstract void ViewStandaloneCollectionTable();

        public void DoViewStandaloneCollection() {
            Login();

            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.Title == "20 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.FindElement(By.CssSelector(".nof-object")).Text);
            var table = br.FindElement(By.ClassName("SalesOrderHeader"));
            Assert.AreEqual(21, table.FindElements(By.TagName("tr")).Count);
        }

        public void DoViewStandaloneCollectionTable() {
            Login();

            wait.ClickAndWait("#SpecialOfferRepository-CurrentSpecialOffers button", wd => wd.Title == "7 Special Offers");

            Assert.AreEqual("Current Special Offers: Query Result: Viewing 7 of 7 Special Offers", br.FindElement(By.CssSelector(".nof-object")).Text);

            wait.ClickAndWait(".nof-table", ".nof-collection-table");

            var table = br.FindElement(By.ClassName("SpecialOffer"));

            Assert.AreEqual(8, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th")).Count);
            Assert.AreEqual(4, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td")).Count);

            Assert.AreEqual("All:", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[0].Text);
            Assert.AreEqual("Description", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[1].Text);
            Assert.AreEqual("Category", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[2].Text);
            Assert.AreEqual("Discount Pct", table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th"))[3].Text);

            Assert.AreEqual("1:", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("No Discount", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[1].Text);
            Assert.AreEqual("No Discount", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[2].Text);
            Assert.AreEqual("0.00 %", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[3].Text);
        }

        public abstract void ViewStandaloneCollectionDefaultToTable();

        public void DoViewStandaloneCollectionDefaultToTable() {
            Login();

            var table = wait.ClickAndWait("#EmployeeRepository-ListAllDepartments button", ".Department");

            Assert.AreEqual("16 Departments", br.Title);
            Assert.AreEqual("List All Departments: Query Result: Viewing 16 of 16 Departments", br.FindElement(By.CssSelector(".nof-object")).Text);

            Assert.AreEqual(17, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual(3, table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th")).Count);
            Assert.AreEqual(3, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td")).Count);
        }

        public abstract void SelectDeselectAll();

        public void DoSelectDeselectAll() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.AreEqual(0, CountCheckedBoxes(coll));

            coll.CheckAll(br);
            Assert.IsTrue(coll.FindElements(By.CssSelector("input[type=checkbox]")).First().Selected);
            Assert.AreEqual(21, CountCheckedBoxes(coll)); //21 because this will include the 'all box'

            coll.UnCheckAll(br); //To deselect
            Assert.AreEqual(0, CountCheckedBoxes(coll));
        }

        private static int CountCheckedBoxes(IWebElement coll) {
            return coll.FindElements(By.CssSelector("input[type=checkbox]")).Count(x => x.GetAttribute("id").StartsWith("checkbox") && x.Selected);
        }

        public abstract void SelectAndUnselectIndividually();

        public void DoSelectAndUnselectIndividually() {
            Login();
            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            coll.GetRow(1).CheckRow(br);
            coll.GetRow(3).CheckRow(br);
            coll.GetRow(5).CheckRow(br);

            Assert.AreEqual(3, CountCheckedBoxes(coll));

            coll.GetRow(3).UnCheckRow(br);

            Assert.AreEqual(2, CountCheckedBoxes(coll));
        }

        public abstract void InvokeContributedActionNoParmsNoReturn();

        public void DoInvokeContributedActionNoParmsNoReturn() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(1, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(2, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(3, 6)));

            coll.GetRow(1).CheckRow(br);
            coll.GetRow(3).CheckRow(br);

            wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-CommentAsUsersUnhappy", wd => wd.FindElement(By.CssSelector(".nof-collection-table")).TextContentsOfCell(1, 6) == "User unhappy");

            Assert.IsTrue(string.IsNullOrEmpty(br.FindElement(By.CssSelector(".nof-collection-table")).TextContentsOfCell(2, 6)));
            Assert.AreEqual("User unhappy", br.FindElement(By.CssSelector(".nof-collection-table")).TextContentsOfCell(3, 6));

            Assert.IsTrue(br.FindElement(By.CssSelector(".nof-standalonetable .nof-object")).Text.StartsWith("Highest Value Orders: Query Result: Viewing 20 of "));
        }

        public abstract void InvokeContributedActionParmsNoReturn();

        public void DoInvokeContributedActionParmsNoReturn() {
            Login();
            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(7, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(8, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(9, 6)));

            coll.GetRow(7).CheckRow(br);
            coll.GetRow(9).CheckRow(br);

            var append = wait.Until(wd => wd.FindElement(By.CssSelector("#ObjectQuery-SalesOrderHeader-AppendComment")));
            var comment = wait.ClickAndWait(append, "#OrderContributedActions-AppendComment-CommentToAppend-Input");
            comment.TypeText("Foo" + Keys.Tab);

            wait.ClickAndWait(".nof-ok", wd => wd.FindElement(By.CssSelector(".nof-collection-table")).TextContentsOfCell(7, 6) == "Foo");

            coll = br.FindElement(By.CssSelector(".nof-collection-table"));

            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(8, 6)));
            Assert.AreEqual("Foo", coll.TextContentsOfCell(9, 6));
            Assert.IsTrue(br.FindElement(By.CssSelector(".nof-standalonetable .nof-object")).Text.StartsWith("Highest Value Orders: Query Result: Viewing 20 of "));
        }

        public abstract void InvokeContributedActionParmsValidateFail();

        public void DoInvokeContributedActionParmsValidateFail() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            coll.GetRow(7).CheckRow(br);
            coll.GetRow(9).CheckRow(br);

            var comment = wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-AppendComment", "#OrderContributedActions-AppendComment-CommentToAppend-Input");
            comment.TypeText("fail" + Keys.Tab);

            var error = wait.ClickAndWait(".nof-ok", ".validation-summary-errors li");

            Assert.AreEqual("For test purposes the comment 'fail' fails validation", error.Text);

            Assert.AreEqual("2 Sales Orders", br.FindElement(By.CssSelector("[name=Details]")).Text);

            wait.ClickAndWait("[name=Details]", wd => wd.Title == "2 Sales Orders");
        }

        public abstract void InvokeContributedActionNoSelections();

        public void DoInvokeContributedActionNoSelections() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            wait.ClickAndWait(table, ".nof-collection-table");
            wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-AppendComment", wd => wd.Title == "20 Sales Orders" && wd.FindElement(By.CssSelector(".Nof-Warnings")).Text == "No objects selected");
            Assert.IsTrue(br.FindElement(By.CssSelector(".nof-standalonetable .nof-object")).Text.StartsWith("Highest Value Orders: Query Result: Viewing 20 of "));

        }

        public abstract void PagingWithDefaultPageSize();

        public void DoPagingWithDefaultPageSize() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.AreEqual("20 Sales Orders", br.Title);
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.FindElement(By.CssSelector(".nof-object")).Text);

            var pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 1574", pageNo.Text);
            var total = br.FindElement(By.ClassName("nof-total-count"));
            Assert.AreEqual("Total of 31465 Sales Orders", total.Text);

            Assert.AreEqual("SO51131", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO51823", coll.TextContentsOfCell(20, 1));

            wait.ClickAndWait("button[title=Next]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 2 of 1574");

            coll = br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual("20 Sales Orders", br.Title);
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.FindElement(By.CssSelector(".nof-object")).Text);

            Assert.AreEqual("SO47441", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO47027", coll.TextContentsOfCell(20, 1));

            br.FindElement(By.CssSelector("button[title=Last]")).Click();

            wait.ClickAndWait("button[title=Last]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 1574 of 1574");

            coll = br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual("5 Sales Orders", br.Title);
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 5 of 31465 Sales Orders", br.FindElement(By.CssSelector(".nof-object")).Text);

            Assert.AreEqual("SO52682", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO51782", coll.TextContentsOfCell(5, 1));

            wait.ClickAndWait("button[title=Previous]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 1573 of 1574");

            coll = br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual("20 Sales Orders", br.Title);
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.FindElement(By.CssSelector(".nof-object")).Text);

            Assert.AreEqual("SO59092", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO52786", coll.TextContentsOfCell(20, 1));
        }

        public abstract void PagingWithOverriddenPageSize();

        public void DoPagingWithOverriddenPageSize() {
            Login();

            var name = wait.ClickAndWait("#CustomerRepository-FindStoreByName button", "#CustomerRepository-FindStoreByName-Name-Input");
            name.TypeText("a" + Keys.Tab);

            var coll = wait.ClickAndWait(".nof-ok", ".nof-collection-list");

            Assert.AreEqual("2 Stores", br.Title);
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.FindElement(By.CssSelector(".nof-object")).Text);
            var pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 249", pageNo.Text);
            var total = br.FindElement(By.ClassName("nof-total-count"));
            Assert.AreEqual("Total of 497 Stores", total.Text);

            wait.ClickAndWait("button[title=Next]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 2 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-list"));
            Assert.AreEqual("2 Stores", br.Title);
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.FindElement(By.CssSelector(".nof-object")).Text);

            wait.ClickAndWait("button[title=Last]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 249 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-list"));
            Assert.AreEqual("1 Store", br.Title);
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 1 of 497 Stores", br.FindElement(By.CssSelector(".nof-object")).Text);

            wait.ClickAndWait("button[title=Previous]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 248 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-list"));
            Assert.AreEqual("2 Stores", br.Title);
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.FindElement(By.CssSelector(".nof-object")).Text);
        }

        public abstract void PagingWithFormat();

        public void DoPagingWithFormat() {
            Login();

            var name = wait.ClickAndWait("#CustomerRepository-FindStoreByName button", "#CustomerRepository-FindStoreByName-Name-Input");
            name.TypeText("a" + Keys.Tab);

            var coll = wait.ClickAndWait(".nof-ok", ".nof-collection-list");
            Assert.AreEqual("2 Stores", br.Title);

            // list has 2 columns 
            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=Next]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 2 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-list"));
            Assert.AreEqual("2 Stores", br.Title);

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=Last]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 249 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-list"));
            Assert.AreEqual("1 Store", br.Title);

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=Table]", ".nof-collection-table");

            Assert.AreEqual("1 Store", br.Title);

            // table has 3 columns
            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=First]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 1 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual("2 Stores", br.Title);

            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=Next]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 2 of 249");

            coll = br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual("2 Stores", br.Title);

            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            wait.ClickAndWait("button[title=List]", ".nof-collection-list");

            Assert.AreEqual("2 Stores", br.Title);

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());
        }

        public abstract void InvokeActionNoResultAfterCollection();

        public void DoInvokeActionNoResultAfterCollection() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            coll.GetRow(1).CheckRow(br);
            coll.GetRow(3).CheckRow(br);

            // fail to find customer
           
            var field = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");
            field.TypeText("AW" + Keys.Tab);
            wait.ClickAndWait(".nof-ok", ".Nof-Warnings");

            Assert.AreEqual("No matching object found", br.FindElement(By.CssSelector(".Nof-Warnings")).Text);

            Assert.IsTrue(br.FindElement(By.CssSelector(".nof-standalonetable .nof-object")).Text.StartsWith("Highest Value Orders: Query Result: Viewing 20 of "));
        }
    }
}