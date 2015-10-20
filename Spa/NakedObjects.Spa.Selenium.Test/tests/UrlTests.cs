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
    public abstract class UrlTests : AWTest
    {
        [TestMethod]
        public virtual void UnrecognisedUrlGoesToHome()
        {
            GeminiUrl( "unrecognised");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        #region Single pane Urls
        [TestMethod]
        public virtual void Home()
        {
            GeminiUrl( "home");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }

        [TestMethod]
        public virtual void HomeWithMenu()
        {
            GeminiUrl( "home?menu1=CustomerRepository");
            WaitForView(Pane.Single, PaneType.Home, "Home");
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
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            AssertObjectElementsPresent();
        }

        private void AssertObjectElementsPresent()
        {
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
        public virtual void ObjectWithNoSuchObject()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Foo-555");
            wait.Until(d => d.FindElement(By.CssSelector(".error")));
        }

        [TestMethod]
        public virtual void ObjectWithActions()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&actions1=open");
            GetObjectAction("Create New Address");
            AssertObjectElementsPresent();
        }

//TODO:  Need to add tests for object & home (later, query) with action (dialog) open

        [TestMethod]
        public virtual void ObjectWithCollections()
        {
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&&collection1_Addresses=List&collection1_Contacts=Table");
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
            GeminiUrl( "object?object1=AdventureWorksModel.Store-350&edit1=true");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".edit")));
            GetSaveButton();
            GetCancelEditButton();
           // AssertObjectElementsPresent();
        }

        [TestMethod]
        public virtual void QueryZeroParameterAction()
        {
            WaitForView(Pane.Single, PaneType.Home, "Home");
            GeminiUrl( "query?menu1=OrderRepository&action1=HighestValueOrders");
            wait.Until(d => d.FindElement(By.CssSelector(".query")));
            WaitForView(Pane.Single, PaneType.Query, "Highest Value Orders");
        }
        #endregion

        #region Split pane Urls

        [TestMethod]
        public virtual void SplitHomeHome()
        {
            GeminiUrl( "home/home");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void SplitHomeObject()
        {
            GeminiUrl( "home/object?object2=AdventureWorksModel.Store-350");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Object,  "Twin Cycles");
        }

        [TestMethod]
        public virtual void SplitHomeQuery()
        {
            GeminiUrl( "home/query?&menu2=OrderRepository&action2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Query, "Highest Value Orders");
        }

        [TestMethod]
        public virtual void SplitObjectHome()
        {
            GeminiUrl( "object/home?object1=AdventureWorksModel.Store-350");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod]
        public virtual void SplitObjectObject()
        {
            GeminiUrl( "object/object?object1=AdventureWorksModel.Store-350&object2=AdventureWorksModel.Store-604");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }

        [TestMethod]
        public virtual void SplitObjectQuery()
        {
            GeminiUrl( "object/query?object1=AdventureWorksModel.Store-350&menu2=OrderRepository&action2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Query,  "Highest Value Orders");
        }

        [TestMethod]
        public virtual void SplitQueryHome()
        {
            GeminiUrl( "query/home?menu1=OrderRepository&action1=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Query, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }

        [TestMethod] 
        public virtual void SplitQueryObject()
        {
            GeminiUrl( "query/object?menu1=OrderRepository&action1=HighestValueOrders&object2=AdventureWorksModel.Store-604");
            WaitForView(Pane.Left, PaneType.Query, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }

        [TestMethod]
        public virtual void SplitQueryQuery()
        {
            GeminiUrl( "query/query?menu1=OrderRepository&action1=HighestValueOrders&menu2=SpecialOfferRepository&action2=CurrentSpecialOffers");

            WaitForView(Pane.Left, PaneType.Query, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Query, "Current Special Offers");
        }
        #endregion

    }
    #region browsers specific subclasses 

   // [TestClass, Ignore]
    public class UrlTestsIe : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
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
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
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
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
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