// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class TabbedHistoryAndClearTests : AWWebTest {
        public abstract void CumulativeHistory();

        public void DoCumulativeHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            // 3rd object
            br.ClickOnObjectLinkInField("SalesPerson-SalesTerritory");
            Assert.AreEqual(3, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            // collection 
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to second object
            br.ClickTabLink(1);
            br.AssertPageTitleEquals("José Saraiva");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            //Go to last object
            br.ClickTabLink(2);
            br.AssertPageTitleEquals("Canada");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            //Go to collection 
            br.ClickTabLink(3);
            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual(4, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetTabbedHistory().FindElements(By.TagName("a"))[2].Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void ClearSingleItem();

        public void DoClearSingleItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public abstract void ClearSingleCollectionItem();

        public void DoClearSingleCollectionItem() {
            Login();
            br.ClickAction("OrderRepository-HighestValueOrders");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }


        public abstract void ClearActiveItem();

        public void DoClearActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }


        public abstract void CollectionKeepsPage();

        public void DoCollectionKeepsPage() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd collection
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            IWebElement pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 1574", pageNo.Text);

            br.ClickLast();
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);

            br.ClickTabLink(0);
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(1);
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("5 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);

            br.AssertPageTitleEquals("5 Sales Orders");
        }

        public abstract void CollectionKeepsFormat();

        public void DoCollectionKeepsFormat() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd collection
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);


            br.ClickTable();
            br.GetStandaloneTable();
        
            br.ClickTabLink(0);
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(1);

            br.GetStandaloneTable();
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);


            br.AssertPageTitleEquals("20 Sales Orders");
        }



        public abstract void ClearActiveCollectionItem();

        public void DoClearActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd collection
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public abstract void ClearActiveMultipleCollectionItems();

        public void DoClearActiveMultipleCollectionItems() {
            Login();
            br.ClickAction("ContactRepository-ValidCountries");

            // 1st collection
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("12 Country Regions", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd collection
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("12 Country Regions", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(1);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("12 Country Regions", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("12 Country Regions");
        }

        public abstract void ClearInActiveItem();

        public void DoClearInActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("José Saraiva");
        }




        public abstract void ClearInActiveCollectionItem();

        public void DoClearInActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public abstract void ClearInActiveCollectionMultipleItems();

        public void DoClearInActiveCollectionMultipleItems() {
            Login();
            br.ClickAction("ContactRepository-ValidCountries");

            // 1st collection
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("12 Country Regions", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd collection
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("12 Country Regions", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearItem(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }


        public abstract void ClearOthersSingleItem();

        public void DoClearOthersSingleItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public abstract void ClearOthersSingleCollectionItem();

        public void DoClearOthersSingleCollectionItem() {
            Login();
            br.ClickAction("OrderRepository-HighestValueOrders");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            br.AssertPageTitleEquals("20 Sales Orders");
        }


        public abstract void ClearOthersActiveItem();

        public void DoClearOthersActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(1);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("José Saraiva");
        }

        public abstract void ClearOthersActiveCollectionItem();

        public void DoClearOthersActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(1);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.AssertPageTitleEquals("20 Sales Orders");
        }

        public abstract void ClearOthersInActiveItem();

        public void DoClearOthersInActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public abstract void ClearOthersInActiveCollectionItem();

        public void DoClearOthersInActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearOthers(0);
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }

        public abstract void ClearAllSingleItem();

        public void DoClearAllSingleItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public abstract void ClearAllSingleCollectionItem();

        public void DoClearAllSingleCollectionItem() {
            Login();
            br.ClickAction("OrderRepository-HighestValueOrders");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }


        public abstract void ClearAllActiveItem();

        public void DoClearAllActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public abstract void ClearAllActiveCollectionItem();

        public void DoClearAllActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }


        public abstract void ClearAllInActiveItem();

        public void DoClearAllInActiveItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }

        public abstract void ClearAllInActiveCollectionItem();

        public void DoClearAllInActiveCollectionItem() {
            Login();
            FindCustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickAction("OrderRepository-HighestValueOrders");
            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("20 Sales Orders", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            br.AssertPageTitleEquals("Home Page");
        }



        public abstract void TransientObjectsDoNotShowUpInHistory();

        public void DoTransientObjectsDoNotShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("CustomerRepository-CreateNewStoreCustomer");
            br.AssertContainsObjectEditTransient();

            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);

            br.GetField("Store-Name").TypeText("Foo Bar", br);
            br.ClickSave();

            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetTabbedHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Foo Bar, AW00029484", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void CollectionsShowUpInHistory();

        public void DoCollectionsShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("OrderRepository-HighestValueOrders");
            br.AssertPageTitleEquals("20 Sales Orders");

            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
        }
    }

    [TestClass]
    public class TabbedHistoryAndClearTestsIE : TabbedHistoryAndClearTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            AWWebTest.InitialiseClass(context);
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
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void ClearSingleItem() {
            DoClearSingleItem();
        }

        [TestMethod]
        public override void ClearSingleCollectionItem () {
            DoClearSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveItem() {
            DoClearActiveItem();
        }

        [TestMethod]
        public override void CollectionKeepsPage() {
            DoCollectionKeepsPage();
        }

        [TestMethod]
        public override void CollectionKeepsFormat() {
            DoCollectionKeepsFormat();
        }

        [TestMethod]
        public override void ClearActiveCollectionItem() {
           DoClearActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveMultipleCollectionItems() {
            DoClearActiveMultipleCollectionItems();
        }

        [TestMethod]
        public override void ClearInActiveItem() {
            DoClearInActiveItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionItem() {
           DoClearInActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionMultipleItems() {
            DoClearInActiveCollectionMultipleItems();
        }

        [TestMethod]
        public override void ClearOthersSingleItem() {
            DoClearOthersSingleItem();
        }

        [TestMethod]
        public override void ClearOthersSingleCollectionItem() {
           DoClearOthersSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersActiveItem() {
            DoClearOthersActiveItem();
        }

        [TestMethod]
        public override void ClearOthersActiveCollectionItem() {
           DoClearOthersActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveItem() {
            DoClearOthersInActiveItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveCollectionItem() {
           DoClearOthersInActiveCollectionItem();
        }


        [TestMethod]
        public override void ClearAllSingleItem() {
            DoClearAllSingleItem();
        }

        [TestMethod]
        public override void ClearAllSingleCollectionItem() {
            DoClearAllSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearAllActiveItem() {
            DoClearAllActiveItem();
        }

        [TestMethod]
        public override void ClearAllActiveCollectionItem() {
            DoClearAllActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearAllInActiveItem() {
            DoClearAllInActiveItem();
        }

        [TestMethod]
        public override void ClearAllInActiveCollectionItem() {
           DoClearAllInActiveCollectionItem();
        }

        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsShowUpInHistory() {
            DoCollectionsShowUpInHistory();
        }
    }

    [TestClass]
    public class TabbedHistoryAndClearTestsFirefox : TabbedHistoryAndClearTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void ClearSingleItem() {
            DoClearSingleItem();
        }

        [TestMethod]
        public override void ClearSingleCollectionItem() {
            DoClearSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveItem() {
            DoClearActiveItem();
        }

        [TestMethod]
        public override void CollectionKeepsPage() {
            DoCollectionKeepsPage();
        }

        [TestMethod]
        public override void CollectionKeepsFormat() {
            DoCollectionKeepsFormat();
        }

        [TestMethod]
        public override void ClearActiveCollectionItem() {
           DoClearActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveMultipleCollectionItems() {
            DoClearActiveMultipleCollectionItems();
        }

        [TestMethod]
        public override void ClearInActiveItem() {
            DoClearInActiveItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionItem() {
           DoClearInActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionMultipleItems() {
            DoClearInActiveCollectionMultipleItems();
        }

        [TestMethod]
        public override void ClearOthersSingleItem() {
            DoClearOthersSingleItem();
        }

        [TestMethod]
        public override void ClearOthersSingleCollectionItem() {
           DoClearOthersSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersActiveItem() {
            DoClearOthersActiveItem();
        }

        [TestMethod]
        public override void ClearOthersActiveCollectionItem() {
           DoClearOthersActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveItem() {
            DoClearOthersInActiveItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveCollectionItem() {
           DoClearOthersInActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearAllSingleItem() {
            DoClearAllSingleItem();
        }

        [TestMethod]
        public override void ClearAllSingleCollectionItem() {
           DoClearAllSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearAllActiveItem() {
            DoClearAllActiveItem();
        }

        [TestMethod]
        public override void ClearAllActiveCollectionItem() {
           DoClearAllActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearAllInActiveItem() {
            DoClearAllInActiveItem();
        }

        [TestMethod]
        public override void ClearAllInActiveCollectionItem() {
            DoClearAllInActiveCollectionItem();
        }

        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsShowUpInHistory() {
            DoCollectionsShowUpInHistory();
        }
    }

    [TestClass, Ignore] //no longer working - investigate later
    public class TabbedHistoryAndClearTestsChrome : TabbedHistoryAndClearTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
            FindCustomerByAccountNumber("AW00000065");
            br.ClickClearAll(0);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void ClearSingleItem() {
            DoClearSingleItem();
        }

        [TestMethod]
        public override void ClearSingleCollectionItem() {
            DoClearSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveItem() {
            DoClearActiveItem();
        }

        [TestMethod]
        public override void CollectionKeepsPage() {
           DoCollectionKeepsPage();
        }

        [TestMethod]
        public override void CollectionKeepsFormat() {
           DoCollectionKeepsFormat();
        }

        [TestMethod]
        public override void ClearActiveCollectionItem() {
            DoClearActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearActiveMultipleCollectionItems() {
            DoClearActiveMultipleCollectionItems();
        }

        [TestMethod]
        public override void ClearInActiveItem() {
            DoClearInActiveItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionItem() {
           DoClearInActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearInActiveCollectionMultipleItems() {
            DoClearInActiveCollectionMultipleItems();
        }

        [TestMethod]
        public override void ClearOthersSingleItem() {
            DoClearOthersSingleItem();
        }

        [TestMethod]
        public override void ClearOthersSingleCollectionItem() {
            DoClearOthersSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersActiveItem() {
            DoClearOthersActiveItem();
        }

        [TestMethod]
        public override void ClearOthersActiveCollectionItem() {
            DoClearOthersActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveItem() {
            DoClearOthersInActiveItem();
        }

        [TestMethod]
        public override void ClearOthersInActiveCollectionItem() {
           DoClearOthersInActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearAllSingleItem() {
            DoClearAllSingleItem();
        }

        [TestMethod]
        public override void ClearAllSingleCollectionItem() {
          DoClearAllSingleCollectionItem();
        }

        [TestMethod]
        public override void ClearAllActiveItem() {
            DoClearAllActiveItem();
        }

        [TestMethod]
        public override void ClearAllActiveCollectionItem() {
           DoClearAllActiveCollectionItem();
        }

        [TestMethod]
        public override void ClearAllInActiveItem() {
            DoClearAllInActiveItem();
        }

        [TestMethod]
        public override void ClearAllInActiveCollectionItem() {
            DoClearAllInActiveCollectionItem();
        }


        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsShowUpInHistory() {
            DoCollectionsShowUpInHistory();
        }
    }
}