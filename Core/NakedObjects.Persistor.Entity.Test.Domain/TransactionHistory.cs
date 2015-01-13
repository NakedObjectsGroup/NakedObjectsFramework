using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class TransactionHistory
    {
    
        #region Primitive Properties
        #region TransactionID (Int32)
    [MemberOrder(100)]
        public virtual int  TransactionID {get; set;}

        #endregion

        #region ReferenceOrderID (Int32)
    [MemberOrder(110)]
        public virtual int  ReferenceOrderID {get; set;}

        #endregion

        #region ReferenceOrderLineID (Int32)
    [MemberOrder(120)]
        public virtual int  ReferenceOrderLineID {get; set;}

        #endregion

        #region TransactionDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  TransactionDate {get; set;}

        #endregion

        #region TransactionType (String)
    [MemberOrder(140), StringLength(1)]
        public virtual string  TransactionType {get; set;}

        #endregion

        #region Quantity (Int32)
    [MemberOrder(150)]
        public virtual int  Quantity {get; set;}

        #endregion

        #region ActualCost (Decimal)
    [MemberOrder(160)]
        public virtual decimal  ActualCost {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(170), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(180)]
    	public virtual Product Product {get; set;}

        #endregion


        #endregion

    }
}
