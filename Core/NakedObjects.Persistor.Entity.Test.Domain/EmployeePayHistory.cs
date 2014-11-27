using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class EmployeePayHistory
    {
    
        #region Primitive Properties
        #region EmployeeID (Int32)
    [MemberOrder(100)]
        public virtual int  EmployeeID {get; set;}

        #endregion

        #region RateChangeDate (DateTime)
    [MemberOrder(110), Mask("d")]
        public virtual System.DateTime  RateChangeDate {get; set;}

        #endregion

        #region Rate (Decimal)
    [MemberOrder(120)]
        public virtual decimal  Rate {get; set;}

        #endregion

        #region PayFrequency (Byte)
    [MemberOrder(130)]
        public virtual byte  PayFrequency {get; set;}

        #endregion

        #region ModifiedDate (DateTime)
    [MemberOrder(140), Mask("d")]
        public virtual System.DateTime  ModifiedDate {get; set;}

        #endregion


        #endregion

        #region Navigation Properties
        #region Employee (Employee)
    		
    [MemberOrder(150)]
    	public virtual Employee Employee {get; set;}

        #endregion


        #endregion

    }
}
