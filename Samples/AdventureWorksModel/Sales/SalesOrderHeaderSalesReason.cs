// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [DisplayName("Reason")]
    [Immutable(WhenTo.OncePersisted)]
    public class SalesOrderHeaderSalesReason : AWDomainObject {
        [Hidden]
        public virtual int SalesOrderID { get; set; }

        public virtual int SalesReasonID { get; set; }

        public virtual SalesOrderHeader SalesOrderHeader { get; set; }

        public virtual SalesReason SalesReason { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(SalesReason);
            return t.ToString();
        }
    }
}