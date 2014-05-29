using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesReason
    {
    
        #region Primitive Properties
        #region SalesReasonID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesReasonID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region ReasonType (String)
    [MemberOrder(120), StringLength(50)]
        public virtual string  ReasonType {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region SalesOrderHeaderSalesReasons (Collection of SalesOrderHeaderSalesReason)
    		
    	    private ICollection<SalesOrderHeaderSalesReason> _salesOrderHeaderSalesReasons = new List<SalesOrderHeaderSalesReason>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons
        {
            get
            {
                return _salesOrderHeaderSalesReasons;
            }
    		set
    		{
    		    _salesOrderHeaderSalesReasons = value;
    		}
        }

        #endregion


        #endregion

    }
}
