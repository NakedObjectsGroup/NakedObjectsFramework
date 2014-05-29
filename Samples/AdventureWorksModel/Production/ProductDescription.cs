// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("information")]
    public class ProductDescription : AWDomainObject {

        private ICollection<ProductModelProductDescriptionCulture> _ProductModelProductDescriptionCulture = new List<ProductModelProductDescriptionCulture>();

        [Hidden]
        public virtual int ProductDescriptionID { get; set; }

        [Title]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(100)]
        [MemberOrder(2)]
        public virtual string Description { get; set; }

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