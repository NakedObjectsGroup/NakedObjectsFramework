// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class CountryRegionCurrency : AWDomainObject {
        public virtual string CountryRegionCode { get; set; }

        public virtual string CurrencyCode { get; set; }

        public virtual CountryRegion CountryRegion { get; set; }

        public virtual Currency Currency { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}