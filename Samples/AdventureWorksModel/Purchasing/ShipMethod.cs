// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Bounded]
    [Immutable]
    public class ShipMethod : AWDomainObject {

        [Hidden]
        public virtual int ShipMethodID { get; set; }

        [Title]
        public virtual string Name { get; set; }

        public virtual decimal ShipBase { get; set; }

        public virtual decimal ShipRate { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion
    }
}