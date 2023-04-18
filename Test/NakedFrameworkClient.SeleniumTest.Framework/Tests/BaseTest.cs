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

    #region overhead

    protected abstract string BaseUrl { get; }
    protected string GeminiBaseUrl => BaseUrl + "gemini/";

    protected static IWebDriver br;
    protected static SafeWebDriverWait wait;

    private static int TimeOut => 10;

    protected static void CleanupChromeDriver() {
        br?.Manage().Cookies.DeleteAllCookies();
        br?.Quit();
        br?.Dispose();
        br = null;
    }

    protected void InitFirefoxDriver() {
        br = new FirefoxDriver();
        wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
        br.Manage().Window.Maximize();
    }

    protected void InitIeDriver() {
        br = new InternetExplorerDriver();
        wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
        br.Manage().Window.Maximize();
    }

    protected static void InitChromeDriver() {
        br = new ChromeDriver();
        wait = new SafeWebDriverWait(br, TimeSpan.FromSeconds(TimeOut));
        br.Manage().Window.Maximize();
    }

    #endregion
}