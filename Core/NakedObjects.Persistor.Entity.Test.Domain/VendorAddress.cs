using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class VendorAddress
    {
    
        #region Primitive Properties
        #region VendorID (Int32)
    [MemberOrder(100)]
        public virtual int  VendorID {get; set;}

        #endregion

        #region AddressID (Int32)
    [MemberOrder(110)]
        public virtual int  AddressID {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(120), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Address (Address)
    		
    [MemberOrder(130)]
    	public virtual Address Address {get; set;}

        #endregion

        #region AddressType (AddressType)
    		
    [MemberOrder(140)]
    	public virtual AddressType AddressType {get; set;}

        #endregion

        #region Vendor (Vendor)
    		
    [MemberOrder(150)]
    	public virtual Vendor Vendor {get; set;}

        #endregion


        #endregion

    }
}
