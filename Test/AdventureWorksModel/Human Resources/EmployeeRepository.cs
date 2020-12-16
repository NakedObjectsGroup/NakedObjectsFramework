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
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Employees")]
    public class EmployeeRepository : AbstractFactoryAndRepository {
        #region Injected Services

        public PersonRepository ContactRepository { set; protected get; }

        #endregion

        #region FindRecentHires

        //This method exists for test purposes only, to test that a hidden Finder Action does not
        //show up in the Find Menu
        [Hidden(WhenTo.Always)]
        [FinderAction]
        public IQueryable<Employee> FindRecentHires() {
            throw new NotImplementedException(); //Deliberately not implemented
        }

        #endregion

        #region FindEmployeeByName

        [FinderAction]
        [TableView(true,
            nameof(Employee.Current),
            nameof(Employee.JobTitle),
            nameof(Employee.Manager))]
        [MultiLine]
        public IQueryable<Employee> FindEmployeeByName([Optionally] string firstName, string lastName) {
            IQueryable<Person> matchingContacts = ContactRepository.FindContactByName(firstName, lastName);

            IQueryable<Employee> query = from emp in Instances<Employee>()
                from contact in matchingContacts
                where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                orderby emp.PersonDetails.LastName
                select emp;

            return query;
        }

        #endregion

        #region FindEmployeeByNationalIDNumber

        [FinderAction]
        [QueryOnly]
        public Employee FindEmployeeByNationalIDNumber(string nationalIDNumber) {
            IQueryable<Employee> query = from obj in Container.Instances<Employee>()
                where obj.NationalIDNumber == nationalIDNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        public Employee CreateNewEmployeeFromContact([ContributedAction("Employees")] [FindMenu] Person contactDetails) {
            var _Employee = Container.NewTransientInstance<Employee>();
            _Employee.BusinessEntityID = contactDetails.BusinessEntityID;
            _Employee.PersonDetails = contactDetails;
            return _Employee;
        }

        [PageSize(20)]
        public IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact([MinLength(2)] string name) {
            return Container.Instances<Person>().Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        [FinderAction]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "GroupName")]
        public IQueryable<Department> ListAllDepartments() {
            return Container.Instances<Department>();
        }

        [NakedObjectsIgnore]
        public virtual Employee CurrentUserAsEmployee() {
            IQueryable<Employee> query = from obj in Container.Instances<Employee>()
                where obj.LoginID == "adventure-works\\" + Principal.Identity.Name
                select obj;

            return query.FirstOrDefault();
        }

        [FinderAction]
        [QueryOnly]
        public Employee Me() {
            return CurrentUserAsEmployee();
        }

        public IQueryable<Employee> MyDepartmentalColleagues() {
            var me = CurrentUserAsEmployee();
            if (me == null) {
                Container.WarnUser("Current user unknown");
                return null;
            }
            else {
                return me.ColleaguesInSameDept();
            }
        }

        #region RandomEmployee

        [FinderAction]
        [QueryOnly]
        public Employee RandomEmployee() {
            return Random<Employee>();
        }

        #endregion

        #region

        //This method is to test use of nullable booleans
        public IQueryable<Employee> ListEmployees(bool? current, //mandatory
                                                  [Optionally] bool? married,
                                                  [DefaultValue(false)] bool? salaried,
                                                  [Optionally] [DefaultValue(true)] bool? olderThan50
            ) {
            var emps = Container.Instances<Employee>();
            emps = emps.Where(e => e.Current == current.Value);
            if (married != null) {
                string value = married.Value ? "M" : "S";
                emps = emps.Where(e => e.MaritalStatus == value);
            }
            emps = emps.Where(e => e.Salaried == salaried.Value);
            if (olderThan50 != null) {
                var date = DateTime.Today.AddYears(-50); //Not an exact calculation!
                if (olderThan50.Value) {
                    emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth < date);
                }
                else {
                    emps = emps.Where(e => e.DateOfBirth != null && e.DateOfBirth > date);
                }
            }
            return emps;
        }

        //This method is to test use of non-nullable booleans
        public IQueryable<Employee> ListEmployees2(bool current,
                                                   [Optionally] bool married,
                                                   [DefaultValue(false)] bool salaried,
                                                   [DefaultValue(true)] bool olderThan50) {
            return ListEmployees(current, married, salaried, olderThan50);
        }


        #endregion

        public IQueryable<Shift> Shifts() {
            return Container.Instances<Shift>();
        }

        [QueryOnly]
        public StaffSummary CreateStaffSummary()
        {
            var emps = Container.Instances<Employee>();
            var sum = Container.NewTransientInstance<StaffSummary>();
            sum.TotalStaff = emps.Count();
            sum.Male = emps.Count(e => e.Gender == "M");
            sum.Female = emps.Count(e => e.Gender == "F");
            return sum;
        }
    }
}