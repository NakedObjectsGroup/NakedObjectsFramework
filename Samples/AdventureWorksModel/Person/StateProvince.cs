// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("lookup.png")]
    [Immutable]
    [Bounded]
    public class StateProvince : AWDomainObject {

        #region StateProvinceID

        [Hidden]
        public virtual int StateProvinceID { get; set; }

        #endregion

        #region StateProvinceCode

        public virtual string StateProvinceCode { get; set; }

        #endregion

        #region IsOnlyStateProvinceFlag

        public virtual bool IsOnlyStateProvinceFlag { get; set; }

        #endregion

        #region Name

        [Title]
        public virtual string Name { get; set; }

        #endregion

        #region CountryRegion

        public virtual CountryRegion CountryRegion { get; set; }

        #endregion

        #region SalesTerritory

        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region "DELETED RELATIONSHIPS"

        //public ICollection<Address> Address
        //{
        //    get
        //    {

        //        return _Address;
        //    }
        //    set
        //    {
        //        _Address = value;

        //    }
        //}

        //public ICollection<SalesTaxRate> SalesTaxRate
        //{
        //    get
        //    {

        //        return _SalesTaxRate;
        //    }
        //    set
        //    {
        //        _SalesTaxRate = value;

        //    }
        //}

        #endregion
    }
}