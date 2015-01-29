// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class SpecialOfferProduct : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(" ");
            return t.ToString();
        }

        #endregion

        //private ICollection<SalesOrderDetail> _SalesOrderDetail = new List<SalesOrderDetail>();

        [Hidden]
        public virtual int SpecialOfferID { get; set; }

        [Hidden]
        public virtual int ProductID { get; set; }

        [MemberOrder(1)]
        public virtual SpecialOffer SpecialOffer { get; set; }

        [MemberOrder(2)]
        public virtual Product Product { get; set; }

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