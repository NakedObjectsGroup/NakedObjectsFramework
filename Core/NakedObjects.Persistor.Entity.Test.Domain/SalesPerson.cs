using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesPerson
    {
    
        #region Primitive Properties
        #region SalesPersonID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesPersonID {get; set;}

        #endregion

        #region SalesQuota (Decimal)
    [MemberOrder(110), Optionally]
        public virtual Nullable<decimal>  SalesQuota {get; set;}

        #endregion

        #region Bonus (Decimal)
    [MemberOrder(120)]
        public virtual decimal  Bonus {get; set;}

        #endregion

        #region CommissionPct (Decimal)
    [MemberOrder(130)]
        public virtual decimal  CommissionPct {get; set;}

        #endregion

        #region SalesYTD (Decimal)
    [MemberOrder(140)]
        public virtual decimal  SalesYTD {get; set;}

        #endregion

        #region SalesLastYear (Decimal)
    [MemberOrder(150)]
        public virtual decimal  SalesLastYear {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(160)]
        public virtual System.Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(170), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Employee (Employee)
    		
    [MemberOrder(180)]
    	public virtual Employee Employee {get; set;}

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)
    		
    	    private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();
    		
    		[MemberOrder(190), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders
        {
            get
            {
                return _salesOrderHeaders;
            }
    		set
    		{
    		    _salesOrderHeaders = value;
    		}
        }

        #endregion

        #region SalesTerritory (SalesTerritory)
    		
    [MemberOrder(200)]
    	public virtual SalesTerritory SalesTerritory {get; set;}

        #endregion

        #region SalesPersonQuotaHistories (Collection of SalesPersonQuotaHistory)
    		
    	    private ICollection<SalesPersonQuotaHistory> _salesPersonQuotaHistories = new List<SalesPersonQuotaHistory>();
    		
    		[MemberOrder(210), Disabled]
        public virtual ICollection<SalesPersonQuotaHistory> SalesPersonQuotaHistories
        {
            get
            {
                return _salesPersonQuotaHistories;
            }
    		set
    		{
    		    _salesPersonQuotaHistories = value;
    		}
        }

        #endregion

        #region SalesTerritoryHistories (Collection of SalesTerritoryHistory)
    		
    	    private ICollection<SalesTerritoryHistory> _salesTerritoryHistories = new List<SalesTerritoryHistory>();
    		
    		[MemberOrder(220), Disabled]
        public virtual ICollection<SalesTerritoryHistory> SalesTerritoryHistories
        {
            get
            {
                return _salesTerritoryHistories;
            }
    		set
    		{
    		    _salesTerritoryHistories = value;
    		}
        }

        #endregion

        #region Stores (Collection of Store)
    		
    	    private ICollection<Store> _stores = new List<Store>();
    		
    		[MemberOrder(230), Disabled]
        public virtual ICollection<Store> Stores
        {
            get
            {
                return _stores;
            }
    		set
    		{
    		    _stores = value;
    		}
        }

        #endregion


        #endregion

    }
}
