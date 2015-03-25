// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class ExceptionTests : AWWebTest {
        public abstract void DomainException();

        public void DoDomainException() {
            Login();

            var err = wait.ClickAndWait("#CustomerRepository-ThrowDomainException button", ".error h2");

            var text = err.Text;
            Assert.AreEqual("An exception was thrown within the application code", text);
        }
    }
}