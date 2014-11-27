using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class AddressType
    {
    
        #region Primitive Properties
        #region AddressTypeID (Int32)
    [MemberOrder(100)]
        public virtual int  AddressTypeID {get; set;}

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
        #region CustomerAddresses (Collection of CustomerAddress)
    		
    	    private ICollection<CustomerAddress> _customerAddresses = new List<CustomerAddress>();
    		
    		[MemberOrder(140), Disabled]
        public virtual ICollection<CustomerAddress> CustomerAddresses
        {
            get
            {
                return _customerAddresses;
            }
    		set
    		{
    		    _customerAddresses = value;
    		}
        }

        #endregion

        #region VendorAddresses (Collection of VendorAddress)
    		
    	    private ICollection<VendorAddress> _vendorAddresses = new List<VendorAddress>();
    		
    		[MemberOrder(150), Disabled]
        public virtual ICollection<VendorAddress> VendorAddresses
        {
            get
            {
                return _vendorAddresses;
            }
    		set
    		{
    		    _vendorAddresses = value;
    		}
        }

        #endregion


        #endregion

    }
}
