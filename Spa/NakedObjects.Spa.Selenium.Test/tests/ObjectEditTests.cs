// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class ObjectEditTests : GeminiTest {

        [TestMethod]
        public virtual void ObjectEditChangeScalar() {
            br.Navigate().GoToUrl(Product870Url);
            EditObject();

            // set price and days to mfctr
            br.FindElement(By.CssSelector("div#listprice input")).SendKeys(Keys.Backspace + Keys.Backspace + Keys.Backspace + "100");
            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("List Price:\r\n4100", properties[5].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeDateTime() {
            br.Navigate().GoToUrl(Product870Url);
            EditObject();

            // set price and days to mfctr
            var date = new DateTime(2014, 7, 18, 0, 0, 0, DateTimeKind.Utc);
            var dateStr = date.ToString("d MMM yyyy");

            for (int i = 0; i < 12; i++) {
                br.FindElement(By.CssSelector("div#sellstartdate input")).SendKeys(Keys.Backspace);
            }

            br.FindElement(By.CssSelector("div#sellstartdate input")).SendKeys(dateStr + Keys.Tab);
            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Sell Start Date:\r\n" + dateStr, properties[18].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeChoices() {
            br.Navigate().GoToUrl(Product870Url);
            EditObject();

            // set product line 

            br.FindElement(By.CssSelector("#productline  select")).SendKeys("S");

            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeConditionalChoices() {
            br.Navigate().GoToUrl(Product870Url);
            EditObject();
            // set product category and sub category

            var selected = new SelectElement(br.FindElement(By.CssSelector("#productcategory  select")));

            // this makes tests really fragile
            //Assert.AreEqual("Accessories", selected.SelectedOption.Text);

            //Assert.AreEqual(4, br.FindElements(By.CssSelector("#productcategory  select option")).Count);
            //Assert.AreEqual(13, br.FindElements(By.CssSelector("#productsubcategory  select option")).Count);

            br.FindElement(By.CssSelector("#productcategory  select")).SendKeys("Clothing" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("#productsubcategory  select option")).Count == 9);

            br.FindElement(By.CssSelector("#productsubcategory  select")).SendKeys("Caps");

            br.FindElement(By.CssSelector("div#daystomanufacture input")).SendKeys(Keys.Backspace + "1");

            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nCaps", properties[7].Text);

            EditObject();

            // set product category and sub category

            var slctd = new SelectElement(br.FindElement(By.CssSelector("#productcategory  select")));

            Assert.AreEqual("Clothing", slctd.SelectedOption.Text);

            Assert.AreEqual(4, br.FindElements(By.CssSelector("#productcategory  select option")).Count);

            wait.Until(d => d.FindElements(By.CssSelector("#productsubcategory  select option")).Count == 9);

            Assert.AreEqual(9, br.FindElements(By.CssSelector("#productsubcategory  select option")).Count);

            br.FindElement(By.CssSelector("#productcategory  select")).SendKeys("Bikes" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("#productsubcategory  select option")).Count == 4);

            br.FindElement(By.CssSelector("#productsubcategory  select")).SendKeys("Mountain Bikes");

            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);

            // set values back
            EditObject();

            br.FindElement(By.CssSelector("#productcategory  select")).SendKeys("Accessories" + Keys.Tab);

            var slpsc = new SelectElement(br.FindElement(By.CssSelector("#productsubcategory  select")));
            wait.Until(d => slpsc.Options.Count == 13);

            br.FindElement(By.CssSelector("#productsubcategory  select")).SendKeys("Bottles and Cages" + Keys.Tab);
            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nAccessories", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nBottles and Cages", properties[7].Text);
        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class ObjectEditPageTestsIe : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class ObjectEditPageTestsFirefox : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
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

    //[TestClass, Ignore]
    public class ObjectEditPageTestsChrome : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
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