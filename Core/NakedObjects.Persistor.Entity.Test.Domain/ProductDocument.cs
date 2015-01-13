using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductDocument
    {
    
        #region Primitive Properties
        #region ProductID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductID {get; set;}

        #endregion

        #region DocumentID (Int32)
    [MemberOrder(110)]
        public virtual int  DocumentID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Document (Document)
    		
    [MemberOrder(130)]
    	public virtual Document Document {get; set;}

        #endregion

        #region Product (Product)
    		
    [MemberOrder(140)]
    	public virtual Product Product {get; set;}

        #endregion


        #endregion

    }
}
