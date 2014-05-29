using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductSubcategory
    {
    
        #region Primitive Properties
        #region ProductSubcategoryID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductSubcategoryID {get; set;}

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
        #region Products (Collection of Product)
    		
    	    private ICollection<Product> _products = new List<Product>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<Product> Products
        {
            get
            {
                return _products;
            }
    		set
    		{
    		    _products = value;
    		}
        }

        #endregion

        #region ProductCategory (ProductCategory)
    		
    [MemberOrder(150)]
    	public virtual ProductCategory ProductCategory {get; set;}

        #endregion


        #endregion

    }
}
