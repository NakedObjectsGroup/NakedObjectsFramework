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

namespace NakedObjects.Web.UnitTests.Selenium
{

    /// <summary>
    /// Tests for collection-contributedActions
    /// </summary>
    public abstract class CCAtestsRoot : AWTest
    {

        public virtual void ListViewWithParmDialogAlreadyOpen()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&d1=ChangeMaxQuantity&f1_newMax=%22%22");
            WaitForView(Pane.Single, PaneType.List);
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
            string maxQty = "Max Qty:";
            CheckIndividualItem(5, maxQty, newMax);
            CheckIndividualItem(7, maxQty, newMax);
            CheckIndividualItem(9, maxQty, newMax);
            //Confirm others have not
            CheckIndividualItem(6, maxQty, newMax, false);
            CheckIndividualItem(8, maxQty, newMax, false);
        }

        public virtual void ListViewWithParmDialogNotOpen()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("tbody tr .checkbox")).Count >= 16);
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Max Quantity");
            var rand = new Random();
            var newMax = rand.Next(10, 10000).ToString();
            TypeIntoFieldWithoutClearing("#newmax1", newMax);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            string maxQty = "Max Qty:";
            CheckIndividualItem(2, maxQty, newMax);
            CheckIndividualItem(3, maxQty,newMax);
            CheckIndividualItem(4, maxQty,newMax);
            //Confirm others have not
            CheckIndividualItem(1, maxQty,newMax, false);
            CheckIndividualItem(5, maxQty,newMax, false);
        }
        private void CheckIndividualItem(int itemNo, string label, string value, bool equal = true)
        {
            GeminiUrl("object?o1=___1.SpecialOffer-" + (itemNo + 1));
            wait.Until(dr => dr.FindElements(By.CssSelector(".property")).Count == 9);
            var properties = br.FindElements(By.CssSelector(".property"));
            var html = label +"\r\n" + value;
            var prop = properties.First(p => p.Text.StartsWith(label));
            if (equal)
            {
                Assert.AreEqual(html, prop.Text);
            }
            else
            {
                Assert.AreNotEqual(html, prop.Text);
            }
        }

        public virtual void TableViewWithParmDialogAlreadyOpen()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&d1=ChangeDiscount&f1_newDiscount=%22%22&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            var rand = new Random();
            var newPct = rand.NextDouble().ToString().Substring(0, 4);
            TypeIntoFieldWithoutClearing("#newdiscount1", newPct);
            //Now select items
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-8");
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
            //Check that exactly two rows were updated
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(4)")).Count(el => el.Text == newPct) == 2);
        }

        public virtual void TableViewWithParmDialogNotOpen()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1=0&as1=open&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            SelectCheckBox("#item1-4");
            OpenActionDialog("Change Discount");
            var rand = new Random();
            var newPct = rand.NextDouble().ToString().Substring(0, 4);
            TypeIntoFieldWithoutClearing("#newdiscount1", newPct);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();
            //Check that exactly three rows were updated

            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(4)")).Count(el => el.Text == newPct) == 3);
        }

        public virtual void DateParam()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1=0&as1=open&c1=Table");
            WaitForView(Pane.Single, PaneType.List);
            Reload();
            SelectCheckBox("#item1-6");
            SelectCheckBox("#item1-9");
            OpenActionDialog("Extend Offers");
            var rand = new Random();
            var futureDate = DateTime.Today.AddDays(rand.Next(1000)).ToString("dd MMM yyyy");
            if (futureDate.StartsWith("0"))
            {
                futureDate = futureDate.Substring(1);
            }
            ClearFieldThenType("#todate1", futureDate + Keys.Escape);
            Click(OKButton());
            WaitUntilElementDoesNotExist(".dialog");
            Reload();

            //Check that exactly two rows were updated
            string endDate = "End Date:";
            CheckIndividualItem(6, endDate, futureDate);
            CheckIndividualItem(9, endDate, futureDate);
            CheckIndividualItem(7, endDate, futureDate, false);
            CheckIndividualItem(8, endDate, futureDate, false);
        }

        public virtual void ZeroParamAction()
        {
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders&pg1=20&ps1=5&s1=0&as1=open&c1=Table");
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td")).Count >30);
            SelectCheckBox("#item1-1");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            Click(GetObjectAction("Comment As Users Unhappy"));
            Thread.Sleep(1000); //Because there is no visible change to wait for
            Reload();
            wait.Until( dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text == "User unhappy") ==3);
            SelectCheckBox("#item1-1");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            Click(GetObjectAction("Clear Comments"));
            Thread.Sleep(1000);
            Reload();
            wait.Until(dr => dr.FindElements(By.CssSelector("td:nth-child(7)")).Count(el => el.Text == "User unhappy") == 0);
        }

    }

    public abstract class CCAtests : CCAtestsRoot
    {

        [TestMethod]
        public override void ListViewWithParmDialogAlreadyOpen() { base.ListViewWithParmDialogAlreadyOpen(); }
        [TestMethod]
        public override void ListViewWithParmDialogNotOpen() { base.ListViewWithParmDialogNotOpen(); }

        [TestMethod]
        public override void TableViewWithParmDialogAlreadyOpen() { base.TableViewWithParmDialogAlreadyOpen(); }

        [TestMethod]
        public override void TableViewWithParmDialogNotOpen() { base.TableViewWithParmDialogNotOpen(); }

        [TestMethod]
        public override void DateParam() { base.DateParam(); }

        [TestMethod]
        public override void ZeroParamAction() { base.ZeroParamAction(); }
    }

    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class CCAtestsIe : CCAtests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class CCAtestsFirefox : CCAtests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass, Ignore]
    public class CCAtestsChrome : CCAtests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion

    #region Mega tests
    public abstract class MegaCCATestsRoot : CCAtestsRoot
    {
        [TestMethod]
        public void MegaCCATest()
        {
            base.ListViewWithParmDialogAlreadyOpen();
            base.ListViewWithParmDialogNotOpen();
            base.TableViewWithParmDialogAlreadyOpen();
            base.TableViewWithParmDialogNotOpen();
            base.DateParam();

        }
    }

    //[TestClass]
    public class MegaCCATestsFirefox : MegaCCATestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }
    #endregion
}