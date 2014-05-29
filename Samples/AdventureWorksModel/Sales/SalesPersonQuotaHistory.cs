// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class SalesPersonQuotaHistory : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(QuotaDate, "d", null).Append(SalesQuota, "C", null);
            return t.ToString();
        }
        #endregion

        [Hidden]
        public virtual int SalesPersonID { get; set; }

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime QuotaDate { get; set; }

        [MemberOrder(2)]
        [Mask("C")]
        public virtual decimal SalesQuota { get; set; }

        [MemberOrder(3)]
        public virtual SalesPerson SalesPerson { get; set; }

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