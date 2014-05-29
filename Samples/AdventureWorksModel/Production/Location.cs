// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("globe.png")]
    [Bounded]
    [Immutable]
    public class Location : AWDomainObject {

        private ICollection<ProductInventory> _ProductInventory = new List<ProductInventory>();
        private ICollection<WorkOrderRouting> _WorkOrderRouting = new List<WorkOrderRouting>();

        [Hidden]
        public virtual short LocationID { get; set; }

        [Title]
        public virtual string Name { get; set; }

        [Mask("C")]
        public virtual decimal CostRate { get; set; }

        [Mask("########.##")]
        public virtual decimal Availability { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        //public ICollection<ProductInventory> ProductInventory {
        //    get {

        //        return _ProductInventory;
        //    }
        //    set {
        //        _ProductInventory = value;

        //    }
        //}

        //public ICollection<WorkOrderRouting> WorkOrderRouting {
        //    get {

        //        return _WorkOrderRouting;
        //    }
        //    set {
        //        _WorkOrderRouting = value;

        //    }
        //}
    }
}