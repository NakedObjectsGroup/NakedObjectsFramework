using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class ContactType
    {
    
        #region Primitive Properties
        #region ContactTypeID (Int32)
    [MemberOrder(100)]
        public virtual int  ContactTypeID {get; set;}

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
        #region StoreContacts (Collection of StoreContact)
    		
    	    private ICollection<StoreContact> _storeContacts = new List<StoreContact>();
    		
    		[MemberOrder(130), Disabled]
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

        #region VendorContacts (Collection of VendorContact)
    		
    	    private ICollection<VendorContact> _vendorContacts = new List<VendorContact>();
    		
    		[MemberOrder(140), Disabled]
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
