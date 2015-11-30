// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Threading;

namespace NakedObjects.Web.UnitTests.Selenium {
    /// <summary>
    /// Tests content and operations within from Home representation
    /// </summary>
    public abstract class CiceroHelp : AWTest
    {
        [TestMethod]
        public void HelpWithoutArguments()
        {
            CiceroUrl("home");
            WaitForOutput("home");
            EnterCommand("help");
            WaitForOutput("Commands available in current context: " +
                "action; back; clipboard; forward; gemini; help; home; menu; where;");
            //Now try an object context
            CiceroUrl("object?object1=AdventureWorksModel.Product-943");
            WaitForOutput("Product: LL Mountain Frame - Black, 40.");
            //First with no params
            EnterCommand("help");
            WaitForOutput("Commands available in current context: action; back; clipboard; copy; description; edit; forward; gemini; go; help; home; open; property; reload; where;");
        }

        [TestMethod]
        public void HelpWithArgument() { 
        CiceroUrl("/home");
            EnterCommand("help me");
            WaitForOutput("menu command: From the Home context, Menu opens a named main menu. " +
                "This command normally takes one argument: the name, or partial name, " +
                "of the menu. If the partial name matches more than one menu, a list of " +
                "matches will be returned but no menu will be opened; if no argument is " +
                "provided a list of all the menus will be returned.");
            EnterCommand("help clipboard");
            WaitForOutput("clipboard command: Reminder of the object reference currently held "+
                "in the clipboard, if any. Does not take any arguments");
            EnterCommand("help menux");
            WaitForOutput("No such command: menux");
            EnterCommand("help home back");
            WaitForOutput("No such command: home back");
            EnterCommand("help home, back");
            WaitForOutput("Wrong number of arguments provided.");
        }
    }
    #region browsers specific subclasses 

    //    [TestClass, Ignore]
    public class CiceroHelpIe : CiceroHelp {
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
    public class CiceroHelpFirefox : CiceroHelp
    {
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
    public class CiceroHelpChrome : CiceroHelp
    {
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
}