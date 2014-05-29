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
    public abstract class HistoryAndClearTests : AWWebTest {
        public abstract void HistoryOnIndex();

        public void DoHistoryOnIndex() {
            Login();
            // title goes to 'Home Page' the empty - may change when #1602 is fixed - in meantime wait a bit so page test is consistent
            Thread.Sleep(2000);
            // br.AssertPageTitleEquals("Home Page"); currently IIS7 - to do fix home page title
            //IWebElement history = br.FindElement(By.ClassName("History"));
            br.AssertElementExists(By.ClassName("nof-history"));
        }

        public abstract void ClearHistoryOnIndex();

        public void DoClearHistoryOnIndex() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            br.GoHome();
            br.AssertElementExists(By.CssSelector("[Title=Clear]"));
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            br.ClickClearHistory();
            br.AssertElementDoesNotExist(By.CssSelector("[Title=Clear]"));
            Assert.AreEqual(0, br.GetHistory().FindElements(By.TagName("a")).Count);
        }


        public abstract void CumulativeHistory();

        public void DoCumulativeHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            br.AssertElementExists(By.CssSelector("[Title=Clear]"));

            // 1st object
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);

            // 2nd object
            br.ClickOnObjectLinkInField("Store-SalesPerson");
            Assert.AreEqual(2, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            // 3rd object
            br.ClickOnObjectLinkInField("SalesPerson-SalesTerritory");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to second object
            br.GoBackViaHistoryBy(1);
            br.AssertPageTitleEquals("José Saraiva");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElement(By.TagName("a")).Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.GoBackViaHistoryBy(2);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Click on last object has no effect
            br.GoBackViaHistoryBy(0);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).Last().Text);

            //Go back to first object
            br.GoBackViaHistoryBy(2);
            br.AssertPageTitleEquals("Canada");
            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("José Saraiva", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a"))[1].Text);
            Assert.AreEqual("Canada", br.GetHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void TransientObjectsDoNotShowUpInHistory();

        public void DoTransientObjectsDoNotShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("CustomerRepository-CreateNewStoreCustomer");
            br.AssertContainsObjectEditTransient();

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.GetField("Store-Name").TypeText("Foo Bar", br);
            br.ClickSave();

            Assert.AreEqual(2, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a"))[0].Text);
            Assert.AreEqual("Foo Bar, AW00029484", br.GetHistory().FindElements(By.TagName("a")).Last().Text);
        }

        public abstract void CollectionsDoNotShowUpInHistory();

        public void DoCollectionsDoNotShowUpInHistory() {
            Login();
            FindCustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickAction("OrderRepository-HighestValueOrders");
            br.AssertPageTitleEquals("20 Sales Orders");

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
        }

        public abstract void ClearButton();

        public void DoClearButton() {
            Login();
            FindCustomerByAccountNumber("AW00000067");
            FindCustomerByAccountNumber("AW00000066");
            FindCustomerByAccountNumber("AW00000065");
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");

            Assert.AreEqual(3, br.GetHistory().FindElements(By.TagName("a")).Count);

            br.ClickClearHistory();

            Assert.AreEqual(1, br.GetHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.GetHistory().FindElements(By.TagName("a")).First().Text);
            br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");
        }


        //public abstract void DragFromHistory();

        //public void DoDragFromHistory() {
        //    FindCustomerByAccountNumber("AW00000065");
        //    IWebElement history = br.GetHistory();
        //    Assert.IsTrue(history.Button(Find.ByTitle("Clear")).Exists);

        //    // 1st object
        //    Assert.AreEqual(1, history.Links.Count);
        //    Assert.AreEqual("Metro Manufacturing, AW00000065", history.Links.First().Text);

        //    // 2nd object
        //    br.ClickOnObjectLinkInField("Store-SalesPerson");
        //    Assert.AreEqual(2, history.Links.Count);
        //    Assert.AreEqual("Metro Manufacturing, AW00000065", history.Links.First().Text);
        //    Assert.AreEqual("José Saraiva", history.Links.Last().Text);

        //    //Go back to first object
        //    br.GoBackViaHistoryBy(1);
        //    br.AssertPageTitleEquals("Metro Manufacturing, AW00000065");

        //    // drag sales person 
        //    var spHistory = history.IWebElements.First();
        //    var startPosition = br.FindPosition(spHistory);

        //   // br.Eval(@"$('.ui-draggable:first').mousedown()");
        //    br.Eval(@"$('.ui-draggable:first').trigger({type:'mousedown', which: 1})");
        //    br.Eval(@"$('.ui-draggable:first').mousemove()"); 

        //    Thread.Sleep(500);

        //    var dropPosition = br.FindPosition(br.GetField("Store-SalesPerson"));

        //    //move down then across

        //    int x = startPosition[0];
        //    for (; x - dropPosition[0]  > 0; x--) {


        //        string js = string.Format(@"$('.ui-draggable-dragging').mousemove( {{pageX : {0}, pageY : {1} }})", x, startPosition[1]);
        //        br.Eval(js); 
        //    }

        //    for (int y = startPosition[1]; dropPosition[1] - y > 0; y++) {


        //        string js = string.Format(@"$('.ui-draggable-dragging').mousemove( {{pageX : {0}, pageY : {1} }})", startPosition[0], y);
        //        br.Eval(js); 
        //    }


        //}
    }


    // Replaced by tabbed history - keep tests until old history is removed 
    [TestClass, Ignore]
    public class HistoryAndClearTestsIE : HistoryAndClearTests {
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
        public override void HistoryOnIndex() {
            DoHistoryOnIndex();
        }

        [TestMethod]
        public override void ClearHistoryOnIndex() {
            DoClearHistoryOnIndex();
        }

        [TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsDoNotShowUpInHistory() {
            DoCollectionsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void ClearButton() {
            DoClearButton();
        }
    }

    [TestClass, Ignore]
    public class HistoryAndClearTestsFirefox : HistoryAndClearTests {
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
        public override void HistoryOnIndex() {
            DoHistoryOnIndex();
        }

        [TestMethod]
        public override void ClearHistoryOnIndex() {
            DoClearHistoryOnIndex();
        }

        [TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsDoNotShowUpInHistory() {
            DoCollectionsDoNotShowUpInHistory();
        }

        [TestMethod] // fails too often 
        public override void ClearButton() {
            DoClearButton();
        }
    }

    [TestClass, Ignore]
    public class HistoryAndClearTestsChrome : HistoryAndClearTests {
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
        public override void HistoryOnIndex() {
            DoHistoryOnIndex();
        }

        [TestMethod]
        public override void ClearHistoryOnIndex() {
            DoClearHistoryOnIndex();
        }

        [TestMethod]
        public override void CumulativeHistory() {
            DoCumulativeHistory();
        }

        [TestMethod]
        public override void TransientObjectsDoNotShowUpInHistory() {
            DoTransientObjectsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void CollectionsDoNotShowUpInHistory() {
            DoCollectionsDoNotShowUpInHistory();
        }

        [TestMethod]
        public override void ClearButton() {
            DoClearButton();
        }
    }
}