using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class WorkOrder
    {
    
        #region Primitive Properties
        #region WorkOrderID (Int32)
    [MemberOrder(100)]
        public virtual int  WorkOrderID {get; set;}

        #endregion

        #region OrderQty (Int32)
    [MemberOrder(110)]
        public virtual int  OrderQty {get; set;}

        #endregion

        #region StockedQty (Int32)
    [MemberOrder(120)]
        public virtual int  StockedQty {get; set;}

        #endregion

        #region ScrappedQty (Int16)
    [MemberOrder(130)]
        public virtual short  ScrappedQty {get; set;}

        #endregion

        #region StartDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  StartDate {get; set;}

        #endregion

        #region EndDate (DateTime)
    [MemberOrder(150), Optionally, Mask("d")]
        public virtual Nullable<DateTime>  EndDate {get; set;}

        #endregion

        #region DueDate (DateTime)
    [MemberOrder(160), Mask("d")]
        public virtual DateTime  DueDate {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(170), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Product (Product)
    		
    [MemberOrder(180)]
    	public virtual Product Product {get; set;}

        #endregion

        #region ScrapReason (ScrapReason)
    		
    [MemberOrder(190)]
    	public virtual ScrapReason ScrapReason {get; set;}

        #endregion

        #region WorkOrderRoutings (Collection of WorkOrderRouting)
    		
    	    private ICollection<WorkOrderRouting> _workOrderRoutings = new List<WorkOrderRouting>();
    		
    		[MemberOrder(200), Disabled]
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
