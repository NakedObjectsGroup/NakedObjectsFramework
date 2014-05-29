// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Bounded]
    [Immutable]
    public class UnitMeasure : AWDomainObject {

        private ICollection<BillOfMaterials> _BillOfMaterials = new List<BillOfMaterials>();

        private ICollection<Product> _Product = new List<Product>();
        private ICollection<Product> _Product1 = new List<Product>();
        private ICollection<ProductVendor> _ProductVendor = new List<ProductVendor>();

        [MemberOrder(10)]
        public virtual string UnitMeasureCode { get; set; }

        [Title]
        [MemberOrder(20)]
        public virtual string Name { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}