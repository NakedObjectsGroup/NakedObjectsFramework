// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium
{
    public abstract class CiceroTestRoot : AWTest
    {
        public virtual void Action()
        {
            //Test from home menu
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu.");
            //First, without paramsd
            EnterCommand("Action");
            WaitForOutput("Actions: Find Product By Name, Find Product By Number, List Products By Sub Category, List Products By Sub Categories, Find By Product Line And Class, Find By Product Lines And Classes, Find Product, Find Products By Category, New Product, Random Product, Find Product By Key, Stock Report,");
            //Filtered list
            EnterCommand("act cateGory  ");
            WaitForOutput("Matching actions: List Products By Sub Category, Find Products By Category,");
            //No match
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            //EnterCommand("act foo, bar  ");  //TODO: needs updating when 2nd param is implemented
            //WaitForOutput("Wrong number of arguments provided.");
            //single match
            EnterCommand("act rand");
            WaitForOutput("Products menu. Action dialog: Random Product.");
            //Test from object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-358");
            WaitForOutput("Product: HL Grip Tape.");
            EnterCommand("Action");
            WaitForOutput("Actions: Add Or Change Photo, Best Special Offer, Associate Special Offer With Product, Open Purchase Orders For Product, Create New Work Order, Work Orders,");
            EnterCommand("act ord");
            WaitForOutput("Matching actions: Open Purchase Orders For Product, Create New Work Order, Work Orders,");
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            EnterCommand("ac best");
            WaitForOutput("Product: HL Grip Tape. Action dialog: Best Special Offer. Quantity: empty,");
            //Not available in current context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ac");
            WaitForOutput("The command: action is not available in the current context");
            //multi clause search
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu.");
            EnterCommand("ac name find by");
            WaitForOutput("Matching actions: Stores - Find Store By Name, Individuals - Find Individual Customer By Name,");
            //Matches includes sub-menu
            EnterCommand("ac stores");
            WaitForOutput("Matching actions: Stores - Find Store By Name, Stores - Create New Store Customer, Stores - Random Store,");
            EnterCommand("ac ores d");
            WaitForOutput("Matching actions: Stores - Find Store By Name, Stores - Random Store,");
            EnterCommand("ac ores ran");
            WaitForOutput("Customers menu. Action dialog: Random Store.");
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu.");
            EnterCommand("ac ran individuals"); //Order doesn't matter
            WaitForOutput("Customers menu. Action dialog: Random Individual.");

            //object with no actions
            CiceroUrl("object?object1=AdventureWorksModel.ProductInventory-442-6");
            WaitForOutput("Product Inventory: 524 in Miscellaneous Storage - G.");
            EnterCommand("ac");
            WaitForOutput("No actions available");

            //second arg
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu.");
            EnterCommand("ac name find by, ?");
            WaitForOutput("Second argument for action is not yet supported.");

            //To many args
            EnterCommand("ac name find by, x, y");
            WaitForOutput("Wrong number of arguments provided.");
        }
        public virtual void BackAndForward() //Tested together for simplicity
        {
            CiceroUrl("home");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu.");
            EnterCommand("action random store");
            WaitForOutput("Customers menu. Action dialog: Random Store.");
            EnterCommand("back");
            WaitForOutput("Customers menu.");
            EnterCommand("Ba");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("forward");
            WaitForOutput("Customers menu.");
            EnterCommand("fO  ");
            WaitForOutput("Customers menu. Action dialog: Random Store.");
            //Can't go forward beyond most recent
            EnterCommand("forward");
            WaitForOutput("Customers menu. Action dialog: Random Store.");

            //No arguments
            EnterCommand("back x");
            WaitForOutput("Wrong number of arguments provided.");
            EnterCommand("forward y");
            WaitForOutput("Wrong number of arguments provided.");
        }
        public virtual void Cancel()
        {
            //Menu dialog
            CiceroUrl("home?menu1=ProductRepository&dialog1=FindProductByName");
            WaitForOutput("Products menu. Action dialog: Find Product By Name. Search String");
            EnterCommand("cancel");
            WaitForOutput("Products menu.");
            //Test on a zero param action
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu. Action dialog: Random Product.");
            EnterCommand("cancel");
            WaitForOutput("Products menu.");
            //Try with argument
            CiceroUrl("home?menu1=EmployeeRepository&dialog1=RandomEmployee");
            WaitForOutput("Employees menu. Action dialog: Random Employee.");
            EnterCommand("cancel x");
            WaitForOutput("Wrong number of arguments provided.");

            //Object dialog
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&dialog1=BestSpecialOffer");
            WaitForOutput("Product: HL Grip Tape. Action dialog: Best Special Offer. Quantity");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape.");
            //Zero param
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688. Action dialog: Last Order.");
            EnterCommand("Ca");
            WaitForOutput("Customer: Handy Bike Services, AW00029688.");

            //Cancel of Edits
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&edit1=true");
            WaitForOutput("Editing Product: HL Grip Tape.");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape.");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu.");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688.");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
        }
        public virtual void Edit()
        {
            CiceroUrl("home");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688.");
            EnterCommand("edit");
            WaitForOutput("Editing Customer: Handy Bike Services, AW00029688.");
            //No arguments
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688.");
            EnterCommand("edit x");
            WaitForOutput("Wrong number of arguments provided.");
            //Invalid contexts
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&edit1=true");
            WaitForOutput("Editing Product: HL Grip Tape.");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
        }
        public virtual void Field()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Product-758");
            WaitForOutput("Product: Road-450 Red, 52.");
            EnterCommand("field num");
            WaitForOutput("Product Number: BK-R68R-52,");
            EnterCommand("fi cat");
            WaitForOutput("Product Category: Bikes, Product Subcategory: Road Bikes,");
            //No argument
            EnterCommand("fi ");
            WaitForOutputStartingWith("Name: Road-450 Red, 52, Product Number: BK-R68R-52, Color: Red, Photo: empty, Product Model: Road-450, List Price: 1457.99,");
            //No match
            EnterCommand("fi x");
            WaitForOutput("x does not match any fields");

            //Invalid context
            CiceroUrl("home");
            EnterCommand("field");
            WaitForOutput("The command: field is not available in the current context");

            //Multi-clause match
            CiceroUrl("object?object1=AdventureWorksModel.SalesPerson-284");
            WaitForOutput("Sales Person: Tete Mensa-Annan.");
            EnterCommand("fi sales a");
            WaitForOutput("Sales Territory: Northwest, Sales Quota: 300000, Sales YTD: 1576562.1966, Sales Last Year: 0,");
            EnterCommand("fi ter ory");
            WaitForOutput("Sales Territory: Northwest,");
            EnterCommand("fi sales z");
            WaitForOutput("sales z does not match any fields");

            //No fields
            CiceroUrl("object?object1=AdventureWorksModel.AddressType-2");
            WaitForOutput("Address Type: Home.");
            EnterCommand("field");
            WaitForOutput("No visible fields");

            //With question mark
            CiceroUrl("object?object1=AdventureWorksModel.Product-758");
            WaitForOutput("Product: Road-450 Red, 52.");
            EnterCommand("field num,?");
            WaitForOutput("Field name: Product Number, Value: BK-R68R-52, Type: String, Mandatory");
            
            //To many args
            EnterCommand("field num,x,y");
            WaitForOutput("Wrong number of arguments provided.");

            //Input: TODO
        }
        public virtual void Gemini()
        {
            //home
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Home);

            CiceroUrl("object?object1=AdventureWorksModel.Product-968");
            WaitForOutput("Product: Touring-1000 Blue, 54.");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");

            //No arguments
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ge x");
            WaitForOutput("Wrong number of arguments provided.");
        }
        public virtual void Help()
        {
            //Help from home
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutput("Commands available in current context: " +
                "back, forward, gemini, help, menu, where,");
            //Now try an object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            //First with no params
            EnterCommand("help");
            WaitForOutput("Commands available in current context: action, back, copy, edit, field, forward, gemini, go, help, menu, open, reload, where,");
            //Now with params
            EnterCommand("help me");
            WaitForOutput("menu command: From any context, Menu opens a named main menu. " +
                "This command normally takes one argument: the name, or partial name, " +
                "of the menu. If the partial name matches more than one menu, a list of " +
                "matches will be returned but no menu will be opened; if no argument is " +
                "provided a list of all the menus will be returned.");
            EnterCommand("help menux");
            WaitForOutput("No such command: menux");
            EnterCommand("help menu back");
            WaitForOutput("No such command: menu back");
            EnterCommand("help menu, back");
            WaitForOutput("Wrong number of arguments provided.");
        }
        public virtual void Menu()
        {   //No argument
            CiceroUrl("home");
            EnterCommand("Menu");
            WaitForOutput("Menus: Customers, Orders, Products, Employees, Sales, Special Offers, Contacts, Vendors, Purchase Orders, Work Orders,");
            //Now with arguments
            EnterCommand("Menu Customers");
            WaitForOutput("Customers menu.");
            EnterCommand("Menu pro");
            WaitForOutput("Products menu.");
            EnterCommand("Menu off");
            WaitForOutput("Special Offers menu.");
            EnterCommand("Menu ord");
            WaitForOutput("Matching menus: Orders, Purchase Orders, Work Orders,");
            EnterCommand("Menu foo");
            WaitForOutput("foo does not match any menu");
            EnterCommand("Menu cust prod");
            WaitForOutput("cust prod does not match any menu");
            EnterCommand("Menu cus, ord");
            WaitForOutput("Wrong number of arguments provided.");
            //Invoked in another context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            EnterCommand("Menu");
            WaitForOutput("Menus: Customers, Orders, Products, Employees, Sales, Special Offers, Contacts, Vendors, Purchase Orders, Work Orders,");
        }
        public virtual void OK()
        {
            CiceroUrl("home");
            //Open a zero-param action on main menu
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu. Action dialog: Random Product.");
            EnterCommand("ok");
            WaitForOutputStartingWith("Product: ");
            // No arguments
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu. Action dialog: Random Product.");
            EnterCommand("ok x");
            WaitForOutput("Wrong number of arguments provided.");

            //Object action
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688. Action dialog: Last Order.");
            EnterCommand("ok");
            WaitForOutput("Sales Order Header: SO69562.");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository");
            WaitForOutput("Products menu.");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");
        }
        public virtual void Where()
        {
            CiceroUrl("home");
            CiceroUrl("object?object1=AdventureWorksModel.Product-358");
            WaitForOutput("Product: HL Grip Tape.");
            //Do something to change the output
            EnterCommand("help");
            WaitForOutputStartingWith("Commands");
            EnterCommand("where");
            WaitForOutput("Product: HL Grip Tape.");

            //Empty command == where
            EnterCommand("help");
            WaitForOutputStartingWith("Commands");
            TypeIntoField("input", Keys.Enter);
            WaitForOutput("Product: HL Grip Tape.");

            //No arguments
            EnterCommand("where x");
            WaitForOutput("Wrong number of arguments provided.");
        }
        public virtual void UpAndDownArrow()
        {
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutputStartingWith("Commands available");
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            TypeIntoField("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help");
            TypeIntoField("input", " gem" + Keys.Enter);
            WaitForOutputStartingWith("gemini command");
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            TypeIntoField("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help gem");
            TypeIntoField("input", Keys.ArrowDown);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
        }
    }
    public abstract class CiceroTests : CiceroTestRoot
    {
        [TestMethod]
        public override void Action() { base.Action(); }
        [TestMethod]
        public override void Cancel() { base.Cancel(); }
        [TestMethod]
        public override void Edit() { base.Edit(); }
        [TestMethod]
        public override void Field() { base.Field(); }
        [TestMethod]
        public override void Help() { base.Help(); }
        [TestMethod]
        public override void Menu() { base.Menu(); }
        [TestMethod]
        public override void OK() { base.OK(); }
        [TestMethod]
        public override void Where() { base.Where(); }
        [TestMethod]
        public override void UpAndDownArrow() { base.UpAndDownArrow(); }
    }

    #region Individual tests - browser specific 

    // [TestClass, Ignore]
    public class CiceroTestsIe : CiceroTests
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
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    //[TestClass] //Comment out if MegaTest is commented in
    public class CiceroTestsFirefox : CiceroTests
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

    // [TestClass, Ignore]
    public class CiceroTestsChrome : CiceroTests
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
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    #endregion

    #region Mega tests
    public abstract class CiceroMegaTestRoot : CiceroTestRoot
    {

        [TestMethod]
        public void CiceroMegaTest()
        {
            base.Action();
            base.BackAndForward();
            base.Cancel();
            base.Edit();
            base.Field();
            base.Gemini();
            base.Help();
            base.Menu();
            base.OK();
            base.UpAndDownArrow();
            base.Where();
        }
    }
    [TestClass]
    public class CiceroMegaTestFirefox : CiceroMegaTestRoot
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