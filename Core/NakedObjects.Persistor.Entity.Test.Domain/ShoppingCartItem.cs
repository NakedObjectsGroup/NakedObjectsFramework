using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ShoppingCartItem
    {
    
        #region Primitive Properties
        #region ShoppingCartItemID (Int32)
    [MemberOrder(100)]
        public virtual int  ShoppingCartItemID {get; set;}

        #endregion

        #region ShoppingCartID (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  ShoppingCartID {get; set;}

        #endregion

        #region Quantity (Int32)
    [MemberOrder(120)]
        public virtual int  Quantity {get; set;}

        #endregion

        #region DateCreated (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  DateCreated {get; set;}

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
