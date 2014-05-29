// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class ProductProductPhoto : AWDomainObject {
        public virtual int ProductID { get; set; }

        public virtual int ProductPhotoID { get; set; }

        public virtual bool Primary { get; set; }

        public virtual Product Product { get; set; }

        public virtual ProductPhoto ProductPhoto { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}