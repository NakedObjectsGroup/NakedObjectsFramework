// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;
using static NakedFunctions.Result;

namespace AdventureWorksModel {
    [DisplayName("Employees")]
    public static class EmployeeRepository {

        [TableView(true,
            nameof(Employee.Current),
            nameof(Employee.JobTitle),
            nameof(Employee.Manager))]
        [MultiLine]
        public static IQueryable<Employee> FindEmployeeByName(
            
            [Optionally] string firstName,
            string lastName,
            [Injected] IQueryable<Person> persons,
            [Injected] IQueryable<Employee> employees)
        {

            IQueryable<Person> matchingContacts = PersonRepository.FindContactByName( firstName, lastName, persons);

            IQueryable<Employee> query = from emp in employees
                                         from contact in matchingContacts
                                         where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                                         orderby emp.PersonDetails.LastName
                                         select emp;

            return query;
        }

        
        public static (Employee, string) FindEmployeeByNationalIDNumber(
            
            string nationalIDNumber,
            [Injected] IQueryable<Employee> employees)
        {
            IQueryable<Employee> query = from obj in employees
                                         where obj.NationalIDNumber == nationalIDNumber
                                         select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        public static (Employee, Employee) CreateNewEmployeeFromContact(
            
            [ContributedAction("Employees")] Person contactDetails,
            [Injected] IQueryable<Employee> employees)
        {
            var e = new Employee(
                contactDetails.BusinessEntityID,
                contactDetails);
            return Result.DisplayAndPersist(e);
        }

        //[PageSize(20)]
        //public static IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact(
        //    [MinLength(2)] string name,
        //    [Injected] IQueryable<Person> persons) {
        //    return persons.Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        //}

        [FinderAction]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "GroupName")]
        public static IQueryable<Department> ListAllDepartments(
            
            [Injected] IQueryable<Department> depts)
        {
            return depts;
        }

        [NakedObjectsIgnore]
        public static Employee CurrentUserAsEmployee(
            
            IQueryable<Employee> employees,
            IPrincipal principal
            )
        {
            return employees.Where(x => x.LoginID == "adventure-works\\" + principal.Identity.Name).FirstOrDefault();
        }

        
        public static Employee Me(
            
            [Injected] IQueryable<Employee> employees,
            [Injected] IPrincipal principal)
        {
            return CurrentUserAsEmployee( employees, principal);
        }

        //public static (IQueryable<Employee>, string) MyDepartmentalColleagues(
        //    
        //    [Injected] IQueryable<Employee> employees,
        //    [Injected] IPrincipal principal,
        //    [Injected] IQueryable<EmployeeDepartmentHistory> edhs) {
        //    var me = CurrentUserAsEmployee(m, employees, principal);
        //    if (me == null) {
        //        return Display((IQueryable<Employee>) null, "Current user unknown");
        //    }
        //    else {
        //        return Display(EmployeeFunctions.ColleaguesInSameDept(me, edhs), null);
        //    }
        //}

        
        public static Employee RandomEmployee(
             
             [Injected] IQueryable<Employee> employees,
             [Injected] int random)
        {
            return Random(employees, random);
        }

        ////This method is to test use of nullable booleans
        //public static IQueryable<Employee> ListEmployees(
        //    
        //    bool? current, //mandatory
        //    [Optionally] bool? married,
        //    [DefaultValue(false)] bool? salaried,
        //    [Optionally] [DefaultValue(true)] bool? olderThan50,
        //    [Injected] IQueryable<Employee> employees
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
        //    [Injected] IQueryable<Employee> employees)
        //{
        //    return ListEmployees(m, current, married, salaried, olderThan50, employees);
        //}

        public static IQueryable<Shift> Shifts(
            
            [Injected] IQueryable<Shift> shifts)
        {
            return shifts;
        }
    }
}