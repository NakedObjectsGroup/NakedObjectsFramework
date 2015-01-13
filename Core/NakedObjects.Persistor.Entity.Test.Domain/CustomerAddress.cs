using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class CustomerAddress
    {
    
        #region Primitive Properties
        #region CustomerID (Int32)
    [MemberOrder(100)]
        public virtual int  CustomerID {get; set;}

        #endregion

        #region AddressID (Int32)
    [MemberOrder(110)]
        public virtual int  AddressID {get; set;}

        #endregion

        #region rowguid (Guid)
    [MemberOrder(120)]
        public virtual Guid  rowguid {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(130), Mask("d")]
        public virtual DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Address (Address)
    		
    [MemberOrder(140)]
    	public virtual Address Address {get; set;}

        #endregion

        #region AddressType (AddressType)
    		
    [MemberOrder(150)]
    	public virtual AddressType AddressType {get; set;}

        #endregion

        #region Customer (Customer)
    		
    [MemberOrder(160)]
    	public virtual Customer Customer {get; set;}

        #endregion


        #endregion

    }
}
