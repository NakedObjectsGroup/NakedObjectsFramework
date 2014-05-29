// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    //This class models an association table, and is never viewed directly by the user.

    [IconName("house.png")]
    public class EmployeeAddress : AWDomainObject, IAddressRole {

        #region Properties

        [Hidden]
        public virtual int EmployeeID { get; set; }

        [Hidden]
        public virtual int AddressID { get; set; }

        [Hidden]
        public virtual Employee Employee { get; set; }

        [Title, Disabled, MemberOrder(2)]
        public virtual Address Address { get; set; }

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

        #endregion

        #region IAddressRole Members

        public void Persisted() {
        }

        #endregion
    }
}