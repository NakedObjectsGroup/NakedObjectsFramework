// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class Contact {
        #region Primitive Properties

        #region ContactID (Int32)

        [MemberOrder(100)]
        public virtual int ContactID { get; set; }

        #endregion

        #region NameStyle (Boolean)

        [MemberOrder(110)]
        public virtual bool NameStyle { get; set; }

        #endregion

        #region Title (String)

        [MemberOrder(120), Optionally, StringLength(8)]
        public virtual string Title { get; set; }

        #endregion

        #region FirstName (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string FirstName { get; set; }

        #endregion

        #region MiddleName (String)

        [MemberOrder(140), Optionally, StringLength(50)]
        public virtual string MiddleName { get; set; }

        #endregion

        #region LastName (String)

        [MemberOrder(150), StringLength(50)]
        public virtual string LastName { get; set; }

        #endregion

        #region Suffix (String)

        [MemberOrder(160), Optionally, StringLength(10)]
        public virtual string Suffix { get; set; }

        #endregion

        #region EmailAddress (String)

        [MemberOrder(170), Optionally, StringLength(50)]
        public virtual string EmailAddress { get; set; }

        #endregion

        #region EmailPromotion (Int32)

        [MemberOrder(180)]
        public virtual int EmailPromotion { get; set; }

        #endregion

        #region Phone (String)

        [MemberOrder(190), Optionally, StringLength(25)]
        public virtual string Phone { get; set; }

        #endregion

        #region PasswordHash (String)

        [MemberOrder(200), StringLength(128)]
        public virtual string PasswordHash { get; set; }

        #endregion

        #region PasswordSalt (String)

        [MemberOrder(210), StringLength(10)]
        public virtual string PasswordSalt { get; set; }

        #endregion

        #region AdditionalContactInfo (String)

        [MemberOrder(220), Optionally]
        public virtual string AdditionalContactInfo { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(230)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(240), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Employees (Collection of Employee)

        private ICollection<Employee> _employees = new List<Employee>();

        [MemberOrder(250), Disabled]
        public virtual ICollection<Employee> Employees {
            get { return _employees; }
            set { _employees = value; }
        }

        #endregion

        #region ContactCreditCards (Collection of ContactCreditCard)

        private ICollection<ContactCreditCard> _contactCreditCards = new List<ContactCreditCard>();

        [MemberOrder(260), Disabled]
        public virtual ICollection<ContactCreditCard> ContactCreditCards {
            get { return _contactCreditCards; }
            set { _contactCreditCards = value; }
        }

        #endregion

        #region Individuals (Collection of Individual)

        private ICollection<Individual> _individuals = new List<Individual>();

        [MemberOrder(270), Disabled]
        public virtual ICollection<Individual> Individuals {
            get { return _individuals; }
            set { _individuals = value; }
        }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(280), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #region StoreContacts (Collection of StoreContact)

        private ICollection<StoreContact> _storeContacts = new List<StoreContact>();

        [MemberOrder(290), Disabled]
        public virtual ICollection<StoreContact> StoreContacts {
            get { return _storeContacts; }
            set { _storeContacts = value; }
        }

        #endregion

        #region VendorContacts (Collection of VendorContact)

        private ICollection<VendorContact> _vendorContacts = new List<VendorContact>();

        [MemberOrder(300), Disabled]
        public virtual ICollection<VendorContact> VendorContacts {
            get { return _vendorContacts; }
            set { _vendorContacts = value; }
        }

        #endregion

        #endregion
    }
}