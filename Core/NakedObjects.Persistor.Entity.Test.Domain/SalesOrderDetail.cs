using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class SalesOrderDetail
    {
    
        #region Primitive Properties
        #region SalesOrderID (Int32)
    [MemberOrder(100)]
        public virtual int  SalesOrderID {get; set;}

        #endregion

        #region SalesOrderDetailID (Int32)
    [MemberOrder(110)]
        public virtual int  SalesOrderDetailID {get; set;}

        #endregion

        #region CarrierTrackingNumber (String)
    [MemberOrder(120), Optionally, StringLength(25)]
        public virtual string  CarrierTrackingNumber {get; set;}

        #endregion

        #region OrderQty (Int16)
    [MemberOrder(130)]
        public virtual short  OrderQty {get; set;}

        #endregion

        #region UnitPrice (Decimal)
    [MemberOrder(140)]
        public virtual decimal  UnitPrice {get; set;}

        #endregion

        #region UnitPriceDiscount (Decimal)
    [MemberOrder(150)]
        public virtual decimal  UnitPriceDiscount {get; set;}

        #endregion

        #region LineTotal (Decimal)
    [MemberOrder(160)]
        public virtual decimal  LineTotal {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(170)]
        public virtual System.Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(180), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region SalesOrderHeader (SalesOrderHeader)
    		
    [MemberOrder(190)]
    	public virtual SalesOrderHeader SalesOrderHeader {get; set;}

        #endregion

        #region SpecialOfferProduct (SpecialOfferProduct)
    		
    [MemberOrder(200)]
    	public virtual SpecialOfferProduct SpecialOfferProduct {get; set;}

        #endregion


        #endregion

    }
}
