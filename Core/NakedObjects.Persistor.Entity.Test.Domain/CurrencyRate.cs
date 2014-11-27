using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class CurrencyRate
    {
    
        #region Primitive Properties
        #region CurrencyRateID (Int32)
    [MemberOrder(100)]
        public virtual int  CurrencyRateID {get; set;}

        #endregion

        #region CurrencyRateDate (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual System.DateTime  CurrencyRateDate {get; set;}

        #endregion

        #region AverageRate (Decimal)
    [MemberOrder(120)]
        public virtual decimal  AverageRate {get; set;}

        #endregion

        #region EndOfDayRate (Decimal)
    [MemberOrder(130)]
        public virtual decimal  EndOfDayRate {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Currency (Currency)
    		
    [MemberOrder(150)]
    	public virtual Currency Currency {get; set;}

        #endregion

        #region Currency1 (Currency)
    		
    [MemberOrder(160)]
    	public virtual Currency Currency1 {get; set;}

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)
    		
    	    private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();
    		
    		[MemberOrder(170), Disabled]
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


        #endregion

    }
}
