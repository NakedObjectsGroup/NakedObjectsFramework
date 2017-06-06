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
    public abstract class PerformanceTestsRoot : AWTest {
        public virtual void RetrieveRandomEmployees() {
            Debug.WriteLine(nameof(RetrieveRandomEmployees));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            GeminiUrl("home?m1=EmployeeRepository");
            WaitForView(Pane.Single, PaneType.Home);
            for (int i = 0; i < 100; i++) {
                Click(GetObjectEnabledAction("Random Employee"));
                WaitForView(Pane.Single, PaneType.Object);
                ClickBackButton();
                WaitForView(Pane.Single, PaneType.Home);
            }
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;
            Assert.IsTrue(time < 150000, $"Elapsed time was {time} milliseconds");
        }
    }

    public abstract class PerformanceTests : PerformanceTestsRoot {
        [TestMethod] //Mega
        public void MegaPerformanceTests() {
            RetrieveRandomEmployees();
        }
    }

    #region browsers specific subclasses

    //[TestClass]
    public class PerformanceTestsIe : PerformanceTests {
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

    //[TestClass]
    public class PerformanceTestsFirefox : PerformanceTests {
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

   [TestClass] //toggle
    public class PerformanceTestsChrome : PerformanceTests {
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
}