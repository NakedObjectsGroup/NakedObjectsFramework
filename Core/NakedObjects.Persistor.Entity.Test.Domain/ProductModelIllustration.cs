using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductModelIllustration
    {
    
        #region Primitive Properties
        #region ProductModelID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductModelID {get; set;}

        #endregion

        #region IllustrationID (Int32)
    [MemberOrder(110)]
        public virtual int  IllustrationID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Illustration (Illustration)
    		
    [MemberOrder(130)]
    	public virtual Illustration Illustration {get; set;}

        #endregion

        #region ProductModel (ProductModel)
    		
    [MemberOrder(140)]
    	public virtual ProductModel ProductModel {get; set;}

        #endregion


        #endregion

    }
}
