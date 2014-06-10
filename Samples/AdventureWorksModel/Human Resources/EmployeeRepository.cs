// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Employees")]
    public class EmployeeRepository : AbstractFactoryAndRepository {
        #region Injected Services

        #region Injected: ContactRepository

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #endregion

        #region FindEmployeeByName

        [TableView(true, "Current", "Title", "Manager")]
        public IQueryable<Employee> FindEmployeeByName([Optionally] string firstName, string lastName) {
            IQueryable<Contact> matchingContacts = ContactRepository.FindContactByName(firstName, lastName);

            IQueryable<Employee> query = from emp in Instances<Employee>()
                from contact in matchingContacts
                where emp.ContactDetails.ContactID == contact.ContactID
                orderby emp.ContactDetails.LastName
                select emp;

            return query;
        }

        #endregion

        #region FindEmployeeByNationalIDNumber

        [QueryOnly]
        public Employee FindEmployeeByNationalIDNumber(string nationalIDNumber) {
            IQueryable<Employee> query = from obj in Instances<Employee>()
                where obj.NationalIDNumber == nationalIDNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        public Employee CreateNewEmployeeFromContact(Contact contactDetails) {
            var _Employee = NewTransientInstance<Employee>();
            _Employee.ContactDetails = contactDetails;
            return _Employee;
        }

        [PageSize(20)]
        public IQueryable<Contact> AutoComplete0CreateNewEmployeeFromContact([MinLength(2)] string name) {
            return Container.Instances<Contact>().Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        }


        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "GroupName")]
        public IQueryable<Department> ListAllDepartments() {
            return Instances<Department>();
        }

        [Hidden]
        public virtual Employee CurrentUserAsEmployee() {
            IQueryable<Employee> query = from obj in Instances<Employee>()
                where obj.LoginID == "adventure-works\\" + Principal.Identity.Name
                select obj;

            return query.FirstOrDefault();
        }

        [QueryOnly]
        public Employee Me() {
            Employee currentUser = CurrentUserAsEmployee();
            if (currentUser == null) {
                WarnUser("No Employee for current user");
            }
            return currentUser;
        }

        #region RandomEmployee

        [QueryOnly]
        public Employee RandomEmployee() {
            return Random<Employee>();
        }

        #endregion
    }
}