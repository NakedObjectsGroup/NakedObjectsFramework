using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ProductDescription
    {
    
        #region Primitive Properties
        #region ProductDescriptionID (Int32)
    [MemberOrder(100)]
        public virtual int  ProductDescriptionID {get; set;}

        #endregion

        #region Description (String)
    [MemberOrder(110), StringLength(400)]
        public virtual string  Description {get; set;}

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
        #region ProductModelProductDescriptionCultures (Collection of ProductModelProductDescriptionCulture)
    		
    	    private ICollection<ProductModelProductDescriptionCulture> _productModelProductDescriptionCultures = new List<ProductModelProductDescriptionCulture>();
    		
    		[MemberOrder(140), Disabled]
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
