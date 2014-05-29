// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class ProductModelIllustration : AWDomainObject {
        [Hidden]
        public virtual int ProductModelID { get; set; }

        [Hidden]
        public virtual int IllustrationID { get; set; }

        public virtual Illustration Illustration { get; set; }

        public virtual ProductModel ProductModel { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}