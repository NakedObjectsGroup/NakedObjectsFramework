// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using AdventureWorksModel.Person;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cellphone.png")]
    public class VendorContact : AWDomainObject, IContactRole {
        #region Title

        public void Persisted() {
            Vendor.Contacts.Add(this);
        }

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Contact).Append(":", ContactType);
            return t.ToString();
        }

        #endregion

        [Hidden]
        public virtual int VendorID { get; set; }

        [Hidden]
        public virtual int ContactID { get; set; }

        [Disabled]
        public virtual Contact Contact { get; set; }

        [Hidden]
        public virtual Vendor Vendor { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region IContactRole Members

        public virtual ContactType ContactType { get; set; }

        #endregion
    }
}