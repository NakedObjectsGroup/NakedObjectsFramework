// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;
using NakedObjects.Menu;

namespace AdventureWorksModel {
    [DisplayName("Customers")]
    public class CustomerRepository : AbstractFactoryAndRepository {
        #region Injected Services

        #region Injected: ContactRepository

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #endregion

        public static void Menu(ITypedMenu<CustomerRepository> menu) {
            menu.AddAction("FindCustomerByAccountNumber");
            menu.CreateSubMenuOfSameType("Stores")
                .AddAction("FindStoreByName")
                .AddAction("CreateNewStoreCustomer")
                .AddAction("RandomStore");
            menu.CreateSubMenuOfSameType("Individuals")
                .AddAction("FindIndividualCustomerByName")
                .AddAction("CreateNewIndividualCustomer")
                .AddAction("RandomIndividual");
            menu.AddAction("CustomerDashboard");
            menu.AddAction("ThrowDomainException");
        }

        #region FindCustomerByAccountNumber

        [FinderAction]
        [MemberOrder(10), QueryOnly]
        public Customer FindCustomerByAccountNumber([DefaultValue("AW")] string accountNumber) {
            IQueryable<Customer> query = from obj in Instances<Customer>()
                where obj.AccountNumber == accountNumber
                orderby obj.AccountNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        public string ValidateFindCustomerByAccountNumber(string accountNumber) {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(!accountNumber.StartsWith("AW"), "Account number must start with AW");
            return rb.Reason;
        }

        #endregion

        #region Stores Menu
        [FinderAction]
        [MemberOrder(20)]
        [PageSize(2)]
        [TableView(true, "StoreName", "SalesPerson")] //Table view == List View
        public IQueryable<Store> FindStoreByName(string name) {
            return from obj in Instances<Store>()
                where obj.Name.ToUpper().Contains(name.ToUpper())
                select obj;
        }

        [FinderAction]
        [MemberOrder(40)]
        public Store CreateNewStoreCustomer() {
            var store = NewTransientInstance<Store>();
            store.CustomerType = "S";
            return store;
        }

        [FinderAction]
        [MemberOrder(60), QueryOnly]
        public Store RandomStore() {
            return Random<Store>();
        }

        #endregion

        #region Individuals Menu

        [FinderAction]
        [MemberOrder(30)]
        [TableView(true)] //Table view == List View
        public IQueryable<Individual> FindIndividualCustomerByName([Optionally] string firstName, string lastName) {
            IQueryable<Contact> matchingContacts = ContactRepository.FindContactByName(firstName, lastName);

            return from indv in Instances<Individual>()
                from contact in matchingContacts
                where indv.Contact.ContactID == contact.ContactID
                orderby indv.Contact.LastName, indv.Contact.LastName
                select indv;
        }

        [FinderAction]
        [MemberOrder(50)]
        public Individual CreateNewIndividualCustomer(string firstName, string lastName, [DataType(DataType.Password)] string initialPassword) {
            var indv = NewTransientInstance<Individual>();
            indv.CustomerType = "I";
            var contact = NewTransientInstance<Contact>();
            contact.FirstName = firstName;
            contact.LastName = lastName;
            contact.EmailPromotion = 0;
            contact.NameStyle = false;
            contact.ChangePassword(null, initialPassword, null);
            indv.Contact = contact;
            Persist(ref indv);
            return indv;
        }

        [FinderAction]
        [MemberOrder(70), QueryOnly]
        public Individual RandomIndividual() {
            return Random<Individual>();
        }

        #endregion

        public void ThrowDomainException() {
            throw new DomainException("Foo");
        }

        [QueryOnly]
        public CustomerDashboard CustomerDashboard(string accountNumber) {
            Customer cust = FindCustomerByAccountNumber(accountNumber);
            var dash = Container.NewViewModel<CustomerDashboard>();
            dash.Root = cust;
            return dash;
        }
    }
}