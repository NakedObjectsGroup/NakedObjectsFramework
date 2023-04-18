// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFrameworkClient.TestFramework.Tests;

namespace NakedObjects.Selenium.Test.ObjectTests;

// because the initial timeout needs to be longer if the server is starting up

public abstract class AAStartupTests : AWTest {
    protected override string BaseUrl => TestConfig.BaseObjectUrl;

    [TestMethod]
    public virtual void WaitForStartServer() {
        WaitForView(Pane.Single, PaneType.Home, "Home");
        WaitForCss(".main-column");
    }
}

[TestClass]
public class AAStartupTestsChrome : AAStartupTests {
    [AssemblyInitialize]
    public static void InitialiseAssembly(TestContext context) {
        FilePath(@"drivers.chromedriver.exe");
        InitChromeDriver();
    }

    [AssemblyCleanup]
    public static void CleanUpAssembly() {
        CleanupChromeDriver();
    }

    [TestInitialize]
    public virtual void InitializeTest() {
        Wait.Timeout = new TimeSpan(0, 0, 40);
        Url(BaseUrl);
    }

    [TestCleanup]
    public virtual void CleanupTest() {
        Wait.Timeout = new TimeSpan(0, 0, 10);
    }
}