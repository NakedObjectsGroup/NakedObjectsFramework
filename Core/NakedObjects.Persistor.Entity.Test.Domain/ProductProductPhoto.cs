using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductProductPhoto
    {
    
        #region Primitive Properties
        #region ProductID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductID {get; set;}

        #endregion

        #region ProductPhotoID (Int32)
    [MemberOrder(110)]
        public virtual int  ProductPhotoID {get; set;}

        #endregion

        #region Primary (Boolean)
    [MemberOrder(120)]
        public virtual bool  Primary {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(140)]
    	public virtual Product Product {get; set;}

        #endregion

        #region ProductPhoto (ProductPhoto)
    		
    [MemberOrder(150)]
    	public virtual ProductPhoto ProductPhoto {get; set;}

        #endregion


        #endregion

    }
}
