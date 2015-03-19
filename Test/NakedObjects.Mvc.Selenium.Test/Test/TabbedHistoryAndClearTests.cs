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
    public abstract class TabbedHistoryAndClearTests : AWWebTest {
        public abstract void CumulativeHistory();

        private void CustomerByAccountNumber(string accountNumber) {
            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            f.Clear();
            f.SendKeys(accountNumber + Keys.Tab);

            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoCumulativeHistory() {
            Login();

            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            // 3rd object
            wait.ClickAndWait("#SalesPerson-SalesTerritory a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 3);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            // collection 
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 4);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            //Go back to second object
            br.ClickTabLink(1);
            br.AssertPageTitleEquals("José Saraiva");
            Assert.AreEqual(4, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(4, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(4, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            //Go to last object
            br.ClickTabLink(2);
            br.AssertPageTitleEquals("Canada");
            Assert.AreEqual(4, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            //Go to collection 
            br.ClickTabLink(3);
            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual(4, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);
        }

        public void DoClearSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearSingleCollectionItem() {
            Login();

            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", ".nof-tabbed-history");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);
            wait.Until(wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public void DoCollectionKeepsPage() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 1574", pageNo.Text);

            br.ClickLast();
            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);

            br.ClickTabLink(0);
            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(1);
            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);

            br.AssertPageTitleEquals("5 Sales Orders");
        }

        public void DoCollectionKeepsFormat() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickTable();
            br.GetStandaloneTable();

            br.ClickTabLink(0);
            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(1);

            br.GetStandaloneTable();
            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public void DoClearActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);

            wait.Until(wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public void DoClearActiveMultipleCollectionItems() {
            Login();

            // 1st collection
            wait.ClickAndWait("#ContactRepository-ValidCountries button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);    
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);
            wait.Until(wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("12 Country Regions");
        }

        public void DoClearInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("José Saraiva");
        }

        public void DoClearInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public void DoClearInActiveCollectionMultipleItems() {
            Login();
           
            wait.ClickAndWait("#ContactRepository-ValidCountries button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);

            // 1st collection 
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public void DoClearOthersSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public void DoClearOthersSingleCollectionItem() {
            Login();
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);

            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public void DoClearOthersActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(1);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("José Saraiva");
        }

        public void DoClearOthersActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(1);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public void DoClearOthersInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public void DoClearOthersInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public void DoClearAllSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearAllSingleCollectionItem() {
            Login();
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 1);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearAllActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearAllActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearAllInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoClearAllInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public void DoTransientObjectsDoNotShowUpInHistory() {
            Login();
            CustomerByAccountNumber("AW00000065");

            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);

            wait.ClickAndWait("#CustomerRepository-CreateNewStoreCustomer button", ".nof-objectedit");

            br.AssertContainsObjectEditTransient();

            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);

            br.FindElement(By.CssSelector("#Store-Name")).TypeText("Foo Bar", br);
            br.ClickSave();

            Assert.AreEqual(2, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Foo Bar, AW00029484", br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Last().Text);
        }

        public void DoCollectionsShowUpInHistory() {
            Login();
            CustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count);

            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElement(By.CssSelector(".nof-tabbed-history")).FindElements(By.TagName("a")).Count == 2);
            br.AssertPageTitleEquals("20 Sales Orders");
        }

        #region abstract

        public abstract void ClearSingleItem();
        public abstract void ClearSingleCollectionItem();
        public abstract void ClearActiveItem();
        public abstract void CollectionsShowUpInHistory();
        public abstract void TransientObjectsDoNotShowUpInHistory();
        public abstract void ClearAllInActiveCollectionItem();
        public abstract void ClearAllInActiveItem();
        public abstract void ClearAllActiveCollectionItem();
        public abstract void ClearAllActiveItem();
        public abstract void ClearAllSingleCollectionItem();
        public abstract void ClearAllSingleItem();
        public abstract void ClearOthersInActiveCollectionItem();
        public abstract void ClearOthersInActiveItem();
        public abstract void ClearOthersActiveCollectionItem();
        public abstract void ClearOthersActiveItem();
        public abstract void ClearOthersSingleCollectionItem();
        public abstract void ClearOthersSingleItem();
        public abstract void ClearInActiveCollectionMultipleItems();
        public abstract void ClearInActiveCollectionItem();
        public abstract void ClearInActiveItem();
        public abstract void ClearActiveMultipleCollectionItems();
        public abstract void ClearActiveCollectionItem();
        public abstract void CollectionKeepsFormat();
        public abstract void CollectionKeepsPage();

        #endregion
    }
}