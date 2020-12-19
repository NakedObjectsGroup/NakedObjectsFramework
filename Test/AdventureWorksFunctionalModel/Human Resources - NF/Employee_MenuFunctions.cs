// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel {
    [Named("Employees")]
    public static class Employee_MenuFunctions {
        [TableView(true,
                   nameof(Employee.Current),
                   nameof(Employee.JobTitle),
                   nameof(Employee.Manager))]
        [MultiLine]
        public static IQueryable<Employee> FindEmployeeByName(
            [Optionally] string firstName,
            string lastName,
            IQueryable<Person> persons,
            IQueryable<Employee> employees) {
            var matchingContacts = Person_MenuFunctions.FindContactByName(firstName, lastName, persons);

            var query = from emp in employees
                        from contact in matchingContacts
                        where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                        orderby emp.PersonDetails.LastName
                        select emp;

            return query;
        }

        public static (Employee, Action<IAlert>) FindEmployeeByNationalIDNumber(
            string nationalIDNumber,
            IQueryable<Employee> employees) {
            var query = from obj in employees
                        where obj.NationalIDNumber == nationalIDNumber
                        select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        public static (Employee, Employee) CreateNewEmployeeFromContact(Person contactDetails) {
            var e = new Employee {
                BusinessEntityID = contactDetails.BusinessEntityID,
                PersonDetails = contactDetails
            };
            return DisplayAndPersist(e);
        }

        [PageSize(20)]
        public static IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact(
            [Range(2, 0)] string name,
            IQueryable<Person> persons) {
            return persons.Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        [RenderEagerly]
        [TableView(true, "GroupName")]
        public static IQueryable<Department> ListAllDepartments(IQueryable<Department> depts) => depts;

        //TODO: 3 functions marked internal temporarily, as IPrincipal is being reflected over.
        internal static Employee CurrentUserAsEmployee(
            IQueryable<Employee> employees,
            IPrincipal principal
        ) {
            return employees.Where(x => x.LoginID == "adventure-works\\" + principal.Identity.Name).FirstOrDefault();
        }

        internal static Employee Me(
            IQueryable<Employee> employees,
            IPrincipal principal) =>
            CurrentUserAsEmployee(employees, principal);

        internal static (IQueryable<Employee>, Action<IAlert>) MyDepartmentalColleagues(
            IQueryable<Employee> employees,
            IPrincipal principal,
            IQueryable<EmployeeDepartmentHistory> edhs) {
            var me = CurrentUserAsEmployee(employees, principal);
            if (me == null) {
                var alert = WarnUser("Current user unknown");
                return ((IQueryable<Employee>) null, alert);
            }

            return (me.ColleaguesInSameDept(edhs), null);
        }

        public static Employee RandomEmployee(
            IQueryable<Employee> employees,
            [Injected] int random) =>
            Random(employees, random);

        ////This method is to test use of nullable booleans
        //public static IQueryable<Employee> ListEmployees(
        //    
        //    bool? current, //mandatory
        //    [Optionally] bool? married,
        //    [DefaultValue(false)] bool? salaried,
        //    [Optionally] [DefaultValue(true)] bool? olderThan50,
        //    IQueryable<Employee> employees
        //    ) {
        //    var emps = employees.Where(e => e.Current == current.Value);
        //    if (married != null) {
        //        string value = married.Value ? "M" : "S";
        //        emps = emps.Where(e => e.MaritalStatus == value);
        //    }
        //    emps = emps.Where(e => e.Salaried == salaried.Value);
        //    if (olderThan50 != null) {
        //        var date = DateTime.Today.AddYears(-50); //Not an exact calculation!
        //        if (olderThan50.Value) {
        //            emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth < date);
        //        }
        //        else {
        //            emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth > date);
        //        }
        //    }
        //    return emps;
        //}

        ////This method is to test use of non-nullable booleans
        //public static IQueryable<Employee> ListEmployees2(
        //    
        //    bool current,
        //    [Optionally] bool married,
        //    [DefaultValue(false)] bool salaried,
        //    [DefaultValue(true)] bool olderThan50,
        //    IQueryable<Employee> employees)
        //{
        //    return ListEmployees(m, current, married, salaried, olderThan50, employees);
        //}

        public static IQueryable<Shift> Shifts(
            IQueryable<Shift> shifts) =>
            shifts;
    }
}