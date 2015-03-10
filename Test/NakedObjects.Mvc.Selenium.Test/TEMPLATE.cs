// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedObjects.Mvc.Selenium.Test {
    // Use this template to create new test classes, following these steps:
    // 1. Copy this template and replace 'MyTests' with new test class name throughout
    // 2. Write all test logic in the abstract class, in the form of an abstract method 
    //    (no annotation) and a corresponding 'Do' method (see comment in class)
    // 3. Make MyTestsIE implement the abstract MyTests class.  Each created method should be
    //    annotated [TestMethod] and simply delegate to the inherited 'Do' method.
    //    The following Regex will do this automatically:
    //    Find:     ^{.*}public override void {.*}\n{.*\{}\n{.*}{throw .*}$
    //    Replace:  \1\[TestMethod\]\n\1public override void \2\n\3\n\4Do\2;
    // 4. When IE tests run OK, uncomment the Firefox and/or Chrome classes and repeat step 3
    public abstract class MyTests : AWWebTest {
        public new static void InitialiseClass(TestContext context) {
            AWWebTest.InitialiseClass(context);
        }

        //public abstract void Test1();

        //protected void DoTest1() { 
        //  Common test logic here
        //}
    }

    [TestClass]
    public class MyTestsIE : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("IEDriverServer.exe");
            MyTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new InternetExplorerDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        //[TestMethod]
        //public override void Test1()
        //{
        //    DoTest1();
        //}
    }

    [TestClass]
    public class MyTestsFirefox : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            MyTests.InitialiseClass(context);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = new FirefoxDriver();
            br.Navigate().GoToUrl(url);
        }
    }

    [TestClass]
    public class MyTestsChrome : MyTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath("chromedriver.exe");
            MyTests.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            br = InitChromeDriver();
            br.Navigate().GoToUrl(url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }
}