using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductReview
    {
    
        #region Primitive Properties
        #region ProductReviewID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductReviewID {get; set;}

        #endregion

        #region ReviewerName (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  ReviewerName {get; set;}

        #endregion

        #region ReviewDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual System.DateTime  ReviewDate {get; set;}

        #endregion

        #region EmailAddress (String)
    [MemberOrder(130), StringLength(50)]
        public virtual string  EmailAddress {get; set;}

        #endregion

        #region Rating (Int32)
    [MemberOrder(140)]
        public virtual int  Rating {get; set;}

        #endregion

        #region Comments (String)
    [MemberOrder(150), Optionally, StringLength(3850)]
        public virtual string  Comments {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(160), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(170)]
    	public virtual Product Product {get; set;}

        #endregion


        #endregion

    }
}
