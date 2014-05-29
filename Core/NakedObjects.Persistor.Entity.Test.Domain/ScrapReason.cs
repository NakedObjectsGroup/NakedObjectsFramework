using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ScrapReason
    {
    
        #region Primitive Properties
        #region ScrapReasonID (Int16)
    [MemberOrder(100)]
        public virtual short  ScrapReasonID {get; set;}

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
        #region WorkOrders (Collection of WorkOrder)
    		
    	    private ICollection<WorkOrder> _workOrders = new List<WorkOrder>();
    		
    		[MemberOrder(130), Disabled]
        public virtual ICollection<WorkOrder> WorkOrders
        {
            get
            {
                return _workOrders;
            }
    		set
    		{
    		    _workOrders = value;
    		}
        }

        #endregion


        #endregion

    }
}
