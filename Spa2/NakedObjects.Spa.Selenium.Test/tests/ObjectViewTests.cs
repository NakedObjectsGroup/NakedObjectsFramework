// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Selenium {
    public abstract class ObjectViewTestsRoot : AWTest {
        public virtual void ActionsAlreadyOpen() {
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            OpenSubMenu("Orders");
            var actions = GetObjectActions(7);
            Assert.AreEqual("Review Sales Responsibility", actions[0].GetAttribute("value"));
            Assert.AreEqual("Create New Order", actions[1].GetAttribute("value"));
            Assert.AreEqual("Quick Order", actions[2].GetAttribute("value"));
            Assert.AreEqual("Search For Orders", actions[3].GetAttribute("value"));
            Assert.AreEqual("Last Order", actions[4].GetAttribute("value"));
            Assert.AreEqual("Open Orders", actions[5].GetAttribute("value"));
            Assert.AreEqual("Recent Orders", actions[6].GetAttribute("value"));
        }

        public virtual void OpenActionsMenuNotAlreadyOpen() {
            GeminiUrl("object?o1=___1.Customer--309");
            WaitForView(Pane.Single, PaneType.Object, "The Gear Store, AW00000309");
            OpenObjectActions();
            OpenSubMenu("Orders");
            GetObjectActions(7);
        }

        public virtual void OpenAndCloseSubMenusTo3Levels() {
            GeminiUrl("object?i1=View&o1=___1.ProductInventory--320--1&as1=open");
            AssertActionNotDisplayed("Action1");
            OpenSubMenu("Sub Menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            OpenSubMenu("Level 2 sub menu");
            GetObjectAction("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            OpenSubMenu("Level 3 sub menu");
            GetObjectAction("Action1");
            GetObjectAction("Action2");
            GetObjectAction("Action3");
            GetObjectAction("Action4");
            CloseSubMenu("Level 3 sub menu");
            GetObjectAction("Action1");
            GetObjectAction("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            CloseSubMenu("Level 2 sub menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
            CloseSubMenu("Sub Menu");
            AssertActionNotDisplayed("Action1");
            AssertActionNotDisplayed("Action2");
            AssertActionNotDisplayed("Action3");
            AssertActionNotDisplayed("Action4");
        }

        public virtual void Properties() {
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 4);

            var properties = br.FindElements(By.CssSelector(".property"));
            // failing because of stale properties
            Assert.AreEqual("Store Name:\r\nTwin Cycles", properties[0].Text);
            properties = br.FindElements(By.CssSelector(".property"));
            Assert.AreEqual("Demographics:\r\nAnnualSales: 800000\r\nAnnualRevenue: 80000\r\nBankName: International Security\r\nBusinessType: BM\r\nYearOpened: 1988\r\nSpecialty: Touring\r\nSquareFeet: 21000\r\nBrands: AW\r\nInternet: T1\r\nNumberEmployees: 11", properties[1].Text);
            properties = br.FindElements(By.CssSelector(".property"));
            Assert.AreEqual("Sales Person:\r\nLynn Tsoflias", properties[2].Text);
            properties = br.FindElements(By.CssSelector(".property"));
            Assert.IsTrue(properties[3].Text.StartsWith("Modified Date:\r\n13 Oct 2008"));
        }

        public virtual void Collections() {
            GeminiUrl("object?i1=View&o1=___1.Product--821");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[0].Text.StartsWith("Product Inventory:\r\n2 Items"));
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[1].Text.StartsWith("Product Reviews:\r\nEmpty"));
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[2].Text.StartsWith("Special Offers:\r\n1 Item"));
        }

        public virtual void CollectionEagerlyRendered() {
            GeminiUrl("object?r=0&i1=View&o1=___1.Product--464");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[0].Text.StartsWith("Product Inventory:\r\n3 Items"));
            var cols = WaitForCss("th", 4).ToArray();
            Assert.AreEqual("Quantity", cols[1].Text);
            Assert.AreEqual("Location", cols[2].Text);
            Assert.AreEqual("Shelf", cols[3].Text);
            Assert.AreEqual("Bin", cols[4].Text);
            WaitForCss("tbody tr", 3);
        }

        public virtual void NonNavigableReferenceProperty() {
            //Tests properties of types that have had the NonNavigable Facet added to them 
            //(in this case by the custom AdventureWorksNotNavigableFacetFactory)
            GeminiUrl("object?i1=View&o1=___1.Product--869");
            var props = WaitForCss(".property", 23);
            //First test a navigable reference
            Assert.AreEqual("Product Model:\r\nWomen's Mountain Shorts", props[4].Text);

            // get again as goes stale
            props = WaitForCss(".property", 23);
            var link = props[4].FindElement(By.CssSelector(".reference"));
            Assert.IsNotNull(link);

            //Now a non-navigable one
            var cat = props[6];
            Assert.AreEqual("Product Category:\r\nClothing", cat.Text);
            var links = cat.FindElements(By.CssSelector(".reference.clickable-area"));
            Assert.AreEqual(0, links.Count);
        }

        public virtual void DateAndCurrencyProperties() {
            GeminiUrl("object?o1=___1.SalesOrderHeader--68389");
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 23);
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            //By default a DateTime property is rendered as date only:
            Assert.AreEqual("Order Date:\r\n16 Apr 2008", properties[8].Text);

            //If marked with ConcurrencyCheck, rendered as date time
            Assert.IsTrue(properties[22].Text.StartsWith("Modified Date:\r\n23 Apr 2008"));
            Assert.IsTrue(properties[22].Text.EndsWith(":00:00")); //Only check mm:ss to avoid TimeZone difference server vs. client

            //Currency properties formatted to 2 places & with default currency symbok (£)
            Assert.AreEqual("Sub Total:\r\n£819.31", properties[11].Text);
        }

        public virtual void TableViewHonouredOnCollection() {
            GeminiUrl("object?i1=View&o1=___1.Employee--83&c1_DepartmentHistory=Summary&c1_PayHistory=Table");
            var cols = WaitForCss("th", 3).ToArray();
            Assert.AreEqual(4, cols.Length);
            Assert.AreEqual("", cols[1].Text); //Title
            Assert.AreEqual("Rate Change Date", cols[2].Text);
            Assert.AreEqual("Rate", cols[3].Text);

            //Dates formatted in table view
            GeminiUrl("object?i1=View&o1=___1.Product--775&c1_SpecialOffers=Table&c1_ProductInventory=List");
            WaitForCss("td", 15);
            var cell = WaitForCss("td:nth-child(6)");
            Assert.AreEqual("31 Dec 2008", cell.Text);
        }

        public virtual void TableViewIgnoresDuplicatedColumnName() {
            GeminiUrl("object?i1=View&r=1&o1=___1.SalesPerson--280&c1_QuotaHistory=Table");
            WaitForView(Pane.Single, PaneType.Object); //i.e. not an error (c.f. bug #57)
        }

        public virtual void ClickReferenceProperty() {
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
            Click(reference);
            WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }

        public virtual void OpenCollectionAsList() {
            GeminiUrl("object?i1=View&o1=___1.Employee--5");
            WaitForCss(".collection", 2);
            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);
            WaitForCss("table");
            // cancel table view 
            Click(WaitForCssNo(".icon-summary", 0));
            WaitUntilGone(d => d.FindElement(By.CssSelector(".table")));
        }

        public virtual void NotCountedCollection() {
            //Test NotCounted collection
            GeminiUrl("object?i1=View&o1=___1.Vendor--1662");
            WaitForView(Pane.Single, PaneType.Object, "Northern Bike Travel");
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text == "Product - Order Info:");
            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);
            WaitForCss("table");
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text.Contains("1 Item"));
            // cancel table view 
            Click(WaitForCss(".icon-summary"));
            WaitUntilGone(dr => dr.FindElement(By.CssSelector(".table")));

            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text == "Product - Order Info:");
        }
        //#60 bug caused by cache
        public virtual void CollectionsUpdateProperly()
        {
            //Open Reasons collection as  List 
            GeminiUrl("object?i1=View&r=1&o1=___1.SalesOrderHeader--65709&c1_SalesOrderHeaderSalesReason=List&as1=open");
            //Now open as table, to initiate the caching
            Click(WaitForCssNo(".collection .icon-table", 1));
            WaitForCss("thead tr th", 5);

            // Go back to summary
            Click(WaitForCssNo(".icon-summary", 0));
            WaitUntilElementDoesNotExist("table");
            //Add a new reason from menu action
            OpenActionDialog("Add New Sales Reasons");
            br.FindElement(By.CssSelector(".value  select option[label='Review']")).Click();
            wait.Until(dr => new SelectElement(WaitForCss("select#reasons1")).AllSelectedOptions.Count == 1);
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection .summary .details"))[1].Text == "2 Items");
            //Open table view and confirm that there are indeed 2 rows
            Click(WaitForCssNo(".collection .icon-table", 1));
            WaitForCss("thead tr th", 5);
            WaitForCss("tbody tr", 2);  //bug #60: only one row showed

            //Attempt to leave object as we found it
            OpenActionDialog("Remove Sales Reason");
            br.FindElement(By.CssSelector(".value  select option[label='Review']")).Click();
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection .summary .details"))[1].Text == "1 Item");
        }
        //#60 - test orginal version of bug involving NotCounted
        public virtual void NotCountedCollectionUpdatesCorrectly()
        {
            GeminiUrl("object?i1=View&r=1&o1=___1.Person--7489&as1=open&d1=CreateNewPhoneNumber");
            SelectDropDownOnField("#type1", "Cell");
            var rnd = new Random();
            var num = rnd.Next(1, 1000000).ToString();
            var title = "Cell: " + num;
            TypeIntoFieldWithoutClearing("#phonenumber1", num);
            Click(OKButton());
            Click(WaitForCssNo(".collection .icon-list", 2));

            wait.Until(dr => dr.FindElements(By.CssSelector("table tbody tr td")).Any(el => el.Text == title));
            Click(WaitForCssNo(".collection .icon-table", 0));

            wait.Until(dr => dr.FindElements(By.CssSelector("table tbody tr td")).Any(el => el.Text == num));
        }
        public virtual void ClickOnLineItemWithCollectionAsList() {
            var testUrl = GeminiBaseUrl + "object?o1=___1.Store--350&as1=open" + "&c1_Addresses=List";
            Url(testUrl);
            var row = WaitForCss("table .reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "Main Office: 2253-217 Palmer Street ...");
        }

        public virtual void ClickOnLineItemWithCollectionAsTable() {
            var testUrl = GeminiBaseUrl + "object?o1=___1.Store--350&as1=open" + "&c1_Addresses=Table&c1_Contacts=Summary";
            Url(testUrl);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            wait.Until(dr => row.FindElements(By.CssSelector(".cell")).Count >= 2);

            var type = row.FindElements(By.CssSelector(".cell"))[0].Text;
            var addr = row.FindElements(By.CssSelector(".cell"))[1].Text;
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, type + ": " + addr);
        }

        public virtual void CanClickOnTitleInTableView() {
            GeminiUrl("object?i1=View&r=1&o1=___1.Employee--56&c1_DepartmentHistory=Summary&c1_PayHistory=Table");
            WaitForView(Pane.Single, PaneType.Object, "Denise Smith");
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            wait.Until(dr => row.FindElements(By.CssSelector(".cell")).Count >= 3);

            var title = row.FindElements(By.CssSelector(".cell"))[0];
            Click(title);
            WaitForView(Pane.Single, PaneType.Object, "$11.00 from 3/9/2003");
        }

        public virtual void QueryOnlyActionDoesNotReloadAutomatically() {
            GeminiUrl("object?o1=___1.Person--8410&as1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(6) .value").Text;
            OpenActionDialog("Update Suffix"); //This is deliberately wrongly marked up as QueryOnly
            WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue);
            Click(OKButton()); //This will have updated server, but not client-cached object
            //Go and do something else, so screen changes, then back again
            wait.Until(dr => dr.FindElements(By.CssSelector(".dialog")).Count == 0);
            Click(br.FindElement(By.CssSelector(".icon-home")));
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Click(br.FindElement(By.CssSelector(".icon-back")));
            WaitForView(Pane.Single, PaneType.Object);
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(6) .value")).Text == original);
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(6) .value")).Text == newValue);
        }

        public virtual void PotentActionDoesReloadAutomatically() {
            GeminiUrl("object?o1=___1.Person--8410&as1=open");
            WaitForView(Pane.Single, PaneType.Object);
            WaitForCss(".property:nth-child(3) .value");
            OpenActionDialog("Update Middle Name");
            WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue);
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(3) .value")).Text == newValue);
        }

        public virtual void Colours() {
            //Specific type matches
            GeminiUrl("object?i1=View&o1=___1.Customer--226");
            WaitForView(Pane.Single, PaneType.Object, "Leisure Activities, AW00000226");
            WaitForCss(".object.object-color1");
            GeminiUrl("object?i1=View&o1=___1.Product--404");
            WaitForView(Pane.Single, PaneType.Object, "External Lock Washer 4");
            WaitForCss(".object.object-color4");

            //Regex matches
            GeminiUrl("object?i1=View&o1=___1.SalesOrderHeader--59289&c1_Details=List");
            WaitForView(Pane.Single, PaneType.Object, "SO59289");
            WaitForCss(".object.object-color2");
            WaitForCss("tr", 2);
            wait.Until(dr => dr.FindElements(By.CssSelector("tr.link-color2")).Count == 2);
            GeminiUrl("object?i1=View&o1=___1.SalesOrderDetail--59289--71041");
            WaitForView(Pane.Single, PaneType.Object, "1 x Mountain-400-W Silver, 46");
            WaitForCss(".object.object-color2");

            //SubType matching
            GeminiUrl("object?i1=View&o1=___1.Person--3238");
            WaitForView(Pane.Single, PaneType.Object, "Maria Cox");
            WaitForCss(".object.object-color8");
            GeminiUrl("object?i1=View&o1=___1.Store--1334");
            WaitForView(Pane.Single, PaneType.Object, "Chic Department Stores");
            WaitForCss(".object.object-color8");

            //Default
            GeminiUrl("object?i1=View&o1=___1.PersonCreditCard--10768--6875");
            WaitForCss(".object.object-color0");
            var cc = GetReferenceFromProperty("Credit Card");
            Assert.IsTrue(cc.GetAttribute("class").Contains("link-color0"));
        }

        public virtual void ZeroIntValues() {
            GeminiUrl("object?i1=View&o1=___1.SpecialOffer--13");
            WaitForView(Pane.Single, PaneType.Object, "Touring-3000 Promotion");
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Max Qty:", properties[7].Text);
            Assert.AreEqual("Min Qty:\r\n0", properties[6].Text);
        }

        public virtual void AddingObjectToCollectionUpdatesTableView() {
            GeminiUrl("object?i1=View&o1=___1.SalesPerson--276&as1=open&c1_QuotaHistory=Table&d1=ChangeSalesQuota");
            var details = WaitForCssNo(".summary .details", 0).Text;
            Assert.IsTrue(details.Contains("Item"));
            var rowCount = Int32.Parse(details.Split(' ')[0]);
            WaitForCss("tbody tr", rowCount);
            Assert.AreEqual(rowCount, br.FindElements(By.CssSelector("tbody"))[0].FindElements(By.CssSelector("tr")).Count);
            ClearFieldThenType("#newquota1", "345");
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector("tbody"))[0].FindElements(By.CssSelector("tr")).Count >= rowCount + 1);
            Assert.AreEqual(rowCount + 1, br.FindElements(By.CssSelector("tbody"))[0].FindElements(By.CssSelector("tr")).Count);
        }

        public virtual void TimeSpanProperty() {
            GeminiUrl("object?i1=View&o1=___1.Shift--2");
            WaitForTextEquals(".property", 2, "Start Time:\r\n15:00");
        }

        #region Actions

        public virtual void DialogAction() {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            OpenActionDialog("Search For Orders");
        }

        public virtual void DialogActionOk() {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            var dialog = OpenActionDialog("Search For Orders", Pane.Single, 2);
            GetInputNumber(dialog, 0).SendKeys("1 Jan 2003");
            GetInputNumber(dialog, 1).SendKeys("1 Dec 2003" + Keys.Escape);
            dialog.FindElements(By.CssSelector(".parameter .value input"))[0].SendKeys("01/01/2003");
            dialog.FindElements(By.CssSelector(".parameter .value input"))[1].SendKeys("01/12/2003" + Keys.Escape);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Search For Orders");
        }

        public virtual void ObjectAction() {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            Click(GetObjectAction("Last Order"));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
        }

        public virtual void CollectionAction() {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            Click(GetObjectAction("Recent Orders"));
            WaitForView(Pane.Single, PaneType.List, "Recent Orders");
        }

        public virtual void DescriptionRenderedAsTooltip() {
            GeminiUrl("home?m1=SalesRepository");
            var a = GetObjectAction("Create New Sales Person");
            Assert.AreEqual("... from an existing Employee", a.GetAttribute("title"));
        }

        public virtual void DisabledAction() {
            GeminiUrl("object?o1=___1.SalesOrderHeader--43893&as1=open");
            //First the control test
            GetObjectAction("Add New Sales Reasons").AssertIsEnabled();

            //Then the real test
            GetObjectAction("Recalculate").AssertIsDisabled("Can only recalculate an 'In Process' order");
        }

        public virtual void ActionsMenuDisabledOnObjectWithNoActions() {
            GeminiUrl("object?o1=___1.Address--21467");
            WaitForView(Pane.Single, PaneType.Object, "3022 Terra Calitina ...");
            var actions = wait.Until(dr => dr.FindElement(By.CssSelector(".header input[value='Actions']")));
            Assert.AreEqual("true", actions.GetAttribute("disabled"));
        }

        public virtual void ZeroParamActionCausesObjectToReload() {
            GeminiUrl("object?i1=View&o1=___1.SalesOrderHeader--72079&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "SO72079");
            // clear any existing comments to make test more robust

            var rn = br.FindElements(By.CssSelector(".property"))[19].Text;

            Click(GetObjectAction("Clear Comment"));
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[19].Text != rn);
            //First set up some comments
            OpenActionDialog("Add Standard Comments");
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Payment on delivery"));
            //Now clear them
            Click(GetObjectAction("Clear Comment"));
            // wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text == "Comment:");
        }

        #endregion

        #region Interlocks on potent actions

        //Context for these tests:
        //To guard against, for example, accidentally double-clicking on a single
        //potent action, or attempting to invoke another action before a slow one has completed.
        //The UI should disable potent actions (zero param or OK on open dialog) until first action has
        //completed.
        public virtual void CanInvokeOneNonPotentActionBeforePreviousHasCompleted() {
            GeminiUrl("object?i1=View&o1=___1.Customer--389&as1=open");
            OpenSubMenu("Orders");
            var open = GetObjectAction("Open Orders");
            var recent = GetObjectAction("Recent Orders");
            RightClick(open);
            RightClick(recent); //i.e. in rapid succession
            WaitForView(Pane.Right, PaneType.List, "Recent Orders");
        }

        public virtual void CannotInvokeAPotentActionUntilPriorOneHasCompleted() {
            GeminiUrl("object?r=0&i1=View&o1=___1.SalesOrderHeader--51863&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "SO51863");
            // clear existing comments
            //Click(GetObjectAction("Clear Comment"));
            //wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Sales Person")); //i.e. Comments property has disappeared
            //Set up a comment
            OpenActionDialog("Add Standard Comments");
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Payment on delivery"));
            
            Thread.Sleep(1000); // sleep here because otherwise clear button may be still disabled from OK
            var clear = GetObjectAction("Clear Comment");
            Click(clear);
            Click(clear); //i.e. twice in rapid succession
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Sales Person")); //i.e. Comments property has disappeared
        }

        public virtual void UpdatingObjectWhileAPotentDialogIsOpenCausesEtagToBeRefreshed() {
            GeminiUrl("object?i1=View&o1=___1.SalesOrderHeader--69143&as1=open&r=0");
            Click(GetObjectAction("Clear Comment"));
            WaitUntilElementDoesNotExist(".tempdisabled");
            OpenActionDialog("Add Standard Comments");
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Payment on delivery"));

            OpenActionDialog("Add Standard Comments");

            Click(GetObjectAction("Clear Comment"));

            WaitForCss("nof-action .tempdisabled");
            //wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Sales Person")); //i.e. Comments property has disappeared

            Click(OKButton()); //On dialog that has remained open

            // ie comment was not cleared 
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[20].Text.Contains("Payment on delivery"));
        }

        #endregion
    }

    public abstract class ObjectViewTests : ObjectViewTestsRoot {
        [TestMethod]
        public override void ActionsAlreadyOpen() {
            base.ActionsAlreadyOpen();
        }

        [TestMethod]
        public override void OpenActionsMenuNotAlreadyOpen() {
            base.OpenActionsMenuNotAlreadyOpen();
        }

        [TestMethod]
        public override void OpenAndCloseSubMenusTo3Levels() {
            base.OpenAndCloseSubMenusTo3Levels();
        }

        [TestMethod]
        public override void Properties() {
            base.Properties();
        }

        [TestMethod]
        public override void Collections() {
            base.Collections();
        }

        [TestMethod]
        public override void CollectionEagerlyRendered() {
            base.CollectionEagerlyRendered();
        }

        [TestMethod]
        public override void DateAndCurrencyProperties() {
            base.DateAndCurrencyProperties();
        }

        [TestMethod]
        public override void TableViewHonouredOnCollection() {
            base.TableViewHonouredOnCollection();
        }

        [TestMethod]
        public override void TableViewIgnoresDuplicatedColumnName() {
            base.TableViewIgnoresDuplicatedColumnName();
        }

        [TestMethod]
        public override void ClickReferenceProperty() {
            base.ClickReferenceProperty();
        }

        [TestMethod]
        public override void OpenCollectionAsList() {
            base.OpenCollectionAsList();
        }

        [TestMethod]
        public override void NotCountedCollection() {
            base.NotCountedCollection();
        }

        [TestMethod]
        public override void NotCountedCollectionUpdatesCorrectly()
        {
            base.NotCountedCollectionUpdatesCorrectly();
        }

        [TestMethod]
        public override void CollectionsUpdateProperly()
        {
            base.CollectionsUpdateProperly();
        }

        [TestMethod]
        public override void ClickOnLineItemWithCollectionAsList() {
            base.ClickOnLineItemWithCollectionAsList();
        }

        [TestMethod]
        public override void ClickOnLineItemWithCollectionAsTable() {
            base.ClickOnLineItemWithCollectionAsTable();
        }

        [TestMethod]
        public override void CanClickOnTitleInTableView() {
            base.CanClickOnTitleInTableView();
        }

        [TestMethod]
        public override void DialogAction() {
            base.DialogAction();
        }

        [TestMethod]
        public override void DialogActionOk() {
            base.DialogActionOk();
        }

        [TestMethod]
        public override void ObjectAction() {
            base.ObjectAction();
        }

        [TestMethod]
        public override void CollectionAction() {
            base.CollectionAction();
        }

        [TestMethod]
        public override void DescriptionRenderedAsTooltip() {
            base.DescriptionRenderedAsTooltip();
        }

        [TestMethod]
        public override void DisabledAction() {
            base.DisabledAction();
        }

        [TestMethod]
        public override void ActionsMenuDisabledOnObjectWithNoActions() {
            base.ActionsMenuDisabledOnObjectWithNoActions();
        }

        [TestMethod]
        public override void QueryOnlyActionDoesNotReloadAutomatically() {
            base.QueryOnlyActionDoesNotReloadAutomatically();
        }

        [TestMethod]
        public override void PotentActionDoesReloadAutomatically() {
            base.PotentActionDoesReloadAutomatically();
        }

        [TestMethod]
        public override void NonNavigableReferenceProperty() {
            base.NonNavigableReferenceProperty();
        }

        [TestMethod]
        public override void Colours() {
            base.Colours();
        }

        [TestMethod]
        public override void ZeroIntValues() {
            base.ZeroIntValues();
        }

        [TestMethod]
        public override void AddingObjectToCollectionUpdatesTableView() {
            base.AddingObjectToCollectionUpdatesTableView();
        }

        [TestMethod]
        public override void TimeSpanProperty() {
            base.TimeSpanProperty();
        }

        [TestMethod]
        public override void ZeroParamActionCausesObjectToReload() {
            base.ZeroParamActionCausesObjectToReload();
        }

        [TestMethod]
        public override void UpdatingObjectWhileAPotentDialogIsOpenCausesEtagToBeRefreshed() {
            base.UpdatingObjectWhileAPotentDialogIsOpenCausesEtagToBeRefreshed();
        }

        [TestMethod]
        public override void CannotInvokeAPotentActionUntilPriorOneHasCompleted() {
            base.CannotInvokeAPotentActionUntilPriorOneHasCompleted();
        }

        [TestMethod]
        public override void CanInvokeOneNonPotentActionBeforePreviousHasCompleted() {
            base.CanInvokeOneNonPotentActionBeforePreviousHasCompleted();
        }
    }

    #region browsers specific subclasses

    public class ObjectViewTestsIe : ObjectViewTests {
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

    //[TestClass] //Firefox Individual
    public class ObjectViewTestsFirefox : ObjectViewTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = $"window.scrollTo({element.Location.X}, {element.Location.Y});return true;";
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    //[TestClass]
    public class ObjectViewTestsChrome : ObjectViewTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            CleanUpTest();
        }
    }

    #endregion

    #region Mega tests

    public abstract class MegaObjectViewTestsRoot : ObjectViewTestsRoot {
        [TestMethod] //Mega
        public void MegaObjectViewTest() {
            //ActionsAlreadyOpen();
            //OpenActionsMenuNotAlreadyOpen();
            //OpenAndCloseSubMenusTo3Levels();
            //Properties();
            //Collections();
            //CollectionEagerlyRendered();
            //DateAndCurrencyProperties();
            //TableViewHonouredOnCollection();
            //TableViewIgnoresDuplicatedColumnName();
            //ClickReferenceProperty();
            //// OpenCollectionAsList();  move to LocallyRun 
            //NotCountedCollection();
            //ClickOnLineItemWithCollectionAsList();
            //ClickOnLineItemWithCollectionAsTable();
            //CanClickOnTitleInTableView();
            //DialogAction();
            //DialogActionOk();
            //ObjectAction();
            //CollectionAction();
            //DescriptionRenderedAsTooltip();
            //DisabledAction();
            //ActionsMenuDisabledOnObjectWithNoActions();
            //QueryOnlyActionDoesNotReloadAutomatically();
            //PotentActionDoesReloadAutomatically();
            //NonNavigableReferenceProperty();
            //Colours();
            //ZeroIntValues();
            //AddingObjectToCollectionUpdatesTableView();
            ////TimeSpanProperty();
            //ZeroParamActionCausesObjectToReload();

            //CanInvokeOneNonPotentActionBeforePreviousHasCompleted();
            //UpdatingObjectWhileAPotentDialogIsOpenCausesEtagToBeRefreshed();
            CannotInvokeAPotentActionUntilPriorOneHasCompleted();

            //CollectionsUpdateProperly();  move to LocallyRun 
            //NotCountedCollectionUpdatesCorrectly();  move to LocallyRun 
        }
    }

    //[TestClass]
    public class MegaObjectViewTestFirefox : MegaObjectViewTestsRoot {
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
    public class MegaObjectViewTestIe : MegaObjectViewTestsRoot {
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
    public class MegaObjectViewTestChrome : MegaObjectViewTestsRoot {
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