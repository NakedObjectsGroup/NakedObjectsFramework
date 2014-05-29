using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public virtual System.DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(120), Optionally, Mask("d")]
        public virtual Nullable<System.DateTime>  EndDate {get; set;}

        #endregion

        #region ListPrice (Decimal)
    [MemberOrder(130)]
        public virtual decimal  ListPrice {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

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
