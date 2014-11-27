using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Vendor
    {
    
        #region Primitive Properties
        #region VendorID (Int32)
    [MemberOrder(100)]
        public virtual int  VendorID {get; set;}

        #endregion

        #region AccountNumber (String)
    [MemberOrder(110), StringLength(15)]
        public virtual string  AccountNumber {get; set;}

        #endregion

        #region Name (String)
    [MemberOrder(120), StringLength(50)]
        public virtual string  Name {get; set;}

        #endregion

        #region CreditRating (Byte)
    [MemberOrder(130)]
        public virtual byte  CreditRating {get; set;}

        #endregion

        #region PreferredVendorStatus (Boolean)
    [MemberOrder(140)]
        public virtual bool  PreferredVendorStatus {get; set;}

        #endregion

        #region ActiveFlag (Boolean)
    [MemberOrder(150)]
        public virtual bool  ActiveFlag {get; set;}

        #endregion

        #region PurchasingWebServiceURL (String)
    [MemberOrder(160), Optionally, StringLength(1024)]
        public virtual string  PurchasingWebServiceURL {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(170), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region ProductVendors (Collection of ProductVendor)
    		
    	    private ICollection<ProductVendor> _productVendors = new List<ProductVendor>();
    		
    		[MemberOrder(180), Disabled]
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

        #region PurchaseOrderHeaders (Collection of PurchaseOrderHeader)
    		
    	    private ICollection<PurchaseOrderHeader> _purchaseOrderHeaders = new List<PurchaseOrderHeader>();
    		
    		[MemberOrder(190), Disabled]
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders
        {
            get
            {
                return _purchaseOrderHeaders;
            }
    		set
    		{
    		    _purchaseOrderHeaders = value;
    		}
        }

        #endregion

        #region VendorAddresses (Collection of VendorAddress)
    		
    	    private ICollection<VendorAddress> _vendorAddresses = new List<VendorAddress>();
    		
    		[MemberOrder(200), Disabled]
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

        #region VendorContacts (Collection of VendorContact)
    		
    	    private ICollection<VendorContact> _vendorContacts = new List<VendorContact>();
    		
    		[MemberOrder(210), Disabled]
        public virtual ICollection<VendorContact> VendorContacts
        {
            get
            {
                return _vendorContacts;
            }
    		set
    		{
    		    _vendorContacts = value;
    		}
        }

        #endregion


        #endregion

    }
}
