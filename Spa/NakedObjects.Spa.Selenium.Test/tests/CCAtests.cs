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
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests for collection-contributedActions
    /// </summary>
    public abstract class CCAtests : AWTest {

        [TestMethod]
        public void ListViewWithParmDialogAlreadyOpen()
        {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&d1=ChangeMaxQuantity&f1_newMax=%22%22");
            Reload();
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);

            //Now select items
            SelectCheckBox("#item1-5");
            SelectCheckBox("#item1-7");
            SelectCheckBox("#item1-9");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
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
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open");
            Reload();
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Max Quantity");
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            CheckIndividualItem(2, newMax);
            CheckIndividualItem(3, newMax);
            CheckIndividualItem(4, newMax);
            //Confirm others have not
            CheckIndividualItem(1, newMax, false);
            CheckIndividualItem(5, newMax, false);
        }
        private void CheckIndividualItem(int itemNo, string value, bool equal = true)
        {
            GeminiUrl("object?o1=___1.SpecialOffer-"+(itemNo+1));
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
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&d1=ChangeMaxQuantity&f1_newMax=%22%22&c1=Table");
            Reload();
            var rand = new Random();
            var newMax = rand.Next(1000, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            //Now select items
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-8");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
            //Check that exactly two rows were updated
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(9)")).Count(el => el.Text == newMax) == 2);
        }

        [TestMethod]
        public void TableViewWithParmDialogNotOpen()
        {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&c1=Table");
            Reload();
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Max Quantity");
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
            //Check that exactly three rows were updated
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(9)")).Count(el => el.Text == newMax)==3);
        }

        [TestMethod, Ignore] //Pending fix to date issues (date recorded is one day off date entered)
        public void DateParamAndZeroParam()
        {
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&c1=Table");
            Reload();
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-9");
            OpenActionDialog("Extend Offers");
            var rand = new Random();
            var futureDate = DateTime.Today.AddDays(rand.Next(1000)).ToString("dd MMM yyyy");
            ClearFieldThenType("#todate1", futureDate + Keys.Escape);
            Thread.Sleep(1000); // need to wait for datepicker :-(
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
            //Check that exactly two rows were updated
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text == futureDate) == 2);

            //Check that 6 & 9 still checked
            //SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-7"); // Not currently active so should not update
            //SelectCheckBox("#item1-9");
            Click(GetObjectAction("Terminate Active Offers"));
            Reload();
            //Check that exactly three rows were updated
            var today = DateTime.Today.ToString("d");
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text == today) == 2);

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