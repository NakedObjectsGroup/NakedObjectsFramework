// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework;
using NakedFrameworkClient.TestFramework.Tests;

namespace NakedFunctions.Selenium.Test;

// because the initial timeout needs to be longer if the server is starting up

[TestClass]
public class AAStartupTests : BaseTest {
    [TestMethod]
    public virtual void WaitForStartServer() {
        helper.GotoHome().OpenMainMenu("Products");
    }

    #region Overhead

    protected override string BaseUrl => TestConfig.BaseFunctionalUrl;

    [AssemblyInitialize]
    public static void InitialiseAssembly(TestContext context) {
        FilePath(@"drivers.chromedriver.exe");
        InitChromeDriver();
    }

    [AssemblyCleanup]
    public static void CleanUpAssembly() {
        CleanupChromeDriver();
    }

    private Helper helper;

    [TestInitialize]
    public virtual void InitializeTest() {
        Wait.Timeout = new TimeSpan(0, 0, 40);
        helper = new Helper(BaseUrl, "gemini", Driver, Wait);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }

    #endregion
}