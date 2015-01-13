using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesPersonQuotaHistory
    {
    
        #region Primitive Properties
        #region SalesPersonID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesPersonID {get; set;}

        #endregion

        #region QuotaDate (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual DateTime  QuotaDate {get; set;}

        #endregion

        #region SalesQuota (Decimal)
    [MemberOrder(120)]
        public virtual decimal  SalesQuota {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(130)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region SalesPerson (SalesPerson)
    		
    [MemberOrder(150)]
    	public virtual SalesPerson SalesPerson {get; set;}

        #endregion


        #endregion

    }
}
