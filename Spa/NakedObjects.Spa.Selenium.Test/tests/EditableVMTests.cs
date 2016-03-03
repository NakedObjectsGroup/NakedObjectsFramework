// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium
{

    public abstract class EditableVMTestsRoot : AWTest
    {
        public virtual void CreateEditableVM()
        {
            GeminiUrl("object?i1=View&o1=___1.Person-9169&as1=open");
            Click(GetObjectAction("Create Email"));
            //TODO: Title of Form will change
            WaitForView(Pane.Single, PaneType.Object, "Form");
            var properties = br.FindElements(By.CssSelector(".property"));

            //By default a read-only DateTime property is rendered as a formatted time stamp:
            Assert.AreEqual("Status:\r\nNew", properties[4].Text);
 
            ClearFieldThenType("#to1", "Stef");
            ClearFieldThenType("#from1", "Richard");
            ClearFieldThenType("#subject1", "Test");
            ClearFieldThenType("#message1", "Hello");

            var action = wait.Until(d => d.FindElements(By.CssSelector(".action")).
                     Single(we => we.Text == "Send"));
            Click(action);
            wait.Until(dr => dr.FindElement(By.CssSelector(".property:nth-child(5)")).Text == "Status:\r\nSent");
            Assert.AreEqual("To:", WaitForCss(".property:nth-child(1)").Text);
        }
    }

    public abstract class EditableVMObjectTests : EditableVMTestsRoot
    {
        [TestMethod]
        public override void CreateEditableVM() { base.CreateEditableVM(); }
    }
    #region browsers specific subclasses

    //[TestClass, Ignore]
    public class EditableVMTestsIe : EditableVMObjectTests
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

    [TestClass]
    public class TEditableVMTestsFirefox : EditableVMObjectTests
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

    //[TestClass, Ignore]
    public class EditableVMTestsChrome : EditableVMObjectTests
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
    public abstract class MegaEditableVMTestsRoot : EditableVMTestsRoot
    {
        [TestMethod]
        public void MegaEditableVMTest()
        {
            base.CreateEditableVM();
        }
    }
    [TestClass]
    public class MegaEditableVMTestsFirefox : MegaEditableVMTestsRoot
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