// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cartons.png")]
    public class ProductInventory : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Quantity.ToString()).Append(" in", Location).Append(" -", Shelf);
            return t.ToString();
        }

        #endregion

        [Hidden]
        public virtual int ProductID { get; set; }

        [Hidden]
        public virtual short LocationID { get; set; }

        [MemberOrder(40)]
        public virtual string Shelf { get; set; }

        [MemberOrder(50)]
        public virtual byte Bin { get; set; }

        [MemberOrder(10)]
        public virtual short Quantity { get; set; }

        [MemberOrder(30)]
        public virtual Location Location { get; set; }

        [MemberOrder(20)]
        public virtual Product Product { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}