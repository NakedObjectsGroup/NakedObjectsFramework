// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("globe.png")]
    [Immutable]
    public class ProductModelProductDescriptionCulture : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(Culture);
            return t.ToString();
        }

        #endregion

        [Hidden]
        public virtual int ProductModelID { get; set; }

        [Hidden]
        public virtual int ProductDescriptionID { get; set; }

        [Hidden]
        public virtual string CultureID { get; set; }

        public virtual Culture Culture { get; set; }

        public virtual ProductDescription ProductDescription { get; set; }

        [Hidden]
        public virtual ProductModel ProductModel { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}