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
            TypeIntoField("#listprice1",Keys.Backspace + Keys.Backspace + Keys.Backspace + "100");
            TypeIntoField("#daystomanufacture1",Keys.Backspace + "1");
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
                TypeIntoField("#sellstartdate1", Keys.Backspace);
            }

            TypeIntoField("#sellstartdate1", dateStr + Keys.Tab);
            TypeIntoField("#daystomanufacture1", Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Sell Start Date:\r\n" + dateStr, properties[18].Text);
            Assert.AreEqual("Days To Manufacture:\r\n1", properties[17].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeChoices() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();

            // set product line 

            SelectDropDownOnField("#productline1", "S");

            TypeIntoField("#daystomanufacture1", Keys.Backspace + "1");
            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Line:\r\nS", properties[8].Text);
        }

        [TestMethod]
        public virtual void ObjectEditChangeConditionalChoices() {
            GeminiUrl( "object?object1=AdventureWorksModel.Product-870");
            EditObject();
            // set product category and sub category
            SelectDropDownOnField("#productcategory1", "Clothing");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 9);

            SelectDropDownOnField("#productsubcategory1", "Caps");

            TypeIntoField("#daystomanufacture1", Keys.Backspace + "1");

            SaveObject();

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nClothing", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nCaps", properties[7].Text);

            EditObject();

            // set product category and sub category

            var slctd = new SelectElement(br.FindElement(By.CssSelector("select#productcategory1")));

            Assert.AreEqual("Clothing", slctd.SelectedOption.Text);

            Assert.AreEqual(5, br.FindElements(By.CssSelector("select#productcategory1 option")).Count);

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 9);

            Assert.AreEqual(9, br.FindElements(By.CssSelector("select#productsubcategory1 option")).Count);

            SelectDropDownOnField("#productcategory1","Bikes");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 4);

            SelectDropDownOnField("#productsubcategory1","Mountain Bikes");

            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nBikes", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nMountain Bikes", properties[7].Text);

            // set values back
            EditObject();

            SelectDropDownOnField("#productcategory1", "Accessories");

            var slpsc = new SelectElement(br.FindElement(By.CssSelector("select#productsubcategory1")));
            wait.Until(d => slpsc.Options.Count == 13);

            SelectDropDownOnField("#productsubcategory1", "Bottles and Cages");
            SaveObject();

            properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Product Category:\r\nAccessories", properties[6].Text);
            Assert.AreEqual("Product Subcategory:\r\nBottles and Cages", properties[7].Text);
        }

        [TestMethod] 
        public void ObjectEditPicksUpLatestServerVersion()
        {

            GeminiUrl("object?object1=AdventureWorksModel.Person-8410&actions1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(6) .value").Text;
            var dialog = OpenActionDialog("Update Suffix"); //This is deliberately wrongly marked up as QueryOnly
            var field1 = WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            TypeIntoField(".parameter:nth-child(1) input", newValue);
            Click(OKButton()); //This will have updated server, but not client-cached object
            WaitUntilElementDoesNotExist(".dialog");
            //Check view has not updated because it was a queryonly action
            Assert.AreEqual(original, WaitForCss(".property:nth-child(6) .value").Text);
            EditObject(); //This will update object from server
            Click(GetCancelEditButton()); //but can't read the value, so go back to view
            Assert.AreEqual(newValue, WaitForCss(".property:nth-child(6) .value").Text);
        }

        [TestMethod]
        public void CoValidationOnSavingChanges()
        {
            GeminiUrl("object?object1=AdventureWorksModel.WorkOrder-43134&edit1=true");
            WaitForView(Pane.Single, PaneType.Object);
            var deleteDate = Repeat(Keys.Backspace, 11);
            TypeIntoField("input#startdate1", deleteDate +"17 Oct 2007");
            TypeIntoField("input#enddate1", deleteDate + "15 Oct 2007");
            Click(SaveButton());
            WaitForMessage("StartDate must be before EndDate");
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