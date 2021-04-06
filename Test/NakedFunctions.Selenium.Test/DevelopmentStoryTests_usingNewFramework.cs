// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Selenium.Helpers.Tests;
using NakedFrameworkClient.TestFramework;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace NakedFunctions.Selenium.Test.FunctionTests
{

    [TestClass]
    public class DevelopmentStoryTests_usingNewFramework
    {
        #region Overhead
        private string baseUrl = "http://nakedfunctionstest.azurewebsites.net/";
        private Helper helper;

        [ClassInitialize]
        public static void InitialiseClass(TestContext context)
        {
            Helper.FilePath(@"drivers.chromedriver.exe");
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            helper = new Helper(baseUrl);
        }

        [TestCleanup]
        public virtual void CleanUpTest() => helper.CleanUp();
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
            EditActionWithDefaultSuppliedAutomaticallyByEditAttribute();
            AccessToIClock();
            RecordsDoNotHaveEditButton();
            EnumProperty();
            EnumParam();
            DisplayValueAsProperty();
            DisplayCollectionAsProperty();
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
            QueryContributedAndObjectContributedActionsOfSameNameDefinedOnSameType();
            LocalCollectionContributedAction();
            SaveNewChildObjectAndTestItsVisibilityInTheParentsCollection();
            UseOfDeferredFunctionIncludingReload();
            UseOfResolveMethodInADeferredFunction();
            WithDelete();
            WithMultipleDeletes();
            ObjectActionRenderedWithinCollection();
            QueryContributedActionWithChoicesFunction();
            QueryContributedActionWithCoValidation();
            ActionReturingImmutableList();
            //MultiLineActionDialog();
        }

        //[TestMethod]
        public void RetrieveObjectViaMenuAction()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Products").GetActionWithDialog("Find Product By Name")
                .AssertIsEnabled().Open().AssertOKIsDisabled("Missing mandatory fields: Search String; ");
            dialog.GetTextField("Search String").Clear().Enter("handlebar tube");
            dialog.ClickOKToViewNewList().AssertTitleIs("Find Product By Name").GetRowFromList(0).AssertTitleIs("Handlebar Tube");
        }

        //[TestMethod]
        public void ObjectActionThatReturnsJustAContext()
        {
            var offer = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--5")
                .GetObjectView();
            var dialog = offer.OpenActions().GetActionWithDialog("Edit Description").Open();
            dialog.GetTextField("Description").Clear().Enter("Volume Discount 41+").AssertNoValidationError();
            offer = dialog.ClickOKToViewObject();
            dialog = offer.AssertTitleIs("Volume Discount 41+").OpenActions().GetActionWithDialog("Edit Description").Open();
            dialog.GetTextField("Description").Clear().Enter("Volume Discount 41 to 60");
            offer = dialog.ClickOKToViewObject();
            offer.AssertTitleIs("Volume Discount 41 to 60");
        }

        //[TestMethod]
        public void OverriddenPrincipalProviderService()
        {
            helper.GotoHome().OpenMainMenu("Employees").GetActionWithoutDialog("Me")
                .ClickToViewObject().AssertTitleIs("Ken Sánchez");
        }

        //[TestMethod]
        public void UseOfRandomSeedGenerator()
        {
            string prod1Title = helper.GotoHome().OpenMainMenu("Products")
                .GetActionWithoutDialog("Random Product")
                .ClickToViewObject()
                .GetTitle();
            helper.GotoHome();
            string prod2Title = helper.GotoHome().OpenMainMenu("Products")
              .GetActionWithoutDialog("Random Product")
              .ClickToViewObject()
              .GetTitle();

            Assert.AreNotEqual(prod2Title, prod1Title);
        }

        //[TestMethod]
        public void ObjectContributedAction()
        {
            //Tests that an action (side effect free) can be associated with an object
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open").GetObjectView()
                .AssertTitleIs("Mountain Tire Sale")
                .OpenActions().GetActionWithoutDialog("List Associated Products")
                .ClickToViewList()
                .AssertNoOfRowsIs(3)
                .GetRowFromList(2)
                .AssertTitleIs("HL Mountain Tire");
        }

        //[TestMethod]
        public void InformUserViaIAlertService()
        {

            var dialog = helper.GotoUrlViaHome("object/object?i1=View&o1=AW.Types.SpecialOffer--10&as1=open&d1=AssociateWithProduct&i2=View&o2=AW.Types.Product--928")
            .GetObjectView(Pane.Left).GetOpenedDialog();
            var field = dialog.GetReferenceField("Product");
            var prod = helper.GetObjectView(Pane.Right).AssertTitleIs("LL Mountain Tire");
            prod.DragTitleAndDropOnto(field);

            //Problem here, because the object view has not changed. Need a different method?
            //Also needed to test co-validation.
            dialog.ClickOKWithNoResultExpected();
            helper.GetFooter().AssertHasMessage("Mountain Tire Sale is already associated with LL Mountain Tire");

            //wait.Until(d => d.FindElement(By.CssSelector(".footer .messages")).Text != "");
            //var msg = WaitForCss(".footer .messages").Text;
            //Assert.AreEqual("Mountain Tire Sale is already associated with LL Mountain Tire", msg);
        }

        //[TestMethod]
        public void EditAction()
        {
            var offer = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--9&as1=open&d1=EditQuantities")
                .GetObjectView(Pane.Left).AssertTitleIs("Road-650 Overstock");

            string original = offer.GetProperty("Max Qty").GetValue();
            string newQty = new Random().Next(1000).ToString();

            var dialog = offer.GetOpenedDialog();
            dialog.GetTextField("Max Qty").Clear().Enter(newQty);
            dialog.ClickOKToViewObject().GetProperty("Max Qty").AssertValueIs(newQty);

            dialog = offer.OpenActions().GetActionWithDialog("Edit Quantities").Open();
            dialog.GetTextField("Max Qty").Clear().Enter(original);
            dialog.ClickOKToViewObject().GetProperty("Max Qty").AssertValueIs(original);
        }

        //[TestMethod]
        public void EditActionWithDefaultSuppliedAutomaticallyByEditAttribute()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--9&as1=open&d1=EditDescription")
              .GetObjectView(Pane.Left).AssertTitleIs("Road-650 Overstock")
              .GetOpenedDialog().GetTextField("Description").AssertDefaultValueIs("Road-650 Overstock");

        }

        //[TestMethod] 
        public void AccessToIClock()
        {
            var oneMonthOn = DateTime.Today.AddMonths(1).ToString("d MMM yyyy");
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--1&as1=open&d1=EditDates")
                .GetObjectView().GetOpenedDialog().GetTextField("End Date").AssertDefaultValueIs(oneMonthOn);
        }

        //[TestMethod]
        public void RecordsDoNotHaveEditButton()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.WorkOrder--4717")
                .GetObjectView().AssertTitleIs("Road-650 Red, 58: 10/30/2005 12:00:00 AM")
                .AssertIsNotEditable();
        }

        //[TestMethod]
        public void EnumProperty()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--55864")
             .GetObjectView().AssertTitleIs("SO55864")
             .GetProperty("Status").AssertValueIs("Shipped");
        }

        //[TestMethod]
        public void EnumParam()
        {
            helper.GotoHome().OpenMainMenu("Orders").GetActionWithDialog("Orders By Status").Open()
                .GetSelectionField("Status")
                .AssertNoOfOptionsIs(7).AssertOptionIs(0, "*").AssertOptionIs(6, "Shipped");
        }

        //[TestMethod]
        public void DisplayValueAsProperty()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--790")
                .GetObjectView().AssertTitleIs("Road-250 Red, 48")
                .GetProperty(5).AssertNameIs("Description").GetReference()
                .AssertTitleIs("Alluminum-alloy frame provides a light, stiff ride, whether you are racing in the velodrome or on a demanding club ride on country roads.");
        }


        //[TestMethod]
        public void DisplayCollectionAsProperty()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--790")
                .GetObjectView().AssertTitleIs("Road-250 Red, 48")
                .GetCollection("Special Offers").AssertDetails("2 Items");
        }

        //[TestMethod]
        public void DisplayGuidProperty()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesTaxRate--1")
            .GetObjectView().AssertTitleIs("Sales Tax Rate for Alberta")
            .GetProperty("Rowguid").AssertValueIs("683de5dd-521a-47d4-a573-06a3cdb1bc5d");
        }

        [TestMethod]
        public void ParameterChoicesSimple()
        {
            var emp = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Employee--105")
            .GetObjectView().AssertTitleIs("Kevin Homer");
            emp.GetProperty("Marital Status").AssertValueIs("S");
            var dialog = emp.OpenActions().GetActionWithDialog("Update Marital Status").Open();

            dialog.GetSelectionField("Marital Status").AssertOptionsAre("S", "M").Select(1);
            emp = dialog.ClickOKToViewObject();
            emp.GetProperty("Marital Status").AssertValueIs("M");

            dialog = emp.OpenActions().GetActionWithDialog("Update Marital Status").Open();
            dialog.GetSelectionField("Marital Status").AssertOptionsAre("S", "M").Select(0);
            emp = dialog.ClickOKToViewObject();
            emp.GetProperty("Marital Status").AssertValueIs("S");
        }

        //[TestMethod]
        public void ParameterChoicesDependent()
        {
           var dialog = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Address--22691&as1=open&d1=EditStateProvince")
            .GetObjectView().AssertTitleIs("2107 Cardinal...").GetOpenedDialog();
            var field1 = dialog.GetSelectionField("Country Region").AssertNoOfOptionsIs(239);
            dialog.GetSelectionField("State Province").AssertNoOfOptionsIs(0);
            field1.Select(36);
            dialog.GetSelectionField("State Province").AssertNoOfOptionsIs(14)
                .AssertOptionIs(13, "Yukon Territory");
        }

        //[TestMethod]
        public void ParameterDefaultFunction()
        {
            var oneMonthOn = DateTime.Today.AddMonths(1).ToString("d MMM yyyy");
            helper.GotoHome().OpenMainMenu("Special Offers")
                .GetActionWithDialog("Create New Special Offer").Open()
                .GetTextField("End Date").AssertDefaultValueIs(oneMonthOn);
        }

        //[TestMethod]
        public void ValidateSingleParam()
        {
            var dialog = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--3&as1=open&d1=EditQuantities")
                .GetObjectView().AssertTitleIs(("Volume Discount 15 to 24")).GetOpenedDialog();

            var min = dialog.GetTextField("Min Qty").Clear().Enter("0");
            dialog.GetTextField("Max Qty").Clear().Enter("5");
            dialog.ClickOKWithNoResultExpected();
            dialog.AssertHasValidationError("See field validation message(s).");
            min.AssertHasValidationError("Must be > 0");
        }

        //[TestMethod]
        public void ValidateMultipleParams()
        {
            var dialog = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SpecialOffer--3&as1=open&d1=EditQuantities")
               .GetObjectView().AssertTitleIs(("Volume Discount 15 to 24")).GetOpenedDialog();

            var min = dialog.GetTextField("Min Qty").Clear().Enter("10");
            dialog.GetTextField("Max Qty").Clear().Enter("5");
            dialog.ClickOKWithNoResultExpected();

            dialog.AssertHasValidationError("Max Qty cannot be < Min Qty");
        }

        //[TestMethod]
        public void DisableFunction()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--75084")
                .GetObjectView().AssertTitleIs("SO75084").OpenActions()
                .GetActionWithDialog("Add New Detail").AssertIsDisabled("Can only add to 'In Process' order");
        }

        //[TestMethod]
        public void HideFunction()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--75084")
              .GetObjectView().AssertTitleIs("SO75084").OpenActions()
              .AssertHasAction("Remove Detail")
              .AssertDoesNotHaveAction("Approve Order");
        }

        //[TestMethod]
        public void AutoCompleteFunction()
        {
            helper.GotoHome().OpenMainMenu("Work Orders").GetActionWithDialog("List Work Orders").Open()
                .GetReferenceField("Product").AssertSupportsAutoComplete().Enter("fr")
                .AssertHasAutoCompleteOption(4, "Front Derailleur Linkage")
                .AssertHasAutoCompleteOption(0, "Freewheel");
        }

        //[TestMethod]
        public void ViewModel1()
        {
            var summary = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.StaffSummary--84--206")
                .GetObjectView().AssertTitleIs("Staff Summary");

            summary.GetProperty("Female").AssertValueIs("84");
            summary.GetProperty("Male").AssertValueIs("206");
            summary.GetProperty("Total Staff").AssertValueIs("290");
        }

       //[TestMethod]
        public void CreateNewObectWithOnlyValueProperties()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Special Offers")
                .GetActionWithDialog("Create New Special Offer").Open();

            dialog.GetTextField("Description").Enter("Manager's Special");
            dialog.GetTextField("Discount Pct").Enter("15");

            var endDate = DateTime.Today.AddDays(7).ToString("d MMM yyyy");
            dialog.GetTextField("End Date").Clear().Enter(endDate);

            var now = DateTime.Now.ToString("d MMM yyyy HH:mm:").Substring(0, 16);
            var modified = dialog.ClickOKToViewObject().AssertTitleIs("Manager's Special")
                .GetProperty("Modified Date").GetValue();
            Assert.AreEqual(now, modified.Substring(0, 16));
        }

        //[TestMethod]
        public void CreateNewObjectWithAReferenceToAnotherExistingObject()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Addresses")
               .GetActionWithDialog("Create New Address").Open();
            dialog.GetSelectionField("Type").Select(2);
            dialog.GetTextField("Line1").Enter("Dunroamin");
            dialog.GetTextField("Line2").Enter("Elm St");
            dialog.GetTextField("City").Enter("Concord");
            var zip = (new Random()).Next(10000).ToString();
            dialog.GetTextField("Post Code").Enter(zip);
            dialog.GetSelectionField("State / Province").Select(1);
            var addr = dialog.ClickOKToViewObject().AssertTitleIs("Dunroamin...");
            addr.GetProperty("Address Line1").AssertValueIs("Dunroamin");
            addr.GetProperty("State Province").AssertValueIs("Alberta");
        }

        //[TestMethod]
        public void CreateNewObjectWithAReferenceToMultipleExistingObjects()
        {
            var order = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Customer--12211&as1=open")
                .GetObjectView().AssertTitleIs("AW00012211 Victor Romero").OpenActions()
                .GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            var num = order.GetProperty("Sales Order Number").GetValue();
            Assert.IsTrue(num.StartsWith("SO75"));
            order.GetProperty("Customer").GetReference().AssertTitleIs("AW00012211 Victor Romero");
        }

        //[TestMethod]
        public void CreateAGraphOfTwoNewRelatedObjects()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Customers").OpenSubMenu("Stores")
                .GetActionWithDialog("Create New Store Customer").Open();
            int rand = (new Random()).Next(1970, 2021);
            var name = $"FooBar Frames {rand} Ltd";
            dialog.GetTextField("Name").Enter(name);
            var cust = dialog.ClickOKToViewObject();
            Assert.IsTrue(cust.GetTitle().EndsWith(name));
            var storeTitle = cust.GetProperty("Store").GetReference().GetTitle();
            Assert.IsTrue(storeTitle.EndsWith(name));
        }

        //[TestMethod]
        public void CreateAGraphOfObjectsThreeLevelsDeep()
        {
            //This story involves creation of a graph of three new objects (Customer, Person, Password)
            //with two levels of dependency
            var dialog = helper.GotoHome().OpenMainMenu("Customers").OpenSubMenu("Individuals")
              .GetActionWithDialog("Create New Individual Customer").Open();

            dialog.GetTextField("First Name").Enter("Fred");
            dialog.GetTextField("Last Name").Enter("Bloggs");
            dialog.GetTextField("Password").Enter("foobar");
            var cust = dialog.ClickOKToViewObject();
           Assert.IsTrue(cust.GetTitle().EndsWith("Fred Bloggs"));
            var person = cust.GetProperty("Person").GetReference().Click();
            person.GetProperty("Password").GetReference().AssertTitleIs("Password");
        }


        //[TestMethod]
        public void PropertyHiddenViaAHideMethod()
        {
            //An Individual Customer should not display a Store property
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Customer--29207")
                .GetObjectView().AssertTitleIs("AW00029207 Katelyn James")
                .AssertPropertiesAre("Account Number", "Customer Type", "Person", "Sales Territory");

            //A Store Customer should not display a Person property
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Customer--29553")
                .GetObjectView().AssertTitleIs("AW00029553 Unsurpassed Bikes")
                .AssertPropertiesAre("Account Number", "Customer Type", "Store", "Sales Territory");
        }

        //[TestMethod]
        public void SubMenuOnObject()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Vendor--1500")
                 .GetObjectView().AssertTitleIs("Morgan Bike Accessories").OpenActions()
                 .AssertHasActions("Show All Products", "Check Credit")
                 .OpenSubMenu("Purchase Orders")
                .AssertHasActions("Create New Purchase Order", "Open Purchase Orders", "List Purchase Orders", "Show All Products", "Check Credit");
        }

        //[TestMethod]
        public void SubMenuOnMainMenu()
        {
            helper.GotoHome().OpenMainMenu("Customers").AssertHasActions(
                "Find Customer By Account Number", 
                "List Customers For Sales Territory")
            .AssertHasSubMenus("Individuals", "Stores")
            .OpenSubMenu("Individuals").AssertHasActions(
                "Find Individual Customer By Name",
                "Create New Individual Customer",
                "Random Individual",
                "Recent Individual Customers",
                "Find Customer By Account Number",
                "List Customers For Sales Territory")  
            .OpenSubMenu("Stores").AssertHasActions(
                "Find Individual Customer By Name",
                "Create New Individual Customer",
                "Random Individual",
                "Recent Individual Customers",
                  "Find Store By Name",
                   "Create New Store Customer",
                   "Random Store",
                 "Find Customer By Account Number",
                "List Customers For Sales Territory");
        }

        //[TestMethod]
        public void ImageProperty()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--881")
                .GetObjectView().AssertTitleIs("Short-Sleeve Classic Jersey, S")
                .GetProperty("Photo").AssertIsImage();
        }

        //[TestMethod]
        public void ImageParameter()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Product--881")
                .GetObjectView().AssertTitleIs("Short-Sleeve Classic Jersey, S")
                .OpenActions().GetActionWithDialog("Add Or Change Photo").Open()
                .AssertHasImageField("New Image");
        }

        //[TestMethod]
        public void QueryContributedActionReturningOnlyAContext()
        {
            var offers = helper.GotoHome().OpenMainMenu("Special Offers")
                 .GetActionWithoutDialog("All Special Offers").ClickToViewList()
                 .AssertTitleIs("All Special Offers")
                 .ClickTableView();

            var original3 = offers.GetRowFromTable(3).GetColumnValue(5);

            int rand = (new Random()).Next(1000);
            var endDate = DateTime.Today.AddDays(rand).ToString("dd MMM yyyy");
            endDate = endDate.StartsWith("0") ? endDate.Substring(1) : endDate;

            //Assert.IsFalse(br.FindElements(By.CssSelector("tbody tr td")).Any(el => el.Text == endDate));

            var dialog = offers.OpenActions().GetActionWithDialog("Extend Offers").Open();
            dialog.GetTextField("To Date").Enter(endDate);

            offers.SelectCheckBoxOnRow(0)
            .SelectCheckBoxOnRow(1)
            .SelectCheckBoxOnRow(2);

            var updated = dialog.ClickOKToViewUpdatedList();

            updated.GetRowFromTable(0).AssertColumnValueIs(5, endDate);
            updated.GetRowFromTable(1).AssertColumnValueIs(5, endDate);
            updated.GetRowFromTable(2).AssertColumnValueIs(5, endDate);

            //Check that row3 was NOT updated:
            updated.GetRowFromTable(3).AssertColumnValueIs(5, original3);
        }

         //[TestMethod]
        public void QueryContributedAndObjectContributedActionsOfSameNameDefinedOnSameType()
        {
            helper.GotoHome().OpenMainMenu("Orders")
               .GetActionWithoutDialog("Orders In Process").ClickToViewList()
                .OpenActions().GetActionWithDialog("Append Comment").Open()
                .GetTextField("Comment");

           helper.GotoUrlDirectly("object?i1=View&o1=AW.Types.SalesOrderHeader--73266")
                .GetObjectView().AssertTitleIs("SO73266")
                .OpenActions().GetActionWithDialog("Append Comment").Open()
                .GetTextField("Comment To Append");
        }

        //[TestMethod]
        public void LocalCollectionContributedAction()
        {
            var details = helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--53535&c1_Details=Table")
            .GetObjectView().AssertTitleIs("SO53535").GetCollection("Details");
            var originalCtn0 = details.GetRowFromTable(0).GetColumnValue(6);

            var dialog = details.GetActionWithDialog("Add Carrier Tracking Number").Open();
            var rnd = (new Random()).Next(100000).ToString();
            var num = dialog.GetTextField("Ctn").Enter(rnd);
            details.SelectCheckBoxOnRow(1)
            .SelectCheckBoxOnRow(2)
            .SelectCheckBoxOnRow(3);
            var updated = dialog.ClickOKToViewObject().Reload();
            details = updated.GetCollection("Details");
            details.GetRowFromTable(1).AssertColumnValueIs(6, rnd);
                details.GetRowFromTable(2).AssertColumnValueIs(6, rnd);
            details.GetRowFromTable(3).AssertColumnValueIs(6, rnd);

            //Confirm that row 0 was not changed
            details.GetRowFromTable(0).AssertColumnValueIs(6, originalCtn0);
        }

        // [TestMethod]
        public void SaveNewChildObjectAndTestItsVisibilityInTheParentsCollection()
        {
            var view = helper.GotoUrlViaHome("object/object?i1=View&o1=AW.Types.Customer--12211&i2=View&o2=AW.Types.Product--707");
            var cust = view.GetObjectView(Pane.Left).AssertTitleIs("AW00012211 Victor Romero");
            var prod = view.GetObjectView(Pane.Right).AssertTitleIs("Sport-100 Helmet, Red");
            var order = cust.OpenActions().GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            Assert.IsTrue(order.GetProperty("Sales Order Number").GetValue().StartsWith("SO75"));
            var dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();

            prod.DragTitleAndDropOnto(dialog.GetReferenceField("Product"));
            dialog.ClickOKToViewObject().GetCollection("Details")
                .ClickListView()
                .GetRowFromList(0)
                .AssertTitleIs("1 x Sport-100 Helmet, Red");
        }

        //[TestMethod]
        public void UseOfDeferredFunctionIncludingReload()
        {
            var view = helper.GotoUrlViaHome("object/object?i1=View&o1=AW.Types.Customer--12211&i2=View&o2=AW.Types.Product--707");
            var cust = view.GetObjectView(Pane.Left).AssertTitleIs("AW00012211 Victor Romero");
            var prod = view.GetObjectView(Pane.Right).AssertTitleIs("Sport-100 Helmet, Red");
            var order = cust.OpenActions().GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            Assert.IsTrue(order.GetProperty("Sales Order Number").GetValue().StartsWith("SO75"));
            var dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();
            prod.DragTitleAndDropOnto(dialog.GetReferenceField("Product"));
            dialog.GetTextField("Quantity").Clear().Enter("9");
            var updated = dialog.ClickOKToViewObject();
            updated.GetProperty("Sub Total").AssertValueIs("£267.67");
            updated.GetProperty("Total Due").AssertValueIs("£267.67");
        }

        //[TestMethod]
        public void UseOfResolveMethodInADeferredFunction()
        {
            var view = helper.GotoUrlViaHome("object/object?i1=View&o1=AW.Types.Customer--12211&i2=View&o2=AW.Types.Product--707");
            var cust = view.GetObjectView(Pane.Left).AssertTitleIs("AW00012211 Victor Romero");
            var prod = view.GetObjectView(Pane.Right).AssertTitleIs("Sport-100 Helmet, Red");
            var order = cust.OpenActions().GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            Assert.IsTrue(order.GetProperty("Sales Order Number").GetValue().StartsWith("SO75"));
            var dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();

            prod.DragTitleAndDropOnto(dialog.GetReferenceField("Product"));
            order = dialog.ClickOKToViewObject();

            var details = order.GetCollection("Details");
            details.ClickListView()
                .GetRowFromList(0)
                .AssertTitleIs("1 x Sport-100 Helmet, Red");

            order.GetProperty("Sub Total").AssertValueIs("£29.74");

            dialog = details.GetRowFromList(0).Click(MouseClick.SecondaryButton)
                .OpenActions().GetActionWithDialog("Change Quantity").Open();

            dialog.GetTextField("New Quantity").Enter("2");
            dialog.ClickOKToViewObject();
            Thread.Sleep(500);
            order.FreshView().GetProperty("Sub Total").AssertValueIs("£59.48");
        }


        //[TestMethod]
        public void WithDelete()
        {
            var view = helper.GotoUrlViaHome("object/object?i1=View&o1=AW.Types.Customer--12211&i2=View&o2=AW.Types.Product--707");
            var cust = view.GetObjectView(Pane.Left).AssertTitleIs("AW00012211 Victor Romero");
            var prod = view.GetObjectView(Pane.Right).AssertTitleIs("Sport-100 Helmet, Red");
            var order = cust.OpenActions().GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            Assert.IsTrue(order.GetProperty("Sales Order Number").GetValue().StartsWith("SO75"));
            var dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();

            prod.DragTitleAndDropOnto(dialog.GetReferenceField("Product"));
            order = dialog.ClickOKToViewObject();
            order.GetCollection("Details").ClickListView().SelectCheckBoxOnRow(0)
                .GetActionWithoutDialog("Remove Details").ClickToViewObject();
            order.GetCollection("Details").AssertDetails("Empty");
        }

        //[TestMethod]
        public void WithMultipleDeletes()
        {
            //Build an order
            var main = helper.GotoUrlViaHome(@"object/list?i1=View&o1=AW.Types.Customer--29861&m2=Product_MenuFunctions&a2=ListProductsByCategory&pg2=1&ps2=20&s2_=0&c2=List&pm2_category=%7B""href"":""___0%2Fobjects%2FAW.Types.ProductCategory%2F1""%7D&pm2_subCategory=null");
            var cust = main.GetObjectView(Pane.Left).AssertTitleIs("AW00029861 Hardware Components");


            var order = cust.OpenActions().GetActionWithoutDialog("Create Another Order").ClickToViewObject();

            var dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();
            var prodField = dialog.GetReferenceField("Product");
            var prods = main.GetReloadedListView(Pane.Right).AssertTitleIs("List Products By Category");
            prods.GetRowFromList(0).AssertTitleIs("Road-150 Red, 62").DragAndDropOnto(prodField);
            order = dialog.ClickOKToViewObject();

            dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();
            prodField = dialog.GetReferenceField("Product");
            prods = main.GetReloadedListView(Pane.Right);
            prods.GetRowFromList(1).AssertTitleIs("Road-150 Red, 44").DragAndDropOnto(prodField);
            order = dialog.ClickOKToViewObject();

            dialog = order.OpenActions().GetActionWithDialog("Add New Detail").Open();
            prodField = dialog.GetReferenceField("Product");
            prods = main.GetReloadedListView(Pane.Right);
            prods.GetRowFromList(1).AssertTitleIs("Road-150 Red, 44").DragAndDropOnto(prodField);
            order = dialog.ClickOKToViewObject();

            var details = order.GetCollection("Details").AssertDetails("3 Items");
             var remove =    details.ClickListView().GetActionWithoutDialog("Remove Details");

            details.SelectCheckBoxOnRow(0).SelectCheckBoxOnRow(2);

            order = remove.ClickToViewObject();
            order.GetCollection("Details").AssertDetails("1 Item");
        }

        //[TestMethod]
        public void ObjectActionRenderedWithinCollection()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.SalesOrderHeader--44868&c1_Details=List")
                .GetObjectView().GetCollection("Details")
                .GetActionWithDialog("Change A Quantity");
        }

        //[TestMethod]
        public void QueryContributedActionWithChoicesFunction()
        {
            var list = helper.GotoUrlViaHome("list?m1=Product_MenuFunctions&a1=AllProducts&pg1=1&ps1=20&s1_=0&c1=List&as1=open&r1=1&d1=AddAnonReviews")
                .GetReloadedListView();
           var dialog = list.GetOpenedDialog();

            dialog.GetSelectionField("No. of Stars (1-5)").Select(4);
            var rand = (new Random()).Next(100000).ToString();
            dialog.GetTextField("Comments").Enter(rand);

            list.SelectCheckBoxOnRow(1);
            dialog.ClickOKToViewUpdatedList();


            helper.GotoUrlDirectly("object?i1=View&o1=AW.Types.Product--2&c1_ProductReviews=Table")
                .GetObjectView().GetCollection("Product Reviews")
                .GetLastRowFromTable().AssertColumnValueIs(1, rand);
            //TODO: column numbers wrong!
        }

        //[TestMethod]
        public void QueryContributedActionWithCoValidation()
        {
            var list = helper.GotoUrlViaHome("list?m1=Product_MenuFunctions&a1=AllProducts&pg1=1&ps1=20&s1_=0&c1=List&as1=open&r1=1&d1=AddAnonReviews")
                .GetReloadedListView().AssertTitleIs("All Products");

            var dialog = list.GetOpenedDialog();
            dialog.GetSelectionField("No. of Stars (1-5)").Select(3);
            list.SelectCheckBoxOnRow(1);
            dialog.ClickOKWithNoResultExpected();
            dialog.AssertHasValidationError("Must provide comments for rating < 5");
        }

        //[TestMethod]
        public void ActionReturingImmutableList()
        {
            helper.GotoUrlViaHome("object?i1=View&o1=AW.Types.Vendor--1696&as1=open")
                .GetObjectView().AssertTitleIs("Chicago City Saddles")
                .OpenActions().GetActionWithoutDialog("Show All Products")
                .ClickToViewList()
                .GetLastRowFromList().AssertTitleIs("HL Touring Seat/Saddle");
        }

       //[TestMethod]
        public void MultiLineActionDialog()
        {
            //GeminiUrl("multiLineDialog?m1=SpecialOffer_MenuFunctions&d1=CreateMultipleSpecialOffers");
            //WaitForTitle("Create Multiple Special Offers");
            //WaitForCssNo(".lineDialog", 1); //i.e. 2 dialogs
            //var ok1 = WaitForCssNo("input.ok", 1);
            //var val1 = WaitForCssNo(".co-validation", 1);
            //var ok0 = WaitForCssNo("input.ok", 0);
            //var val0 = WaitForCssNo(".co-validation", 0);
            //TypeIntoFieldWithoutClearing("input#description0", "Manager's Special");
            //TypeIntoFieldWithoutClearing("input#discountpct0", "15");
            //var endDate = DateTime.Today.AddDays(7).ToString("d MMM yyyy");
            //TypeIntoFieldWithoutClearing("input#enddate0", endDate);
            //wait.Until(d => ok0.GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            //Assert.AreEqual("", val0.Text);
            //Click(ok0);
            //wait.Until(br => br.FindElements(By.CssSelector(".co-validation")).First().Text == "Submitted");
            ////Second line
            //TypeIntoFieldWithoutClearing("input#description1", "Manager's Special II");
            //TypeIntoFieldWithoutClearing("input#discountpct1", "12.5");
            //TypeIntoFieldWithoutClearing("input#enddate1", endDate);
            //wait.Until(d => ok1.GetAttribute("disabled") is null || OKButton().GetAttribute("disabled") == "");
            //Assert.AreEqual("", val1.Text);
            //Click(ok1);
            //wait.Until(br => br.FindElements(By.CssSelector(".co-validation")).ElementAt(1).Text == "Submitted");

            ////Check third line has now appeared
            //WaitForCssNo(".lineDialog", 2);
        }

    }
}