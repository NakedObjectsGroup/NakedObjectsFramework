using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class EmployeeAddress
    {
    
        #region Primitive Properties
        #region EmployeeID (Int32)
    [MemberOrder(100)]
        public virtual int  EmployeeID {get; set;}

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
        #region Employee (Employee)
    		
    [MemberOrder(140)]
    	public virtual Employee Employee {get; set;}

        #endregion

        #region Address (Address)
    		
    [MemberOrder(150)]
    	public virtual Address Address {get; set;}

        #endregion


        #endregion

    }
}
