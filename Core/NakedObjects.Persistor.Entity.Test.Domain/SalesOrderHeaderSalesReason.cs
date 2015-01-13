using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesOrderHeaderSalesReason
    {
    
        #region Primitive Properties
        #region SalesOrderID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesOrderID {get; set;}

        #endregion

        #region SalesReasonID (Int32)
    [MemberOrder(110)]
        public virtual int  SalesReasonID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region SalesOrderHeader (SalesOrderHeader)
    		
    [MemberOrder(130)]
    	public virtual SalesOrderHeader SalesOrderHeader {get; set;}

        #endregion

        #region SalesReason (SalesReason)
    		
    [MemberOrder(140)]
    	public virtual SalesReason SalesReason {get; set;}

        #endregion


        #endregion

    }
}
