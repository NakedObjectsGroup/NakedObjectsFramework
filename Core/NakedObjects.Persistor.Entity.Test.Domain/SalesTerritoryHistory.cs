using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesTerritoryHistory
    {
    
        #region Primitive Properties
        #region SalesPersonID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesPersonID {get; set;}

        #endregion

        #region TerritoryID (Int32)
    [MemberOrder(110)]
        public virtual int  TerritoryID {get; set;}

        #endregion

        #region StartDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(130), Optionally, Mask("d")]
        public virtual Nullable<DateTime>  EndDate {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(140)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(150), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region SalesPerson (SalesPerson)
    		
    [MemberOrder(160)]
    	public virtual SalesPerson SalesPerson {get; set;}

        #endregion

        #region SalesTerritory (SalesTerritory)
    		
    [MemberOrder(170)]
    	public virtual SalesTerritory SalesTerritory {get; set;}

        #endregion


        #endregion

    }
}
