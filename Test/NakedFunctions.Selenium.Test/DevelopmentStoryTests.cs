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
using System.Threading;

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
            ObjectActionThatReturnsJustAContext();
            OverriddenPrincipalProviderService();
            UseOfRandomSeedGenerator();
            ObjectContributedAction();
            InformUserViaIAlertService();
            EditAction();
            AccessToIClock();
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
            ViewModel1();
            MultiLineActionDialog();
            CreateNewObectWithOnlyValueProperties();
            CreateNewObjectWithAReferenceToAnotherExistingObject();
            CreateNewObjectWithAReferenceToMultipleExistingObjects();
            CreateAGraphOfTwoNewRelatedObjects();
            CreateAGraphOfObjectsThreeLevelsDeep();
            PropertyHiddenViaAHideMethod();
            SubMenuOnObject();
            SubMenuOnMainMenu();
            ImageProperty();
            ImageParameter();
            QueryContributedActionReturningOnlyAContext();
            LocalCollectionContributedAction();
            SaveNewChildObjectAndTestItsVisibilityInTheParentsCollection();
            UseOfDeferredFunctionIncludingReload();
            UseOfResolveMethodInADeferredFunction();
            //QueryContributedActionWithCoValidation();
            //QueryContributedActionWithChoicesFunction();

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
        public void ObjectActionThatReturnsJustAContext()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--5&as1=open&d1=EditDescription");
            var original = "Volume Discount 41 to 60";
            WaitForTitle(original);
            var newDesc = "Volume Discount 41+";
            TypeIntoFieldWithoutClearing("#description1", newDesc);
            Click(OKButton());
            WaitForTitle(newDesc);
            OpenActionDialog("Edit Description");
            TypeIntoFieldWithoutClearing("#description1", original);
            Click(OKButton());
            WaitForTitle(original);
        }

        //[TestMethod]
        public void OverriddenPrincipalProviderService()
        {
            GeminiUrl("home");
            WaitForTitle("Home");
            OpenMainMenuAction("Employees", "Me");
            WaitForView(Pane.Single, PaneType.Object);
            Assert.AreEqual("Ken Sánchez", WaitForCss(".title").Text);
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
            wait.Until(d => d.FindElement(By.CssSelector(".footer .messages")).Text != "");
            var msg = WaitForCss(".footer .messages").Text;
            Assert.AreEqual("Mountain Tire Sale is already associated with LL Mountain Tire", msg);
        }

        //[TestMethod]
        public void EditAction()
        {
            //Corresponds to Story #202
            GeminiUrl("object?i1=View&o1=AW.Types.SpecialOffer--9&as1=open&d1=EditQuantities");
            var title = WaitForTitle("Road-650 Overstock");
            var original = GetPropertyValue("Max Qty");
            var newQty = original+"1";
            TypeIntoFieldWithoutClearing("#maxqty1", newQty);
            Click(OKButton());
            Thread.Sleep(1000);
            Assert.AreEqual(newQty, GetPropertyValue("Max Qty"));
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
            wait.Until(dr => dr.FindElement(By.CssSelector(".co-validation")).Text == msg);
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
            GeminiUrl("home?m1=WorkOrder_MenuFunctions&d1=ListWorkOrders");
            WaitForTitle("Home");
            TypeIntoFieldWithoutClearing("input#product1", "fr");
            Assert.AreEqual("Front Derailleur Linkage", WaitForCssNo("nof-auto-complete .suggestions li", 4).Text);
            Assert.AreEqual("Freewheel", WaitForCssNo("nof-auto-complete .suggestions li", 0).Text);
        }

        //[TestMethod]
        public void ViewModel1()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.StaffSummary--84--206");
            WaitForTitle("Staff Summary");
            Assert.AreEqual("84", GetPropertyValue("Female"));
            Assert.AreEqual("206", GetPropertyValue("Male"));
            Assert.AreEqual("290", GetPropertyValue("Total Staff"));
            Reload();
            Thread.Sleep(1000);
            Assert.AreEqual("84", GetPropertyValue("Female"));
            Assert.AreEqual("206", GetPropertyValue("Male"));
            Assert.AreEqual("290", GetPropertyValue("Total Staff"));
        }

        //[TestMethod]
        public void MultiLineActionDialog()
        {
            GeminiUrl("multiLineDialog?m1=SpecialOffer_MenuFunctions&d1=CreateMultipleSpecialOffers");
            WaitForTitle("Create Multiple Special Offers");
            WaitForCssNo(".lineDialog", 1); //i.e. 2 dialogs
            var ok1 = WaitForCssNo("input.ok", 1);
            var val1 = WaitForCssNo(".co-validation", 1);
            var ok0 = WaitForCssNo("input.ok", 0);
            var val0 = WaitForCssNo(".co-validation", 0);
            TypeIntoFieldWithoutClearing("input#description0", "Manager's Special");
            TypeIntoFieldWithoutClearing("input#discountpct0", "15");
            var endDate = DateTime.Today.AddDays(7).ToString("d MMM yyyy");
            TypeIntoFieldWithoutClearing("input#enddate0", endDate);
            wait.Until(d => ok0.GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            Assert.AreEqual("", val0.Text);
            Click(ok0);
            wait.Until(br => br.FindElements(By.CssSelector(".co-validation")).First().Text == "Submitted");
            //Second line
            TypeIntoFieldWithoutClearing("input#description1", "Manager's Special II");
            TypeIntoFieldWithoutClearing("input#discountpct1", "12.5");
            TypeIntoFieldWithoutClearing("input#enddate1", endDate);
            wait.Until(d => ok1.GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            Assert.AreEqual("", val1.Text);
            Click(ok1);
            wait.Until(br => br.FindElements(By.CssSelector(".co-validation")).ElementAt(1).Text == "Submitted");


            //Check third line has now appeared
            WaitForCssNo(".lineDialog", 2);

        }

        //[TestMethod]
        public void CreateNewObectWithOnlyValueProperties()
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
        public void CreateNewObjectWithAReferenceToAnotherExistingObject()
        {
            GeminiUrl("home?m1=Address_MenuFunctions&d1=CreateNewAddress");
            SelectDropDownOnField("select#type1", 2);
            TypeIntoFieldWithoutClearing("#line11", "Dunroamin");
            TypeIntoFieldWithoutClearing("#line21", "Elm St");
            TypeIntoFieldWithoutClearing("#city1", "Concord");
            var zip = (new Random()).Next(10000).ToString();
            TypeIntoFieldWithoutClearing("#postcode1", zip);
            SelectDropDownOnField("select#sp1", 1);
            Click(OKButton());
            WaitForTitle("Dunroamin...");
            Assert.AreEqual("Dunroamin", GetPropertyValue("Address Line1"));
            Assert.AreEqual("Alberta", GetReferenceFromProperty("State Province").Text);
        }

        //[TestMethod]
        public void CreateNewObjectWithAReferenceToMultipleExistingObjects()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Customer--12211&as1=open");
            WaitForTitle("AW00012211 Victor Romero");
            Click(GetObjectAction("Create Another Order"));
            WaitForView(Pane.Single, PaneType.Object);
            var num = GetPropertyValue("Sales Order Number");
            Assert.IsTrue(num.StartsWith("SO75"));
            Assert.AreEqual("AW00012211 Victor Romero", GetReferenceFromProperty("Customer").Text);
        }

        //[TestMethod]
        public void CreateAGraphOfTwoNewRelatedObjects()
        {
            GeminiUrl("home?m1=Customer_MenuFunctions&d1=CreateNewStoreCustomer");
            WaitForTitle("Home");
            int rand = (new Random()).Next(1970, 2021);
            var name = $"FooBar Frames {rand} Ltd";
            TypeIntoFieldWithoutClearing("#name1", name);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object);
            Assert.IsTrue(WaitForCss(".title").Text.EndsWith(name));
            var store = GetReferenceFromProperty("Store");
            Assert.IsTrue(store.Text.EndsWith(name));
        }

        //[TestMethod]
        public void CreateAGraphOfObjectsThreeLevelsDeep()
        {
            //This story involves creation of a graph of three new objects (Customer, Person, Password)
            //with two levels of dependency
            GeminiUrl("home?m1=Customer_MenuFunctions&d1=CreateNewIndividualCustomer");
            WaitForTitle("Home");
            TypeIntoFieldWithoutClearing("#firstname1", "Fred");
            TypeIntoFieldWithoutClearing("#lastname1", "Bloggs");
            TypeIntoFieldWithoutClearing("#password1", "foobar");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object);
            var title = WaitForCss(".title").Text;
            Assert.IsTrue(title.EndsWith("Fred Bloggs"));
            var p = GetReferenceFromProperty("Person");
            Click(p);
            var pw = GetReferenceFromProperty("Password");
            Assert.AreEqual("Password", pw.Text);
        }


        //[TestMethod]
        public void PropertyHiddenViaAHideMethod()
        {
            //An Individual Customer should not display a Store property
            GeminiUrl("object?i1=View&o1=AW.Types.Customer--29207");
            WaitForTitle("AW00029207 Katelyn James");
            Assert.AreEqual("Sales Territory:", WaitForCssNo(".property .name", 3).Text);
            Assert.AreEqual("Person:", WaitForCssNo(".property .name", 2).Text);
            Assert.AreEqual("Customer Type:", WaitForCssNo(".property .name", 1).Text);
            Assert.AreEqual("Account Number:", WaitForCssNo(".property .name", 0).Text);

            //An Individual Customer should not display a Store property
            GeminiUrl("object?i1=View&o1=AW.Types.Customer--29553");
            WaitForTitle("AW00029553 Unsurpassed Bikes");
            Assert.AreEqual("Sales Territory:", WaitForCssNo(".property .name", 3).Text);
            Assert.AreEqual("Store:", WaitForCssNo(".property .name", 2).Text);
            Assert.AreEqual("Customer Type:", WaitForCssNo(".property .name", 1).Text);
            Assert.AreEqual("Account Number:", WaitForCssNo(".property .name", 0).Text);
        }

        //[TestMethod]
        public void SubMenuOnObject()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Vendor--1500&as1=open");
            WaitForTitle("Morgan Bike Accessories");
            Assert.AreEqual("Check Credit", WaitForCssNo("nof-action-list input", 1).GetAttribute("value"));
            Assert.AreEqual("Show All Products", WaitForCssNo("nof-action-list input", 0).GetAttribute("value"));
            OpenSubMenu("Purchase Orders");
            WaitForCssNo("nof-action", 4); //i.e. so we are sure menu has opened
            Assert.AreEqual("Create New Purchase Order", WaitForCssNo("nof-action-list input", 0).GetAttribute("value"));
            Assert.AreEqual("Open Purchase Orders", WaitForCssNo("nof-action-list input", 1).GetAttribute("value"));
            Assert.AreEqual("List Purchase Orders", WaitForCssNo("nof-action-list input", 2).GetAttribute("value"));

        }

        //[TestMethod]
        public void SubMenuOnMainMenu()
        {
            GeminiUrl("home?m1=Customer_MenuFunctions");
            WaitForTitle("Home");
            WaitForCssNo("nof-action", 1);
            Assert.AreEqual("Find Customer By Account Number", WaitForCssNo("nof-action-list input", 0).GetAttribute("value"));
            Assert.AreEqual("List Customers For Sales Territory", WaitForCssNo("nof-action-list input", 1).GetAttribute("value"));
            OpenSubMenu("Individuals");
            OpenSubMenu("Stores");
            WaitForCssNo("nof-action", 7); //i.e. so we are sure menu has opened
            Assert.AreEqual("Find Individual Customer By Name", WaitForCssNo("nof-action-list input", 0).GetAttribute("value"));
            Assert.AreEqual("Find Store By Name", WaitForCssNo("nof-action-list input", 4).GetAttribute("value"));
        }

        //[TestMethod]
        public void ImageProperty()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Product--881");
            WaitForTitle("Short-Sleeve Classic Jersey, S");
            var photo = GetProperty("Photo");
            var imgSrc = photo.FindElement(By.CssSelector("img")).GetAttribute("src");
            Assert.IsTrue(imgSrc.StartsWith("data:image/gif;"));
        }

        //[TestMethod]
        public void ImageParameter()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.Product--881&as1=open");
            WaitForTitle("Short-Sleeve Classic Jersey, S");
            OpenActionDialog("Add Or Change Photo");
            WaitForCss(".value.input-control input#newimage1");
        }

        //[TestMethod]
        public void QueryContributedActionReturningOnlyAContext()
        {
            GeminiUrl("list?m1=SpecialOffer_MenuFunctions&a1=AllSpecialOffers&pg1=1&ps1=20&s1_=0&c1=Table&as1=open&d1=ExtendOffers");
            WaitForTitle("All Special Offers");
            Reload();
            WaitForCssNo("tbody tr", 10);
            int rand = (new Random()).Next(1000);
            var endDate = DateTime.Today.AddDays(rand).ToString("dd MMM yyyy");
            Assert.IsFalse(br.FindElements(By.CssSelector("tbody tr td")).Any(el => el.Text == endDate));
            TypeIntoFieldWithoutClearing("#todate1", endDate);
            SelectCheckBox("#item1-1");
            SelectCheckBox("#item1-2");
            SelectCheckBox("#item1-3");
            Click(OKButton());
            Thread.Sleep(1000);
            Reload();
            WaitForCssNo("tbody tr", 10);
            wait.Until(br => br.FindElements(By.CssSelector("tbody tr td")).Count(el => el.Text == endDate) >= 3);
        }

        //[TestMethod]
        public void LocalCollectionContributedAction()
        {
            GeminiUrl("object?i1=View&o1=AW.Types.SalesOrderHeader--53535&c1_Details=List&d1=AddCarrierTrackingNumber");
            WaitForTitle("SO53535");
            var rnd = (new Random()).Next(100000).ToString();
            SelectCheckBox("#details1-1");
            SelectCheckBox("#details1-2");
            SelectCheckBox("#details1-3");
            TypeIntoFieldWithoutClearing("#ctn1", rnd);
            Click(OKButton());
            Thread.Sleep(2000); //Otherwise next line does not open the table??!
            Click(WaitForCss(".icon.table"));
            WaitForCssNo("tbody tr", 10);
            Assert.AreEqual(3, br.FindElements(By.CssSelector("tbody tr td")).Count(el => el.Text == rnd));
        }

       // [TestMethod]
        public void SaveNewChildObjectAndTestItsVisibilityInTheParentsCollection()
        {
            GeminiUrl("object/object?i1=View&o1=AW.Types.Customer--12211&as1=open&i2=View&o2=AW.Types.Product--707");
            WaitForTitle("AW00012211 Victor Romero", Pane.Left);
            Click(GetObjectAction("Create Another Order", Pane.Left));
            WaitForView(Pane.Left, PaneType.Object);
            var num = GetPropertyValue("Sales Order Number", Pane.Left);
            Assert.IsTrue(num.StartsWith("SO75"));
            OpenObjectActions(Pane.Left);
            OpenActionDialog("Add New Detail", Pane.Left);
            var product = WaitForCss("#pane2 .title");
            CopyToClipboard(product);
            PasteIntoInputField("#pane1 .parameter .value.droppable");
            Click(OKButton());
            Click(FullIcon());
            WaitForView(Pane.Single, PaneType.Object);
            Thread.Sleep(1000);
            var listIcon1 = WaitForCssNo(".collection .icon.list", 0);
            Click(listIcon1);
            wait.Until(dr => dr.FindElements(By.CssSelector("tr td")).Any(el => el.Text == "1 x Sport-100 Helmet, Red"));
        }

        //[TestMethod]
        public void UseOfDeferredFunctionIncludingReload()
        {
            GeminiUrl("object/object?i1=View&o1=AW.Types.Customer--12211&as1=open&i2=View&o2=AW.Types.Product--707");
            WaitForTitle("AW00012211 Victor Romero", Pane.Left);
            Click(GetObjectAction("Create Another Order", Pane.Left));
            WaitForView(Pane.Left, PaneType.Object);
            var num = GetPropertyValue("Sales Order Number", Pane.Left);
            Assert.IsTrue(num.StartsWith("SO75"));
            OpenObjectActions(Pane.Left);
            OpenActionDialog("Add New Detail", Pane.Left);
            ClearFieldThenType("#quantity1", "10");
            var product = WaitForCss("#pane2 .title");
            CopyToClipboard(product);
            PasteIntoInputField("#pane1 .parameter .value.droppable");
            Click(OKButton());
            Click(FullIcon());
            WaitForView(Pane.Single, PaneType.Object);
            Assert.AreEqual("£297.41", GetPropertyValue("Sub Total"));
            Assert.AreEqual("£297.41", GetPropertyValue("Total Due"));
        }

        [TestMethod]
        public void UseOfResolveMethodInADeferredFunction()
        {
            GeminiUrl("object/object?i1=View&o1=AW.Types.Customer--12211&as1=open&i2=View&o2=AW.Types.Product--707");
            WaitForTitle("AW00012211 Victor Romero", Pane.Left);
            Click(GetObjectAction("Create Another Order", Pane.Left));
            WaitForView(Pane.Left, PaneType.Object);
            var num = GetPropertyValue("Sales Order Number", Pane.Left);
            Assert.IsTrue(num.StartsWith("SO75"));
            OpenObjectActions(Pane.Left);
            OpenActionDialog("Add New Detail", Pane.Left);
            var product = WaitForCss("#pane2 .title");
            CopyToClipboard(product);
            PasteIntoInputField("#pane1 .parameter .value.droppable");
            Click(OKButton());
            Click(FullIcon());
            WaitForView(Pane.Single, PaneType.Object);
            Thread.Sleep(1000);
            var listIcon1 = WaitForCssNo(".collection .icon.list", 0);
            Click(listIcon1);
            //var detail = wait.Until(dr => dr.FindElements(By.CssSelector("tr td")).First(el => el.Text == "1 x Sport-100 Helmet, Red"));
            var detail = WaitForCssNo("tbody tr td", 1);
            Assert.AreEqual("1 x Sport-100 Helmet, Red", detail.Text);
            Assert.AreEqual("£29.74", GetPropertyValue("Sub Total", Pane.Left));
            RightClick(detail);
            WaitForView(Pane.Right, PaneType.Object, "1 x Sport-100 Helmet, Red");
            OpenObjectActions(Pane.Right);
            OpenActionDialog("Change Quantity", Pane.Right);
            TypeIntoFieldWithoutClearing("#newquantity2", "2");
            Click(OKButton());
            Thread.Sleep(500);
            Assert.AreEqual("£59.48", GetPropertyValue("Sub Total", Pane.Left));

        }

        [TestMethod, Ignore] //NOT currently working
        public void QueryContributedActionWithChoicesFunction()
        {
            GeminiUrl("list?m1=Product_MenuFunctions&a1=AllProducts&pg1=1&ps1=20&s1_=0&c1=List&as1=open&r1=1&d1=AddAnonReviews");
            WaitForTitle("All Products");
            var option = "select#rating1 option";
            Assert.AreEqual("5", WaitForCssNo(option, 4).Text);
            Assert.AreEqual("1", WaitForCssNo(option, 0).Text);
            SelectDropDownOnField("select#rating", 2);
            var rand = (new Random()).Next(100000).ToString();
            TypeIntoFieldWithoutClearing("#comments1", rand);
            SelectCheckBox("#item1-1");
            Click(OKButton());
            Thread.Sleep(1000);
            Reload();
            WaitForCssNo("tbody tr", 10);
            Assert.AreEqual(1, br.FindElements(By.CssSelector("tbody tr td")).Count(el => el.Text == rand));
        }

        [TestMethod, Ignore] //NOT currently working
        public void QueryContributedActionWithCoValidation()
        {
            GeminiUrl("list?m1=Product_MenuFunctions&a1=AllProducts&pg1=1&ps1=20&s1_=0&c1=List&as1=open&r1=1&d1=AddAnonReviews");
            WaitForTitle("All Products");
            TypeIntoFieldWithoutClearing("#rating1", "4");
            Click(OKButton());
            Thread.Sleep(1000);
            var expected = "Must provide comments for rating < 5";
            var actual = WaitForCss(".co-validation");
            Assert.AreEqual(expected, actual);
        }





    }
}