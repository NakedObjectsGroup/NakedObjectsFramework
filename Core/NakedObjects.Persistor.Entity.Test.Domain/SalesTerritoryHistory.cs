using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public virtual System.DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(130), Optionally, Mask("d")]
        public virtual Nullable<System.DateTime>  EndDate {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(140)]
        public virtual System.Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(150), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

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
