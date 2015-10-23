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

    public abstract class ObjectEditTests : AWTest {

        [TestMethod]
        public virtual void ObjectEditChangeScalar() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();

            // set price and days to mfctr
            TypeIntoField("#listprice",Keys.Backspace + Keys.Backspace + Keys.Backspace + "100");
            TypeIntoField("#daystomanufacture",Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("List Price:\r\n4100", properties[5].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeDateTime() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();

            // set price and days to mfctr
            var date = new DateTime(2014, 7, 18, 0, 0, 0, DateTimeKind.Utc);
            var dateStr = date.ToString("d MMM yyyy");

            for (int i = 0; i < 12; i++) {
                TypeIntoField("#sellstartdate", Keys.Backspace);
            }

            TypeIntoField("#sellstartdate", dateStr + Keys.Tab);
            TypeIntoField("#daystomanufacture", Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Sell Start Date:\r\n" + dateStr, properties[18].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod, Ignore] //The dependent selector is not changing in the demo
        public virtual void ObjectEditChangeChoices() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();

            // set product line 

            SelectDropDownOnField("#productline", "S");

            TypeIntoField("#daystomanufacture", Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeConditionalChoices() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();
            // set product category and sub category

            var selected = new SelectElement(br.FindElement(By.CssSelector("select#productcategory")));

            // this makes tests really fragile
            //Assert.AreEqual("Accessories", selected.SelectedOption.Text);

            //Assert.AreEqual(4, br.FindElements(By.CssSelector("#productcategory  select option")).Count);
            //Assert.AreEqual(13, br.FindElements(By.CssSelector("#productsubcategory  select option")).Count);

            SelectDropDownOnField("#productcategory", "Clothing" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory option")).Count == 9);

            SelectDropDownOnField("#productsubcategory", "Caps");

            TypeIntoField("#daystomanufacture", Keys.Backspace + "1");

            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nCaps", properties[7].Text);

            EditObject();

            // set product category and sub category

            var slctd = new SelectElement(br.FindElement(By.CssSelector("select#productcategory")));

            Assert.AreEqual("Clothing", slctd.SelectedOption.Text);

            Assert.AreEqual(5, br.FindElements(By.CssSelector("select#productcategory option")).Count);

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory option")).Count == 9);

            Assert.AreEqual(9, br.FindElements(By.CssSelector("select#productsubcategory option")).Count);

            SelectDropDownOnField("#productcategory","Bikes" + Keys.Tab);

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory option")).Count == 4);

            SelectDropDownOnField("#productsubcategory","Mountain Bikes");

            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);

            // set values back
            EditObject();

            SelectDropDownOnField("#productcategory", "Accessories" + Keys.Tab);

            var slpsc = new SelectElement(br.FindElement(By.CssSelector("select#productsubcategory")));
            wait.Until(d => slpsc.Options.Count == 13);

            SelectDropDownOnField("#productsubcategory", "Bottles and Cages" + Keys.Tab);
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
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class ObjectEditPageTestsFirefox : ObjectEditTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
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
            AWTest.InitialiseClass(context);
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