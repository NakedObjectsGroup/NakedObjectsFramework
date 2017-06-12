// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Selenium {
    public abstract class ViewModelTestsRoot : AWTest {
        public virtual void CreateVM() {
            Debug.WriteLine(nameof(CreateVM));

            GeminiUrl("object?i1=View&o1=___1.CustomerDashboard--20071&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Sean Campbell - Dashboard");
            //TODO: test for no Edit button?
        }

        public virtual void CreateEditableVM() {
            Debug.WriteLine(nameof(CreateEditableVM));

            GeminiUrl("object?i1=View&o1=___1.Person--9169&as1=open");
            Click(GetObjectEnabledAction("Create Email"));
            WaitForView(Pane.Single, PaneType.Object, "New email");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[4].Text == "Status:\r\nNew");

            ClearFieldThenType("#to1", "Stef");
            ClearFieldThenType("#from1", "Richard");
            ClearFieldThenType("#subject1", "Test");
            ClearFieldThenType("#message1", "Hello");

            Click(GetInputButton("Send"));
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(5)")).Text == "Status:\r\nSent");
            Assert.AreEqual("To:", WaitForCss(".property:nth-child(1)").Text);
            var title = WaitForCss(".title");
            Assert.AreEqual("Sent email", title.Text);
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
        }

        //Test for #46
        public virtual void EditableVMWithEmptyLeadingKeys() {
            Debug.WriteLine(nameof(EditableVMWithEmptyLeadingKeys));

            GeminiUrl("object?i1=View&o1=___1.Person--9169&as1=open");
            Click(GetObjectEnabledAction("Create Email"));
            WaitForView(Pane.Single, PaneType.Object, "New email");
            wait.Until(dr => dr.FindElements(By.CssSelector(".property"))[4].Text == "Status:\r\nNew");

            //leave 3/4 of the optional fields empty
            ClearFieldThenType("#subject1", "Test2");

            Click(GetInputButton("Send"));
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(5)")).Text == "Status:\r\nSent");
            Assert.AreEqual("To:", WaitForCss(".property:nth-child(1)").Text);
            var title = WaitForCss(".title");
            Assert.AreEqual("Sent email", title.Text);
        }

        public virtual void CreateSwitchableVM() {
            Debug.WriteLine(nameof(CreateSwitchableVM));

            GeminiUrl("object?i1=View&o1=___1.StoreSalesInfo--AW00000293--False&as1=open");
            WaitForView(Pane.Single, PaneType.Object, "Sales Info for: Fashionable Bikes and Accessories");
            Click(GetObjectEnabledAction("Edit")); //Note: not same as the generic (object) Edit button
            WaitForView(Pane.Single, PaneType.Object, "Editing - Sales Info for: Fashionable Bikes and Accessories");
            SelectDropDownOnField("#salesterritory1", "Central");
            Click(SaveVMButton()); //TODO: check if this works
            WaitForView(Pane.Single, PaneType.Object, "Sales Info for: Fashionable Bikes and Accessories");
            WaitForTextEquals(".property", 2, "Sales Territory:\r\nCentral");
        }
    }

    public abstract class ViewModelsTests : ViewModelTestsRoot {
        [TestMethod]
        public override void CreateVM() {
            base.CreateVM();
        }

        [TestMethod]
        public override void CreateEditableVM() {
            base.CreateEditableVM();
        }

        [TestMethod]
        public override void EditableVMWithEmptyLeadingKeys() {
            base.EditableVMWithEmptyLeadingKeys();
        }

        [TestMethod]
        public override void CreateSwitchableVM() {
            base.CreateSwitchableVM();
        }
    }


    #region Mega tests

    public abstract class MegaViewModelTestsRoot : ViewModelTestsRoot {
        [TestMethod] //Mega
        [Priority(0)]
        public void MegaViewModelTest() {
            CreateVM();
            CreateEditableVM();
            EditableVMWithEmptyLeadingKeys();
            CreateSwitchableVM();
        }
        [TestMethod]
        [Priority(-1)]
        public void ProblematicTests() {

        }
    }

    //[TestClass]
    public class MegaViewModelTestsFirefox : MegaViewModelTestsRoot {
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
    public class MegaViewModelTestsIe : MegaViewModelTestsRoot {
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
    public class MegaViewModelTestsChrome : MegaViewModelTestsRoot {
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