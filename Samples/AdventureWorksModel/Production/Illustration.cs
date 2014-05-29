// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    public class Illustration : AWDomainObject {
        private ICollection<ProductModelIllustration> _ProductModelIllustration = new List<ProductModelIllustration>();

        public virtual int IllustrationID { get; set; }

        public virtual string Diagram { get; set; }

        public ICollection<ProductModelIllustration> ProductModelIllustration {
            get { return _ProductModelIllustration; }
            set { _ProductModelIllustration = value; }
        }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}