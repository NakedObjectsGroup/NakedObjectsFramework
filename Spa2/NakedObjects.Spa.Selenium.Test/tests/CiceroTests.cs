// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class CiceroTestRoot : AWTest {
        public virtual void Action() {
            //Test from home menu
            CiceroUrl("home?m1=ProductRepository");
            WaitForOutput("Products menu");
            //First, without paramsd
            EnterCommand("Action");
            WaitForOutputStarting("Actions:\r\n" +
                                  "Find Product By Name\r\n" +
                                  "Find Product By Number\r\n");

            //Filtered list
            EnterCommand("act cateGory  ");
            WaitForOutput("Matching actions:\r\nList Products By Sub Category\r\nFind Products By Category");
            //No match
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            //single match
            EnterCommand("act rand");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            //Test from object context
            CiceroUrl("object?o1=___1.Product--358");
            WaitForOutput("Product: HL Grip Tape");

            EnterCommand("Action");
            WaitForOutputStarting("Actions:\r\nAdd Or Change Photo\r\nBest Special Offer\r\nSpecial Offers - Associate Special Offer With Product");
            EnterCommand("act ord");
            WaitForOutputStarting("Matching actions:\r\nPurchase Orders - Open Purchase Orders For Product\r\nWork Orders - Create New Work Order\r\n");
            EnterCommand("act foo  ");
            WaitForOutput("foo does not match any actions");
            EnterCommand("ac best");
            WaitForOutput("Product: HL Grip Tape\r\nAction dialog: Best Special Offer\r\nQuantity: empty");
            //Not available in current context
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("ac");
            WaitForOutput("The command: action is not available in the current context");
            //multi clause search
            CiceroUrl("home?m1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("ac name find by");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nIndividuals - Find Individual Customer By Name");
            //Matches includes sub-menu
            EnterCommand("ac stores");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nStores - Create New Store Customer\r\nStores - Random Store");
            EnterCommand("ac ores d");
            WaitForOutput("Matching actions:\r\nStores - Find Store By Name\r\nStores - Random Store");
            EnterCommand("ac ores ran");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");
            CiceroUrl("home?m1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("ac ran individuals"); //Order doesn't matter
            WaitForOutput("Customers menu\r\nAction dialog: Random Individual");

            //object with no actions
            CiceroUrl("object?i1=View&o1=___1.BusinessEntityAddress--19891--28992--2");
            WaitForOutputContaining("Reiherweg 7450");
            EnterCommand("ac");
            WaitForOutput("No actions available");

            //To many args
            EnterCommand("ac name find by, x, y");
            WaitForOutput("Too many arguments provided");

            //Exact match takes precedence over partial match
            CiceroUrl("home?m1=WorkOrderRepository");
            WaitForOutput("Work Orders menu");
            EnterCommand("ac Create New Work Order"); //which would also match Create New Work Order2
            WaitForOutput("Work Orders menu\r\nAction dialog: Create New Work Order\r\nProduct: empty");

            //Invoking action (no args) on an object with only one action goes straight to dialog
            CiceroUrl("object?o1=___1.SpecialOffer--1");
            WaitForOutput("Special Offer: No Discount");
            EnterCommand("ac");
            WaitForOutput("Special Offer: No Discount\r\nAction dialog: Associate Special Offer With Product\r\nProduct: empty");

            //Disabled action - listed as such
            CiceroUrl("object?o1=___1.Vendor--1644");
            WaitForOutput("Vendor: International Sport Assoc.");
            EnterCommand("ac");
            WaitForOutputContaining("Check Credit (disabled: Not yet implemented)");

            //Disabled action -  cannot open dialog
            EnterCommand("ac check credit");
            WaitForOutput("Action: Check Credit is disabled. Not yet implemented");

            //Question mark to get details
            CiceroUrl("home?m1=PurchaseOrderRepository");
            WaitForOutput("Purchase Orders menu");
            EnterCommand("ac random,?");
            WaitForOutput("Description for action: Random Purchase Order\r\n" +
                          "For demonstration purposes only");

            //Invalid second param
            EnterCommand("ac random,x");
            WaitForOutput("Second argument may only be a question mark -  to get action details");
        }

        public virtual void BackAndForward() //Tested together for simplicity
        {
            CiceroUrl("home");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu");
            EnterCommand("action random store");
            WaitForOutput("Customers menu\r\nAction dialog: Random Store");
            EnterCommand("OK");
            WaitForOutputContaining("Customer:");
            EnterCommand("Go store");
            WaitForOutputContaining("Store:");
            EnterCommand("back");
            WaitForOutputContaining("Customer:");
            EnterCommand("back");
            WaitForOutputContaining("Customers menu");
            EnterCommand("forward");
            WaitForOutputContaining("Customer:");
            EnterCommand("fo");
            WaitForOutputContaining("Store:");
            EnterCommand("fo"); //can't go further forward
            WaitForOutputContaining("Store:");

            //No arguments
            EnterCommand("back x");
            WaitForOutput("Too many arguments provided");
            EnterCommand("forward y");
            WaitForOutput("Too many arguments provided");
        }

        public virtual void Cancel() {
            //Menu dialog
            CiceroUrl("home?m1=ProductRepository&d1=FindProductByName");
            WaitForOutputStarting("Products menu\r\nAction dialog: Find Product By Name");
            EnterCommand("cancel");
            WaitForOutput("Products menu");
            //Test on a zero param action
            CiceroUrl("home?m1=ProductRepository&d1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("cancel");
            WaitForOutput("Products menu");
            //Try with argument
            CiceroUrl("home?m1=EmployeeRepository&d1=RandomEmployee");
            WaitForOutput("Employees menu\r\nAction dialog: Random Employee");
            EnterCommand("cancel x");
            WaitForOutput("Too many arguments provided");

            //Object dialog
            CiceroUrl("object?o1=___1.Product--358&d1=BestSpecialOffer");
            WaitForOutputStarting("Product: HL Grip Tape\r\nAction dialog: Best Special Offer");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape");
            //Zero param
            CiceroUrl("object?o1=___1.Customer--29688&d1=LastOrder");
            WaitForOutputStarting("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("Ca");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");

            //Cancel of Edits
            CiceroUrl("object?o1=___1.Product--358&i1=Edit");
            WaitForOutput("Editing Product: HL Grip Tape");
            EnterCommand("cancel");
            WaitForOutput("Product: HL Grip Tape");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("home?m1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
            CiceroUrl("object?o1=___1.Customer--29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("cancel");
            WaitForOutput("The command: cancel is not available in the current context");
        }

        public virtual void Clipboard() {
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");

            CiceroUrl("object?o1=___1.Person--12941");
            WaitForOutput("Person: Dakota Wood");
            CiceroUrl("object?o1=___1.Person--12942");
            WaitForOutput("Person: Jaclyn Liang");

            EnterCommand("back");
            WaitForOutput("Person: Dakota Wood");
            EnterCommand("clipboard cop");
            WaitForOutput("Clipboard contains: Person: Dakota Wood");
            EnterCommand("fo");
            WaitForOutput("Person: Jaclyn Liang");
            //Check clipboard still unmodified
            EnterCommand("clipboard sh");
            //WaitForOutput("Clipboard contains: Person: Dakota Wood"); todo wont be becuase change of url will have cleared 
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
            WaitForOutput("No arguments provided");

            //Too many arguments
            EnterCommand("clipboard copy, show");
            WaitForOutput("Too many arguments provided");

            //Attempt to copy from home
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing an object");
            //Attempt to copy from list
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers");
            WaitForOutputStarting("Result from Current Special Offers:\r\n16 items");
            EnterCommand("clipboard c");
            WaitForOutput("Clipboard copy may only be used in the context of viewing an object");
        }

        public virtual void Edit() {
            CiceroUrl("home");
            CiceroUrl("object?o1=___1.Customer--29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("edit");
            WaitForOutput("Editing Customer: Handy Bike Services, AW00029688");
            //No arguments
            CiceroUrl("object?o1=___1.Customer--29688");
            WaitForOutput("Customer: Handy Bike Services, AW00029688");
            EnterCommand("edit x");
            WaitForOutput("Too many arguments provided");
            //Invalid contexts
            CiceroUrl("object?o1=___1.Product--358&i1=Edit");
            WaitForOutput("Editing Product: HL Grip Tape");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("edit");
            WaitForOutput("The command: edit is not available in the current context");
        }

        public virtual void Enter() {
            //Entering fields (into dialogs)
            CiceroUrl("home?m1=CustomerRepository&d1=FindIndividualCustomerByName");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: empty\r\nLast Name: empty");
            EnterCommand("enter first, Arthur G");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: Arthur G\r\nLast Name: empty");
            EnterCommand("enter last, Fenton-Jones III");
            WaitForOutput("Customers menu\r\nAction dialog: Find Individual Customer By Name\r\nFirst Name: Arthur G\r\nLast Name: Fenton-Jones III");

            //Different types (bool, date, number)
            CiceroUrl("object?o1=___1.Product--897&as1=open&d1=BestSpecialOffer");
            WaitForOutputContaining("Action dialog: Best Special Offer");
            EnterCommand("enter quantity,25");
            WaitForOutputContaining("Quantity: 25");
            CiceroUrl("home?m1=PurchaseOrderRepository&d1=ListPurchaseOrders");
            WaitForOutputContaining("Action dialog: List Purchase Orders");
            EnterCommand("enter from, 1 Jan 2016");
            WaitForOutputContaining("From Date: 1 Jan 2016");
            CiceroUrl("object?o1=___1.Customer--140&as1=open&d1=CreateNewOrder");
            WaitForOutputContaining("Action dialog: Create New Order");
            EnterCommand("enter copy,false");
            WaitForOutputContaining("Copy Header From Last Order: false");
            EnterCommand("enter copy,true");
            WaitForOutputContaining("Copy Header From Last Order: true");

            //Todo: test selections
            CiceroUrl("home?m1=ProductRepository&d1=ListProductsBySubCategory");
            WaitForOutput("Products menu\r\nAction dialog: List Products By Sub Category\r\nSub Category: Mountain Bikes");
            EnterCommand("enter cat, hand");
            WaitForOutput("Products menu\r\nAction dialog: List Products By Sub Category\r\nSub Category: Handlebars");
            EnterCommand("enter cat, xx");
            WaitForOutput("None of the choices matches xx");
            EnterCommand("enter cat, frame");
            WaitForOutput("Multiple matches:\r\nMountain Frames\r\nRoad Frames\r\nTouring Frames");

            //Then property entries
            CiceroUrl("object?o1=___1.Product--871&i1=Edit");
            WaitForOutput("Editing Product: Mountain Bottle Cage");
            EnterCommand("show price");
            WaitForOutput("List Price: £9.99");
            EnterCommand("enter list price, 10.50");
            WaitForOutput("Editing Product: Mountain Bottle Cage\r\nModified properties:\r\nList Price: £10.50");

            //Multiple matches
            CiceroUrl("object?o1=___1.WorkOrder--33787&i1=Edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter date,1 Jan 2015");
            WaitForOutput("date matches multiple fields:\r\n" +
                          "Start Date\r\n" +
                          "End Date\r\n" +
                          "Due Date");

            //No matches
            EnterCommand("enter strt date,1 Jan 2015");
            WaitForOutput("strt date does not match any properties");

            //No entry value provided
            EnterCommand("enter start date");
            WaitForOutput("Too few arguments provided");

            //Can enter an empty value
            EnterCommand("enter end date,");
            WaitForOutputContaining("End Date: required");

            //Invalid context
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("enter");
            WaitForOutput("The command: enter is not available in the current context");

            //Auto-complete on dialog - single match
            CiceroUrl("home?m1=CustomerRepository&d1=FindCustomer&f1_customer=null");
            WaitForOutputContaining("Action dialog: Find Customer");
            EnterCommand("enter cus, 00456");
            WaitForOutputContaining("Action dialog: Find Customer\r\n" +
                                    "Customer: Riding Excursions, AW00000456");
            //Auto-complete on dialog - multi match
            EnterCommand("enter cus, 456");
            WaitForOutputContaining("Multiple matches:");
            WaitForOutputContaining("Fernando Carter, AW00014563");
            //Auto-complete on dialog - no match
            EnterCommand("enter cus, 456x");
            WaitForOutput("None of the choices matches 456x");

            //Auto-complete on a scalar field
            CiceroUrl("object?i1=View&o1=___1.SalesOrderHeader--54461&as1=open&d1=AddComment&f1_comment=%22%22");
            WaitForOutputContaining("Action dialog: Add Comment");
            EnterCommand("enter comment,parc");
            WaitForOutputContaining("Multiple matches:");
            WaitForOutputContaining("Leave parcel with neighbour");
            EnterCommand("enter comment,payment");
            WaitForOutputContaining("Comment: Payment on delivery");

            //Auto-complete on an object edit
            CiceroUrl("object?i1=Edit&o1=___1.Product--415");
            WaitForOutput("Editing Product: Internal Lock Washer 5");
            EnterCommand("enter model,HL Road Frame");
            WaitForOutputContaining("Product Model: HL Road Frame");

            //TODO: Entering fields on an editable view model
            //CiceroUrl("object?o1=___1.EmailTemplate--1");
            //WaitForOutput("Email Template: Untitled Email Template");
            //EnterCommand("enter to,info@nakedobjects.net");
            //WaitForOutputContaining("To: info@nakedobjects.net");

            //Cannot use enter on a non-modifiable property
            CiceroUrl("object?i1=Edit&o1=___1.WorkOrder--37879");
            WaitForOutputStarting("Editing Work Order: HL Crankset");
            EnterCommand("enter stocked,3");
            WaitForOutput("Stocked Qty is not modifiable");

            // '?' to get details on param
            CiceroUrl("home?m1=CustomerRepository&d1=FindStoreByName&f1_name=%22%22");
            WaitForOutputContaining("Action dialog: Find Store By Name");
            EnterCommand("enter name,?");
            WaitForOutput("Field name: Name\r\n" +
                          "Description: partial match\r\n" +
                          "Type: String\r\n" +
                          "Mandatory");
            // '?' to get details on editable prop
            CiceroUrl("object?i1=Edit&o1=___1.Person--4463&as1=open");
            WaitForOutput("Editing Person: Nina Nath");
            EnterCommand("enter mid,?");
            WaitForOutput("Field name: Middle Name\r\n" +
                          "Type: String\r\n" +
                          "Optional");

            //Conditional choices
            CiceroUrl("home?m1=ProductRepository&d1=ListProducts");
            WaitForOutputContaining("Action dialog: List Products");
            EnterCommand("enter category,cloth");
            WaitForOutputContaining("Category: Clothing");
            EnterCommand("enter sub, glove");
            WaitForOutputContaining("Sub Category: Gloves");
            EnterCommand("enter sub, bike");
            WaitForOutput("None of the choices matches bike");
            EnterCommand("enter category,bike");
            //Check that the sub category field has been cleared by change in category
            WaitForOutputContaining("Sub Category: empty");
            WaitForOutputContaining("Category: Bikes");
            EnterCommand("enter sub, bike");
            WaitForOutputContaining("Multiple matches:\r\nMountain Bikes");

            //Conditional choices on editing an object
            CiceroUrl("object?r1=0&o1=___1.Product--840");
            WaitForOutput("Product: HL Road Frame - Black, 52");
            EnterCommand("edit");
            WaitForOutput("Editing Product: HL Road Frame - Black, 52");
            EnterCommand("enter product category,cloth");
            WaitForOutputContaining("Modified properties:");
            WaitForOutputContaining("Product Subcategory: empty");
            WaitForOutputContaining("Product Category: Clothing");
            EnterCommand("enter sub, glove");
            WaitForOutputContaining("Product Subcategory: Gloves");
            EnterCommand("enter sub, bike");
            WaitForOutput("None of the choices matches bike");
            EnterCommand("enter product category,bike");
            WaitForOutputContaining("Product Category: Bikes");
            EnterCommand("enter sub, bike");
            WaitForOutputContaining("Multiple matches:\r\nMountain Bikes");

            //Multiple choices
            CiceroUrl("home?m1=ProductRepository&d1=ListProductsBySubCategories&f1_subCategories=%5B%7B%22href%22:%22http:%2F%2Flocalhost:61546%2Fobjects%2F___1.ProductSubcategory%2F1%22,%22title%22:%22Mountain%20Bikes%22%7D,%7B%22href%22:%22http:%2F%2Flocalhost:61546%2Fobjects%2F___1.ProductSubcategory%2F3%22,%22title%22:%22Touring%20Bikes%22%7D%5D");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes");
            EnterCommand("enter sub, handle");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes-Handlebars");
            EnterCommand("enter sub, mountain bikes");
            WaitForOutputContaining("Sub Categories: -Touring Bikes-Handlebars");

          
            CiceroUrl("home?m1=ProductRepository&d1=FindProductsByCategory");
            WaitForOutputContaining("Action dialog: Find Products By Category");
            EnterCommand("enter Categories,cloth");
            WaitForOutputContaining("Categories: -Bikes-Clothing");
            EnterCommand("enter sub, glove");
            WaitForOutputContaining("Subcategories: -Mountain Bikes-Road Bikes-Gloves");
            EnterCommand("enter sub, mount");
            WaitForOutputContaining("Subcategories: -Road Bikes-Gloves");
            EnterCommand("enter sub, road bikes");
            WaitForOutputContaining("Subcategories: -Gloves");
            EnterCommand("enter sub, gloves");
            WaitForOutputContaining("Subcategories: empty");

            CiceroUrl("home?m1=ProductRepository&d1=FindProductsByCategory");
            WaitForOutputContaining("Action dialog: Find Products By Category");
            EnterCommand("enter Categories,bikes");
            WaitForOutputContaining("Categories: empty");        
            WaitForOutputContaining("Subcategories: empty");

            //Finish somewhere other than home!
            EnterCommand("menu products");
            WaitForOutput("Products menu");
        }

        public virtual void Gemini() {
            //home
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Home);

            CiceroUrl("object?o1=___1.Product--968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            EnterCommand("gemini");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");

            //No arguments
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("ge x");
            WaitForOutput("Too many arguments provided");
        }

        public virtual void Goto() {
            CiceroUrl("object?o1=___1.Customer--577");
            WaitForOutput("Customer: Synthetic Materials Manufacturing, AW00000577");
            //Full match
            EnterCommand("go Store");
            WaitForOutput("Store: Synthetic Materials Manufacturing");
            //Partial match (on Sales Person)
            EnterCommand("go pers");
            WaitForOutput("Sales Person: Pamela Ansman-Wolfe");
            //No match
            EnterCommand("go x");
            WaitForOutput("x does not match any reference fields or collections");
            //Matches only value fields
            EnterCommand("go bonus");
            WaitForOutput("bonus does not match any reference fields or collections");

            //Multiple matches
            EnterCommand("go details");
            WaitForOutput("Multiple matches for details:\r\nEmployee Details\r\nPerson Details");
            //Multiple clause match
            EnterCommand("go details pers"); //Person Details
            WaitForOutput("Person: Pamela Ansman-Wolfe");
            //Wrong no of args
            EnterCommand("go");
            WaitForOutput("No arguments provided");
            EnterCommand("go x,y");
            WaitForOutput("Too many arguments provided");
            //Now try for a list context
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&c1=List");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 1");
            WaitForOutput("Special Offer: No Discount");
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&c1=List");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 16");
            WaitForOutput("Special Offer: Mountain-500 Silver Clearance Sale");
            //Try out of range
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&pg1=1&ps1=20&s1_=0&c1=List");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("go 0");
            WaitForOutput("0 is out of range for displayed items");
            EnterCommand("go 17");
            WaitForOutput("17 is out of range for displayed items");
            EnterCommand("go x");
            WaitForOutput("x is not a number");
            EnterCommand("go 1x"); //Because of behaviour of tryParse
            WaitForOutput("Special Offer: No Discount");
            //Wrong context
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("go x");
            WaitForOutput("The command: goto is not available in the current context");
            CiceroUrl("home?m1=ProductRepository&d1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("go x");
            WaitForOutput("The command: goto is not available in the current context");

            //Goto a collection within an object
            CiceroUrl("object?o1=___1.SalesOrderHeader--60485");
            WaitForOutput("Sales Order: SO60485");

            //Simple case
            EnterCommand("goto details");
            WaitForOutput("Details: 3 items\r\n(Collection on Sales Order: SO60485)");

            //Multiple matches
            CiceroUrl("object?o1=___1.Product--901");
            WaitForOutput("Product: LL Touring Frame - Yellow, 54");
            EnterCommand("goto pr");
            WaitForOutput("Multiple matches for pr:\r\nProduct Model\r\nProduct Category\r\nProduct Subcategory\r\nProduct Inventory\r\nProduct Reviews");

            //Multi-clause match
            EnterCommand("goto v ory");
            WaitForOutput("Product Inventory: empty\r\n(Collection on Product: LL Touring Frame - Yellow, 54)");

            //No matches
            CiceroUrl("object?o1=___1.SalesOrderHeader--60485");
            WaitForOutput("Sales Order: SO60485");
            EnterCommand("go x son");
            WaitForOutput("x son does not match any reference fields or collections");

            //Too many arguments
            EnterCommand("go de,tails");
            WaitForOutput("Too many arguments provided");

            //Goto applied to collection lines
            EnterCommand("go details");
            WaitForOutputStarting("Details:");

            EnterCommand("go 4");
            WaitForOutput("4 is out of range for displayed items");

            EnterCommand("go 2");
            WaitForOutput("Sales Order Detail: 1 x ML Road Tire");
        }

        public virtual void Help() {
            //Help from home
            CiceroUrl("home");
            EnterCommand("help");
            WaitForOutputStarting("Cicero is a user interface purpose-designed");
            EnterCommand("help ?");
            WaitForOutput("Commands available in current context:\r\nback\r\nclipboard\r\nforward\r\ngemini\r\nhelp\r\nmenu\r\nwhere");
            //Now try an object context
            CiceroUrl("object?o1=___1.Product--943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40");
            //First with no params
            EnterCommand("help ?");
            WaitForOutput("Commands available in current context:\r\naction\r\nback\r\nclipboard\r\nedit\r\nforward\r\ngemini\r\ngoto\r\nhelp\r\nmenu\r\nreload\r\nshow\r\nwhere");
            //Now with params
            EnterCommand("help me");
            WaitForOutputContaining("menu command:\r\nOpen a named main menu");
            EnterCommand("help menux");
            WaitForOutput("No such command: menux");
            EnterCommand("help menu back");
            WaitForOutput("No such command: menu back");
            EnterCommand("help menu, back");
            WaitForOutput("Too many arguments provided");
            //List context
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0");
            WaitForOutputStarting("Result from Current Special Offers:");
            EnterCommand("help ?");
            WaitForOutput("Commands available in current context:\r\naction\r\nback\r\nclipboard\r\nforward\r\ngemini\r\ngoto\r\nhelp\r\nmenu\r\npage\r\nreload\r\nselection\r\nshow\r\nwhere");
        }

        public virtual void Menu() {
            //No argument
            CiceroUrl("home");
            EnterCommand("Menu");
            WaitForOutput("Menus:\r\nCustomers\r\nOrders\r\nProducts\r\nEmployees\r\nSales\r\nSpecial Offers\r\nContacts\r\nVendors\r\nPurchase Orders\r\nWork Orders");
            //Now with arguments
            EnterCommand("Menu Customers");
            WaitForOutput("Customers menu");
            EnterCommand("Menu pro");
            WaitForOutput("Products menu");
            EnterCommand("Menu off");
            WaitForOutput("Special Offers menu");
            EnterCommand("Menu ord");
            WaitForOutput("Matching menus:\r\nOrders\r\nPurchase Orders\r\nWork Orders");
            EnterCommand("Menu foo");
            WaitForOutput("foo does not match any menu");
            EnterCommand("Menu cust prod");
            WaitForOutput("cust prod does not match any menu");
            EnterCommand("Menu cus, ord");
            WaitForOutput("Too many arguments provided");
            //Invoked in another context
            CiceroUrl("object?o1=___1.Product--943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40");
            EnterCommand("Menu");
            WaitForOutput("Menus:\r\nCustomers\r\nOrders\r\nProducts\r\nEmployees\r\nSales\r\nSpecial Offers\r\nContacts\r\nVendors\r\nPurchase Orders\r\nWork Orders");

            //Test for exact match taking precedence over partial matches
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("menu orders"); //which would match 3, but one exactly
            WaitForOutput("Orders menu");

            //Invoking menu from a non-home context, clears state
            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers");
            WaitForOutputStarting("Result from Current Special Offers:");
            EnterCommand("menu cus");
            WaitForOutput("Customers menu");
        }

        public virtual void OK() {
            CiceroUrl("home");
            //Open a zero-param action on main menu
            CiceroUrl("home?m1=ProductRepository&d1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("ok");
            WaitForOutputStarting("Product: ");
            // No arguments
            CiceroUrl("home?m1=ProductRepository&d1=RandomProduct");
            WaitForOutput("Products menu\r\nAction dialog: Random Product");
            EnterCommand("ok x");
            WaitForOutput("Too many arguments provided");

            //Object action
            CiceroUrl("object?o1=___1.Customer--29688&d1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("ok");
            WaitForOutput("Sales Order: SO69562");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");
            CiceroUrl("home?m1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("ok");
            WaitForOutput("The command: ok is not available in the current context");

            //Menu action that returns a List
            CiceroUrl("home?m1=OrderRepository&d1=HighestValueOrders");
            WaitForOutput("Orders menu\r\nAction dialog: Highest Value Orders");
            EnterCommand("ok");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1 of ");

            //Menu action with params
            CiceroUrl("home?m1=CustomerRepository&d1=FindIndividualCustomerByName");
            WaitForOutputContaining("Action dialog: Find Individual Customer By Name");
            EnterCommand("enter first,a");
            WaitForOutputContaining("First Name: a");
            EnterCommand("enter last,b");
            WaitForOutputContaining("Last Name: b");
            EnterCommand("ok");
            WaitForOutputStarting("Result from Find Individual Customer By Name:\r\nPage 1 of 8 containing 20 of");

            //Menu action with missing mandatory params
            EnterCommand("menu customers");
            WaitForOutputContaining("Customers menu");
            EnterCommand("ac Find Indiv Name");
            WaitForOutputContaining("Action dialog: Find Individual Customer By Name");
            WaitForOutputContaining("Last Name: empty");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields:\r\nLast Name: required");

            //Menu action with invalid entry
            CiceroUrl("home?m1=CustomerRepository&d1=FindCustomerByAccountNumber");
            WaitForOutputContaining("Action dialog: Find Customer By Account Number");
            EnterCommand("enter num,12345");
            WaitForOutputContaining("Account Number: 12345");
            EnterCommand("ok");
            WaitForOutput("Please complete or correct these fields:\r\nAccount Number: 12345 Account number must start with AW");

            //Menu action with select param
            CiceroUrl("home?m1=ProductRepository&d1=ListProductsBySubCategory");
            //&f1_subCategory=%7B%22href%22%3A%22http%3A%2F%2Flocalhost%3A61546%2Fobjects%2F___1.ProductSubcategory%2F10%22%2C%22title%22%3A%22Forks%22%7D");
            WaitForOutputContaining("Action dialog: List Products By Sub Category");
            EnterCommand("enter sub,forks");
            WaitForOutputContaining("Sub Category: Forks");
            EnterCommand("ok");
            WaitForOutputStarting("Result from List Products By Sub Category:\r\n3 items");

            //Action resulting in an error
            CiceroUrl("home?m1=CustomerRepository&d1=ThrowDomainException");
            WaitForOutput("Customers menu\r\nAction dialog: Throw Domain Exception");
            EnterCommand("ok");
            WaitForOutput("Sorry, an application error has occurred. Foo");

            //Co-validation error
            CiceroUrl("object?o1=___1.Vendor--1668&as1=open&d1=ListPurchaseOrders");
            WaitForOutputContaining("Action dialog: List Purchase Orders");
            EnterCommand("enter from, 6 Jan 2016");
            WaitForOutputContaining("From Date: 6 Jan 2016");
            EnterCommand("enter to date, 3 Jan 2016");
            ;
            WaitForOutputContaining("To Date: 3 Jan 2016");
            EnterCommand("ok");
            WaitForOutput("To Date cannot be before From Date");

            //User Warnings and Info
            CiceroUrl("home?m1=WorkOrderRepository&d1=GenerateInfoAndWarning");
            WaitForOutputContaining("Action dialog: Generate Info And Warning");
            EnterCommand("ok");
            WaitForOutputContaining("Warning: Warn User of something else");
            WaitForOutputContaining("Inform User of something");
        }

        public virtual void Page() {
            CiceroUrl("list?m1=OrderRepository&a1=HighestValueOrders&p1=1&ps1=20&s1_=0");
            WaitForOutputStarting("Result from Highest Value Orders:\r\nPage 1 of 157");

            var lastPage = br.FindElement(By.CssSelector(".output")).Text.Split(' ')[7];
            var lastPagePlusOne = (int.Parse(lastPage) + 1).ToString();
            var lastPageLessOne = (int.Parse(lastPage) - 1).ToString();

            EnterCommand("page 0");
            WaitForOutput($"Specified page number must be between 1 and {lastPage}");
            EnterCommand("page 2");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage 2 of {lastPage}");
            EnterCommand($"page {lastPage}");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage {lastPage} of {lastPage}");
            EnterCommand($"page {lastPagePlusOne}");
            WaitForOutput($"Specified page number must be between 1 and {lastPage}");
            EnterCommand("page f");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage 1 of {lastPage}");
            EnterCommand("page p");
            WaitForOutput("List is already showing the first page");
            EnterCommand("pa last");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage {lastPage} of {lastPage}");
            EnterCommand("pa nex");
            WaitForOutput("List is already showing the last page");
            EnterCommand("page p");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage {lastPageLessOne} of {lastPage}");
            EnterCommand("pa ne");
            WaitForOutputStarting($"Result from Highest Value Orders:\r\nPage {lastPage} of {lastPage}");

            //Invalid argument
            EnterCommand("page furst");
            WaitForOutput("The argument must match: first, previous, next, last, or a single number");

            //No argument
            EnterCommand("page");
            WaitForOutput("No arguments provided");

            //Invalid context
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("page 1");
            WaitForOutput("The command: page is not available in the current context");
            CiceroUrl("object?o1=___1.SalesOrderHeader--51131");
            WaitForOutput("Sales Order: SO51131");
            EnterCommand("page 1");
            WaitForOutput("The command: page is not available in the current context");
        }

        public virtual void Root() {
            CiceroUrl("object?o1=___1.Product--459&c1_ProductInventory=List");
            WaitForOutputStarting("Product Inventory: 3 items\r\n(Collection on Product: Lock Nut 19)");
            EnterCommand("root");
            WaitForOutput("Product: Lock Nut 19");

            //Invalid contexts
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            CiceroUrl("object?o1=___1.Product--459");
            WaitForOutput("Product: Lock Nut 19");
            EnterCommand("root");
            WaitForOutput("The command: root is not available in the current context");

            //Argument added
            CiceroUrl("object?o1=___1.Product--459&c1_ProductInventory=List");
            WaitForOutputStarting("Product Inventory: 3 items\r\n(Collection on Product: Lock Nut 19)");
            EnterCommand("root x");
            WaitForOutput("Too many arguments provided");
        }

        public virtual void Save() {
            //Happy case
            CiceroUrl("object?o1=___1.Product--839&i1=Edit");
            WaitForOutput("Editing Product: HL Road Frame - Black, 48");
            EnterCommand("enter list price, 1500");
            WaitForOutputContaining("Modified properties:\r\nList Price: £1,500.00");
            EnterCommand("save");
            WaitForOutput("Product: HL Road Frame - Black, 48");

            //Not valid once object is saved, or other contexts.
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");
            CiceroUrl("object?o1=___1.Customer--29688&d1=LastOrder");
            WaitForOutput("Customer: Handy Bike Services, AW00029688\r\nAction dialog: Last Order");
            EnterCommand("save");
            WaitForOutput("The command: save is not available in the current context");

            //No arguments
            CiceroUrl("object?o1=___1.Product--839&i1=Edit");
            WaitForOutputStarting("Editing Product: HL Road Frame - Black, 48");
            EnterCommand("save x");
            WaitForOutput("Too many arguments provided");

            //Field validation
            CiceroUrl("object?o1=___1.WorkOrder--43134&i1=Edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter order qty,0");
            WaitForOutputContaining("Order Qty: 0");
            EnterCommand("save");
            WaitForOutput("Please complete or correct these fields:\r\n" +
                          "Order Qty: 0 Order Quantity must be > 0");

            //Co-validation
            CiceroUrl("object?o1=___1.WorkOrder--43133&i1=Edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter start date, 17/10/2015");
            WaitForOutputContaining("Start Date: 17 Oct 2015");
            EnterCommand("save");
            WaitForOutput("StartDate must be before DueDate");
        }

        public virtual void Show() {
            #region In an object  -  to show properties

            CiceroUrl("object?o1=___1.Product--758");
            WaitForOutput("Product: Road-450 Red, 52");
            EnterCommand("show num");
            WaitForOutput("Product Number: BK-R68R-52");
            EnterCommand("sh product");
            WaitForOutput("Product Number: BK-R68R-52\r\nProduct Model: Road-450\r\nProduct Category: Bikes\r\nProduct Subcategory: Road Bikes\r\nProduct Line: R \r\nProduct Inventory: 2 items\r\nProduct Reviews: empty");
            //Note that spacing of Road-450 and  Line: R is different to how it appears on screen!
            //Name: Road-450 Red, 52

            //No argument
            EnterCommand("sh ");
            WaitForOutputStarting("Name: Road-450 Red"); //\r\nProduct Number: BK-R68R-52\r\nColor: Red\r\nPhoto: empty\r\nProduct Model: Road-450\r\nList Price: 1,457.99");
            //No match
            EnterCommand("sh x");
            WaitForOutput("x does not match any properties");

            //Invalid context
            CiceroUrl("home");
            EnterCommand("show");
            WaitForOutput("The command: show is not available in the current context");

            //Multi-clause match
            CiceroUrl("object?o1=___1.SalesPerson--284");
            WaitForOutput("Sales Person: Tete Mensa-Annan");
            EnterCommand("sh sales a");
            WaitForOutput("Sales Territory: Northwest\r\nSales Quota: £300,000.00\r\nSales YTD: £1,576,562.20\r\nSales Last Year: £0.00");
            EnterCommand("sh ter ory");
            WaitForOutputStarting("Sales Territory: Northwest\r\nTerritory History: 1 item");
            EnterCommand("sh sales z");
            WaitForOutput("sales z does not match any properties");

            //No fields
            CiceroUrl("object?o1=___1.AddressType--2");
            WaitForOutput("Address Type: Home");
            EnterCommand("show");
            WaitForOutput("No visible properties");

            //To many args
            EnterCommand("show num,x");
            WaitForOutput("Too many arguments provided");

            //Reading properties in edit mode
            CiceroUrl("object?o1=___1.Product--369&i1=Edit");
            WaitForOutputStarting("Editing");
            EnterCommand("enter style,U");
            WaitForOutputContaining("Style: U");
            EnterCommand("enter list price,500");
            WaitForOutputContaining("List Price: £500");
            EnterCommand("show");
            WaitForOutputContaining("List Price: £500.00 (modified)");
            WaitForOutputContaining("Style: U  (modified)");
            //Test that date properties are correctly formatted/masked
            EnterCommand("sh modified");
            WaitForOutputContaining("Modified Date: 11 Mar 2008 10:01:36");
            WaitForOutputContaining("10:01:36");

            //exact match takes priority over partial match
            //TODO: Need example from properties, not action params -  transfer this to Enter test
            //EnterCommand("field product category"); //which would also match product subcategory
            //WaitForOutput("Product Category: Bikes");

            //Check that enum property renders as text, not number
            CiceroUrl("object?o1=___1.SalesOrderHeader--70996");
            WaitForOutput("Sales Order: SO70996");
            EnterCommand("sh status");
            WaitForOutput("Status: Shipped");

            //Test that date properties are correctly formatted/masked
            CiceroUrl("object?i1=View&o1=___1.WorkOrder--71031");
            WaitForOutput("Work Order: Road-750 Black, 52: 19 Jun 2008");
            EnterCommand("sh start");
            WaitForOutput("Start Date: 19 Jun 2008");

            #endregion

            #region In a list

            CiceroUrl("list?m1=SpecialOfferRepository&a1=CurrentSpecialOffers");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
            EnterCommand("show 1");
            WaitForOutput("item 1: No Discount");
            EnterCommand("show 16");
            WaitForOutput("item 16: Mountain-500 Silver Clearance Sale");
            EnterCommand("show 2-4");
            WaitForOutput("item 2: Volume Discount 11 to 14\r\nitem 3: Volume Discount 15 to 24\r\nitem 4: Volume Discount 25 to 40");
            EnterCommand("show 5-5");
            WaitForOutput("item 5: Volume Discount 41 to 60");
            EnterCommand("show -3");
            WaitForOutput("item 1: No Discount\r\nitem 2: Volume Discount 11 to 14\r\nitem 3: Volume Discount 15 to 24");
            EnterCommand("show 15-");
            WaitForOutput("item 15: Half-Price Pedal Sale\r\nitem 16: Mountain-500 Silver Clearance Sale");

            //Invalid numbers
            EnterCommand("show 17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("show 0");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("show 15-17");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("show 0-3");
            WaitForOutput("Item number or range values must be greater than zero");
            EnterCommand("show 5-4");
            WaitForOutput("Starting item number cannot be greater than the ending item number");

            #endregion

            #region In a collection

            CiceroUrl("object?o1=___1.SalesOrderHeader--44518&c1_Details=List");
            WaitForOutput("Details: 20 items\r\n(Collection on Sales Order: SO44518)");
            EnterCommand("show 1");
            WaitForOutput("item 1: 5 x Mountain-100 Black, 44");
            EnterCommand("show 20");
            WaitForOutput("item 20: 2 x HL Mountain Frame - Black, 38");

            //No number
            EnterCommand("show");
            WaitForOutputStarting("item 1: 5 x Mountain-100 Black, 44\r\nitem 2: 3 x Sport-100 Helmet, Black\r\nitem 3:");

            //Too many parms
            EnterCommand("show 4,5");
            WaitForOutput("Too many arguments provided");

            //Alpha parm
            EnterCommand("show one");
            WaitForOutput("one is not a number");

            #endregion

            #region Invalid context

            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("show 1");
            WaitForOutput("The command: show is not available in the current context");
            CiceroUrl("home?m1=CustomerRepository");
            WaitForOutput("Customers menu");
            EnterCommand("show 1");
            WaitForOutput("The command: show is not available in the current context");

            #endregion
        }

        public virtual void Where() {
            CiceroUrl("home");
            CiceroUrl("object?o1=___1.Product--358");
            WaitForOutput("Product: HL Grip Tape");
            //Do something to change the output
            EnterCommand("help ?");
            WaitForOutputStarting("Commands");
            EnterCommand("where");
            WaitForOutput("Product: HL Grip Tape");

            //Empty command == where
            EnterCommand("help ?");
            WaitForOutputStarting("Commands");
            ClearFieldThenType("input", Keys.Enter);
            WaitForOutput("Product: HL Grip Tape");

            //No arguments
            EnterCommand("where x");
            WaitForOutput("Too many arguments provided");
        }

        public virtual void SpaceBarAutoComplete() {
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "sel" + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "selection ");

            CiceroUrl("object?o1=___1.Product--968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            //Hitting Tab with no entry has no effect
            TypeIntoFieldWithoutClearing("input", Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            WaitForOutput("Product: Touring-1000 Blue, 54");

            //Unrecognised two chars
            TypeIntoFieldWithoutClearing("input", "xx" + Keys.Space);
            WaitForOutput("No command begins with xx");

            //Single character
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "f" + Keys.Space);
            WaitForOutput("Command word must have at least 2 characters");

            //if there's any argument specified, just adds a space
            CiceroUrl("object?o1=___1.Product--968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            TypeIntoFieldWithoutClearing("input", "he menu" + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help menu ");

            //chained commands
            ClearFieldThenType("input", "me pr;ac rand;ok " + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");

            //Space bar before command eventually removed
            ClearFieldThenType("input", " me pr; ac rand; ok " + Keys.Space);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");
        }

        public virtual void TabAutoComplete() {
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "sel" + Keys.Tab);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "selection ");

            CiceroUrl("object?o1=___1.Product--968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            //Hitting Tab with no entry has no effect
            TypeIntoFieldWithoutClearing("input", Keys.Tab);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            WaitForOutput("Product: Touring-1000 Blue, 54");

            //Unrecognised two chars
            TypeIntoFieldWithoutClearing("input", "xx" + Keys.Tab);
            WaitForOutput("No command begins with xx");

            //Single character
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            TypeIntoFieldWithoutClearing("input", "f" + Keys.Tab);
            WaitForOutput("Command word must have at least 2 characters");

            //if there's any argument specified, just adds a space
            CiceroUrl("object?o1=___1.Product--968");
            WaitForOutput("Product: Touring-1000 Blue, 54");
            TypeIntoFieldWithoutClearing("input", "he menu" + Keys.Tab);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help menu ");

            //chained commands
            ClearFieldThenType("input", "me pr;ac rand;ok " + Keys.Tab);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");

            //Space bar before command eventually removed
            ClearFieldThenType("input", " me pr; ac rand; ok " + Keys.Tab);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "menu pr;action rand;ok ");
        }

        public virtual void UnrecognisedCommand() {
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("m");
            WaitForOutput("Command word must have at least 2 characters");

            EnterCommand("hl");
            WaitForOutput("No command begins with hl");
        }

        public virtual void UpAndDownArrow() {
            CiceroUrl("home");
            EnterCommand("he");
            WaitForOutputStarting("Cicero is");
            Assert.AreEqual("", WaitForCss("input").GetAttribute("value"));
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);

            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help");
            TypeIntoFieldWithoutClearing("input", " gem" + Keys.Enter);
            WaitForOutputStarting("gemini command");
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "help gem");
            TypeIntoFieldWithoutClearing("input", Keys.ArrowDown);
            wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value") == "");

            //TODO
            //CiceroUrl("home");
            //WaitForOutputStarting("Welcome to Cicero");
            //EnterCommand("me pr; act rand; ok;show 1");
            //WaitForOutput("The command: show is not available in the current context");
            ////Test that the up arrow produces full string
            //Assert.AreEqual("", WaitForCss("input").GetAttribute("value"));
            //TypeIntoFieldWithoutClearing("input", Keys.ArrowUp);
            //wait.Until(dr => dr.FindElement(By.CssSelector("input")).GetAttribute("value")
            //== "menu pr;action rand;ok;show 1");
        }

        public virtual void ScenarioEditAndSave() {
            //happy case -  edit one property
            CiceroUrl("object?o1=___1.Product--838");
            WaitForOutput("Product: HL Road Frame - Black, 44");
            EnterCommand("show list price");
            WaitForOutputStarting("List Price: ");
            var output = WaitForCss(".output").Text;
            var oldPrice = output.Split(' ').Last();
            EnterCommand("Edit");
            WaitForOutput("Editing Product: HL Road Frame - Black, 44");
            var rand = new Random();
            var newPrice = rand.Next(50, 150);
            string currency = "£" + newPrice.ToString("c").Substring(1);
            EnterCommand("Enter list price, " + newPrice.ToString());
            WaitForOutput("Editing Product: HL Road Frame - Black, 44\r\n" +
                          "Modified properties:\r\n" +
                          "List Price: " + currency);
            EnterCommand("Save");
            WaitForOutput("Product: HL Road Frame - Black, 44");
            EnterCommand("show list price");
            WaitForOutput("List Price: " + currency);

            //Updating a date and a link
            CiceroUrl("object?o1=___1.WorkOrder--43132");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter end date,31 Dec 2015");
            WaitForOutputContaining("End Date: 31 Dec 2015");
            EnterCommand("enter scrap reas, gouge");
            WaitForOutputContaining("Scrap Reason: Gouge in metal");
            EnterCommand("save");
            WaitForOutputStarting("Work Order:");
            EnterCommand("show end date");
            WaitForOutputContaining("2015"); // because Masks not being honoured
            EnterCommand("show scrap rea");
            WaitForOutput("Scrap Reason: Gouge in metal");

            //Field validation
            //Order Quantity must be > 0
            CiceroUrl("object?o1=___1.WorkOrder--43134");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter order qty,0");
            WaitForOutputContaining("Order Qty: 0");
            EnterCommand("save");
            WaitForOutput("Please complete or correct these fields:\r\n" +
                          "Order Qty: 0 Order Quantity must be > 0");

            //Co-Validation
            CiceroUrl("object?o1=___1.WorkOrder--43133");
            WaitForOutputStarting("Work Order:");
            EnterCommand("edit");
            WaitForOutputStarting("Editing Work Order:");
            EnterCommand("enter start date,17 Sep 2007"); //Seems to be necessary to clear the date fields fully
            WaitForOutputContaining("Start Date: 17 Sep 2007");
            EnterCommand("enter due date,15 Sep 2007");
            WaitForOutputContaining("Due Date: 15 Sep 2007");
            EnterCommand("save");
            WaitForOutput("StartDate must be before DueDate");
            EnterCommand("enter due date,28 Sep 2008");
            WaitForOutputContaining("Due Date: 28 Sep 2008");
            EnterCommand("save");
            WaitForOutputStarting("Work Order:");
        }

        public virtual void ScenarioMultiSelect() {
            //Multi-select and deselect - reference objects
            CiceroUrl("home?m1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("action List Products By Sub Categories");
            WaitForOutputContaining("Action dialog: List Products By Sub Categories");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes");
            Thread.Sleep(200);
            //Add a new selection
            EnterCommand("enter sub, handlebars");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Touring Bikes-Handlebars");
            //Remove an existing selection
            EnterCommand("enter sub, touring bikes");
            WaitForOutputContaining("Sub Categories: -Mountain Bikes-Handlebars");
            EnterCommand("enter sub, frames");
            WaitForOutput("Multiple matches:\r\nMountain Frames\r\nRoad Frames\r\nTouring Frames");
            EnterCommand("enter sub, x");
            WaitForOutput("None of the choices matches x");

            //Multi-select enums
            CiceroUrl("home?m1=ProductRepository");
            WaitForOutput("Products menu");
            EnterCommand("action Find By Product Lines And Classes");
            WaitForOutputContaining("Product Line: M,S,");
            WaitForOutputContaining("Product Class: H,");
            Thread.Sleep(200);
            EnterCommand("enter line, r");
            WaitForOutputContaining("Product Line: M,S,R");
            EnterCommand("enter line, m");
            WaitForOutputContaining("Product Line: S,R");
            EnterCommand("enter line, x");
            WaitForOutputContaining("None of the choices matches x");
            EnterCommand("enter line,");
            WaitForOutputContaining("Multiple matches:\r\nM\r\nR\r\nS\r\nT");
        }

        public virtual void ScenarioTransientObject() {
            //Happy case
            CiceroUrl("object?o1=___1.Person--12044");
            WaitForOutput("Person: Gail Moore");
            EnterCommand("action Create New Credit Card");
            WaitForOutputContaining("Action dialog: Create New Credit Card");
            EnterCommand("ok");
            WaitForOutputStarting("Unsaved Credit Card");
            EnterCommand("enter card type, Vista");
            WaitForOutputContaining("Card Type: Vista");
            string number = DateTime.Now.Ticks.ToString(); //pseudo-random string
            var obfuscated = number.Substring(number.Length - 4).PadLeft(number.Length, '*');
            EnterCommand(" enter number," + number);
            WaitForOutputContaining("Card Number: " + number);
            EnterCommand("enter month,12");
            WaitForOutputContaining("Exp Month: 12");
            EnterCommand("enter year,2020");
            WaitForOutputContaining("Exp Year: 2020");
            EnterCommand("save");
            WaitForOutput("Credit Card: " + obfuscated);
            //Incomplete fields
            CiceroUrl("object?o1=___1.Person--12045");
            WaitForOutput("Person: Rakesh Tangirala");
            EnterCommand("action Create New Credit Card");
            WaitForOutputContaining("Action dialog: Create New Credit Card");
            EnterCommand("ok");
            WaitForOutput("Unsaved Credit Card");
            EnterCommand("save");
            WaitForOutputStarting("Please complete or correct these fields:");
            //Request for an expired transient
            CiceroUrl("object?i1=Transient&o1=___1.CreditCard--37");
            WaitForOutput("The requested view of unsaved object details has expired.");
        }

        public virtual void ScenarioUsingClipboard() {
            //Copy a Product to clipboard
            CiceroUrl("object?o1=___1.Product--980");
            WaitForOutput("Product: Mountain-400-W Silver, 38");
            EnterCommand("clip copy");
            WaitForOutput("Clipboard contains: Product: Mountain-400-W Silver, 38");
            EnterCommand("menu emp");
            WaitForOutput("Employees menu");
            EnterCommand("ac create new");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Contents of Clipboard are not compatible with the field");
            EnterCommand("clip show");
            WaitForOutput("Clipboard contains: Product: Mountain-400-W Silver, 38");
            EnterCommand("clip discard");
            WaitForOutput("Clipboard is empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Cannot use Clipboard as it is empty");
            CiceroUrl("object?o1=___1.Person--7185");
            WaitForOutput("Person: Carmen Perez");
            EnterCommand("clip copy");
            WaitForOutput("Clipboard contains: Person: Carmen Perez");
            EnterCommand("menu emp");
            WaitForOutput("Employees menu");
            EnterCommand("ac create new");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: empty");
            EnterCommand("enter details, paste");
            WaitForOutput("Employees menu\r\nAction dialog: Create New Employee From Contact\r\nContact Details: Carmen Perez");
            EnterCommand("ok");
            WaitForOutput("Unsaved Employee");
        }

        public virtual void ScenarioTestEditableVM() {
            CiceroUrl("object?i1=View&o1=___1.Person--5968");
            WaitForOutput("Person: Nathan Diaz");
            EnterCommand("ac email");
            WaitForOutputContaining("Action dialog: Create Email");
            EnterCommand("ok");
            WaitForOutput("Editing Email Template: New email");
            EnterCommand("enter to,Stef");
            WaitForOutputContaining("Modified properties:\r\nTo: Stef");
            EnterCommand("enter from,Richard");
            WaitForOutputContaining("From: Richard");
            EnterCommand("enter sub,Subject1");
            WaitForOutputContaining("Subject: Subject1");
            EnterCommand("enter mes,Hello");
            WaitForOutputContaining("Message: Hello");
            EnterCommand("ac send");
            WaitForOutputContaining("Action dialog: Send");
            EnterCommand("ok");
            WaitForOutput("Editing Email Template: Sent email");
        }

        public virtual void ChainedCommands() {
            //Happy case
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("menu pr; action rand; ok");
            WaitForOutputStarting("Product:");

            //Try to chain a command that may never be chained
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("menu pr; action rand; ok; edit");
            WaitForOutputStarting("edit command may not be chained. Use Where command to see where execution stopped.");
            EnterCommand("where");
            WaitForOutputStarting("Product:");

            CiceroUrl("home");
            //Try to chain an action invocation on a non-query action
            EnterCommand("menu special; ac create new; ok");
            WaitForOutputStarting("ok command may not be chained unless the action is query-only. Use Where command to see where execution stopped.");
            EnterCommand("where");
            WaitForOutputStarting("Special Offers menu\r\nAction dialog: Create New Special Offer");

            //Try to chain a command that is not avialable in the current context
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("menu pr;action rand;show 1");
            WaitForOutput("The command: show is not available in the current context");

            //Error in execution -  Timing problem?
            CiceroUrl("home");
            WaitForOutputStarting("Welcome to Cicero");
            EnterCommand("menu special; ac current; ok; show 20");
            WaitForOutput("The highest numbered item is 16");
            EnterCommand("where");
            WaitForOutput("Result from Current Special Offers:\r\n16 items");
        }

        public virtual void LaunchCiceroFromIcon() {
            GeminiUrl("object?o1=___1.Product--968");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            Click(WaitForCss(".icon.speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54"); //Cicero
            GeminiUrl("object/list?o1=___1.Store--350&m2=OrderRepository&a2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            Click(WaitForCss(".icon.speech"));
            WaitForOutput("Store: Twin Cycles"); //Cicero

            GeminiUrl("object?o1=___1.Product--968&as1=open&d1=BestSpecialOffer&f1_quantity=%22%22");
            WaitForView(Pane.Single, PaneType.Object, "Touring-1000 Blue, 54");
            WaitForCss("#quantity1"); //i.e. dialog open
            Click(WaitForCss(".icon.speech"));
            WaitForOutput("Product: Touring-1000 Blue, 54\r\nAction dialog: Best Special Offer\r\nQuantity: empty");
        }
    }

    public abstract class CiceroTests : CiceroTestRoot {
        [TestMethod]
        public override void Action() {
            base.Action();
        }

        [TestMethod]
        public override void BackAndForward() {
            base.BackAndForward();
        }

        [TestMethod]
        public override void Cancel() {
            base.Cancel();
        }

        [TestMethod]
        public override void Clipboard() {
            base.Clipboard();
        }

        [TestMethod]
        public override void Edit() {
            base.Edit();
        }

        [TestMethod]
        public override void Enter() {
            base.Enter();
        }

        [TestMethod]
        public override void Gemini() {
            base.Gemini();
        }

        [TestMethod]
        public override void Goto() {
            base.Goto();
        }

        [TestMethod]
        public override void Help() {
            base.Help();
        }

        [TestMethod]
        public override void Menu() {
            base.Menu();
        }

        [TestMethod]
        public override void OK() {
            base.OK();
        }

        [TestMethod]
        public override void Page() {
            base.Page();
        }

        [TestMethod]
        public override void Root() {
            base.Root();
        }

        [TestMethod]
        public override void Save() {
            base.Save();
        }

        [TestMethod]
        public override void Show() {
            base.Show();
        }

        [TestMethod]
        public override void Where() {
            base.Where();
        }

        [TestMethod]
        public override void SpaceBarAutoComplete() {
            base.SpaceBarAutoComplete();
        }

        [TestMethod]
        public override void TabAutoComplete() {
            base.TabAutoComplete();
        }

        [TestMethod]
        public override void UnrecognisedCommand() {
            base.UnrecognisedCommand();
        }

        [TestMethod]
        public override void UpAndDownArrow() {
            base.UpAndDownArrow();
        }

        [TestMethod]
        public override void ScenarioEditAndSave() {
            base.ScenarioEditAndSave();
        }

        [TestMethod]
        public override void ScenarioMultiSelect() {
            base.ScenarioMultiSelect();
        }

        [TestMethod]
        public override void ScenarioTransientObject() {
            base.ScenarioTransientObject();
        }

        [TestMethod]
        public override void ScenarioUsingClipboard() {
            base.ScenarioUsingClipboard();
        }

        [TestMethod]
        public override void ScenarioTestEditableVM() {
            base.ScenarioTestEditableVM();
        }

        [TestMethod]
        public override void ChainedCommands() {
            base.ChainedCommands();
        }
    }

    #region Mega tests

    public abstract class MegaCiceroTestsRoot : CiceroTestRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void CiceroTests() {
            Action();
            BackAndForward();
            Cancel();
            Edit();
            Enter();
            Gemini();
            Goto();
            Help();
            Menu();
            OK();
            Page();
            Root();
            Save();
            Show();
            Where();
            Clipboard();
            SpaceBarAutoComplete();
            TabAutoComplete();
            UnrecognisedCommand();
            UpAndDownArrow();
            ChainedCommands();
            ScenarioEditAndSave();
            ScenarioMultiSelect();
            ScenarioTestEditableVM();
            ScenarioUsingClipboard();
            ScenarioTransientObject();
        }

        //[TestMethod]
        [Priority(-1)]
        public void ProblematicCiceroTests() {
            LaunchCiceroFromIcon();
        }
    }

    //[TestClass]
    public class MegaCiceroTestsFirefox : MegaCiceroTestsRoot {
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

    //[TestClass] TODO too many failures
    public class MegaCiceroTestsIe : MegaCiceroTestsRoot {
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

    [TestClass]
    public class MegaCiceroTestsChrome : MegaCiceroTestsRoot {
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