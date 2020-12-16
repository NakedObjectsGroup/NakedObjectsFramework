// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Spa.Selenium.Test.ObjectTests {
    /// <summary>
    /// Tests only that a given URLs return the correct views. No actions performed on them
    /// </summary>
    /// 
    public abstract class UrlTestsRoot : AWTest {

        protected override string BaseUrl => TestConfig.BaseObjectUrl;

        public virtual void UnrecognisedUrlGoesToHome() {
            Debug.WriteLine(nameof(UnrecognisedUrlGoesToHome));
            GeminiUrl("unrecognised");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        #region Single pane Urls

        public virtual void Home() {
            Debug.WriteLine(nameof(Home));
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        public virtual void HomeWithMenu() {
            Debug.WriteLine(nameof(HomeWithMenu));
            GeminiUrl("home?m1=CustomerRepository");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElement(By.CssSelector("nof-action-list")));
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector("nof-action-list nof-action"));
            Assert.AreEqual(CustomerServiceActions, actions.Count);
            //Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            //Assert.AreEqual("Find Store By Name", actions[1].Text);
            //Assert.AreEqual("Create New Store Customer", actions[2].Text);
            //Assert.AreEqual("Random Store", actions[3].Text);
            //Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            //Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            //Assert.AreEqual("Random Individual", actions[6].Text);
            //Assert.AreEqual("Customer Dashboard", actions[7].Text);
            //Assert.AreEqual("Throw Domain Exception", actions[8].Text);
        }

        public virtual void Object() {
            Debug.WriteLine(nameof(Object));
            GeminiUrl("object?o1=___1.Store--350");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            AssertObjectElementsPresent();
        }

        private void AssertObjectElementsPresent() {
            wait.Until(d => d.FindElement(By.CssSelector(".single")));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            wait.Until(d => d.FindElement(By.CssSelector(".header")));
            wait.Until(d => d.FindElement(By.CssSelector("input[value='Actions']")));
            wait.Until(d => d.FindElement(By.CssSelector(".main-column")));
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));

            Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);
        }

        public virtual void ObjectWithNoSuchObject() {
            Debug.WriteLine(nameof(ObjectWithNoSuchObject));
            GeminiUrl("object?o1=___1.Foo--555");
            wait.Until(d => d.FindElement(By.CssSelector(".error")));
        }

        public virtual void ObjectWithActions() {
            Debug.WriteLine(nameof(ObjectWithActions));
            GeminiUrl("object?o1=___1.Store--350&as1=open");
            GetObjectEnabledAction("Create New Address");
            AssertObjectElementsPresent();
        }

        //TODO:  Need to add tests for object & home (later, list) with action (dialog) open
        public virtual void ObjectWithCollections() {
            Debug.WriteLine(nameof(ObjectWithCollections));
            GeminiUrl("object?o1=___1.Store--350&&c1_Addresses=List&c1_Contacts=Table");
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));
            AssertObjectElementsPresent();
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).Count == 2);
            var collections = br.FindElements(By.CssSelector(".collection"));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.TagName("table")));
            //Assert.IsNotNull(collections[0].FindElement(By.TagName("table")));

            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon.table")));
            //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon.table")));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon.summary")));

            //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon.summary")));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElements(By.CssSelector(".icon.list")).Count == 0);

            //Assert.IsTrue(collections[0].FindElements(By.CssSelector(".icon.list")).Count == 0);
        }

        public virtual void ObjectInEditMode() {
            Debug.WriteLine(nameof(ObjectInEditMode));
            GeminiUrl("object?o1=___1.Store--350&i1=Edit");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".edit")));
            SaveButton();
            GetCancelEditButton();
            // AssertObjectElementsPresent();
        }

        public virtual void ListZeroParameterAction() {
            Debug.WriteLine(nameof(ListZeroParameterAction));
            GeminiUrl("list?m1=OrderRepository&a1=HighestValueOrders");
            Reload();
            wait.Until(d => d.FindElement(By.CssSelector(".list")));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
        }

        #endregion

        #region Split pane Urls

        public virtual void SplitHomeHome() {
            Debug.WriteLine(nameof(ListZeroParameterAction));
            GeminiUrl("home/home");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        public virtual void SplitHomeObject() {
            GeminiUrl("home/object?o2=___1.Store--350");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Object, "Twin Cycles");
        }

        public virtual void SplitHomeList() {
            GeminiUrl("home/list?&m2=OrderRepository&a2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
        }

        public virtual void SplitObjectHome() {
            GeminiUrl("object/home?o1=___1.Store--350");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        public virtual void SplitObjectObject() {
            GeminiUrl("object/object?o1=___1.Store--350&o2=___1.Store--604");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }

        public virtual void SplitObjectList() {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            GeminiUrl("object/list?o1=___1.Store--350&m2=OrderRepository&a2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
        }

        public virtual void SplitListHome() {
            GeminiUrl("list/home?m1=OrderRepository&a1=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        public virtual void SplitListObject() {
            GeminiUrl("list/object?m1=OrderRepository&a1=HighestValueOrders&o2=___1.Store--604");
            WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }

        public virtual void SplitListList() {
            GeminiUrl("list/list?m2=PersonRepository&pm2_firstName=%22a%22&pm2_lastName=%22a%22&a2=FindContactByName&p2=1&ps2=20&s2_=0&m1=SpecialOfferRepository&a1=CurrentSpecialOffers&p1=1&ps1=20&s1_=0");
            WaitForView(Pane.Left, PaneType.List, "Current Special Offers");
            WaitForView(Pane.Right, PaneType.List, "Find Contact By Name");
        }

        #endregion
    }

    public abstract class UrlTests : UrlTestsRoot {
        [TestMethod]
        public override void UnrecognisedUrlGoesToHome() {
            base.UnrecognisedUrlGoesToHome();
        }

        #region Single pane Urls

        [TestMethod]
        public override void Home() {
            base.Home();
        }

        [TestMethod]
        public override void HomeWithMenu() {
            base.HomeWithMenu();
        }

        [TestMethod]
        public override void Object() {
            base.Object();
        }

        private void AssertObjectElementsPresent() {
            wait.Until(d => d.FindElement(By.CssSelector(".single")));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            wait.Until(d => d.FindElement(By.CssSelector(".header")));
            wait.Until(d => d.FindElement(By.CssSelector(".menu")).Text == "Actions");
            wait.Until(d => d.FindElement(By.CssSelector(".main-column")));
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));

            Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);
        }

        [TestMethod]
        public override void ObjectWithNoSuchObject() {
            base.ObjectWithNoSuchObject();
        }

        [TestMethod]
        public override void ObjectWithActions() {
            base.ObjectWithActions();
        }

        [TestMethod]
        public override void ObjectWithCollections() {
            base.ObjectWithCollections();
        }

        [TestMethod]
        public override void ObjectInEditMode() {
            base.ObjectInEditMode();
        }

        [TestMethod]
        public override void ListZeroParameterAction() {
            base.ListZeroParameterAction();
        }

        #endregion

        #region Split pane Urls

        [TestMethod]
        public override void SplitHomeHome() {
            base.SplitHomeHome();
        }

        [TestMethod]
        public override void SplitHomeObject() {
            base.SplitHomeObject();
        }

        [TestMethod]
        public override void SplitHomeList() {
            base.SplitHomeList();
        }

        [TestMethod]
        public override void SplitObjectHome() {
            base.SplitObjectHome();
        }

        [TestMethod]
        public override void SplitObjectObject() {
            base.SplitObjectObject();
        }

        [TestMethod]
        public override void SplitObjectList() {
            base.SplitObjectList();
        }

        [TestMethod]
        public override void SplitListHome() {
            base.SplitListHome();
        }

        [TestMethod]
        public override void SplitListObject() {
            base.SplitListObject();
        }

        [TestMethod]
        public override void SplitListList() {
            base.SplitListList();
        }

        #endregion
    }

    public class MegaUrlTestRoot : UrlTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public virtual void UrlTests() {
            Home();
            UnrecognisedUrlGoesToHome();
            HomeWithMenu();
            Object();
            ObjectInEditMode();
            ObjectWithActions();
            ObjectWithCollections();
            ObjectWithNoSuchObject();
            SplitHomeHome();
            SplitHomeObject();
            SplitHomeList();
            SplitObjectHome();
            SplitObjectObject();
            SplitObjectList();
            SplitListHome();
            SplitListObject();
            SplitListList();
        }

        //[TestMethod]
        [Priority(-1)]
        public void ProblematicUrlTests() { }
    }

    //[TestClass]
    public class MegaUrlTestFirefox : MegaUrlTestRoot {
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
    public class MegaUrlTestIe : MegaUrlTestRoot {
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
    public class MegaUrlTestChrome : MegaUrlTestRoot {
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
}