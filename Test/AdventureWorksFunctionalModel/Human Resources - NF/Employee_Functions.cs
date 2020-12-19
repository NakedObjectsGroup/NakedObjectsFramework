// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using System;
using System.Linq;

namespace AdventureWorksModel {
    public static class Employee_Functions {
        #region Life Cycle Methods

        public static Employee Updating(this Employee x, [Injected] DateTime now) => x with { ModifiedDate = now };

        #endregion

        //public static bool HideLoginID(
        //    Employee e,
        //    IQueryable<Employee> employees,
        //    IPrincipal principal)
        //{
        //    var userAsEmployee = EmployeeRepository.CurrentUserAsEmployee(null, employees, principal);
        //    return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        //}

        public static IQueryable<Employee> ColleaguesInSameDept(
            this Employee e,
            IQueryable<EmployeeDepartmentHistory> edhs
        ) {
            var allCurrent = edhs.Where(edh => edh.EndDate == null);
            var thisId = e.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }

        //[MemberOrder(10)]
        //public static (EmployeePayHistory, EmployeePayHistory) ChangePayRate(
        //    Employee e,
        //    [Injected] DateTime now
        //)
        //{
        //    EmployeePayHistory current = CurrentEmployeePayHistory(e);
        //    var eph = new EmployeePayHistory(e, now, current.PayFrequency);
        //    return Result.DisplayAndPersist(eph);
        //}

        public static EmployeePayHistory CurrentEmployeePayHistory(Employee e) {
            return e.PayHistory.OrderByDescending(x => x.RateChangeDate).FirstOrDefault();
        }

        //#region ChangeDepartmentOrShift (Action)
        //[MemberOrder(20)]
        //public static (object[], object[]) ChangeDepartmentOrShift(
        //    Employee e,
        //    Department department, 
        //     Shift shift,
        //    [Injected] DateTime now)
        //{
        //    var edh = CurrentAssignment(e) with {EndDate =  now};
        //    var newAssignment = new EmployeeDepartmentHistory(department, shift, e, now );
        //    return Result.DisplayAndPersist(new object[] { edh, newAssignment });
        //}

        //public static Department Default0ChangeDepartmentOrShift(Employee e)
        //{
        //    EmployeeDepartmentHistory current = CurrentAssignment(e);
        //    return current != null ? current.Department : null;
        //}

        //private static EmployeeDepartmentHistory CurrentAssignment(Employee e)
        //{
        //    return e.DepartmentHistory.Where(n => n.EndDate == null).FirstOrDefault();
        //}

        //#endregion

        public static (Employee, Employee) SpecifyManager(
            Employee e,
            IEmployee manager) {
            var e2 = e with {ManagerID = manager.BusinessEntityID};
            return (e2, e2);
        }

        //[PageSize(20)]
        //public static IQueryable<Employee> AutoCompleteManager(
        //     Employee e,
        //    [Range(2,0)] string name,
        //    IQueryable<Person> persons,
        //    IQueryable<Employee> employees)
        //{
        //    return EmployeeRepository.FindEmployeeByName(null, null, name, persons, employees);
        //}

        //public static  IList<string> ChoicesGender(Employee e)
        //{
        //    return new[] { "M", "F" };
        //}

        //public static IList<string> ChoicesMaritalStatus(Employee e)
        //{
        //    return new[] { "S", "M" };
        //}

        public static (Employee, Employee) CreateNewEmployeeFromContact(this Person contactDetails) => Employee_MenuFunctions.CreateNewEmployeeFromContact(contactDetails);
    }
}