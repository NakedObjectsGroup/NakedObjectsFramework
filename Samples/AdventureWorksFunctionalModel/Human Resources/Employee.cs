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
using NakedObjects;

namespace AdventureWorksModel
{

    public interface IEmployee : IBusinessEntity { } //Interface is for testing purposes
    [IconName("person.png")]
    public class Employee : IEmployee, IHasRowGuid, IHasModifiedDate
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

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(1), Disabled]
        public virtual Person PersonDetails { get; set; }

        [MemberOrder(10)]
        public virtual string NationalIDNumber { get; set; }

        [MemberOrder(12)]
        public virtual string JobTitle { get; set; }

        [MemberOrder(13)]
        [Mask("d")]
        public virtual DateTime? DateOfBirth { get; set; }

        [MemberOrder(14)]
        [StringLength(1)]
        public virtual string MaritalStatus { get; set; }

        [MemberOrder(15)]
        [StringLength(1)]
        public virtual string Gender { get; set; }

        [MemberOrder(16)]
        [Mask("d")]
        public virtual DateTime? HireDate { get; set; }

        [MemberOrder(17)]
        public virtual bool Salaried { get; set; }

        [MemberOrder(18)]
        public virtual short VacationHours { get; set; }

        [MemberOrder(19)]
        public virtual short SickLeaveHours { get; set; }

        [MemberOrder(20)]
        public virtual bool Current { get; set; }

        [NakedObjectsIgnore]
        public virtual int? ManagerID { get; set; }

        [Optionally]
        [MemberOrder(30)]
        public virtual Employee Manager { get; set; }

        [MemberOrder(11)]
        public virtual string LoginID { get; set; }

        [NakedObjectsIgnore]
        public virtual SalesPerson SalesPerson { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        [TableView(true,
            nameof(EmployeeDepartmentHistory.StartDate),
            nameof(EmployeeDepartmentHistory.EndDate),
            nameof(EmployeeDepartmentHistory.Department),
            nameof(EmployeeDepartmentHistory.Shift))]
        public virtual ICollection<EmployeeDepartmentHistory> DepartmentHistory { get; set; }

        [TableView(true,
            nameof(EmployeePayHistory.RateChangeDate),
            nameof(EmployeePayHistory.Rate))]
        public virtual ICollection<EmployeePayHistory> PayHistory { get; set; }
    }

    public static class EmployeeFunctions
    {

        public static string Title(this Employee e)
        {
            return e.CreateTitle($"{PersonFunctions.Title(e.PersonDetails)}");
        }

        #region LifeCycle methods
        public static Employee Updating(Employee a, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(a, now);

        }

        #endregion

        //public static bool HideLoginID(
        //    Employee e,
        //    [Injected] IQueryable<Employee> employees,
        //    [Injected] IPrincipal principal)
        //{
        //    var userAsEmployee = EmployeeRepository.CurrentUserAsEmployee(null, employees, principal);
        //    return userAsEmployee != null ? userAsEmployee.LoginID != e.LoginID : true;
        //}

        public static IQueryable<Employee> ColleaguesInSameDept(
            Employee e,
            [Injected] IQueryable<EmployeeDepartmentHistory> edhs
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
        //    [Optionally] Shift shift,
        //    [Injected] DateTime now)
        //{
        //    var edh = CurrentAssignment(e).With(x => x.EndDate, now);
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
            return Result.DisplayAndPersist(e.With(x => x.ManagerID, manager.BusinessEntityID));
        }

        //[PageSize(20)]
        //public static IQueryable<Employee> AutoCompleteManager(
        //     Employee e,
        //    [MinLength(2)] string name,
        //    [Injected] IQueryable<Person> persons,
        //    [Injected] IQueryable<Employee> employees)
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