// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cellphone.png")]
    public class ContactCreditCard : AWDomainObject {


        [Hidden]
        public virtual int ContactID { get; set; }

        [Hidden]
        public virtual int CreditCardID { get; set; }

        [Title]
        public virtual Contact Contact { get; set; }

        public virtual CreditCard CreditCard { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}