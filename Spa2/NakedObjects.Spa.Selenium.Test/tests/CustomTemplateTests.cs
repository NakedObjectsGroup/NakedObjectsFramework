// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class CustomTemplateTestsRoot : AWTest {
        public virtual void CustomViewTemplate() {
            GeminiUrl("object?i1=View&o1=___1.Location--60");
            WaitForView(Pane.Single, PaneType.Object, "Location - custom view");
            Assert.AreEqual("Topaz", WaitForCss(".presentationHint").Text);
        }

        public virtual void CustomEditTemplate() {
            GeminiUrl("object?i1=Edit&o1=___1.WorkOrderRouting--43375--980--7");
            WaitForView(Pane.Single, PaneType.Object, "Work Order Routing - custom edit");
        }

        public virtual void CustomListTemplate() {
            GeminiUrl("list?m1=WorkOrderRepository&a1=AllLocations&pg1=1&ps1=20&s1_=0&c1=List");
            Reload();
            WaitForView(Pane.Single, PaneType.List, "Location - custom list");
        }

        public virtual void CustomErrorHandling() {
            Url(CustomersMenuUrl);
            WaitForCss(".actions nof-action", CustomerServiceActions);
            Click(GetObjectEnabledAction("Throw Domain Exception"));
            WaitForView(Pane.Single, PaneType.Error);
            Assert.AreEqual("Internal Server Error", WaitForCss(".title").Text);
            Assert.AreEqual("Foo", WaitForCss(".message").Text);
        }
    }

    public abstract class CustomTemplateTests : CustomTemplateTestsRoot {
        [TestMethod]
        public override void CustomViewTemplate() {
            base.CustomViewTemplate();
        }

        [TestMethod]
        public override void CustomEditTemplate() {
            base.CustomEditTemplate();
        }

        [TestMethod]
        public override void CustomListTemplate() {
            base.CustomListTemplate();
        }

        [TestMethod]
        public override void CustomErrorHandling() {
            base.CustomErrorHandling();
        }
    }

    #region browsers specific subclasses

    public class CustomTemplateTestsIe : CustomTemplateTests {
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
    public class CustomTemplateFirefox : CustomTemplateTests {
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

    public class CustomTemplateTestsChrome : CustomTemplateTests {
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

    public abstract class MegaCustomTemplateTestsRoot : CustomTemplateTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void MegaCustomTemplateTests() {
            CustomViewTemplate();
            CustomEditTemplate();
            CustomListTemplate();
            CustomErrorHandling();
        }
        [TestMethod]
        [Priority(-1)]
        public void ProblematicTests() {

        }
    }

    //[TestClass]
    public class MegaCustomTemplateTestsFirefox : MegaCustomTemplateTestsRoot {
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

    //[TestClass]
    public class MegaCustomTemplateTestsIe : MegaCustomTemplateTestsRoot {
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

    #endregion
}