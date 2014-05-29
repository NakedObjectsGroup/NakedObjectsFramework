// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Bounded]
    [Immutable]
    public class ScrapReason : AWDomainObject {

        [Hidden]
        public virtual short ScrapReasonID { get; set; }

        [Title]
        public virtual string Name { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}