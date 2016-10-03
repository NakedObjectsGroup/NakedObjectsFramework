// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class LocalCollectionActionsTestsRoot : AWTest {
        public void ActionsAndCheckBoxesVisible()
        {
            GeminiUrl("object?i1=View&r=1&o1=___1.SalesOrderHeader--44284&c1_Details=List");
            //Test that first collection has two actions within it
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector(".action")).Count == 2);
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector(".action"))[0].Text == "Adjust Quantities");
            wait.Until(dr => dr.FindElements(By.CssSelector(".collection"))[0].FindElements(By.CssSelector(".action"))[1].Text == "Remove Details");
            //Confirm that checkboxes, including All are rendered

        }
    }

    public abstract class LocalCollectionActionsTests : LocalCollectionActionsTestsRoot {
     
    }

    #region browsers specific subclasses

    public class LocalCollectionActionsTestsIe : LocalCollectionActionsTests {
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

    //[TestClass] //Firefox Individual
    public class LocalCollectionActionsFirefox : LocalCollectionActionsTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    public class LocalCollectionActionsTestsChrome : LocalCollectionActionsTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    #endregion

    #region Mega tests

    public abstract class MegaLocalCollectionActionsTestsRoot : LocalCollectionActionsTestsRoot {
        [TestMethod] //Mega
        public void MegaLocalCollectionActionsTest() {
        }
    }

    //[TestClass]
    public class MegaLocalCollectionActionsTestsFirefox : MegaLocalCollectionActionsTestsRoot {
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

    public class MegaLocalCollectionActionsTestsIe : MegaLocalCollectionActionsTestsRoot {
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
    public class MegaLocalCollectionActionsTestsChrome : MegaLocalCollectionActionsTestsRoot
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
    }

    #endregion
}