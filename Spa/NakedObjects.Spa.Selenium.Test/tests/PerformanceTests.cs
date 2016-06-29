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
using System.Diagnostics;

namespace NakedObjects.Selenium
{
    public abstract class PerformanceTestsRoot : AWTest
    {
        public virtual void RetrieveRandomEmployees()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            GeminiUrl("home?m1=EmployeeRepository");
            for (int i = 0; i < 100; i++)
            {
                Click(GetObjectAction("Random Employee"));
                WaitForView(Pane.Single, PaneType.Object);
                ClickBackButton();
                WaitForView(Pane.Single, PaneType.Home);
            }
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;
            Assert.IsTrue(time < 140000, string.Format("Elapsed time was {0} milliseconds",time));
        }

    }

    public abstract class PerformanceTests : PerformanceTestsRoot
    {
        [TestMethod] //Mega
        public override void RetrieveRandomEmployees() { base.RetrieveRandomEmployees(); }

    }
    #region browsers specific subclasses

    //[TestClass]
    public class PerformanceTestsIe : PerformanceTests
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
    public class PerformanceTestsFirefox : PerformanceTests
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

    [TestClass]
    public class PerformanceTestsChrome : PerformanceTests
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
}