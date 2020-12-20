// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using System.Linq;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel
{
    public static class Employee_Functions
    {
        #region Life Cycle Methods

        public static Employee Updating(this Employee x, IContainer container) => x with { ModifiedDate = container.Now() };

        #endregion

        public static bool HideLoginID(Employee e, IContainer container)
        {
            var userAsEmployee = Employee_MenuFunctions.CurrentUserAsEmployee(container);
            return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        }

        public static IQueryable<Employee> ColleaguesInSameDept(
            this Employee e, IContainer container)
        {
            var allCurrent = container.Instances<EmployeeDepartmentHistory>().Where(edh => edh.EndDate == null);
            var thisId = e.BusinessEntityID;
            var thisDeptId = allCurrent.Single(edh => edh.EmployeeID == thisId).DepartmentID;
            return allCurrent.Where(edh => edh.DepartmentID == thisDeptId).Select(edh => edh.Employee);
        }

        [MemberOrder(10)]
        public static (EmployeePayHistory, IContainer) ChangePayRate(Employee e, IContainer container)
        {
            EmployeePayHistory current = CurrentEmployeePayHistory(e);
            var eph = new EmployeePayHistory() { Employee = e, RateChangeDate = container.Now(), PayFrequency = current.PayFrequency };
            return DisplayAndSave(eph, container);
        }

        public static EmployeePayHistory CurrentEmployeePayHistory(Employee e) => e.PayHistory.OrderByDescending(x => x.RateChangeDate).FirstOrDefault();

        #region ChangeDepartmentOrShift
        [MemberOrder(20)]
        public static (Employee, IContainer) ChangeDepartmentOrShift(
           this Employee e, Department department, Shift shift, IContainer container)
        {
            var edh = CurrentAssignment(e) with { EndDate = container.Now() };
            var newAssignment = new EmployeeDepartmentHistory() { Department = department, Shift = shift, Employee = e, StartDate = container.Today() };
            return (e, container.WithPendingSave(edh, newAssignment));
        }

        public static Department Default0ChangeDepartmentOrShift(this Employee e)
        {
            EmployeeDepartmentHistory current = CurrentAssignment(e);
            return current != null ? current.Department : null;
        }

        private static EmployeeDepartmentHistory CurrentAssignment(Employee e)
        {
            return e.DepartmentHistory.Where(n => n.EndDate == null).FirstOrDefault();
        }

        #endregion
        [Edit]
        public static (Employee, IContainer) EditManager(Employee e, IEmployee manager, IContainer container) =>
            DisplayAndSave(e with { ManagerID = manager.BusinessEntityID }, container);

        [PageSize(20)]
        public static IQueryable<Employee> AutoCompleteManager(
             this Employee e, [Range(2, 0)] string name, IContainer container) =>
             Employee_MenuFunctions.FindEmployeeByName(null, name, container);


        //public static  IList<string> ChoicesGender(Employee e)
        //{
        //    return new[] { "M", "F" };
        //}

        //public static IList<string> ChoicesMaritalStatus(Employee e)
        //{
        //    return new[] { "S", "M" };
        //}

        public static (Employee, IContainer) CreateNewEmployeeFromContact(this Person contactDetails, IContainer container) => Employee_MenuFunctions.CreateNewEmployeeFromContact(contactDetails, container);
    }
}