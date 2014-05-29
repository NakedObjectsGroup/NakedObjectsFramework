using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductCategory
    {
    
        #region Primitive Properties
        #region ProductCategoryID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductCategoryID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(120)]
        public virtual System.Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductSubcategories (Collection of ProductSubcategory)
    		
    	    private ICollection<ProductSubcategory> _productSubcategories = new List<ProductSubcategory>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<ProductSubcategory> ProductSubcategories
        {
            get
            {
                return _productSubcategories;
            }
    		set
    		{
    		    _productSubcategories = value;
    		}
        }

        #endregion


        #endregion

    }
}
