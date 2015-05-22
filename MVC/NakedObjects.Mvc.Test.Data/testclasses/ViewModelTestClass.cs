// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Expenses.Currencies;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    public class ViewModelTestClass : IViewModel {
        public IDomainObjectContainer Container { protected get; set; }

        [Hidden(WhenTo.Always)]
        public Employee Employee { get; set; }

        public string UserName {
            get { return Employee.UserName; }
            set { Employee.UserName = value; }
        }

        #region IViewModel Members

        public string[] DeriveKeys() {
            return new[] {Employee.Name};
        }

        public void PopulateUsingKeys(string[] instanceId) {
            string name = instanceId.First();
            Employee = Container.Instances<Employee>().Single(e => e.Name == name);
        }

        #endregion

        public void ChangeCurrency(Currency newCurrency) {
            Employee.Currency = newCurrency;
        }
    }
}