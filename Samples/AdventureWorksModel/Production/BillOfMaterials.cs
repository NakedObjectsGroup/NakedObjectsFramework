// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class BillOfMaterials : AWDomainObject {
        [Hidden]
        public virtual int BillOfMaterialsID { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual short BOMLevel { get; set; }

        public virtual decimal PerAssemblyQty { get; set; }

        public virtual Product Product { get; set; }

        public virtual Product Product1 { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}