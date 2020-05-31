// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Person.Contact")]
    public class Contact {
        public int ContactID { get; set; }

        public bool NameStyle { get; set; }

        [StringLength(8)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string Suffix { get; set; }

        [StringLength(50)]
        public string EmailAddress { get; set; }

        public int EmailPromotion { get; set; }

        [StringLength(25)]
        public string Phone { get; set; }

        [Required]
        [StringLength(128)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(10)]
        public string PasswordSalt { get; set; }

        [Column(TypeName = "xml")]
        public string AdditionalContactInfo { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

        public virtual ICollection<ContactCreditCard> ContactCreditCards { get; set; } = new HashSet<ContactCreditCard>();

        public virtual ICollection<Individual> Individuals { get; set; } = new HashSet<Individual>();

        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; } = new HashSet<SalesOrderHeader>();

        public virtual ICollection<StoreContact> StoreContacts { get; set; } = new HashSet<StoreContact>();

        public virtual ICollection<VendorContact> VendorContacts { get; set; } = new HashSet<VendorContact>();
    }
}