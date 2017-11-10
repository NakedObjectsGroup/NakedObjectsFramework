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
using NakedFunctions;

namespace AdventureWorksModel
{
    [DisplayName("Employees")]
    public class EmployeeRepository : AWAbstractFactoryAndRepository
    {

        #region FindRecentHires

        //This method exists for test purposes only, to test that a hidden Finder Action does not
        //show up in the Find Menu
        [Hidden(WhenTo.Always)]      
        public static QueryResultList FindRecentHires()
        {
            throw new NotImplementedException(); //Deliberately not implemented
        }

        #endregion

        #region FindEmployeeByName

        
        [TableView(true,
            nameof(Employee.Current),
            nameof(Employee.JobTitle),
            nameof(Employee.Manager))]
        [MultiLine]
        public static QueryResultList FindEmployeeByName(
            [Optionally] string firstName,
            string lastName,
            IFunctionalContainer container)
        {
            return new QueryResultList(QueryEmployeesByName(firstName, lastName, container));
        }

        internal static IQueryable<Employee> QueryEmployeesByName(
            string firstName,
            string lastName,
            IFunctionalContainer container)
        {
            IQueryable<Person> matchingContacts = PersonRepository.QueryContactByName(firstName, lastName, container);

            return from emp in container.Instances<Employee>()
                   from contact in matchingContacts
                   where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                   orderby emp.PersonDetails.LastName
                   select emp;
        }

        #endregion

        #region FindEmployeeByNationalIDNumber

        
        [QueryOnly]
        public static QueryResultSingle FindEmployeeByNationalIDNumber(
            string nationalIDNumber,
            IFunctionalContainer container)
        {
            IQueryable<Employee> query = from obj in container.Instances<Employee>()
                                         where obj.NationalIDNumber == nationalIDNumber
                                         select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        //TODO: Add params for all mandatory fields in Employee & add validation methods
        public static PotentResultSingle CreateNewEmployeeFromContact(
            [ContributedAction("Employees")]  Person contactDetails,
            string nationalIDNumber,
            string jobTitle
            )
        {
            return new PotentResultSingle(new Employee(contactDetails.BusinessEntityID, contactDetails, nationalIDNumber, jobTitle), null);
        }

        [PageSize(20)]
        public static IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact(
            [MinLength(2)] string name, IFunctionalContainer container)
        {
            return container.Instances<Person>().Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "GroupName")]
        public static QueryResultList ListAllDepartments(
            IFunctionalContainer container)
        {
            return new QueryResultList(container.Instances<Department>());
        }

        internal static Employee CurrentUserAsEmployee(
            IFunctionalContainer container)
        {
            return container.Instances<Employee>().Where(obj => obj.LoginID == "adventure-works\\" + Principal.Identity.Name).FirstOrDefault();
        }

        
        [QueryOnly]
        public static QueryResultSingle Me(IFunctionalContainer container)
        {
            return new QueryResultSingle(CurrentUserAsEmployee(container));
        }

        public static QueryResultList MyDepartmentalColleagues(IFunctionalContainer container)
        {
            return CurrentUserAsEmployee(container) == null ?
                new QueryResultList(null, null, ("Current user unknown")) :
                new QueryResultList(CurrentUserAsEmployee(container).ColleaguesInSameDept(container));
        }
 
    #region RandomEmployee

    
    [QueryOnly]
    public static QueryResultSingle RandomEmployee(IFunctionalContainer container)
    {
        return Random<Employee>(container);
    }

    #endregion

    #region

    //This method is to test use of nullable booleans
    public static QueryResultList ListEmployees(bool? current, //mandatory
                                              [Optionally] bool? married,
                                              [DefaultValue(false)] bool? salaried,
                                              [Optionally] [DefaultValue(true)] bool? olderThan50,
                                              IFunctionalContainer container)
    {
        var emps = container.Instances<Employee>();
        emps = emps.Where(e => e.Current == current.Value);
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
        return new QueryResultList(emps);
    }

    //This method is to test use of non-nullable booleans
    public static QueryResultList ListEmployees2(bool current,
                                               [Optionally] bool married,
                                               [DefaultValue(false)] bool salaried,
                                               [DefaultValue(true)] bool olderThan50,
                                               IFunctionalContainer container)
    {
        return ListEmployees(current, married, salaried, olderThan50, container);
    }


    #endregion

    public static QueryResultList Shifts(IFunctionalContainer container)
    {
        return new QueryResultList(container.Instances<Shift>());
    }

    public static QueryResultSingle CreateStaffSummary(IFunctionalContainer container)
    {
        var emps = container.Instances<Employee>();
        var sum = new StaffSummary();
        sum.TotalStaff = emps.Count();
        sum.Male = emps.Count(e => e.Gender == "M");
        sum.Female = emps.Count(e => e.Gender == "F");
        return new QueryResultSingle(sum);
    }
}
}