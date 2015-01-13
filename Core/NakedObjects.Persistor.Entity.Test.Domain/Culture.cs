using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Culture
    {
    
        #region Primitive Properties
        #region CultureID (String)
    [MemberOrder(100), StringLength(6)]
        public virtual string  CultureID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductModelProductDescriptionCultures (Collection of ProductModelProductDescriptionCulture)
    		
    	    private ICollection<ProductModelProductDescriptionCulture> _productModelProductDescriptionCultures = new List<ProductModelProductDescriptionCulture>();
    		
    		[MemberOrder(130), Disabled]
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
