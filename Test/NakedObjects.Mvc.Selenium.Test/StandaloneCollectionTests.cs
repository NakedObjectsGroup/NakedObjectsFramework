// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class StandaloneCollectionTests : AWWebTest {
        public abstract void ViewStandaloneCollection();

        public void DoViewStandaloneCollection() {
            Login();
            br.ClickAction("OrderRepository-HighestValueOrders");
            br.AssertPageTitleEquals("20 Sales Orders");
            Assert.AreEqual("Highest Value Orders: Query Result: Viewing 20 of 31465 Sales Orders", br.GetTopObject().Text);
            IWebElement table = br.FindElement(By.ClassName("SalesOrderHeader"));
            Assert.AreEqual(21, table.FindElements(By.TagName("tr")).Count);
        }

        public abstract void ViewStandaloneCollectionTable();

        public void DoViewStandaloneCollectionTable() {
            Login();
            br.ClickAction("SpecialOfferRepository-CurrentSpecialOffers");
            br.AssertPageTitleEquals("7 Special Offers");
            Assert.AreEqual("Current Special Offers: Query Result: Viewing 7 of 7 Special Offers", br.GetTopObject().Text);

            br.FindElement(By.ClassName("nof-table")).BrowserSpecificClick(br);
            br.WaitForAjaxComplete();

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
            br.ClickAction("EmployeeRepository-ListAllDepartments");
            br.AssertPageTitleEquals("16 Departments");
            Assert.AreEqual("List All Departments: Query Result: Viewing 16 of 16 Departments", br.GetTopObject().Text);

            IWebElement table = br.FindElement(By.ClassName("Department"));

            Assert.AreEqual(17, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual(3, table.FindElements(By.TagName("tr"))[0].FindElements(By.TagName("th")).Count);
            Assert.AreEqual(3, table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td")).Count);
        }


        public abstract void SelectDeselectAll();

        public void DoSelectDeselectAll() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();

            Assert.AreEqual(0, CountCheckedBoxes(coll));

            coll.CheckAll(br);
            Assert.IsTrue(coll.FindElements(By.CssSelector("input[type=checkbox]")).First().Selected);
            Assert.AreEqual(21, CountCheckedBoxes(coll)); //21 because this will include the 'all box'

            coll.UnCheckAll(br); //To deselect
            Assert.AreEqual(0, CountCheckedBoxes(coll));
        }

        private static int CountCheckedBoxes(IWebElement coll) {
            return coll.FindElements(By.CssSelector("input[type=checkbox]")).Where(x => x.GetAttribute("id").StartsWith("checkbox") && x.Selected).Count();
        }

        public abstract void SelectAndUnselectIndividually();

        public void DoSelectAndUnselectIndividually() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();

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
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();

            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(1, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(2, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(3, 6)));

            coll.GetRow(1).CheckRow(br);
            coll.GetRow(3).CheckRow(br);

            br.ClickAction("ObjectQuery-SalesOrderHeader-CommentAsUsersUnhappy");

            br.AssertPageTitleEquals("20 Sales Orders");

            coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();
            Assert.AreEqual("User unhappy", coll.TextContentsOfCell(1, 6));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(2, 6)));
            Assert.AreEqual("User unhappy", coll.TextContentsOfCell(3, 6));
        }

        public abstract void InvokeContributedActionParmsNoReturn();

        public void DoInvokeContributedActionParmsNoReturn() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(7, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(8, 6)));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(9, 6)));

            coll.GetRow(7).CheckRow(br);
            coll.GetRow(9).CheckRow(br);
            br.ClickAction("ObjectQuery-SalesOrderHeader-AppendComment");
            br.GetField("OrderContributedActions-AppendComment-CommentToAppend").TypeText("Foo", br);
            br.ClickOk();

            br.AssertPageTitleEquals("20 Sales Orders");

            coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();
            Assert.AreEqual("Foo", coll.TextContentsOfCell(7, 6));
            Assert.IsTrue(string.IsNullOrEmpty(coll.TextContentsOfCell(8, 6)));
            Assert.AreEqual("Foo", coll.TextContentsOfCell(9, 6));
        }

        public abstract void InvokeContributedActionParmsValidateFail();

        public void DoInvokeContributedActionParmsValidateFail() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();
        
            coll.GetRow(7).CheckRow(br);
            coll.GetRow(9).CheckRow(br);
            br.ClickAction("ObjectQuery-SalesOrderHeader-AppendComment");
            br.GetField("OrderContributedActions-AppendComment-CommentToAppend").TypeText("fail", br);
            br.ClickOk();

            br.FindElement(By.CssSelector(".validation-summary-errors li")).AssertTextEquals("For test purposes the comment 'fail' fails validation");

            br.FindElement(By.CssSelector("[name=Details]")).AssertTextEquals("2 Sales Orders");

            br.FindElement(By.CssSelector("[name=Details]")).Click();
            br.WaitForAjaxComplete();    

            br.AssertPageTitleEquals("2 Sales Orders");
        }


        public abstract void InvokeContributedActionNoSelections();

        public void DoInvokeContributedActionNoSelections() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();

            br.ClickAction("ObjectQuery-SalesOrderHeader-AppendComment");

            br.AssertPageTitleEquals("20 Sales Orders");

            Thread.Sleep(5000);

            br.FindElement(By.ClassName("Nof-Warnings")).AssertTextEquals("No objects selected");
        }

        public abstract void PagingWithDefaultPageSize();

        public void DoPagingWithDefaultPageSize() {
            Login();
            IWebElement coll = br.ClickAction("OrderRepository-HighestValueOrders").ClickTable().GetStandaloneTable();
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
            br.ClickAction("CustomerRepository-FindStoreByName");
            br.GetField("CustomerRepository-FindStoreByName-Name").TypeText("a", br);
            br.ClickOk();
            IWebElement coll = br.GetStandaloneList();
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
            br.ClickAction("CustomerRepository-FindStoreByName");
            br.GetField("CustomerRepository-FindStoreByName-Name").TypeText("a", br);
            br.ClickOk();
            IWebElement coll = br.GetStandaloneList();
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

    [TestClass, Ignore]
    public class StandaloneCollectionTestsIE : StandaloneCollectionTests {
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
        public override void ViewStandaloneCollection() {
            DoViewStandaloneCollection();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionTable() {
            DoViewStandaloneCollectionTable();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionDefaultToTable() {
            DoViewStandaloneCollectionDefaultToTable();
        }

        [TestMethod]
        public override void SelectDeselectAll() {
            DoSelectDeselectAll();
        }

        [TestMethod]
        public override void SelectAndUnselectIndividually() {
            DoSelectAndUnselectIndividually();
        }

        [TestMethod, Ignore]
        public override void InvokeContributedActionNoParmsNoReturn() {
            DoInvokeContributedActionNoParmsNoReturn();
        }

        [TestMethod, Ignore]
        public override void InvokeContributedActionParmsNoReturn() {
            DoInvokeContributedActionParmsNoReturn();
        }

        [TestMethod, Ignore]
        public override void InvokeContributedActionParmsValidateFail() {
           DoInvokeContributedActionParmsValidateFail();
        }

        [TestMethod, Ignore]
        public override void InvokeContributedActionNoSelections() {
            DoInvokeContributedActionNoSelections();
        }

        [TestMethod]
        public override void PagingWithDefaultPageSize() {
            DoPagingWithDefaultPageSize();
        }

        [TestMethod]
        public override void PagingWithOverriddenPageSize() {
            DoPagingWithOverriddenPageSize();
        }

        [TestMethod]
        public override void PagingWithFormat() {
            DoPagingWithFormat();
        }
    }

    [TestClass]
    public class StandaloneCollectionTestsFirefox : StandaloneCollectionTests {
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
        public override void ViewStandaloneCollection() {
            DoViewStandaloneCollection();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionTable() {
            DoViewStandaloneCollectionTable();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionDefaultToTable() {
            DoViewStandaloneCollectionDefaultToTable();
        }

        [TestMethod]
        public override void SelectDeselectAll() {
            DoSelectDeselectAll();
        }

        [TestMethod]
        public override void SelectAndUnselectIndividually() {
            DoSelectAndUnselectIndividually();
        }

        [TestMethod]
        public override void InvokeContributedActionNoParmsNoReturn() {
            DoInvokeContributedActionNoParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsNoReturn() {
            DoInvokeContributedActionParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsValidateFail() {
            DoInvokeContributedActionParmsValidateFail();
        }

        [TestMethod]
        public override void InvokeContributedActionNoSelections() {
            DoInvokeContributedActionNoSelections();
        }

        [TestMethod]
        public override void PagingWithDefaultPageSize() {
            DoPagingWithDefaultPageSize();
        }

        [TestMethod] // fails on server too often
        public override void PagingWithOverriddenPageSize() {
            DoPagingWithOverriddenPageSize();
        }

        [TestMethod]
        public override void PagingWithFormat() {
            DoPagingWithFormat();
        }
    }

    [TestClass, Ignore]
    public class StandaloneCollectionTestsChrome : StandaloneCollectionTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            AWWebTest.InitialiseClass(context);
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
        public override void ViewStandaloneCollection() {
            DoViewStandaloneCollection();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionTable() {
            DoViewStandaloneCollectionTable();
        }

        [TestMethod]
        public override void ViewStandaloneCollectionDefaultToTable() {
            DoViewStandaloneCollectionDefaultToTable();
        }

        [TestMethod]
        public override void SelectDeselectAll() {
            DoSelectDeselectAll();
        }

        [TestMethod]
        public override void SelectAndUnselectIndividually() {
            DoSelectAndUnselectIndividually();
        }

        [TestMethod]
        public override void InvokeContributedActionNoParmsNoReturn() {
            DoInvokeContributedActionNoParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsNoReturn() {
            DoInvokeContributedActionParmsNoReturn();
        }

        [TestMethod]
        public override void InvokeContributedActionParmsValidateFail() {
            DoInvokeContributedActionParmsValidateFail();
        }

        [TestMethod]
        public override void InvokeContributedActionNoSelections() {
            DoInvokeContributedActionNoSelections();
        }

        [TestMethod]
        public override void PagingWithDefaultPageSize() {
            DoPagingWithDefaultPageSize();
        }

        [TestMethod]
        public override void PagingWithOverriddenPageSize() {
            DoPagingWithOverriddenPageSize();
        }

        [TestMethod]
        public override void PagingWithFormat() {
            DoPagingWithFormat();
        }
    }
}