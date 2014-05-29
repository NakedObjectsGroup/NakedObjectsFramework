// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class TransactionHistory : AWDomainObject {
        public virtual int TransactionID { get; set; }

        public virtual int ReferenceOrderID { get; set; }

        public virtual int ReferenceOrderLineID { get; set; }

        public virtual DateTime TransactionDate { get; set; }

        public virtual string TransactionType { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal ActualCost { get; set; }

        public Product Product { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}