// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests for collection-contributedActions
    /// </summary>
    public abstract class CCAtests : AWTest {

        [TestMethod]
        public void ListViewWithParmDialogAlreadyOpen()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open&dialog1=ChangeMaxQuantity&field1_newMax=%2522%2522");
            Reload();
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);

            //Now select items
            SelectCheckBox("#item1-5");
            SelectCheckBox("#item1-7");
            SelectCheckBox("#item1-9");
            Click(OKButton());
            CheckIndividualItem(5, newMax);
            CheckIndividualItem(7, newMax);
            CheckIndividualItem(9, newMax);
            //Confirm others have not
            CheckIndividualItem(6, newMax, false);
            CheckIndividualItem(8, newMax, false);
        }

        [TestMethod]
        public void ListViewWithParmDialogNotOpen()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open");
            Reload();
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Max Quantity");
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            Click(OKButton());
            CheckIndividualItem(2, newMax);
            CheckIndividualItem(3, newMax);
            CheckIndividualItem(4, newMax);
            //Confirm others have not
            CheckIndividualItem(1, newMax, false);
            CheckIndividualItem(5, newMax, false);
        }
        private void CheckIndividualItem(int itemNo, string value, bool equal = true)
        {
            GeminiUrl("object?object1=AdventureWorksModel.SpecialOffer-"+(itemNo+1));
            wait.Until(dr => dr.FindElements(By.CssSelector(".property")).Count == 9);
            var properties = br.FindElements(By.CssSelector(".property"));
            var html = "Max Qty:\r\n" + value;
            if (equal)
            {
                Assert.AreEqual(html, properties[7].Text);
            } else
            {
                Assert.AreNotEqual(html, properties[7].Text);
            }
        }

        [TestMethod]
        public void TableViewWithParmDialogAlreadyOpen()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open&dialog1=ChangeMaxQuantity&field1_newMax=%2522%2522&collection1=Table");
            Reload();
            var rand = new Random();
            var newMax = rand.Next(1000, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            //Now select items
            SelectCheckBox("#item1-5");
            SelectCheckBox("#item1-7");
            SelectCheckBox("#item1-9");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            //Now check individual items
            CheckIndividualItem(5, newMax);
            CheckIndividualItem(7, newMax);
            CheckIndividualItem(9, newMax);
            //Confirm others have not
            CheckIndividualItem(6, newMax, false);
            CheckIndividualItem(8, newMax, false);
        }

        [TestMethod]
        public void TableViewWithParmDialogNotOpen()
        {
            GeminiUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0&actions1=open&collection1=Table");
            Reload();
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Max Quantity");
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            Click(OKButton());
            CheckIndividualItem(2, newMax);
            CheckIndividualItem(3, newMax);
            CheckIndividualItem(4, newMax);
            //Confirm others have not
            CheckIndividualItem(1, newMax, false);
            CheckIndividualItem(5, newMax, false);
        }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class CCAtestsIe : CCAtests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class CCAtestsFirefox : CCAtests
    {
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
    }

    //[TestClass, Ignore]
    public class CCAtestsChrome : CCAtests
    {
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

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}