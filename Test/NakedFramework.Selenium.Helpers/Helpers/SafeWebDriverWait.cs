// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedFramework.Selenium.Helpers.Helpers; 

public class SafeWebDriverWait : IWait<IWebDriver> {
    private readonly WebDriverWait wait;

    public SafeWebDriverWait(IWebDriver driver, TimeSpan timeout) {
        wait = new WebDriverWait(driver, timeout);
        Driver = driver;
    }

    public IWebDriver Driver { get; }

    #region IWait<IWebDriver> Members

    public void IgnoreExceptionTypes(params Type[] exceptionTypes) {
        wait.IgnoreExceptionTypes(exceptionTypes);
    }

    private void DebugDumpPage() {
        var page = Driver.PageSource;
        Debug.WriteLine(page);
    }

    public TResult Until<TResult>(Func<IWebDriver, TResult> condition) {
        return wait.Until(d => {
            try {
                return condition(d);
            }
            catch { }

            return default;
        });
    }

    public TimeSpan Timeout {
        get => wait.Timeout;
        set => wait.Timeout = value;
    }

    public TimeSpan PollingInterval {
        get => wait.PollingInterval;
        set => wait.PollingInterval = value;
    }

    public string Message {
        get => wait.Message;
        set => wait.Message = value;
    }

    #endregion
}