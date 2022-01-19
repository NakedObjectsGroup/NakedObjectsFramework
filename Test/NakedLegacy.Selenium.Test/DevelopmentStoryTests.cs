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
    public class DevelopmentStoryTests
    {
        #region Overhead
        private string baseUrl = "http://nakedlegacytest.azurewebsites.net/";
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
            ViewPersistentObjectsAndProperties();
            ReferencePropertiesAndCollections();
            Titles();
            MemberOrder();
            BoundedTypes();
            ActionsThatRetrieveObjects();
            EditingObjects();
            Menus();
            FieldAboutSpecifyingName_Description_Editability();
            CreatingAndSavingObjects();
            //ActionAboutControl();
        }

        #region ViewPersistentObjectsAndProperties
        //[TestMethod]
        public void ViewPersistentObjectsAndProperties()
        {
            ViewInstanceDirectlyByUrl();
            TextStringProperty();
            WholeNumberProperty();
            DateProperty();
            TimeStampProperty();
            BooleanProperty();
            TitleConstructedFromValueFields();
        }

        //[TestMethod]
        public void ViewInstanceDirectlyByUrl()
        {
                helper.GotoUrlDirectly(prefix + "Address--13618");
                helper.GetObjectView();
        }

        //[TestMethod]
        public void TextStringProperty()
        {
            helper.GotoUrlDirectly(prefix + "Address--13618");
            helper.GetObjectView().GetProperty("Address Line1").AssertValueIs("Waldstr 91");
        }

       //[TestMethod]
        public void TimeStampProperty()
        {
            helper.GotoUrlDirectly(prefix + "Address--13618");
            var d = helper.GetObjectView().GetProperty("Modified Date").GetValue();
            Assert.IsTrue(d.StartsWith("31 Jul 2008") && d.EndsWith(":00:00")); //Miss out the hours due to time zone issues
        }

        //[TestMethod]
        public void DateProperty()
        {
            helper.GotoUrlDirectly(prefix + "Employee--2");
            helper.GetObjectView().GetProperty("Hire Date").AssertValueIs("3 Mar 2002");
        }


        //[TestMethod]
        public void WholeNumberProperty()
        {
            helper.GotoUrlDirectly(prefix + "Employee--2");
            helper.GetObjectView().GetProperty("Sick Leave Hours").AssertValueIs("20");
        }


        //[TestMethod]
        public void BooleanProperty()
        {
            helper.GotoUrlDirectly(prefix + "Employee--2");
            var flag = helper.GetObjectView().GetProperty("Salaried");
            flag.AssertIsCheckbox();
        }
        #endregion

        #region Reference properties and collections
        //[TestMethod]
        public void ReferencePropertiesAndCollections()
        {
            ReferenceProperty();
            TitleConstructedFromReferenceFields();
            InternalCollection();
        }

        //[TestMethod]
        public void ReferenceProperty()
        {
            helper.GotoUrlDirectly(prefix + "Employee--66");
            helper.GetObjectView().GetProperty("Person Details").GetReference().AssertTitleIs("Karan Khanna");
        }

        //[TestMethod]
        public void InternalCollection()
        {
            helper.GotoUrlDirectly(prefix + "SalesOrderHeader--52035");
            var obj = helper.GetObjectView().AssertTitleIs("SO52035");
            var coll = obj.GetCollection("Details").AssertDetails("2 Items");
            coll.ClickListView().GetRowFromList(0).AssertTitleIs("1 x AWC Logo Cap");
            coll.ClickTableView().GetRowFromTable(1).AssertColumnValueIs(3,"£32.60"); //TODO should become €32.60
        }
        #endregion

        #region Titles
        //[TestMethod]
        public void Titles()
        {
            TitleConstructedFromReferenceFields();
            TitleConstructedFromValueFields();
        }

        //[TestMethod]
        public void TitleConstructedFromReferenceFields()
        {
            helper.GotoUrlDirectly(prefix + "Employee--67");
            helper.GetObjectView().AssertTitleIs("Jay Adams");
        }

        //[TestMethod]
        public void TitleConstructedFromValueFields()
        {
            helper.GotoUrlDirectly(prefix + "Person--2284");
            helper.GetObjectView().AssertTitleIs("Lynn Tsoflias");
        }
        #endregion

        #region Member Order

        //[TestMethod]
        public void MemberOrder()
        {
            FieldOrderSpecifiedByAttribute();
            FieldOrderSpecifiedByMethod();
        }

        //[TestMethod]
        public void FieldOrderSpecifiedByAttribute() //Also tests that system value properties are not displayed
        {
            var obj = AccessInstanceWithTitle("Address--24082", "4669 Berry Dr....");
            obj.AssertPropertiesAre("Address Line1", "Address Line2", "City", "Postal Code", "State Province", "Modified Date");
        }

        //[TestMethod]
        public void FieldOrderSpecifiedByMethod()
        {
            var obj = AccessInstanceWithTitle("Vendor--1674", "Varsity Sport Co.");
            obj.AssertPropertiesAre("Account Number", "Name", "Credit Rating", "Preferred Vendor Status",
                "Active Flag", "Purchasing Web Service URL", "Modified Date");
        }

        #endregion

        #region Bounded Types
        //[TestMethod]
        public void BoundedTypes()
        {
            AccessInstanceWithTitle("Employee--33", "Annik Stahl")
                .OpenActions().GetActionWithDialog("Change Department Or Shift")
                .Open().GetSelectionField("Department")
                .AssertOptionIs(0, "Engineering")
                .AssertOptionIs(1, "Tool Design")
                .AssertOptionIs(2, "Sales");
        }

        #endregion

        #region Actions that retrieve objects
        //[TestMethod]
        public void ActionsThatRetrieveObjects()
        {
            //MainMenuActionToRetrieveAnArrayList(); TODO: Unreliable
            ObjectActionToRetrieveAQueryable();
            ObjectActionToRetrieveASingleInstance();
            ObjectActionThatDelegatesToARepositoryService();
            MenuActionThatTakesParameters();
            MenuActionThatReturnsArrayList();
        }

        //[TestMethod]
        public void MainMenuActionToRetrieveAnArrayList()
        {
            helper.GotoHome().OpenMainMenu("Employees")
                .GetActionWithoutDialog("List All Departments").ClickToViewList()
                .AssertTitleIs("List All Departments").AssertNoOfRowsIs(16).GetRowFromList(3)
                .AssertTitleIs("Marketing");
        }

        //[TestMethod]
        public void ObjectActionToRetrieveAQueryable()
        {
            helper.GotoHome().OpenMainMenu("Products")
                 .GetActionWithoutDialog("All Products").ClickToViewList()
                 .AssertDetails("Page 1 of 26; viewing 20 of 504 items")
                 .GetRowFromList(0).AssertTitleIs("Adjustable Race");
        }

        //[TestMethod]
        public void ObjectActionToRetrieveASingleInstance()
        {
            var dialog = AccessInstanceWithTitle("Product--829", "Touring Rear Wheel").OpenActions().
        GetActionWithDialog("Best Special Offer").Open();
            dialog.GetTextField("Quantity").Enter("10");
            dialog.ClickOKToViewObject().AssertTitleIs("No Discount");
        }

        //[TestMethod]
        public void ObjectActionThatDelegatesToARepositoryService()
        {
            AccessInstanceWithTitle("Person--1", "Ken Sánchez").OpenActions()
                .GetActionWithoutDialog("Others With Same Initials").ClickToViewList()
                .AssertDetails("Page 1 of 8; viewing 20 of 142 items")
                .GetRowFromList(0).AssertTitleIs("Kaitlin Sai");
        }

        //[TestMethod]
        public void MenuActionThatTakesParameters()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Employees")
                .GetActionWithDialog("Find Employee By National ID Number").Open();
            dialog.GetTextField("National ID Number").Enter("416679555");
            dialog.ClickOKToViewObject().AssertTitleIs("Hao Chen");
        }

        //[TestMethod]
        public void MenuActionThatReturnsArrayList()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Employees")
               .GetActionWithDialog("Find Employee By Name").Open();
            dialog.GetTextField("First Name").Enter("a");
            dialog.GetTextField("Last Name").Enter("b");
            dialog.ClickOKToViewNewList().AssertNoOfRowsIs(3)
                .GetRowFromList(0).AssertTitleIs("Angela Barbariol");
        }

        #endregion

        #region Editing objects
        //[TestMethod]
        public void EditingObjects()
        {
            EditAndCancelWithoutModification();
            EditAndSaveChange();
        }

        //[TestMethod]
        public void EditAndCancelWithoutModification()
        {
            AccessInstanceWithTitle("WorkOrder--16080", "LL Bottom Bracket: 04/07/2006")
                .Edit().AssertTitleIs("Editing - LL Bottom Bracket: 04/07/2006")
                .Cancel().AssertTitleIs("LL Bottom Bracket: 04/07/2006");
        }

        //[TestMethod]
        public void EditAndSaveChange()
        {
            var editView = AccessInstanceWithTitle("WorkOrder--16080", "LL Bottom Bracket: 04/07/2006")
                .Edit();
            editView.GetEditableSelectionProperty("Scrap Reason")
            .AssertOptionIs(0, "").AssertOptionIs(3, "Gouge in metal").Select(3);
            editView.GetEditableTextInputProperty("Scrapped Qty").Clear().Enter("2");
            var updated = editView.Save();
            updated.GetProperty("Scrap Reason").AssertValueIs("Gouge in metal");
            updated.GetProperty("Scrapped Qty").AssertValueIs("2");
            editView = updated.Edit();
            editView.GetEditableSelectionProperty("Scrap Reason")
            .AssertOptionIs(0, "").Select(0);
            editView.GetEditableTextInputProperty("Scrapped Qty").Clear();
            updated = editView.Save();
            updated.GetProperty("Scrap Reason").AssertValueIs("");
            updated.GetProperty("Scrapped Qty").AssertValueIs("0");
        }
        #endregion

        #region Menus
        //[TestMethod]
        public void Menus()
        {
            MainMenuWithSubMenus();
            ObjectActionsMenu();
        }

        //[TestMethod]
        public void MainMenuWithSubMenus()
        {
            helper.GotoHome().OpenMainMenu("Employees").AssertHasActions("Random Employee",
                "All Employees", "Find Employee By Name", "Find Employee By National ID Number")
                .AssertHasSubMenus("Organisation").OpenSubMenu("Organisation").AssertHasAction("List All Departments");

        }

        //[TestMethod]
        public void ObjectActionsMenu()
        {
            AccessInstanceWithTitle("Product--897", "LL Touring Frame - Blue, 58")
                .OpenActions().AssertHasActions("Best Special Offer", "Associate With Special Offer")
                .AssertHasSubMenus("Work Orders").OpenSubMenu("Work Orders").AssertHasAction(
                "Current Work Orders").AssertHasAction("Create New Work Order");
        }

        #endregion

        #region FieldAboutSpecifyingName_Description_Editability
        //[TestMethod]
        public void FieldAboutSpecifyingName_Description_Editability()
        {
            PropertyHiddenUsingFieldAbout();
            PropertyRenamedUsingFieldAbout();
            PropertyMadeUneditableUsingFieldAbout();
            PropertyValidationUsingFieldAbout();
        }

        //[TestMethod]
        public void PropertyHiddenUsingFieldAbout()
        {
            var obj = AccessInstanceWithTitle("PhoneNumberType--1", "Cell");
            obj.AssertPropertiesAre(); //Because all properties have been hidden individually using FieldAbout
        }

        //[TestMethod]
        public void PropertyRenamedUsingFieldAbout()
        {
            var obj = AccessInstanceWithTitle("Person--115", "Angela Barbariol");
            obj.GetProperty(4).AssertNameIs("Reverse name order");
        }

        //[TestMethod]
        public void PropertyMadeUneditableUsingFieldAbout()
        {
            AccessInstanceWithTitle("Department--1", "Engineering").Edit()
                .AssertPropertyIsDisabledForEdit("Modified Date");   
        }

        //[TestMethod]
        public void PropertyValidationUsingFieldAbout()
        {
            var editView = AccessInstanceWithTitle("Department--10", "Finance").Edit();
            editView.GetEditableTextInputProperty("Group Name").Clear()
                .Enter("Now is the time for all good men to come to the aid of the party");
            editView.AttemptUnsuccessfulSave()
                .GetEditableTextInputProperty("Group Name")
                .AssertHasValidationError("Cannot be > 50 chars");
            editView.Cancel();
        }

        #endregion

        #region Creating & Saving objects
        //[TestMethod]
        public void CreatingAndSavingObjects()
        {
            CreateAndSaveObjectProgrammatically();
        }
        //[TestMethod]
        public void CreateAndSaveObjectProgrammatically()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Employees").OpenSubMenu("Organisation")
                .GetActionWithDialog("Create New Department").Open();

            int rnd = (new Random()).Next(1, 1000);
            string name = $"Dept. {rnd}";
            dialog.GetTextField("Name").Enter(name);
            dialog.GetTextField("Group Name").Enter("Testing");
            dialog.ClickOKToViewObject().AssertTitleIs($"Dept. {rnd}");

            helper.GotoHome().OpenMainMenu("Employees").OpenSubMenu("Organisation")
                .GetActionWithoutDialog("List New Departments").ClickToViewList()
                .GetRowFromList(0).AssertTitleIs(name);
        }
        #endregion

        #region ActionAbout control
        //[TestMethod]
        public void ActionAboutControl()
        {
            ActionName();
            ActionDescription();
            ActionVisibilityBasedOnObjectState();
            ActionUsabilityBasedOnObjectState_WithoutParams();
            ActionUsabilityBasedOnObjectState_WithParams();
            ActionParameterValidation();
        }

        [TestMethod]
        public void ActionName()
        {
            //Test invoked within Name mode
        }

        [TestMethod]
        public void ActionDescription()
        {
            //Test invoked within Name mode
        }

        [TestMethod]
        public void ActionVisibilityBasedOnObjectState()
        {
            //Test invoked within Visible mode
        }

        [TestMethod]
        public void ActionUsabilityBasedOnObjectState_WithoutParams()
        {
            //Test invoked within Usable mode
        }

        [TestMethod]
        public void ActionUsabilityBasedOnObjectState_WithParams()
        {
            //Test invoked within Usable mode
        }


        [TestMethod]
        public void ActionParameterValidation()
        {
            //Test invoked within Valid mode

        }


        #endregion

        #region Helpers

        private const string prefix = "object?i1=View&o1=AW.Types.";
        private ObjectView AccessInstanceWithTitle(string identifier, string title)
        {
            helper.GotoUrlViaHome(prefix + identifier);
            return helper.GetObjectView().AssertTitleIs(title);
        }
        #endregion
    }
}