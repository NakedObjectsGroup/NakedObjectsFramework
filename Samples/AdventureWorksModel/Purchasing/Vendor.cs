// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("skyscraper.png")]
    public class Vendor : IBusinessEntity {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(10)]
        public virtual string AccountNumber { get; set; }

        [Title]
        [MemberOrder(20)]
        public virtual string Name { get; set; }

        [MemberOrder(30)]
        public virtual byte CreditRating { get; set; }

        [MemberOrder(40)]
        public virtual bool PreferredVendorStatus { get; set; }

        [MemberOrder(50)]
        public virtual bool ActiveFlag { get; set; }

        [Optionally]
        [MemberOrder(60)]
        public virtual string PurchasingWebServiceURL { get; set; }

        private ICollection<ProductVendor> _ProductVendor = new List<ProductVendor>();

        [DisplayName("Product - Order Info")]
        [TableView(true)] //  Not obvious which of many possible fields should be shown here
        public virtual ICollection<ProductVendor> Products {
            get { return _ProductVendor; }
            set { _ProductVendor = value; }
        }

        //private ICollection<VendorAddress> _VendorAddress = new List<VendorAddress>();

        //[Eagerly(EagerlyAttribute.Do.Rendering)]
        //[TableView(true)] // TableView == ListView
        //public virtual ICollection<VendorAddress> Addresses {
        //    get { return _VendorAddress; }
        //    set { _VendorAddress = value; }
        //}

        //private ICollection<VendorContact> _VendorContact = new List<VendorContact>();

        //[Eagerly(EagerlyAttribute.Do.Rendering)]
        //[TableView(true)] // TableView == ListView
        //public virtual ICollection<VendorContact> Contacts {
        //    get { return _VendorContact; }
        //    set { _VendorContact = value; }
        //}

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        public Person CreateNewContact() {
            var _Contact = Container.NewTransientInstance<Person>();

            _Contact.Contactee = this;

            return _Contact;
        }
    }
}