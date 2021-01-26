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
using System.Linq;

namespace NakedFunctions.Selenium.Test.FunctionTests
{

    [TestClass]
    public class DevelopmentStoryTests : GeminiTest
    {
        protected override string BaseUrl => TestConfig.BaseFunctionalUrl;

        #region initialization
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
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
            RecordsDoNotHaveEditButton();
            EnumProperty();
            EnumParam();
            DisplayGuidProperty();
            ParameterChoicesSimple();
            ParameterChoicesDependent();
            ParameterDefaultFunction();
            ValidateSingleParam();
            ValidateMultipleParams();
            DisableFunction();
            HideFunction();
            AutoCompleteFunction();
        }

        //[TestMethod]
        public void RetrieveObjectViaMenuAction()
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
        public void UseOfRandomSeedGenerator()
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
        public void ObjectContributedAction()
        {
            //Tests that an action (side effect free) can be associated with an object
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open");
            WaitForTitle("Mountain Tire Sale");
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
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open&d1=EditQuantities");
            var title = WaitForTitle("Mountain Tire Sale");
            var original = "";
            Assert.AreEqual(original, GetPropertyValue("Max Qty"));
            var newType = "5";
            TypeIntoFieldWithoutClearing("#maxqty1", newType);
            Click(OKButton());
            Reload();
            Assert.AreEqual(newType, GetPropertyValue("Max Qty"));
            OpenObjectActions();
            OpenActionDialog("Edit Quantities");
            TypeIntoFieldWithoutClearing("#maxqty1", original);
            Click(OKButton());
            Reload();
            Assert.AreEqual(original, GetPropertyValue("Max Qty"));
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
            TypeIntoFieldWithoutClearing("input#enddate1", endDate);
            wait.Until(d => OKButton().GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            var now = DateTime.Now.ToString("d MMM yyyy HH:mm:").Substring(0, 16);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Manager's Special");
            var modified = WaitForCssNo("nof-view-property .value", 8).Text;
            Assert.AreEqual(now, modified.Substring(0, 16));
        }

        //[TestMethod]
        public void RecordsDoNotHaveEditButton()
        {
            //Corresponds to #229
            GeminiUrl("object?i1=View&o1=AW.Types.WorkOrder--4717");
            WaitForTitle("Road-650 Red, 58: 10/30/2005 12:00:00 AM");
            WaitForCssNo("nof-action-bar nof-action", 2);
            var edit = WaitForCssNo("nof-action-bar nof-action", 1);
            var title = edit.GetAttribute("value");
            Assert.IsTrue(title is null || title == "");
        }

        //[TestMethod]
        public void EnumProperty()
        {
            //Coresponds to (part of) #232
            GeminiUrl("object?i1=View&o1=AW.Types.SalesOrderHeader--55864");
            WaitForTitle("SO55864");
            string value = GetPropertyValue("Status");
            Assert.AreEqual("Shipped", value);
        }

        //[TestMethod]
        public void EnumParam()
        {
            //Coresponds to (part of) #232
            GeminiUrl("home?m1=Order_MenuFunctions&d1=OrdersByStatus");
            WaitForCssNo("#status1 option", 6);
            Assert.AreEqual("Shipped", WaitForCssNo("#status1 option", 6).Text);
            Assert.AreEqual("Rejected", WaitForCssNo("#status1 option", 5).Text);
            Assert.AreEqual("*", WaitForCssNo("#status1 option", 0).Text);
        }

        //[TestMethod]
        public void DisplayGuidProperty()
        {
            //Corresponds to #236
            GeminiUrl("object?i1=View&o1=AW.Types.SalesTaxRate--1");
            WaitForTitle("Sales Tax Rate for Alberta");
            var guid = GetPropertyValue("Rowguid");
            Assert.AreEqual("683de5dd-521a-47d4-a573-06a3cdb1bc5d", guid);
        }

        //[TestMethod]
        public void ParameterChoicesSimple()
        {
            //Corresponds to #242
            GeminiUrl("object?i1=View&o1=AW.Types.Employee--105");
            WaitForTitle("Kevin Homer");
            var currentStatus = GetPropertyValue("Marital Status");
            Assert.AreEqual("S", currentStatus);
            GeminiUrl("object?i1=View&o1=AW.Types.Employee--105&as1=open&d1=UpdateMaritalStatus");
            var option = "select#maritalstatus1 option";
            Assert.AreEqual("M", WaitForCssNo(option, 2).Text);
            Assert.AreEqual("S", WaitForCssNo(option, 1).Text);
            Assert.AreEqual("*", WaitForCssNo(option, 0).Text);
            SelectDropDownOnField("select#maritalstatus1", 2);
            Click(OKButton());
            wait.Until(dr => GetPropertyValue("Marital Status") == "M");
            GeminiUrl("object?i1=View&o1=AW.Types.Employee--105&as1=open&d1=UpdateMaritalStatus");
            SelectDropDownOnField("select#maritalstatus1", 1);
            Click(OKButton());
            wait.Until(dr => GetPropertyValue("Marital Status") == "S");
        }

       // [TestMethod]
        public void ParameterChoicesDependent()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Address--22691&as1=open&d1=EditStateProvince");
            WaitForTitle("2107 Cardinal...");
            var field1 = WaitForCss("select#countryregion1");
            var field2 = WaitForCss("select#stateprovince1");
            Assert.AreEqual(239, field1.FindElements(By.CssSelector("option")).Count);
            Assert.AreEqual(0, field2.FindElements(By.CssSelector("option")).Count);
            SelectDropDownOnField("select#countryregion1", 36);
            wait.Until(br => field2.FindElements(By.CssSelector("option")).Count > 0);
            var option = "select#stateprovince1 option";
            var yukon = WaitForCssNo(option, 13);
            Assert.AreEqual("Yukon Territory", yukon.Text);
        }

        //[TestMethod]
        public void ParameterDefaultFunction()
        {
            GeminiUrl("home?m1=SpecialOffer_MenuFunctions&d1=CreateNewSpecialOffer");
            WaitForTitle("Home");
            var field = WaitForCss("input#enddate1");
            var oneMonthOn = DateTime.Today.AddMonths(1).ToString("d MMM yyyy");
            Assert.AreEqual(oneMonthOn, field.GetAttribute("value"));
        }

        //[TestMethod]
        public void ValidateSingleParam()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--3&as1=open&d1=EditQuantities");
            WaitForTitle("Volume Discount 15 to 24");
            ClearFieldThenType("input#minqty1", "0");
            ClearFieldThenType("input#maxqty1", "5");
            Click(OKButton());
            var msg = "Must be > 0";
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-edit-parameter .validation")).First().Text == msg);
        }

        //[TestMethod]
        public void ValidateMultipleParams()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--3&as1=open&d1=EditQuantities");
            WaitForTitle("Volume Discount 15 to 24");
            ClearFieldThenType("input#minqty1", "10");
            ClearFieldThenType("input#maxqty1", "5");
            Click(OKButton());
            var msg = "Max Qty cannot be < Min Qty";
            wait.Until(dr => dr.FindElement(By.CssSelector(".co-validation")).Text == msg );
        }

        //[TestMethod]
        public void DisableFunction()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SalesOrderHeader--75084&as1=open");
            WaitForTitle("SO75084");
            var act = WaitForCss("nof-action input[value=\"Add New Detail\"");
            Assert.IsNotNull(act.GetAttribute("disabled"));
            Assert.AreEqual("Can only add to 'In Process' order", act.GetAttribute("title"));
        }

        //[TestMethod]
        public void HideFunction()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SalesOrderHeader--75084&as1=open");
            WaitForTitle("SO75084");
            WaitForCssNo("nof-action input", 3); //To ensure all loaded
            var actions = br.FindElements(By.CssSelector("nof-action input"));
            Assert.IsFalse(actions.Any(act => act.GetAttribute("title") == "Approve Order"));
        }

        //[TestMethod]
        public void AutoCompleteFunction()
        {
            GeminiUrl("home?m1=Employee_MenuFunctions&d1=CreateNewEmployeeFromContact");
            WaitForTitle("Home");
            TypeIntoFieldWithoutClearing("input#contactdetails1", "ab");
            Assert.AreEqual("Sam Abolrous", WaitForCssNo("nof-auto-complete .suggestions li", 4).Text);
            Assert.AreEqual("Kim Abercrombie", WaitForCssNo("nof-auto-complete .suggestions li", 0).Text);
        }

    }
}