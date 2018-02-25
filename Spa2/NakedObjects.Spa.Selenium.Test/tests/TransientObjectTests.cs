// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class TransientObjectTestsRoot : AWTest {
        public virtual void CreateAndSaveTransientObject() {
            Debug.WriteLine(nameof(CreateAndSaveTransientObject));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            wait.Until(d => d.FindElements(By.CssSelector("select#cardtype1 option")).First(el => el.Text == "*"));
            SelectDropDownOnField("#cardtype1", "Vista");
            string number = DateTime.Now.Ticks.ToString(); //pseudo-random string
            var obfuscated = number.Substring(number.Length - 4).PadLeft(number.Length, '*');
            ClearFieldThenType("#cardnumber1", number);
            SelectDropDownOnField("#expmonth1", "12");
            SelectDropDownOnField("#expyear1", "2020");
            Click(SaveButton());
            WaitForView(Pane.Single, PaneType.Object, obfuscated);
        }

        public virtual void SaveAndClose() {
            Debug.WriteLine(nameof(SaveAndClose));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype1", "Vista");
            string number = DateTime.Now.Ticks.ToString(); //pseudo-random string
            var obfuscated = number.Substring(number.Length - 4).PadLeft(number.Length, '*');
            ClearFieldThenType("#cardnumber1", number);
            SelectDropDownOnField("#expmonth1", "12");
            SelectDropDownOnField("#expyear1", "2020");
            Click(SaveAndCloseButton());
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
            //But check that credit card was saved nonetheless
            GetObjectEnabledAction("Recent Credit Cards").Click();
            WaitForView(Pane.Single, PaneType.List, "Recent Credit Cards");
            wait.Until(dr => dr.FindElements(By.CssSelector(".list table tbody tr td.reference")).First().Text == obfuscated);
        }

        public virtual void MissingMandatoryFieldsNotified() {
            Debug.WriteLine(nameof(MissingMandatoryFieldsNotified));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype1", "Vista");
            SelectDropDownOnField("#expyear1", "2020");
            SaveButton().AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Card Number; Exp Month; ");
        }

        public virtual void IndividualFieldValidation() {
            Debug.WriteLine(nameof(IndividualFieldValidation));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype1", "Vista");
            ClearFieldThenType("input#cardnumber1", "123");
            SelectDropDownOnField("#expmonth1", "1");
            SelectDropDownOnField("#expyear1", "2020");
            Click(SaveButton());
            wait.Until(dr => dr.FindElements(
                By.CssSelector(".validation")).Any(el => el.Text == "card number too short"));
            WaitForMessage("See field validation message(s).");
        }

        public virtual void MultiFieldValidation() {
            Debug.WriteLine(nameof(MultiFieldValidation));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            SelectDropDownOnField("#cardtype1", "Vista");
            ClearFieldThenType("#cardnumber1", "1111222233334444");
            SelectDropDownOnField("#expmonth1", "1");
            SelectDropDownOnField("#expyear1", "2008");
            Click(SaveButton());
            WaitForMessage("Expiry date must be in the future");
        }

        public virtual void PropertyDescriptionAndRequiredRenderedAsPlaceholder() {
            Debug.WriteLine(nameof(PropertyDescriptionAndRequiredRenderedAsPlaceholder));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            var name = WaitForCss("input#cardnumber1");
            Assert.AreEqual("* Without spaces", name.GetAttribute("placeholder"));
        }

        public virtual void CancelTransientObject() {
            Debug.WriteLine(nameof(CancelTransientObject));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            Click(GetCancelEditButton());
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
        }

        public virtual void SwapPanesWithTransients() {
            Debug.WriteLine(nameof(SwapPanesWithTransients));
            GeminiUrl("object/object?o1=___1.Product--738&as1=open&o2=___1.Person--20774&as2=open");
            WaitForView(Pane.Left, PaneType.Object, "LL Road Frame - Black, 52");
            WaitForView(Pane.Right, PaneType.Object, "Isabella Richardson");

            OpenSubMenu("Work Orders", Pane.Left);
            Click(GetObjectEnabledAction("Create New Work Order", Pane.Left));
            WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Work Order");
            ClearFieldThenType("#orderqty1", "4");
            Thread.Sleep(1000);
            Click(GetObjectEnabledAction("Create New Credit Card", Pane.Right));
            WaitForView(Pane.Right, PaneType.Object, "Editing - Unsaved Credit Card");
            ClearFieldThenType("#cardnumber2", "1111222233334444");

            Click(SwapIcon());
            WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Credit Card");
            wait.Until(dr => dr.FindElement(By.CssSelector("#cardnumber1")).GetAttribute("value") == "1111222233334444");
            WaitForView(Pane.Right, PaneType.Object, "Editing - Unsaved Work Order");
            wait.Until(dr => dr.FindElement(By.CssSelector("#orderqty2")).GetAttribute("value") == "4");
        }

        public virtual void BackAndForwardOverTransient() {
            Debug.WriteLine(nameof(BackAndForwardOverTransient));
            GeminiUrl("object?o1=___1.Person--12043&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
            Click(GetObjectEnabledAction("Create New Credit Card"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Credit Card");
            Click(br.FindElement(By.CssSelector(".icon.back")));
            WaitForView(Pane.Single, PaneType.Object, "Arthur Wilson");
            Click(br.FindElement(By.CssSelector(".icon.forward")));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Credit Card");
        }

        public virtual void RequestForExpiredTransient() {
            Debug.WriteLine(nameof(RequestForExpiredTransient));
            GeminiUrl("object?i1=Transient&o1=___1.CreditCard--100");
            wait.Until(dr => dr.FindElement(By.CssSelector(".title")).Text == "The requested view of unsaved object details has expired.");
        }

        public virtual void ConditionalChoicesOnTransient() {
            Debug.WriteLine(nameof(ConditionalChoicesOnTransient));
            GeminiUrl("home?m1=ProductRepository");
            WaitForView(Pane.Single, PaneType.Home);
            Thread.Sleep(1000); // no idea why this keeps failing on server 
            Click(GetObjectEnabledAction("New Product"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Product");

            var sellStartDate = br.FindElement(By.CssSelector("#sellstartdate1"));
            Assert.AreEqual("* ", sellStartDate.GetAttribute("placeholder"));

            // set product category and sub category
            SelectDropDownOnField("#productcategory1", "Clothing");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Any(el => el.Text == "Bib-Shorts"));

            SelectDropDownOnField("#productsubcategory1", "Bib-Shorts");

            SelectDropDownOnField("#productcategory1", "Bikes");

            wait.Until(d => d.FindElements(By.CssSelector("select#productsubcategory1 option")).Count == 4);

            SelectDropDownOnField("#productsubcategory1", "Mountain Bikes");
        }

        public virtual void TransientWithHiddenNonOptionalFields() {
            Debug.WriteLine(nameof(TransientWithHiddenNonOptionalFields));
            GeminiUrl("object?i1=View&o1=___1.Product--380&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Hex Nut 8");
            Click(GetObjectEnabledAction("Create New Work Order", Pane.Single, "Work Orders"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
            ClearFieldThenType("#scrappedqty1", "1");
            ClearFieldThenType("#orderqty1", "1");
            SaveObject();
        }

        //Test for a previous bug  -  where Etag error was resulting
        public virtual void CanInvokeActionOnASavedTransient() {
            Debug.WriteLine(nameof(CanInvokeActionOnASavedTransient));
            GeminiUrl("object?i1=View&o1=___1.Customer--11783&as1=open&d1=CreateNewOrder&f1_copyHeaderFromLastOrder=true");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
            Click(SaveButton());
            OpenObjectActions();
            OpenActionDialog("Add New Sales Reasons");
            SelectDropDownOnField("#reasons1", 1);
            Click(OKButton());
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[1].Text == "Reasons:\r\n1 Item");
        }

        public virtual void TransientCreatedFromDialogClosesDialog() {
            Debug.WriteLine(nameof(TransientCreatedFromDialogClosesDialog));
            GeminiUrl("object?i1=View&o1=___1.Customer--30107&as1=open");
            OpenSubMenu("Orders");
            OpenActionDialog("Create New Order");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
            SaveObject();
            ClickBackButton();
            WaitForTextEquals(".title", "The requested view of unsaved object details has expired.");
            ClickBackButton();
            WaitForView(Pane.Single, PaneType.Object, "Permanent Finish Products, AW00030107");
            OpenSubMenu("Orders"); //Would fail if already open
        }

        public virtual void CreateAndSaveNotPersistedObject() {
            Debug.WriteLine(nameof(CreateAndSaveNotPersistedObject));
            GeminiUrl("home?m1=EmployeeRepository");
            Click(GetObjectEnabledAction("Create Staff Summary"));
            WaitForView(Pane.Single, PaneType.Object, "Staff Summary");
            // todo fix once type is no longer displayed
            WaitForTextStarting(".object", "Staff Summary\r\nFemale"); //i.e. no buttons in the header
        }

        public virtual void ValuePropOnTransientEmptyIfNoDefault() {
            Debug.WriteLine(nameof(ValuePropOnTransientEmptyIfNoDefault));
            GeminiUrl("object?i1=View&o1=___1.Product--497&as1=open");
            OpenSubMenu("Work Orders");
            Click(GetObjectEnabledAction("Create New Work Order"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
            var field = WaitForCss("#orderqty1");
            Assert.AreEqual("", field.GetAttribute("value"));
        }

        //Test written against a specific failure scenario
        public virtual void InvalidPropOnTransientClearedAndReentered() {
            Debug.WriteLine(nameof(InvalidPropOnTransientClearedAndReentered));
            GeminiUrl("object?i1=View&o1=___1.Product--497&as1=open");
            OpenSubMenu("Work Orders");
            Click(GetObjectEnabledAction("Create New Work Order"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
            ClearFieldThenType("#scrappedqty1", "0");
            ClearFieldThenType("#orderqty1", "0");
            Click(SaveButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".validation"))
                .Any(el => el.Text == "Order Quantity must be > 0"));
            ClearFieldThenType("#orderqty1", "1");
            Click(SaveButton());
            WaitForTextStarting(".title", "Pinch Bolt");
        }

        public virtual void AutoCompletePropOnTransient() {
            Debug.WriteLine(nameof(AutoCompletePropOnTransient));
            GeminiUrl("object?i1=View&o1=___1.Customer--635&as1=open&d1=CreateNewOrder");
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Sales Order");
            wait.Until(dr => dr.FindElement(By.CssSelector("#salesperson1")).GetAttribute("placeholder") == "(auto-complete or drop)");
            ClearFieldThenType("#salesperson1", "Va");

            // nof custom 
            wait.Until(d => d.FindElements(By.CssSelector(".suggestions a")).Count > 0);

            // anagular/material
            //wait.Until(d => d.FindElements(By.CssSelector("md-option")).Count > 0);
        }

        // test for bug #104
        public virtual void TransientWithHiddenUntilPersistedFields() {
            Debug.WriteLine(nameof(TransientWithHiddenNonOptionalFields));
            GeminiUrl("object?i1=View&o1=___1.Product--390&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Hex Nut 20");
            Click(GetObjectEnabledAction("Create New Work Order", Pane.Single, "Work Orders"));
            WaitForView(Pane.Single, PaneType.Object, "Editing - Unsaved Work Order");
            ClearFieldThenType("#scrappedqty1", "1");
            ClearFieldThenType("#orderqty1", "1");

            // no end date or routings 
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-edit-property .name"))[6].Text.StartsWith("Due Date:"));
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-collection .name")).Count == 0);

            SaveObject();

            // visible end date and routings
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name"))[6].Text == "End Date:");
            wait.Until(dr => dr.FindElement(By.CssSelector("nof-collection .name")).Text == "Work Order Routings:");
            // visible add routing action
            OpenObjectActions();
            GetObjectEnabledAction("Add New Routing");
        }

        // test for bug #128
        public virtual void PersistentWithHiddenUntilPersistedFields() {
            Debug.WriteLine(nameof(TransientWithHiddenNonOptionalFields));
            GeminiUrl("object?i1=View&o1=___1.Product--390&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Hex Nut 20");

            Click(GetObjectEnabledAction("Create New Work Order3", Pane.Single, "Work Orders"));
            ClearFieldThenType("#orderqty1", "1");
            Click(OKButton());

            // visible end date and routings
            wait.Until(dr => dr.FindElements(By.CssSelector("nof-view-property .name"))[6].Text == "End Date:");
            wait.Until(dr => dr.FindElement(By.CssSelector("nof-collection .name")).Text == "Work Order Routings:");
            // visible add routing action
            OpenObjectActions();
            GetObjectEnabledAction("Add New Routing");
        }

        // test for bug #137
        public virtual void TransientWithOtherPaneChanges() {
            Debug.WriteLine(nameof(TransientWithOtherPaneChanges));
            GeminiUrl("home/home");

            WaitForCss("#pane1 nof-menu-bar nof-action input", MainMenusCount);
            WaitForCss("#pane2 nof-menu-bar nof-action input", MainMenusCount);

            OpenMenu("Purchase Orders", Pane.Left);
            Click(GetObjectEnabledAction("Create New Purchase Order2", Pane.Left));
            WaitForView(Pane.Left, PaneType.Object, "Editing - Unsaved Purchase Order Header");
                
            OpenMenu("Vendors", Pane.Right);
            Click(GetObjectEnabledAction("Random Vendor", Pane.Right));
            WaitForView(Pane.Right, PaneType.Object);

            WaitForCss("#pane1 [value='Save']");

            SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Vendor; Order Placed By; Ship Date; ");

            var title = WaitForCss("#pane2 .header .title");
            title.Click();
            CopyToClipboard(title);

            PasteIntoReferenceField("#vendor1");

            ClickBackButton();
            WaitForView(Pane.Right, PaneType.Home);
            WaitForCss("#pane1 [value='Save']");

            SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Order Placed By; Ship Date; ");

            OpenMenu("Employees", Pane.Right);
            Click(GetObjectEnabledAction("Random Employee", Pane.Right));
            WaitForView(Pane.Right, PaneType.Object);

            var title1 = WaitForCss("#pane2 .header .title");
            title1.Click();
            CopyToClipboard(title1);

            PasteIntoReferenceField("#orderplacedby1");

            WaitForCss("#pane1 [value='Save']");
            SaveButton(Pane.Left).AssertIsDisabled().AssertHasTooltip("Missing mandatory fields: Ship Date; ");

            var today = DateTime.Today;
            ClearFieldThenType("#shipdate1", today.ToString("dd/MM/yyyy"));
            WaitForCss("#pane1 [value='Save']");

            WaitUntilEnabled(SaveButton(Pane.Left));
            SaveButton(Pane.Left).AssertIsEnabled().AssertHasTooltip("");

            Click(SaveButton(Pane.Left));
            var date = today.ToString("M/d/yyyy") + " 12:00:00 AM";
            WaitForView(Pane.Left, PaneType.Object, date);
        }
    }

    #region Mega tests

    public abstract class MegaTransientObjectTestsRoot : TransientObjectTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void TransientObjectTests() {
            CreateAndSaveTransientObject();
            SaveAndClose();
            MissingMandatoryFieldsNotified();
            IndividualFieldValidation();
            MultiFieldValidation();
            PropertyDescriptionAndRequiredRenderedAsPlaceholder();
            CancelTransientObject();
            BackAndForwardOverTransient();
            RequestForExpiredTransient();
            TransientWithHiddenNonOptionalFields();
            CanInvokeActionOnASavedTransient();
            TransientCreatedFromDialogClosesDialog();
            CreateAndSaveNotPersistedObject();
            ValuePropOnTransientEmptyIfNoDefault();
            InvalidPropOnTransientClearedAndReentered();
            AutoCompletePropOnTransient();
            TransientWithHiddenUntilPersistedFields();
            PersistentWithHiddenUntilPersistedFields();
            TransientWithOtherPaneChanges();
        }

        [TestMethod]
        [Priority(-1)]
        public void ProblematicTransientObjectTests() {
            ConditionalChoicesOnTransient();
            SwapPanesWithTransients();
        }
    }

    //[TestClass]
    public class MegaTransientObjectTestsFirefox : MegaTransientObjectTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }

    //[TestClass]
    public class MegaTransientObjectTestsIe : MegaTransientObjectTestsRoot {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }

    [TestClass] //toggle
    public class MegaTransientObjectTestsChrome : MegaTransientObjectTestsRoot {
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
    }

    #endregion
}