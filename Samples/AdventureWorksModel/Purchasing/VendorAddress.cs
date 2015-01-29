// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("house.png")]
    public class VendorAddress : AWDomainObject, IAddressRole {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(AddressType).Append(":", Address);
            return t.ToString();
        }

        #endregion

        [Hidden]
        public virtual int VendorID { get; set; }

        [Hidden]
        public virtual int AddressID { get; set; }

        [Disabled]
        public virtual Address Address { get; set; }

        public virtual AddressType AddressType { get; set; }

        [Hidden]
        public virtual Vendor Vendor { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

    }
}