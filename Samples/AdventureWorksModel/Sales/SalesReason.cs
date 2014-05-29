// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("speech.png")]
    [Bounded]
    [Immutable]
    public class SalesReason : AWDomainObject {

        private ICollection<SalesOrderHeaderSalesReason> _SalesOrderHeaderSalesReason = new List<SalesOrderHeaderSalesReason>();

        [Hidden]
        public virtual int SalesReasonID { get; set; }

        [Title]
        public virtual string Name { get; set; }

        public virtual string ReasonType { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

    }
}