// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class AWWebTest {
        #region chrome helper

        protected static string FilePath(string resourcename) {
            var fileName = resourcename; //.Remove(0, resourcename.IndexOf(".") + 1);

            var newFile = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (File.Exists(newFile)) {
                File.Delete(newFile);
            }

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream("NakedObjects.Mvc.Selenium.Test." + resourcename)) {
                using (var fileStream = File.Create(newFile, (int) stream.Length)) {
                    var bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
            }

            return newFile;
        }

        #endregion

        #region overhead

        protected const string url = "http://mvc.nakedobjects.net:1081/UnitTestAjax";
        protected const string server = @"Saturn\SqlExpress";
        protected const string database = "AdventureWorks";
        protected const string backup = "AdventureWorks";

        //protected const string url = "http://localhost:56696/";
        //protected const string server = @".\SQLEXPRESS";
        //protected const string server = @"(localdb)\ProjectsV12";

        //protected const string database = "AdventureWorks";
        //protected const string backup = "AdventureWorks";

        protected IWebDriver br;
        protected SafeWebDriverWait wait;
        protected TimeSpan DefaultTimeOut = new TimeSpan(0, 0, 10);

        public static void Reset(TestContext context) {
            try {
                DatabaseUtils.RestoreDatabase(database, backup, server);
            }
            catch (Exception e) {
                // just carry on - tests may fail
                var m = e.Message;
                Console.WriteLine(m);
            }
            KillAllProcesses("iexplore");
            KillAllProcesses("firefox");
        }

        private static void KillAllProcesses(string name) {
            try {
                var processes = Process.GetProcessesByName(name);
                foreach (var p in processes) {
                    try {
                        p.Kill();
                    }
                    catch {
                        //ignore
                    }
                }
            }
            catch {
                // ignore
            }
        }

        public virtual void CleanUp() {
            if (br != null) {
                try {
                    br.Manage().Cookies.DeleteAllCookies();
                    br.Quit();
                    br.Dispose();
                    br = null;
                }
                catch {
                    // to suppress error 
                }
            }
        }

        protected IWebDriver InitChromeDriver() {
            var crOptions = new ChromeOptions();
            crOptions.AddArgument(@"--test-type");
            return new ChromeDriver(crOptions);
        }

        #endregion

        #region Helpers

        protected void Login() {
            //Login("scascarini", "password");
            Thread.Sleep(2000);
        }

        protected void Login(string username, string password) {
            br.FindElement(By.CssSelector("#UserName")).SendKeys(username + Keys.Tab);
            br.FindElement(By.CssSelector("#Password")).SendKeys(password + Keys.Tab);
            br.FindElement(By.CssSelector("input[type='submit']")).Click();
            Thread.Sleep(5000);
        }

        protected void Find(string actionSelector, string fieldSelector, string value) {
            var field = wait.ClickAndWait(actionSelector, fieldSelector);
            field.Clear();
            field.SendKeys(value + Keys.Tab);
            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        protected void FindEmployeeByLastName(string lastName) {
            Find("#EmployeeRepository-FindEmployeeByName button", "#EmployeeRepository-FindEmployeeByName-LastName-Input", lastName);
        }

        protected void FindCustomerByAccountNumber(string accountNumber) {
            Find("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input", accountNumber);
        }

        protected void FindOrder(string orderNumber) {
            Find("#OrderRepository-FindOrder button", "#OrderRepository-FindOrder-OrderNumber-Input", orderNumber);
        }

        protected void FindProduct(string productNumber) {
            Find("#ProductRepository-FindProductByNumber button", "#ProductRepository-FindProductByNumber-Number-Input", productNumber);
        }

        protected void FindSalesPerson(string lastName) {
            Find("#SalesRepository-FindSalesPersonByName button", "#SalesRepository-FindSalesPersonByName-LastName-Input", lastName);
        }

        #endregion
    }
}