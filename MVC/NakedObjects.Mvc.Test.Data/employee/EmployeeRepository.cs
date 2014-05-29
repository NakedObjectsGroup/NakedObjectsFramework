// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using Expenses.Services;
using NakedObjects;
using NakedObjects.Services;

namespace Expenses.ExpenseEmployees {
    [Named("Employees")]
    public class EmployeeRepository : AbstractFactoryAndRepository, IUserFinder {
        private static int MAX_NUM_EMPLOYEES = 10;

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

        [MemberOrder(Sequence = "2")]
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