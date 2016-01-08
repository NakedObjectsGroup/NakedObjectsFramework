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
            WaitForOutput("Too many arguments provided.");

            //Exact match takes precedence over partial match
            CiceroUrl("home?menu1=WorkOrderRepository");
            WaitForOutput("Work Orders menu.");
            EnterCommand("ac Create New Work Order"); //which would also match Create New Work Order2
            WaitForOutput("Work Orders menu. Action dialog: Create New Work Order. Product: empty,");

            //Invoking action (no args) on an object with only one action goes straight to dialog
            CiceroUrl("object?object1=AdventureWorksModel.SpecialOffer-1");
            WaitForOutput("Special Offer: No Discount.");
            EnterCommand("ac");
            WaitForOutput("Special Offer: No Discount. Action dialog: Associate Special Offer With Product. Product: empty,");
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
            WaitForOutput("Too many arguments provided.");
            EnterCommand("forward y");
            WaitForOutput("Too many arguments provided.");
        }
        public virtual void Cancel()
        {
            //Menu dialog
            CiceroUrl("home?menu1=ProductRepository&dialog1=FindProductByName");
            WaitForOutputStartingWith("Products menu. Action dialog: Find Product By Name");
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
            WaitForOutput("Too many arguments provided.");

            //Object dialog
            CiceroUrl("object?object1=AdventureWorksModel.Product-358&dialog1=BestSpecialOffer");
            WaitForOutputStartingWith("Product: HL Grip Tape. Action dialog: Best Special Offer");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape.");
            //Zero param
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29688&dialog1=LastOrder");
            WaitForOutputStartingWith("Customer: Handy Bike Services, AW00029688. Action dialog: Last Order.");
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
        public virtual void Clipboard()
        {
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");


            CiceroUrl("object?object1=AdventureWorksModel.Person-12941");
            WaitForOutput("Person: Dakota Wood.");
            EnterCommand("clipboard cop");
            WaitForOutput("Clipboard contains: Person: Dakota Wood");
            CiceroUrl("object?object1=AdventureWorksModel.Person-12942");
            WaitForOutput("Person: Jaclyn Liang.");
            //Check clipboard still unmodified
            EnterCommand("clipboard sh");
            WaitForOutput("Clipboard contains: Person: Dakota Wood");
            //Now change it
            EnterCommand("clipboard cop");
            WaitForOutput("Clipboard contains: Person: Jaclyn Liang");
            //Discard
            EnterCommand("clipboard d");
            WaitForOutput("Clipboard is empty");

            //Invalid argument
            EnterCommand("clipboard copyx");
            WaitForOutput("Clipboard command may only be followed by copy, show, go, or discard");

            //No argument
            EnterCommand("clipboard");
            WaitForOutput("No arguments provided.");

            //Too many arguments
            EnterCommand("clipboard copy, show");
            WaitForOutput("Too many arguments provided.");

            //Attempt to copy from home
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing and object");
            //Attempt to copy from list
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutput("Current Special Offers: Page 1 of 1 containing 16 of 16 items");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing and object");
        }
        public virtual void Collection()
        {
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-60485");
            WaitForOutput("Sales Order Header: SO60485.");

            //Simple case
            EnterCommand("collection details");
            WaitForOutput("Collection: Details on Sales Order Header: SO60485, 3 items");

            //Multiple matches
            CiceroUrl("object?object1=AdventureWorksModel.Product-901");
            WaitForOutput("Product: LL Touring Frame - Yellow, 54.");
            EnterCommand("coll pr");
            WaitForOutput("Matching collections: Product Inventory, Product Reviews,");

            //No argument
            EnterCommand("co");
            WaitForOutput("Collections: Product Inventory, Product Reviews, Special Offers,");

            //Multi-clause match
            EnterCommand("coll v ory");
            WaitForOutput("Collection: Product Inventory on Product: LL Touring Frame - Yellow, 54, empty");

            //No matches
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-60485");
            WaitForOutput("Sales Order Header: SO60485.");
            EnterCommand("co x son");
            WaitForOutput("x son does not match any collections");

            //Too many arguments
            EnterCommand("co de,tails");
            WaitForOutput("Too many arguments provided.");

            //Invalid context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("co");
            WaitForOutput("The command: collection is not available in the current context");

            //Switching between different collections
            CiceroUrl("object?object1=AdventureWorksModel.Product-901");
            WaitForOutput("Product: LL Touring Frame - Yellow, 54.");
            EnterCommand("coll inventory");
            WaitForOutput("Collection: Product Inventory on Product: LL Touring Frame - Yellow, 54, empty");
            EnterCommand("coll reviews");
            WaitForOutput("Collection: Product Reviews on Product: LL Touring Frame - Yellow, 54, empty");
            EnterCommand("coll spec");
            WaitForOutput("Collection: Special Offers on Product: LL Touring Frame - Yellow, 54, 1 item");
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
            WaitForOutput("Too many arguments provided.");
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
            WaitForOutput("Too many arguments provided.");

            //exact match takes priority over partial match
            EnterCommand("field product category"); //which would also match product subcategory
            WaitForOutput("Product Category: Bikes,");

            //Entering fields (into dialogs)
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522%2522&field1_lastName=%2522%2522");
            WaitForOutput("Customers menu. Action dialog: Find Individual Customer By Name. First Name: empty, Last Name: empty,");
            EnterCommand("field first, a");
            WaitForOutput("Customers menu. Action dialog: Find Individual Customer By Name. First Name: a, Last Name: empty,");
            EnterCommand("field last, b");
            WaitForOutput("Customers menu. Action dialog: Find Individual Customer By Name. First Name: a, Last Name: b,");

            //Todo: test selections
            CiceroUrl("home?menu1=ProductRepository&dialog1=ListProductsBySubCategory");
            WaitForOutput("Products menu. Action dialog: List Products By Sub Category.");
            EnterCommand("field cat, hand");
            WaitForOutput("Products menu. Action dialog: List Products By Sub Category. Sub Category: Handlebars,");
            EnterCommand("field cat, xx");
            WaitForOutput("None of the choices matches xx");
            EnterCommand("field cat, frame");
            WaitForOutput("Multiple matches: Mountain Frames, Road Frames, Touring Frames,");
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
            WaitForOutput("Too many arguments provided.");
        }
        public virtual void Go()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Customer-577");
            WaitForOutput("Customer: Synthetic Materials Manufacturing, AW00000577.");
            //Full match
            EnterCommand("go Details");
            WaitForOutput("Store: Synthetic Materials Manufacturing.");
            //Partial match (on Sales Person)
            EnterCommand("go pers");
            WaitForOutput("Sales Person: Pamela Ansman-Wolfe.");
            //No match
            EnterCommand("go x");
            WaitForOutput("x does not match any reference fields");
            //Matches only value fields
            EnterCommand("go bonus");
            WaitForOutput("bonus does not match any reference fields");

            //Multiple matches
            EnterCommand("go details");
            WaitForOutput("Multiple reference fields match details: Employee DetailsPerson Details");
            //Multiple clause match
            EnterCommand("go details pers"); //Person Details
            WaitForOutput("Person: Pamela Ansman-Wolfe.");
            //Wrong no of args
            EnterCommand("go");
            WaitForOutput("No arguments provided.");
            EnterCommand("go x,y");
            WaitForOutput("Too many arguments provided.");
            //Now try for a list context
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Current Special Offers: Page 1 of 1 containing 16 of 16 items");
            EnterCommand("go 1");
            WaitForOutput("Special Offer: No Discount.");
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Current Special Offers: Page 1 of 1 containing 16 of 16 items");
            EnterCommand("go 16");
            WaitForOutput("Special Offer: Mountain-500 Silver Clearance Sale.");
            //Try out of range
            CiceroUrl("list?menu1=SpecialOfferRepository&dialog1=CurrentSpecialOffers&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            WaitForOutput("Current Special Offers: Page 1 of 1 containing 16 of 16 items");
            EnterCommand("go 0");
            WaitForOutput("0 is out of range for displayed items");
            EnterCommand("go 17");
            WaitForOutput("17 is out of range for displayed items");
            EnterCommand("go x");
            WaitForOutput("x is not a valid number");
            EnterCommand("go 1x"); //Because of behaviour of tryParse
            WaitForOutput("Special Offer: No Discount.");
            //Wrong context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("go x");
            WaitForOutput("The command: go is not available in the current context");
            CiceroUrl("home?menu1=ProductRepository&dialog1=RandomProduct");
            WaitForOutput("Products menu. Action dialog: Random Product.");
            EnterCommand("go x");
            WaitForOutput("The command: go is not available in the current context");
            
        }
        public virtual void Help()
        {
            //Help from home
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutput("Commands available in current context: back, clipboard, forward, gemini, help, menu, where,");
            //Now try an object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            //First with no params
            EnterCommand("help");
            WaitForOutput("Commands available in current context: action, back, collection, clipboard, edit, field, forward, gemini, go, help, menu, reload, select, where,");
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
            WaitForOutput("Too many arguments provided.");
        }
        public virtual void Item()
        {
            //Applied to List
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutput("Current Special Offers: Page 1 of 1 containing 16 of 16 items");
            EnterCommand("item 1");
            WaitForOutput("Item 1: No Discount;");
            EnterCommand("item 16");
            WaitForOutput("Item 16: Mountain-500 Silver Clearance Sale;");

            EnterCommand("item 2,4");
            WaitForOutput("Item 2: Volume Discount 11 to 14; Item 3: Volume Discount 15 to 24; Item 4: Volume Discount 25 to 40;");
            EnterCommand("item 5,5");
            WaitForOutput("Item 5: Volume Discount 41 to 60;");

            //Invalid numbers
            EnterCommand("item 17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("item 0");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("item -1");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("item 15,17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("item 0,3");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("item 5,4");
            WaitForOutput("Starting item number cannot be greater than the ending item number");

            //Applied to collection
            CiceroUrl("object?object1=AdventureWorksModel.SalesOrderHeader-44518&selected1=256&collection1_Details=List");
            WaitForOutput("Collection: Details on Sales Order Header: SO44518, 20 items");
            EnterCommand("item 1");
            WaitForOutput("Item 1: 5 x Mountain-100 Black, 44;");
            EnterCommand("item 20");
            WaitForOutput("Item 20: 2 x HL Mountain Frame - Black, 38;");

            //No number
            EnterCommand("item");
            WaitForOutputStartingWith("Item 1: 5 x Mountain-100 Black, 44; Item 2: 3 x Sport-100 Helmet, Black; Item 3:");

            //Too many parms
            EnterCommand("item 4,5,6");
            WaitForOutput("Too many arguments provided.");

            //Alpha parm
            EnterCommand("item one");
            WaitForOutput("Argument number 1 must be a number");

            //Invalid context
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("item 1");
            WaitForOutput("The command: item is not available in the current context");
            CiceroUrl("home?menu1=CustomerRepository");
            WaitForOutput("Customers menu.");
            EnterCommand("item 1");
            WaitForOutput("The command: item is not available in the current context");
            CiceroUrl("object?object1=AdventureWorksModel.Customer-29863");
            WaitForOutput("Customer: Efficient Cycling, AW00029863.");
            EnterCommand("item 1");
            WaitForOutput("The command: item is not available in the current context");

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
            WaitForOutput("Too many arguments provided.");
            //Invoked in another context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            EnterCommand("Menu");
            WaitForOutput("Menus: Customers, Orders, Products, Employees, Sales, Special Offers, Contacts, Vendors, Purchase Orders, Work Orders,");

            //Test for exact match taking precedence over partial matches
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("menu orders"); //which would match 3, but one exactly
            WaitForOutput("Orders menu.");

            //Invoking menu from a non-home context, clears state
            CiceroUrl("list?menu1=SpecialOfferRepository&action1=CurrentSpecialOffers");
            WaitForOutputStartingWith("Current Special Offers: Page 1");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu.");
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
            WaitForOutput("Too many arguments provided.");

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

            //Menu action that returns a List
            CiceroUrl("home?menu1=OrderRepository&dialog1=HighestValueOrders");
            WaitForOutput("Orders menu. Action dialog: Highest Value Orders.");
            EnterCommand("ok");
            WaitForOutputStartingWith("Highest Value Orders: Page 1 of ");

            //Menu action with params
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522a%2522&field1_lastName=%2522b%2522");
            WaitForOutput("Customers menu. Action dialog: Find Individual Customer By Name. First Name: a, Last Name: b,");
            EnterCommand("ok");
            WaitForOutputStartingWith("Find Individual Customer By Name: Page 1 of 8 containing 20 of");

            //Menu action with missing mandatory params
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindIndividualCustomerByName&field1_firstName=%2522a%2522&field1_lastName=%2522%2522");
            WaitForOutput("Customers menu. Action dialog: Find Individual Customer By Name. First Name: a, Last Name: empty,");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields: Last Name: required,");

            //Menu action with invalid entry
            CiceroUrl("home?menu1=CustomerRepository&dialog1=FindCustomerByAccountNumber&field1_accountNumber=%252212345%2522");
            WaitForOutput("Customers menu. Action dialog: Find Customer By Account Number. Account Number: 12345,");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields: Account Number: 12345 Account number must start with AW,");

            //Menu action with select param
            CiceroUrl("home?menu1=ProductRepository&dialog1=ListProductsBySubCategory&field1_subCategory=%257B%2522href%2522%253A%2522http%253A%252F%252Flocalhost%253A61546%252Fobjects%252FAdventureWorksModel.ProductSubcategory%252F10%2522%252C%2522title%2522%253A%2522Forks%2522%257D");
            WaitForOutput("Products menu. Action dialog: List Products By Sub Category. Sub Category: Forks,");
            EnterCommand("ok");
            WaitForOutputStartingWith("List Products By Sub Category: Page 1 of 1 containing 3 of 3 items");
        }
        public virtual void Root()
        {
            CiceroUrl("object?object1=AdventureWorksModel.Product-459&collection1_ProductInventory=List");
            WaitForOutput("Collection: Product Inventory on Product: Lock Nut 19, 3 items");
            EnterCommand("root");
            WaitForOutput("Product: Lock Nut 19.");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutput("Welcome to Cicero");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            CiceroUrl("object?object1=AdventureWorksModel.Product-459");
            WaitForOutput("Product: Lock Nut 19.");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            //Argument added
            CiceroUrl("object?object1=AdventureWorksModel.Product-459&collection1_ProductInventory=List");
            WaitForOutput("Collection: Product Inventory on Product: Lock Nut 19, 3 items");
            EnterCommand("root x");
            WaitForOutput("Too many arguments provided.");
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
            ClearFieldThenType("input", Keys.Enter);
            WaitForOutput("Product: HL Grip Tape.");

            //No arguments
            EnterCommand("where x");
            WaitForOutput("Too many arguments provided.");
        }
        public virtual void UpAndDownArrow()
        {
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutputStartingWith("Commands available");
            Assert.AreEqual("", WaitForCss("input").GetAttribute("value"));
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help");
            TypeIntoFieldWithoutClearing("input", " gem" + Keys.Enter);
            WaitForOutputStartingWith("gemini command");
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help gem");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowDown);
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
        public override void Clipboard() { base.Clipboard(); }
        [TestMethod]
        public override void Collection() { base.Collection(); }
        [TestMethod]
        public override void Edit() { base.Edit(); }
        [TestMethod]
        public override void Field() { base.Field(); }
        [TestMethod]
        public override void Gemini() { base.Gemini(); }
        [TestMethod]
        public override void Go() { base.Go(); }
        [TestMethod]
        public override void Help() { base.Help(); }
        [TestMethod]
        public override void Item() { base.Item(); }
        [TestMethod]
        public override void Menu() { base.Menu(); }
        [TestMethod]
        public override void OK() { base.OK(); }
        [TestMethod]
        public override void Root() { base.Root(); }
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

    [TestClass] //Comment out if MegaTest is commented in
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
            base.Clipboard();
            base.Collection();
            base.Edit();
            base.Field();
            base.Gemini();
            base.Go();
            base.Help();
            base.Item();
            base.Menu();
            base.OK();
            base.Root();
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