// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests only that a given URLs return the correct views. No actions performed on them
    /// </summary>
    public abstract class UrlTests : GeminiTest
    {
        [TestMethod]
        public virtual void UnrecognisedUrlGoesToHome()
        {
            br.Navigate().GoToUrl(Url + "#/unrecognised");
            WaitForSingleHome();
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        #region Single pane Urls
        [TestMethod]
        public virtual void Home()
        {
            br.Navigate().GoToUrl(Url + "#/home");
            WaitForSingleHome();
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        [TestMethod]
        public virtual void HomeWithMenu()
        {
            br.Navigate().GoToUrl(Url + "#/home?menu1=CustomerRepository");
            WaitForSingleHome();
            wait.Until(d => d.FindElement(By.CssSelector(".actions")));
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector(".action"));

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Assert.AreEqual("Create New Store Customer", actions[2].Text);
            Assert.AreEqual("Random Store", actions[3].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            Assert.AreEqual("Random Individual", actions[6].Text);
            Assert.AreEqual("Customer Dashboard", actions[7].Text);
            Assert.AreEqual("Throw Domain Exception", actions[8].Text);
        }

       [TestMethod]
        public virtual void Object()
        {
            br.Navigate().GoToUrl(Url + "#/object?object1=AdventureWorksModel.Store-555");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            AssertObjectElementsPresent();
        }

        private void AssertObjectElementsPresent()
        {
            Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);
            Assert.IsNotNull(br.FindElement(By.CssSelector(".single")));
            Assert.IsNotNull(br.FindElement(By.CssSelector(".object")));
            Assert.IsNotNull(br.FindElement(By.CssSelector(".view")));
            Assert.IsNotNull(br.FindElement(By.CssSelector(".header")));
            var menu = br.FindElement(By.CssSelector(".menu"));
            Assert.AreEqual("Actions", menu.Text);
            Assert.IsNotNull(br.FindElement(By.CssSelector(".main-column")));
            Assert.IsNotNull(br.FindElement(By.CssSelector(".collections")));
        }

        [TestMethod]
        public virtual void ObjectWithNoSuchObject()
        {
            br.Navigate().GoToUrl(Url + "#/object?object1=AdventureWorksModel.Foo-555");
            wait.Until(d => d.FindElement(By.CssSelector(".error")));
        }

        [TestMethod]
        public virtual void ObjectWithActions()
        {
            br.Navigate().GoToUrl(Url + "#/object?object1=AdventureWorksModel.Store-555&actions=open");
            GetObjectActions(StoreActions);
            GetObjectAction("Create New Address");
            AssertObjectElementsPresent();
        }

//TODO:  Need to add tests for object & home (later, query) with action (dialog) open

        [TestMethod]
        public virtual void ObjectWithCollections()
        {
            br.Navigate().GoToUrl(Url + "#/object?object1=AdventureWorksModel.Store-555&&collection1_Addresses=List&collection1_Contacts=Table");
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));
            AssertObjectElementsPresent();
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).Count == 2);
            var collections = br.FindElements(By.CssSelector(".collection"));
            Assert.IsNotNull(collections[0].FindElement(By.TagName("table")));
            Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon-table")));
            Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon-summary")));
            Assert.IsTrue(collections[0].FindElements(By.CssSelector(".icon-list")).Count == 0);
        }

        [TestMethod]
        public virtual void ObjectInEditMode()
        {
            br.Navigate().GoToUrl(Url + "#/object?object1=AdventureWorksModel.Store-555&edit1=true");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".edit")));
            GetSaveButton();
            GetCancelEditButton();
           // AssertObjectElementsPresent();
        }

        [TestMethod]
        public virtual void QueryZeroParameterAction()
        {
            WaitForSingleHome();
            br.Navigate().GoToUrl(Url + "#/query?menu1=OrderRepository&action1=HighestValueOrders");
            wait.Until(d => d.FindElement(By.CssSelector(".query")));
            WaitForSingleQuery();
        }
        #endregion

        #region Split pane Urls

        [TestMethod]
        public virtual void SplitHomeHome()
        {
            br.Navigate().GoToUrl(Url + "#/home/home");
            WaitForSplit(PaneType.Home, PaneType.Home, "Home", "Home");
        }

        [TestMethod]
        public virtual void SplitHomeObject()
        {
            br.Navigate().GoToUrl(Url + "#/home/object?object2=AdventureWorksModel.Store-555");
            WaitForSplit(PaneType.Home, PaneType.Object, "Home", "Twin Cycles, AW00000555");
        }

        [TestMethod, Ignore] //Stef -  not implemented yet
        public virtual void SplitHomeQuery()
        {
            br.Navigate().GoToUrl(Url + "#/home/query?object1=AdventureWorksModel.Store-555&menu1=OrderRepository&action1=HighestValueOrders");
            WaitForSplit(PaneType.Home, PaneType.Query, "Home", "Highest Value Orders");
        }

        [TestMethod]
        public virtual void SplitObjectHome()
        {
            br.Navigate().GoToUrl(Url + "#/object/home?object1=AdventureWorksModel.Store-555");
            WaitForSplit(PaneType.Object, PaneType.Home, "Twin Cycles, AW00000555", "Home");
        }

        [TestMethod]
        public virtual void SplitObjectObject()
        {
            br.Navigate().GoToUrl(Url + "#/object/object?object1=AdventureWorksModel.Store-555&object2=AdventureWorksModel.Store-359");
            WaitForSplit(PaneType.Object, PaneType.Object, "Twin Cycles, AW00000555", "Mechanical Sports Center, AW00000359");
        }

        [TestMethod, Ignore] //Stef -  not implemented yet
        public virtual void SplitObjectQuery()
        {
            br.Navigate().GoToUrl(Url + "#/object/query?object1=AdventureWorksModel.Store-555&menu1=OrderRepository&action1=HighestValueOrders");
            WaitForSplit(PaneType.Object, PaneType.Query, "Twin Cycles, AW00000555", "Highest Value Orders");
        }

        [TestMethod]
        public virtual void SplitQueryHome()
        {
            br.Navigate().GoToUrl(Url + "#/query/home?menu1=OrderRepository&action1=HighestValueOrders");
            WaitForSplit(PaneType.Query, PaneType.Home, "Highest Value Orders", "Home");
        }

        [TestMethod, Ignore] //Stef -  not implemented yet
        public virtual void SplitQueryObject()
        {
            br.Navigate().GoToUrl(Url + "#/query/object?menu1=OrderRepository&action1=HighestValueOrders&object2=AdventureWorksModel.Store-359");
            WaitForSplit(PaneType.Query, PaneType.Object, "Highest Value Orders", "Mechanical Sports Center, AW00000359");
        }

        [TestMethod, Ignore] //Stef -  not implemented yet
        public virtual void SplitQueryQuery()
        {
            br.Navigate().GoToUrl(Url + "#/query/query?menu1=OrderRepository&action1=HighestValueOrders&menu2=SpecialOfferRepository&action2=CurrentSpecialOffers");
            WaitForSplit(PaneType.Query, PaneType.Query, "Highest Value Orders", "Current Special Offers");
        }
        #endregion

    }
    #region browsers specific subclasses 

   // [TestClass, Ignore]
    public class UrlTestsIe : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class UrlTestsFirefox : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

   // [TestClass, Ignore]
    public class UrlTestsChrome : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion

    #region Running all tests in one go
    //[TestClass]
    public class MegaUrlTestFirefox : UrlTests
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            GeminiTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        [TestMethod]
        public virtual void MegaTest()
        {
            Home();
            UnrecognisedUrlGoesToHome();
            Object();
            ObjectInEditMode();
            ObjectWithActions();
            ObjectWithCollections();
            ObjectWithNoSuchObject();
            //QueryZeroParameterAction();
            SplitHomeHome();
            SplitObjectHome();
            //SplitQueryHome();
        }
    }
    #endregion
}