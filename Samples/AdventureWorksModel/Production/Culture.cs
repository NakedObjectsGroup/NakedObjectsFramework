// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [Bounded]
    [IconName("lookup.png")]
    [Immutable]
    public class Culture : AWDomainObject {

        [Hidden]
        public virtual string CultureID { get; set; }

        [Title]
        [MemberOrder(10)]
        public virtual string Name { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}