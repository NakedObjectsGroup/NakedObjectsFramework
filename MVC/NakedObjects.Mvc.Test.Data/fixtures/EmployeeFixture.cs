// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Expenses.Currencies;
using Expenses.ExpenseClaims;
using Expenses.ExpenseEmployees;
using NakedObjects;

namespace Expenses.Fixtures {
    public class EmployeeFixture {
        public static Employee BOB;
        public static Employee DICK;
        public static Employee JOE;
        public static Employee SVEN;

        public void Install() {
            createEmployees();
        }

        private void createEmployees() {
            SVEN = CreateEmployee("Sven Bloggs", "sven", "sven@example.com", CurrencyFixture.GBP);
            DICK = CreateEmployee("Dick Barton", "dick", "dick@example.com", CurrencyFixture.GBP);
            BOB = CreateEmployee("Robert Bruce", "bob", "bob@example.com", CurrencyFixture.USD);
            JOE = CreateEmployee("Joe Sixpack", "joe", "joe@example.com", CurrencyFixture.USD);
            CreateEmployee("Intrepid Explorer", "exploration", "exploration@example.com", CurrencyFixture.USD);

            SVEN.NormalApprover = DICK;
            DICK.NormalApprover = BOB;
        }

        [Hidden]
        public virtual Employee CreateEmployee(string myName, string userName, string emailAddress, Currency currency) {
            var emp = Container.NewTransientInstance<Employee>();
            emp.Name = myName;
            emp.UserName = userName;
            emp.EmailAddress = emailAddress;
            emp.Currency = currency;
            Container.Persist(ref emp);
            return emp;
        }

        #region Injected Services

        public IDomainObjectContainer Container { protected get; set; }

        #region Injected: ClaimantRepository

        private EmployeeRepository m_employeeRepository;

        public EmployeeRepository EmployeeRepository {
            set { m_employeeRepository = value; }
        }

        #endregion

        #region Injected: ClaimRepository

        private ClaimRepository m_claimRepository;

        public ClaimRepository ClaimRepository {
            set { m_claimRepository = value; }
        }

        #endregion

        //
        //		* This region contains references to the services (Repositories, Factories or other Services) used by
        //		* this domain object. The references are injected by the application container.
        //		

        #endregion
    }
}