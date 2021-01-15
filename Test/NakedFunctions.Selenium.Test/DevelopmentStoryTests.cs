// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;
using OpenQA.Selenium;
using System;
using System.Threading;

namespace NakedFunctions.Selenium.Test.FunctionTests {

    [TestClass] 
    public class DevelopmentStoryTests : GeminiTest
    {
        protected override string BaseUrl => TestConfig.BaseFunctionalUrl;

        #region initialization
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
        #endregion

        [TestMethod]
        public void AllWorkingStories()
        {
            RetrieveObjectViaMenuAction();
            UseOfRandomSeedGenerator();
            ObjectContributedAction();
            InformUserViaIAlertService();
            EditAction();
            AccessToIClock();
            SaveNewInstance();
        }

        //[TestMethod]
        public  void RetrieveObjectViaMenuAction()
        {
            //Corresponds to Story #199. Tests that IContext is injected as param, and that its Instances<T> method works
            Home(); 
            OpenMainMenuAction("Products", "Find Product By Name");
            ClearFieldThenType("#searchstring1", "handlebar tube");
            Click(OKButton());         
            WaitForView(Pane.Single, PaneType.List, "Find Product By Name");
            AssertTopItemInListIs("Handlebar Tube");
        }

        //[TestMethod]
        public  void UseOfRandomSeedGenerator()
        {
            //Corresponds to Story #200. Tests that IContext provides access to IRandomSeedGenerator & that the latter works
            Home(); 
            OpenMainMenuAction("Products", "Random Product");
            WaitForView(Pane.Single, PaneType.Object);
            Assert.IsTrue(br.Url.Contains(".Product-"));
            string product1Url = br.Url;           
            OpenMainMenuAction("Products", "Random Product");
            WaitForView(Pane.Single, PaneType.Object);
            Assert.IsTrue(br.Url.Contains(".Product-"));
            Assert.AreNotEqual(product1Url, br.Url);
        }

        //[TestMethod]
        public  void ObjectContributedAction()
        {
            //Tests that an action (side effect free) can be associated with an object
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open");
            WaitForTitle( "Mountain Tire Sale");
            var action = GetObjectAction("List Associated Products");
            RightClick(action);
            WaitForView(Pane.Right, PaneType.List);
            var product = WaitForCss("td label[for=\"item2-0\"]");
            Assert.AreEqual("LL Mountain Tire", product.Text);
        }

        //[TestMethod]
        public void InformUserViaIAlertService()
        {
            //Corresponds to Story #201
            GeminiUrl("object/object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open&d1=AssociateWithProduct&i2=View&o2=AW.Types.Product--928");
            var title = WaitForCss("#pane2 .header .title");
            Assert.AreEqual("LL Mountain Tire", title.Text);
            title.Click();
            CopyToClipboard(title);
            PasteIntoInputField("#pane1 .parameter .value.droppable");
            Click(OKButton());
            wait.Until(d => d.FindElement(By.CssSelector(".co-validation")).Text != "");
            var msg = WaitForCss(".co-validation").Text;
            Assert.AreEqual("Mountain Tire Sale is already associated with LL Mountain Tire", msg);
        }

        //[TestMethod]
        public void EditAction()
        {
            //Corresponds to Story #202
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--6&as1=open&d1=EditDescription");
            string original = "Volume Discount over 60";
            var title =WaitForTitle( original);
            string newDesc = "Volume Discount 60+";
            TypeIntoFieldWithoutClearing("#description1", newDesc);
            Click(OKButton());
            wait.Until(d => d.FindElement(By.CssSelector(".title")).Text == newDesc);
            OpenObjectActions();
            OpenActionDialog("Edit Description");
            TypeIntoFieldWithoutClearing("#description1", original);
            Click(OKButton());
            wait.Until(d => d.FindElement(By.CssSelector(".title")).Text == original);
        }

        //[TestMethod] 
        public void AccessToIClock()
        {
            //Corresponds to #203
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--1&as1=open&d1=EditDates");
            var endDate = WaitForCss("input#enddate1");
            var oneMonthOn = DateTime.Today.AddMonths(1).ToString("d MMM yyyy");
            Assert.AreEqual(oneMonthOn, endDate.GetAttribute("value"));
        }

        //[TestMethod]
        public void SaveNewInstance()
        {
            //Corresponds to #204
            GeminiUrl("home?m1=SpecialOffer_MenuFunctions&d1=CreateNewSpecialOffer");
            TypeIntoFieldWithoutClearing("input#description1", "Manager's Special");
            TypeIntoFieldWithoutClearing("input#discountpct1", "15");
            var endDate = DateTime.Today.AddDays(7).ToString("d MMM yyyy");
            TypeIntoFieldWithoutClearing("input#enddate1",endDate);
            wait.Until(d => OKButton().GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            var now = DateTime.Now.ToString("d MMM yyyy HH:mm:").Substring(0,16);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Manager's Special");
            var modified = WaitForCssNo("nof-view-property .value", 8).Text;
            Assert.AreEqual(now, modified.Substring(0, 16));
        }
    }
}