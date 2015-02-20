// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Services;

namespace Expenses.ExpenseEmployees {
    [Named("Employees")]
    public class EmployeeRepository : AbstractFactoryAndRepository, IUserFinder {
        private static readonly int MAX_NUM_EMPLOYEES = 10;

        #region IUserFinder Members

        [Named("Me"), Executed(Where.Remotely)]
        public virtual object CurrentUserAsObject() {
            IQueryable<Employee> query =
                from employee in Instances<Employee>()
                where employee.UserName == Principal.Identity.Name
                select employee;

            if (query.Count() == 0) {
                WarnUser("No Employee representing current user");
            }
            return query.FirstOrDefault();
        }

        #endregion

        [MemberOrder(Sequence = "2"), FinderAction()]
        public virtual IList<Employee> FindEmployeeByName(string name) {
            IQueryable<Employee> query =
                from employee in Instances<Employee>()
                where employee.Name == name
                select employee;

            if (query.Count() == 0) {
                WarnUser("No employees found matching name: " + name);
                return null;
            }
            if (query.Count() > MAX_NUM_EMPLOYEES) {
                WarnUser("Too many employees found matching name: " + name + "" + '\n' + " Please refine search.");
                return null;
            }
            return query.ToList();
        }
    }
}