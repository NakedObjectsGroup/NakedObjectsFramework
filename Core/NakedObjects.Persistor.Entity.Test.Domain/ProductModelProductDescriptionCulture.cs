using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductModelProductDescriptionCulture
    {
    
        #region Primitive Properties
        #region ProductModelID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductModelID {get; set;}

        #endregion

        #region ProductDescriptionID (Int32)
    [MemberOrder(110)]
        public virtual int  ProductDescriptionID {get; set;}

        #endregion

        #region CultureID (String)
    [MemberOrder(120), StringLength(6)]
        public virtual string  CultureID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Culture (Culture)
    		
    [MemberOrder(140)]
    	public virtual Culture Culture {get; set;}

        #endregion

        #region ProductDescription (ProductDescription)
    		
    [MemberOrder(150)]
    	public virtual ProductDescription ProductDescription {get; set;}

        #endregion

        #region ProductModel (ProductModel)
    		
    [MemberOrder(160)]
    	public virtual ProductModel ProductModel {get; set;}

        #endregion


        #endregion

    }
}
