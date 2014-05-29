using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class UnitMeasure
    {
    
        #region Primitive Properties
        #region UnitMeasureCode (String)
    [MemberOrder(100), StringLength(3)]
        public virtual string  UnitMeasureCode {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region BillOfMaterials (Collection of BillOfMaterial)
    		
    	    private ICollection<BillOfMaterial> _billOfMaterials = new List<BillOfMaterial>();
    		
    		[MemberOrder(130), Disabled]
        public virtual ICollection<BillOfMaterial> BillOfMaterials
        {
            get
            {
                return _billOfMaterials;
            }
    		set
    		{
    		    _billOfMaterials = value;
    		}
        }

        #endregion

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

        #region Products1 (Collection of Product)
    		
    	    private ICollection<Product> _products1 = new List<Product>();
    		
    		[MemberOrder(150), Disabled]
        public virtual ICollection<Product> Products1
        {
            get
            {
                return _products1;
            }
    		set
    		{
    		    _products1 = value;
    		}
        }

        #endregion

        #region ProductVendors (Collection of ProductVendor)
    		
    	    private ICollection<ProductVendor> _productVendors = new List<ProductVendor>();
    		
    		[MemberOrder(160), Disabled]
        public virtual ICollection<ProductVendor> ProductVendors
        {
            get
            {
                return _productVendors;
            }
    		set
    		{
    		    _productVendors = value;
    		}
        }

        #endregion


        #endregion

    }
}
