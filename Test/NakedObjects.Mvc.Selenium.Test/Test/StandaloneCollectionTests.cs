// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Threading;
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
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.GetTopObject().Text);
            IWebElement table = br.FindElement(By.ClassName("SalesOrderHeader"));
            Assert.AreEqual(21, table.FindElements(By.TagName("tr")).Count);
        }

        public void DoViewStandaloneCollectionTable() {
            Login();

            wait.ClickAndWait("#SpecialOfferRepository-CurrentSpecialOffers button", wd => wd.Title == "7 Special Offers");

            Assert.AreEqual("Current Special Offers: Query Result: Viewing 7 of 7 Special Offers", br.GetTopObject().Text);

            wait.ClickAndWait(".nof-table", ".nof-collection-table"); 

            IWebElement table = br.FindElement(By.ClassName("SpecialOffer"));

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

            var table  = wait.ClickAndWait("#EmployeeRepository-ListAllDepartments button", ".Department");

            br.AssertPageTitleEquals("16 Departments");
            Assert.AreEqual("List All Departments: Query Result: Viewing 16 of 16 Departments", br.GetTopObject().Text);

           // IWebElement table = br.FindElement(By.ClassName("Department"));

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

            wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-CommentAsUsersUnhappy", wd => wd.Title == "20 Sales Orders");

            table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.AreEqual("User unhappy", coll.TextContentsOfCell(1, 6));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(2, 6)));
            Assert.AreEqual("User unhappy", coll.TextContentsOfCell(3, 6));
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

            var comment = wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-AppendComment", "#OrderContributedActions-AppendComment-CommentToAppend-Input");
            comment.Clear();
            comment.SendKeys("Foo" + Keys.Tab);

            wait.ClickAndWait(".nof-ok", wd => wd.Title == "20 Sales Orders");   

            table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            coll = wait.ClickAndWait(table, ".nof-collection-table");

            Assert.AreEqual("Foo", coll.TextContentsOfCell(7, 6));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(8, 6)));
            Assert.AreEqual("Foo", coll.TextContentsOfCell(9, 6));
        }

        public abstract void InvokeContributedActionParmsValidateFail();

        public void DoInvokeContributedActionParmsValidateFail() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            coll.GetRow(7).CheckRow(br);
            coll.GetRow(9).CheckRow(br);

            var comment = wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-AppendComment", "#OrderContributedActions-AppendComment-CommentToAppend-Input");
            comment.Clear();
            comment.SendKeys("fail" + Keys.Tab);

            var error = wait.ClickAndWait(".nof-ok", ".validation-summary-errors li");

            error.AssertTextEquals("For test purposes the comment 'fail' fails validation");

            br.FindElement(By.CssSelector("[name=Details]")).AssertTextEquals("2 Sales Orders");

            wait.ClickAndWait("[name=Details]", wd => wd.Title == "2 Sales Orders");
        }

        public abstract void InvokeContributedActionNoSelections();

        public void DoInvokeContributedActionNoSelections() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            wait.ClickAndWait(table, ".nof-collection-table");

            wait.ClickAndWait("#ObjectQuery-SalesOrderHeader-AppendComment", wd => wd.Title == "20 Sales Orders");
         
            Thread.Sleep(5000);

            br.FindElement(By.ClassName("Nof-Warnings")).AssertTextEquals("No objects selected");
        }

        public abstract void PagingWithDefaultPageSize();

        public void DoPagingWithDefaultPageSize() {
            Login();

            var table = wait.ClickAndWait("#OrderRepository-HighestValueOrders button", "button[title=Table]");
            var coll = wait.ClickAndWait(table, ".nof-collection-table");

            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.GetTopObject().Text);

            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 1574", pageNo.Text);
            IWebElement total = br.FindElement(By.ClassName("nof-total-count"));
            Assert.AreEqual("Total of 31465 Sales Orders", total.Text);

            Assert.AreEqual("SO51131", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO51823", coll.TextContentsOfCell(20, 1));

            br.ClickNext();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 2 of 1574", pageNo.Text);
            Assert.AreEqual("SO47441", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO47027", coll.TextContentsOfCell(20, 1));

            br.ClickLast();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("5 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 5 of 31465 Sales Orders", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);
            Assert.AreEqual("SO52682", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO51782", coll.TextContentsOfCell(5, 1));

            br.ClickPrevious();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1573 of 1574", pageNo.Text);
            Assert.AreEqual("SO59092", coll.TextContentsOfCell(1, 1));
            Assert.AreEqual("SO52786", coll.TextContentsOfCell(20, 1));
        }

        public abstract void PagingWithOverriddenPageSize();

        public void DoPagingWithOverriddenPageSize() {
            Login();

            var name = wait.ClickAndWait("#CustomerRepository-FindStoreByName button", "#CustomerRepository-FindStoreByName-Name-Input");
            name.Clear();
            name.SendKeys("a" + Keys.Tab);

            var coll = wait.ClickAndWait(".nof-ok", ".nof-collection-list");

            br.AssertPageTitleEquals("2 Stores");
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.GetTopObject().Text);
            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 249", pageNo.Text);
            IWebElement total = br.FindElement(By.ClassName("nof-total-count"));
            Assert.AreEqual("Total of 497 Stores", total.Text);

            // this tends to change and not sure what it's usefully testing 
            //Assert.AreEqual("Metropolitan Sports Supply, AW00000005", coll.TextContentsOfCell(1, 1));
            //Assert.AreEqual("Aerobic Exercise Company, AW00000006", coll.TextContentsOfCell(2, 1));

            br.ClickNext();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("2 Stores");
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 2 of 249", pageNo.Text);
            //Assert.AreEqual("Associated Bikes, AW00000007", coll.TextContentsOfCell(1, 1));
            //Assert.AreEqual("Exemplary Cycles, AW00000008", coll.TextContentsOfCell(2, 1));

            br.ClickLast();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("1 Store");
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 1 of 497 Stores", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 249 of 249", pageNo.Text);
            //Assert.AreEqual("Underglaze and Finish Company, AW00000700", coll.TextContentsOfCell(1, 1));

            br.ClickPrevious();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("2 Stores");
            Assert.AreEqual("Find Store By Name: Query Result: Viewing 2 of 497 Stores", br.GetTopObject().Text);
            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 248 of 249", pageNo.Text);
            //Assert.AreEqual("Brakes and Gears, AW00000697", coll.TextContentsOfCell(1, 1));
            //Assert.AreEqual("Sensational Discount Store, AW00000699", coll.TextContentsOfCell(2, 1));
        }

        public abstract void PagingWithFormat();

        public void DoPagingWithFormat() {
            Login();
        

            var name = wait.ClickAndWait("#CustomerRepository-FindStoreByName button", "#CustomerRepository-FindStoreByName-Name-Input");
            name.Clear();
            name.SendKeys("a" + Keys.Tab);

            var coll = wait.ClickAndWait(".nof-ok", ".nof-collection-list");
            br.AssertPageTitleEquals("2 Stores");

            // list has 2 columns 
            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickNext();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("2 Stores");

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickLast();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("1 Store");

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickTable();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("1 Store");

            // table has 3 columns
            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickFirst();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("2 Stores");

            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickNext();
            coll = br.GetStandaloneTable();
            br.AssertPageTitleEquals("2 Stores");

            Assert.AreEqual(3, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());

            br.ClickList();
            coll = br.GetStandaloneList();
            br.AssertPageTitleEquals("2 Stores");

            Assert.AreEqual(2, br.FindElement(By.TagName("tr")).FindElements(By.TagName("th")).Count());
        }
    }
}