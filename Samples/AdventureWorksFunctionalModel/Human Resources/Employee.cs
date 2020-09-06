// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using NakedFunctions;
using NakedFunctions;

namespace AdventureWorksModel
{

    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes
        public record Employee : IEmployee, IHasRowGuid, IHasModifiedDate
    {
        //TODO: add all properties
        public Employee(
            int businessEntityID, 
            Person personDetails)
        {
            BusinessEntityID = businessEntityID;
            PersonDetails = personDetails;
        }

        public Employee() { }

        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [MemberOrder(1)]
        public virtual Person PersonDetails { get; init; }

        [MemberOrder(10)]
        public virtual string NationalIDNumber { get; init; }

        [MemberOrder(12)]
        public virtual string JobTitle { get; init; }

        [MemberOrder(13)]
        [Mask("d")]
        public virtual DateTime? DateOfBirth { get; init; }

        [MemberOrder(14)]
        
        public virtual string MaritalStatus { get; init; }

        [MemberOrder(15)]
        
        public virtual string Gender { get; init; }

        [MemberOrder(16)]
        [Mask("d")]
        public virtual DateTime? HireDate { get; init; }

        [MemberOrder(17)]
        public virtual bool Salaried { get; init; }

        [MemberOrder(18)]
        public virtual short VacationHours { get; init; }

        [MemberOrder(19)]
        public virtual short SickLeaveHours { get; init; }

        [MemberOrder(20)]
        public virtual bool Current { get; init; }

        [Hidden]
        public virtual int? ManagerID { get; init; }

        
        [MemberOrder(30)]
        public virtual Employee Manager { get; init; }

        [MemberOrder(11)]
        public virtual string LoginID { get; init; }

        [Hidden]
        public virtual SalesPerson SalesPerson { get; init; }

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [TableView(true,
            nameof(EmployeeDepartmentHistory.StartDate),
            nameof(EmployeeDepartmentHistory.EndDate),
            nameof(EmployeeDepartmentHistory.Department),
            nameof(EmployeeDepartmentHistory.Shift))]
        public virtual ICollection<EmployeeDepartmentHistory> DepartmentHistory { get; init; }

        [TableView(true,
            nameof(EmployeePayHistory.RateChangeDate),
            nameof(EmployeePayHistory.Rate))]
        public virtual ICollection<EmployeePayHistory> PayHistory { get; init; }

        public override string ToString()
        {
            return $"{PersonDetails}";
        }
    }

    public static class EmployeeFunctions
    {

        #region Life Cycle Methods
        public static Employee Updating(this Employee x, [Injected] DateTime now) => x with { ModifiedDate = now };
        #endregion

        //public static bool HideLoginID(
        //    Employee e,
        //    IQueryable<Employee> employees,
        //    [Injected] IPrincipal principal)
        //{
        //    var userAsEmployee = EmployeeRepository.CurrentUserAsEmployee(null, employees, principal);
        //    return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        //}

        public static IQueryable<Employee> ColleaguesInSameDept(
            Employee e,
            IQueryable<EmployeeDepartmentHistory> edhs
        )
        {
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

        public static EmployeePayHistory CurrentEmployeePayHistory(Employee e)
        {
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
            IEmployee manager)
        {
            var e2 = e with {ManagerID =  manager.BusinessEntityID};
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
    }
}