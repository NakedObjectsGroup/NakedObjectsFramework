// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class ObjectEditPageTests : SpiroTest {
       
        [TestMethod]
        public virtual void ObjectEditChangeScalar() {
            br.Navigate().GoToUrl(product469Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));

            // set price and days to mfctr

            br.FindElement(By.CssSelector("div#listprice input")).SendKeys( Keys.Backspace + Keys.Backspace + Keys.Backspace + "100");
            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");

            Click(br.FindElement(By.ClassName("save")));

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("List Price:\r\n100", properties[5].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
          
        }


        [TestMethod]
        public virtual void ObjectEditChangeDateTime()
        {
            br.Navigate().GoToUrl(product469Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));

            // set price and days to mfctr

            var today = DateTime.Now.ToString("d MMM yyyy");

            for (int i = 0; i < 12; i++) {
                br.FindElement(By.CssSelector("div#sellstartdate input")).SendKeys(Keys.Backspace);
            }

            br.FindElement(By.CssSelector("div#sellstartdate input")).SendKeys(today + Keys.Tab);
            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");

            Click(br.FindElement(By.ClassName("save")));

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("Sell Start Date:\r\n" + today, properties[18].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeChoices()
        {
            br.Navigate().GoToUrl(product469Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));

            // set product line 

            br.FindElement(By.CssSelector("#productline  select")).SendKeys("S");

            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");

            Click(br.FindElement(By.ClassName("save")));

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
         

        }

        [TestMethod]
        public virtual void ObjectEditChangeConditionalChoices()
        {
            br.Navigate().GoToUrl(product469Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            wait.Until(d => d.FindElement(By.ClassName("edit")).Displayed);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));

            // set product category and sub category

            Assert.AreEqual("Bikes", br.FindElement(By.CssSelector("#productcategory  select option[selected=selected]")).Text);
            Assert.AreEqual(4, br.FindElements(By.CssSelector("#productcategory  select option")).Count);
            Assert.AreEqual(4, br.FindElements(By.CssSelector("#productsubcategory  select option")).Count);

            br.FindElement(By.CssSelector("#productcategory  select")).SendKeys("Clothing" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("#productsubcategory  select option")).Count == 9);

            br.FindElement(By.CssSelector("#productsubcategory  select")).SendKeys("Caps");

            Click(br.FindElement(By.ClassName("save")));

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nCaps", properties[7].Text);

            Click(br.FindElement(By.ClassName("edit")));

            wait.Until(d => br.FindElement(By.ClassName("save")));

            // set product category and sub category

            Assert.AreEqual("Clothing", br.FindElement(By.CssSelector("#productcategory  select option[selected=selected]")).Text);
            Assert.AreEqual(4, br.FindElements(By.CssSelector("#productcategory  select option")).Count);
            Assert.AreEqual(9, br.FindElements(By.CssSelector("#productsubcategory  select option")).Count);

            br.FindElement(By.CssSelector("#productcategory  select")).SendKeys("Bikes" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("#productsubcategory  select option")).Count == 4);

            br.FindElement(By.CssSelector("#productsubcategory  select")).SendKeys("Mountain Bikes");

            Click(br.FindElement(By.ClassName("save")));

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == 7);

            properties = br.FindElements(By.ClassName("property"));

            Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);
        }






    }

    #region browsers specific subclasses

    [TestClass, Ignore]
    public class ObjectEditPageTestsIe : ObjectEditPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class ObjectEditPageTestsFirefox : ObjectEditPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    [TestClass, Ignore]
    public class ObjectEditPageTestsChrome : ObjectEditPageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    #endregion
}