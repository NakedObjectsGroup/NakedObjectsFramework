// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
            var persons = context.Instances<Person>().
                    Where(p => (firstName == null || p.FirstName.ToUpper().StartsWith(firstName.ToUpper())) &&
                        p.LastName.ToUpper().StartsWith(lastName.ToUpper()));

            return from emp in employees
                   from p in persons
                   where emp.PersonDetails.BusinessEntityID == p.BusinessEntityID
                   orderby p.LastName
                   select emp;
        }

       public static Employee FindEmployeeByNationalIDNumber(string nationalIDNumber, IContext context) 
            => context.Instances<Employee>().Where(e => e.NationalIDNumber == nationalIDNumber).FirstOrDefault();

        public static StaffSummary GenerateStaffSummary(IContext context)
        {
            var staff = context.Instances<Employee>();
            int female = staff.Where(x => x.Gender == "F").Count();
            int male = staff.Where(x => x.Gender == "M").Count();
            return new() { Female = female, Male = male };
        }

        [RenderEagerly]
        [TableView(true, "GroupName")]
        public static IQueryable<Department> ListAllDepartments(IContext context) => context.Instances<Department>();


        internal static Employee CurrentUserAsEmployee(IContext context)
        {
            string login = context.CurrentUser().Identity.Name;
            return context.Instances<Employee>().Where(x => x.LoginID == login).FirstOrDefault();
        }
        public static Employee Me(IContext context) =>CurrentUserAsEmployee(context);


        public static Employee RandomEmployee(IContext context) => Random<Employee>(context);

        ////This method is to test use of nullable booleans
        public static IQueryable<Employee> ListEmployees(

            bool? current, //mandatory
            [Optionally] bool? married,
            [DefaultValue(false)] bool? salaried,
            [Optionally][DefaultValue(true)] bool? olderThan50,
            IQueryable<Employee> employees
            )
        {
            var emps = employees.Where(e => e.Current == current.Value);
            if (married != null)
            {
                string value = married.Value ? "M" : "S";
                emps = emps.Where(e => e.MaritalStatus == value);
            }
            emps = emps.Where(e => e.Salaried == salaried.Value);
            if (olderThan50 != null)
            {
                var date = DateTime.Today.AddYears(-50); //Not an exact calculation!
                if (olderThan50.Value)
                {
                    emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth < date);
                }
                else
                {
                    emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth > date);
                }
            }
            return emps;
        }

        public static IQueryable<Shift> Shifts(IContext context) => context.Instances<Shift>();

        public static IQueryable<JobCandidate> JobCandidates(IContext context) => context.Instances<JobCandidate>();
    }
}