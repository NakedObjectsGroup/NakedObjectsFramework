// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("gear.png")]
    public class ProductVendor : AWDomainObject {

        [Hidden]
        public virtual int ProductID { get; set; }

        [Hidden]
        public virtual int VendorID { get; set; }

        [MemberOrder(30)]
        public virtual int AverageLeadTime { get; set; }

        [Mask("C")]
        [MemberOrder(40)]
        public virtual decimal StandardPrice { get; set; }

        [Mask("C")]
        [MemberOrder(41)]
        public virtual decimal? LastReceiptCost { get; set; }

        [Mask("d")]
        [MemberOrder(50)]
        public virtual DateTime? LastReceiptDate { get; set; }

        [MemberOrder(60)]
        public virtual int MinOrderQty { get; set; }

        [MemberOrder(61)]
        public virtual int MaxOrderQty { get; set; }

        [MemberOrder(62)]
        public virtual int? OnOrderQty { get; set; }

        [Title]
        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        [MemberOrder(20)]
        public virtual UnitMeasure UnitMeasure { get; set; }

        [Hidden]
        public virtual Vendor Vendor { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}