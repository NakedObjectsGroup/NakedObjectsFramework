// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("skyscraper.png")]
    public class Vendor : AWDomainObject {

        private ICollection<ProductVendor> _ProductVendor = new List<ProductVendor>();
        private ICollection<PurchaseOrderHeader> _PurchaseOrderHeader = new List<PurchaseOrderHeader>();
        private ICollection<VendorAddress> _VendorAddress = new List<VendorAddress>();
        private ICollection<VendorContact> _VendorContact = new List<VendorContact>();

        [Hidden]
        public virtual int VendorID { get; set; }

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

        [DisplayName("Product - Order Info")]
        [TableView(true)] //  Not obvious which of many possible fields should be shown here
        public virtual ICollection<ProductVendor> Products {
            get { return _ProductVendor; }
            set { _ProductVendor = value; }
        }

         [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true)]  // TableView == ListView
        public virtual ICollection<VendorAddress> Addresses {
            get { return _VendorAddress; }
            set { _VendorAddress = value; }
        }

         [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true)]  // TableView == ListView
        public virtual ICollection<VendorContact> Contacts {
            get { return _VendorContact; }
            set { _VendorContact = value; }
        }


        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        public Contact CreateNewContact()
        {
            var _Contact = Container.NewTransientInstance<Contact>();

            _Contact.Contactee = this;

            return _Contact;
        }
    }
}