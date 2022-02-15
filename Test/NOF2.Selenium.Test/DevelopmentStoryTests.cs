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

namespace NOF2.Selenium.Test
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
            PropertyControl();
            CreatingAndSavingObjects();
            ActionAboutControl();
            ParameterControl();
            AddInformationAndWarningMessages();
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
            coll.ClickTableView().GetRowFromTable(1).AssertColumnValueIs(3, "£32.60"); //TODO should become €32.60
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
        public void FieldOrderSpecifiedByAttribute()
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
            SharedActionWithContainerAsParamater();
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

        //[TestMethod]
        public void SharedActionWithContainerAsParamater()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Contacts")
               .GetActionWithDialog("Find Person By Name").Open();
            dialog.GetTextField("First Name").Enter("a");
            dialog.GetTextField("Last Name").Enter("c");
            dialog.ClickOKToViewNewList().GetRowFromList(0).AssertTitleIs("Albert Cabello");
        }
        #endregion

        #region Editing objects
        //[TestMethod]
        public void EditingObjects()
        {
            EditAndCancelWithoutModification();
            EditAndSaveChange();
            MakeEditablePropertyRequiredViaAttribute();
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

        //[TestMethod] 
        public void MakeEditablePropertyRequiredViaAttribute()
        {
            var edit = AccessInstanceWithTitle("Department--7", "Production").Edit();
            edit.AssertSaveIsEnabled();
            edit.GetEditableTextInputProperty("Name").Clear().AssertHasPlaceholder("* ");
            edit.AssertSaveIsDisabled("Missing mandatory fields: Name; ");
            edit.GetEditableTextInputProperty("Group Name").Clear().AssertHasPlaceholder("* ");
            edit.AssertSaveIsDisabled("Missing mandatory fields: Name; Group Name; ");
            edit.Cancel();
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
                "All Employees", "Find Employee By Name", "Find Employee By National ID Number", "Me", "Create New Job Candidate")
                .AssertHasSubMenus("Organisation").OpenSubMenu("Organisation").AssertHasAction("List All Departments");

        }

        //[TestMethod]
        public void ObjectActionsMenu()
        {
            AccessInstanceWithTitle("Product--897", "LL Touring Frame - Blue, 58")
                .OpenActions().AssertHasActions("Best Special Offer", "Associate With Special Offer", "Change Subcategory")
                .AssertHasSubMenus("Work Orders").OpenSubMenu("Work Orders").AssertHasAction(
                "Current Work Orders").AssertHasAction("Create New Work Order");
        }

        #endregion

        #region PropertyControl_UsingFieldAbout
        //[TestMethod]
        public void PropertyControl()
        {
            PropertyHiddenUsingFieldAbout();
            PropertyHiddenUsingAttribute();
            PropertyRenamedUsingFieldAbout();
            PropertyRenamedUsingAttribute();
            PropertyMadeUneditableUsingFieldAbout();
            PropertyValidationUsingFieldAbout();
            TypeImplementingINotEditableOncePersistent();
            TableColumnsSpecifiedViaAttribute();
        }

        //[TestMethod]
        public void PropertyHiddenUsingFieldAbout()
        {
            var obj = AccessInstanceWithTitle("PhoneNumberType--1", "Cell");
            obj.AssertPropertiesAre(); //Because all properties have been hidden individually using FieldAbout
        }


        //[TestMethod]
        public void PropertyHiddenUsingAttribute()
        {
            var obj = AccessInstanceWithTitle("ContactType--1", "Accounting Manager");
            obj.AssertPropertiesAre("Modified Date"); //Because Name property hidden by Attribute

        }

        //[TestMethod]
        public void PropertyRenamedUsingAttribute()
        {
            AccessInstanceWithTitle("EmailAddress--9--9", "gigi0@adventure-works.com")
                .GetProperty(0).AssertNameIs("Email Address");
        }

        //[TestMethod]
        public void PropertyRenamedUsingFieldAbout()
        {
            AccessInstanceWithTitle("Person--115", "Angela Barbariol")
                .GetProperty(4).AssertNameIs("Reverse name order");
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

        //[TestMethod]
        public void TypeImplementingINotEditableOncePersistent()
        {
            AccessInstanceWithTitle("Vendor--1660", "Magic Cycles").AssertIsNotEditable();
        }

        //[TestMethod]
        public void PropertyMadeVisibleOnlyWhenPersistent()
        {
            helper.GotoHome().OpenMainMenu("Special Offers")
            .GetActionWithoutDialog("Create New Special Offer").ClickToViewTransientObject()
            .AssertPropertiesAre("Description", "Discount Pct", "Type", "Category",
                "Start Date", "End Date", "Min Qty", "Max Qty"); //i.e. no Modified Date
            AccessInstanceWithTitle("SpecialOffer--2", "Volume Discount 11 to 14")
                .AssertPropertiesAre("Description", "Discount Pct", "Type", "Category",
                "Start Date", "End Date", "Min Qty", "Max Qty", "Modified Date");
        }

        //[TestMethod]
        public void TableColumnsSpecifiedViaAttribute()
        {
            AccessInstanceWithTitle("Employee--187&c1_DepartmentHistory=Table", "Yvonne McKay")
                .GetCollection("Department History").AssertIsOpenAsTable()
                .AssertTableHeaderHasColumns("", "Start Date", "End Date", "Department", "Shift");
            //First column title being empty indicates that the object title is shown as a column
        }

        #endregion

        #region Creating & Saving objects
        //[TestMethod]
        public void CreatingAndSavingObjects()
        {
            CreateAndSaveObjectProgrammatically();
            //Too unreliable on server
            //DisplayingAndSavingATransientObjectFromTheUI(); 
            //ControlOverSaving();
            PersistingALinkObjectToFormAMultipleAssociation();
            SavingATransientWithAReferenceToAPersistentObject();
        }

        //[TestMethod]
        public void CreateAndSaveObjectProgrammatically()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Employees").OpenSubMenu("Organisation")
                .GetActionWithDialog("Create New Department").Open();

            int rnd = (new Random()).Next(1, 100000);
            string name = $"Dept. {rnd}";
            dialog.GetTextField("Name").Enter(name);
            dialog.GetTextField("Group Name").Enter("Testing");
            dialog.ClickOKToViewObject().AssertTitleIs($"Dept. {rnd}");

            helper.GotoHome().OpenMainMenu("Employees").OpenSubMenu("Organisation")
                .GetActionWithoutDialog("List New Departments").ClickToViewList()
                .GetRowFromList(0).AssertTitleIs(name);
        }

        //[TestMethod]
        public void DisplayingAndSavingATransientObjectFromTheUI()
        {
            var transient = helper.GotoHome().OpenMainMenu("Special Offers")
              .GetActionWithoutDialog("Create New Special Offer").ClickToViewTransientObject();
            transient.AssertTitleIs("Editing - Unsaved Special Offer");
            //Moved these 2 fields up to give more time
            transient.GetEditableTextInputProperty("Start Date").Clear().Enter(DateTime.Today.ToString("d"));
            transient.GetEditableTextInputProperty("End Date").Clear().Enter(DateTime.Today.ToString("d"));
            var rnd = new Random().Next(1, 10000);
            var desc = $"Sale {rnd}";
            transient.GetEditableTextInputProperty("Description").Enter(desc);
            transient.GetEditableTextInputProperty("Discount Pct").Clear().Enter("0.5");
            transient.GetEditableTextInputProperty("Type").Enter("A");
            transient.GetEditableSelectionProperty("Category").Select(1);
            transient.GetEditableTextInputProperty("Min Qty").Clear().Enter("1");
            var result = transient.Save();
            helper.GotoHome().OpenMainMenu("Special Offers")
             .GetActionWithoutDialog("Recently Updated Special Offers").ClickToViewList()
             .GetRowFromList(0).AssertTitleIs(desc);
        }

        //[TestMethod]
        public void ControlOverSaving()
        {
            var transient = helper.GotoHome().OpenMainMenu("Special Offers")
              .GetActionWithoutDialog("Create New Special Offer").ClickToViewTransientObject();
            transient.AssertTitleIs("Editing - Unsaved Special Offer");
            //Test that individual property validation (via AboutProperty) are applied first
            transient.AttemptUnsuccessfulSave();
            var rnd = new Random().Next(1, 10000);
            var desc = $"Sale {rnd}";
            transient.GetEditableTextInputProperty("Description")
                    .AssertHasValidationError("Cannot be empty").Enter(desc);
            transient.GetEditableTextInputProperty("Discount Pct").Clear().Enter("5");
            transient.GetEditableTextInputProperty("Type")
                .AssertHasValidationError("Cannot be empty").Enter("A");
            transient.GetEditableSelectionProperty("Category").Select(1);
            transient.GetEditableTextInputProperty("Start Date").Clear().Enter(DateTime.Today.ToString("d"));
            transient.GetEditableTextInputProperty("End Date").Clear().Enter(DateTime.Today.ToString("d"));
            transient.GetEditableTextInputProperty("Min Qty").Clear().Enter("10");
            transient.GetEditableTextInputProperty("Max Qty").Clear().Enter("5");
            //Test that vaidation rules implemente in AboutActionSave are applied next.
            transient.AttemptUnsuccessfulSave();
            transient.WaitForMessage("Max Qty cannot be less than Min Qty");
            transient.GetEditableTextInputProperty("Max Qty").Clear().Enter("20");

            transient.Save().AssertTitleIs(desc);
        }

        //[TestMethod]
        public void PersistingALinkObjectToFormAMultipleAssociation()
        {
            var dialog = helper.GotoHome().OpenMainMenu("Special Offers")
                  .GetActionWithDialog("Demo Special Offer Product Bug").Open();
            dialog.GetTextField("Special Offer Id").Enter("15");
            var rnd = new Random().Next(720, 999);
            dialog.GetTextField("Product Id").Enter(rnd.ToString());
            var sop = dialog.ClickOKToViewObject().AssertTitleIs($"SpecialOfferProduct: 15-{rnd}");
            var prodTitle = sop.GetProperty("Product").GetReference().GetTitle();
            AccessInstanceWithTitle("SpecialOffer--15", "Half-Price Pedal Sale")
                .OpenActions().GetActionWithoutDialog("Products Covered").ClickToViewList()
                .GetRowFromList(0).AssertTitleIs(prodTitle);
        }

        //[TestMethod]
        public void SavingATransientWithAReferenceToAPersistentObject()
        {
            var start = helper.GotoUrlDirectly("home/object?m1=Employees&i2=View&o2=AW.Types.Employee--240");
            var jc = helper.GetHomeView(Pane.Left).OpenMainMenu("Employees").GetActionWithoutDialog("Create New Job Candidate").ClickToViewTransientObject();
            var emp = helper.GetObjectView(Pane.Right);
            var empField = jc.GetEditableReferenceProperty("Employee");
                emp.DragTitleAndDropOnto(empField);
            var saved = jc.Save(Pane.Left);
            saved.GetProperty("Employee").GetReference().AssertTitleIs("Willis Johnson");
        }

        #endregion

        #region ActionAbout control
        //[TestMethod]
        public void ActionAboutControl()
        {
            ActionVisibility();
            ActionNameAndDescription();
            ActionUsabilityBasedOnObjectState();
            ActionParameterValidation();
            EnforcingMandatoryParameters();
        }

        //[TestMethod]
        public void ActionVisibility()
        {
            AccessInstanceWithTitle("SalesOrderHeader--43660", "SO43660")
                .OpenActions().AssertHasActions("Add New Detail",
                "Add Comment", "Clear Comments", "Remove Detail");
            //i.e. no action called No Comment
        }

        //[TestMethod]
        public void ActionNameAndDescription()
        {
            AccessInstanceWithTitle("SalesOrderHeader--43660", "SO43660")
                .OpenActions().GetActionWithDialog("Add Comment")
            //because actual method name is ActionAppendComment
            .AssertHasTooltip("Append new comment to any existing");
        }

        //[TestMethod]
        public void ActionUsabilityBasedOnObjectState()
        {
            var order = AccessInstanceWithTitle("SalesOrderHeader--43670", "SO43670");
            order.GetProperty("Comment").AssertValueIs("");
            order.OpenActions().GetActionWithoutDialog("Clear Comments")
               .AssertIsDisabled("Comment field is already clear");
        }

        //[TestMethod]
        public void ActionParameterValidation()
        {
            var dialog = AccessInstanceWithTitle("SalesOrderHeader--43660", "SO43660").OpenActions()
              .GetActionWithDialog("Add Comment").Open();
            dialog.GetTextField("Comment")
                .Enter("Now is the time for all good men to come to the aid of the party");
            dialog.ClickOKWithNoResultExpected();
            Thread.Sleep(300);
            dialog.AssertHasValidationError("Total comment length would exceed 50 chars");
        }

        //[TestMethod]
        public void EnforcingMandatoryParameters()
        {
            //Note that this is also testing #372
            var dialog = helper.GotoHome().OpenMainMenu("Employees")
                .GetActionWithDialog("Find Employee By Name").Open();
            dialog.ClickOKWithNoResultExpected()
                .AssertHasValidationError("Last Name cannot be empty");
            dialog.GetTextField("Last Name").Enter("bradley");
            dialog.ClickOKToViewNewList().GetRowFromList(0).AssertTitleIs("David Bradley");
        }
        #endregion

        #region Parameter Control
        //[TestMethod]
        public void ParameterControl()
        {
            ParameterNaming_Options_Default();
        }

        //[TestMethod]
        public void ParameterNaming_Options_Default()
        {
            var emp = AccessInstanceWithTitle("Employee--66", "Karan Khanna");
            emp.GetProperty("Marital Status").AssertValueIs("S");
            var dialog = emp.OpenActions().GetActionWithDialog("Change Marital Status").Open();
            var field = dialog.GetSelectionField("New Marital Status"); //Param name in code is just 'status'
            field.AssertDefaultValueIs("M")
             .AssertNoOfOptionsIs(2)
             .AssertOptionIs(0, "S")
             .Select(0);
            dialog.ClickOKWithNoResultExpected()
                .AssertHasValidationError("New Status cannot be the same as current");
            field.Select(1);
            var updated = dialog.ClickOKToViewObject();
            updated.GetProperty("Marital Status").AssertValueIs("M");
            dialog = emp.OpenActions().GetActionWithDialog("Change Marital Status").Open();
            field = dialog.GetSelectionField("New Marital Status");
            field.AssertDefaultValueIs("S");
            dialog.ClickOKToViewObject()
            .GetProperty("Marital Status").AssertValueIs("S");
        }

        #endregion

        #region Messages
        //[TestMethod]
        public void AddInformationAndWarningMessages()
        {
            var home = helper.GotoHome();
            home.OpenMainMenu("Employees").GetActionWithoutDialog("Me")
                .ClickExpectingToStayOnHome();
            home.WaitForFooterWarning("Not implemented yet");
            home.WaitForFooterMessage("Please be patient");
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