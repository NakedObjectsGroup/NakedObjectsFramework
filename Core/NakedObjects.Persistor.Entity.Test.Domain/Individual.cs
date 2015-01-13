using System;
using NakedObjects;

namespace AdventureWorksModel
{
    public partial class Individual
    {
    
        #region Primitive Properties
        #region CustomerID (Int32)
    [MemberOrder(100)]
        public virtual int  CustomerID {get; set;}

        #endregion

        #region Demographics (String)
    [MemberOrder(110), Optionally]
        public virtual string  Demographics {get; set;}

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

        #region Customer (Customer)
    		
    [MemberOrder(140)]
    	public virtual Customer Customer {get; set;}

        #endregion


        #endregion

    }
}
