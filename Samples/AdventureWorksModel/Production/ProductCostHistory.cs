// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class ProductCostHistory : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(StandardCost).Append(StartDate).Append("~");
            return t.ToString();
        }

        #endregion

        [Hidden]
        public virtual int ProductID { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual decimal StandardCost { get; set; }

        [Hidden]
        public virtual Product Product { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}