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

namespace NakedObjects.Web.UnitTests.Selenium
{

    public abstract class ObjectViewTestsRoot : AWTest
    {
        public virtual void ActionsAlreadyOpen()
        {
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles, AW00000555");
            OpenSubMenu("Orders");
            var actions = GetObjectActions(7);
            Assert.AreEqual("Review Sales Responsibility", actions[0].Text);
            Assert.AreEqual("Create New Order", actions[1].Text);
            Assert.AreEqual("Quick Order", actions[2].Text);
            Assert.AreEqual("Search For Orders", actions[3].Text);
            Assert.AreEqual("Last Order", actions[4].Text);
            Assert.AreEqual("Open Orders", actions[5].Text);
            Assert.AreEqual("Recent Orders", actions[6].Text);
        }
        public virtual void OpenActionsMenuNotAlreadyOpen()
        {
            GeminiUrl("object?o1=___1.Customer--309");
            WaitForView(Pane.Single, PaneType.Object, "The Gear Store, AW00000309");
            OpenObjectActions();
            OpenSubMenu("Orders");
            GetObjectActions(7);
        }
        public virtual void OpenAndCloseSubMenusTo2Levels()
        {
            GeminiUrl("object?i1=View&o1=___1.ProductInventory--320--1&as1=open");
            AssertActionNotDisplayed("Action1");
            OpenSubMenu("Sub Menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            OpenSubMenu("Level 2 sub menu");
            GetObjectAction("Action2");
            CloseSubMenu("Level 2 sub menu");
            GetObjectAction("Action1");
            AssertActionNotDisplayed("Action2");
            CloseSubMenu("Sub Menu");
            AssertActionNotDisplayed("Action1");
            AssertActionNotDisplayed("Action2");
        }
        public virtual void Properties()
        {
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            wait.Until(dr => dr.FindElement(By.CssSelector(".object")));
            wait.Until(dr => dr.FindElement(By.CssSelector(".view")).Displayed == true);
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 4);

            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Store Name:\r\nTwin Cycles", properties[0].Text);
            Assert.AreEqual("Demographics:\r\nAnnualSales: 800000\r\nAnnualRevenue: 80000\r\nBankName: International Security\r\nBusinessType: BM\r\nYearOpened: 1988\r\nSpecialty: Touring\r\nSquareFeet: 21000\r\nBrands: AW\r\nInternet: T1\r\nNumberEmployees: 11", properties[1].Text);
            Assert.AreEqual("Sales Person:\r\nLynn Tsoflias", properties[2].Text);
            Assert.IsTrue(properties[3].Text.StartsWith("Modified Date:\r\n13 Oct 2008"));
        }
        public virtual void Collections() { 
            //Test collection count
            GeminiUrl("object?i1=View&o1=___1.Product--821");
            wait.Until(d => br.FindElements(By.CssSelector(".collection")).Count == 3);
            ReadOnlyCollection<IWebElement> collections = br.FindElements(By.CssSelector(".collection"));
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[0].Text == "Product Inventory:\r\n2 Items");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[1].Text == "Product Reviews:\r\nEmpty");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[2].Text.StartsWith("Special Offers:\r\n1 Item"));
        }
        public virtual void CollectionEagerlyRendered()
        {
            GeminiUrl("object?i1=View&o1=___1.WorkOrder--35410");
            wait.Until(d => br.FindElements(By.CssSelector(".collection"))[0].Text.StartsWith("Work Order Routings:\r\n1 Item"));
            var cols = WaitForCss("th", 6).ToArray();
            Assert.AreEqual("", cols[0].Text); //Title
            Assert.AreEqual("Operation Sequence", cols[1].Text);
            Assert.AreEqual("Planned Cost", cols[5].Text);
            WaitForCss("tbody tr", 1);
        }
        public virtual void NonNavigableReferenceProperty()
        {
            //Tests properties of types that have had the NonNavigable Facet added to them 
            //(in this case by the custom AdventureWorksNotNavigableFacetFactory)
            GeminiUrl("object?i1=View&o1=___1.Product--869");
            var props = WaitForCss(".property", 23);
            //First test a navigable reference
            var model = props[4];
            Assert.AreEqual("Product Model:\r\nWomen's Mountain Shorts", model.Text);
            var link = model.FindElement(By.CssSelector(".reference.clickable-area"));
            Assert.IsNotNull(link);

            //Now a non-navigable one
            var cat = props[6];
            Assert.AreEqual("Product Category:\r\nClothing", cat.Text);
            var links = cat.FindElements(By.CssSelector(".reference.clickable-area"));
            Assert.AreEqual(0, links.Count());
        }
        public virtual void DateAndCurrencyProperties() {
            GeminiUrl("object?o1=___1.SalesOrderHeader--68389");
            wait.Until(d => br.FindElements(By.CssSelector(".property")).Count >= 24);
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            //By default a DateTime property is rendered as date only:
            Assert.AreEqual("Order Date:\r\n16 Apr 2008", properties[8].Text);

            //If marked with ConcurrencyCheck, rendered as date time
            Assert.IsTrue(properties[23].Text.StartsWith("Modified Date:\r\n23 Apr 2008"));
            Assert.IsTrue(properties[23].Text.EndsWith(":00:00")); //Only check mm:ss to avoid TimeZone difference server vs. client

            //Currency properties formatted to 2 places & with default currency symbok (£)
            Assert.AreEqual("Sub Total:\r\n£819.31", properties[11].Text);
        }
        public virtual void TableViewHonouredOnCollection()
        {
            GeminiUrl("object?i1=View&o1=___1.Employee--83&c1_DepartmentHistory=Summary&c1_PayHistory=Table");
            var header = WaitForCss("thead");
            var cols = WaitForCss("th", 3).ToArray();
            Assert.AreEqual(3, cols.Length);
            Assert.AreEqual("", cols[0].Text); //Title
            Assert.AreEqual("Rate Change Date", cols[1].Text);
            Assert.AreEqual("Rate", cols[2].Text);

            //Dates formatted in table view
            GeminiUrl("object?i1=View&o1=___1.Product--775&c1_SpecialOffers=Table");
            WaitForCss("td", 15);
            var cell = WaitForCss("td:nth-child(5)");
            Assert.AreEqual("31 Dec 2008", cell.Text);
        }
        public virtual void ClickReferenceProperty()
        {
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
            Click(reference);
            WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }
        public virtual void OpenCollectionAsList()
        {
            GeminiUrl("object?i1=View&o1=___1.Employee--5");
            WaitForCss(".collection", 2);
            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);
            WaitForCss("table");
            // cancel table view 
            Click(WaitForCssNo(".icon-summary",0));
            WaitUntilGone(d => d.FindElement(By.CssSelector(".table")));           
        }
        public virtual void NotCountedCollection()
        {
            //Test NotCounted collection
            GeminiUrl("object?i1=View&o1=___1.Vendor--1662");
            WaitForView(Pane.Single, PaneType.Object, "Northern Bike Travel");

            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text.Contains("Unknown Size"));

            var iconList = WaitForCssNo(".collection .icon-list", 0);
            Click(iconList);
            WaitForCss("table");
            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text.Contains("1 Item"));

            //wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].Text == "Product - Order Info:\r\n1 Item");

            // cancel table view 
            Click(WaitForCss(".icon-summary"));
            WaitUntilGone(dr => dr.FindElement(By.CssSelector(".table")));

            wait.Until(dr => dr.FindElement(By.CssSelector(".collection")).Text.Contains("Unknown Size"));
        }
        public virtual void ClickOnLineItemWithCollectionAsList()
        {
            var testUrl = GeminiBaseUrl + "object?o1=___1.Store--350&as1=open" + "&c1_Addresses=List";
            Url(testUrl);
            var row = WaitForCss("table .reference");
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, "Main Office: 2253-217 Palmer Street ...");
        }
        public virtual void ClickOnLineItemWithCollectionAsTable()
        {
            var testUrl = GeminiBaseUrl + "object?o1=___1.Store--350&as1=open" + "&c1_Addresses=Table";
            Url(testUrl);
            var row = wait.Until(dr => dr.FindElement(By.CssSelector("table tbody tr")));
            wait.Until(dr => row.FindElements(By.CssSelector(".cell")).Count >= 2);

            var type = row.FindElements(By.CssSelector(".cell"))[0].Text;
            var addr = row.FindElements(By.CssSelector(".cell"))[1].Text;
            Click(row);
            WaitForView(Pane.Single, PaneType.Object, type + ": " + addr);
        }

        #region Actions
        public virtual void DialogAction()
        {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            OpenActionDialog("Search For Orders");
        }
        public virtual void DialogActionOk()
        {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            var dialog = OpenActionDialog("Search For Orders", Pane.Single, 2);
            GetInputNumber(dialog, 0).SendKeys("1 Jan 2003");
            GetInputNumber(dialog, 1).SendKeys("1 Dec 2003" + Keys.Escape);
            dialog.FindElements(By.CssSelector(".parameter .value input"))[0].SendKeys("1 Jan 2003");
            dialog.FindElements(By.CssSelector(".parameter .value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);
            Click(OKButton());
            WaitForView(Pane.Single, PaneType.List, "Search For Orders");
        }
        public virtual void ObjectAction()
        {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            Click(GetObjectAction("Last Order"));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
        }
        public virtual void CollectionAction()
        {
            GeminiUrl("home");
            GeminiUrl("object?o1=___1.Customer--555&as1=open");
            OpenSubMenu("Orders");
            Click(GetObjectAction("Recent Orders"));
            WaitForView(Pane.Single, PaneType.List, "Recent Orders");
        }
        public virtual void DescriptionRenderedAsTooltip()
        {
            GeminiUrl("home?m1=SalesRepository");
            var a = GetObjectAction("Create New Sales Person");
            Assert.AreEqual("... from an existing Employee", a.GetAttribute("title"));
        }
        public virtual void DisabledAction()
        {
            GeminiUrl("object?o1=___1.SalesOrderHeader--43893&as1=open");
            //First the control test
            GetObjectAction("Add New Sales Reason").AssertIsEnabled();

            //Then the real test
            GetObjectAction("Add New Detail").AssertIsDisabled("Can only add to 'In Process' order");
        }
        public virtual void ActionsMenuDisabledOnObjectWithNoActions()
        {
            GeminiUrl("object?o1=___1.Address--21467");
            WaitForView(Pane.Single, PaneType.Object, "3022 Terra Calitina ...");
            var actions = wait.Until(dr => dr.FindElement(By.CssSelector(".header .menu")));
            Assert.AreEqual("true", actions.GetAttribute("disabled"));
        }
        #endregion
        public virtual void QueryOnlyActionDoesNotReloadAutomatically()
        {
            GeminiUrl("object?o1=___1.Person--8410&as1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(6) .value").Text;
            var dialog = OpenActionDialog("Update Suffix"); //This is deliberately wrongly marked up as QueryOnly
            var field1 = WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue);
            Click(OKButton()); //This will have updated server, but not client-cached object
            //Go and do something else, so screen changes, then back again
            wait.Until(dr => dr.FindElements(By.CssSelector(".dialog")).Count == 0);
            GeminiUrl("");
            WaitForView(Pane.Single, PaneType.Home);
            Click(br.FindElement(By.CssSelector(".icon-back")));
            WaitForView(Pane.Single, PaneType.Object);
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(6) .value")).Text == original);
            Reload();
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(6) .value")).Text == newValue);
        }
        public virtual void PotentActionDoesReloadAutomatically()
        {
            GeminiUrl("object?o1=___1.Person--8410&as1=open");
            WaitForView(Pane.Single, PaneType.Object);
            var original = WaitForCss(".property:nth-child(3) .value").Text;
            var dialog = OpenActionDialog("Update Middle Name"); 
            var field1 = WaitForCss(".parameter:nth-child(1) input");
            var newValue = DateTime.Now.Millisecond.ToString();
            ClearFieldThenType(".parameter:nth-child(1) input", newValue);
            Click(OKButton());
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(3) .value")).Text == newValue);
        }
        public virtual void Colours()
        {
            //Specific type matches
            GeminiUrl("object?i1=View&o1=___1.Customer--226");
            WaitForView(Pane.Single, PaneType.Object, "Leisure Activities, AW00000226");
            WaitForCss(".object.object-color1");
            var terr = GetReferenceFromProperty("Sales Territory");
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
        public virtual void ZeroIntValues()
        {
            GeminiUrl("object?i1=View&o1=___1.SpecialOffer--13");
            WaitForView(Pane.Single, PaneType.Object, "Touring-3000 Promotion");
            ReadOnlyCollection<IWebElement> properties = br.FindElements(By.CssSelector(".property"));

            Assert.AreEqual("Max Qty:", properties[7].Text);
            Assert.AreEqual("Min Qty:\r\n0", properties[6].Text);

        }

        //Test for a specific earlier bug
        public virtual void AddingObjectToCollectionUpdatesTableView()
        {
            GeminiUrl("object?i1=View&o1=___1.WorkOrder--10321&as1=open&d1=AddNewRouting");
            var details = WaitForCss(".summary .details").Text;
            Assert.IsTrue(details.Contains("Item"));
            var rowCount = Int32.Parse(details.Split(' ')[0]);
            WaitForCss("tbody tr", rowCount);
            Assert.AreEqual(rowCount, br.FindElements(By.CssSelector("tbody tr")).Count);
            SelectDropDownOnField("#loc1", "Tool Crib");
            Click(OKButton());
            wait.Until(dr => dr.FindElements(By.CssSelector("tbody tr")).Count >= rowCount + 1);
            Assert.AreEqual(rowCount + 1, br.FindElements(By.CssSelector("tbody tr")).Count);
        }

        public virtual void TimeSpanProperty()
        {
            GeminiUrl("object?i1=View&o1=___1.Shift--965");
            WaitForTextEquals(".property", 2, "07:00"); //TODO value not correct
        }
    }
    public abstract class ObjectViewTests : ObjectViewTestsRoot
    {
        [TestMethod]
        public override void ActionsAlreadyOpen() { base.ActionsAlreadyOpen(); }
        [TestMethod]
        public override void OpenActionsMenuNotAlreadyOpen() { base.OpenActionsMenuNotAlreadyOpen(); }
        [TestMethod]
        public override void OpenAndCloseSubMenusTo2Levels() { base.OpenAndCloseSubMenusTo2Levels(); }
        [TestMethod]
        public override void Properties() { base.Properties(); }
        [TestMethod]
        public override void Collections() { base.Collections(); }
        [TestMethod]
        public override void CollectionEagerlyRendered() { base.CollectionEagerlyRendered(); }
        [TestMethod]
        public override void DateAndCurrencyProperties() { base.DateAndCurrencyProperties(); }
       
        [TestMethod]
        public override void TableViewHonouredOnCollection() { base.TableViewHonouredOnCollection(); }

        [TestMethod]
        public override void ClickReferenceProperty() { base.ClickReferenceProperty(); }

        [TestMethod]
        public override void OpenCollectionAsList() { base.OpenCollectionAsList(); }
        [TestMethod]
        public override void NotCountedCollection() { base.NotCountedCollection(); }

        [TestMethod]
        public override void ClickOnLineItemWithCollectionAsList() { base.ClickOnLineItemWithCollectionAsList(); }
        [TestMethod]
        public override void ClickOnLineItemWithCollectionAsTable() { base.ClickOnLineItemWithCollectionAsTable(); }
        [TestMethod]
        public override void DialogAction() { base.DialogAction(); }
        [TestMethod]
        public override void DialogActionOk() { base.DialogActionOk(); }
        [TestMethod]
        public override void ObjectAction() { base.ObjectAction(); }
        [TestMethod]
        public override void CollectionAction() { base.CollectionAction(); }
        [TestMethod]
        public override void DescriptionRenderedAsTooltip() { base.DescriptionRenderedAsTooltip(); }
        [TestMethod]
        public override void DisabledAction() { base.DisabledAction(); }
        [TestMethod]
        public override void ActionsMenuDisabledOnObjectWithNoActions() { base.ActionsMenuDisabledOnObjectWithNoActions(); }
        [TestMethod]
        public override void QueryOnlyActionDoesNotReloadAutomatically() { base.QueryOnlyActionDoesNotReloadAutomatically(); }
        [TestMethod]
        public override void PotentActionDoesReloadAutomatically() { base.PotentActionDoesReloadAutomatically(); }
        [TestMethod]
        public override void NonNavigableReferenceProperty() { base.NonNavigableReferenceProperty(); }
        [TestMethod]
        public override void Colours() { base.Colours(); }
        [TestMethod]
        public override void ZeroIntValues() { base.ZeroIntValues(); }
        [TestMethod]
        public override void AddingObjectToCollectionUpdatesTableView() { base.AddingObjectToCollectionUpdatesTableView(); }
        [TestMethod] //Unreliable on server
        public override void TimeSpanProperty() { base.TimeSpanProperty(); }


    }
    #region browsers specific subclasses

    public class ObjectViewTestsIe : ObjectViewTests
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

    //[TestClass] //Firefox Individual
    public class ObjectViewTestsFirefox : ObjectViewTests
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
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element)
        {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor)br).ExecuteScript(script);
        }
    }

    public class ObjectViewTestsChrome : ObjectViewTests
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
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }
    }

    #endregion
    #region Mega tests
    public abstract class MegaObjectViewTestsRoot : ObjectViewTestsRoot
    {
        [TestMethod] //Mega
        public void MegaObjectViewTest()
        {
            base.ActionsAlreadyOpen();
            base.OpenActionsMenuNotAlreadyOpen();
            base.OpenAndCloseSubMenusTo2Levels();
            base.Properties();
            base.Collections();
            base.CollectionEagerlyRendered();
            base.DateAndCurrencyProperties();
            base.TableViewHonouredOnCollection();
            base.ClickReferenceProperty();
            base.OpenCollectionAsList();
            base.NotCountedCollection();
            base.ClickOnLineItemWithCollectionAsList();
            base.ClickOnLineItemWithCollectionAsTable();
            base.DialogAction();
            base.DialogActionOk();
            base.ObjectAction();
            base.CollectionAction();
            base.DescriptionRenderedAsTooltip();
            base.DisabledAction();
            base.ActionsMenuDisabledOnObjectWithNoActions();
            base.QueryOnlyActionDoesNotReloadAutomatically();
            base.PotentActionDoesReloadAutomatically();
            base.NonNavigableReferenceProperty();
            base.Colours();
            base.ZeroIntValues();
            //base.AddingObjectToCollectionUpdatesTableView();
            //base.TimeSpanProperty();
        }
    }
    [TestClass]
    public class MegaObjectViewTestFirefox : MegaObjectViewTestsRoot
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