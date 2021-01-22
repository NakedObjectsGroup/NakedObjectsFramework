// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

using static AW.Helpers;

namespace AW.Functions {
    [Named("Employees")]
    public static class Employee_MenuFunctions {

        [PageSize(15)]
        public static IQueryable<Employee> AllEmployees(IContext context) => context.Instances<Employee>();

        [TableView(true, nameof(Employee.Current), nameof(Employee.JobTitle), nameof(Employee.Manager))]
        public static IQueryable<Employee> FindEmployeeByName(
            [Optionally] string firstName, string lastName, IContext context)
        {
            var employees = context.Instances<Employee>();
            var persons = context.Instances<Person>();
            return from emp in employees
            from p in persons
            where emp.PersonDetails.BusinessEntityID == p.BusinessEntityID &&
                  firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper()) &&
                  p.LastName.ToUpper().StartsWith(lastName.ToUpper())
            orderby p.LastName
            select emp;     
        }

        [TableView(true, nameof(Employee.Current), nameof(Employee.JobTitle), nameof(Employee.Manager))]
        public static IQueryable<Employee> FindEmployeeByName2(
    [Optionally] string firstName, string lastName, IContext context)
        {
            var employees = context.Instances<Employee>();
            var persons = context.Instances<Person>().
                    Where( p=> firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper()) &&
                         p.LastName.ToUpper().StartsWith(lastName.ToUpper()));
            return from emp in employees
                   from p in persons
                   where emp.PersonDetails.BusinessEntityID == p.BusinessEntityID                    
                   orderby p.LastName
                   select emp;
        }

        public static Employee FindEmployeeByNationalIDNumber(string nationalIDNumber, IContext context) 
            => context.Instances<Employee>().Where(e => e.NationalIDNumber == nationalIDNumber).FirstOrDefault();


        #region CreateNewEmployeeFromContact
        public static (Employee, IContext) CreateNewEmployeeFromContact(
            Person contactDetails, 
            [MaxLength(15)] string  nationalIDNumber,
            [MaxLength(256)] string LoginID,
            [MaxLength(50)] string jobTitle,
            DateTime dateOfBirth,
            string maritalStatus,
            string gender,
            [DefaultValue(0)] DateTime hireDate,
            bool salaried,
            short vacationHours,
            IContext context) {
            var e = new Employee {
                BusinessEntityID = contactDetails.BusinessEntityID,
                PersonDetails = contactDetails,
                NationalIDNumber = nationalIDNumber,
                JobTitle = jobTitle,
                DateOfBirth = dateOfBirth,
                MaritalStatus = maritalStatus,
                Gender = gender,
                HireDate = hireDate,
                Salaried = salaried,
                VacationHours = vacationHours,
                SickLeaveHours = 0,
                Current = true,
                ModifiedDate = context.Now(),
                rowguid = context.NewGuid(),
            };
            return context.SaveAndDisplay(e);
        }
        [PageSize(20)]
        public static IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact(
    [MinLength(2)] string name, IContext context) =>
            context.Instances<Person>().Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));

        public static IList<string> Choices5CreateNewEmployeeFromContact() =>
            Employee_Functions.MaritalStatuses;

        public static IList<string> Choices6CreateNewEmployeeFromContact() =>
            Employee_Functions.Genders;

        public static string Validate4CreateNewEmployeeFromContact(DateTime dob, IContext context) => 
            Employee_Functions.ValidateDateOfBirth(dob, context);
        #endregion


        [RenderEagerly]
        [TableView(true, "GroupName")]
        public static IQueryable<Department> ListAllDepartments(IContext context) => context.Instances<Department>();

        //TODO: 3 functions marked internal temporarily, as IPrincipal is being reflected over.
        internal static Employee CurrentUserAsEmployee(IContext context) =>
            context.Instances<Employee>().Where(x => x.LoginID == "adventure-works\\" + context.CurrentUser().Identity.Name).FirstOrDefault();

        internal static Employee Me(IContext context) =>CurrentUserAsEmployee(context);

        internal static (IQueryable<Employee>, IContext) MyDepartmentalColleagues(IContext context) {
            var me = CurrentUserAsEmployee(context);
            return me is null? ((IQueryable<Employee>) null, context.WithWarnUser("Current user unknown")) : 
                (me.ColleaguesInSameDept(context), context);
        }

        public static Employee RandomEmployee(IContext context) => Random<Employee>(context);

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

        public static IQueryable<Shift> Shifts(IContext context) => context.Instances<Shift>();

        public static IQueryable<JobCandidate> JobCandidates(IContext context) => context.Instances<JobCandidate>();

    }
}