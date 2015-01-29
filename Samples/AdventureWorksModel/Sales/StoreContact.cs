// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using AdventureWorksModel.Person;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cellphone.png")]
    public class StoreContact : AWDomainObject, IContactRole {
        #region Title & Icon

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Contact).Append(",", ContactType);
            return t.ToString();
        }
        #endregion

        #region ID

        [Hidden]
        public virtual int CustomerID { get; set; }

        [Hidden]
        public virtual int ContactID { get; set; }

        #endregion

        #region ContactType

        [MemberOrder(1)]
        public virtual ContactType ContactType { get; set; }

        #endregion

        #region Contact

        [Disabled, MemberOrder(2)]
        public virtual Contact Contact { get; set; }

        #endregion

        #region Store

        [Hidden]
        public virtual Store Store { get; set; }

        #endregion

        #region ModifiedDate & rowguid

        public override Guid rowguid { get; set; }

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}