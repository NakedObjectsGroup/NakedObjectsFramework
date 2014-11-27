using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Location
    {
    
        #region Primitive Properties
        #region LocationID (Int16)
    [MemberOrder(100)]
        public virtual short  LocationID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region CostRate (Decimal)
    [MemberOrder(120)]
        public virtual decimal  CostRate {get; set;}

        #endregion

        #region Availability (Decimal)
    [MemberOrder(130)]
        public virtual decimal  Availability {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductInventories (Collection of ProductInventory)
    		
    	    private ICollection<ProductInventory> _productInventories = new List<ProductInventory>();
    		
    		[MemberOrder(150), Disabled]
        public virtual ICollection<ProductInventory> ProductInventories
        {
            get
            {
                return _productInventories;
            }
    		set
    		{
    		    _productInventories = value;
    		}
        }

        #endregion

        #region WorkOrderRoutings (Collection of WorkOrderRouting)
    		
    	    private ICollection<WorkOrderRouting> _workOrderRoutings = new List<WorkOrderRouting>();
    		
    		[MemberOrder(160), Disabled]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings
        {
            get
            {
                return _workOrderRoutings;
            }
    		set
    		{
    		    _workOrderRoutings = value;
    		}
        }

        #endregion


        #endregion

    }
}
