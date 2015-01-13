using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Store
    {
    
        #region Primitive Properties
        #region CustomerID (Int32)
    [MemberOrder(100)]
        public virtual int  CustomerID {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(110), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region Demographics (String)
    [MemberOrder(120), Optionally]
        public virtual string  Demographics {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(130)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Customer (Customer)
    		
    [MemberOrder(150)]
    	public virtual Customer Customer {get; set;}

        #endregion

        #region SalesPerson (SalesPerson)
    		
    [MemberOrder(160)]
    	public virtual SalesPerson SalesPerson {get; set;}

        #endregion

        #region StoreContacts (Collection of StoreContact)
    		
    	    private ICollection<StoreContact> _storeContacts = new List<StoreContact>();
    		
    		[MemberOrder(170), Disabled]
        public virtual ICollection<StoreContact> StoreContacts
        {
            get
            {
                return _storeContacts;
            }
    		set
    		{
    		    _storeContacts = value;
    		}
        }

        #endregion


        #endregion

    }
}
