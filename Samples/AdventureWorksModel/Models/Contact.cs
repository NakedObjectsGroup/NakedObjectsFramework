using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class Contact
    {
        public Contact()
        {
            this.Employees = new List<Employee>();
            this.ContactCreditCards = new List<ContactCreditCard>();
            this.Individuals = new List<Individual>();
            this.SalesOrderHeaders = new List<SalesOrderHeader>();
            this.StoreContacts = new List<StoreContact>();
            this.VendorContacts = new List<VendorContact>();
        }

        public int ContactID { get; set; }
        public bool NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string EmailAddress { get; set; }
        public int EmailPromotion { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string AdditionalContactInfo { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<ContactCreditCard> ContactCreditCards { get; set; }
        public virtual ICollection<Individual> Individuals { get; set; }
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public virtual ICollection<StoreContact> StoreContacts { get; set; }
        public virtual ICollection<VendorContact> VendorContacts { get; set; }
    }
}
