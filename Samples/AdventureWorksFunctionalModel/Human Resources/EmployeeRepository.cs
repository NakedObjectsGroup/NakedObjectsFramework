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
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;


namespace AdventureWorksModel {
    [Named("Employees")]
    public static class EmployeeRepository {

        [TableView(true,
            nameof(Employee.Current),
            nameof(Employee.JobTitle),
            nameof(Employee.Manager))]
        [MultiLine]
        public static IQueryable<Employee> FindEmployeeByName(
            
            [Optionally] string firstName,
            string lastName,
            IQueryable<Person> persons,
            IQueryable<Employee> employees)
        {

            IQueryable<Person> matchingContacts = PersonRepository.FindContactByName( firstName, lastName, persons);

            IQueryable<Employee> query = from emp in employees
                                         from contact in matchingContacts
                                         where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                                         orderby emp.PersonDetails.LastName
                                         select emp;

            return query;
        }

        
        public static (Employee, Action<IUserAdvisory>) FindEmployeeByNationalIDNumber(
            
            string nationalIDNumber,
            IQueryable<Employee> employees)
        {
            IQueryable<Employee> query = from obj in employees
                                         where obj.NationalIDNumber == nationalIDNumber
                                         select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        //[ContributedAction("Employees")]
        public static (Employee, Employee) CreateNewEmployeeFromContact(
            
             Person contactDetails,
            IQueryable<Employee> employees)
        {
            var e = new Employee(
                contactDetails.BusinessEntityID,
                contactDetails);
            return DisplayAndPersist(e);
        }

        //[PageSize(20)]
        //public static IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact(
        //    [Range(2,0)] string name,
        //    IQueryable<Person> persons) {
        //    return persons.Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        //}

        //[FinderAction]
        [RenderEagerly]
        [TableView(true, "GroupName")]
        public static IQueryable<Department> ListAllDepartments(
            
            IQueryable<Department> depts)
        {
            return depts;
        }

        [Hidden]
        public static Employee CurrentUserAsEmployee(
            
            IQueryable<Employee> employees,
            IPrincipal principal
            )
        {
            return employees.Where(x => x.LoginID == "adventure-works\\" + principal.Identity.Name).FirstOrDefault();
        }

        
        public static Employee Me(
            
            IQueryable<Employee> employees,
            [Injected] IPrincipal principal)
        {
            return CurrentUserAsEmployee( employees, principal);
        }

        //public static (IQueryable<Employee>, string) MyDepartmentalColleagues(
        //    
        //    IQueryable<Employee> employees,
        //    [Injected] IPrincipal principal,
        //    IQueryable<EmployeeDepartmentHistory> edhs) {
        //    var me = CurrentUserAsEmployee(m, employees, principal);
        //    if (me == null) {
        //        return Display((IQueryable<Employee>) null, "Current user unknown");
        //    }
        //    else {
        //        return Display(EmployeeFunctions.ColleaguesInSameDept(me, edhs), null);
        //    }
        //}

        
        public static Employee RandomEmployee(
             
             IQueryable<Employee> employees,
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
            
            IQueryable<Shift> shifts)
        {
            return shifts;
        }
    }
}