using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class VendorContact
    {
    
        #region Primitive Properties
        #region VendorID (Int32)
    [MemberOrder(100)]
        public virtual int  VendorID {get; set;}

        #endregion

        #region ContactID (Int32)
    [MemberOrder(110)]
        public virtual int  ContactID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Contact (Contact)
    		
    [MemberOrder(130)]
    	public virtual Contact Contact {get; set;}

        #endregion

        #region ContactType (ContactType)
    		
    [MemberOrder(140)]
    	public virtual ContactType ContactType {get; set;}

        #endregion

        #region Vendor (Vendor)
    		
    [MemberOrder(150)]
    	public virtual Vendor Vendor {get; set;}

        #endregion


        #endregion

    }
}
