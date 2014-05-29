// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;
using System.ComponentModel;

namespace AdventureWorksModel {
    [IconName("house.png"), Description("Use Action menu to add")]
    public class CustomerAddress : AWDomainObject, IAddressRole {

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(AddressType).Append(":", Address);
            return t.ToString();
        }


        #region ID

        [Hidden]
        public virtual int CustomerID { get; set; }

        [Hidden]
        public virtual int AddressID { get; set; }

        #endregion

        #region AddressType

        [MemberOrder(1)]
        public virtual AddressType AddressType { get; set; }

        #endregion

        #region Address

        [MemberOrder(2)]
        [Disabled]
        public virtual Address Address { get; set; }

        #endregion

        #region Customer

        [Hidden]
        public virtual Customer Customer { get; set; }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion
    }
}