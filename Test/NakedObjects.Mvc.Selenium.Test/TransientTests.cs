// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Web.UnitTests.Selenium {
    public abstract class TransientTests : AWWebTest {
        protected new const string url = "http://mvc.nakedobjects.net:1081/unittesttransient";

        //protected new const string url = "http://localhost:62468";

        public abstract void PopulateAndNavigateToTransient();

        public void DoPopulateAndNavigateToTransient() {
            Login();
            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("Fred", br);
            br.ClickSave();
            br.GetField("Person-Name").AssertValueEquals("Fred");
            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("Ted", br);
            br.ClickSave();
            br.GetField("Person-Name").AssertValueEquals("Ted");

            Assert.AreEqual(2, br.GetTabbedHistory().FindElements(By.TagName("a")).Count);
            Assert.AreEqual("Fred", br.GetTabbedHistory().FindElements(By.TagName("a")).First().Text);
            Assert.AreEqual("Ted", br.GetTabbedHistory().FindElements(By.TagName("a")).Last().Text);

            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Fred");

            br.ClickTabLink(1);
            br.AssertPageTitleEquals("Ted");
        }

        public abstract void PopulateAndReloadTransient();

        public void DoPopulateAndReloadTransient() {
            Login();
            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("Fred", br);
            br.ClickSave();

            br.Navigate().Refresh();
            br.WaitForAjaxComplete();

            br.AssertPageTitleEquals("Fred");
        }

        public abstract void ValidateOnTransient();

        public void DoValidateOnTransient() {
            Login();
            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("fail", br);
            br.ClickSave();
            br.GetField("Person-Name").AssertValidationErrorIs("fail");
        }


        public abstract void TransientReference();

        public void DoTransientReference() {
            Login();
            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("Fred", br);
            br.ClickSave();
            br.ClickAction("Person-NewPet");
            br.GetField("Person-NewPet-Name").TypeText("Sky", br);
            br.ClickOk();
            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Fred");
            br.ClickEdit();
            br.ClickRecentlyViewed("Person-FavouritePet");
            br.ClickSave();
            br.GetField("Person-FavouritePet").AssertObjectHasTitle("Sky");
        }

        public abstract void TransientCollection();

        public void DoTransientCollection() {
            Login();

            br.ClickAction("SimpleRepository-Person-NewInstance");
            br.ClickEdit();
            br.GetField("Person-Name").TypeText("Fred", br);
            br.ClickSave();
            br.ClickAction("Person-NewPet");
            br.GetField("Person-NewPet-Name").TypeText("Sky", br);
            br.ClickOk();
            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Fred");
            br.ClickAction("Person-NewPet");
            br.GetField("Person-NewPet-Name").TypeText("Sunny", br);
            br.ClickOk();
            br.ClickTabLink(0);
            br.AssertPageTitleEquals("Fred");
            br.ClickAction("Person-AddToPets");

            br.ClickRecentlyViewed("Person-AddToPets-Value");
            br.SelectFinderOption("Person-AddToPets-Value", "Sky");
            br.ClickOk();
            br.ClickAction("Person-AddToPets");

            br.ClickRecentlyViewed("Person-AddToPets-Value");
            br.SelectFinderOption("Person-AddToPets-Value", "Sunny");
            br.ClickOk();

            // Collection Summary
            br.GetInternalCollection("Person-Pets").AssertSummaryEquals("2 Pets");

            // Collection List
            br.ViewAsList("Person-Pets");
            Assert.AreEqual("nof-collection-list", br.GetInternalCollection("Person-Pets").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            IWebElement table = br.GetInternalCollection("Person-Pets").FindElement(By.TagName("table"));
            Assert.AreEqual(2, table.FindElements(By.TagName("tr")).Count);
            Assert.AreEqual("Sky", table.FindElement(By.ClassName("nof-object")).Text);

            // Collection Table
            br.ViewAsTable("Person-Pets");
            Assert.AreEqual("nof-collection-table", br.GetInternalCollection("Person-Pets").FindElements(By.TagName("div"))[1].GetAttribute("class"));
            table = br.GetInternalCollection("Person-Pets").FindElement(By.TagName("table"));
            Assert.AreEqual(3, table.FindElements(By.TagName("tr")).Count); //First row is header
            Assert.AreEqual("Sky", table.FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"))[0].Text);
            Assert.AreEqual("Sunny", table.FindElements(By.TagName("tr"))[2].FindElements(By.TagName("td"))[0].Text);
        }
    }

    [TestClass]
    public class TransientTestsIE : TransientTests {
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
        public override void PopulateAndNavigateToTransient() {
            DoPopulateAndNavigateToTransient();
        }

        [TestMethod]
        //[Ignore] // causes reload prompt in ie
        public override void PopulateAndReloadTransient() {
            DoPopulateAndReloadTransient();
        }

        [TestMethod]
        public override void ValidateOnTransient() {
            DoValidateOnTransient();
        }

        [TestMethod]
        public override void TransientReference() {
            DoTransientReference();
        }

        [TestMethod]
        public override void TransientCollection() {
            DoTransientCollection();
        }
    }

    [TestClass]
    public class TransientTestsFirefox : TransientTests {
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
        public override void PopulateAndReloadTransient() {
            DoPopulateAndReloadTransient();
        }

        [TestMethod]
        public override void ValidateOnTransient() {
            DoValidateOnTransient();
        }

        [TestMethod]
        public override void PopulateAndNavigateToTransient() {
            DoPopulateAndNavigateToTransient();
        }

        [TestMethod]
        public override void TransientReference() {
            DoTransientReference();
        }

        [TestMethod]
        public override void TransientCollection() {
            DoTransientCollection();
        }
    }

    [TestClass]
    public class TransientTestsChrome : TransientTests {
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
        public override void PopulateAndReloadTransient() {
            DoPopulateAndReloadTransient();
        }

        [TestMethod]
        public override void ValidateOnTransient() {
            DoValidateOnTransient();
        }

        [TestMethod]
        public override void PopulateAndNavigateToTransient() {
            DoPopulateAndNavigateToTransient();
        }

        [TestMethod] 
        [Ignore] // fails randomly on server
        public override void TransientReference() {
            DoTransientReference();
        }

        [TestMethod]
        public override void TransientCollection() {
            DoTransientCollection();
        }
    }
}