using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductListPriceHistory
    {
    
        #region Primitive Properties
        #region ProductID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductID {get; set;}

        #endregion

        #region StartDate (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(120), Optionally, Mask("d")]
        public virtual Nullable<DateTime>  EndDate {get; set;}

        #endregion

        #region ListPrice (Decimal)
    [MemberOrder(130)]
        public virtual decimal  ListPrice {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(150)]
    	public virtual Product Product {get; set;}

        #endregion


        #endregion

    }
}
