using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductModel
    {
    
        #region Primitive Properties
        #region ProductModelID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductModelID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region CatalogDescription (String)
    [MemberOrder(120), Optionally]
        public virtual string  CatalogDescription {get; set;}

        #endregion

        #region Instructions (String)
    [MemberOrder(130), Optionally]
        public virtual string  Instructions {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(140)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(150), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Products (Collection of Product)
    		
    	    private ICollection<Product> _products = new List<Product>();
    		
    		[MemberOrder(160), Disabled]
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

        #region ProductModelIllustrations (Collection of ProductModelIllustration)
    		
    	    private ICollection<ProductModelIllustration> _productModelIllustrations = new List<ProductModelIllustration>();
    		
    		[MemberOrder(170), Disabled]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustrations
        {
            get
            {
                return _productModelIllustrations;
            }
    		set
    		{
    		    _productModelIllustrations = value;
    		}
        }

        #endregion

        #region ProductModelProductDescriptionCultures (Collection of ProductModelProductDescriptionCulture)
    		
    	    private ICollection<ProductModelProductDescriptionCulture> _productModelProductDescriptionCultures = new List<ProductModelProductDescriptionCulture>();
    		
    		[MemberOrder(180), Disabled]
        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCultures
        {
            get
            {
                return _productModelProductDescriptionCultures;
            }
    		set
    		{
    		    _productModelProductDescriptionCultures = value;
    		}
        }

        #endregion


        #endregion

    }
}
