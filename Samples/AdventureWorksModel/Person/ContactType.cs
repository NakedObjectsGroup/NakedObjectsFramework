// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Bounded]
    public class ContactType : AWDomainObject {

        #region ID

        [Hidden]
        public virtual int ContactTypeID { get; set; }

        #endregion

        #region Name

        [Title]
        public virtual string Name { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}