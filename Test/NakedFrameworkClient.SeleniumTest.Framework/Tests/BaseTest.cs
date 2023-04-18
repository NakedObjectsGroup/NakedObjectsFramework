// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Reflection;
using NakedFrameworkClient.TestFramework.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace NakedFrameworkClient.TestFramework.Tests;

public abstract class BaseTest {
    protected abstract string BaseUrl { get; }
    protected static IWebDriver Driver { get; private set; }
    protected static SafeWebDriverWait Wait { get; private set; }
    private static int TimeOut => 10;

    protected static void FilePath(string resourcename, int attempt = 0) {
        var assembly = Assembly.GetExecutingAssembly();
        var fileName = resourcename.Remove(0, resourcename.IndexOf(".") + 1);
        var newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        using var stream = assembly.GetManifestResourceStream("NakedFrameworkClient.TestFramework." + resourcename);
        using var fileStream = File.Create(newFile, (int)stream.Length);
        var bytesInStream = new byte[stream.Length];
        stream.Read(bytesInStream, 0, bytesInStream.Length);
        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
    }

    protected static void CleanupChromeDriver() {
        Driver?.Manage().Cookies.DeleteAllCookies();
        Driver?.Quit();
        Driver?.Dispose();
        Driver = null;
    }

    protected void InitFirefoxDriver() {
        Driver = new FirefoxDriver();
        Wait = new SafeWebDriverWait(Driver, TimeSpan.FromSeconds(TimeOut));
        Driver.Manage().Window.Maximize();
    }

    protected void InitIeDriver() {
        Driver = new InternetExplorerDriver();
        Wait = new SafeWebDriverWait(Driver, TimeSpan.FromSeconds(TimeOut));
        Driver.Manage().Window.Maximize();
    }

    protected static void InitChromeDriver() {
        Driver = new ChromeDriver();
        Wait = new SafeWebDriverWait(Driver, TimeSpan.FromSeconds(TimeOut));
        Driver.Manage().Window.Maximize();
    }
}