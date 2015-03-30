using System;
using System.Collections.Generic;

namespace AdventureWorksModel.Models
{
    public partial class vAdditionalContactInfo
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string TelephoneNumber { get; set; }
        public string TelephoneSpecialInstructions { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string CountryRegion { get; set; }
        public string HomeAddressSpecialInstructions { get; set; }
        public string EMailAddress { get; set; }
        public string EMailSpecialInstructions { get; set; }
        public string EMailTelephoneNumber { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}
